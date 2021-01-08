using System;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.BusinessLogic
{
    public class WriteUserReviewLogic : BaseBusinessLogic<UserReview>, IWriteUserReviewLogic
    {
        private readonly IReadOnlyRepository<UserReview> _readUserReviewRepository;

        public WriteUserReviewLogic(
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<UserReview> readUserReviewRepository,
            IWriteOnlyRepository<UserReview> writeUserReviewRepository)
            : base(thunderCqrs, writeUserReviewRepository)
        {
            _readUserReviewRepository = readUserReviewRepository;
        }

        public async Task Insert(UserReview userReview)
        {
            await SendAverageReviewMessage(userReview);

            await ThunderCqrs.SendEvent(new UserReviewChangeEvent(userReview, UserReviewEventChangeType.Created));

            await WriteRepository.InsertAsync(userReview);
        }

        public async Task Update(UserReview userReview)
        {
            await SendAverageReviewMessage(userReview);

            await ThunderCqrs.SendEvent(new UserReviewChangeEvent(userReview, UserReviewEventChangeType.Updated));

            await WriteRepository.UpdateAsync(userReview);
        }

        private async Task SendAverageReviewMessage(UserReview userReview)
        {
            // For digital content only
            if (userReview.ItemType != ItemType.DigitalContent)
            {
                return;
            }

            var rateReviews = await _readUserReviewRepository
                .GetAll()
                .Where(p => p.ItemId == userReview.ItemId)
                .Where(p => p.ItemType == userReview.ItemType)
                .Where(p => p.Rate.HasValue)
                .Select(p => p.Rate.Value)
                .ToListAsync();

            if (!rateReviews.Any())
            {
                return;
            }

            var averageReviewRating = new AverageReviewRatingModel
            {
                ItemId = userReview.ItemId,
                ItemType = userReview.ItemType,
                AverageRating = Math.Round(rateReviews.Average(), 1),
                ReviewsCount = rateReviews.Count
            };

            await ThunderCqrs.SendEvent(new AverageReviewRatingChangedEvent(averageReviewRating, AverageReviewRatingType.Updated));
        }
    }
}
