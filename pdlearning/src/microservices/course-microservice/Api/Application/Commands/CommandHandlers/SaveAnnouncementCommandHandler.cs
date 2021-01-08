using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveAnnouncementCommandHandler : BaseCommandHandler<SaveAnnouncementCommand>
    {
        private readonly IReadOnlyRepository<Announcement> _readAnnouncementRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly AnnouncementCudLogic _announcementCudLogic;
        private readonly SendAnnouncementNotifyLearnerLogic _sendAnnouncementNotifyLearnerLogic;

        public SaveAnnouncementCommandHandler(
          IReadOnlyRepository<Announcement> readAnnouncementRepository,
          IReadOnlyRepository<CourseEntity> readCourseRepository,
          IReadOnlyRepository<ClassRun> readClassRunRepository,
          IReadOnlyRepository<Registration> readRegistrationRepository,
          AnnouncementCudLogic announcementCudLogic,
          IUnitOfWorkManager unitOfWorkManager,
          IUserContext userContext,
          IAccessControlContext<CourseUser> accessControlContext,
          SendAnnouncementNotifyLearnerLogic sendAnnouncementNotifyLearnerLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAnnouncementRepository = readAnnouncementRepository;
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _sendAnnouncementNotifyLearnerLogic = sendAnnouncementNotifyLearnerLogic;
            _announcementCudLogic = announcementCudLogic;
        }

        protected override async Task HandleAsync(SaveAnnouncementCommand command, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(command.CourseId);
            var classRun = await _readClassRunRepository.GetAsync(command.ClassrunId);

            EnsureValidPermission(Announcement.HasCudPermission(
                CurrentUserId,
                CurrentUserRoles,
                course,
                classRun,
                _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext)));

            if (command.IsCreate)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, course, cancellationToken);
            }
        }

        protected override Task<Validation<SaveAnnouncementCommand>> ValidateCommand(SaveAnnouncementCommand command)
        {
            var isValid = (command.IsSentToAllParticipants ||
                           (command.RegistrationIds != null && command.RegistrationIds.Any())) &&
                          !string.IsNullOrWhiteSpace(command.Title) &&
                          !string.IsNullOrWhiteSpace(command.Message);
            return Task.FromResult(Validation.ValidIf(command, isValid));
        }

        private async Task CreateNew(SaveAnnouncementCommand command, CancellationToken cancellationToken)
        {
            var announcement = new Announcement
            {
                Id = command.Id,
                CreatedBy = CurrentUserId,
                CreatedDate = Clock.Now
            };

            await SetAnnouncementValues(announcement, command, cancellationToken);

            await _announcementCudLogic.Insert(announcement, cancellationToken);

            await SendAnnouncementNotifyLearner(cancellationToken, announcement);
        }

        private async Task SendAnnouncementNotifyLearner(CancellationToken cancellationToken, Announcement announcement)
        {
            await _sendAnnouncementNotifyLearnerLogic.Execute(announcement, CurrentUserId, cancellationToken);
        }

        private async Task<List<Guid>> GetParticipantIds(SaveAnnouncementCommand command, CancellationToken cancellationToken)
        {
            var participantIds = await _readRegistrationRepository
                .GetAll()
                .WhereIf(!command.IsSentToAllParticipants, p => command.RegistrationIds.Contains(p.Id))
                .Where(p => p.CourseId == command.CourseId && p.ClassRunId == command.ClassrunId)
                .Where(Registration.IsParticipantExpr())
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);
            return participantIds;
        }

        private async Task Update(SaveAnnouncementCommand command, CourseEntity course, CancellationToken cancellationToken)
        {
            var announcement = await _readAnnouncementRepository.GetAsync(command.Id);

            EnsureBusinessLogicValid(announcement, p => p.ValidateCanModify(course));

            await SetAnnouncementValues(announcement, command, cancellationToken);
            announcement.ChangedDate = Clock.Now;
            announcement.ChangedBy = CurrentUserId;

            await _announcementCudLogic.Update(announcement, cancellationToken);

            await SendAnnouncementNotifyLearner(cancellationToken, announcement);
        }

        private async Task SetAnnouncementValues(Announcement announcement, SaveAnnouncementCommand command, CancellationToken cancellationToken)
        {
            var participantIds = await GetParticipantIds(command, cancellationToken);
            announcement.Title = command.Title;
            announcement.ScheduleDate = command.ScheduleDate;
            announcement.ClassrunId = command.ClassrunId;
            announcement.CourseId = command.CourseId;
            announcement.Message = command.Message;
            announcement.Participants = participantIds;
            announcement.Status = command.ScheduleDate == null
                    ? AnnouncementStatus.Sent
                    : AnnouncementStatus.Scheduled;

            if (announcement.Status == AnnouncementStatus.Sent)
            {
                announcement.SentDate = Clock.Now;
            }
        }
    }
}
