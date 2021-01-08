import { LMMRoutePaths, LMMTabConfiguration, RouterPageInput } from '@opal20/domain-components';

export const NAVIGATORS: { [id: string]: RouterPageInput<unknown, unknown, unknown> } = {
  [LMMRoutePaths.CourseManagementPage]: {
    activeTab: LMMTabConfiguration.CoursesTab,
    path: LMMRoutePaths.CourseManagementPage
  },
  [LMMRoutePaths.LearningPathManagementPage]: {
    activeTab: LMMTabConfiguration.LearningPathsTab,
    path: LMMRoutePaths.LearningPathManagementPage
  },
  [LMMRoutePaths.CourseDetailPage]: {
    activeTab: LMMTabConfiguration.CourseInfoTab,
    path: LMMRoutePaths.CourseDetailPage
  },
  [LMMRoutePaths.ClassRunDetailPage]: {
    activeTab: LMMTabConfiguration.ClassRunInfoTab,
    path: LMMRoutePaths.ClassRunDetailPage
  },
  [LMMRoutePaths.SessionDetailPage]: {
    activeTab: LMMTabConfiguration.SessionInfoTab,
    path: LMMRoutePaths.SessionDetailPage
  },
  [LMMRoutePaths.AssignmentDetailPage]: {
    activeTab: LMMTabConfiguration.AssignmentInfoTab,
    path: LMMRoutePaths.AssignmentDetailPage
  },
  [LMMRoutePaths.LearningPathDetailPage]: {
    activeTab: LMMTabConfiguration.LearningPathInfoTab,
    path: LMMRoutePaths.LearningPathDetailPage
  },
  [LMMRoutePaths.LearnerProfilePage]: {
    activeTab: LMMTabConfiguration.PersonalInfoTab,
    path: LMMRoutePaths.LearnerProfilePage
  },
  [LMMRoutePaths.ParticipantAssignmentTrackPage]: {
    activeTab: LMMTabConfiguration.LearnerAssignmentAnswerTab,
    path: LMMRoutePaths.ParticipantAssignmentTrackPage
  },
  [LMMRoutePaths.ReportsPage]: {
    path: LMMRoutePaths.ReportsPage
  },
  [LMMRoutePaths.DigitalBadgesManagementPage]: {
    activeTab: LMMTabConfiguration.DigitalLearnerTab,
    path: LMMRoutePaths.DigitalBadgesManagementPage
  }
};
