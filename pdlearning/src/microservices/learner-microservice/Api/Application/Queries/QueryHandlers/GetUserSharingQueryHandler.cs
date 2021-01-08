using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetUserSharingQueryHandler : BaseQueryHandler<GetUserSharingQuery, PagedResultDto<UserSharingModel>>
    {
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;
        private readonly IRepository<UserFollowing> _userFollowingRepository;

        public GetUserSharingQueryHandler(
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IRepository<UserFollowing> userFollowingRepository,
            IUserContext userContext) : base(userContext)
        {
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
            _userFollowingRepository = userFollowingRepository;
        }

        protected override async Task<PagedResultDto<UserSharingModel>> HandleAsync(GetUserSharingQuery query, CancellationToken cancellationToken)
        {
            var userSharingQuery = _userSharingRepository
                .GetAll()
                .Where(p => p.CreatedBy == CurrentUserId)
                .Where(p => p.ItemType == query.ItemType);

            var totalCount = await userSharingQuery.CountAsync(cancellationToken);

            var sortCriteria = $"{nameof(UserSharing.CreatedDate)} DESC";
            userSharingQuery = ApplySorting(userSharingQuery, query.PageInfo, sortCriteria);
            userSharingQuery = ApplyPaging(userSharingQuery, query.PageInfo);

            var userSharingList = await userSharingQuery.ToListAsync(cancellationToken);

            var userSharingIds = userSharingList.Select(p => p.Id).ToList();

            var userSharingDetails = await _userSharingDetailRepository
                .GetAll()
                .Where(p => userSharingIds.Contains(p.UserSharingId))
                .ToListAsync(cancellationToken);

            var userSharedIds = userSharingDetails
                .Select(p => p.UserId)
                .Distinct()
                .ToList();

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

            var userSharingItems = userSharingList
                .Select(userSharing => new UserSharingModel(
                    entity: userSharing,
                    sharedUsers: userSharingDetailsWithFollowing
                        .Where(p => p.UserSharingId == userSharing.Id)
                        .ToList()))
                .ToList();

            return new PagedResultDto<UserSharingModel>(totalCount, userSharingItems);
        }
    }
}
