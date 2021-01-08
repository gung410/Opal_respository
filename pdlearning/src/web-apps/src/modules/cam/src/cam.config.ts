import { CAMRoutePaths, CAMTabConfiguration, RouterPageInput } from '@opal20/domain-components';

export const NAVIGATORS: Dictionary<RouterPageInput<unknown, unknown, unknown>> = {
  [CAMRoutePaths.CourseManagementPage]: {
    activeTab: CAMTabConfiguration.CoursesTab,
    path: CAMRoutePaths.CourseManagementPage
  },
  [CAMRoutePaths.CourseDetailPage]: {
    activeTab: CAMTabConfiguration.CourseInfoTab,
    path: CAMRoutePaths.CourseDetailPage
  },
  [CAMRoutePaths.ClassRunDetailPage]: {
    activeTab: CAMTabConfiguration.ClassRunInfoTab,
    path: CAMRoutePaths.ClassRunDetailPage
  },
  [CAMRoutePaths.SessionDetailPage]: {
    activeTab: CAMTabConfiguration.SessionInfoTab,
    path: CAMRoutePaths.SessionDetailPage
  },
  [CAMRoutePaths.LearnerProfilePage]: {
    activeTab: CAMTabConfiguration.PersonalInfoTab,
    path: CAMRoutePaths.LearnerProfilePage
  },
  [CAMRoutePaths.CoursePlanningPage]: {
    activeTab: CAMTabConfiguration.PlanningCycleTab,
    path: CAMRoutePaths.CoursePlanningPage
  },
  [CAMRoutePaths.CoursePlanningCycleDetailPage]: {
    activeTab: CAMTabConfiguration.CoursePlanningCycleInfoTab,
    path: CAMRoutePaths.CoursePlanningCycleDetailPage
  },
  [CAMRoutePaths.ReportsPage]: {
    path: CAMRoutePaths.ReportsPage
  },
  [CAMRoutePaths.BlockoutDateDetailPage]: {
    path: CAMRoutePaths.BlockoutDateDetailPage
  },
  [CAMRoutePaths.ECertificateManagementPage]: {
    activeTab: CAMTabConfiguration.AllECertificateTab,
    path: CAMRoutePaths.ECertificateManagementPage
  },
  [CAMRoutePaths.ECertificateTemplateDetailPage]: {
    activeTab: CAMTabConfiguration.ECertificateInfoTab,
    path: CAMRoutePaths.ECertificateTemplateDetailPage
  }
};
