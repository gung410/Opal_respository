import { Component, HostBinding, Inject, OnDestroy, OnInit, Type } from '@angular/core';
import { Fragment, IModuleParameters, ModuleFlowManager, ModuleInfo, ShellManager } from '@opal20/infrastructure';
import {
  FragmentPosition,
  HeaderFragment,
  HeaderService,
  LearnerRoutePaths,
  MyLearningTab,
  NavigationPageService,
  OpalViewportService,
  RouterPageInput,
  RouterPageInputExt,
  StandaloneSurveyRoutePaths
} from '@opal20/domain-components';
import { cloudfrontCheckingKey, redirectModuleKey } from './app.config';

import { APP_BASE_HREF } from '@angular/common';
import { AuthDataService } from '@opal20/authentication';
import { MY_ACHIEVEMENT_TYPE_ENUM } from 'modules/learner/src/constants/my-achievement.constant';
import { Subject } from 'rxjs';
import { SystemRoleEnum } from '@opal20/domain-api';
import { takeUntil } from 'rxjs/operators';
import { verifyIgnorePath } from './app.module';

@Component({
  selector: 'app-root',
  template: `
    <app-shell class="app-shell column flex"></app-shell>
    <div kendoDialogContainer></div>
    <div kendoWindowContainer></div>
  `
})
export class AppComponent implements OnInit, OnDestroy {
  public onDestroy$: Subject<unknown> = new Subject<unknown>();
  public modules: IRegistrationModule[] = AppGlobal.registrationModules;
  public router: Router = AppGlobal.router;
  private defaultFragments: { [position: string]: Type<Fragment> } = {
    [FragmentPosition.Header]: HeaderFragment
  };
  private readonly moduleIdMap: { [target: string]: string } = {
    LearnerSite: 'learner',
    CourseContentSite: 'ccpm',
    LearningManagement: 'lmm',
    CourseAdminManagement: 'cam'
  };
  private currentHref: string | undefined;

  constructor(
    private moduleFlowManager: ModuleFlowManager,
    private shellManager: ShellManager,
    private headerService: HeaderService,
    private authDataService: AuthDataService,
    private viewportService: OpalViewportService,
    @Inject(APP_BASE_HREF) private baseHref: string
  ) {
    AppGlobal.accessibleModules = this.getAccessibleModules();

    const opalAllowedModuleIds: string[] = [
      'dashboard',
      'digital-content-player',
      'video-annotation-player',
      'scorm-player',
      'quiz-player',
      'common',
      'community-metadata',
      'form-standalone',
      'form-standalone-player',
      'assignment-player',
      'calendar',
      'assessment-player',
      'standalone-survey'
    ];

    this.modules.forEach(m => {
      if (!opalAllowedModuleIds.some(id => id === m.id) && !AppGlobal.accessibleModules.some(am => am.id === m.id)) {
        return;
      }

      let location: string = `${this.baseHref}${m.id}`;
      let callback: () => void | Promise<void> = () => this.openModule(m.id, m.name);

      switch (m.id) {
        case 'dashboard':
          location = this.baseHref;
          this.router.on(location, callback);
          break;
        case 'learner':
          callback = () => this.ifUserHasOnboarding(() => this.openModule(m.id, m.name));
          // Register learner sub-routes
          this.registerSubRoutesLearnerModule(m);
          this.router.on(location, callback);
          break;
        case 'cam':
          callback = () => this.openModule(m.id, m.name);
          // Register cam sub-routes
          this.registerSubRoutesCamLmmModule(m);
          break;
        case 'ccpm':
          callback = () => this.openModule(m.id, m.name);
          this.registerCCPMSubRoutes(m);
          this.router.on(location, callback);
          break;
        case 'lmm':
          callback = () => this.openModule(m.id, m.name);
          // Register lmm sub-routes
          this.registerSubRoutesCamLmmModule(m);
          break;
        case 'form-standalone':
          callback = () => this.openModule(m.id, m.name);
          this.registerFormStandaloneSubRoutes(m);
          this.router.on(location, callback);
          break;
        case 'form-standalone-player':
          callback = () => this.openModule(m.id, m.name);
          this.router.on(location, callback);
          break;
        case 'common':
          callback = () => this.openModule(m.id, m.name);
          this.registerCommonPages(m);
          this.router.on(location, callback);
          break;
        case 'calendar':
          callback = () => this.openModule(m.id, m.name);
          this.registerCalendarSubRoutes(m);
          this.router.on(location, callback);
          break;
        case 'standalone-survey':
          callback = () => this.openModule(m.id, m.name);
          this.registerStandaloneSurveySubRoutes(m);
          this.router.on(location, callback);
          break;
        default:
          this.router.on(location, callback);
          break;
      }
      this.shellManager.registerModule(new ModuleInfo(m.id, m.loadNgModule));
    });
    const redirectModuleKeyLocalStorage = localStorage.getItem(redirectModuleKey);
    if (
      AppGlobal.environment.authConfig.ignoredPaths.indexOf(location.pathname) < 0 &&
      location.pathname.indexOf('calendar/') < 0 &&
      (redirectModuleKeyLocalStorage == null ||
        (redirectModuleKeyLocalStorage !== 'lmm' &&
          redirectModuleKeyLocalStorage.indexOf('lmm/') !== 0 &&
          redirectModuleKeyLocalStorage !== 'cam' &&
          redirectModuleKeyLocalStorage.indexOf('cam/') !== 0))
    ) {
      this.viewportService.setViewPortDesktopMode();
      this.viewportService.resize$.pipe(takeUntil(this.onDestroy$)).subscribe(windowWidth => {
        this.viewportService.setViewPortDesktopMode();
      });
    }
    this.router.init();
    this.preventBackButtonClick();
  }

  @HostBinding('class.page-host')
  public getPageHostClassName(): boolean {
    return true;
  }

  public ngOnInit(): void {
    if (localStorage.getItem(cloudfrontCheckingKey)) {
      return;
    }

    this.moduleFlowManager.bootstrapMainApp().then(() => {
      this.shellManager.setDefaultFragments(this.defaultFragments);
      const redirectModuleId: string = localStorage.getItem(redirectModuleKey);

      const isIgnorePath = verifyIgnorePath();
      if (isIgnorePath) {
        this.router.setRoute(this.router.getPath() + location.search);
      } else if (redirectModuleId === 'learner') {
        this.ifUserHasOnboarding(() => this.router.setRoute(`${this.baseHref}${redirectModuleId}`));
      } else if (redirectModuleId && redirectModuleId !== 'common') {
        this.router.setRoute(`${this.baseHref}${redirectModuleId}`);
      } else if (document.location.pathname === `${this.baseHref}index.html` || document.location.pathname === `${this.baseHref}`) {
        if (AppGlobal.accessibleModules.findIndex(m => m.id === 'learner') > -1) {
          this.router.setRoute(`${this.baseHref}learner`);
        } else {
          this.router.setRoute(`${this.baseHref}common`);
        }
      } else {
        this.router.setRoute(this.router.getPath() + location.search);
      }

      localStorage.removeItem(redirectModuleKey);
    });
  }

  public ngOnDestroy(): void {
    this.onDestroy$.next();
    this.onDestroy$.complete();
  }

  private ifUserHasOnboarding(elseCallback?: () => void): void {
    this.authDataService.getUserProfileAsync(AppGlobal.user.extId).then(profile => {
      // Check if the current user finish the on boarding or not.
      if (
        profile.jsonDynamicAttributes.finishOnBoarding !== true &&
        profile.systemRoles.filter(role => role.identity.extId === SystemRoleEnum.Learner).length > 0
      ) {
        window.location.href = AppGlobal.environment.userOnboardingUrl;
      } else {
        if (elseCallback) {
          elseCallback();
        }
      }
    });
  }

  private openModule(id: string, name: string, moduleParameters?: IModuleParameters): Promise<void> {
    this.headerService.moduleName = name;

    return this.moduleFlowManager.openModule(id, moduleParameters);
  }

  private openModuleKeepCurrentPath(id: string, name: string, routerPageInut: RouterPageInput<unknown, unknown, unknown>): Promise<void> {
    const currentPath = this.router.getPath();
    return this.openModule(id, name, {
      data: {
        path: routerPageInut.path,
        navigationData: routerPageInut
      }
    }).then(() => {
      NavigationPageService.updateDeeplink(currentPath);
    });
  }

  private getAccessibleModules(): IRegistrationModule[] {
    let menuItems: IRegistrationModule[] = [];

    /**
     * TODO: This logic will be refactor with authentication integration later.
     */
    try {
      // WARNING: The code below just be used in development and should not be allowed on production.
      menuItems = AppGlobal.user.siteData.menus[0].menuItems.map(item => {
        const moduleId: string = this.moduleIdMap[item.target.trim()];
        let navigateToModule: () => void = () => (document.location.href = item.path);

        if (moduleId) {
          navigateToModule = () => this.router.setRoute(moduleId);
        }

        return {
          id: moduleId,
          name: item.localizedData[0].fields[0].localizedText,
          description: item.localizedData[0].fields[1].localizedText,
          shortName: item.localizedData[0].fields[0].localizedText,
          navigateToModule
        } as IRegistrationModule;
      });
    } catch {
      /** Ignore errors */
    }

    return menuItems;
  }

  private registerSubRoutesCamLmmModule(m: IRegistrationModule): void {
    const defaultRoute = new Array(this.baseHref, m.id);
    this.router.on(defaultRoute.join('/'), () => {
      this.openModuleKeepCurrentPath(m.id, m.name, RouterPageInputExt.buildRouterPageInput(''));
    });

    const routeLevel1 = defaultRoute.concat(':pathLevel1');
    this.router.on(routeLevel1.join('/'), (pathLevel1: string) => {
      this.openModuleKeepCurrentPath(m.id, m.name, RouterPageInputExt.buildRouterPageInput(pathLevel1, pathLevel1));
    });

    const routeLevel1WithData = routeLevel1.concat(':encodedPageInputDataLevel1');
    this.router.on(routeLevel1WithData.join('/'), (pathLevel1: string, encodedPageInputDataLevel1?: string) => {
      const pageInputDataLevel1 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel1);
      this.openModuleKeepCurrentPath(m.id, m.name, RouterPageInputExt.buildRouterPageInput(pathLevel1, pathLevel1, pageInputDataLevel1));
    });

    const routeLevel2 = routeLevel1WithData.concat(':pathLevel2');
    this.router.on(routeLevel2.join('/'), (pathLevel1: string, encodedPageInputDataLevel1: string, pathLevel2: string) => {
      const pageInputDataLevel1 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel1);
      const pageInputLevel1 = RouterPageInputExt.buildRouterPageInput(pathLevel1, pathLevel1, pageInputDataLevel1);
      this.openModuleKeepCurrentPath(m.id, m.name, RouterPageInputExt.buildRouterPageInput(pathLevel2, pathLevel1, pageInputLevel1));
    });

    const routeLevel2WithData = routeLevel2.concat(':encodedPageInputDataLevel2');
    this.router.on(
      routeLevel2WithData.join('/'),
      (pathLevel1: string, encodedPageInputDataLevel1: string, pathLevel2: string, encodedPageInputDataLevel2?: string) => {
        const pageInputDataLevel1 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel1);
        const pageInputLevel1 = RouterPageInputExt.buildRouterPageInput(pathLevel1, pathLevel1, pageInputDataLevel1);
        const pageInputDataLevel2 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel2);
        this.openModuleKeepCurrentPath(
          m.id,
          m.name,
          RouterPageInputExt.buildRouterPageInput(pathLevel2, pathLevel1, pageInputDataLevel2, pageInputLevel1)
        );
      }
    );

    const routeLevel3 = routeLevel2WithData.concat(':pathLevel3');
    this.router.on(
      routeLevel3.join('/'),
      (
        pathLevel1: string,
        encodedPageInputDataLevel1: string,
        pathLevel2: string,
        encodedPageInputDataLevel2: string,
        pathLevel3: string
      ) => {
        const pageInputDataLevel1 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel1);
        const pageInputLevel1 = RouterPageInputExt.buildRouterPageInput(pathLevel1, pathLevel1, pageInputDataLevel1);
        const pageInputDataLevel2 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel2);
        const pageInputLevel2 = RouterPageInputExt.buildRouterPageInput(pathLevel2, pathLevel1, pageInputDataLevel2, pageInputLevel1);
        this.openModuleKeepCurrentPath(m.id, m.name, RouterPageInputExt.buildRouterPageInput(pathLevel3, pathLevel1, pageInputLevel2));
      }
    );

    const routeLevel3WithData = routeLevel3.concat(':encodedPageInputDataLevel3');
    this.router.on(
      routeLevel3WithData.join('/'),
      (
        pathLevel1: string,
        encodedPageInputDataLevel1: string,
        pathLevel2: string,
        encodedPageInputDataLevel2: string,
        pathLevel3: string,
        encodedPageInputDataLevel3?: string
      ) => {
        const pageInputDataLevel1 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel1);
        const pageInputLevel1 = RouterPageInputExt.buildRouterPageInput(pathLevel1, pathLevel1, pageInputDataLevel1);
        const pageInputDataLevel2 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel2);
        const pageInputLevel2 = RouterPageInputExt.buildRouterPageInput(pathLevel2, pathLevel1, pageInputDataLevel2, pageInputLevel1);
        const pageInputDataLevel3 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel3);
        this.openModuleKeepCurrentPath(
          m.id,
          m.name,
          RouterPageInputExt.buildRouterPageInput(pathLevel3, pathLevel1, pageInputDataLevel3, pageInputLevel2)
        );
      }
    );

    const routeLevel4 = routeLevel3WithData.concat(':pathLevel4');
    this.router.on(
      routeLevel4.join('/'),
      (
        pathLevel1: string,
        encodedPageInputDataLevel1: string,
        pathLevel2: string,
        encodedPageInputDataLevel2: string,
        pathLevel3: string,
        encodedPageInputDataLevel3: string,
        pathLevel4: string
      ) => {
        const pageInputDataLevel1 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel1);
        const pageInputLevel1 = RouterPageInputExt.buildRouterPageInput(pathLevel1, pathLevel1, pageInputDataLevel1);
        const pageInputDataLevel2 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel2);
        const pageInputLevel2 = RouterPageInputExt.buildRouterPageInput(pathLevel2, pathLevel1, pageInputDataLevel2, pageInputLevel1);
        const pageInputDataLevel3 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel3);
        const pageInputLevel3 = RouterPageInputExt.buildRouterPageInput(pathLevel3, pathLevel1, pageInputDataLevel3, pageInputLevel2);
        this.openModuleKeepCurrentPath(
          m.id,
          m.name,
          RouterPageInputExt.buildRouterPageInput(pathLevel4, pathLevel1, null, pageInputLevel3)
        );
      }
    );

    const routeLevel4WithData = routeLevel4.concat(':encodedPageInputDataLevel4');
    this.router.on(
      routeLevel4WithData.join('/'),
      (
        pathLevel1: string,
        encodedPageInputDataLevel1: string,
        pathLevel2: string,
        encodedPageInputDataLevel2: string,
        pathLevel3: string,
        encodedPageInputDataLevel3: string,
        pathLevel4: string,
        encodedPageInputDataLevel4?: string
      ) => {
        const pageInputDataLevel1 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel1);
        const pageInputLevel1 = RouterPageInputExt.buildRouterPageInput(pathLevel1, pathLevel1, pageInputDataLevel1);
        const pageInputDataLevel2 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel2);
        const pageInputLevel2 = RouterPageInputExt.buildRouterPageInput(pathLevel2, pathLevel1, pageInputDataLevel2, pageInputLevel1);
        const pageInputDataLevel3 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel3);
        const pageInputLevel3 = RouterPageInputExt.buildRouterPageInput(pathLevel3, pathLevel1, pageInputDataLevel3, pageInputLevel2);
        const pageInputDataLevel4 = RouterPageInputExt.decodePageInputData(encodedPageInputDataLevel4);
        this.openModuleKeepCurrentPath(
          m.id,
          m.name,
          RouterPageInputExt.buildRouterPageInput(pathLevel4, pathLevel1, pageInputDataLevel4, pageInputLevel3)
        );
      }
    );
  }

  private registerSubRoutesLearnerModule(m: IRegistrationModule): void {
    const homePath = `${LearnerRoutePaths.Home}`;
    this.router.on(`${this.baseHref}${m.id}/${homePath}`, () => this.openModule(m.id, m.name, { data: { path: homePath } }));
    this.router.on(`${this.baseHref}${m.id}/${homePath}/:type`, (type?: string) => {
      this.openModule(m.id, m.name, { data: { path: homePath, navigationData: { type } } });
    });

    const ePortfolioPath = `${LearnerRoutePaths.EPortfolio}`;
    this.router.on(`${this.baseHref}${m.id}/${ePortfolioPath}`, () => this.openModule(m.id, m.name, { data: { path: ePortfolioPath } }));

    const reportsPath = `${LearnerRoutePaths.ReportsPage}`;
    this.router.on(`${this.baseHref}${m.id}/${reportsPath}`, () => this.openModule(m.id, m.name, { data: { path: reportsPath } }));

    const path = `${LearnerRoutePaths.MyLearning}`;
    this.router.on(`${this.baseHref}${m.id}/${path}`, () => this.openModule(m.id, m.name, { data: { path } }));
    this.router.on(`${this.baseHref}${m.id}/${path}/:activeTab`, (activeTab: MyLearningTab) =>
      this.openModule(m.id, m.name, { data: { path, navigationData: { activeTab: activeTab } } })
    );
    this.router.on(`${this.baseHref}${m.id}/${path}/${MyLearningTab.LearningPaths}/:pathId`, (pathId: string) =>
      this.openModule(m.id, m.name, { data: { path, navigationData: { pathId, activeTab: MyLearningTab.LearningPaths } } })
    );
    this.router.on(`${this.baseHref}${m.id}/${path}/${MyLearningTab.LearningPaths}/:pathId/:type`, (pathId: string, type: string) =>
      this.openModule(m.id, m.name, { data: { path, navigationData: { pathId, activeTab: MyLearningTab.LearningPaths, type } } })
    );

    const detailPath = `${LearnerRoutePaths.Detail}`;
    this.router.on(`${this.baseHref}${m.id}/${detailPath}/:courseType/:courseId`, (courseType?: string, courseId?: string) => {
      this.openModule(m.id, m.name, { data: { path: detailPath, navigationData: { courseId, courseType } } });
    });

    const pdPlanPath = `${LearnerRoutePaths.PdPlan}`;
    this.router.on(`${this.baseHref}${m.id}/${pdPlanPath}`, () => {
      this.openModule(m.id, m.name, { data: { path: pdPlanPath, navigationData: {} } });
    });

    this.router.on(`${this.baseHref}${m.id}/${pdPlanPath}/:tab`, (tab?: string) => {
      this.openModule(m.id, m.name, { data: { path: pdPlanPath, navigationData: { tab } } });
    });

    const cataloguePath = `${LearnerRoutePaths.Catalogue}`;
    this.router.on(`${this.baseHref}${m.id}/${cataloguePath}`, () => {
      this.openModule(m.id, m.name, { data: { path: cataloguePath, navigationData: {} } });
    });

    this.router.on(`${this.baseHref}${m.id}/${cataloguePath}/:catalogueType`, (catalogueType?: string) => {
      this.openModule(m.id, m.name, { data: { path: cataloguePath, navigationData: { catalogueType } } });
    });

    const calendarPath = `${LearnerRoutePaths.Calendar}`;
    this.router.on(`${this.baseHref}${m.id}/${calendarPath}`, () => {
      this.openModule(m.id, m.name, { data: { path: calendarPath, navigationData: {} } });
    });

    const myAchievementsPath = `${LearnerRoutePaths.MyAchievements}`;
    this.router.on(`${this.baseHref}${m.id}/${myAchievementsPath}`, () => {
      this.openModule(m.id, m.name, { data: { path: myAchievementsPath, navigationData: {} } });
    });
    this.router.on(`${this.baseHref}${m.id}/${myAchievementsPath}/:myAchievementsType`, (myAchievementsType?: MY_ACHIEVEMENT_TYPE_ENUM) => {
      this.openModule(m.id, m.name, { data: { path: myAchievementsPath, navigationData: { myAchievementsType } } });
    });
  }

  private registerCCPMSubRoutes(m: IRegistrationModule): void {
    this.router.on(`${this.baseHref}${m.id}/content/:contentId`, (contentId?: string) =>
      this.openModule(m.id, m.name, { data: { contentId } })
    );
    this.router.on(`${this.baseHref}${m.id}/form`, () => this.openModule(m.id, m.name, { data: { isFormRepository: true } }));
    this.router.on(`${this.baseHref}${m.id}/form/:formId`, (formId?: string) => this.openModule(m.id, m.name, { data: { formId } }));
    this.router.on(`${this.baseHref}${m.id}/lnaform`, () => this.openModule(m.id, m.name, { data: { isLnaForm: true } }));
    this.router.on(`${this.baseHref}${m.id}/lnaform/:lnaFormId`, (lnaFormId?: string) =>
      this.openModule(m.id, m.name, { data: { lnaFormId } })
    );
  }

  private registerFormStandaloneSubRoutes(m: IRegistrationModule): void {
    this.router.on(`${this.baseHref}${m.id}/form/:formId`, (formId?: string) =>
      this.openModule(m.id, m.name, { data: { formId, from: 'form' } })
    );
    this.router.on(`${this.baseHref}${m.id}/standalonesurvey/lna/:formId`, (formId?: string) =>
      this.openModule(m.id, m.name, { data: { formId, from: 'lnaform' } })
    );

    // origin router configuration
    // this.router.on(`${this.baseHref}${m.id}/:formId`, (formId?: string) => this.openModule(m.id, m.name, { data: { formId } }));
  }

  private registerCommonPages(m: IRegistrationModule): void {
    this.router.on(`${this.baseHref}${m.id}/meeting/:source/:meetingId`, (source?: string, meetingId?: string) =>
      this.openModule(m.id, m.name, { data: { source, meetingId } })
    );
    this.router.on(`${this.baseHref}${m.id}/webinar-error/:errorCode`, (errorCode?: string) =>
      this.openModule(m.id, m.name, { data: { errorCode } })
    );
  }

  private registerCalendarSubRoutes(m: IRegistrationModule): void {
    // Create new community event WITH select communities
    this.router.on(`${this.baseHref}${m.id}/communities/events/:eventType/new-event`, (eventType: string) =>
      this.openModule(m.id, m.name, { data: { eventType, createEvent: true } })
    );
    // Create new community event WITHOUT choose communities
    this.router.on(
      `${this.baseHref}${m.id}/communities/:communityId/events/:eventType/new-event`,
      (communityId: string, eventType: string) => this.openModule(m.id, m.name, { data: { createEvent: true, communityId, eventType } })
    );
    // Edit an community event.
    this.router.on(`${this.baseHref}${m.id}/communities/events/:eventId`, (eventId: string) =>
      this.openModule(m.id, m.name, { data: { editEvent: true, eventId } })
    );

    // Team calendar
    this.router.on(`${this.baseHref}${m.id}/team`, () => this.openModule(m.id, m.name, { data: { teamCalendar: true } }));
  }

  private registerStandaloneSurveySubRoutes(m: IRegistrationModule): void {
    this.router.on(`${this.baseHref}${m.id}/community/:communityId/${StandaloneSurveyRoutePaths.RepositoryPage}`, (communityId: string) =>
      this.openModule(m.id, m.name, {
        data: { path: StandaloneSurveyRoutePaths.RepositoryPage, navigationData: { communityId } }
      })
    );
    this.router.on(`${this.baseHref}${m.id}/community/:communityId/${StandaloneSurveyRoutePaths.LearningPage}`, (communityId: string) =>
      this.openModule(m.id, m.name, {
        data: { path: StandaloneSurveyRoutePaths.LearningPage, navigationData: { communityId } }
      })
    );
    this.router.on(
      `${this.baseHref}${m.id}/community/:communityId/${StandaloneSurveyRoutePaths.DetailPage}/:formId`,
      (communityId: string, formId: string) =>
        this.openModule(m.id, m.name, {
          data: {
            path: StandaloneSurveyRoutePaths.DetailPage,
            navigationData: { communityId, formId }
          }
        })
    );
    this.router.on(
      `${this.baseHref}${m.id}/community/:communityId/${StandaloneSurveyRoutePaths.PlayerPage}/:formId`,
      (communityId: string, formId: string) =>
        this.openModule(m.id, m.name, {
          data: {
            path: StandaloneSurveyRoutePaths.PlayerPage,
            navigationData: { communityId, formId }
          }
        })
    );
  }

  private preventBackButtonClick(): void {
    const redirectModuleId = localStorage.getItem(redirectModuleKey);
    if (redirectModuleId) {
      this.currentHref = this.baseHref + redirectModuleId;
    } else {
      this.currentHref = document.location.href;
    }

    const onPopStateFn = window.onpopstate as (event: PopStateEvent) => void;

    window.onpopstate = (event: PopStateEvent) => {
      if (location.pathname === `${this.baseHref}index.html`) {
        history.pushState(null, document.title, this.currentHref);
      } else {
        this.currentHref = location.href;
        onPopStateFn(event);
      }
    };
  }
}
