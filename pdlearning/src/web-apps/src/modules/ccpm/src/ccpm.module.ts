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
import {
  AnswerFeedbackDialogComponent,
  AppToolbarFragment,
  CommentDialogComponent,
  DigitalContentSearchTermService,
  DomainComponentsModule,
  FormSearchTermService,
  FragmentPosition,
  NavigationMenuFragment,
  NavigationMenuService,
  OpalOutletComponent,
  PersonalFilePreviewDialogComponent,
  PersonalSpaceSearchTermService,
  QuestionBankListService,
  QuestionOptionImageUploadDialogComponent,
  SelectUserDialogComponent,
  StandaloneSurveySearchTermService
} from '@opal20/domain-components';
import { ButtonModule, ButtonsModule, DropDownButtonModule } from '@progress/kendo-angular-buttons';
import {
  CollaborativeSocialLearningApiService,
  ContentDomainApiModule,
  FormDomainApiModule,
  FormSectionDomainApiModule,
  LnaFormDomainApiModule,
  PersonalSpaceApiModule,
  StandaloneSurveyApiService,
  SystemRoleEnum,
  TaggingDomainApiModule,
  UserInfoModel
} from '@opal20/domain-api';
import { ContextMenuModule, MenuModule } from '@progress/kendo-angular-menu';
import { DateInputsModule, DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { LayoutModule, PanelBarModule, PanelBarService, TabStripModule } from '@progress/kendo-angular-layout';
import { NgModule, Type } from '@angular/core';
import { Observable, of } from 'rxjs';

import { AccessRightTabComponent } from './components/tabs/access-right-tab/access-right-tab.component';
import { AddParticipantDialogComponent } from './components/dialogs/add-participant-dialog.component';
import { ArchivalTabComponent } from './components/tabs/additional-information-tab/archival-tab.component';
import { AssessmentRubricCompareIndicatorComponent } from './components/assessment-rubric-compare-indicator.component';
import { AssessmentRubricComparisonComponent } from './components/assessment-rubric-comparison.component';
import { AuditLogTabComponent } from './components/tabs/audit-log-tab/audit-log-tab.component';
import { CCPMComponent } from './ccpm.component';
import { CCPMRoutePaths } from './ccpm.config';
import { CCPMRoutingModule } from './ccpm-routing.module';
import { CommentDialogTemplateComponent } from './components/dialogs/comment-dialog-template.component';
import { CommonComponentsModule } from '@opal20/common-components';
import { CompareVersionContentDialogComponent } from './components/dialogs/compare-version-content-dialog.component';
import { CompareVersionDialogComponent } from './components/dialogs/compare-version-dialog.component';
import { CompareVersionFormDialogComponent } from './components/dialogs/compare-version-form-dialog.component';
import { CompareVersionStandaloneSurveyDialogComponent } from './components/dialogs/compare-version-standalone-survey-dialog.component';
import { CopyMetadataDialogComponent } from './components/dialogs/copy-metadata-dialog.component';
import { DigitalAdditionalInformationTabComponent } from './components/digital-additional-information-tab.component';
import { DigitalContentComparisonComponent } from './components/digital-content-comparison.component';
import { DigitalContentDetailPageComponent } from './components/digital-content-detail-page.component';
import { DigitalContentFeedbackTabComponent } from './components/digital-content-feedback-tab.component';
import { DigitalContentGeneralTabComponent } from './components/digital-content-general-tab.component';
import { DigitalContentRepositoryPageComponent } from './components/digital-content-repository-page.component';
import { DigitalContentUploadDialogComponent } from './components/dialogs/digital-content-upload-dialog.component';
import { DigitalLearningContentDialog } from './components/dialogs/digital-learning-content-dialog.component';
import { DigitalLearningContentEditorPageComponent } from './components/digital-learning-content-editor-page.component';
import { DigitalUploadContentEditorComponent } from './components/digital-upload-content-editor-page.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DueDateTabComponent } from './components/tabs/additional-information-tab/due-date-tab.component';
import { EditorModule } from '@progress/kendo-angular-editor';
import { FormAdditionalInformationTabComponent } from './components/form-additional-information-tab.component';
import { FormAssessmentAdderComponent } from './components/form-assessment-adder.component';
import { FormAssessmentIndicatorComponent } from './components/form-assessment-indicator.component';
import { FormAssessmentRubricEditorComponent } from './components/form-assessment-rubric-editor.component';
import { FormAssessmentRubricManagementPage } from './components/form-assessment-rubric-management-page.component';
import { FormComparisonComponent } from './components/form-comparison.component';
import { FormDetailPageComponent } from './components/form-detail-page.component';
import { FormEditModeService } from './services/form-edit-mode.service';
import { FormEditorComponent } from './components/form-editor.component';
import { FormEditorConfigComponent } from './components/form-editor-config.component';
import { FormEditorPageComponent } from './components/form-editor-page.component';
import { FormGeneralTabComponent } from './components/form-general-tab.component';
import { FormListComponent } from './components/form-list.component';
import { FormMetadataComponent } from './components/tabs/additional-information-tab/form-metadata/form-metadata.component';
import { FormQuestionAdderComponent } from './components/form-question-adder.component';
import { FormQuestionComparisonComponent } from './components/form-question-comparison.component';
import { FormQuestionConfigComponent } from './components/form-question-config.component';
import { FormQuestionEditorComponent } from './components/form-question-editor.component';
import { FormQuestionIndicatorComponent } from './components/form-question-indicator.component';
import { FormQuestionOptionEditorComponent } from './components/form-question-option-editor.component';
import { FormQuestionTemplateComponent } from './components/form-question-template.component';
import { FormRepositoryPageComponent } from './components/form-repository-page.component';
import { FormRepositoryPageService } from './services/form-repository-page.service';
import { FormSectionEditorComponent } from './components/form-section-editor.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { ImportFormDialogComponent } from './components/dialogs/import-form-dialog.component';
import { InlineDisplayTextboxDirective } from './directives/inline-display-textbox.directive';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PersonalSpaceRepositoryPageComponent } from './components/personal-space-repository-page.component';
import { PublishSettingTabComponent } from './components/tabs/additional-information-tab/publish-setting-tab.component';
import { QuestionBankAddDialogComponent } from './components/dialogs/question-bank-add-dialog.component';
import { QuestionBankGroupComponent } from './components/question-bank-group.component';
import { QuestionBankImportDialogComponent } from './components/dialogs/question-bank-import-dialog.component';
import { QuestionBankListComponent } from './components/question-bank-list.component';
import { QuestionBankPreviewDialogComponent } from './components/dialogs/question-bank-preview-dialog.component';
import { QuestionBankRepositoryPageComponent } from './components/question-bank-repository-page.component';
import { QuestionDateOptionSelectionDialogComponent } from './components/dialogs/question-date-option-selection-dialog.component';
import { QuestionTypeSelectionService } from './services/question-type-selection.service';
import { ReportsPageComponent } from './components/reports-page.component';
import { Router } from '@angular/router';
import { StandaloneFormAnswerReviewDialogComponent } from './components/tabs/standalone-form/standalone-form-answer-review-dialog.component';
import { StandaloneFormComponent } from './components/tabs/standalone-form/standalone-form.component';
import { StandaloneSurveyDateQuestionSelectionDialogComponent } from './components/dialogs/standalone-survey-date-question-selection-dialog.component';
import { StandaloneSurveyDetailPageComponent } from './components/standalone-survey-detail-page.component';
import { StandaloneSurveyList } from './components/standalone-survey-list.component';
import { StandaloneSurveyRepositoryPageComponent } from './components/standalone-survey-repository-page.component';
import { ToolBarModule } from '@progress/kendo-angular-toolbar';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { TransferOwnershipDialogComponent } from './components/dialogs/transfer-ownership-dialog.component';
@NgModule({
  imports: [
    FunctionModule,
    CCPMRoutingModule,
    CommonComponentsModule,
    DomainComponentsModule,
    TranslationModule.registerModules([{ moduleId: 'ccpm' }]),
    AmazonS3UploaderModule,
    ButtonModule,
    ContextMenuModule,
    MenuModule,
    DropDownsModule,
    InputsModule,
    GridModule,
    ToolBarModule,
    TooltipModule,
    ButtonsModule,
    EditorModule,
    DateInputsModule,
    DatePickerModule,
    DocumentViewerModule,
    ContentDomainApiModule,
    TaggingDomainApiModule,
    FormDomainApiModule,
    DropDownButtonModule,
    LayoutModule,
    TabStripModule,
    PanelBarModule,
    DragDropModule,
    FormSectionDomainApiModule,
    LnaFormDomainApiModule,
    PersonalSpaceApiModule
  ],
  declarations: [
    CCPMComponent,
    DigitalContentRepositoryPageComponent,
    DigitalLearningContentEditorPageComponent,
    DigitalUploadContentEditorComponent,
    FormEditorPageComponent,
    FormRepositoryPageComponent,
    FormQuestionIndicatorComponent,
    FormQuestionConfigComponent,
    FormEditorConfigComponent,
    FormEditorComponent,
    FormQuestionAdderComponent,
    FormQuestionEditorComponent,
    FormQuestionTemplateComponent,
    FormQuestionOptionEditorComponent,
    DigitalContentDetailPageComponent,
    FormDetailPageComponent,
    DigitalContentUploadDialogComponent,
    DigitalLearningContentDialog,
    AuditLogTabComponent,
    AccessRightTabComponent,
    DigitalAdditionalInformationTabComponent,
    DigitalContentGeneralTabComponent,
    FormAdditionalInformationTabComponent,
    FormGeneralTabComponent,
    FormListComponent,
    DigitalContentFeedbackTabComponent,
    InlineDisplayTextboxDirective,
    CommentDialogTemplateComponent,
    QuestionDateOptionSelectionDialogComponent,
    CopyMetadataDialogComponent,
    TransferOwnershipDialogComponent,
    ReportsPageComponent,
    FormSectionEditorComponent,
    ArchivalTabComponent,
    StandaloneFormComponent,
    AddParticipantDialogComponent,
    ImportFormDialogComponent,
    StandaloneFormAnswerReviewDialogComponent,
    DueDateTabComponent,
    CompareVersionDialogComponent,
    CompareVersionFormDialogComponent,
    FormQuestionComparisonComponent,
    FormComparisonComponent,
    CompareVersionContentDialogComponent,
    DigitalContentComparisonComponent,
    StandaloneSurveyRepositoryPageComponent,
    StandaloneSurveyList,
    StandaloneSurveyDetailPageComponent,
    PersonalSpaceRepositoryPageComponent,
    FormMetadataComponent,
    FormAssessmentRubricEditorComponent,
    FormAssessmentIndicatorComponent,
    FormAssessmentAdderComponent,
    FormAssessmentRubricManagementPage,
    CompareVersionStandaloneSurveyDialogComponent,
    StandaloneSurveyDateQuestionSelectionDialogComponent,
    PublishSettingTabComponent,
    QuestionBankRepositoryPageComponent,
    QuestionBankListComponent,
    QuestionBankPreviewDialogComponent,
    QuestionBankGroupComponent,
    QuestionBankImportDialogComponent,
    QuestionBankAddDialogComponent,
    AssessmentRubricCompareIndicatorComponent,
    AssessmentRubricComparisonComponent
  ],
  entryComponents: [
    OpalOutletComponent,
    DigitalContentUploadDialogComponent,
    CommentDialogComponent,
    QuestionOptionImageUploadDialogComponent,
    QuestionDateOptionSelectionDialogComponent,
    AnswerFeedbackDialogComponent,
    CopyMetadataDialogComponent,
    SelectUserDialogComponent,
    TransferOwnershipDialogComponent,
    AddParticipantDialogComponent,
    ImportFormDialogComponent,
    StandaloneFormAnswerReviewDialogComponent,
    CompareVersionFormDialogComponent,
    CompareVersionContentDialogComponent,
    CompareVersionStandaloneSurveyDialogComponent,
    PersonalFilePreviewDialogComponent,
    StandaloneSurveyDateQuestionSelectionDialogComponent,
    QuestionBankPreviewDialogComponent,
    QuestionBankGroupComponent,
    QuestionBankAddDialogComponent,
    QuestionBankImportDialogComponent
  ],
  exports: [],
  providers: [
    FormRepositoryPageService,
    FormEditModeService,
    QuestionTypeSelectionService,
    DigitalContentSearchTermService,
    FormSearchTermService,
    PanelBarService,
    StandaloneSurveySearchTermService,
    CollaborativeSocialLearningApiService,
    StandaloneSurveyApiService,
    CollaborativeSocialLearningApiService,
    PersonalSpaceSearchTermService,
    QuestionBankListService
  ],
  bootstrap: [CCPMComponent]
})
export class CCPMModule extends BaseRoutingModule {
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected router: Router,
    protected navigationMenuService: NavigationMenuService
  ) {
    super(moduleFacadeService, router);

    const moduleData = this.getModuleData();
    const activateFormTab: boolean = moduleData.isFormRepository || !!moduleData.formId;

    const menus = [
      {
        id: CCPMRoutePaths.DigitalContentRepository,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Digital Content Repository'),
        isActivated: !activateFormTab
      },
      {
        id: CCPMRoutePaths.FormRepository,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Form'),
        isActivated: activateFormTab
      },
      {
        id: CCPMRoutePaths.QuestionBankRepository,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Question Bank')
      },
      {
        id: CCPMRoutePaths.PersonalSpaceRepository,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Personal Space')
      },
      {
        id: CCPMRoutePaths.ReportsPage,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Reports')
      }
    ];

    const user = UserInfoModel.getMyUserInfo();
    // show/hide lna tab by roles
    const canAccessLnaTab: boolean = user.hasRole(
      SystemRoleEnum.DivisionAdministrator,
      SystemRoleEnum.SchoolStaffDeveloper,
      SystemRoleEnum.SchoolAdministrator,
      SystemRoleEnum.BranchAdministrator,
      SystemRoleEnum.SystemAdministrator,
      SystemRoleEnum.DivisionTrainingCoordinator
    );
    if (canAccessLnaTab) {
      menus.push({
        id: CCPMRoutePaths.StandaloneSurveyRepository,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'LNA Surveys')
      });
    }

    this.navigationMenuService.init(
      (menuId, parameters, skipLocationChange) =>
        this.moduleFacadeService.navigationService.navigateTo(menuId, parameters, skipLocationChange),
      menus
    );
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return OpalOutletComponent;
  }

  protected get fragments(): { [position: string]: Type<Fragment> } {
    return {
      [FragmentPosition.NavigationMenu]: NavigationMenuFragment,
      [FragmentPosition.AppToolbar]: AppToolbarFragment
    };
  }

  protected get defaultPath(): Observable<string> {
    const moduleData = this.getModuleData();

    if (moduleData.contentId) {
      this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, { id: moduleData.contentId });
      return of(CCPMRoutePaths.DigitalContentDetailPage);
    } else if (moduleData.isFormRepository) {
      return of(CCPMRoutePaths.FormRepository);
    } else if (moduleData.formId) {
      this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, moduleData);
      return of(CCPMRoutePaths.FormDetail);
    } else if (moduleData.isLnaForm) {
      return of(CCPMRoutePaths.StandaloneSurveyRepository);
    } else if (moduleData.lnaFormId) {
      this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, { formId: moduleData.lnaFormId });
      return of(CCPMRoutePaths.StandaloneSurveyDetailPage);
    }

    return of(null);
  }
  protected onInit(): void {
    this.shellManager.registerDefaultFragments();
  }

  protected onDestroy(): void {
    this.shellManager.unregisterDefaultFragments();
  }

  private getModuleData(): { contentId?: string; formId?: string; isFormRepository?: boolean; lnaFormId?: string; isLnaForm?: boolean } {
    const moduleDataService: ModuleDataService = this.moduleFacadeService.moduleDataService;
    return (moduleDataService && moduleDataService.getData(MODULE_INPUT_DATA)) || {};
  }
}
