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
    public class GetTrackingByItemIdQueryHandler : BaseQueryHandler<GetTrackingByItemIdQuery, TrackingModel>
    {
        private readonly IRepository<UserLike> _userLikeRepository;
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;
        private readonly IRepository<LearningTracking> _learningTrackingRepository;

        public GetTrackingByItemIdQueryHandler(
            IRepository<UserLike> userLikeRepository,
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IRepository<LearningTracking> learningTrackingRepository,
            IUserContext userContext) : base(userContext)
        {
            _userLikeRepository = userLikeRepository;
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
            _learningTrackingRepository = learningTrackingRepository;
        }

        protected override async Task<TrackingModel> HandleAsync(GetTrackingByItemIdQuery query, CancellationToken cancellationToken)
        {
            var isLike = false;
            var totalDownload = -1;
            var totalLike = -1;
            var totalShare = -1;
            var totalView = -1;

            var trackingDownload = query.TrackingAction == LearningTrackingAction.DownloadContent;
            var trackingLike = query.TrackingAction == LearningTrackingAction.Like;
            var trackingShare = query.TrackingAction == LearningTrackingAction.Share;
            var trackingView = query.TrackingAction == LearningTrackingAction.View;

            if (query.TrackingAction == LearningTrackingAction.All)
            {
                trackingDownload = trackingLike = trackingShare = trackingView = true;
            }

            if (trackingLike)
            {
                var queryUserLike = _userLikeRepository
                    .GetAll()
                    .Where(user => user.ItemId == query.ItemId)
                    .Where(user => user.ItemType == query.ItemType);

                isLike = await queryUserLike
                    .Where(user => user.CreatedBy == CurrentUserId)
                    .CountAsync(cancellationToken) == 1;

                totalLike = await queryUserLike
                    .CountAsync(cancellationToken);
            }

            if (trackingShare)
            {
                var itemType = (SharingType)query.ItemType;
                var userSharingIds = _userSharingRepository
                    .GetAll()
                    .Where(user => user.ItemId == query.ItemId)
                    .Where(user => user.ItemType == itemType)
                    .Select(user => user.Id);

                totalShare = await _userSharingDetailRepository
                     .GetAll()
                     .Where(detail => userSharingIds.Contains(detail.UserSharingId))
                     .GroupBy(detail => new { detail.UserId, detail.CreatedBy })
                     .CountAsync(cancellationToken);
            }

            if (trackingDownload || trackingView)
            {
                var learningTrackingDict = await _learningTrackingRepository
                    .GetAll()
                    .Where(learningTracking => learningTracking.ItemId == query.ItemId)
                    .Where(learningTracking => learningTracking.TrackingType == query.ItemType)
                    .Select(learningTracking => new
                    {
                        learningTracking.TrackingAction,
                        learningTracking.TotalCount
                    })
                    .ToDictionaryAsync(learningTracking => learningTracking.TrackingAction, cancellationToken);

                totalDownload = learningTrackingDict.ContainsKey(LearningTrackingAction.DownloadContent)
                    ? learningTrackingDict[LearningTrackingAction.DownloadContent].TotalCount
                    : 0;

                totalView = learningTrackingDict.ContainsKey(LearningTrackingAction.View)
                    ? learningTrackingDict[LearningTrackingAction.View].TotalCount
                    : 0;
            }

            return new TrackingModel
            {
                ItemId = query.ItemId,
                ItemType = query.ItemType,
                IsLike = isLike,
                TotalDownload = totalDownload,
                TotalLike = totalLike,
                TotalShare = totalShare,
                TotalView = totalView
            };
        }
    }
}
