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
import { CommonComponentsModule, OpalDialogService, RootElementScrollableService } from '@opal20/common-components';
import { ContextMenuModule, MenuModule } from '@progress/kendo-angular-menu';
import { CourseDomainApiModule, TaggingDomainApiModule, UserDomainApiModule, UserInfoModel } from '@opal20/domain-api';
import { DateInputsModule, DatePickerModule } from '@progress/kendo-angular-dateinputs';
import {
  DomainComponentsModule,
  FragmentPosition,
  INavigationMenuItem,
  LMMMenu,
  LMMRoutePaths,
  NavigationMenuFragment,
  NavigationMenuService,
  NavigationPageService,
  RouterPageInput,
  RouterPageInputExt
} from '@opal20/domain-components';
import { Injector, NgModule, Type } from '@angular/core';
import { Observable, of } from 'rxjs';
import { PanelBarModule, TabStripModule } from '@progress/kendo-angular-layout';

import { AnnouncementActivityLogPageComponent } from './components/announcement-activity-log-page.component';
import { AnnouncementPageComponent } from './components/announcement-page.component';
import { AssessmentAnswerEditorComponent } from './components/assessment-answer-editor.component';
import { AssignAssignmentDialogComponent } from './components/dialogs/assign-assignment-dialog.component';
import { AssignmentDetailPageComponent } from './components/assignment-detail-page.component';
import { AssignmentEditorComponent } from './components/assignment-editor.component';
import { AssignmentEditorConfigComponent } from './components/assignment-editor-config.component';
import { AssignmentFormEditor } from './components/assignment-form-editor.component';
import { AssignmentQuestionAdderComponent } from './components/assignment-question-adder.component';
import { AssignmentQuestionConfigComponent } from './components/assignment-question-config.component';
import { AssignmentQuestionDateOptionSelectionDialogComponent } from './components/dialogs/assignment-question-date-option-selection-dialog.component';
import { AssignmentQuestionTemplateComponent } from './components/assignment-question-template.component';
import { AssignmentQuestionTypeSelectionService } from './services/assignment-question-type-selection.service';
import { AttendanceTrackingManagementPageComponent } from './components/attendance-tracking-management-page.component';
import { ClassRunDetailPageComponent } from './components/classrun-detail-page.component';
import { ClassRunManagementPageComponent } from './components/classrun-management-page.component';
import { ContentDetailPageComponent } from './components/content-detail-page.component';
import { CourseDetailDialogComponent } from './components/dialogs/course-detail-dialog.component';
import { CourseDetailPageComponent } from './components/course-detail-page.component';
import { CourseManagementPageComponent } from './components/course-management-page.component';
import { DigitalBadgesManagementPageComponent } from './components/digital-badges-management-page.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { EditorModule } from '@progress/kendo-angular-editor';
import { FeedbackDetailPageComponent } from './components/feedback-detail-page.component';
import { FormReferenceDialogComponent } from './components/dialogs/form-reference-dialog.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { ImageCropperModule } from 'ngx-image-cropper';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LMMComponent } from './lmm.component';
import { LMMOutletComponent } from './lmm-outlet.component';
import { LMMRoutingModule } from './lmm-routing.module';
import { LearnerProfilePageComponent } from './components/learner-profile-page.component';
import { LearningPathDetailPageComponent } from './components/learning-path-detail-page.component';
import { LearningPathManagementPageComponent } from './components/learning-path-management-page.component';
import { LectureContentEditor } from './components/lecture-content-editor.component';
import { ListFeedbackGridComponentService } from './services/list-feedback-grid-component.service';
import { NAVIGATORS } from './lmm.config';
import { ParticipantAssignmentTrackDetailPageComponent } from './components/participant-assignment-track-page.component';
import { ParticipantAssignmentTrackEditor } from './components/participant-assignment-track-editor.component';
import { PeerAssessmentPageComponent } from './components/peer-assessment-page.component';
import { PeerAssessmentPageComponentService } from './services/peer-assessment-page-component.service';
import { PeerAssessorSelectionComponent } from './components/peer-assessor-selection.component';
import { PreviewAssessmentDialogComponent } from './components/dialogs/preview-assessment-dialog.component';
import { PreviewContentDialogComponent } from './components/dialogs/preview-content-dialog.component';
import { PreviewQuizPlayerComponent } from './components/preview-quiz-player.component';
import { QRCodeDialogComponent } from './components/dialogs/qr-code-dialog.component';
import { QRCodeModule } from 'angularx-qrcode';
import { ReportsPageComponent } from './components/reports-page.component';
import { Router } from '@angular/router';
import { SectionFormEditorDialogComponent } from './components/dialogs/section-form-editor-dialog.component';
import { SendAnnouncementPageComponent } from './components/send-announcement-page.component';
import { SessionDetailPageComponent } from './components/session-detail-page.component';
import { SessionManagementPageComponent } from './components/session-management-page.component';
import { SettingActiveContributorBadgeCriteriaDialogComponent } from './components/dialogs/setting-active-contributor-badge-criteria-dialog.component';
import { SetupPeerAssessmentDialogComponent } from './components/dialogs/setup-peer-assessment-dialog.component';
import { StatisticDetailPageComponent } from './components/statistic-detail-page.component';
import { TableOfContentComponent } from './components/table-of-content.component';
import { ToolBarModule } from '@progress/kendo-angular-toolbar';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
const DIRECTIVES: Type<unknown>[] = [];

const ENTRY_COMPONENT: Type<unknown>[] = [
  LMMOutletComponent,
  CourseDetailDialogComponent,
  FormReferenceDialogComponent,
  AssignmentFormEditor,
  QRCodeDialogComponent,
  SectionFormEditorDialogComponent,
  LectureContentEditor,
  AssignmentQuestionDateOptionSelectionDialogComponent,
  PreviewContentDialogComponent,
  AssignAssignmentDialogComponent,
  PreviewAssessmentDialogComponent,
  SetupPeerAssessmentDialogComponent,
  SettingActiveContributorBadgeCriteriaDialogComponent
];

const MAIN_COMPONENT: Type<unknown>[] = [
  ...ENTRY_COMPONENT,
  ...DIRECTIVES,
  LMMComponent,
  CourseManagementPageComponent,
  LearningPathDetailPageComponent,
  LearningPathManagementPageComponent,
  AttendanceTrackingManagementPageComponent,
  CourseDetailPageComponent,
  StatisticDetailPageComponent,
  ContentDetailPageComponent,
  FeedbackDetailPageComponent,
  TableOfContentComponent,
  PreviewQuizPlayerComponent,
  ClassRunDetailPageComponent,
  ClassRunManagementPageComponent,
  AssignmentDetailPageComponent,
  LearnerProfilePageComponent,
  ReportsPageComponent,
  ParticipantAssignmentTrackDetailPageComponent,
  AssignmentEditorComponent,
  AssignmentQuestionTemplateComponent,
  AssignmentQuestionAdderComponent,
  AssignmentQuestionConfigComponent,
  AssignmentQuestionDateOptionSelectionDialogComponent,
  AssignmentEditorConfigComponent,
  ParticipantAssignmentTrackEditor,
  AnnouncementPageComponent,
  SendAnnouncementPageComponent,
  AnnouncementActivityLogPageComponent,
  ReportsPageComponent,
  SessionDetailPageComponent,
  SessionManagementPageComponent,
  PeerAssessorSelectionComponent,
  PeerAssessmentPageComponent,
  AssessmentAnswerEditorComponent,
  DigitalBadgesManagementPageComponent
];

const LAYOUT_MODULES: Type<unknown>[] = [
  ButtonModule,
  ButtonsModule,
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
  TabStripModule,
  QRCodeModule,
  PanelBarModule
];

const DOMAIN_API_MODULES: Type<unknown>[] = [CourseDomainApiModule, TaggingDomainApiModule, UserDomainApiModule];

@NgModule({
  imports: [
    FunctionModule,
    LMMRoutingModule,
    CommonComponentsModule,
    DomainComponentsModule,
    TranslationModule.registerModules([{ moduleId: 'lmm' }]),
    AmazonS3UploaderModule,
    ...LAYOUT_MODULES,
    ...DOMAIN_API_MODULES
  ],
  declarations: [...MAIN_COMPONENT],
  entryComponents: [...ENTRY_COMPONENT],
  exports: [LMMOutletComponent],
  providers: [ListFeedbackGridComponentService, AssignmentQuestionTypeSelectionService, PeerAssessmentPageComponentService],
  bootstrap: [LMMComponent]
})
export class LMMModule extends BaseRoutingModule {
  private moduleData: { path: string; navigationData: RouterPageInput<unknown, unknown, unknown> };
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

  protected get defaultPath(): Observable<string> {
    const moduleDataService: ModuleDataService = this.moduleFacadeService.moduleDataService;
    const navigationMenuService: NavigationMenuService = this.injector.get(NavigationMenuService);
    this.moduleData = moduleDataService && moduleDataService.getData(MODULE_INPUT_DATA);

    if (this.moduleData) {
      navigationMenuService.activate(this.moduleData.path);
      this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, this.moduleData.navigationData);

      return of(this.moduleData.path);
    }

    return of(null);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return LMMOutletComponent;
  }

  protected get fragments(): { [position: string]: Type<Fragment> } {
    return {
      [FragmentPosition.NavigationMenu]: NavigationMenuFragment
    };
  }

  protected onInit(): void {
    this.shellManager.registerDefaultFragments();
  }

  protected onDestroy(): void {
    this.shellManager.unregisterDefaultFragments();
  }

  private correctNavigationDataActiveMenuToDefaultIfNull(
    moduleDataService: ModuleDataService,
    navigationPageService: NavigationPageService
  ): void {
    const rootRoute = RouterPageInputExt.getRootRoute(this.moduleData.navigationData);
    if (rootRoute.activeMenu == null) {
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
    if (rootRoute.path == null) {
      rootRoute.path = this.getDefaultPath();
      moduleDataService.setData(MODULE_INPUT_DATA, this.moduleData);
      navigationPageService.setCurrentRoute(this.moduleData.navigationData);
      navigationPageService.redirectByRouter(this.moduleData.navigationData);
    }
  }

  private setupNavigation(): void {
    this.setupNavigationPage();
    this.setupNavigationMenu();

    this.correctNavigationDataActiveMenuToDefaultIfNull(this.moduleFacadeService.moduleDataService, this.navigationPageService);
    this.correctNavigationDataRootPathToDefaultIfNull(this.moduleFacadeService.moduleDataService, this.navigationPageService);
  }

  private setupNavigationPage(): void {
    this.navigationPageService.init(
      this.moduleFacadeService.moduleInstance.moduleInfo.id,
      this.moduleFacadeService.navigationService,
      this.rootElementScrollableService,
      NAVIGATORS,
      this.moduleData.navigationData,
      this.opalDialogService,
      LMMRoutePaths.CourseManagementPage
    );
  }

  private setupNavigationMenu(): void {
    const currentUser = UserInfoModel.getMyUserInfo();
    this.navigationMenuService.init(
      (menuId, parameters: RouterPageInput<unknown, unknown, unknown>, skipLocationChange) =>
        this.navigationPageService.navigateByRouter({ ...parameters, activeMenu: menuId }),
      <INavigationMenuItem[]>[
        {
          id: LMMRoutePaths.CourseManagementPage,
          name: new TranslationMessage(this.moduleFacadeService.translator, 'Learning Management'),
          isActivated: this.getCurrentOrDefaultActiveMenu() === LMMMenu.LearningManagement
        },
        CourseManagementPageComponent.hasViewPermissions(currentUser)
          ? {
              id: LMMRoutePaths.LearningPathManagementPage,
              name: new TranslationMessage(this.moduleFacadeService.translator, 'Learning Path Administration'),
              isActivated: this.getCurrentOrDefaultActiveMenu() === LMMMenu.LearningPathManagement
            }
          : null,
        CourseManagementPageComponent.hasViewPermissions(currentUser)
          ? {
              id: LMMRoutePaths.ReportsPage,
              name: new TranslationMessage(this.moduleFacadeService.translator, 'Reports'),
              isActivated: this.getCurrentOrDefaultActiveMenu() === LMMMenu.Reports
            }
          : null,
        {
          id: LMMRoutePaths.DigitalBadgesManagementPage,
          name: new TranslationMessage(this.moduleFacadeService.translator, 'Digital Badges Management'),
          isActivated: this.getCurrentOrDefaultActiveMenu() === LMMMenu.DigitalBadgesManagement
        }
      ],
      item => {
        return NAVIGATORS[item.id];
      },
      false
    );
  }

  private getDefaultActiveMenu(): LMMMenu {
    return LMMMenu.LearningManagement;
  }

  private getDefaultPath(): LMMRoutePaths {
    return LMMRoutePaths.CourseManagementPage;
  }

  private getCurrentOrDefaultActiveMenu(): string {
    return this.moduleData.navigationData.activeMenu != null ? this.moduleData.navigationData.activeMenu : this.getDefaultActiveMenu();
  }
}
