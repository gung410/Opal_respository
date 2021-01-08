using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.SharedQueries
{
    public class ReadUserReviewShared : BaseReadShared<UserReview>, IReadUserReviewShared
    {
        public ReadUserReviewShared(
            IReadOnlyRepository<UserReview> readUserReviewRepository) : base(readUserReviewRepository)
        {
        }

        public async Task<Dictionary<Guid, ReviewSummary>> GetReviewSummary(List<Guid> itemIds)
        {
            if (!itemIds.Any())
            {
                return new Dictionary<Guid, ReviewSummary>();
            }

            var userReviews = await ReadRepository
                .GetAll()
                .Where(p => p.Rate.HasValue && itemIds.Contains(p.ItemId))
                .Select(p => new { p.ItemId, Rating = p.Rate.Value })
                .ToListAsync();

            var userReviewDictionary = userReviews
                .GroupBy(p => p.ItemId)
                .ToDictionary(p => p.Key, p => p.Select(_ => _.Rating).ToArray());

            return itemIds.ToDictionary(
                p => p,
                p =>
                {
                    userReviewDictionary.TryGetValue(p, out var reviews);

                    return reviews != null
                        ? new ReviewSummary(reviews.Length, Math.Round(reviews.Average(), 1))
                        : ReviewSummary.Default();
                });
        }
    }
}
