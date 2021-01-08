import { MODULE_INPUT_DATA, NAVIGATION_PARAMETERS_KEY } from '../constants';
import { Observable, of } from 'rxjs';
import { Route, Router } from '@angular/router';

import { BaseModule } from './base-module';
import { Injector } from '@angular/core';
import { ModuleDataService } from '../services/context-data.service';
import { ModuleFacadeService } from '../services/module-facade.service';
import { map } from 'rxjs/operators';

export abstract class BaseRoutingModule extends BaseModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router, protected injector?: Injector) {
    super(moduleFacadeService.shellManager);
    this.processNavigation();
  }

  /**
   * @virtual
   * Override this function to set default screen path.
   */
  protected get defaultPath(): Observable<string | null> {
    return of(null);
  }

  protected processModuleInput(inputData: unknown): Observable<{ path?: string; inputData?: unknown }> {
    return this.defaultPath.pipe(map(path => ({ path, inputData })));
  }

  /**
   * @virtual
   * Override this function init navigation service.
   */
  protected initNavigationService(): void {
    // Implement me!!
  }

  private processNavigation(): void {
    if (!this.router) {
      return;
    }

    // Need to init navigation service before any navigation processing.
    this.initNavigationService();

    const routes: Route[] = this.router.config;

    if (routes && routes.length > 0) {
      const moduleDataService: ModuleDataService = this.moduleFacadeService.moduleDataService;
      const moduleData: unknown = moduleDataService && moduleDataService.getData(MODULE_INPUT_DATA);
      const defaultRoutePath: string = routes[0].path || '';

      if (moduleData) {
        this.subscribeOne(this.processModuleInput(moduleData), processedInputResult => {
          const { path, inputData } = processedInputResult;

          this.navigate(path || defaultRoutePath, inputData);
        });
      } else {
        this.subscribeOne(this.defaultPath, (path: string) => {
          this.navigate(path || defaultRoutePath);
        });
      }
    }
  }

  private navigate(path: string, inputData?: unknown): void {
    const contextDataKey: string = `${NAVIGATION_PARAMETERS_KEY}#${path}`;

    this.moduleFacadeService.moduleInstance.defaultPath = path;
    this.moduleFacadeService.moduleInstance.contextDataKey = contextDataKey;

    if (inputData) {
      this.moduleFacadeService.contextDataService.setData(contextDataKey, inputData);
    }

    this.router.navigate([path], { skipLocationChange: true });
  }
}
