export const CAM_PERMISSIONS = {
  CourseAdministration: 'CAM.CourseAdministration',
  ViewCourseList: 'CAM.CourseAdministration-Courses',
  ViewCourseDetail: 'CAM.CourseAdministration-Courses.CourseDetail',
  ViewClassRunList: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns',
  ViewClassRunDetail: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail',
  ViewSessionList: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail.Sessions',
  ViewSessionDetail: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail.Sessions.SessionDetail',
  CreateEditCourse: 'CAM.CourseAdministration-Courses.CourseDetail-Modify',
  PublishUnpublishCourse: 'CAM.CourseAdministration-Courses.CourseDetail-Publish',
  CreateEditClassRun: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Modify',
  PublishUnpublishClassRun: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Publish',
  CreateEditSession: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Sessions.Detail-Modify',
  CancelClassRun: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Cancel',
  RescheduleClassRun: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-Reschedule',
  CourseApproval: 'CAM.CourseAdministration-CoursesPending.CourseDetail',
  CancellationClassRunRequestApproval:
    'CAM.CourseAdministration-CoursesWithClassesPending.CourseDetail-ClassRuns-CancellationRequest.ClassRunDetail',
  RescheduledClassRunRequestApproval:
    'CAM.CourseAdministration-CoursesWithClassesPending.CourseDetail-ClassRuns-RescheduledRequest.ClassRunDetail',
  // View list of registration list/withdrawal request/waitlist/participant list
  ViewRegistrations: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-ViewRegistrations',
  // Action on registration list/withdrawal request/waitlist/participant list
  ManageRegistrations: 'CAM.CourseAdministration-Courses.CourseDetail-ClassRuns.ClassRunDetail-ManageRegistrations',
  // View, export statistical report
  Reports: 'CAM.Reports'
};
