using System;
using System.Linq.Expressions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;

namespace Microservice.Content.Infrastructure
{
    public class DigitalContentExpressions
    {
        public static Expression<Func<T, bool>> HasOwnerOrApprovalPermissionExpr<T>(Guid userId)
            where T : DigitalContent
        {
            return dc => dc.OwnerId == userId
                || dc.PrimaryApprovingOfficerId == userId
                || dc.AlternativeApprovingOfficerId == userId;
        }

        public static Expression<Func<T, bool>> HasPermissionToSeeContentExpr<T>(Guid userId)
            where T : DigitalContent
        {
            return dc => dc.Status == DigitalContentStatus.Published
            || dc.Status == DigitalContentStatus.ReadyToUse
            || dc.OwnerId == userId
            || dc.PrimaryApprovingOfficerId == userId
            || dc.AlternativeApprovingOfficerId == userId;
        }

        public static Expression<Func<DigitalContent, bool>> HasOwnerPermissionExpr(Guid userId)
        {
            return dc => dc.OwnerId == userId;
        }

        public static Expression<Func<DigitalContent, bool>> HasApprovalPermissionExpr(Guid userId)
        {
            return dc => dc.PrimaryApprovingOfficerId == userId
            || dc.AlternativeApprovingOfficerId == userId;
        }

        public static Expression<Func<DigitalContent, bool>> HasOwnerOrApprovalPermissionExpr(Guid userId)
        {
            return HasOwnerOrApprovalPermissionExpr<DigitalContent>(userId);
        }

        public static Expression<Func<DigitalContent, bool>> HasPermissionToSeeContentExpr(Guid userId)
        {
            return HasPermissionToSeeContentExpr<DigitalContent>(userId);
        }

        public static Expression<Func<LearningContent, bool>> HasPermissionToSeeLearningContentExpr(Guid userId)
        {
            return HasPermissionToSeeContentExpr<LearningContent>(userId);
        }

        public static Expression<Func<UploadedContent, bool>> HasPermissionToSeeUploadedContentExpr(Guid userId)
        {
            return HasPermissionToSeeContentExpr<UploadedContent>(userId);
        }

        public static Expression<Func<DigitalContent, bool>> AvailableForImportToCourseExpr(Guid userId)
        {
            return dc => dc.Status == DigitalContentStatus.Published
                         || dc.Status == DigitalContentStatus.Approved
                         || (
                             dc.Status == DigitalContentStatus.ReadyToUse
                             && (dc.OwnerId == userId
                                 || dc.PrimaryApprovingOfficerId == userId
                                 || dc.AlternativeApprovingOfficerId == userId));
        }
    }
}
