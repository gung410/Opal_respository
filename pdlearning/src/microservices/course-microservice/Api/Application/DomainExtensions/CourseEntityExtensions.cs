using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.DomainExtensions
{
    public static class CourseEntityExtensions
    {
        public static Expression<Func<CourseEntity, bool>> HasContentCreatorPermissionQueryExpr(
            Guid? userId,
            IEnumerable<string> userRoles)
        {
            return x => userId == null
                        || UserRoles.IsSysAdministrator(userRoles)
                        || EF.Functions.Contains(x.CollaborativeContentCreatorIdsFullTextSearch, userId.ToString())
                        || x.CreatedBy == userId;
        }

        public static Expression<Func<CourseEntity, bool>> HasViewCourseOrClassRunPermissionQueryExpr(
            Guid? userId,
            IEnumerable<string> userRoles,
            Func<string, bool> checkHasPermissionFn)
        {
            Expression<Func<CourseEntity, bool>> expr1 = x =>
                userId == null ||
                UserRoles.IsSysAdministrator(userRoles) ||
                (checkHasPermissionFn(CourseAdminManagementPermissionKeys.ViewCourseList) &&
                 (x.FirstAdministratorId == userId ||
                  x.SecondAdministratorId == userId ||
                  x.PrimaryApprovingOfficerId == userId ||
                  x.AlternativeApprovingOfficerId == userId));
            return expr1.Or(HasContentCreatorPermissionQueryExpr(userId, userRoles));
        }

        public static Expression<Func<CourseEntity, bool>> HasViewContentPermissionQueryExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x =>
                userId == null ||
                UserRoles.IsSysAdministrator(userRoles) ||
                (x.CreatedBy == userId
                 || x.FirstAdministratorId == userId
                 || x.SecondAdministratorId == userId
                 || x.PrimaryApprovingOfficerId == userId
                 || x.AlternativeApprovingOfficerId == userId
                 || EF.Functions.Contains(x.CollaborativeContentCreatorIdsFullTextSearch, userId.ToString())
                 || EF.Functions.Contains(x.CourseCoFacilitatorIdsFullTextSearch, userId.ToString())
                 || EF.Functions.Contains(x.CourseFacilitatorIdsFullTextSearch, userId.ToString()));
        }

        public static Expression<Func<CourseEntity, bool>> HasViewSessionPermissionQueryExpr(Guid? userId, IEnumerable<string> userRoles, Func<string, bool> checkHasPermissionFn)
        {
            return x => userId == null
                        || UserRoles.IsSysAdministrator(userRoles)
                        || (checkHasPermissionFn(CourseAdminManagementPermissionKeys.ViewSessionList)
                            && ((x.CreatedBy == userId || x.FirstAdministratorId == userId || x.SecondAdministratorId == userId || x.PrimaryApprovingOfficerId == userId || x.AlternativeApprovingOfficerId == userId)
                                || EF.Functions.Contains(x.CollaborativeContentCreatorIdsFullTextSearch, userId.ToString())
                                || EF.Functions.Contains(x.CourseCoFacilitatorIdsFullTextSearch, userId.ToString())
                                || EF.Functions.Contains(x.CourseFacilitatorIdsFullTextSearch, userId.ToString())));
        }

        // course CCC can view in CAM and LMM
        public static Expression<Func<CourseEntity, bool>> CanCopyDataQueryExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x =>
                userId == null ||
                UserRoles.IsSysAdministrator(userRoles) ||
                (x.CreatedBy == userId
                 || x.FirstAdministratorId == userId
                 || x.SecondAdministratorId == userId
                 || x.PrimaryApprovingOfficerId == userId
                 || x.AlternativeApprovingOfficerId == userId
                 || ((EF.Functions.Contains(x.CollaborativeContentCreatorIdsFullTextSearch, userId.ToString())
                        || EF.Functions.Contains(x.CourseCoFacilitatorIdsFullTextSearch, userId.ToString())
                        || EF.Functions.Contains(x.CourseFacilitatorIdsFullTextSearch, userId.ToString()))
                     && (x.Status == CourseStatus.Approved
                         || x.Status == CourseStatus.PlanningCycleVerified
                         || x.Status == CourseStatus.PlanningCycleCompleted
                         || x.Status == CourseStatus.Published
                         || x.Status == CourseStatus.Unpublished
                         || x.Status == CourseStatus.Completed)));
        }
    }
}
