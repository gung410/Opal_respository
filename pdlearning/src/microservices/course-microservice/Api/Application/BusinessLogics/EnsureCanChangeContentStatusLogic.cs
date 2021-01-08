using System;
using System.Collections.Generic;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.BusinessLogics
{
    public class EnsureCanChangeContentStatusLogic : BaseBusinessLogic
    {
        public EnsureCanChangeContentStatusLogic(IUserContext userContext) : base(userContext)
        {
        }

        public void EnsureForCourse(CourseEntity course, ContentStatus newStatus, Func<CourseEntity, bool> haveFullRight)
        {
            Ensure(course, null, newStatus, haveFullRight);
        }

        public void EnsureForClassrun(CourseEntity course, ClassRun classRun, ContentStatus newStatus, Func<CourseEntity, bool> haveFullRight)
        {
            Ensure(course, classRun, newStatus, haveFullRight);
        }

        private void Ensure(
            CourseEntity course,
            ClassRun classRun,
            ContentStatus newStatus,
            Func<CourseEntity, bool> haveFullRight)
        {
            switch (newStatus)
            {
                case ContentStatus.Approved:
                case ContentStatus.Rejected:
                    {
                        EnsureValidPermission(course.HasApproveRejectCourseContentPermission(CurrentUserId, CurrentUserRoles, haveFullRight, x => HasPermissionPrefix(x)));
                        EnsureBusinessLogicValid(classRun != null ? classRun.ValidateCanApproveRejectContent(course) : course.ValidateCanApproveRejectContent());
                        break;
                    }

                case ContentStatus.Published:
                    {
                        EnsureValidPermission(course.HasPublishContentPermission(CurrentUserId, CurrentUserRoles, haveFullRight, x => HasPermissionPrefix(x)));
                        EnsureBusinessLogicValid(classRun != null ? classRun.ValidateCanPublishContent(course) : course.ValidateCanPublishContent());
                        break;
                    }

                case ContentStatus.Unpublished:
                    {
                        EnsureValidPermission(course.HasPublishContentPermission(CurrentUserId, CurrentUserRoles, haveFullRight, x => HasPermissionPrefix(x)));
                        EnsureBusinessLogicValid(classRun != null ? classRun.ValidateCanUnpublishContent(course) : course.ValidateCanUnpublishContent());
                        break;
                    }

                default:
                    {
                        EnsureBusinessLogicValid(course.ValidateNotArchived());
                        break;
                    }
            }
        }
    }
}
