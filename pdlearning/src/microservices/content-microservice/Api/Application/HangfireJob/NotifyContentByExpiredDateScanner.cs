using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.HangfireJob
{
    public class NotifyContentByExpiredDateScanner : BaseHangfireJob, INotifyContentByExpiredDateScanner
    {
        private const int BatchSize = 10;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly ILogger<NotifyContentByExpiredDateScanner> _logger;
        private readonly IConfiguration _configuration;

        public NotifyContentByExpiredDateScanner(
            IServiceProvider serviceProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IRepository<DigitalContent> digitalContentRepository,
            ILogger<NotifyContentByExpiredDateScanner> logger,
            IRepository<UserEntity> userRepository,
            IConfiguration configuration) : base(serviceProvider)
        {
            _uowManager = unitOfWorkManager;
            _thunderCqrs = thunderCqrs;
            _digitalContentRepository = digitalContentRepository;
            _logger = logger;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        protected override async Task InternalHandleAsync()
        {
            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
                await SendNotifyContentExpiredEvent();
                await SendPreNotifyContentExpiredEvent(day: 3);
                await SendPreNotifyContentExpiredEvent(month: 3);
                await SendPreNotifyContentExpiredEvent(month: 6);
                await SendPreNotifyContentExpiredEvent(month: 9);
            }
        }

        private async Task<List<NotifyContentExpiredModel<PreNotifyContentExpiredPayload>>> GetListPreExpiredContents(int skipItem, int? day = null, int? month = null)
        {
            string subject = month.HasValue ? $"Expiry date for Digital content [ContentName] expires in {month.Value} months"
            : "Your Content is going to be expired";
            string template = month.HasValue ? "ContentGoingExpiredInMonthsSystemAlert" :
                "ContentGoingExpiredSystemAlert";
            string time = month.HasValue ? $"in {month.Value} months"
                : "on [ExpiredDate]";
            var now = DateTime.Now.Date.ToUniversalTime().Date;
            var query = _digitalContentRepository
                   .GetAll()
                   .WhereIf(day.HasValue, p => p.ExpiredDate.HasValue && p.ExpiredDate.Value.Date == now.AddDays(day.Value))
                   .WhereIf(month.HasValue, p => p.ExpiredDate.HasValue && p.ExpiredDate.Value.Date == now.AddMonths(month.Value))
                   .Join(
                           _userRepository.GetAll(),
                           content => content.OwnerId,
                           user => user.Id,
                           (content, user) => new NotifyContentExpiredModel<PreNotifyContentExpiredPayload>
                           {
                               Payload = new PreNotifyContentExpiredPayload
                               {
                                   ContentName = content.Title,
                                   RecipientName = user.FullName(),
                                   ContentDetailURL = WebAppLinkBuilder.GetDigitalContentDetailLink(_configuration, content.Id),
                                   ActionUrl = WebAppLinkBuilder.GetDigitalContentDetailLink(_configuration, content.Id),
                                   ActionName = subject.Replace("[ContentName]", content.Title),
                                   Time = time.Replace("[ExpiredDate]", content.ExpiredDate.Value.AddHours(8).ToString("dd/MM/yyyy")),
                                   Date = content.ExpiredDate.Value.AddHours(8).ToString("dd/MM/yyyy"),
                                   Subject = subject.Replace("[ContentName]", content.Title),
                                   Template = template
                               },
                               DigitalContentId = content.Id,
                               ContentCreatorId = content.OwnerId,
                               ExpiredDate = content.ExpiredDate.Value
                           })
                   .Skip(skipItem)
                   .Take(BatchSize);
            return await query.ToListAsync();
        }

        private async Task SendPreNotifyContentExpiredEvent(int? day = null, int? month = null)
        {
            bool continueToScan = true;
            int skipItem = 0;
            while (continueToScan)
            {
                List<NotifyContentExpiredModel<PreNotifyContentExpiredPayload>> models = await GetListPreExpiredContents(skipItem, day, month);
                var reminderByConditions = new ReminderByDto
                {
                    Type = ReminderByType.AbsoluteDateTimeUTC,

                    // Add 2 minutes to ensure the time is valid after the message was sent to Communication.
                    Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
                };

                foreach (var model in models)
                {
                    await _thunderCqrs.SendEvent(
                        new PreNotifyContentExpiredEvent(
                            payload: model.Payload,
                            model.ContentCreatorId,
                            model.DigitalContentId,
                            reminderByConditions));
                }

                continueToScan = models.Count == BatchSize;
                skipItem += BatchSize;
            }
        }

        private async Task SendNotifyContentExpiredEvent()
        {
            bool continueToScan = true;
            int skipItem = 0;
            while (continueToScan)
            {
                List<NotifyContentExpiredModel<NotifyContentExpiredPayload>> models = await GetListExpiredContents(skipItem);
                var reminderByConditions = new ReminderByDto
                {
                    Type = ReminderByType.AbsoluteDateTimeUTC,

                    // Add 2 minutes to ensure the time is valid after the message was sent to Communication.
                    Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
                };
                foreach (var model in models)
                {
                    await _thunderCqrs.SendEvent(new NotifyContentExpiredEvent(payload: model.Payload, model.ContentCreatorId, model.DigitalContentId, model.ExpiredDate.AddHours(8), reminderByConditions));
                }

                continueToScan = models.Count == BatchSize;
                skipItem += BatchSize;
            }
        }

        private async Task<List<NotifyContentExpiredModel<NotifyContentExpiredPayload>>> GetListExpiredContents(int skipItem)
        {
            var now = DateTime.Now.Date.ToUniversalTime().Date;
            return await _digitalContentRepository
                   .GetAll()
                   .Where(p => p.ExpiredDate.HasValue && p.ExpiredDate.Value.Date == now)
                   .Join(
                           _userRepository.GetAll(),
                           content => content.OwnerId,
                           user => user.Id,
                           (content, user) => new NotifyContentExpiredModel<NotifyContentExpiredPayload>
                           {
                               Payload = new NotifyContentExpiredPayload
                               {
                                   ContentName = content.Title,
                                   RecipientName = user.FullName(),
                                   ContentDetailURL = WebAppLinkBuilder.GetDigitalContentDetailLink(_configuration, content.Id),
                                   ActionUrl = WebAppLinkBuilder.GetDigitalContentDetailLink(_configuration, content.Id),
                                   ActionName = "Your Content is expired",
                               },
                               DigitalContentId = content.Id,
                               ContentCreatorId = content.OwnerId,
                               ExpiredDate = content.ExpiredDate.Value
                           })
                   .Skip(skipItem)
                   .Take(BatchSize)
                   .ToListAsync();
        }
    }
}
