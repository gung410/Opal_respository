export const LMM_PERMISSIONS = {
  // Basic
  LearningManagement: 'LMM.LearningManagement',
  ViewCourseList: 'LMM.LearningManagement-Courses',
  ViewCourseDetail: 'LMM.LearningManagement-Courses.CourseDetail',
  // Course Content
  ApproveRejectCourseContent: 'LMM.LearningManagement-PendingApproval.CourseDetail-Content-Approve',
  CreateCourseContent: 'LMM.LearningManagement-Courses.CourseDetail-Content-AddContent',
  EditDeleteMineCourseContent: 'LMM.LearningManagement-Courses.CourseDetail-Content-MineModify',
  EditDeleteOthersCourseContent: 'LMM.LearningManagement-Courses.CourseDetail-Content-OthersModify',
  PublishCourseContent: 'LMM.LearningManagement-Courses.CourseDetail-Content-Publish',
  AllowDownloadCourseContent: 'LMM.LearningManagement-Courses.CourseDetail-Content-AllowDownload',
  ViewQuizStatistics: 'LMM.LearningManagement-Courses.CourseDetail-Content-QuizStatistics',
  // Assignment
  ViewAnswerDoneAssignment: 'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail',
  ViewCommentFeedbackAssignment:
    'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees.Comments',
  ViewLearnerAssignmentTrack: 'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees',
  // Action on Assignment
  ManageAssignments: 'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees.Score',
  CreateAssignment: 'LMM.LearningManagement-Courses.CourseDetail-Content-AddAssignment',
  AssignAssignment: 'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees',
  ScoreGivingAssignment:
    'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees.Score',
  // Track Learner's Learning Progress
  ViewParticipantTab: 'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Participants',
  ViewCompletionRate: 'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Participants.Participant-CompletionRate',
  // View Course's Effectiveness
  ViewPostCourseEvaluation: 'LMM.LearningManagement-Courses.CourseDetail-PreviewPostCourse',
  // Attendance Tracking
  GetSessionCode: 'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-AttendanceTracking.AttendanceTracking-SessionCode',
  SetPresentAbsent:
    'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-AttendanceTracking.AttendanceTracking-SetPresentAbsent',
  ViewReasonForAbsent:
    'LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-AttendanceTracking.AttendanceTracking-ReasonForAbsent',
  // Learning Path
  ViewLearningPathDetail: 'LMM.LearningPathAdministration-LearningPaths.LearningPathDetail',
  CreateEditPublishUnpublishLP: 'LMM.LearningPathAdministration-LearningPaths.LearningPathDetail-Modify',
  PopulateMetadataLearningPath: 'LMM.LearningPathAdministration-LearningPaths.LearningPathDetail-Populate',
  CopyHyperLinkLearningPathButton: 'LMM.LearningPathAdministration-LearningPaths.LearningPathDetail-HyperLink'
};
