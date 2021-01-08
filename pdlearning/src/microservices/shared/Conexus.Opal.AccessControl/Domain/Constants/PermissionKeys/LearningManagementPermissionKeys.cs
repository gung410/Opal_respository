using System;
using System.Collections.Generic;
using System.Text;

namespace Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys
{
    public static class LearningManagementPermissionKeys
    {
        // Course Content
        public static readonly string ApproveRejectCourseContent = "LMM.LearningManagement-PendingApproval.CourseDetail-Content-Approve"; // Checked
        public static readonly string CreateCourseContent = "LMM.LearningManagement-Courses.CourseDetail-Content-AddContent"; // Checked
        public static readonly string EditDeleteMineCourseContent = "LMM.LearningManagement-Courses.CourseDetail-Content-MineModify"; // Checked
        public static readonly string EditDeleteOthersCourseContent = "LMM.LearningManagement-Courses.CourseDetail-Content-OthersModify"; // Checked
        public static readonly string PublishCourseContent = "LMM.LearningManagement-Courses.CourseDetail-Content-Publish"; // Checked
        public static readonly string AllowDownloadCourseContent = "LMM.LearningManagement-Courses.CourseDetail-Content-AllowDownload"; // Check In Client
        public static readonly string ViewQuizStatistics = "LMM.LearningManagement-Courses.CourseDetail-Content-QuizStatistics"; // Check In Client

        // Assignment
        public static readonly string CreateAssignment = "LMM.LearningManagement-Courses.CourseDetail-Content-AddAssignment"; // Checked
        public static readonly string AssignAssignment = "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees"; // Checked
        public static readonly string ScoreGivingAssignment =
            "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees.Score";

        public static readonly string ViewAnswerDoneAssignment = "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail"; // Check In Client

        public static readonly string ViewCommentFeedbackAssignment =
            "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees.Comments"; // Check In Client

        public static readonly string ViewLearnerAssignmentTrack = "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Assignments.AssignmentDetail-Assignees"; // Check In Client

        // Track Learner"s Learning Progress
        public static readonly string ViewParticipantTab = "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Participants"; // Check In Client
        public static readonly string ViewCompletionRate = "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-Participants.Participant-CompletionRate"; // Check In Client

        // View Course"s Effectiveness
        public static readonly string ViewPostCourseEvaluation = "LMM.LearningManagement-Courses.CourseDetail-PreviewPostCourse"; // Check In Client

        // Attendance Tracking
        public static readonly string GetSessionCode = "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-AttendanceTracking.AttendanceTracking-SessionCode";

        public static readonly string SetPresentAbsent =
            "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-AttendanceTracking.AttendanceTracking-SetPresentAbsent";

        public static readonly string ViewReasonForAbsent =
            "LMM.LearningManagement-Courses.CourseDetail-ClassRuns.ClassRunDetail-AttendanceTracking.AttendanceTracking-ReasonForAbsent"; // Client Checked

        // Learning Path
        public static readonly string ViewLearningPathDetail = "LMM.LearningPathAdministration-LearningPaths.LearningPathDetail"; // Detail -> CAM not do
        public static readonly string CreateEditPublishUnpublishLP = "LMM.LearningPathAdministration-LearningPaths.LearningPathDetail-Modify"; // Done
        public static readonly string PopulateMetadataLearningPath = "LMM.LearningPathAdministration-LearningPaths.LearningPathDetail-Populate"; // Client Checked

        public static readonly string CopyHyperLinkLearningPathButton =
              "LMM.LearningPathAdministration-LearningPaths.LearningPathDetail-HyperLink"; // Client Checked
    }
}
