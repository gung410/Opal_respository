using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.BrokenLinkChecker;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.BrokenLink.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.BrokenLink.Application.Consumers
{
    [OpalConsumer("microservice.events.brokenlink.enqueue-urls")]
    public class EnqueueUrlConsumer : ScopedOpalMessageConsumer<EnqueueUrlMessage>
    {
        public async Task InternalHandleAsync(
            EnqueueUrlMessage message,
            IRepository<ExtractedUrl> extractedUrlRepository)
        {
            var objectId = message.ObjectId;

            var currentExtractedUrls = extractedUrlRepository
                .GetAll()
                .Where(p => p.ObjectId == objectId)
                .ToList();

            await extractedUrlRepository.DeleteManyAsync(currentExtractedUrls);

            var extractedUrls = message.Urls.Select(url => new ExtractedUrl
            {
                Id = Guid.NewGuid(),
                ObjectDetailUrl = message.ObjectDetailUrl,
                ObjectId = message.ObjectId,
                ObjectOwnerId = message.ObjectOwnerId,
                ObjectOwnerName = message.ObjectOwnerName,
                ObjectTitle = message.ObjectTitle,
                OriginalObjectId = message.OriginalObjectId,
                ParentId = message.ParentId,
                Status = ScanUrlStatus.None,
                Url = url,
                Module = message.Module,
                ContentType = message.ContentType
            });

            await extractedUrlRepository.InsertManyAsync(extractedUrls);
        }
    }
}
