import {
  AccessRightType,
  AnswerFeedbackDisplayOption,
  BrokenLinkModuleIdentifier,
  CommentApiService,
  CommentServiceType,
  FormConfiguration,
  FormModel,
  FormParticipantType,
  FormQuestionModel,
  FormSectionsQuestions,
  FormStatus,
  FormType,
  IRevertVersionTrackingResult,
  ISaveResourceMetadataRequest,
  LearningContentApiService,
  ResourceModel,
  SystemRoleEnum,
  TaggingApiService,
  UserInfoModel,
  VersionTrackingType
} from '@opal20/domain-api';
import {
  AppToolbarService,
  BrokenLinkReportTabComponent,
  CCPM_PERMISSIONS,
  CommentTabInput,
  FORM_STATUS_COLOR_MAP,
  FormDetailMode,
  FormEditorPageService,
  HeaderService,
  NavigationMenuService,
  OpalFooterService,
  PreviewMode
} from '@opal20/domain-components';
import {
  BasePageComponent,
  DateUtils,
  ModuleFacadeService,
  NotificationType,
  ScheduledTask,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import { Component, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { DetailTitleComponent, DetailTitleSettings } from '@opal20/common-components';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import { forkJoin, of } from 'rxjs';

import { AccessRightTabComponent } from './tabs/access-right-tab/access-right-tab.component';
import { AuditLogTabComponent } from './tabs/audit-log-tab/audit-log-tab.component';
import { CCPMRoutePaths } from '../ccpm.config';
import { Constants } from '../constants/ccpm.common.constant';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormAdditionalInformationTabComponent } from './form-additional-information-tab.component';
import { FormAssessmentRubricManagementPage } from './form-assessment-rubric-management-page.component';
import { FormEditModeService } from '../services/form-edit-mode.service';
import { FormEditorPageComponent } from './form-editor-page.component';
import { FormRepositoryPageService } from '../services/form-repository-page.service';
import { IFormEditorPageNavigationData } from '../ccpm-navigation-data';
import { TransferOwnershipDialogComponent } from './dialogs/transfer-ownership-dialog.component';
import { XmlEntities } from 'html-entities';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'form-detail-page',
  templateUrl: './form-detail-page.component.html',
  encapsulation: ViewEncapsulation.None
})
export class FormDetailPageComponent extends BasePageComponent {
  public formPermission = CCPM_PERMISSIONS;
  public get commentTabInput(): CommentTabInput {
    return {
      originalObjectId: this.formData.originalObjectId,
      commentServiceType: CommentServiceType.Form,
      hasReply: true
    };
  }

  public removeUnsupportedQuestionTypes = FormQuestionModel.removeUnsupportedQuestionTypes;
  public statusColorMap = FORM_STATUS_COLOR_MAP;
  public navigationData: IFormEditorPageNavigationData | undefined;
  public previewOptions: unknown[];
  public detailTitleSettings: DetailTitleSettings;
  public statusItems: IDataItem[];
  public draftStatusItems: IDataItem[];
  public urlSubIcon: string;
  public get formStatusList(): IDataItem[] {
    return !this.formStatus || this.formStatus.value === FormStatus.Draft ? this.draftStatusItems : this.statusItems;
  }

  @ViewChild(FormAssessmentRubricManagementPage, { static: false })
  public assessmentRubricManagmentPage: FormAssessmentRubricManagementPage;
  @ViewChild(FormEditorPageComponent, { static: false }) public formEditorPage: FormEditorPageComponent;
  @ViewChild(DetailTitleComponent, { static: false }) public detailTitleForm: DetailTitleComponent;
  @ViewChild(FormAdditionalInformationTabComponent, { static: false }) public additionalInfoTab: FormAdditionalInformationTabComponent;
  @ViewChild(BrokenLinkReportTabComponent, { static: false }) public brokenLinkReportTab: BrokenLinkReportTabComponent;

  @ViewChild(TabStripComponent, { static: false })
  public formTabs: TabStripComponent;

  public currentUser: UserInfoModel;
  public previewMode: PreviewMode = PreviewMode.None;
  public formStatus: IDataItem;
  public formConfig: FormConfiguration = new FormConfiguration();
  public savedFormData: FormModel = new FormModel();
  public savedFormSectionsQuestions: FormSectionsQuestions = new FormSectionsQuestions({ formQuestions: [], formSections: [] });
  public formData: FormModel = new FormModel();
  public selectedQuestionid: string | undefined;
  public selectedFormQuestion: FormQuestionModel | undefined;
  public isWebPreview: boolean = false;
  public FormStatus: typeof FormStatus = FormStatus;
  public isFormQuestionsDataLoaded: boolean = false;
  public isFormDataLoaded: boolean = false;
  public resource: ResourceModel = new ResourceModel();
  public savedResource: ResourceModel = new ResourceModel();

  // Broken link properties
  public brokenLinkModule: BrokenLinkModuleIdentifier = BrokenLinkModuleIdentifier.Form;

  // Versioning properties
  @ViewChild(AuditLogTabComponent, { static: false })
  public auditLogTab: AuditLogTabComponent;
  public versionTrackingType: VersionTrackingType = VersionTrackingType.Form;
  public PreviewMode: typeof PreviewMode = PreviewMode;
  // Comments properties
  public commentFormType: CommentServiceType = CommentServiceType.Form;
  public titleApprovalDialog: string;
  public titleRejectDialog: string;

  public dialogRejectRef: DialogRef;
  public dialogApprovalRef: DialogRef;

  public readonly MAX_LENGTH_TITLE: number = Constants.MAX_LENGTH_FORM_TITLE;

  // Access rights properties
  @ViewChild(AccessRightTabComponent, { static: false })
  public accessRightTab: AccessRightTabComponent;
  public accessRightType: AccessRightType = AccessRightType.Form;

  // stand alone properties
  public formParticipantType: FormParticipantType = FormParticipantType.Form;

  private _mode: FormDetailMode = FormDetailMode.View;
  private _formSectionsQuestions: FormSectionsQuestions = new FormSectionsQuestions({ formQuestions: [], formSections: [] });
  private autoSaveFormScheduleTaskId = 'auto-save-form';
  private isSubmitting: boolean = false;
  private pollIcon: string = 'assets/images/icons/poll.svg';
  private surveyIcon: string = 'assets/images/icons/survey.svg';
  private quizIcon: string = 'assets/images/icons/quiz.svg';

  @ViewChild('dialogRejectCommentTemplate', { static: false })
  private dialogRejectCommentTemplate: TemplateRef<unknown>;

  @ViewChild('dialogApprovalCommentTemplate', { static: false })
  private dialogApprovalCommentTemplate: TemplateRef<unknown>;

  public get formSectionsQuestions(): FormSectionsQuestions {
    return this._formSectionsQuestions;
  }

  public set formSectionsQuestions(v: FormSectionsQuestions) {
    this._formSectionsQuestions = v;
    const selectedQuestionIndex = this.formSectionsQuestions.formQuestions.findIndex(question => question.id === this.selectedQuestionid);
    if (selectedQuestionIndex > -1) {
      this.selectedFormQuestion = this.formSectionsQuestions.formQuestions[selectedQuestionIndex];
    }
  }

  public get mode(): FormDetailMode {
    return this._mode;
  }

  public set mode(v: FormDetailMode) {
    this._mode = v;
    this.formEditModeService.modeChanged.next(this._mode);
    this.formEditModeService.initMode = this._mode;
  }

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected headerService: HeaderService,
    protected formEditorPageService: FormEditorPageService,
    private navigationMenuService: NavigationMenuService,
    private opalFooterService: OpalFooterService,
    private appToolbarService: AppToolbarService,
    private commentApiService: CommentApiService,
    private formEditModeService: FormEditModeService,
    private formRepositoryPageService: FormRepositoryPageService,
    private learningContentApiService: LearningContentApiService,
    private taggingApiService: TaggingApiService
  ) {
    super(moduleFacadeService);
    this.registerFormAutoSaveScheduledTask(this.autoSaveFormScheduleTaskId);
  }

  public get canSaveForm(): boolean {
    return (
      this.mode === FormDetailMode.Edit &&
      (this.formData.status === FormStatus.Draft ||
        this.formData.status === FormStatus.Rejected ||
        this.formData.status === FormStatus.Unpublished ||
        this.formData.status === FormStatus.Approved)
    );
  }

  public get canSubmitForApproval(): boolean {
    return (
      (this.mode === FormDetailMode.Edit || this.mode === FormDetailMode.View) &&
      (this.formData.status === FormStatus.Draft || this.formData.status === FormStatus.Rejected)
    );
  }

  public get canApprove(): boolean {
    return this.mode === FormDetailMode.ForApprover && this.formData.status === FormStatus.PendingApproval;
  }

  public get canPublish(): boolean {
    return this.mode !== FormDetailMode.ForApprover && this.formData.status === FormStatus.Approved;
  }

  public get canUnpublish(): boolean {
    return (
      this.mode !== FormDetailMode.ForApprover && this.formData.status === FormStatus.Published && this.formData.canUnpublishFormStandalone
    );
  }

  public get canShowMarkAsReadyToUseButton(): boolean {
    return this.formData.status === FormStatus.Draft && !this.formData.isStandalone;
  }

  public get canShowMarkAsDraftButton(): boolean {
    return this.formData.status === FormStatus.ReadyToUse;
  }

  public get canShowArchiveButton(): boolean {
    return (
      this.isOwner &&
      this.mode !== FormDetailMode.ForApprover &&
      (this.formData.ownerId === this.currentUser.extId || this.currentUser.hasAdministratorRoles()) &&
      (this.formData.status === FormStatus.Draft ||
        this.formData.status === FormStatus.Rejected ||
        this.formData.status === FormStatus.Unpublished ||
        this.formData.status === FormStatus.Approved)
    );
  }

  public get canShowDuplicateButton(): boolean {
    const threeYearsBefore = DateUtils.addYear(new Date(), -3);
    return (
      this.formData.archiveDate &&
      (this.formData.status === FormStatus.Archived && DateUtils.compareDate(new Date(this.formData.archiveDate), threeYearsBefore) >= 0)
    );
  }

  public get isSystemAdmin(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.SystemAdministrator);
  }

  public get isContentCreator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.ContentCreator);
  }

  public get isCourseFacilitator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.CourseFacilitator);
  }

  public get isCourseContentCreator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.CourseContentCreator);
  }

  public get isOwner(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.extId === this.formData.ownerId;
  }

  public get isFormOwner(): boolean {
    return this.currentUser.extId === this.formData.ownerId;
  }

  public get isApprovingOfficer(): boolean {
    return (
      this.currentUser.hasAdministratorRoles() ||
      this.currentUser.hasRole(SystemRoleEnum.MOEHQContentApprovingOfficer, SystemRoleEnum.SchoolContentApprovingOfficer)
    );
  }

  public get canShowEditButton(): boolean {
    return (
      this.mode === FormDetailMode.View &&
      (this.formData.status === FormStatus.Draft ||
        this.formData.status === FormStatus.Rejected ||
        this.formData.status === FormStatus.Unpublished ||
        this.formData.status === FormStatus.Approved)
    );
  }

  public get isAllowAddCollaborator(): boolean {
    return (
      this.isOwner &&
      this.formData.status !== this.FormStatus.Archived &&
      (!this.formData.isSurveyTemplate || (this.formData.isSurveyTemplate && this.isSystemAdmin))
    );
  }

  private get isFormDataChanged(): boolean {
    const isFormSectionsQuestionsDifferent = Utils.isDifferent(this.savedFormSectionsQuestions, this.formSectionsQuestions);
    const isFormDataDifferent = Utils.isDifferent(this.savedFormData, this.formData);
    const isResourceDataDifferent = Utils.isDifferent(this.savedResource, this.resource);

    return isFormDataDifferent || isFormSectionsQuestionsDifferent || isResourceDataDifferent;
  }

  public onTitleChange(title: string): void {
    this.formData.title = title;
  }

  public onSubmitForApprovalClicked(): void {
    this.changeFormStatus(FormStatus.PendingApproval);
  }

  public onSaveBtnClicked(status: FormStatus, isSave?: boolean): void {
    this.changeFormStatus(status, isSave);
  }

  public onMarkAsReadyClicked(): void {
    this.changeFormStatus(FormStatus.ReadyToUse);
  }

  public onArchiveBtnClicked(): void {
    this.formRepositoryPageService.archiveForm(this.formData.id, this.currentUser.extId).subscribe(_ => {
      this.showNotification(`${this.formData.title} is successfully archived`, NotificationType.Success);
      this.onBackAction();
    });
  }

  public onMarkAsDraftClicked(): void {
    this.learningContentApiService.hasReferenceToResource(this.formData.id, true).then(isCourseReferenced => {
      this.changeFormStatus(FormStatus.Draft, false, isCourseReferenced);
    });
  }

  public async onApproveClicked(): Promise<void> {
    const isFormValid = await this.validateForm();
    if (isFormValid) {
      this.dialogApprovalRef = this.moduleFacadeService.dialogService.open({
        content: this.dialogApprovalCommentTemplate,
        width: 700
      });
    } else {
      await this.focusInvalidField();
    }
  }

  public onRejectClicked(): void {
    this.dialogRejectRef = this.moduleFacadeService.dialogService.open({
      content: this.dialogRejectCommentTemplate,
      width: 700
    });
  }

  public onPublishClicked(): void {
    if (this.formData.isStandalone) {
      this.modalService.showConfirmMessage(
        new TranslationMessage(
          this.moduleFacadeService.globalTranslator,
          'You can not edit once this form is published. Please check your content before publishing.'
        ),
        () => {
          this.changeFormStatus(FormStatus.Published);
        }
      );
    } else {
      this.changeFormStatus(FormStatus.Published);
    }
  }

  public onUnpublishClicked(): void {
    this.changeFormStatus(FormStatus.Unpublished);
  }

  public onDuplicateBtnClicked(): void {
    this.formRepositoryPageService
      .cloneForm(this.formData.id, `${this.translate('Copy of')} ${this.formData.title}`)
      .subscribe(formWithQuestions => {
        this.taggingApiService.cloneResource(formWithQuestions.form.id, this.formData.id).toPromise();
        this.showNotification(`${this.formData.title} is successfully duplicated`, NotificationType.Success);
        this.onBackAction();
      });
  }

  public onTransferOwnershipBtnClicked(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: TransferOwnershipDialogComponent });

    dialogRef.result.toPromise().then((newOwnerId: string) => {
      if (typeof newOwnerId === 'string') {
        this.formRepositoryPageService.transferOwnership(this.formData.id, newOwnerId).subscribe(() => {
          this.showNotification('Ownership transferred successfully', NotificationType.Success);
          this.onBackAction();
        });
      }
    });
  }

  public onBackButtonClick(): void {
    this.performAutoSaveFormAndQuestionForm();
    this.navigateTo(CCPMRoutePaths.FormRepository, <IFormEditorPageNavigationData>{});
  }

  public showWebPreviewer(mode: PreviewMode): void {
    this.previewMode = mode;
    this.isWebPreview = true;
    this.appToolbarService.hide();
    this.headerService.hide();
    this.navigationMenuService.hide();
    this.opalFooterService.hide();

    AppGlobal.quizPlayerIntegrations.setFormId(this.formData.id);
  }

  public hideWebPreviewer(): void {
    this.headerService.show();
    this.appToolbarService.show();
    this.navigationMenuService.show();
    this.opalFooterService.show();
    this.isWebPreview = false;
    this.previewMode = PreviewMode.None;
  }

  public onRevertData(revertChangeResult: IRevertVersionTrackingResult): void {
    if (revertChangeResult && revertChangeResult.isSuccess) {
      this.subcribeFormData();
      this.subcribeFormQuestionsData();
      this.formEditorPageService.loadFormAndFormQuestionsData(revertChangeResult.newActiveId).subscribe();
    }
  }

  public reloadVersionTracking(): void {
    this.auditLogTab.loadData(false);
    this.auditLogTab.loadActiveVersion(false);
  }

  public async saveCommentAndRejectClick(comment: string): Promise<void> {
    this.markFormAsSubmitting();
    if (comment) {
      await this.commentApiService.saveComment(
        {
          content: comment,
          objectId: this.formData.originalObjectId
        },
        true
      );
    }
    this.dialogRejectRef.close();
    this.formData.status = FormStatus.Rejected;
    this.encodeFormQuestionsTitle(this.formSectionsQuestions.formQuestions);
    const request: ISaveResourceMetadataRequest = {
      tagIds: this.resource.tags,
      mainSubjectAreaTagId: this.resource.mainSubjectAreaTagId,
      objectivesOutCome: this.resource.objectivesOutCome,
      preRequisties: this.resource.preRequisties,
      searchTags: this.resource.searchTags
    };
    await forkJoin(
      this.formEditorPageService.changeStatusAndUpdateData(
        this.formData,
        FormQuestionModel.removeUnsupportedQuestionTypes(this.formSectionsQuestions.formQuestions),
        this.formSectionsQuestions.formSections,
        false,
        true,
        null,
        comment
      ),
      this.taggingApiService.saveFormMetadata(this.formData.id, request)
    ).subscribe(() => {
      this.showNotification(`${this.formData.title} is successfully rejected`, NotificationType.Success);
    });
    this.isSubmitting = false;
    this.onBackAction();
  }

  public async saveCommentAndApproveClick(comment: string): Promise<void> {
    this.markFormAsSubmitting();

    if (comment) {
      await this.commentApiService.saveComment(
        {
          content: comment,
          objectId: this.formData.originalObjectId
        },
        false
      );
    }

    this.encodeFormQuestionsTitle(this.formSectionsQuestions.formQuestions);

    this.formData.status = FormStatus.Approved;

    this.dialogApprovalRef.close();
    const request: ISaveResourceMetadataRequest = {
      tagIds: this.resource.tags,
      mainSubjectAreaTagId: this.resource.mainSubjectAreaTagId,
      objectivesOutCome: this.resource.objectivesOutCome,
      preRequisties: this.resource.preRequisties,
      searchTags: this.resource.searchTags
    };
    await forkJoin(
      this.formEditorPageService.changeStatusAndUpdateData(
        this.formData,
        FormQuestionModel.removeUnsupportedQuestionTypes(this.formSectionsQuestions.formQuestions),
        this.formSectionsQuestions.formSections,
        false,
        true,
        null,
        comment
      ),
      this.taggingApiService.saveFormMetadata(this.formData.id, request)
    ).subscribe(() => {
      this.showNotification(`${this.formData.title} is successfully approved`, NotificationType.Success);
    });
    this.isSubmitting = false;
    this.onBackAction();
  }

  /**
   * TODO: find a best solution to remove error tool tip.
   */
  public onTabSelect(event?: SelectEvent): void {
    const popups = document.querySelectorAll('kendo-popup.error-tooltip');
    popups.forEach(popup => popup.parentNode.removeChild(popup));

    // Lazy load unnecessary sub component data
    if (event) {
      switch (event.index) {
        case 2:
          this.auditLogTab.loadRevertableVersions(false);
          break;
        case 3:
          this.accessRightTab.getAllCollaboratorsId().then(_ => {
            this.accessRightTab.createFetchCollaboratorsFn();
          });
          break;
        case 5:
          this.brokenLinkReportTab.loadData(true);
          break;
      }
    }
  }

  public onEditButtonClick(): void {
    this.changeModeStatus(FormStatus.Draft);
  }

  public onQuestionAreaClick(): void {
    if (
      this.formData.status === FormStatus.PendingApproval ||
      this.formData.status === FormStatus.Approved ||
      this.formData.status === FormStatus.Published ||
      this.formData.status === FormStatus.Archived ||
      (this.formData.status === FormStatus.Rejected &&
        !this.currentUser.hasAdministratorRoles() &&
        (this.formData.alternativeApprovingOfficerId === this.currentUser.extId ||
          this.formData.primaryApprovingOfficerId === this.currentUser.extId))
    ) {
      return;
    }
    this.changeModeStatus(FormStatus.Draft);
  }

  public getDetailsTitleSubIcon(): string {
    switch (this.formData.type) {
      case FormType.Quiz:
        return this.quizIcon;
      case FormType.Poll:
        return this.pollIcon;
      default:
        return this.surveyIcon;
    }
  }

  public onCloseDialogReject(): void {
    this.dialogRejectRef.close();
  }

  public onCloseDialogApprove(): void {
    this.dialogApprovalRef.close();
  }

  public get canShowQuestionEditor(): boolean {
    return (
      this.formData &&
      (this.formData.type === FormType.Poll || this.formData.type === FormType.Survey || this.formData.type === FormType.Quiz) &&
      this.isFormDataLoaded &&
      this.isFormQuestionsDataLoaded
    );
  }

  public get canShowAssessmentEditor(): boolean {
    return (
      this.formData &&
      (this.formData.type === FormType.Analytic || this.formData.type === FormType.Holistic) &&
      this.isFormDataLoaded &&
      this.isFormQuestionsDataLoaded
    );
  }

  protected onInit(): void {
    this.currentUser = UserInfoModel.getMyUserInfo();
    this.navigationData = this.getNavigateData<IFormEditorPageNavigationData>();
    this.updateDeeplink(`ccpm/form/${this.navigationData.formId}`);
    this.mode = this.navigationData.mode ? this.navigationData.mode : FormDetailMode.View;
    this.formEditModeService.initMode = this.mode;
    this.initDetailTitleSettings();
    this.initPreviewButtonOptions();
    this.initDraftStatusItems();
    this.initStatusItems();
    this.subcribeFormData();
    this.subcribeFormQuestionsData();
    this.formEditorPageService
      .loadFormAndFormQuestionsData(this.navigationData.formId)
      .pipe(
        catchError(err => {
          this.formAccessDenied(false);
          return of(err);
        })
      )
      .subscribe();

    this.taggingApiService
      .getResource(this.navigationData.formId)
      .pipe(this.untilDestroy())
      .subscribe(resource => {
        if (resource) {
          this.resource = resource;
          this.savedResource = Utils.cloneDeep(resource);
        }
      });
  }

  protected onDestroy(): void {
    this.unregisterFormAutoSaveScheduledTask(this.autoSaveFormScheduleTaskId);
    this.formEditorPageService.resetFormAutoSaveInformerSubjet();
  }

  private formAccessDenied(showError: boolean = true): void {
    this.onBackAction();
    if (showError) {
      this.moduleFacadeService.modalService.showErrorMessage('You do not have the permission to access this form.');
    }
  }
  private markFormAsSubmitting(): void {
    this.isSubmitting = true;
  }

  private onLoadFormData(): void {
    this.isSubmitting = false;
    this.reloadVersionTracking();
  }

  private registerFormAutoSaveScheduledTask(taskId: string): void {
    const autoSaveFormTask = new ScheduledTask(
      taskId,
      Constants.AUTOSAVE_INTERVALTIME,
      () => this.performAutoSaveFormAndQuestionForm(),
      false
    );
    this.moduleFacadeService.localScheduleService.register(autoSaveFormTask);
  }

  private unregisterFormAutoSaveScheduledTask(taskId: string): void {
    this.moduleFacadeService.localScheduleService.unregister(taskId);
  }

  private performAutoSaveFormAndQuestionForm(): void {
    if (
      !this.isSubmitting &&
      this.previewMode !== 'Web' &&
      this.previewMode !== 'Mobile' &&
      this.isFormDataChanged &&
      this.formData.status !== FormStatus.Published
    ) {
      this.encodeFormQuestionsTitle(this.formSectionsQuestions.formQuestions);
      const request: ISaveResourceMetadataRequest = {
        tagIds: this.resource.tags,
        mainSubjectAreaTagId: this.resource.mainSubjectAreaTagId,
        objectivesOutCome: this.resource.objectivesOutCome,
        preRequisties: this.resource.preRequisties,
        searchTags: this.resource.searchTags
      };
      forkJoin(
        this.formEditorPageService.updateFormData(
          this.formData,
          FormQuestionModel.removeUnsupportedQuestionTypes(this.formSectionsQuestions.formQuestions),
          this.formSectionsQuestions.formSections,
          true,
          false
        ),
        this.taggingApiService.saveFormMetadata(this.formData.id, request)
      ).subscribe(() => {
        this.savedResource = Utils.cloneDeep(this.resource);
        this.showNotification(this.translate(`${this.formData.type} saved successfully.`), NotificationType.Success);
      });
    }
  }

  private initStatusItems(): void {
    this.statusItems = [
      {
        text: this.translateCommon('Publish'),
        value: FormStatus.Published
      },
      {
        text: this.translateCommon('Unpublish'),
        value: FormStatus.Unpublished
      }
    ];
  }

  private initDraftStatusItems(): void {
    this.draftStatusItems = [
      {
        text: this.translateCommon('Draft'),
        value: FormStatus.Draft
      },
      {
        text: this.translateCommon('Publish'),
        value: FormStatus.Published
      }
    ];
  }

  private initPreviewButtonOptions(): void {
    this.previewOptions = [
      {
        text: this.translate('Mobile'),
        click: () => {
          this.previewForm(PreviewMode.Mobile);
        }
      },
      {
        text: this.translate('Web'),
        click: () => {
          this.previewForm(PreviewMode.Web);
        }
      }
    ];
  }

  private previewForm(mode: PreviewMode): void {
    if (this.formData.isSurveyTemplate) {
      this.showWebPreviewer(mode);
    } else {
      this.encodeFormQuestionsTitle(this.formSectionsQuestions.formQuestions);

      const request: ISaveResourceMetadataRequest = {
        tagIds: this.resource.tags,
        mainSubjectAreaTagId: this.resource.mainSubjectAreaTagId,
        objectivesOutCome: this.resource.objectivesOutCome,
        preRequisties: this.resource.preRequisties,
        searchTags: this.resource.searchTags
      };
      forkJoin(
        this.formEditorPageService.changeStatusAndUpdateData(
          this.formData,
          FormQuestionModel.removeUnsupportedQuestionTypes(this.formSectionsQuestions.formQuestions),
          this.formSectionsQuestions.formSections,
          true,
          true
        ),
        this.taggingApiService.saveFormMetadata(this.formData.id, request)
      ).subscribe(() => {
        this.savedResource = Utils.cloneDeep(this.resource);
        this.showWebPreviewer(mode);
      });
    }
  }

  private initDetailTitleSettings(): void {
    this.detailTitleSettings = new DetailTitleSettings();
    this.detailTitleSettings.titleValidators = FormModel.titleValidators;
    if (this.navigationData.formId) {
      return;
    }
    this.detailTitleSettings.editModeEnabled = true;
  }

  private subcribeFormData(): void {
    this.formEditorPageService.formData$.pipe(this.untilDestroy()).subscribe(data => {
      if (data.isArchived || !data) {
        this.formAccessDenied();
        return;
      }

      this.formData = data;
      this.savedFormData = Utils.cloneDeep(data);
      this.formStatus = {
        text: undefined,
        value: this.formData.status
      };
      this.formConfig = new FormConfiguration({
        inSecondTimeLimit: this.formData.inSecondTimeLimit,
        randomizedQuestions: this.formData.randomizedQuestions,
        maxAttempt: this.formData.maxAttempt,
        passingMarkPercentage: this.formData.passingMarkPercentage,
        passingMarkScore: this.formData.passingMarkScore,
        attemptToShowFeedback: this.formData.attemptToShowFeedback,
        answerFeedbackDisplayOption: this.formData.answerFeedbackDisplayOption
      });
      this.initApprovalRejectDialogTitle();
      this.isFormDataLoaded = true;
    });
  }

  private initApprovalRejectDialogTitle(): void {
    switch (this.formData.type) {
      case FormType.Quiz:
        this.titleApprovalDialog = this.translate('Approval Quiz');
        this.titleRejectDialog = this.translate('Reject Quiz');
        break;
      case FormType.Poll:
        this.titleApprovalDialog = this.translate('Approval Poll');
        this.titleRejectDialog = this.translate('Reject Poll');
        break;
      case FormType.Survey:
        this.titleApprovalDialog = this.translate('Approval Survey');
        this.titleRejectDialog = this.translate('Reject Survey');
        break;
      default:
        this.titleApprovalDialog = this.translate('Approval');
        this.titleRejectDialog = this.translate('Reject reason');
        break;
    }
  }

  private subcribeFormQuestionsData(): void {
    this.formEditorPageService.formQuestionsData$.pipe(this.untilDestroy()).subscribe(formSectionsQuestions => {
      this.savedFormSectionsQuestions.formQuestions = this.markQuestionsNoRequireAnswer(
        Utils.cloneDeep(formSectionsQuestions.formQuestions)
      );
      this.savedFormSectionsQuestions.formSections = Utils.cloneDeep(formSectionsQuestions.formSections);

      this.isFormQuestionsDataLoaded = true;

      const newFormQuestions = this.markQuestionsNoRequireAnswer(formSectionsQuestions.formQuestions);
      const newSectionsQuestionsViewModel = <FormSectionsQuestions>{
        formQuestions: this.decodeFormQuestionsTitle(newFormQuestions),
        formSections: formSectionsQuestions.formSections
      };

      this.formSectionsQuestions = newSectionsQuestionsViewModel;
    });
  }

  private markQuestionsNoRequireAnswer(formQuestions: FormQuestionModel[]): FormQuestionModel[] {
    switch (this.formData.type) {
      case FormType.Survey:
      case FormType.Poll:
        formQuestions = formQuestions.map(fq => fq.markQuestionAsNoRequireAnswer());
        break;
    }

    return formQuestions;
  }

  private async changeFormStatus(status: FormStatus, isSave?: boolean, isUpdateToNewVersion: boolean = false): Promise<void> {
    this.buildAdditionalInfoToFormModel();
    if (status === FormStatus.Draft || status === FormStatus.Archived || ((await this.validateForm()) && this.validateQuestionList())) {
      const formRequest = Utils.cloneDeep(this.formData);
      formRequest.status = status ? status : this.formData.status;
      this.changeModeStatus(formRequest.status);
      this.markFormAsSubmitting();
      this.encodeFormQuestionsTitle(this.formSectionsQuestions.formQuestions);

      const request: ISaveResourceMetadataRequest = {
        tagIds: this.resource.tags,
        mainSubjectAreaTagId: this.resource.mainSubjectAreaTagId,
        objectivesOutCome: this.resource.objectivesOutCome,
        preRequisties: this.resource.preRequisties,
        searchTags: this.resource.searchTags
      };
      forkJoin(
        this.formEditorPageService.changeStatusAndUpdateData(
          formRequest,
          FormQuestionModel.removeUnsupportedQuestionTypes(this.formSectionsQuestions.formQuestions),
          this.formSectionsQuestions.formSections,
          status === FormStatus.Draft,
          true,
          isUpdateToNewVersion
        ),
        this.taggingApiService.saveFormMetadata(this.formData.id, request)
      ).subscribe(() => {
        this.savedResource = Utils.cloneDeep(this.resource);
        this.onLoadFormData();
        if (status === FormStatus.Draft) {
          if (!isSave) {
            this.showNotification(`${this.formData.title} is successfully marked as draft`, NotificationType.Success);
            this.onBackAction();
          } else {
            this.showNotification(`${this.formData.title} is successfully saved`, NotificationType.Success);
          }
        } else {
          this.onBackAction();
          if (status === FormStatus.PendingApproval) {
            this.showNotification(`${this.formData.title} is successfully submitted for approval`, NotificationType.Success);
          } else if (status === FormStatus.Published) {
            this.showNotification(`${this.formData.title} is successfully published`, NotificationType.Success);
          } else if (status === FormStatus.Unpublished) {
            this.showNotification(`${this.formData.title} is successfully unpublished`, NotificationType.Success);
          } else if (status === FormStatus.ReadyToUse) {
            this.showNotification(`${this.formData.title} is successfully marked as ready`, NotificationType.Success);
          }
        }
      });
      return;
    } else {
      await this.focusInvalidField();
    }
  }

  private buildAdditionalInfoToFormModel(): void {
    this.formData.primaryApprovingOfficerId = this.additionalInfoTab.form.value.primaryApprovingOfficerId;
    this.formData.alternativeApprovingOfficerId = this.additionalInfoTab.form.value.alternativeApprovingOfficerId;
    this.formData.surveyType = this.additionalInfoTab.form.value.surveyType || null;
    this.formData.isSurveyTemplate = this.additionalInfoTab.form.value.isSurveyTemplate || null;
    this.formData.sqRatingType = this.additionalInfoTab.form.value.sqRatingType || null;
    this.formData.startDate = this.additionalInfoTab.form.value.startDate || null;

    this.formData.endDate = this.additionalInfoTab.form.value.endDate
      ? DateUtils.setTimeToEndInDay(this.additionalInfoTab.form.value.endDate)
      : null;

    this.formData.archiveDate = this.additionalInfoTab.form.value.archiveDate
      ? DateUtils.setTimeToEndInDay(this.additionalInfoTab.form.value.archiveDate)
      : null;

    this.formData.formRemindDueDate = this.additionalInfoTab.form.value.formRemindDueDate
      ? DateUtils.setTimeToEndInDay(this.additionalInfoTab.form.value.formRemindDueDate)
      : null;
  }

  private async validateForm(): Promise<boolean> {
    const isFormTitleValid = await this.detailTitleForm.validate();
    const isAdditionalInfoTabValid = await this.additionalInfoTab.validate();
    const isformQuestionConfigValid = await this.validateQuestionConfig();
    const isFormConfigValid = await this.validateFormConfig();

    let isFormManagementValid: boolean;
    if (this.formData.type === FormType.Holistic || this.formData.type === FormType.Analytic) {
      isFormManagementValid = await this.assessmentRubricManagmentPage.validate();
    } else {
      isFormManagementValid = await this.formEditorPage.formEditor.validate();
    }

    return isFormTitleValid && isFormManagementValid && isAdditionalInfoTabValid && isFormConfigValid && isformQuestionConfigValid;
  }

  private validateQuestionList(): boolean {
    return Array.isArray(this.formSectionsQuestions.formQuestions) && this.formSectionsQuestions.formQuestions.length > 0;
  }

  private validateQuestionConfig(): boolean {
    if (!this.formData.passingMarkPercentage && !this.formData.passingMarkScore) {
      return true;
    }

    const currentQuestionList = this.formSectionsQuestions.formQuestions;
    return currentQuestionList.filter(q => q.isScoreEnabled).length !== 0;
  }

  private validateFormConfig(): boolean {
    if (this.formData.answerFeedbackDisplayOption === AnswerFeedbackDisplayOption.AfterXAtemps) {
      return !this.formData.attemptToShowFeedback ? false : true;
    }
    if (this.formData.passingMarkScore) {
      return this.formData.passingMarkScore <= FormQuestionModel.calcMaxScore(this._formSectionsQuestions.formQuestions);
    }
    return true;
  }

  private async openFormConfigInvalidField(): Promise<void> {
    if (this.formEditorPage.isExpandedOption === true) {
      this.formEditorPage.isExpandedOption = false;
    }
  }

  private async focusInvalidField(): Promise<void> {
    if (this.mode === FormDetailMode.View) {
      this.mode = FormDetailMode.Edit;
    }
    const isQuestionListNotNull = await this.validateQuestionList();
    if (isQuestionListNotNull === false) {
      const tabIndex = 0;
      this.formTabs.selectTab(tabIndex);
      this.showNotification(this.translate('Question list must not be empty'), NotificationType.Error);
      return;
    }
    const isFormTitleValid = await this.detailTitleForm.validate();
    if (!isFormTitleValid) {
      this.detailTitleForm.enableEditTitle();
      setTimeout(() => this.detailTitleForm.validate(), 100);
      this.detailTitleForm.focusInput();
      return;
    }

    if (this.formData.type === FormType.Analytic || this.formData.type === FormType.Holistic) {
      const isAssessmentManagementValid = await this.assessmentRubricManagmentPage.validate();
      if (!isAssessmentManagementValid) {
        const tabIndex = 0;
        this.formTabs.selectTab(tabIndex);
        setTimeout(() => this.assessmentRubricManagmentPage.validate());
        return;
      }
    } else {
      const isFormDetailValid = await this.formEditorPage.formEditor.validate();
      if (!isFormDetailValid) {
        const tabIndex = 0;
        this.formTabs.selectTab(tabIndex);
        setTimeout(() => this.formEditorPage.formEditor.validate());
        return;
      }
    }

    const isAdditionalInfoTabValid = await this.additionalInfoTab.validate();
    if (!isAdditionalInfoTabValid) {
      const tabIndex = 1;
      this.formTabs.selectTab(tabIndex);
      setTimeout(() => this.additionalInfoTab.validate());
      return;
    }
    const isformQuestionValid = await this.validateQuestionConfig();
    if (!isformQuestionValid) {
      this.formTabs.selectTab(0);
      this.showNotification('Please set at least 1 question with a mark.', NotificationType.Error);
      return;
    }
    const isFormConfigValid = await this.validateFormConfig();
    if (!isFormConfigValid) {
      const tabIndex = 0;
      this.formTabs.selectTab(tabIndex);
      this.openFormConfigInvalidField();

      if (this.formData.passingMarkScore > FormQuestionModel.calcMaxScore(this._formSectionsQuestions.formQuestions)) {
        this.showNotification(this.translate('Passing mark score must not be higher than total score'), NotificationType.Error);
        return;
      }

      this.showNotification(this.translate('Attempt must not be empty when choose After X Attempt'), NotificationType.Error);
      return;
    }
  }

  private onBackAction(): void {
    this.navigateTo(CCPMRoutePaths.FormRepository);
  }

  private changeModeStatus(formStatus: FormStatus): void {
    switch (formStatus) {
      case FormStatus.Draft:
        this.mode = FormDetailMode.Edit;
        break;
      case FormStatus.ReadyToUse:
      case FormStatus.PendingApproval:
        this.mode = FormDetailMode.View;
        break;
      case FormStatus.Archived:
        this.mode = FormDetailMode.View;
        break;
      default:
        break;
    }
  }

  private encodeFormQuestionsTitle(formQuestions: FormQuestionModel[]): FormQuestionModel[] {
    formQuestions = Utils.clone(formQuestions, newFormQuestions => {
      newFormQuestions.forEach(question => {
        question.questionTitle = new XmlEntities().encode(question.questionTitle);
      });
    });
    return formQuestions;
  }

  private decodeFormQuestionsTitle(formQuestions: FormQuestionModel[]): FormQuestionModel[] {
    formQuestions.forEach(question => {
      question.questionTitle = new XmlEntities().decode(question.questionTitle);
    });
    return formQuestions;
  }
}
