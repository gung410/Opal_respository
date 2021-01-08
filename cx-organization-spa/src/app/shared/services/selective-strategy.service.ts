import { Injectable } from '@angular/core';
import { PreloadingStrategy, Route } from '@angular/router';
import { Observable, of } from 'rxjs';

import { RouteConfigService } from './route-config.service';

@Injectable()
/**
 * This service class will check and load all the routes have preload configure to tre
 * and also add the routes which have sendInReturnUrlOidc to the RouteConfigService to use when
 * redirect to IDM for login
 */
export class SelectiveStrategyService implements PreloadingStrategy {
  constructor(private routeConfigService: RouteConfigService) {}

  preload(route: Route, load: Function): Observable<any> {
    if (route.data) {
      if (route.data.sendInReturnUrlOidc) {
        this.routeConfigService.SendInRedirectUrlRoutePaths.push(route.path);
      }
      if (route.data.preload) {
        return load();
      }
    }

    return of(null);
  }
}
