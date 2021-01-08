using System;
using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.OutboxPattern.HostedServices.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Conexus.Opal.OutboxPattern.HostedServices
{
    public class OutboxMessageCleaner : OutboxMessageHostedService, IOutboxMessageCleaner
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<OutboxMessageCleaner> _logger;
        private readonly OutboxOptions _outboxOptions;

        public OutboxMessageCleaner(
            IHostApplicationLifetime lifetime,
            IServiceProvider provider,
            IOptions<OutboxOptions> options,
            ILoggerFactory loggerFactory) : base(lifetime)
        {
            _provider = provider;
            _outboxOptions = options.Value;
            _logger = loggerFactory.CreateLogger<OutboxMessageCleaner>();
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
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IRepository<OutboxMessage>>();
                bool continueDelete = true;

                while (continueDelete)
                {
                    var legacyMessages = TakeMessageToDelete(outboxRepository, uowManager);
                    if (!legacyMessages.Any())
                    {
                        break;
                    }

                    uowManager.StartNewTransaction(() =>
                    {
                        outboxRepository.DeleteMany(legacyMessages);
                    });

                    _logger.NumberOfCleanedMessages(legacyMessages.Count);
                    continueDelete = legacyMessages.Count >= _outboxOptions.DeleteBatchSize;
                }
            }
        }

        private List<OutboxMessage> TakeMessageToDelete(IRepository<OutboxMessage> outboxRepository, IUnitOfWorkManager uowManager)
        {
            var deleteCheckpoint = Clock.Now.AddDays(-1 * _outboxOptions.DeleteMessageAfterDays);

            try
            {
                var result = new List<OutboxMessage>();
                uowManager.StartNewTransaction(() =>
                {
                    result = outboxRepository
                        .GetAll()
                        .Where(m => m.CreatedDate < deleteCheckpoint)
                        .Where(m => m.Status == MessageStatus.Sent)
                        .Take(_outboxOptions.DeleteBatchSize)
                        .ToList();

                    foreach (var sentMessage in result)
                    {
                        sentMessage.ReadyToDelete = true;
                    }

                    outboxRepository.UpdateMany(result);
                });

                return result;
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                _logger.ExceptionWhenDeleteMessageFromQueue(concurrencyException);
                return new List<OutboxMessage>();
            }
            catch (Exception exception)
            {
                _logger.ExceptionWhenDeleteMessageFromQueue(exception);
                return new List<OutboxMessage>();
            }
        }
    }
}
