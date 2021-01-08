//#region Root
export { DomainComponentsModule } from './domain-components.module';
export { OpalOutletComponent } from './opal-outlet.component';
//#endregion

//#region Components
export { OpalFooterComponent } from './components/opal-footer/opal-footer.component';
export { ContentDialogComponent, ContentDialogComponentTabType } from './components/content-dialog/content-dialog.component';
export { SelectFilesDialogComponent } from './components/select-files-dialog/select-files-dialog.component';
export { CommentDialogComponent } from './components/comment-dialog/comment-dialog.component';
export {
  CourseCriteriaMaxParticipantSelect
} from './components/course-criteria-max-participant-select/course-criteria-max-participant-select.component';
export {
  CourseCriteriaRegistrationViolationDialogComponent
} from './components/course-criteria-registration-violation-dialog/course-criteria-registration-violation-dialog.component';
export { DigitalContentPlayerComponent } from './components/digital-content-player/digital-content-player.component';
export { ScormPlayerComponent } from './components/scorm-player/scorm-player.component';
export { CopyrightFormComponent } from './components/copyright-form/copyright-form.component';
export { PreviewAudioPlayerComponent } from './components/preview-digital-content/preview-audio-player.component';
export { PreviewDigitalContentComponent } from './components/preview-digital-content/preview-digital-content.component';
export { PreviewDocumentPlayerComponent } from './components/preview-digital-content/preview-document-player.component';
export { PreviewVideoPlayerComponent } from './components/preview-digital-content/preview-video-player.component';
export { OpalReportDynamicComponent } from './components/opal-report-dynamic/opal-report-dynamic.component';
export { AbsenceDetailDialogComponent } from './components/absence-detail-dialog/absence-detail-dialog.component';
export { SelectCourseDialogComponent } from './components/select-course-dialog/select-course-dialog.component';
export { SelectUserDialogComponent } from './components/select-user-dialog/select-user-dialog.component';
export { BrokenLinkReportTabComponent } from './components/broken-link-report-tab/broken-link-report-tab.component';
export { QuestionTitleEditorComponent } from './components/question-title-editor/question-title-editor.component';
export { RichTextEditorComponent } from './components/rich-text-editor/rich-text-editor.component';
export { OpalKendoEditorComponent } from './components/opal-kendo-editor/opal-kendo-editor-component';
export { BatchUploadFilesDialogComponent } from './components/batch-upload-files-dialog/batch-upload-files-dialog.component';
export { PersonalFileListComponent } from './components/personal-space-list/personal-file-list.component';
export { PersonalFileDialogComponent } from './components/personal-file-dialog/personal-file-dialog.component';
export { UploadCardComponent } from './components/upload-card/upload-card.component';
export {
  BlockOutDateDependenciesDetailDialog
} from './components/blockout-date-dependencies-detail-dialog/blockout-date-dependencies-detail-dialog.component';
export {
  PreviewEcertificateTemplateDialogComponent
} from './components/preview-ecertificate-template-dialog/preview-ecertificate-template-dialog.component';
export { PreviewFormDialogComponent } from './components/preview-form-dialog/preview-form-dialog.component';
//#endregion

//#region Directives
export { KendoTreeviewMetadataAutoCheckAllChildsDirective } from './directives/kendo-treeview-metadata-auto-check-all-childs.directive';
export { PermissionDirective } from './directives/permission.directive';
//#endregion

//#region Digital Content
export { IDigitalContentSearchRequest } from './dtos/digital-content-search-request';
export { IDigitalContentSearchResult } from './dtos/digital-content-search-result';
export { ListDigitalContentPageComponent } from './components/list-digital-content-page/list-digital-content-page.component';
export { DigitalContentViewModel } from './models/digital-content-view.model';
export { DIGITAL_CONTENT_STATUS_MAPPING } from './models/digital-content-status.model';
export { DigitalContentDetailMode } from './models/digital-content-detail-mode.model';
export { DigitalContentDetailViewModel } from './view-models/digital-content-detail-view.model';
export { LectureContentViewModel } from './view-models/lecture-content-view.model';
export { IOpalReportDynamicParams, OpalReportDynamicParamsSchema } from './models/opal-report-dynamic-params.model';
export { DigitalContentReferenceDialog } from './components/digital-content-reference-dialog/digital-content-reference-dialog.component';
//#endregion

//#region Scorm Player
export { IScormPlayerParameters, ScormPlayerMode } from './models/scorm-player.model';
//#endregion

////#region Standalone Survey Quiz player
export {
  StandaloneSurveyQuizPlayerPageComponent
} from './components/standalone-survey-quiz-player/standalone-survey-quiz-player-page.component';
export { StandaloneSurveyEditorPageComponent } from './components/standalone-survey/standalone-survey-editor-page.component';
export {
  StandaloneSurveyAdditionalInformationTabComponent
} from './components/standalone-survey/standalone-survey-additional-information-tab.component';
export { StandaloneSurveyEditModeService } from './services/standalone-survey-edit-mode.service';
export { StandaloneSurveyQuestionTypeSelectionService } from './services/standalone-survey-question-type-selection.service';
export {
  StandaloneSurveyRepositoryPageService,
  StandaloneSurveyRepositoryPageFormListData
} from './services/standalone-survey-repository-page.service';

////#endregion

//#region Quiz Player
export { FormAnswerPlayerComponent } from './components/quiz-player/form-answer-player.component';
export { FormQuestionAnswerPlayerComponent } from './components/quiz-player/form-question-answer-player.component';
export { MainQuizPlayerPageComponent } from './components/quiz-player/main-quiz-player-page.component';
export { MainFormStandalonePlayerPageComponent } from './components/form-standalone-player/main-form-standalone-player-page.component';
export { FormQuestionAnswerPlayerReviewComponent } from './components/quiz-player/form-question-answer-player-review.component';
export { QuestionAnswerReviewComponent } from './components/quiz-player/question-answer-review.component';
export { FormQuestionAnswerReviewComponent } from './components/quiz-player/form-question-answer-review.component';

export { MainQuizPlayerPageService } from './services/main-quiz-player-page.service';
//#endregion

//#region Video Annotation
export { VideoAnnotationMode } from './view-models/video-annotation-view.model';
//#endregion

//#region Form
export { ISearchFormResponse, SearchFormResponse, SearchFormRequest } from './dtos/form.dtos';
export { ListParticipantGridComponent } from './components/list/participant-grid/participant-grid.component';
//#endregion

//#region Metadata
export { ResourceMetadataFormModel } from './models/resource-metadata-form.model';
export { MetadataEditorComponent } from './components/metadata/metadata-editor.component';
export { MetadataEditorService } from './services/metadata-editor.service';
//#endregion

//#region Validators
export {
  copyrightExpiryDateValidatorType,
  copyrightExpiryDateValidator,
  copyrightValidateExpiryDate
} from './validators/copyright-expiry-date-validator';
export {
  copyrightStartDateValidatorType,
  copyrightStartDateValidator,
  copyrightValidateStartDate
} from './validators/copyright-start-date-validator';
//#endregion

//#region Models
export { CopyrightFormModel, ICopyrightFormModelSelectItem } from './models/copyright-form.model';
export { SelectCourseModel } from './models/select-course.model';
export { MenuEmit } from './models/menu-emit.model';
export { INavigationMenuItem } from './models/navigation-menu.model';
export { LEARNING_PATH_STATUS_COLOR_MAP } from './models/learning-path-status-color-map.model';
export { COURSE_IN_COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP, COURSE_STATUS_COLOR_MAP } from './models/course-status-color-map.model';
export { CONTENT_STATUS_COLOR_MAP } from './models/content-status-color-map.model';
export { CLASSRUN_STATUS_COLOR_MAP } from './models/classrun-status-color-map.model';
export { FORM_STATUS_COLOR_MAP } from './models/form-status-color-map.model';
export {
  REGISTRATION_STATUS_COLOR_MAP,
  WITHDRAWAL_STATUS_COLOR_MAP,
  REGISTRATION_CHANGE_CLASSRUN_STATUS_COLOR_MAP,
  REGISTRATION_LEARNING_STATUS_COLOR_MAP,
  REGISTRATION_STATUS_ADDING_PARTICIPANTS_COLOR_MAP
} from './models/course-registration-status-color-map.model';
export { COURSE_REGISTRATION_STATUS_PREFIX_MAP } from './models/course-registration-type.model';
export { ATTENDANCE_TRACKING_STATUS_COLOR_MAP } from './models/attendance-tracking-status-color-map.model';
export { COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP } from './models/course-planning-cycle-status-color-map.model';
export { STANDALONE_SURVEY_STATUS_COLOR_MAP } from './models/standalone-survey-status-color-map.model';
export { ECERTIFICATE_STATUS_COLOR_MAP } from './models/ecertificate-status-color-map.model';
export { ContextMenuAction } from './models/context-menu-action.model';
export { IRowCallbackModel } from './models/row-callback.model';
export { IDictionary } from './models/dictionary.model';
export { NavigationData } from './models/navigation-data.model';
export { LearningPathDetailViewModel } from './view-models/learning-path-detail-view.model';
export { LearningPathViewModel } from './models/learning-path-view.model';
export { BlockoutDateViewModel } from './models/blockout-date-view.model';
export { LearningPathCourseViewModel } from './view-models/learning-path-course-view.model';
export { CourseViewModel } from './models/course-view.model';
export { CourseReviewViewModel } from './models/course-review-view.model';
export { CoursePlanningCycleViewModel } from './models/course-planning-cycle-view.model';
export { ECertificateViewModel } from './models/ecertificate-view.model';
export { DigitalContentContextMenuEmit } from './models/digital-content-context-menu-emit.model';
export { buildPrerequisiteEcertificateTypeSelectItems } from './models/prerequisite-ecertificate-type-select-item.model';
export { SectionDetailViewModel } from './view-models/section-detail-view.model';
export { FormDetailMode } from './models/form-detail-mode.model';
export { CommentTabInput } from './models/comment-tab-input.model';
export { BreadcrumbItem } from './models/breadcrumb-item.model';
export { PermissionsTermsOfUse } from './models/permissions-terms-of-use.model';
export { LearningPathDetailMode } from './models/learning-path-detail-mode.model';
export { INavigationFilterCriteria } from './models/navigation-filter-criteria.model';
export { LearningPathTabInfo } from './models/learning-path-tab.model';
export { PreviewMode } from './models/preview-mode.model';
export { ParticipantAssignmentTrackViewModel } from './models/participant-assignment-track-view.model';
export { PARTICIPANT_ASSIGNMENT_TRACK_STATUS_COLOR_MAP } from './models/participant-assignment-track-status-color-map.model';
export { StandaloneSurveyDetailMode } from './models/standalone-survey-detail-mode.model';
//#endregion

//#region  Constant
export { CAMMenu } from './module-constants/cam/menu';
export { CAMRoutePaths } from './module-constants/cam/route-path';
export { CAMTabConfiguration } from './module-constants/cam/tab';
export { LMMMenu } from './module-constants/lmm/menu';
export { LMMRoutePaths } from './module-constants/lmm/route-path';
export { LMMTabConfiguration } from './module-constants/lmm/tab';
export { CCPM_PERMISSIONS } from './module-constants/ccpm/ccpm-permission.constant';
export { LEARNER_PERMISSIONS } from './module-constants/learner/learner-permission.constant';
export { StandaloneSurveyRoutePaths } from './module-constants/standalone-survey/route-path';
//#endregion

//#region Fragments
export { AppToolbarDirective } from './fragments/app-toolbar/directives/app-toolbar.directive';
export { ToolbarLeftDirective } from './fragments/app-toolbar/directives/toolbar-left.directive';
export { ToolbarCenterDirective } from './fragments/app-toolbar/directives/toolbar-center.directive';
export { ToolbarRightDirective } from './fragments/app-toolbar/directives/toolbar-right.directive';
export { AppToolbarFragment } from './fragments/app-toolbar/app-toolbar.fragment';
export { AppToolbarService } from './fragments/app-toolbar/app-toolbar.service';

export { HeaderTitleDirective } from './fragments/header/header-title.directive';
export { HeaderFragment } from './fragments/header/header.fragment';
export { HeaderService } from './fragments/header/header.service';

export { DetailContentDirective } from './fragments/detail-content/directives/detail-content.directive';
export { DetailContentLeftDirective } from './fragments/detail-content/directives/toolbar-left.directive';
export { DetailContentRightDirective } from './fragments/detail-content/directives/toolbar-right.directive';
export { DetailContentFragment } from './fragments/detail-content/detail-content.fragment';
export { DetailContentService } from './fragments/detail-content/detail-content.service';

export { NavigationMenuFragment } from './fragments/navigation-menu/navigation-menu.fragment';
export { NavigationMenuService } from './fragments/navigation-menu/navigation-menu.service';
export { FragmentPosition } from './fragments/fragment-position';
//#endregion

//#region Services
export { OpalViewportService } from './services/viewport.service';
export { OpalFooterService } from './services/opal-footer.services';
export { ListLearningPathGridComponentService } from './services/list-learning-path-grid-component.service';
export { QuizPlayerIntegrationsService } from './services/quiz-player-integrations.service';
export { ListCourseGridComponentService } from './services/list-course-grid-component.service';
export { ListClassRunGridComponentService } from './services/list-classrun-grid-component.service';
export { ListAttendanceTrackingGridComponentService } from './services/list-attendance-tracking.service';
export { ListCoursePlanningCycleGridComponentService } from './services/list-course-planning-cycle-grid-component.service';
export { NavigationPageService } from './services/navigation-page.service';
export { ListRegistrationGridComponentService } from './services/list-registration-component.service';
export { BreadcrumbService } from './services/breadcrumb.service';
export { DigitalContentSearchTermService } from './services/digital-content-search-term.service';
export { FormSearchTermService } from './services/form-search-term.service';
export { PersonalSpaceSearchTermService } from './services/personal-space-search-term.service';
export { LearnerProfileVmService } from './services/learner-profile-vm.service';
export { FormEditorPageService } from './services/form-editor-page.service';
export { AssignmentPlayerIntegrationsService } from './services/assignment-player-integrations.service';
export { FormParticipantListService } from './services/form-participant-list.service';
export { StandaloneSurveySearchTermService } from './services/standalone-survey-search-term.service';
export { StandaloneSurveyEditorPageService } from './services/standalone-survey-editor-page.service';
export { QuestionBankListService } from './services/question-bank-list.service';
//#endregion

//#region Course Management Page
export { LearningPathBasicInfoTabComponent } from './components/learning-path-detail/learning-path-basic-info-tab.component';
export { ListLearningPathGridComponent } from './components/list-learning-path-grid/list-learning-path-grid.component';
export { ListCourseGridComponent } from './components/list-course-grid/list-course-grid.component';
export { ListRegistrationGridComponent } from './components/list-registration-grid/list-registration-grid.component';
export { ListAssignmentGridComponent } from './components/list-assignment-grid/list-assignment-grid.component';
export { ListParticipantAssignmentTrackGridComponent } from './components/list-assignment-track-grid/list-assignment-track-grid.component';
export { ListCoursePlanningCycleGridComponent } from './components/list-course-planning-cycle/list-course-planning-cycle-grid.component';
export { CourseDetailViewModel } from './view-models/course-detail-view.model';
export { CourseCriteriaDetailViewModel } from './view-models/course-criteria-detail-view.model';
export { AttendanceTrackingDetailViewModel } from './view-models/attendance-tracking-detail-view.model';
export { ClassRunDetailViewModel } from './view-models/classrun-detail-view.model';
export { LearningPathDetailComponent } from './components/learning-path-detail/learning-path-detail.component';
export { LearningPathMetadataTabComponent } from './components/learning-path-detail/learning-path-metadata-tab.component';
export { CourseDetailComponent } from './components/course-detail/course-detail.component';
export { CourseDetailMode } from './models/course-detail-mode.model';
export { CourseCriteriaDetailMode } from './models/course-criteria-detail-mode.model';
export { ContextMenuEmit } from './models/context-menu-emit.model';
export { ClassRunViewModel, IClassRunViewModel } from './models/classrun-view.model';
export { AssignmentViewModel, IAssignmentViewModel } from './models/assignment-view.model';
export { AssignmentDetailViewModel } from './view-models/assignment-detail-view.model';
export { ListAssignmentGridComponentService } from './services/list-assignment-grid-component.service';
export { ClassRunDetailMode, showClassRunDetailViewOnly } from './models/classrun-detail-mode.model';
export { AssignmentDetailMode } from './models/assignment-detail-mode.model';
export { AttendanceTrackingViewModel } from './models/attendance-tracking-view.model';
export { RegistrationViewModel } from './models/registration-view.model';
export {
  ICourseCriteriaRegistrationViolationDetailItemViewModel,
  CourseCriteriaRegistrationViolationDetailItemViewModel
} from './models/course-criteria-registration-violation-detail-item-view.model';
export { SessionDetailViewModel } from './view-models/session-detail-view.model';
export { ECertificateTemplateDetailViewModel } from './view-models/ecertificate-template-detail-view.model';
export { BlockoutDateDetailViewModel } from './view-models/blockout-date-detail-view.model';
export { SessionViewModel } from './models/session-view.model';
export { SessionDetailMode } from './models/session-detail-mode.model';
export { IDialogActionEvent } from './models/dialog-action.model';
export { RescheduleClassRunDetailViewModel } from './view-models/reschedule-classrun-detail-view.model';
export { RouterPageInput, RouterPageInputExt } from './models/router-info.model';
export { LearnerProfileViewModel } from './view-models/learner-profile-view.model';
export { ListCourseGridDisplayColumns } from './components/list-course-grid/list-course-grid.component';
export { ListClassrunGridDisplayColumns } from './components/list-classrun-grid/list-classrun-grid.component';
export { ListBlockoutDateGridDisplayColumns } from './components/list-blockout-date-grid/list-blockout-date-grid.component';
export { ListRegistrationGridDisplayColumns } from './components/list-registration-grid/list-registration-grid.component';
export { ListSessionGridDisplayColumns } from './components/list-session-grid/list-session-grid.component';
export { CoursePlanningCycleDetailMode } from './models/course-planning-cycle-detail-mode.model';
export { BlockoutDateDetailMode } from './models/blockout-date-detail-mode.model';
export { CoursePlanningCycleDetailViewModel } from './view-models/course-planning-cycle-detail-view.model';
export { ECertificateTemplateDetailMode } from './models/ecertificate-template-detail-mode.model';
export { MailTag } from './models/mail-tag.model';
export {
  QuestionOptionImageUploadDialogComponent
} from './components/question-option-image-upload-dialog/question-option-image-upload-dialog.component';
export { IQuestionOptionImageUploadSettings, QuestionOptionImageUploadSettings } from './models/question-option-image-upload-setting.model';
export { AnswerFeedbackDialogComponent } from './components/answer-feedback-dialog/answer-feedback-dialog.component';
export { ISelectUserDialogResult, SelectUserDialogResult } from './models/select-user-dialog-result.model';
export { AssignmentQuestionEditorComponent } from './components/assignment-player/assignment-question-editor.component';
export { AssignmentQuestionOptionEditorComponent } from './components/assignment-player/assignment-question-option-editor.component';
export { MainAssignmentPlayerComponent } from './components/assignment-player/main-assignment-player.component';
export { ParticipantAssignmentTrackDetailViewModel } from './view-models/participant-assignment-track-detail-view.model';
export { AssignmentMode } from './models/assignment-mode.model';
export { CourseFilterComponent } from './components/course-filter/course-filter.component';
export { BlockoutDateFilterComponent } from './components/blockout-date-filter/blockout-date-filter.component';
export { CourseFilterModel } from './models/course-filter.model';
export { BlockoutDateFilterModel } from './models/blockout-date-filter.model';
export { MY_ASSIGNMENT_STATUS_COLOR_MAP } from './models/my-assignment-status-color-map.model';
export { ANNOUNCEMENT_STATUS_COLOR_MAP } from './models/announcement-status-color-map.model';
export { AnnouncementFilterModel } from './models/announcement-filter.model';
export { AnnouncementViewModel } from './models/announcement-view.model';
export { RegistrationFilterComponent } from './components/registration-filter/registration-filter.component';
export { RegistrationFilterModel } from './models/registration-filter.model';
export { ICourseFilterSetting } from './models/course-filter-setting.model';
export { IDownloadTemplateFormat, IDownloadTemplateOption } from './models/download-template-file.model';
export { PersonalFilePreviewDialogComponent } from './components/personal-file-preview-dialog/personal-file-preview-dialog.component';
export { LearningPathSharingDialogComponent } from './components/learning-path-detail/learning-path-sharing-dialog.component';
export { MyAchievementsDialogComponent } from './components/my-achievements-dialog/learning-my-achievements-dialog.component';
export { MainAssessmentPlayerComponent } from './components/assessment-player/main-assessment-player.component';
export { BLOCKOUT_DATE_STATUS_COLOR_MAP } from './models/blockout-date-status-color-map.model';
export { ListParticipantAssignmentTrackGridComponentService } from './services/list-participant-assignment-track-component.service';
export { CommonFilterAction } from './models/common-filter-action.model';
export { VideoAnnotationCommentInfo } from './view-models/video-annotation-view.model';
export { IAssessmentPlayerInput } from './models/assessment-player-input.model';
export { AssessmentCriteriaComponent } from './components/assessment-player/assessment-criteria.component';
export { AssessmentAnswerDetailViewModel } from './view-models/assessment-answer-detail-view.model';
export { DigitalBadgesFilterModel } from './models/digital-badges-filter.model';
//#endregion

//#region Comment tab component
export { CommentTabComponent } from './components/comment-tab/comment-tab.component';
//#endregion

//#region Digital paging
export { ListBadgeLearnerGridComponentService } from './services/list-badge-learner-grid-component.service';
export { ListBadgeLearnerGridDisplayColumns } from './components/list-badge-learner-grid/list-badge-learner-grid.component';
export { YearlyUserStatisticViewModel } from './models/yearly-user-statistic-view.model';
//#endregion

//#region Others

//#region Helpers
export { PlayerHelpers } from './helpers/player.helper';
export { EncryptUrlPathHelper } from './helpers/encrypt-url-path.helper';
export { WebAppLinkBuilder } from './helpers/webapp-link-builder.helper';
export { Task } from './helpers/task-queue.helper';
export { DiffHelper, IDiffResult } from './helpers/diff-string.helper';
export { FileUploaderHelpers } from './helpers/file-uploader.helper';
//#endregion

export {
  PLAYER_ACCESS_TOKEN_KEY,
  PLAYER_CONTENT_ID_KEY,
  PLAYER_LECTURE_ID_KEY,
  PLAYER_CLASSRUN_ID_KEY,
  PLAYER_DISPLAY_MODE_KEY,
  PLAYER_MY_LECTURE_ID_KEY,
  PLAYER_DISABLE_FULLSCREEN_KEY,
  PLAYER_FULLSCREEN_CALLBACK_KEY,
  PLAYER_LOCAL_STORAGE_KEYS
} from './domain-components.constants';
//#endregion

//#region modules-constants
export { LearnerRoutePaths } from './module-constants/learner/learner-route-paths';
export { MyLearningTab } from './module-constants/learner/my-learning-tab';
//#endregion
