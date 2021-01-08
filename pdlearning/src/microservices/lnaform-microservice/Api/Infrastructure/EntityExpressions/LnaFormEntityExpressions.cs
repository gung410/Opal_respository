using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Service.Authentication;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Infrastructure
{
    public class LnaFormEntityExpressions
    {
        public static Expression<Func<FormEntity, bool>> HasAuthorizedRolesExpr(List<string> roles)
        {
            return f => roles.Any(r =>
                r == UserRoles.DivisionalTrainingCoordinator ||
                r == UserRoles.SchoolStaffDeveloper ||
                r == UserRoles.DivisionAdministrator ||
                r == UserRoles.BranchAdministrator ||
                r == UserRoles.SystemAdministrator);
        }

        public static Expression<Func<FormEntity, bool>> HasOwnerPermissionExpr(Guid userId)
        {
            return f => f.OwnerId == userId;
        }

        public static Expression<Func<FormEntity, bool>> HasPermissionToSeeFormExpr(Guid userId)
        {
            return f => f.Status == FormStatus.Published
                || f.OwnerId == userId;
        }

        public static Expression<Func<FormEntity, bool>> AvailableForImportToCourseExpr(Guid userId)
        {
            return f => f.Status == FormStatus.Published && f.OwnerId == userId;
        }
    }
}
