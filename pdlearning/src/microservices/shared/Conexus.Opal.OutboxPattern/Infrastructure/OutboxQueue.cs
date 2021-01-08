using System;
using System.Text.Json;
using System.Threading.Tasks;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Json;

namespace Conexus.Opal.OutboxPattern
{
    public class OutboxQueue : IOutboxQueue
    {
        private readonly IRepository<OutboxMessage> _outboxMessageRepository;
        private readonly IUserContext _userContext;

        public OutboxQueue(IRepository<OutboxMessage> outboxMessageRepository, IUserContext userContext)
        {
            _outboxMessageRepository = outboxMessageRepository;
            _userContext = userContext;
        }

        /// <inheritdoc/>
        public async Task QueueMessageAsync(QueueMessage queueMessage)
        {
            await _outboxMessageRepository.InsertAsync(new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                MessageData = JsonSerializer.Serialize(queueMessage.Message, ThunderJsonSerializerOptions.SharedJsonSerializerOptions),
                RoutingKey = queueMessage.RoutingKey,
                Exchange = queueMessage.Exchange,
                SourceIp = _userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                Status = MessageStatus.New,
                UserId = _userContext.GetValue<string>(CommonUserContextKeys.UserId)
            });
        }

        public async Task RequeueMessageAsync()
        {
            var failureMessages = await _outboxMessageRepository.GetAllListAsync(x => x.Status == MessageStatus.Failure);
            failureMessages.ForEach(item =>
            {
                item.Status = MessageStatus.New;
            });

            await _outboxMessageRepository.UpdateManyAsync(failureMessages);
        }
    }
}
