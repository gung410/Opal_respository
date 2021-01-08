using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Content.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Content.Application.Consumers
{
    [OpalConsumer("microservice.events.learner.averagereviewrating.updated")]
    public class RatingScoreAndReviewCountUpdateConsumer : ScopedOpalMessageConsumer<RatingScoreAndReviewCountChangeMessage>
    {
        public async Task InternalHandleAsync(RatingScoreAndReviewCountChangeMessage message, IRepository<DigitalContent> digitalContentRepo)
        {
            var existingDigitalContent = await digitalContentRepo.GetAllListAsync(p => p.OriginalObjectId == message.ItemId);
            if (existingDigitalContent == null)
            {
                throw new EntityNotFoundException(typeof(DigitalContent), message.ItemId);
            }

            foreach (var digitalContent in existingDigitalContent)
            {
                digitalContent.ReviewCount = message.ReviewsCount;
                digitalContent.AverageRating = message.AverageRating;
                await digitalContentRepo.UpdateAsync(digitalContent);
            }
        }
    }
}
