using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetDetailByDigitalContentIdQueryHandler : BaseQueryHandler<GetDetailByDigitalContentIdQuery, DigitalContentModel>
    {
        private readonly IRepository<MyDigitalContent> _myDigitalContentRepository;
        private readonly IRepository<UserBookmark> _userBookmarkRepository;
        private readonly IRepository<UserReview> _userReviewRepository;
        private readonly IRepository<LearningTracking> _learningTrackingRepository;

        public GetDetailByDigitalContentIdQueryHandler(
            IRepository<MyDigitalContent> myDigitalContentRepository,
            IRepository<UserBookmark> userBookmarkRepository,
            IRepository<UserReview> userReviewRepository,
            IRepository<LearningTracking> learningTrackingRepository,
            IUserContext userContext) : base(userContext)
        {
            _myDigitalContentRepository = myDigitalContentRepository;
            _userBookmarkRepository = userBookmarkRepository;
            _userReviewRepository = userReviewRepository;
            _learningTrackingRepository = learningTrackingRepository;
        }

        protected override async Task<DigitalContentModel> HandleAsync(GetDetailByDigitalContentIdQuery query, CancellationToken cancellationToken)
        {
            var myDigitalContent = await _myDigitalContentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.DigitalContentId == query.DigitalContentId)
                .FirstOrDefaultAsync(cancellationToken);

            var bookmark = await _userBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemId == query.DigitalContentId)
                .Where(p => p.ItemType == BookmarkType.DigitalContent)
                .FirstOrDefaultAsync(cancellationToken);

            var userReviews = await _userReviewRepository
                .GetAll()
                .Where(p => p.ItemId == query.DigitalContentId)
                .Where(p => p.Rate.HasValue)
                .Select(p => new
                {
                    Rate = p.Rate.Value
                })
                .ToListAsync(cancellationToken);

            var averageRating = userReviews.Any()
                ? Math.Round(userReviews.Select(p => p.Rate).Average(), 1)
                : 0;

            var learningTracking = await _learningTrackingRepository
                .GetAll()
                .Where(p => p.ItemId == query.DigitalContentId)
                .Where(p => p.TrackingType == LearningTrackingType.DigitalContent)
                .Select(p => new
                {
                    p.TrackingAction,
                    p.TotalCount
                })
                .ToDictionaryAsync(p => p.TrackingAction, cancellationToken);

            var viewsCount = learningTracking.ContainsKey(LearningTrackingAction.View)
                ? learningTracking[LearningTrackingAction.View].TotalCount
                : 0;

            var downloadsCount = learningTracking.ContainsKey(LearningTrackingAction.DownloadContent)
                ? learningTracking[LearningTrackingAction.DownloadContent].TotalCount
                : 0;

            return DigitalContentModel.New(
                    query.DigitalContentId,
                    averageRating,
                    userReviews.Count)
                .WithBookmarkInfo(bookmark)
                .WithMyDigitalContent(myDigitalContent)
                .WithViewsCount(viewsCount)
                .WithDownloadsCount(downloadsCount);
        }
    }
}
