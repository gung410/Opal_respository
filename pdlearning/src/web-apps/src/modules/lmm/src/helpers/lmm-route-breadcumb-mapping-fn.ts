import { BreadcrumbItem, LMMRoutePaths, RouterPageInput, RouterPageInputExt } from '@opal20/domain-components';

export function LMM_ROUTE_BREADCUMB_MAPPING_FN(
  currentRoute: RouterPageInput<unknown, unknown, unknown>,
  navigate: (route: RouterPageInput<unknown, unknown, unknown>) => void,
  combineWithRouteBreakcumbMapping: Dictionary<BreadcrumbItem> = {}
): Dictionary<BreadcrumbItem> {
  return {
    [LMMRoutePaths.CourseManagementPage]: {
      text: 'Learning Management',
      navigationFn: navigationFnBuilder(LMMRoutePaths.CourseManagementPage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.CourseManagementPage]
    },
    [LMMRoutePaths.LearningPathManagementPage]: {
      text: 'Learning Path Administration',
      navigationFn: navigationFnBuilder(LMMRoutePaths.LearningPathManagementPage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.LearningPathManagementPage]
    },
    [LMMRoutePaths.CourseDetailPage]: {
      iconClass: 'hat' + iconClassSelectedSuffix(LMMRoutePaths.CourseDetailPage),
      navigationFn: navigationFnBuilder(LMMRoutePaths.CourseDetailPage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.CourseDetailPage]
    },
    [LMMRoutePaths.ClassRunDetailPage]: {
      iconClass: 'class-run' + iconClassSelectedSuffix(LMMRoutePaths.ClassRunDetailPage),
      navigationFn: navigationFnBuilder(LMMRoutePaths.ClassRunDetailPage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.ClassRunDetailPage]
    },
    [LMMRoutePaths.SessionDetailPage]: {
      iconClass: 'session' + iconClassSelectedSuffix(LMMRoutePaths.SessionDetailPage),
      navigationFn: navigationFnBuilder(LMMRoutePaths.SessionDetailPage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.SessionDetailPage]
    },
    [LMMRoutePaths.AssignmentDetailPage]: {
      navigationFn: navigationFnBuilder(LMMRoutePaths.AssignmentDetailPage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.AssignmentDetailPage]
    },
    [LMMRoutePaths.LearningPathDetailPage]: {
      navigationFn: navigationFnBuilder(LMMRoutePaths.LearningPathDetailPage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.LearningPathDetailPage]
    },
    [LMMRoutePaths.LearnerProfilePage]: {
      iconClass: 'user' + iconClassSelectedSuffix(LMMRoutePaths.LearnerProfilePage),
      navigationFn: navigationFnBuilder(LMMRoutePaths.LearnerProfilePage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.LearnerProfilePage]
    },
    [LMMRoutePaths.ParticipantAssignmentTrackPage]: {
      iconClass: 'user' + iconClassSelectedSuffix(LMMRoutePaths.ParticipantAssignmentTrackPage),
      navigationFn: navigationFnBuilder(LMMRoutePaths.ParticipantAssignmentTrackPage),
      ...combineWithRouteBreakcumbMapping[LMMRoutePaths.ParticipantAssignmentTrackPage]
    }
  };

  function iconClassSelectedSuffix(forPath: string): string {
    return forPath === currentRoute.path ? '-selected' : '';
  }

  function navigationFnBuilder(forPath: string): (() => void) | null {
    if (currentRoute.path === forPath) {
      return null;
    }
    return () => {
      const forPathRoute = RouterPageInputExt.findRouteInTreeByPath(currentRoute, forPath);
      if (forPathRoute != null) {
        navigate(forPathRoute);
      }
    };
  }
}
