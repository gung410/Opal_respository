import { AuthDataService, AuthService, OAuthService } from '@opal20/authentication';
import {
  BaseModuleOutlet,
  BaseRoutingModule,
  FunctionModule,
  MODULE_INPUT_DATA,
  ModuleDataService,
  ModuleFacadeService,
  NAVIGATION_PARAMETERS_KEY
} from '@opal20/infrastructure';
import { DomainComponentsModule, PLAYER_ACCESS_TOKEN_KEY, StandaloneSurveyRoutePaths } from '@opal20/domain-components';
import { Injector, NgModule, NgZone, Type } from '@angular/core';
import { Observable, of } from 'rxjs';

import { CommonComponentsModule } from '@opal20/common-components';
import { Router } from '@angular/router';
import { StandaloneSurveyComponent } from './standalone-survey.component';
import { StandaloneSurveyDetailPageComponent } from './components/standalone-survey-detail-page.component';
import { StandaloneSurveyLearningPageComponent } from './components/standalone-survey-learning-page.component';
import { StandaloneSurveyOutletComponent } from './standalone-survey-outlet.component';
import { StandaloneSurveyPlayerPageComponent } from './components/standalone-survey-player-page.component';
import { StandaloneSurveyRepositoryPageComponent } from './components/standalone-survey-repository-page.component';
import { StandaloneSurveyRoutingModule } from './standalone-survey-routing.module';

@NgModule({
  imports: [FunctionModule, StandaloneSurveyRoutingModule, CommonComponentsModule, DomainComponentsModule],
  declarations: [
    StandaloneSurveyComponent,
    StandaloneSurveyOutletComponent,
    StandaloneSurveyRepositoryPageComponent,
    StandaloneSurveyLearningPageComponent,
    StandaloneSurveyDetailPageComponent,
    StandaloneSurveyPlayerPageComponent
  ],
  entryComponents: [StandaloneSurveyOutletComponent],
  bootstrap: [StandaloneSurveyComponent]
})
export class StandaloneSurveyModule extends BaseRoutingModule {
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected router: Router,
    protected injector: Injector,
    private ngZone: NgZone,
    private oAuthService: OAuthService,
    private authService: AuthService,
    private authDataService: AuthDataService
  ) {
    super(moduleFacadeService, router, injector);
    this.setupAccessToken();
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return StandaloneSurveyOutletComponent;
  }

  protected get defaultPath(): Observable<string> {
    const moduleDataService: ModuleDataService = this.moduleFacadeService.moduleDataService;
    const moduleData: {
      path: StandaloneSurveyRoutePaths;
      navigationData: { communityId: string; formId?: string };
    } = moduleDataService && moduleDataService.getData(MODULE_INPUT_DATA);
    if (moduleData) {
      this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, moduleData.navigationData);

      return of(moduleData.path);
    }

    return of(null);
  }

  private setupAccessToken(): void {
    AppGlobal.standaloneSurveyManagementIntegrations.init = (accessToken: string) => {
      this.authService.setAccessToken(accessToken);
      localStorage.setItem(PLAYER_ACCESS_TOKEN_KEY, accessToken);
      this.signinCloudfront();
    };

    AppGlobal.standaloneSurveyManagementIntegrations.refreshAccessToken = (accessToken: string) => {
      this.authService.setAccessToken(accessToken);
    };

    AppGlobal.standaloneSurveyManagementIntegrations.changeRoute = (routePath: string, communityId: string, formId?: string) => {
      this.ngZone.run(() => {
        this.moduleFacadeService.navigationService.navigateTo(routePath, { communityId, formId });
        let fullRoutePath = `standalone-survey/community/${communityId}/${routePath}`;
        fullRoutePath += formId ? `/${formId}` : '';
        this.moduleFacadeService.navigationService.updateDeeplink(fullRoutePath);
      });
    };

    const playerAccessToken: string = localStorage.getItem(PLAYER_ACCESS_TOKEN_KEY);

    if (playerAccessToken) {
      let extId: string;
      this.oAuthService.skipSubjectCheck = true;
      Promise.resolve()
        .then(() => this.authService.setAccessToken(playerAccessToken))
        .then(() => this.oAuthService.loadDiscoveryDocument())
        .then(() => this.oAuthService.loadUserProfile())
        .then(userProfile => {
          // tslint:disable:no-string-literal
          extId = userProfile['sub'];
          return this.authDataService.getUserProfileAsync(extId);
        })
        .then(user => {
          AppGlobal.user = user;
          AppGlobal.user['extId'] = extId;
        })
        .then(() => {
          localStorage.removeItem(PLAYER_ACCESS_TOKEN_KEY);
        });
    }
  }

  private signinCloudfront(): void {
    const form: HTMLFormElement = document.querySelector('#cloudfront-form');
    form.action = `${AppGlobal.environment.cloudfrontUrl}/api/cloudfront/signin?returnUrl=${encodeURIComponent(location.href)}`;

    const input: HTMLInputElement = document.querySelector('#token');
    input.value = this.authService.getAccessToken();

    form.submit();
  }
}
