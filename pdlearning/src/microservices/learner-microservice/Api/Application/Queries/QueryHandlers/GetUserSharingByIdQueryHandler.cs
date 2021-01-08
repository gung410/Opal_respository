using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetUserSharingByIdQueryHandler : BaseQueryHandler<GetUserSharingByIdQuery, UserSharingModel>
    {
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;
        private readonly IRepository<UserFollowing> _userFollowingRepository;

        public GetUserSharingByIdQueryHandler(
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IRepository<UserFollowing> userFollowingRepository,
            IUserContext userContext) : base(userContext)
        {
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
            _userFollowingRepository = userFollowingRepository;
        }

        protected override async Task<UserSharingModel> HandleAsync(GetUserSharingByIdQuery query, CancellationToken cancellationToken)
        {
            var userSharing = await _userSharingRepository.FirstOrDefaultAsync(p => p.Id == query.Id);
            if (userSharing == null)
            {
                throw new EntityNotFoundException(typeof(UserSharing), query.Id);
            }

            // TODO: consider to resolve duplicate code in GetUserSharingByItemIdsQueryHandler, GetUserSharingByItemIdQueryHandler, GetUserSharingByIdQueryHandler
            var userSharingDetails = await _userSharingDetailRepository
                .GetAll()
                .Where(p => p.UserSharingId == userSharing.Id)
                .ToListAsync(cancellationToken);

            var userSharedIds = userSharingDetails.Select(p => p.UserId).ToList();
            if (userSharedIds.Count == 0)
            {
                return new UserSharingModel(userSharing, new List<UserSharingDetailModel>());
            }

            var userFollowing = await _userFollowingRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => userSharedIds.Contains(p.FollowingUserId))
                .Select(p => p.FollowingUserId)
                .ToListAsync(cancellationToken);

            var userSharingDetailsWithFollowing = userSharingDetails
                .Select(p => new UserSharingDetailModel(
                    entity: p,
                    isFollowing: userFollowing.Any(q => q == p.UserId)))
                .ToList();

            return new UserSharingModel(userSharing, userSharingDetailsWithFollowing);
        }
    }
}
