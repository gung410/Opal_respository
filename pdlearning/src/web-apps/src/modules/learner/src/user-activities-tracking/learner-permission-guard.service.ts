import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { LearnerPermissionHelper, RETURNED_ROUTE_PRIORITY } from '../learner-permission-helper';

import { Injectable } from '@angular/core';
import { NavigationMenuService } from '@opal20/domain-components';
import { NavigationService } from '@opal20/infrastructure';

@Injectable()
export class LearnerPermissionGuardService implements CanActivate {
  constructor(private navigationService: NavigationService, private navigationMenuService: NavigationMenuService) {}

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const firstUrlSegment = route.url[0];
    if (firstUrlSegment != null && LearnerPermissionHelper.hasPermissionToAccessRoute(firstUrlSegment.path)) {
      return true;
    }
    const accessibleRoute = RETURNED_ROUTE_PRIORITY.find(p => LearnerPermissionHelper.hasPermissionToAccessRoute(p));
    if (accessibleRoute) {
      this.navigationMenuService.activate(accessibleRoute);
      this.navigationService.navigateTo(accessibleRoute);
    }
    return false;
  }
}
