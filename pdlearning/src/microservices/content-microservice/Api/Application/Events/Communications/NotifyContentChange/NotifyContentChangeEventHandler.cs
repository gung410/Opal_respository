using Conexus.Opal.OutboxPattern;
using Thunder.Platform.Core.Context;

namespace Microservice.Content.Application.Events
{
    public class NotifyContentChangeEventHandler : BaseContentCommunicationEventHandler<NotifyContentChangeEvent>
    {
        public NotifyContentChangeEventHandler(IUserContext userContext, IOutboxQueue outboxQueue) : base(userContext, outboxQueue)
        {
        }
    }
}
