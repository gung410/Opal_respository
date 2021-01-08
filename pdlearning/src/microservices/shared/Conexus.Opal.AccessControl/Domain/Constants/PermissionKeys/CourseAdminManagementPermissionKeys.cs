using System;
using System.Collections.Generic;
using System.Text;

namespace Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys
{
    public static class CourseAdminManagementPermissionKeys
    {
        public static readonly string ViewCourseList = "CAM.CourseAdministration-Courses";
        public static readonly string ViewCourseDetail = "CAM.CourseAdministration-Courses.CourseDetail";
        public static readonly string ViewClassRunList = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns";
        public static readonly string ViewClassRunDetail = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail";
        public static readonly string ViewSessionList = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail.Sessions";
        public static readonly string ViewSessionDetail = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail.Sessions.SessionDetail";
        public static readonly string CreateEditCourse = "CAM.CourseAdministration-Courses.CourseDetail-Modify";
        public static readonly string PublishUnpublishCourse = "CAM.CourseAdministration-Courses.CourseDetail-Publish";
        public static readonly string CreateEditClassRun = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Modify";
        public static readonly string PublishUnpublishClassRun = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Publish";
        public static readonly string CreateEditSession = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Sessions.Detail-Modify";
        public static readonly string CancelClassRun = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Cancel";
        public static readonly string RescheduleClassRun = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Reschedule";
        public static readonly string CourseApproval = "CAM.CourseAdministration-CoursesPending.CourseDetail";
        public static readonly string CancellationClassRunRequestApproval = "CAM.CourseAdministration-CoursesWithClassesPending.CourseDetail-ClassRuns-CancellationRequest.ClassRunDetail";
        public static readonly string RescheduledClassRunRequestApproval = "CAM.CourseAdministration-CoursesWithClassesPending.CourseDetail-ClassRuns-RescheduledRequest.ClassRunDetail";
        public static readonly string ViewRegistrations = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-ViewRegistrations";
        public static readonly string ManageRegistrations = "CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-ManageRegistrations";
        public static readonly string Reports = "CAM.Reports";
    }
}
