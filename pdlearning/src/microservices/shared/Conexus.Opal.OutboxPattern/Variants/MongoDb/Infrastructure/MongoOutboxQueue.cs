using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.OutboxPattern.Variants.MongoDb.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Json;

namespace Conexus.Opal.OutboxPattern.Variants.MongoDb.Infrastructure
{
    public class MongoOutboxQueue : IOutboxQueue
    {
        private readonly IHasOutboxCollection _outboxDbContext;
        private readonly IUserContext _userContext;

        public MongoOutboxQueue(IHasOutboxCollection outboxDbContext, IUserContext userContext)
        {
            _outboxDbContext = outboxDbContext;
            _userContext = userContext;
        }

        /// <inheritdoc/>
        public Task QueueMessageAsync(QueueMessage queueMessage)
        {
            return _outboxDbContext
                .OutboxMessageCollection
                .InsertOneAsync(new MongoOutboxMessage
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
            var failureMessages = await _outboxDbContext
                .OutboxMessageCollection
                .AsQueryable()
                .Where(x => x.Status == MessageStatus.Failure)
                .ToListAsync();

            failureMessages.ForEach(item =>
            {
                item.Status = MessageStatus.New;
            });

            var tasks = new List<Task>();

            foreach (var failureMessage in failureMessages)
            {
                var task = _outboxDbContext
                    .OutboxMessageCollection
                    .UpdateWithIncreaseVersionAsync(failureMessage);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
    }
}
