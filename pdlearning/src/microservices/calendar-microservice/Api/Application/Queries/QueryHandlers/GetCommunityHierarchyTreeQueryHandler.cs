using System;
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
    /// Get community hierarchy tree by user's id.
    /// </summary>
    public class GetCommunityHierarchyTreeQueryHandler : BaseQueryHandler<GetCommunityHierarchyTreeQuery, List<CommunityModel>>
    {
        private readonly IRepository<CommunityMembership> _communityMembershipRepo;
        private readonly IRepository<Community> _communityRepo;

        public GetCommunityHierarchyTreeQueryHandler(
            IRepository<CommunityMembership> communityMembershipRepo,
            IRepository<Community> communityRepo)
        {
            _communityMembershipRepo = communityMembershipRepo;
            _communityRepo = communityRepo;
        }

        protected override async Task<List<CommunityModel>> HandleAsync(GetCommunityHierarchyTreeQuery query, CancellationToken cancellationToken)
        {
            var communityIds = _communityMembershipRepo
                .GetAll()
                .Where(cm => cm.UserId == query.UserId)
                .Select(cm => cm.CommunityId);

            var allUserCommunities = await _communityRepo
                .GetAllIncluding(c => c.Parent)
                .Where(c => communityIds.Contains(c.Id))
                .Where(c => c.Status == CommunityStatus.Enabled)
                .ToListAsync(cancellationToken);

            var parentCommunities = allUserCommunities.Where(c => c.ParentId == null);
            var subCommunities = allUserCommunities.Where(c => c.ParentId != null);

            var communityHierarchy =
                from parentCommunity in parentCommunities
                join subCommunity in subCommunities
                    on parentCommunity.Id equals subCommunity.ParentId into joinedSubCommunities
                select new CommunityModel
                {
                    Id = parentCommunity.Id,
                    Title = parentCommunity.Title,
                    SubCommunities = joinedSubCommunities.Select(child => new CommunityModel
                    {
                        Id = child.Id,
                        Title = child.Title
                    }).ToList()
                };

            return communityHierarchy.ToList();
        }
    }
}
