using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.Microservice.Tagging.DataProviders;

namespace Conexus.Opal.Microservice.Tagging.Consumers
{
    [OpalConsumer("microservice.events.metadata.clone_resouce")]
    public class CloneResourceConsumer : OpalMessageConsumer<CloneResourcePayload>
    {
        private readonly ITaggingDataProvider taggingDataProvider;

        public CloneResourceConsumer(ITaggingDataProvider taggingDataProvider)
        {
            this.taggingDataProvider = taggingDataProvider;
        }

        protected override async Task InternalHandleAsync(CloneResourcePayload message)
        {
            await taggingDataProvider.CloneResource(message.CloneToResouceId, message.CloneFromResouceId, message.UserId);
        }
    }
}
