using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.BrokenLinkChecker;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.BrokenLink.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.BrokenLink.Application.Consumers
{
    [OpalConsumer("microservice.events.brokenlink.dequeue-object")]
    public class DequeueObjectConsumer : ScopedOpalMessageConsumer<DequeueObjectMessage>
    {
        public async Task InternalHandleAsync(
            DequeueObjectMessage message,
            IRepository<ExtractedUrl> extractedUrlRepository,
            IRepository<BrokenLinkReport> brokenLinkReportRepository)
        {
            var objectId = message.ObjectId;

            var extractedUrls = extractedUrlRepository
                .GetAll()
                .Where(p => p.ObjectId == objectId)
                .ToList();

            await extractedUrlRepository.DeleteManyAsync(extractedUrls);

            var brokenLinkReports = brokenLinkReportRepository
                .GetAll()
                .Where(p => p.ObjectId == objectId)
                .ToList();

            await brokenLinkReportRepository.DeleteManyAsync(brokenLinkReports);
        }
    }
}
