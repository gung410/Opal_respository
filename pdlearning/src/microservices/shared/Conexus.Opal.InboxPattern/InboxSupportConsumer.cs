using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern.Entities;
using Conexus.Opal.InboxPattern.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Conexus.Opal.InboxPattern
{
    public class InboxSupportConsumer<TMessage> : ScopedOpalMessageConsumer<TMessage>
         where TMessage : class
    {
        protected override async Task<bool> OnBeforeMessageHandling(IServiceScope currentServiceScope)
        {
            var unitOfWorkManager = currentServiceScope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            var inboxQueue = currentServiceScope.ServiceProvider.GetService<IInboxQueue>();
            var inboxRepo = currentServiceScope.ServiceProvider.GetService<IRepository<InboxMessage>>();

            bool continueProcess = true;
            await unitOfWorkManager.StartNewTransactionAsync(async () =>
            {
                var existedMessage = await inboxRepo.GetAll().AnyAsync(_ => _.MessageId == OriginMessage.Id);

                if (existedMessage)
                {
                    // not continue process so far
                    continueProcess = false;
                    return;
                }

                await inboxQueue.QueueMessageAsync(new QueueMessage(OriginMessage.Id, OriginMessage, Clock.Now));
            });

            return continueProcess;
        }

        protected override async Task OnAfterMessageHandling(IServiceScope currentServiceScope)
        {
            var unitOfWorkManager = currentServiceScope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            await unitOfWorkManager.StartNewTransactionAsync(async () =>
            {
                var inboxRepo = currentServiceScope.ServiceProvider.GetService<IRepository<InboxMessage>>();

                // expect message was existed.
                var existedMessage = await inboxRepo.GetAll().FirstAsync(_ => _.MessageId == OriginMessage.Id);

                existedMessage.Status = MessageStatus.Processed;
                await inboxRepo.UpdateAsync(existedMessage);
            });
        }
    }
}
