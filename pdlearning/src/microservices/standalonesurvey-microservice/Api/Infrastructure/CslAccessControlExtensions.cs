using System;
using System.Linq;
using System.Linq.Expressions;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Thunder.Platform.Core.Extensions;

namespace Microservice.StandaloneSurvey.Infrastructure
{
    public static class CslAccessControlExtensions
    {
        public static IQueryable<Domain.Entities.StandaloneSurvey> ApplyCslAccessControl(
            this IQueryable<Domain.Entities.StandaloneSurvey> query,
            ICslAccessControlContext cslAccessControlContext,
            CommunityMembershipRole[] roles = null,
            Guid? communityId = null,
            Expression<Func<Domain.Entities.StandaloneSurvey, bool>> includePredicate = null)
        {
            return query.WhereIfElse(
                    communityId != null,
                    s => s.CommunityId.HasValue && s.CommunityId.Value == communityId,
                    s => s.CommunityId.HasValue)
                .WhereIf(includePredicate != null, includePredicate)
                .Join(cslAccessControlContext.GetCommunityIds(roles), s => s.CommunityId, i => i, (survey, communityId) => survey);
        }
    }
}
