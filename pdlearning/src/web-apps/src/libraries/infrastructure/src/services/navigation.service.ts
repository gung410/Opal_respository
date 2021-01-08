import { ContextDataService } from './context-data.service';
import { Injectable } from '@angular/core';
import { ModuleFlowManager } from '../module-flow-manager';
import { ModuleInstance } from '../shell/shell.models';
import { NAVIGATION_PARAMETERS_KEY } from '../constants';
import { NavigationError } from '../errors';
import { Router } from '@angular/router';

@Injectable()
export class NavigationService {
  constructor(
    private moduleInstance: ModuleInstance,
    private moduleFlowManager: ModuleFlowManager,
    private contextDataService: ContextDataService
  ) {}

  /**
   * Navigate to specific component by path.
   * @param path The path of component which is defined in routing module.
   * E.g,: navigationService.navigateTo('s01');
   * @param parameters This args is optional and used for navigation with parameters
   */
  public navigateTo<T>(path: string, parameters?: T, skipLocationChange: boolean = true): void {
    const router: Router | null = this.moduleInstance.router;

    if (!this.moduleInstance || !router) {
      return;
    }

    if (!this.hasChildRoute(path, router)) {
      throw new NavigationError(`The configuration error for screen with id ${path} at module ${this.moduleInstance.moduleInfo.id}`);
    }

    this.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, parameters);

    router.navigate([path], { skipLocationChange: skipLocationChange });
  }

  public updateDeeplink(path: string): void {
    const popStateFn = window.onpopstate;

    window.onpopstate = () => (window.onpopstate = popStateFn);
    AppGlobal.router.setRoute(path);
  }

  public returnRoot(): void {
    this.moduleFlowManager.checkAndUnloadCurrentModule();
  }

  public returnHome(): void {
    this.navigateTo(this.moduleInstance.defaultPath);
  }

  private hasChildRoute(path: string, router: Router): boolean {
    const routeParamsRegex = /\/:\w*\//g;
    const paramsRegexString = '/.*/';

    const matchChildRoute =
      router.config.findIndex(child => {
        let childPath = child.path + '/';
        const hasAnyParam = childPath.match(routeParamsRegex);
        if (!hasAnyParam) {
          return child.path === path;
        }
        childPath = childPath.replace(routeParamsRegex, paramsRegexString);
        const pathRegex = new RegExp(childPath);
        return `${path}/`.match(pathRegex) != null;
      }) > -1;

    return matchChildRoute;
  }
}
