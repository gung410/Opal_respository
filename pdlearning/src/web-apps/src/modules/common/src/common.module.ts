import {
  BaseModuleOutlet,
  BaseRoutingModule,
  MODULE_INPUT_DATA,
  ModuleDataService,
  ModuleFacadeService,
  NAVIGATION_PARAMETERS_KEY,
  TranslationMessage,
  TranslationModule
} from '@opal20/infrastructure';
import { DomainComponentsModule, NavigationMenuService, OpalOutletComponent } from '@opal20/domain-components';
import { Injector, NgModule, Type } from '@angular/core';
import { Observable, of } from 'rxjs';

import { CommonComponent } from './common.component';
import { CommonComponentsModule } from '@opal20/common-components';
import { CommonModule } from '@angular/common';
import { CommonRoutePaths } from './common.config';
import { CommonRoutingModule } from './common-routing.module';
import { ErrorPageComponent } from './components/error-page/error-page.component';
import { Router } from '@angular/router';
import { WebinarErrorPageComponent } from './components/webinar-error-page/webinar-error-page.component';
import { WebinarSeekingServerPageComponent } from './components/webinar-seeking-server-page/webinar-seeking-server-page.component';

@NgModule({
  imports: [
    CommonModule,
    CommonRoutingModule,
    CommonComponentsModule,
    DomainComponentsModule,
    TranslationModule.registerModules([{ moduleId: 'common' }])
  ],
  declarations: [CommonComponent, ErrorPageComponent, WebinarErrorPageComponent, WebinarSeekingServerPageComponent],
  entryComponents: [OpalOutletComponent],
  bootstrap: [CommonComponent]
})
export class OpalCommonModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router, protected injector: Injector) {
    super(moduleFacadeService, router, injector);
  }

  protected initNavigationService(): void {
    const navigationService: NavigationMenuService = this.injector.get(NavigationMenuService);

    navigationService.init(
      (menuId, parameters, skipLocationChange) =>
        this.moduleFacadeService.navigationService.navigateTo(menuId, parameters, skipLocationChange),
      [
        {
          id: CommonRoutePaths.Error,
          name: new TranslationMessage(this.moduleFacadeService.translator, 'Error')
        },
        {
          id: CommonRoutePaths.WebinarError,
          name: new TranslationMessage(this.moduleFacadeService.translator, 'Webinar Error')
        },
        {
          id: CommonRoutePaths.WebinarSeekingServer,
          name: new TranslationMessage(this.moduleFacadeService.translator, 'Webinar Seeking Server')
        }
      ]
    );
  }

  protected get defaultPath(): Observable<string> {
    const moduleDataService: ModuleDataService = this.moduleFacadeService.moduleDataService;
    const moduleData: { path: string; navigationData: {}; errorCode?: string; meetingId?: string; source?: string } =
      moduleDataService && moduleDataService.getData(MODULE_INPUT_DATA);
    const navigationService: NavigationMenuService = this.injector.get(NavigationMenuService);
    if (moduleData) {
      // Route to webinar error page.
      if (moduleData.errorCode) {
        this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, { errorCode: moduleData.errorCode });
        return of(CommonRoutePaths.WebinarError);
      }

      // Route to webinar seeking server page.
      if (moduleData.meetingId) {
        this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, {
          meetingId: moduleData.meetingId,
          source: moduleData.source
        });
        return of(CommonRoutePaths.WebinarSeekingServer);
      }

      navigationService.activate(moduleData.path);
      this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, moduleData.navigationData);

      return of(moduleData.path);
    }

    return of(null);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return OpalOutletComponent;
  }

  protected onInit(): void {
    this.shellManager.registerDefaultFragments();
  }

  protected onDestroy(): void {
    this.shellManager.unregisterDefaultFragments();
  }
}
