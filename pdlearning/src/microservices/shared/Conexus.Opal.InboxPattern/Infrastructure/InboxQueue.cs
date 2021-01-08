using System;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.InboxPattern.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Json;

namespace Conexus.Opal.InboxPattern.Infrastructure
{
    public class InboxQueue : IInboxQueue
    {
        private readonly IRepository<InboxMessage> _inboxMessageRepository;

        public InboxQueue(IRepository<InboxMessage> inboxMessageRepository)
        {
            _inboxMessageRepository = inboxMessageRepository;
        }

        public async Task QueueMessageAsync(QueueMessage queueMessage)
        {
            await _inboxMessageRepository.InsertAsync(new InboxMessage
            {
                Id = Guid.NewGuid(),
                MessageId = queueMessage.MessageId,
                MessageCreatedAt = queueMessage.Created,
                MessageData = JsonSerializer.Serialize(
                    queueMessage.Message,
                    ThunderJsonSerializerOptions.SharedJsonSerializerOptions),
                Status = MessageStatus.New
            });
        }
    }
}
