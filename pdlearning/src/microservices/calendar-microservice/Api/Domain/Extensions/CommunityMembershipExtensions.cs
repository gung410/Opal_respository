using System;
using System.Linq;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class CommunityMembershipExtensions
    {
        public static IQueryable<CommunityMembership> OnlyOwnerOrAdmin(this IQueryable<CommunityMembership> query)
        {
            return query.Where(cm => cm.Role == CommunityMembershipRole.Owner || cm.Role == CommunityMembershipRole.Admin);
        }

        public static IQueryable<CommunityMembership> IsMemberOf(this IQueryable<CommunityMembership> query, Guid userId, Guid communityId)
        {
            return query.Where(cm => cm.CommunityId == communityId && cm.UserId == userId);
        }
    }
}
