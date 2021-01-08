using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetUserSharingByItemIdsQueryHandler : BaseQueryHandler<GetUserSharingByItemIdsQuery, List<UserSharingModel>>
    {
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;
        private readonly IRepository<UserFollowing> _userFollowingRepository;

        public GetUserSharingByItemIdsQueryHandler(
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IRepository<UserFollowing> userFollowingRepository,
            IUserContext userContext) : base(userContext)
        {
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
            _userFollowingRepository = userFollowingRepository;
        }

        protected override async Task<List<UserSharingModel>> HandleAsync(GetUserSharingByItemIdsQuery query, CancellationToken cancellationToken)
        {
            if (query.ItemIds.Length == 0)
            {
                return new List<UserSharingModel>();
            }

            // TODO: consider to resolve duplicate code in GetUserSharingByItemIdsQueryHandler, GetUserSharingByItemIdQueryHandler, GetUserSharingByIdQueryHandler
            var userSharingList = await _userSharingRepository
                .GetAll()
                .Where(p => query.ItemIds.Contains(p.ItemId))
                .ToListAsync(cancellationToken);

            var userSharingIds = userSharingList.Select(p => p.Id).ToList();

            var userSharingDetails = await _userSharingDetailRepository
                .GetAll()
                .Where(p => userSharingIds.Contains(p.UserSharingId))
                .ToListAsync(cancellationToken);

            var userSharedIds = userSharingDetails.Select(p => p.UserId).Distinct().ToList();

            var userFollowing = await _userFollowingRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => userSharedIds.Contains(p.FollowingUserId))
                .Select(p => p.FollowingUserId)
                .ToListAsync(cancellationToken);

            var userSharingDetailsWithFollowing = userSharingDetails
                .Select(userSharingDetail => new UserSharingDetailModel(
                    entity: userSharingDetail,
                    isFollowing: userFollowing.Any(q => q == userSharingDetail.UserId)))
                .ToList();

            return userSharingList
                .Select(userSharing => new UserSharingModel(
                    entity: userSharing,
                    sharedUsers: userSharingDetailsWithFollowing
                        .Where(p => p.UserSharingId == userSharing.Id)
                        .ToList()))
                .ToList();
        }
    }
}
