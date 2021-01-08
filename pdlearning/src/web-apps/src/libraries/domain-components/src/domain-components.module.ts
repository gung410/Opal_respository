import { AmazonS3UploaderModule, DocumentViewerModule, FunctionModule } from '@opal20/infrastructure';
import { ButtonModule, ButtonsModule } from '@progress/kendo-angular-buttons';
import {
  CalendarDomainApiModule,
  CommentDomainApiModule,
  ContentDomainApiModule,
  CourseDomainApiModule,
  FormDomainApiModule,
  FormParticipantDomainApiModule,
  LearnerDomainApiModule,
  OrganizationDomainApiModule,
  PersonalSpaceApiModule,
  TaggingDomainApiModule,
  UserDomainApiModule
} from '@opal20/domain-api';
import { ContextMenuModule, MenuModule } from '@progress/kendo-angular-menu';
import { DateInputsModule, DatePickerModule, TimePickerModule } from '@progress/kendo-angular-dateinputs';
import {
  DeviceViewSimulationComponent,
  MobileTemplateDirective,
  WebTemplateDirective
} from './components/device-view-simulation/device-view-simulation.component';
import { InputsModule, NumericTextBoxModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { ModuleWithProviders, NgModule, Type } from '@angular/core';
import { PanelBarModule, TabStripModule } from '@progress/kendo-angular-layout';

import { AbsenceDetailDialogComponent } from './components/absence-detail-dialog/absence-detail-dialog.component';
import { AnnouncementDetailDialogComponent } from './components/announcement-detail-dialog/announcement-detail-dialog.component';
import { AnswerFeedbackDialogComponent } from './components/answer-feedback-dialog/answer-feedback-dialog.component';
import { AppToolbarDirective } from './fragments/app-toolbar/directives/app-toolbar.directive';
import { AppToolbarFragment } from './fragments/app-toolbar/app-toolbar.fragment';
import { AppToolbarService } from './fragments/app-toolbar/app-toolbar.service';
import { AssessmentCriteriaComponent } from './components/assessment-player/assessment-criteria.component';
import { AssessmentPlayerIntegrationsService } from './services/assessment-player-integrations.service';
import { AssessmentScaleComponent } from './components/assessment-player/assessment-scale.component';
import { AssignmentPlayerIntegrationsService } from './services/assignment-player-integrations.service';
import { AssignmentQuestionEditorComponent } from './components/assignment-player/assignment-question-editor.component';
import { AssignmentQuestionOptionEditorComponent } from './components/assignment-player/assignment-question-option-editor.component';
import { AuthenticationModule } from '@opal20/authentication';
import { BasicInfoTabComponent } from './components/course-detail/basic-info-tab.component';
import { BatchFileUploaderComponent } from './components/batch-file-uploader/batch-file-uploader.component';
import { BatchUploadFileContentComponent } from './components/batch-upload-file-content/batch-upload-file-content.component';
import { BatchUploadFilesDialogComponent } from './components/batch-upload-files-dialog/batch-upload-files-dialog.component';
import { BlockOutDateDependenciesDetailDialog } from './components/blockout-date-dependencies-detail-dialog/blockout-date-dependencies-detail-dialog.component';
import { BlockoutDateDetailComponent } from './components/blockout-date-detail/blockout-date-detail.component';
import { BlockoutDateFilterComponent } from './components/blockout-date-filter/blockout-date-filter.component';
import { BlockoutDateWarningComponent } from './components/blockout-date-warning/blockout-date-warning.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { BreadcrumbService } from './services/breadcrumb.service';
import { BroadcastMessageNotificationComponent } from './components/broadcast-message-notification/broadcast-message-notification.component';
import { BrokenLinkReportDialogComponent } from './components/broken-link-report-dialog/broken-link-report-dialog.component';
import { BrokenLinkReportTabComponent } from './components/broken-link-report-tab/broken-link-report-tab.component';
import { BrokenLinkScannerDialogComponent } from './components/broken-link-scanner-dialog/broken-link-scanner-dialog.component';
import { ClassApplicationTabComponent } from './components/classrun-detail/classrun-application-tab.component';
import { ClassRunDetailComponent } from './components/classrun-detail/classrun-detail.component';
import { ClassRunOverviewInfoTabComponent } from './components/classrun-detail/classrun-overview-info-tab.component';
import { ClassRunPlanningTabComponent } from './components/classrun-detail/classrun-planning-tab.component';
import { CommentDialogComponent } from './components/comment-dialog/comment-dialog.component';
import { CommentTabComponent } from './components/comment-tab/comment-tab.component';
import { CommonComponentsModule } from '@opal20/common-components';
import { CommonModule } from '@angular/common';
import { ContentDialogComponent } from './components/content-dialog/content-dialog.component';
import { CopyLearningPathHyperlinkComponent } from './components/copy-learning-path-hyperlink/copy-learning-path-hyperlink.component';
import { CopyRightTabComponent } from './components/course-detail/copyright-tab.component';
import { CopyrightAttributionElementFormComponent } from './components/copyright-form/copyright-attribution-element-form.component';
import { CopyrightFormComponent } from './components/copyright-form/copyright-form.component';
import { CourseAdministrationTabComponent } from './components/course-detail/course-administration-tab.component';
import { CourseCriteriaDetailComponent } from './components/course-criteria-detail/course-criteria-detail.component';
import { CourseCriteriaMaxParticipantSelect } from './components/course-criteria-max-participant-select/course-criteria-max-participant-select.component';
import { CourseCriteriaRegistrationViolationDialogComponent } from './components/course-criteria-registration-violation-dialog/course-criteria-registration-violation-dialog.component';
import { CourseDetailComponent } from './components/course-detail/course-detail.component';
import { CourseFilterComponent } from './components/course-filter/course-filter.component';
import { CoursePlanningCycleDetailComponent } from './components/course-planning-cycle-detail/course-planning-cycle-detail.component';
import { CoursePlanningCycleOverviewInfoTabComponent } from './components/course-planning-cycle-detail/course-planning-cycle-overview-info-tab.component';
import { CoursePlanningTabComponent } from './components/course-detail/course-planning-tab.component';
import { CourseRelatedCriteriaTabComponent } from './components/course-criteria-detail/course-related-criteria-tab.component';
import { DetailContentDirective } from './fragments/detail-content/directives/detail-content.directive';
import { DetailContentFragment } from './fragments/detail-content/detail-content.fragment';
import { DetailContentLeftDirective } from './fragments/detail-content/directives/toolbar-left.directive';
import { DetailContentRightDirective } from './fragments/detail-content/directives/toolbar-right.directive';
import { DetailContentService } from './fragments/detail-content/detail-content.service';
import { DigitalContentListPageService } from './services/digital-content-list-page.service';
import { DigitalContentPlayerComponent } from './components/digital-content-player/digital-content-player.component';
import { DigitalContentReferenceDialog } from './components/digital-content-reference-dialog/digital-content-reference-dialog.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ECertificateDetailComponent } from './components/ecertificate-detail/ecertificate-detail.component';
import { ECertificateLayoutSelectionComponent } from './components/ecertificate-detail/ecertificate-layout-selection.component';
import { ECertificateTemplateCustomiseComponent } from './components/ecertificate-detail/ecertificate-template-customise.component';
import { EditorModule } from '@progress/kendo-angular-editor';
import { EvaluationEcertificateTabComponent } from './components/course-detail/evaluation-ecertificate-tab.component';
import { FormAnswerAttachmentsComponent } from './components/quiz-player/form-answer-attachments.component';
import { FormAnswerPlayerComponent } from './components/quiz-player/form-answer-player.component';
import { FormEditorPageService } from './services/form-editor-page.service';
import { FormParticipantListService } from './services/form-participant-list.service';
import { FormPollResultsComponent } from './components/quiz-player/form-poll-results.component';
import { FormQuestionAnswerPlayerComponent } from './components/quiz-player/form-question-answer-player.component';
import { FormQuestionAnswerPlayerReviewComponent } from './components/quiz-player/form-question-answer-player-review.component';
import { FormQuestionAnswerReviewComponent } from './components/quiz-player/form-question-answer-review.component';
import { FormSectionInfoComponent } from './components/quiz-player/form-section-info.component';
import { FundingAndSubsidyComponent } from './components/funding-and-subsidy/funding-and-subsidy.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { HeaderFragment } from './fragments/header/header.fragment';
import { HeaderService } from './fragments/header/header.service';
import { HeaderTitleDirective } from './fragments/header/header-title.directive';
import { ImageCropperModule } from 'ngx-image-cropper';
import { KendoBatchUploadFilesButtonDirective } from './directives/kendo-batch-upload-files-button.directive';
import { KendoTreeviewMetadataAutoCheckAllChildsDirective } from './directives/kendo-treeview-metadata-auto-check-all-childs.directive';
import { KendoTreeviewMetadataCustomSelectDeselectAllDirective } from './directives/kendo-treeview-metadata-select-deselect-all.directive';
import { LearnerBasicInfoComponent } from './components/learner-profile/learner-basic-info.component';
import { LearnerMetadataInfoComponent } from './components/learner-profile/learner-metadata-info.component';
import { LearnerProfileComponent } from './components/learner-profile/learner-profile.component';
import { LearnerProfileVmService } from './services/learner-profile-vm.service';
import { LearnerRelatedCriteriaTabComponent } from './components/course-criteria-detail/learner-related-criteria-tab.component';
import { LearningPathBasicInfoTabComponent } from './components/learning-path-detail/learning-path-basic-info-tab.component';
import { LearningPathDetailComponent } from './components/learning-path-detail/learning-path-detail.component';
import { LearningPathMetadataTabComponent } from './components/learning-path-detail/learning-path-metadata-tab.component';
import { LearningPathPdOpportunitiesComponent } from './components/learning-path-detail/learning-path-pd-opportunities.component';
import { LearningPathPdOpportunitiesDialogComponent } from './components/learning-path-pd-opportunities-dialog/learning-path-pd-opportunities-dialog.component';
import { LearningPathSelectedSoursesComponent } from './components/learning-path-detail/learning-path-selected-courses.component';
import { LearningPathSharingComponent } from './components/learning-path-detail/learning-path-sharing.component';
import { LearningPathSharingDialogComponent } from './components/learning-path-detail/learning-path-sharing-dialog.component';
import { ListAnnouncementGridComponent } from './components/list-announcement-grid/list-announcement-grid.component';
import { ListAnnouncementGridComponentService } from './services/list-announcement-grid-component.service';
import { ListAssignmentGridComponent } from './components/list-assignment-grid/list-assignment-grid.component';
import { ListAssignmentGridComponentService } from './services/list-assignment-grid-component.service';
import { ListAttendanceTrackingGridComponent } from './components/list-attendance-tracking-grid/list-attendance-tracking-grid.component';
import { ListAttendanceTrackingGridComponentService } from './services/list-attendance-tracking.service';
import { ListBadgeLearnerGridComponent } from './components/list-badge-learner-grid/list-badge-learner-grid.component';
import { ListBadgeLearnerGridComponentService } from './services/list-badge-learner-grid-component.service';
import { ListBlockoutDateGridComponent } from './components/list-blockout-date-grid/list-blockout-date-grid.component';
import { ListBlockoutDateGridComponentService } from './services/list-blockout-date-grid-component.service';
import { ListClassRunGridComponent } from './components/list-classrun-grid/list-classrun-grid.component';
import { ListClassRunGridComponentService } from './services/list-classrun-grid-component.service';
import { ListCourseGridComponent } from './components/list-course-grid/list-course-grid.component';
import { ListCourseGridComponentService } from './services/list-course-grid-component.service';
import { ListCoursePlanningCycleGridComponent } from './components/list-course-planning-cycle/list-course-planning-cycle-grid.component';
import { ListCoursePlanningCycleGridComponentService } from './services/list-course-planning-cycle-grid-component.service';
import { ListDigitalContentPageComponent } from './components/list-digital-content-page/list-digital-content-page.component';
import { ListECertificateGridComponent } from './components/list-ecertificate/list-ecertificate-grid.component';
import { ListECertificateGridComponentService } from './services/list-ecertificate-grid-component.service';
import { ListLearningPathGridComponent } from './components/list-learning-path-grid/list-learning-path-grid.component';
import { ListLearningPathGridComponentService } from './services/list-learning-path-grid-component.service';
import { ListParticipantAssignmentTrackGridComponent } from './components/list-assignment-track-grid/list-assignment-track-grid.component';
import { ListParticipantAssignmentTrackGridComponentService } from './services/list-participant-assignment-track-component.service';
import { ListParticipantGridComponent } from './components/list/participant-grid/participant-grid.component';
import { ListRegistrationGridComponent } from './components/list-registration-grid/list-registration-grid.component';
import { ListRegistrationGridComponentService } from './services/list-registration-component.service';
import { ListSessionGridComponent } from './components/list-session-grid/list-session-grid.component';
import { ListSessionGridComponentService } from './services/list-session-grid-component.service';
import { MailEditorComponent } from './components/mail-editor/mail-editor.component';
import { MainAssessmentPlayerComponent } from './components/assessment-player/main-assessment-player.component';
import { MainAssignmentPlayerComponent } from './components/assignment-player/main-assignment-player.component';
import { MainFormStandalonePlayerPageComponent } from './components/form-standalone-player/main-form-standalone-player-page.component';
import { MainQuizPlayerPageComponent } from './components/quiz-player/main-quiz-player-page.component';
import { MainQuizPlayerPageService } from './services/main-quiz-player-page.service';
import { MegaMenuComponent } from './fragments/navigation-menu/mega-menu.component';
import { MetadataEditorComponent } from './components/metadata/metadata-editor.component';
import { MetadataEditorService } from './services/metadata-editor.service';
import { MetadataSelectDeselectAllNodeTemplateComponent } from './components/metadata-select-deselect-all-node-template/metadata-select-deselect-all-node-template.component';
import { MetadataTabComponent } from './components/course-detail/metadata-tab.component';
import { MyAchievementsDialogComponent } from './components/my-achievements-dialog/learning-my-achievements-dialog.component';
import { NavigationMenuFragment } from './fragments/navigation-menu/navigation-menu.fragment';
import { NavigationMenuService } from './fragments/navigation-menu/navigation-menu.service';
import { NavigationPageService } from './services/navigation-page.service';
import { OpalFooterComponent } from './components/opal-footer/opal-footer.component';
import { OpalFooterService } from './services/opal-footer.services';
import { OpalKendoEditorComponent } from './components/opal-kendo-editor/opal-kendo-editor-component';
import { OpalOutletComponent } from './opal-outlet.component';
import { OpalReportComponent } from './components/opal-report/opal-report.component';
import { OpalReportDynamicComponent } from './components/opal-report-dynamic/opal-report-dynamic.component';
import { OpalViewportService } from './services/viewport.service';
import { OwnerInfoComponent } from './components/owner-info/owner-info.component';
import { PackageConfirmDialog } from './components/learning-package-confirmation-dialog/learning-package-confirmation-dialog.component';
import { PermissionDirective } from './directives/permission.directive';
import { PersonalFileDialogComponent } from './components/personal-file-dialog/personal-file-dialog.component';
import { PersonalFileListComponent } from './components/personal-space-list/personal-file-list.component';
import { PersonalFilePreviewDialogComponent } from './components/personal-file-preview-dialog/personal-file-preview-dialog.component';
import { PopupModule } from '@progress/kendo-angular-popup';
import { PreviewAudioPlayerComponent } from './components/preview-digital-content/preview-audio-player.component';
import { PreviewDigitalContentComponent } from './components/preview-digital-content/preview-digital-content.component';
import { PreviewDocumentPlayerComponent } from './components/preview-digital-content/preview-document-player.component';
import { PreviewEcertificateTemplateDialogComponent } from './components/preview-ecertificate-template-dialog/preview-ecertificate-template-dialog.component';
import { PreviewFormDialogComponent } from './components/preview-form-dialog/preview-form-dialog.component';
import { PreviewVideoPlayerComponent } from './components/preview-digital-content/preview-video-player.component';
import { ProviderInfoTabComponent } from './components/course-detail/provider-info-tab.component';
import { QuestionAnswerReviewComponent } from './components/quiz-player/question-answer-review.component';
import { QuestionBlankOptionEditorComponent } from './components/question-blank-option-editor/question-blank-option-editor.component';
import { QuestionOptionImageUploadDialogComponent } from './components/question-option-image-upload-dialog/question-option-image-upload-dialog.component';
import { QuestionTextOptionEditorComponent } from './components/question-text-option-editor/question-text-option-editor.component';
import { QuestionTitleEditorComponent } from './components/question-title-editor/question-title-editor.component';
import { QuizPlayerIntegrationsService } from './services/quiz-player-integrations.service';
import { RegistrationFilterComponent } from './components/registration-filter/registration-filter.component';
import { RichTextEditorComponent } from './components/rich-text-editor/rich-text-editor.component';
import { ScormPlayerComponent } from './components/scorm-player/scorm-player.component';
import { SecondToTimePipe } from './pipes/second-to-time.pipe';
import { SelectCourseDialogComponent } from './components/select-course-dialog/select-course-dialog.component';
import { SelectFilesDialogComponent } from './components/select-files-dialog/select-files-dialog.component';
import { SelectUserDialogComponent } from './components/select-user-dialog/select-user-dialog.component';
import { SessionDetailComponent } from './components/session-detail/session-detail.component';
import { SessionOverviewInfoTabComponent } from './components/session-detail/session-overview-info-tab.component';
import { StandaloneSurveyAdditionalInformationTabComponent } from './components/standalone-survey/standalone-survey-additional-information-tab.component';
import { StandaloneSurveyAnswerPlayerComponent } from './components/standalone-survey-quiz-player/standalone-survey-answer-player.component';
import { StandaloneSurveyDateQuestionSelectionDialogComponent } from './components/standalone-survey-date-selection-dialog/standalone-survey-date-question-selection-dialog.component';
import { StandaloneSurveyEditModeService } from './services/standalone-survey-edit-mode.service';
import { StandaloneSurveyEditorComponent } from './components/standalone-survey/standalone-survey-editor.component';
import { StandaloneSurveyEditorPageComponent } from './components/standalone-survey/standalone-survey-editor-page.component';
import { StandaloneSurveyEditorPageService } from './services/standalone-survey-editor-page.service';
import { StandaloneSurveyGeneralTabComponent } from './components/standalone-survey/standalone-survey-general-tab.component';
import { StandaloneSurveyQuestionAdderComponent } from './components/standalone-survey/standalone-survey-question-adder.component';
import { StandaloneSurveyQuestionAnswerPlayerComponent } from './components/standalone-survey-quiz-player/standalone-survey-question-answer-player.component';
import { StandaloneSurveyQuestionEditorComponent } from './components/standalone-survey/standalone-survey-question-editor.component';
import { StandaloneSurveyQuestionOptionEditorComponent } from './components/standalone-survey/standalone-survey-question-option-editor.component';
import { StandaloneSurveyQuestionTemplateComponent } from './components/standalone-survey/standalone-survey-question-template.component';
import { StandaloneSurveyQuestionTypeSelectionService } from './services/standalone-survey-question-type-selection.service';
import { StandaloneSurveyQuizPlayerIntegrationsService } from './services/standalone-survey-quiz-player-integrations.service';
import { StandaloneSurveyQuizPlayerPageComponent } from './components/standalone-survey-quiz-player/standalone-survey-quiz-player-page.component';
import { StandaloneSurveyQuizPlayerPageService } from './services/standalone-survey-quiz-player-page.service';
import { StandaloneSurveyRepositoryPageService } from './services/standalone-survey-repository-page.service';
import { StandaloneSurveySectionEditorComponent } from './components/standalone-survey/standalone-survey-section-editor.component';
import { TargetAudienceTabComponent } from './components/course-detail/target-audience-tab.component';
import { ToolbarCenterDirective } from './fragments/app-toolbar/directives/toolbar-center.directive';
import { ToolbarLeftDirective } from './fragments/app-toolbar/directives/toolbar-left.directive';
import { ToolbarRightDirective } from './fragments/app-toolbar/directives/toolbar-right.directive';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { UploadCardComponent } from './components/upload-card/upload-card.component';
import { UploadsModule } from '@progress/kendo-angular-upload';
import { VideoAnnotationCommentsComponent } from './components/video-annotation/video-annotation-comments.component';
import { VideoAnnotationComponent } from './components/video-annotation/video-annotation.component';
import { VideoAnnotationPlayerComponent } from './components/video-annotation-player/video-annotation-player.component';
import { VideoChapterDetailComponent } from './components/video-annotation/video-chapter-detail.component';
import { VideoChapterListComponent } from './components/video-annotation/video-chapter-list.component';
import { VideoChapterListItemComponent } from './components/video-annotation/video-chapter-list-item.component';

const COMMON_FRAGMENTS: Type<unknown>[] = [HeaderFragment, AppToolbarFragment, NavigationMenuFragment, DetailContentFragment];

const COMMON_PIPES: Type<unknown>[] = [SecondToTimePipe];

const COMMON_DIRECTIVES: Type<unknown>[] = [
  HeaderTitleDirective,
  AppToolbarDirective,
  ToolbarCenterDirective,
  ToolbarLeftDirective,
  ToolbarRightDirective,
  DetailContentLeftDirective,
  DetailContentRightDirective,
  DetailContentDirective,
  KendoTreeviewMetadataAutoCheckAllChildsDirective,
  KendoBatchUploadFilesButtonDirective,
  WebTemplateDirective,
  MobileTemplateDirective,
  KendoTreeviewMetadataCustomSelectDeselectAllDirective,
  PermissionDirective
];

const COMMON_COMPONENT: Type<unknown>[] = [
  BroadcastMessageNotificationComponent,
  OpalFooterComponent,
  ContentDialogComponent,
  CommentDialogComponent,
  CourseCriteriaMaxParticipantSelect,
  CourseCriteriaRegistrationViolationDialogComponent,
  SelectCourseDialogComponent,
  SelectUserDialogComponent,
  DigitalContentPlayerComponent,
  ScormPlayerComponent,
  CopyrightFormComponent,
  CopyrightAttributionElementFormComponent,
  MainQuizPlayerPageComponent,
  StandaloneSurveyQuizPlayerPageComponent,
  MainFormStandalonePlayerPageComponent,
  FormQuestionAnswerPlayerComponent,
  StandaloneSurveyQuestionAnswerPlayerComponent,
  FormQuestionAnswerPlayerReviewComponent,
  QuestionAnswerReviewComponent,
  FormQuestionAnswerReviewComponent,
  FormAnswerPlayerComponent,
  StandaloneSurveyAnswerPlayerComponent,
  MetadataEditorComponent,
  ListLearningPathGridComponent,
  ListBlockoutDateGridComponent,
  ListCourseGridComponent,
  ListParticipantGridComponent,
  ListCoursePlanningCycleGridComponent,
  ListECertificateGridComponent,
  ListRegistrationGridComponent,
  OwnerInfoComponent,
  CourseDetailComponent,
  BlockoutDateDetailComponent,
  LearningPathDetailComponent,
  LearningPathMetadataTabComponent,
  BasicInfoTabComponent,
  LearningPathBasicInfoTabComponent,
  LearningPathPdOpportunitiesComponent,
  ProviderInfoTabComponent,
  CopyRightTabComponent,
  MetadataTabComponent,
  CourseAdministrationTabComponent,
  CoursePlanningTabComponent,
  TargetAudienceTabComponent,
  EvaluationEcertificateTabComponent,
  ListDigitalContentPageComponent,
  OpalOutletComponent,
  ListClassRunGridComponent,
  ListAttendanceTrackingGridComponent,
  ClassRunDetailComponent,
  CourseCriteriaDetailComponent,
  ClassRunOverviewInfoTabComponent,
  ClassRunPlanningTabComponent,
  LearnerRelatedCriteriaTabComponent,
  CourseRelatedCriteriaTabComponent,
  ClassApplicationTabComponent,
  SessionDetailComponent,
  SessionOverviewInfoTabComponent,
  ListSessionGridComponent,
  OpalReportComponent,
  OpalReportDynamicComponent,
  PreviewAudioPlayerComponent,
  PreviewDigitalContentComponent,
  PreviewDocumentPlayerComponent,
  PreviewVideoPlayerComponent,
  ListAssignmentGridComponent,
  ListParticipantAssignmentTrackGridComponent,
  CommentTabComponent,
  BreadcrumbComponent,
  FundingAndSubsidyComponent,
  LearnerBasicInfoComponent,
  LearnerMetadataInfoComponent,
  LearnerProfileComponent,
  AbsenceDetailDialogComponent,
  CoursePlanningCycleDetailComponent,
  CoursePlanningCycleOverviewInfoTabComponent,
  MailEditorComponent,
  QuestionTextOptionEditorComponent,
  QuestionBlankOptionEditorComponent,
  QuestionOptionImageUploadDialogComponent,
  AnswerFeedbackDialogComponent,
  LearningPathPdOpportunitiesDialogComponent,
  LearningPathSelectedSoursesComponent,
  LearningPathSharingComponent,
  LearningPathSharingDialogComponent,
  SelectFilesDialogComponent,
  MegaMenuComponent,
  BrokenLinkReportDialogComponent,
  BrokenLinkScannerDialogComponent,
  BrokenLinkReportTabComponent,
  AssignmentQuestionEditorComponent,
  AssignmentQuestionOptionEditorComponent,
  MainAssignmentPlayerComponent,
  QuestionTitleEditorComponent,
  FormSectionInfoComponent,
  CourseFilterComponent,
  BlockoutDateFilterComponent,
  ListAnnouncementGridComponent,
  AnnouncementDetailDialogComponent,
  CopyLearningPathHyperlinkComponent,
  UploadCardComponent,
  BatchUploadFilesDialogComponent,
  BatchFileUploaderComponent,
  PackageConfirmDialog,
  RegistrationFilterComponent,
  RichTextEditorComponent,
  OpalKendoEditorComponent,
  DeviceViewSimulationComponent,
  BlockOutDateDependenciesDetailDialog,
  VideoAnnotationComponent,
  VideoChapterListComponent,
  VideoChapterListItemComponent,
  VideoChapterDetailComponent,
  VideoAnnotationCommentsComponent,
  VideoAnnotationPlayerComponent,
  BlockoutDateWarningComponent,
  FormPollResultsComponent,
  PersonalFileListComponent,
  PersonalFileDialogComponent,
  PersonalFilePreviewDialogComponent,
  MetadataSelectDeselectAllNodeTemplateComponent,
  DigitalContentReferenceDialog,
  ECertificateDetailComponent,
  ECertificateTemplateCustomiseComponent,
  ECertificateLayoutSelectionComponent,
  FormAnswerAttachmentsComponent,
  PreviewEcertificateTemplateDialogComponent,
  PreviewFormDialogComponent,
  MainAssessmentPlayerComponent,
  AssessmentScaleComponent,
  AssessmentCriteriaComponent,
  ListBadgeLearnerGridComponent,
  MyAchievementsDialogComponent,
  StandaloneSurveyEditorPageComponent,
  StandaloneSurveyQuestionOptionEditorComponent,
  StandaloneSurveyQuestionEditorComponent,
  StandaloneSurveySectionEditorComponent,
  StandaloneSurveyEditorComponent,
  StandaloneSurveyQuestionAdderComponent,
  StandaloneSurveyQuestionTemplateComponent,
  StandaloneSurveyDateQuestionSelectionDialogComponent,
  StandaloneSurveyAdditionalInformationTabComponent,
  StandaloneSurveyGeneralTabComponent
];

@NgModule({
  imports: [
    FunctionModule,
    MenuModule,
    CommonModule,
    CommonComponentsModule,
    AmazonS3UploaderModule,
    ContextMenuModule,
    TreeViewModule,
    GridModule,
    ButtonModule,
    TextBoxModule,
    DropDownsModule,
    AuthenticationModule,
    DocumentViewerModule,
    ImageCropperModule,
    DateInputsModule,
    InputsModule,
    DatePickerModule,
    TimePickerModule,
    ButtonsModule,
    TaggingDomainApiModule,
    ContentDomainApiModule,
    CommentDomainApiModule,
    FormDomainApiModule,
    LearnerDomainApiModule,
    CourseDomainApiModule,
    UserDomainApiModule,
    OrganizationDomainApiModule,
    NumericTextBoxModule,
    PopupModule,
    TooltipModule,
    EditorModule,
    UploadsModule,
    TabStripModule,
    FormParticipantDomainApiModule,
    PersonalSpaceApiModule,
    PanelBarModule,
    CalendarDomainApiModule,
    PersonalSpaceApiModule
  ],
  declarations: [...COMMON_FRAGMENTS, ...COMMON_PIPES, ...COMMON_DIRECTIVES, ...COMMON_COMPONENT, BatchUploadFileContentComponent],
  entryComponents: [
    ...COMMON_FRAGMENTS,
    LearningPathPdOpportunitiesDialogComponent,
    BrokenLinkReportDialogComponent,
    BrokenLinkScannerDialogComponent,
    CommentDialogComponent,
    CourseCriteriaRegistrationViolationDialogComponent,
    SelectCourseDialogComponent,
    SelectUserDialogComponent,
    SelectFilesDialogComponent,
    CourseFilterComponent,
    BlockoutDateFilterComponent,
    AbsenceDetailDialogComponent,
    SelectCourseDialogComponent,
    QuestionOptionImageUploadDialogComponent,
    AnswerFeedbackDialogComponent,
    AnnouncementDetailDialogComponent,
    BatchUploadFilesDialogComponent,
    RegistrationFilterComponent,
    RichTextEditorComponent,
    OpalKendoEditorComponent,
    BlockOutDateDependenciesDetailDialog,
    PackageConfirmDialog,
    PersonalFileDialogComponent,
    PersonalFilePreviewDialogComponent,
    MetadataSelectDeselectAllNodeTemplateComponent,
    DigitalContentReferenceDialog,
    PreviewEcertificateTemplateDialogComponent,
    LearningPathSharingDialogComponent,
    PreviewFormDialogComponent,
    MyAchievementsDialogComponent
  ],
  exports: [...COMMON_FRAGMENTS, ...COMMON_PIPES, ...COMMON_DIRECTIVES, ...COMMON_COMPONENT]
})
export class DomainComponentsModule {
  public static forRoot(): ModuleWithProviders[] {
    return [
      {
        ngModule: DomainComponentsModule,
        providers: [
          MainQuizPlayerPageService,
          MetadataEditorService,
          HeaderService,
          AppToolbarService,
          OpalViewportService,
          QuizPlayerIntegrationsService,
          NavigationMenuService,
          DetailContentService,
          DigitalContentListPageService,
          OpalFooterService,
          ListLearningPathGridComponentService,
          ListBlockoutDateGridComponentService,
          ListCourseGridComponentService,
          FormParticipantListService,
          ListClassRunGridComponentService,
          ListRegistrationGridComponentService,
          ListParticipantAssignmentTrackGridComponentService,
          ListSessionGridComponentService,
          ListAttendanceTrackingGridComponentService,
          ListCoursePlanningCycleGridComponentService,
          ListECertificateGridComponentService,
          ListBadgeLearnerGridComponentService,
          NavigationPageService,
          BreadcrumbService,
          LearnerProfileVmService,
          FormEditorPageService,
          ListAssignmentGridComponentService,
          AssignmentPlayerIntegrationsService,
          ListAnnouncementGridComponentService,
          StandaloneSurveyEditorPageService,
          StandaloneSurveyQuizPlayerPageService,
          StandaloneSurveyQuizPlayerIntegrationsService,
          AssessmentPlayerIntegrationsService,
          StandaloneSurveyEditModeService,
          StandaloneSurveyQuestionTypeSelectionService,
          StandaloneSurveyRepositoryPageService
        ]
      }
    ];
  }
}
