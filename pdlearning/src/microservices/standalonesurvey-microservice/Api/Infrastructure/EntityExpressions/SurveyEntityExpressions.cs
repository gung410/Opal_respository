using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Service.Authentication;

namespace Microservice.StandaloneSurvey.Infrastructure
{
    public class SurveyEntityExpressions
    {
        public static Expression<Func<Domain.Entities.StandaloneSurvey, bool>> HasAuthorizedRolesExpr(List<string> roles)
        {
            return f => roles.Any(r =>
                r == UserRoles.DivisionalTrainingCoordinator ||
                r == UserRoles.SchoolStaffDeveloper ||
                r == UserRoles.DivisionAdministrator ||
                r == UserRoles.BranchAdministrator ||
                r == UserRoles.SystemAdministrator);
        }

        public static Expression<Func<Domain.Entities.StandaloneSurvey, bool>> HasOwnerPermissionExpr(Guid userId)
        {
            return f => f.OwnerId == userId;
        }

        public static Expression<Func<Domain.Entities.StandaloneSurvey, bool>> HasPermissionToSeeFormExpr(Guid userId)
        {
            return f => f.Status == SurveyStatus.Published
                || f.OwnerId == userId;
        }

        public static Expression<Func<Domain.Entities.StandaloneSurvey, bool>> AvailableForImportToCourseExpr(Guid userId)
        {
            return f => f.Status == SurveyStatus.Published && f.OwnerId == userId;
        }

        public static Expression<Func<Domain.Entities.StandaloneSurvey, bool>> FilterCslSurveyPublishedExpr()
        {
            return f => f.Status == SurveyStatus.Published;
        }

        public static CommunityMembershipRole[] AllManageableCslRoles()
        {
            return new[] { CommunityMembershipRole.Admin, CommunityMembershipRole.Owner };
        }

        public static CommunityMembershipRole[] AllViewableCslRoles()
        {
            return new[] { CommunityMembershipRole.Admin, CommunityMembershipRole.Owner, CommunityMembershipRole.Moderator, CommunityMembershipRole.Member };
        }
    }
}
