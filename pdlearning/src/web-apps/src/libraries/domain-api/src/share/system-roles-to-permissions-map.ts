import { CAM_PERMISSIONS } from './permission-keys/cam-permission-key';
import { LMM_PERMISSIONS } from './permission-keys/lmm-permission-key';
import { SystemRoleEnum } from './models/user-info.model';

export const ROLE_TO_PERMISSIONS: Dictionary<string[]> = {
  [SystemRoleEnum.CourseFacilitator]: [
    LMM_PERMISSIONS.CreateCourseContent,
    LMM_PERMISSIONS.ManageAssignments,
    LMM_PERMISSIONS.ViewAnswerDoneAssignment,
    LMM_PERMISSIONS.ViewCommentFeedbackAssignment,
    LMM_PERMISSIONS.ViewLearnerAssignmentTrack,
    LMM_PERMISSIONS.ViewParticipantTab,
    LMM_PERMISSIONS.ViewCompletionRate,
    LMM_PERMISSIONS.ViewPostCourseEvaluation
  ],
  [SystemRoleEnum.CourseAdministrator]: [
    CAM_PERMISSIONS.ViewSessionList,
    CAM_PERMISSIONS.ViewSessionDetail,
    CAM_PERMISSIONS.PublishUnpublishClassRun,
    CAM_PERMISSIONS.CreateEditSession,
    CAM_PERMISSIONS.CancelClassRun,
    CAM_PERMISSIONS.RescheduleClassRun,
    CAM_PERMISSIONS.ViewRegistrations,
    CAM_PERMISSIONS.ManageRegistrations,
    CAM_PERMISSIONS.Reports,
    LMM_PERMISSIONS.ViewQuizStatistics,
    LMM_PERMISSIONS.ViewAnswerDoneAssignment,
    LMM_PERMISSIONS.ViewParticipantTab,
    LMM_PERMISSIONS.ViewCompletionRate,
    LMM_PERMISSIONS.ViewPostCourseEvaluation
  ],
  [SystemRoleEnum.CourseContentCreator]: [
    CAM_PERMISSIONS.ViewSessionList,
    CAM_PERMISSIONS.ViewSessionDetail,
    CAM_PERMISSIONS.CreateEditCourse,
    CAM_PERMISSIONS.PublishUnpublishCourse,
    CAM_PERMISSIONS.ViewRegistrations,
    LMM_PERMISSIONS.CreateCourseContent,
    LMM_PERMISSIONS.EditDeleteMineCourseContent,
    LMM_PERMISSIONS.PublishCourseContent,
    LMM_PERMISSIONS.AllowDownloadCourseContent,
    LMM_PERMISSIONS.ManageAssignments,
    LMM_PERMISSIONS.ViewAnswerDoneAssignment,
    LMM_PERMISSIONS.ViewCommentFeedbackAssignment,
    LMM_PERMISSIONS.ViewLearnerAssignmentTrack,
    LMM_PERMISSIONS.ViewParticipantTab,
    LMM_PERMISSIONS.ViewCompletionRate,
    LMM_PERMISSIONS.ViewPostCourseEvaluation,
    LMM_PERMISSIONS.ViewLearningPathDetail,
    LMM_PERMISSIONS.CreateEditPublishUnpublishLP
  ]
};
