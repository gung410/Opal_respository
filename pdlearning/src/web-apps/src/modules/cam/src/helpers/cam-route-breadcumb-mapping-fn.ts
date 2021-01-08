import { BreadcrumbItem, CAMRoutePaths, RouterPageInput, RouterPageInputExt } from '@opal20/domain-components';

export function CAM_ROUTE_BREADCUMB_MAPPING_FN(
  currentRoute: RouterPageInput<unknown, unknown, unknown>,
  navigate: (route: RouterPageInput<unknown, unknown, unknown>) => void,
  combineWithRouteBreakcumbMapping: Dictionary<BreadcrumbItem> = {}
): Dictionary<BreadcrumbItem> {
  return {
    [CAMRoutePaths.CoursePlanningPage]: {
      text: 'Course Planning',
      navigationFn: navigationFnBuilder(CAMRoutePaths.CoursePlanningPage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.CoursePlanningPage]
    },
    [CAMRoutePaths.CourseManagementPage]: {
      text: 'Course Administration',
      navigationFn: navigationFnBuilder(CAMRoutePaths.CourseManagementPage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.CourseManagementPage]
    },
    [CAMRoutePaths.CourseDetailPage]: {
      iconClass: 'hat' + iconClassSelectedSuffix(CAMRoutePaths.CourseDetailPage),
      navigationFn: navigationFnBuilder(CAMRoutePaths.CourseDetailPage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.CourseDetailPage]
    },
    [CAMRoutePaths.ClassRunDetailPage]: {
      iconClass: 'class-run' + iconClassSelectedSuffix(CAMRoutePaths.ClassRunDetailPage),
      navigationFn: navigationFnBuilder(CAMRoutePaths.ClassRunDetailPage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.ClassRunDetailPage]
    },
    [CAMRoutePaths.SessionDetailPage]: {
      iconClass: 'session' + iconClassSelectedSuffix(CAMRoutePaths.SessionDetailPage),
      navigationFn: navigationFnBuilder(CAMRoutePaths.SessionDetailPage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.SessionDetailPage]
    },
    [CAMRoutePaths.LearnerProfilePage]: {
      iconClass: 'user' + iconClassSelectedSuffix(CAMRoutePaths.LearnerProfilePage),
      navigationFn: navigationFnBuilder(CAMRoutePaths.LearnerProfilePage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.LearnerProfilePage]
    },
    [CAMRoutePaths.CoursePlanningCycleDetailPage]: {
      navigationFn: navigationFnBuilder(CAMRoutePaths.CoursePlanningCycleDetailPage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.CoursePlanningCycleDetailPage]
    },
    [CAMRoutePaths.BlockoutDateDetailPage]: {
      navigationFn: navigationFnBuilder(CAMRoutePaths.BlockoutDateDetailPage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.BlockoutDateDetailPage]
    },
    [CAMRoutePaths.ECertificateTemplateDetailPage]: {
      navigationFn: navigationFnBuilder(CAMRoutePaths.ECertificateTemplateDetailPage),
      ...combineWithRouteBreakcumbMapping[CAMRoutePaths.ECertificateTemplateDetailPage]
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
