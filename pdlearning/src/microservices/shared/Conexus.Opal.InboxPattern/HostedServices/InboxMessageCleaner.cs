using System;
using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.InboxPattern.Common;
using Conexus.Opal.InboxPattern.Entities;
using Conexus.Opal.InboxPattern.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Conexus.Opal.InboxPattern.HostedServices
{
    public class InboxMessageCleaner : InboxMessageHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<InboxMessageCleaner> _logger;
        private readonly InboxOptions _inboxOptions;

        public InboxMessageCleaner(
            IHostApplicationLifetime lifetime,
            IServiceProvider provider,
            IOptions<InboxOptions> options,
            ILoggerFactory loggerFactory) : base(lifetime)
        {
            _provider = provider;
            _inboxOptions = options.Value;
            _logger = loggerFactory.CreateLogger<InboxMessageCleaner>();
        }

        protected override TimeSpan TimerPeriodConfig()
        {
            return TimeSpan.FromDays(1);
        }

        protected override void Process(object state)
        {
            // We need this check to ensure that the database was seeded successfully in the Startup.cs class.
            // This process needs database tables to interact with.
            if (!ApplicationStarted)
            {
                return;
            }

            using (var scope = _provider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                var inboxRepository = scope.ServiceProvider.GetRequiredService<IRepository<InboxMessage>>();
                bool continueDelete = true;

                while (continueDelete)
                {
                    var legacyMessages = TakeMessageToDelete(inboxRepository, uowManager);
                    if (!legacyMessages.Any())
                    {
                        break;
                    }

                    uowManager.StartNewTransaction(() =>
                    {
                        inboxRepository.DeleteMany(legacyMessages);
                    });

                    _logger.NumberOfCleanedMessages(legacyMessages.Count);
                    continueDelete = legacyMessages.Count >= _inboxOptions.DeleteBatchSize;
                }
            }
        }

        private List<InboxMessage> TakeMessageToDelete(IRepository<InboxMessage> inboxRepository, IUnitOfWorkManager uowManager)
        {
            var deleteCheckpoint = Clock.Now.AddDays(-1 * _inboxOptions.DeleteMessageAfterDays);

            try
            {
                var result = new List<InboxMessage>();
                uowManager.StartNewTransaction(() =>
                {
                    result = inboxRepository
                        .GetAll()
                        .Where(m => m.CreatedDate < deleteCheckpoint)
                        .Where(m => m.Status == MessageStatus.Processed)
                        .Take(_inboxOptions.DeleteBatchSize)
                        .ToList();

                    foreach (var sentMessage in result)
                    {
                        sentMessage.ReadyToDelete = true;
                    }

                    inboxRepository.UpdateMany(result);
                });

                return result;
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                _logger.ExceptionWhenDeleteMessageFromQueue(concurrencyException);
                return new List<InboxMessage>();
            }
            catch (Exception exception)
            {
                _logger.ExceptionWhenDeleteMessageFromQueue(exception);
                return new List<InboxMessage>();
            }
        }
    }
}
