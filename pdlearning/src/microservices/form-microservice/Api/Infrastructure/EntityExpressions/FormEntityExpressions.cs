using System;
using System.Linq.Expressions;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Helpers;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Infrastructure
{
    public class FormEntityExpressions
    {
        public static Expression<Func<FormEntity, bool>> HasOwnerPermissionExpr(Guid userId)
        {
            return f => f.OwnerId == userId;
        }

        public static Expression<Func<FormEntity, bool>> HasApprovalPermissionExpr(Guid userId)
        {
            return f => f.PrimaryApprovingOfficerId == userId
                || f.AlternativeApprovingOfficerId == userId;
        }

        public static Expression<Func<FormEntity, bool>> HasOwnerOrApprovalPermissionExpr(Guid userId)
        {
            return f => f.OwnerId == userId
                || f.PrimaryApprovingOfficerId == userId
                || f.AlternativeApprovingOfficerId == userId;
        }

        public static Expression<Func<FormEntity, bool>> HasPermissionToSeeFormExpr(Guid userId)
        {
            return f => f.Status == FormStatus.Published
                || f.Status == FormStatus.ReadyToUse
                || f.Status == FormStatus.Approved
                || f.OwnerId == userId
                || f.PrimaryApprovingOfficerId == userId
                || f.AlternativeApprovingOfficerId == userId;
        }

        public static Expression<Func<FormEntity, bool>> AvailableForImportToCourseExpr(Guid userId)
        {
            return f => f.Status == FormStatus.Published
                        || (f.Status == FormStatus.ReadyToUse
                        && (f.OwnerId == userId
                            || f.PrimaryApprovingOfficerId == userId
                            || f.AlternativeApprovingOfficerId == userId));
        }
    }
}
