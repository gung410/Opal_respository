using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetUserReviewsQueryHandler : BaseQueryHandler<GetUserReviewsQuery, PagedReviewDto<UserReviewModel>>
    {
        private readonly IRepository<UserReview> _userReviewRepository;

        public GetUserReviewsQueryHandler(
            IRepository<UserReview> userReviewRepository,
            IUserContext userContext) : base(userContext)
        {
            _userReviewRepository = userReviewRepository;
        }

        protected override async Task<PagedReviewDto<UserReviewModel>> HandleAsync(GetUserReviewsQuery query, CancellationToken cancellationToken)
        {
            var userReviewQuery = _userReviewRepository
                .GetAll()
                .Where(p => p.ItemId == query.ItemId);

            if (query.ClassRunId != null)
            {
                userReviewQuery = userReviewQuery.Where(p => p.ClassRunId == query.ClassRunId);
            }

            if (query.ItemTypeFilter?.Count > 0)
            {
                userReviewQuery = userReviewQuery.Where(p => query.ItemTypeFilter.Contains(p.ItemType));
            }

            var rateReviews = await userReviewQuery
                .Where(p => p.Rate.HasValue)
                .Select(p => p.Rate.Value)
                .ToListAsync(cancellationToken);

            var reviewSummary = new ReviewSummary(rateReviews.Count, rateReviews.Any() ? Math.Round(rateReviews.Average(), 1) : 0);

            var sortCriteria = $"{nameof(UserReview.ChangedDate)} DESC, {nameof(UserReview.CreatedDate)} DESC";
            userReviewQuery = ApplySorting(userReviewQuery, query.PageInfo, sortCriteria);
            userReviewQuery = ApplyPaging(userReviewQuery, query.PageInfo);

            var userReviews = await userReviewQuery.ToListAsync(cancellationToken);

            return new PagedReviewDto<UserReviewModel>(
                reviewSummary.ReviewCount,
                items: userReviews.Select(p => new UserReviewModel(p)).ToList(),
                reviewSummary.AverageRating);
        }
    }
}
