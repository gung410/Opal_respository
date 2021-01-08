import {
  AmazonS3UploaderModule,
  BaseModuleOutlet,
  BaseRoutingModule,
  DocumentViewerModule,
  Fragment,
  FunctionModule,
  MODULE_INPUT_DATA,
  ModuleDataService,
  ModuleFacadeService,
  NAVIGATION_PARAMETERS_KEY,
  TranslationMessage,
  TranslationModule
} from '@opal20/infrastructure';
import { ButtonModule, ButtonsModule } from '@progress/kendo-angular-buttons';
import {
  CAMMenu,
  CAMRoutePaths,
  DomainComponentsModule,
  FragmentPosition,
  INavigationMenuItem,
  NavigationMenuFragment,
  NavigationMenuService,
  NavigationPageService,
  RouterPageInput,
  RouterPageInputExt
} from '@opal20/domain-components';
import { CommonComponentsModule, OpalDialogService, RootElementScrollableService } from '@opal20/common-components';
import { ContextMenuModule, MenuModule } from '@progress/kendo-angular-menu';
import {
  CourseDomainApiModule,
  OrganizationDomainApiModule,
  TaggingDomainApiModule,
  UserDomainApiModule,
  UserInfoModel
} from '@opal20/domain-api';
import { DateInputsModule, DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { Injector, NgModule, Type } from '@angular/core';
import { LayoutModule, TabStripModule } from '@progress/kendo-angular-layout';
import { Observable, of } from 'rxjs';

import { AddingParticipantPageComponent } from './components/adding-participant-page.component';
import { BlockoutDateDetailPageComponent } from './components/blockout-date-detail-page.component';
import { BlockoutDateManagementPageComponent } from './components/blockout-date-management-page.component';
import { CAMComponent } from './cam.component';
import { CAMOutletComponent } from './cam-outlet.component';
import { CAMRoutingModule } from './cam-routing.module';
import { CancellationRequestDialogComponent } from './components/dialogs/cancellation-request-dialog.component';
import { ChangeRegistrationClassDialogComponent } from './components/dialogs/change-registration-class-dialog.component';
import { ClassRunDetailPageComponent } from './components/classrun-detail-page.component';
import { ClassRunManagementPageComponent } from './components/classrun-management-page.component';
import { CourseCriteriaDetailPageComponent } from './components/course-criteria-detail-page.component';
import { CourseDetailPageComponent } from './components/course-detail-page.component';
import { CourseManagementPageComponent } from './components/course-management-page.component';
import { CourseOfPlanningCycleManagementPageComponent } from './components/course-of-planning-cycle-management-page.component';
import { CoursePlanningCycleDetailPageComponent } from './components/course-planning-cycle-detail-page.component';
import { CoursePlanningPageComponent } from './components/course-planning-page.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ECertificateManagementPageComponent } from './components/ecertificate-management-page.component';
import { ECertificateTemplateDetailPageComponent } from './components/ecertificate-template-detail-page.component';
import { EditorModule } from '@progress/kendo-angular-editor';
import { ExportParticipantDialogComponent } from './components/dialogs/export-participants-dialog.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { ImageCropperModule } from 'ngx-image-cropper';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LearnerProfilePageComponent } from './components/learner-profile-page.component';
import { NAVIGATORS } from './cam.config';
import { RegistrationECertificateDialogComponent } from './components/dialogs/registration-ecertificate-dialog.component';
import { ReportsPageComponent } from './components/reports-page.component';
import { RescheduleRequestDialogComponent } from './components/dialogs/reschedule-request-dialog.component';
import { Router } from '@angular/router';
import { SendCourseNominationAnnoucementPageComponent } from './components/send-course-nomination-announcement-page.component';
import { SendCoursePublicityPageComponent } from './components/send-course-publicity-page.component';
import { SendOrderRefreshmentComponent } from './components/send-order-refreshment-page.component';
import { SendPlacementLetterDialogComponent } from './components/dialogs/send-placement-letter-dialog.component';
import { SessionDetailPageComponent } from './components/session-detail-page.component';
import { SessionManagementPageComponent } from './components/session-management-page.component';
import { ToolBarModule } from '@progress/kendo-angular-toolbar';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { VenueManagementPageComponent } from './components/venue-management-page.component';
const DIRECTIVES: Type<unknown>[] = [];

const ENTRY_COMPONENT: Type<unknown>[] = [
  CAMOutletComponent,
  CancellationRequestDialogComponent,
  RescheduleRequestDialogComponent,
  ChangeRegistrationClassDialogComponent,
  ExportParticipantDialogComponent,
  SendPlacementLetterDialogComponent,
  RegistrationECertificateDialogComponent
];

const MAIN_COMPONENT: Type<unknown>[] = [
  ...ENTRY_COMPONENT,
  ...DIRECTIVES,
  CAMComponent,
  VenueManagementPageComponent,
  CourseManagementPageComponent,
  CourseDetailPageComponent,
  ClassRunManagementPageComponent,
  ClassRunDetailPageComponent,
  SessionManagementPageComponent,
  ReportsPageComponent,
  SessionDetailPageComponent,
  BlockoutDateDetailPageComponent,
  LearnerProfilePageComponent,
  CoursePlanningPageComponent,
  CoursePlanningCycleDetailPageComponent,
  CourseOfPlanningCycleManagementPageComponent,
  BlockoutDateManagementPageComponent,
  AddingParticipantPageComponent,
  SendCoursePublicityPageComponent,
  SendCourseNominationAnnoucementPageComponent,
  CourseCriteriaDetailPageComponent,
  ECertificateManagementPageComponent,
  ECertificateTemplateDetailPageComponent,
  SendOrderRefreshmentComponent
];

const LAYOUT_MODULES: Type<unknown>[] = [
  ButtonModule,
  ButtonsModule,
  LayoutModule,
  ContextMenuModule,
  MenuModule,
  DropDownsModule,
  InputsModule,
  GridModule,
  ToolBarModule,
  TooltipModule,
  EditorModule,
  DateInputsModule,
  DatePickerModule,
  DocumentViewerModule,
  ImageCropperModule,
  TabStripModule
];

const DOMAIN_API_MODULES: Type<unknown>[] = [
  CourseDomainApiModule,
  TaggingDomainApiModule,
  UserDomainApiModule,
  OrganizationDomainApiModule
];

@NgModule({
  imports: [
    ...DOMAIN_API_MODULES,
    FunctionModule,
    CAMRoutingModule,
    DomainComponentsModule,
    CommonComponentsModule,
    TranslationModule.registerModules([{ moduleId: 'cam' }]),
    AmazonS3UploaderModule,
    LAYOUT_MODULES
  ],
  declarations: [...MAIN_COMPONENT],
  entryComponents: [...ENTRY_COMPONENT],
  exports: [CAMOutletComponent],
  providers: [],
  bootstrap: [CAMComponent]
})
export class CAMModule extends BaseRoutingModule {
  protected get defaultPath(): Observable<string> {
    const navigationMenuService = this.injector.get(NavigationMenuService);
    const moduleDataService: ModuleDataService = this.moduleFacadeService.moduleDataService;
    this.moduleData = moduleDataService && moduleDataService.getData(MODULE_INPUT_DATA);
    if (this.moduleData) {
      navigationMenuService.activate(this.moduleData.path);
      this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, this.moduleData.navigationData);

      return of(this.moduleData.path);
    }

    return of(null);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return CAMOutletComponent;
  }

  protected get fragments(): { [position: string]: Type<Fragment> } {
    return {
      [FragmentPosition.NavigationMenu]: NavigationMenuFragment
    };
  }
  private moduleData: { path: string; navigationData: RouterPageInput<unknown, unknown, unknown> } | null;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected router: Router,
    protected injector: Injector,
    protected navigationMenuService: NavigationMenuService,
    protected navigationPageService: NavigationPageService,
    protected opalDialogService: OpalDialogService,
    protected rootElementScrollableService: RootElementScrollableService
  ) {
    super(moduleFacadeService, router, injector);
    this.moduleData = this.moduleFacadeService.moduleDataService.getData(MODULE_INPUT_DATA);
    this.setupNavigation();
  }

  protected onInit(): void {
    this.shellManager.registerDefaultFragments();
  }

  protected onDestroy(): void {
    this.shellManager.unregisterDefaultFragments();
  }

  protected buildMenuStatusDict(): { validActiveMenu: string; dic: Dictionary<boolean> } {
    const menuStatusDict: Dictionary<boolean> = {};
    let validActiveMenu: string;
    const defaultActiveMenu = this.getDefaultActiveMenu();
    Object.values(CAMMenu).forEach(p => {
      menuStatusDict[p] =
        this.moduleData && this.moduleData.navigationData.activeMenu
          ? this.moduleData.navigationData.activeMenu === p
          : defaultActiveMenu === p;
      if (menuStatusDict[p] === true) {
        validActiveMenu = p;
      }
    });
    if (validActiveMenu == null) {
      validActiveMenu = defaultActiveMenu;
    }
    return { validActiveMenu: validActiveMenu, dic: menuStatusDict };
  }

  protected setupNavigation(): void {
    this.setupNavigationPage();
    this.setupNavigationMenu();

    this.correctNavigationDataActiveMenuToDefaultIfNull(this.moduleFacadeService.moduleDataService, this.navigationPageService);
    this.correctNavigationDataRootPathToDefaultIfNull(this.moduleFacadeService.moduleDataService, this.navigationPageService);
  }

  private correctNavigationDataActiveMenuToDefaultIfNull(
    moduleDataService: ModuleDataService,
    navigationPageService: NavigationPageService
  ): void {
    const rootRoute = RouterPageInputExt.getRootRoute(this.moduleData.navigationData);
    if (rootRoute.activeMenu == null || rootRoute.activeMenu === '') {
      rootRoute.activeMenu = this.getDefaultActiveMenu();
      moduleDataService.setData(MODULE_INPUT_DATA, this.moduleData);
      navigationPageService.setCurrentRoute(this.moduleData.navigationData);
    }
  }

  private correctNavigationDataRootPathToDefaultIfNull(
    moduleDataService: ModuleDataService,
    navigationPageService: NavigationPageService
  ): void {
    const rootRoute = RouterPageInputExt.getRootRoute(this.moduleData.navigationData);
    if (rootRoute.path == null || rootRoute.path === '') {
      rootRoute.path = this.getDefaultPath();
      moduleDataService.setData(MODULE_INPUT_DATA, this.moduleData);
      navigationPageService.setCurrentRoute(this.moduleData.navigationData);
      navigationPageService.redirectByRouter(this.moduleData.navigationData);
    }
  }

  private setupNavigationPage(): void {
    this.navigationPageService.init(
      this.moduleFacadeService.moduleInstance.moduleInfo.id,
      this.moduleFacadeService.navigationService,
      this.rootElementScrollableService,
      NAVIGATORS,
      this.moduleData ? this.moduleData.navigationData : null,
      this.opalDialogService,
      this.getDefaultPath()
    );
  }

  private setupNavigationMenu(): void {
    const currentUser = UserInfoModel.getMyUserInfo();
    const navigationMenuItems: INavigationMenuItem[] = [
      {
        id: CAMRoutePaths.CoursePlanningPage,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Course Planning'),
        isActivated: this.getCurrentOrDefaultActiveMenu() === CAMMenu.CoursePlanning
      },
      CourseManagementPageComponent.hasViewPermissions(currentUser)
        ? {
            id: CAMRoutePaths.CourseManagementPage,
            name: new TranslationMessage(this.moduleFacadeService.translator, 'Course Administration'),
            isActivated: this.getCurrentOrDefaultActiveMenu() === CAMMenu.CourseAdministration
          }
        : null,
      ReportsPageComponent.hasViewPermissions(currentUser)
        ? {
            id: CAMRoutePaths.ReportsPage,
            name: new TranslationMessage(this.moduleFacadeService.translator, 'Reports'),
            isActivated: this.getCurrentOrDefaultActiveMenu() === CAMMenu.Reports
          }
        : null,
      {
        id: CAMRoutePaths.ECertificateManagementPage,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'E-Certificate Management'),
        isActivated: this.getCurrentOrDefaultActiveMenu() === CAMMenu.ECertificateManagement
      }
    ];
    this.navigationMenuService.init(
      (menuId, parameters: RouterPageInput<unknown, unknown, unknown>, skipLocationChange) =>
        this.navigationPageService.navigateByRouter({ ...parameters, activeMenu: menuId }),
      navigationMenuItems.filter(p => p != null),
      item => NAVIGATORS[item.id],
      false
    );
  }

  private getCurrentOrDefaultActiveMenu(): string {
    return this.moduleData.navigationData.activeMenu != null ? this.moduleData.navigationData.activeMenu : this.getDefaultActiveMenu();
  }

  private getDefaultActiveMenu(): CAMMenu {
    const currentUser = UserInfoModel.getMyUserInfo();
    if (currentUser.hasAdministratorRoles()) {
      return CAMMenu.CoursePlanning;
    }
    if (CourseManagementPageComponent.hasViewPermissions(currentUser)) {
      return CAMMenu.CourseAdministration;
    }
    return CAMMenu.CoursePlanning;
  }

  private getDefaultPath(): CAMRoutePaths {
    const currentUser = UserInfoModel.getMyUserInfo();
    if (currentUser.hasAdministratorRoles()) {
      return CAMRoutePaths.CoursePlanningPage;
    }
    if (CourseManagementPageComponent.hasViewPermissions(currentUser)) {
      return CAMRoutePaths.CourseManagementPage;
    }
    return CAMRoutePaths.CoursePlanningPage;
  }
}
