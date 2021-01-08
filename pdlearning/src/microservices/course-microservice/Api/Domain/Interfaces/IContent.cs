using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Domain.Interfaces
{
    public interface IContent
    {
        Guid ForTargetId();

        bool HasModifyPermission(
            Guid? currentUserId,
            List<string> currentUserRoles,
            CourseEntity course,
            Func<CourseEntity, bool> haveCourseFullRight,
            Func<string, bool> checkHasPermissionFn,
            ClassRun forClassRun = null);
    }
}
