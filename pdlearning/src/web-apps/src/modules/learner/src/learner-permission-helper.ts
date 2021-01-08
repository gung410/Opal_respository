import { LEARNER_PERMISSIONS, LearnerRoutePaths } from '@opal20/domain-components';

import { UserInfoModel } from '@opal20/domain-api';
export class LearnerPermissionHelper {
  public static hasPermissionToAccessRoute(routePath: LearnerRoutePaths | string): boolean {
    const learnerRoutePath = routePath as LearnerRoutePaths;
    if (!ROUTES_PERMISSION_KEYS_MAP.has(learnerRoutePath)) {
      // case detail deeplink
      return true;
    }
    return UserInfoModel.getMyUserInfo().hasPermission(ROUTES_PERMISSION_KEYS_MAP.get(learnerRoutePath));
  }
}

export const RETURNED_ROUTE_PRIORITY = [
  LearnerRoutePaths.Home,
  LearnerRoutePaths.MyLearning,
  LearnerRoutePaths.Catalogue,
  LearnerRoutePaths.Calendar,
  LearnerRoutePaths.PdPlan,
  LearnerRoutePaths.EPortfolio,
  LearnerRoutePaths.ReportsPage
];

const ROUTES_PERMISSION_KEYS_MAP: Map<LearnerRoutePaths, string> = new Map<LearnerRoutePaths, string>([
  [LearnerRoutePaths.Home, LEARNER_PERMISSIONS.Home],
  [LearnerRoutePaths.MyLearning, LEARNER_PERMISSIONS.MyLearning],
  [LearnerRoutePaths.Catalogue, LEARNER_PERMISSIONS.Catalogue],
  [LearnerRoutePaths.Calendar, LEARNER_PERMISSIONS.Calendar],
  [LearnerRoutePaths.PdPlan, LEARNER_PERMISSIONS.PDPlan],
  [LearnerRoutePaths.EPortfolio, LEARNER_PERMISSIONS.EPortfolio],
  [LearnerRoutePaths.ReportsPage, LEARNER_PERMISSIONS.Report]
]);
