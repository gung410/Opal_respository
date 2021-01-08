using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.AccessControl.Entities;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Services;
using Microservice.Form.Application.SharedQueries;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.HangfireJob
{
    public class FormDailyCheckingJob : BaseHangfireJob, IFormDailyCheckingJob
    {
        private const int BatchSize = 10;

        private readonly GetFormsSharedQuery _formsSharedQuery;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly FormNotifyApplicationService _formNotificationService;

        private readonly ILogger<FormDailyCheckingJob> _logger;

        public FormDailyCheckingJob(
            GetFormsSharedQuery formsSharedQuery,
            ILogger<FormDailyCheckingJob> logger,
            IServiceProvider serviceProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<FormEntity> formRepository,
            IRepository<UserEntity> userRepository,
            FormNotifyApplicationService formNotificationService) : base(serviceProvider)
        {
            _uowManager = unitOfWorkManager;
            _thunderCqrs = thunderCqrs;
            _logger = logger;
            _formsSharedQuery = formsSharedQuery;
            _formParticipantRepository = formParticipantRepository;
            _formRepository = formRepository;
            _userRepository = userRepository;
            _formNotificationService = formNotificationService;
        }

        protected override async Task InternalHandleAsync()
        {
            await ArchiveFormDailyChecking();
            await QuizDueDateDailyChecking();
            await SurveyEndDateDailyChecking();
        }

        private async Task ArchiveFormDailyChecking()
        {
            bool continueToScan = true;

            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
                while (continueToScan)
                {
                    var formIds = await _formsSharedQuery.CanArchiveBecauseReachedArchiveDate(10);

                    if (formIds.Count == 0)
                    {
                        break;
                    }

                    foreach (var id in formIds)
                    {
                        await _thunderCqrs.SendCommand(new ArchiveFormCommand
                        {
                            FormId = id,
                            ArchiveBy = Guid.Empty
                        });
                    }

                    continueToScan = formIds.Count == BatchSize;
                }

                await uow.CompleteAsync();
                _logger.LogInformation("[Archive Form By Archive Date Scanner] Finished: {0}", Clock.Now.ToLongTimeString());
            }
        }

        private async Task QuizDueDateDailyChecking()
        {
            await DailyNotifyForm(FormType.Quiz);
        }

        private async Task SurveyEndDateDailyChecking()
        {
            await DailyNotifyForm(FormType.Survey);
        }

        private async Task DailyNotifyForm(FormType formType)
        {
            bool continueToScan = true;
            int skipItem = 0;
            var now = Clock.Now.Date;

            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
                while (continueToScan)
                {
                    var existedForm = _formRepository
                        .GetAll()
                        .Where(form => form.Type == formType &&
                                       form.IsStandalone == true &&
                                       form.Status == FormStatus.Published &&
                                       form.IsSendNotification == true);

                    if (formType == FormType.Quiz)
                    {
                        existedForm = existedForm
                            .Where(form => form.FormRemindDueDate.HasValue &&
                                           form.FormRemindDueDate.Value.Date == now.AddDays(form.RemindBeforeDays));
                    }

                    if (formType == FormType.Survey)
                    {
                        existedForm = existedForm
                           .Where(form => form.EndDate.HasValue &&
                                          form.EndDate.Value.Date == now.AddDays(form.RemindBeforeDays));
                    }

                    var participantInfo = await existedForm
                                                .Join(
                                                    _formParticipantRepository
                                                            .GetAll()
                                                            .Where(p => p.Status != FormParticipantStatus.Completed),
                                                    form => form.OriginalObjectId,
                                                    participant => participant.FormOriginalObjectId,
                                                    (form, participant) => new NotifyFormDueDateModel
                                                    {
                                                        FormID = form.Id,
                                                        FormName = form.Title,
                                                        UserId = participant.UserId,
                                                        ReminderBeforeDays = form.RemindBeforeDays
                                                    })
                                                .Join(
                                                    _userRepository.GetAll(),
                                                    formParticipant => formParticipant.UserId,
                                                    user => user.Id,
                                                    (formParticipant, user) => new NotifyFormDueDateModel
                                                    {
                                                        FormID = formParticipant.FormID,
                                                        FormName = formParticipant.FormName,
                                                        UserId = user.Id,
                                                        UserName = user.FullName(),
                                                        ReminderBeforeDays = formParticipant.ReminderBeforeDays
                                                    })
                                                .Skip(skipItem)
                                                .Take(BatchSize)
                                                .ToListAsync();

                    if (participantInfo.Count == 0)
                    {
                        break;
                    }

                    foreach (var item in participantInfo)
                    {
                        switch (formType)
                        {
                            case FormType.Quiz:
                                await _formNotificationService.PerformSendNotifyFormDueDate(item);
                                break;
                            case FormType.Survey:
                                await _formNotificationService.PerformSendNotifySurveyEndDate(item);
                                break;
                            default:
                                break;
                        }
                    }

                    skipItem += BatchSize;
                    continueToScan = participantInfo.Count == BatchSize;
                }

                await uow.CompleteAsync();
                _logger.LogInformation($"[{formType} Notify Daily Checking] finished: {Clock.Now.ToLongTimeString()}");
            }
        }
    }
}
