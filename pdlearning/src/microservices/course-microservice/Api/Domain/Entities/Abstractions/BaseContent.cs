using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Domain.Interfaces;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities.Abstractions
{
    public abstract class BaseContent : FullAuditedEntity, ISoftDelete, IContent
    {
        public bool IsDeleted { get; set; }

        public bool HasModifyPermission(
            Guid? currentUserId,
            List<string> currentUserRoles,
            CourseEntity course,
            Func<CourseEntity, bool> haveCourseFullRight,
            Func<string, bool> checkHasPermissionFn,
            ClassRun forClassRun = null)
        {
            return currentUserId == null ||
                   UserRoles.IsSysAdministrator(currentUserRoles) ||
                   (course.HasModifyCourseContentPermission(currentUserId, currentUserRoles, haveCourseFullRight, checkHasPermissionFn) ||
                    (forClassRun != null && forClassRun.HasFacilitatorPermission(currentUserId, currentUserRoles, haveCourseFullRight(course))));
        }

        public abstract Guid ForTargetId();
    }

    public abstract class BaseOrderableContent : BaseContent, IOrderable
    {
        public int? Order { get; set; }
    }
}
