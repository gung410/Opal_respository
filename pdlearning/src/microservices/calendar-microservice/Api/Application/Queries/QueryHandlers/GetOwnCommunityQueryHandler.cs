using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.Queries
{
    /// <summary>
    /// Get communities that user is owner / admin (co-owner).
    /// </summary>
    public class GetOwnCommunityQueryHandler : BaseQueryHandler<GetOwnCommunityQuery, List<CommunityModel>>
    {
        private readonly IRepository<CommunityMembership> _communityMembershipRepo;
        private readonly IRepository<Community> _communityRepository;

        public GetOwnCommunityQueryHandler(
            IRepository<CommunityMembership> communityMembershipRepo,
            IRepository<Community> communityRepo)
        {
            _communityMembershipRepo = communityMembershipRepo;
            _communityRepository = communityRepo;
        }

        protected override async Task<List<CommunityModel>> HandleAsync(GetOwnCommunityQuery query, CancellationToken cancellationToken)
        {
            var communityIds = _communityMembershipRepo
                .GetAll()
                .Where(c => c.UserId == query.UserId)
                .Where(c => c.Role == CommunityMembershipRole.Admin || c.Role == CommunityMembershipRole.Owner)
                .Select(c => c.CommunityId);

            var communities = from communityId in communityIds
                              join community in _communityRepository.GetAll().Where(c => c.Status == CommunityStatus.Enabled)
                                  on communityId equals community.Id
                              select new CommunityModel { Id = community.Id, Title = community.Title };

            return await communities
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
