import {
  AccessRightType,
  BrokenLinkModuleIdentifier,
  CommentServiceType,
  FormParticipantType,
  IRevertVersionTrackingResult,
  LearningContentApiService,
  StandaloneSurveyModel,
  StandaloneSurveySectionsQuestions,
  SurveyConfiguration,
  SurveyQuestionModel,
  SurveyStatus,
  SystemRoleEnum,
  UserInfoModel,
  VersionTrackingType
} from '@opal20/domain-api';
import {
  AppToolbarService,
  BrokenLinkReportTabComponent,
  CommentTabInput,
  HeaderService,
  NavigationMenuService,
  OpalFooterService,
  PreviewMode,
  STANDALONE_SURVEY_STATUS_COLOR_MAP,
  StandaloneSurveyAdditionalInformationTabComponent,
  StandaloneSurveyDetailMode,
  StandaloneSurveyEditModeService,
  StandaloneSurveyEditorPageComponent,
  StandaloneSurveyEditorPageService,
  StandaloneSurveyRepositoryPageService
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
import { Component, TemplateRef, ViewChild } from '@angular/core';
import { DetailTitleComponent, DetailTitleSettings } from '@opal20/common-components';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';

import { AccessRightTabComponent } from './tabs/access-right-tab/access-right-tab.component';
import { AuditLogTabComponent } from './tabs/audit-log-tab/audit-log-tab.component';
import { CCPMRoutePaths } from '../ccpm.config';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { IStandaloneSurveyEditorPageNavigationData } from '../ccpm-navigation-data';
import { TransferOwnershipDialogComponent } from './dialogs/transfer-ownership-dialog.component';
import { XmlEntities } from 'html-entities';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
@Component({
  selector: 'standalone-survey-detail-page',
  templateUrl: './standalone-survey-detail-page.component.html'
})
export class StandaloneSurveyDetailPageComponent extends BasePageComponent {
  public get commentTabInput(): CommentTabInput {
    return {
      originalObjectId: this.formData.originalObjectId,
      commentServiceType: CommentServiceType.LnaForm,
      hasReply: true
    };
  }

  public removeUnsupportedQuestionTypes = SurveyQuestionModel.removeUnsupportedQuestionTypes;
  public statusColorMap = STANDALONE_SURVEY_STATUS_COLOR_MAP;
  public navigationData: IStandaloneSurveyEditorPageNavigationData | undefined;
  public previewOptions: unknown[];
  public detailTitleSettings: DetailTitleSettings;
  public statusItems: IDataItem[];
  public draftStatusItems: IDataItem[];
  public urlSubIcon: string;
  public get formStatusList(): IDataItem[] {
    return !this.formStatus || this.formStatus.value === SurveyStatus.Draft ? this.draftStatusItems : this.statusItems;
  }

  @ViewChild(StandaloneSurveyEditorPageComponent, { static: false }) public formEditorPage: StandaloneSurveyEditorPageComponent;
  @ViewChild(DetailTitleComponent, { static: false }) public detailTitleForm: DetailTitleComponent;
  @ViewChild(StandaloneSurveyAdditionalInformationTabComponent, { static: false })
  public additionalInfoTab: StandaloneSurveyAdditionalInformationTabComponent;
  @ViewChild(BrokenLinkReportTabComponent, { static: false }) public brokenLinkReportTab: BrokenLinkReportTabComponent;

  @ViewChild(TabStripComponent, { static: false })
  public formTabs: TabStripComponent;

  public currentUser: UserInfoModel;
  public previewMode: PreviewMode = PreviewMode.None;
  public formStatus: IDataItem;
  public formConfig: SurveyConfiguration = new SurveyConfiguration();
  public savedFormData: StandaloneSurveyModel = new StandaloneSurveyModel();
  public savedFormSectionsQuestions: StandaloneSurveySectionsQuestions = new StandaloneSurveySectionsQuestions({
    formQuestions: [],
    formSections: []
  });
  public formData: StandaloneSurveyModel = new StandaloneSurveyModel();
  public selectedQuestionid: string | undefined;
  public selectedFormQuestion: SurveyQuestionModel | undefined;
  public isWebPreview: boolean = false;
  public SurveyStatus: typeof SurveyStatus = SurveyStatus;
  public isFormQuestionsDataLoaded: boolean = false;
  public isFormDataLoaded: boolean = false;

  // Broken link properties
  public brokenLinkModule: BrokenLinkModuleIdentifier = BrokenLinkModuleIdentifier.LnaForm;

  // Versioning properties
  @ViewChild(AuditLogTabComponent, { static: false })
  public auditLogTab: AuditLogTabComponent;
  public versionTrackingType: VersionTrackingType = VersionTrackingType.LnaForm;
  public PreviewMode: typeof PreviewMode = PreviewMode;
  // Comments properties
  public commentFormType: CommentServiceType = CommentServiceType.LnaForm;
  public titleApprovalDialog: string;
  public titleRejectDialog: string;

  public dialogRejectRef: DialogRef;
  public dialogApprovalRef: DialogRef;

  // Access rights properties
  @ViewChild(AccessRightTabComponent, { static: false })
  public accessRightTab: AccessRightTabComponent;
  public accessRightType: AccessRightType = AccessRightType.LnaForm;

  public readonly MAX_LENGTH_TITLE: number = 1000;
  // stand alone properties
  public formParticipantType: FormParticipantType = FormParticipantType.LnaForm;

  private _mode: StandaloneSurveyDetailMode = StandaloneSurveyDetailMode.View;
  private _formSectionsQuestions: StandaloneSurveySectionsQuestions = new StandaloneSurveySectionsQuestions({
    formQuestions: [],
    formSections: []
  });
  private autoSaveFormScheduleTaskId = 'auto-save-form';
  private isSubmitting: boolean = false;

  @ViewChild('dialogRejectCommentTemplate', { static: false })
  private dialogRejectCommentTemplate: TemplateRef<unknown>;

  @ViewChild('dialogApprovalCommentTemplate', { static: false })
  private dialogApprovalCommentTemplate: TemplateRef<unknown>;

  public get formSectionsQuestions(): StandaloneSurveySectionsQuestions {
    return this._formSectionsQuestions;
  }

  public set formSectionsQuestions(v: StandaloneSurveySectionsQuestions) {
    this._formSectionsQuestions = v;
    const selectedQuestionIndex = this.formSectionsQuestions.formQuestions.findIndex(question => question.id === this.selectedQuestionid);
    if (selectedQuestionIndex > -1) {
      this.selectedFormQuestion = this.formSectionsQuestions.formQuestions[selectedQuestionIndex];
    }
  }

  public get mode(): StandaloneSurveyDetailMode {
    return this._mode;
  }

  public set mode(v: StandaloneSurveyDetailMode) {
    this._mode = v;
    this.editModeService.modeChanged.next(this._mode);
    this.editModeService.initMode = this._mode;
  }

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected headerService: HeaderService,
    protected editorPageService: StandaloneSurveyEditorPageService,
    private navigationMenuService: NavigationMenuService,
    private opalFooterService: OpalFooterService,
    private appToolbarService: AppToolbarService,
    private editModeService: StandaloneSurveyEditModeService,
    private formRepositoryPageService: StandaloneSurveyRepositoryPageService,
    private learningContentApiService: LearningContentApiService
  ) {
    super(moduleFacadeService);
    this.registerFormAutoSaveScheduledTask(this.autoSaveFormScheduleTaskId);
  }

  public get canSaveForm(): boolean {
    return (
      this.mode === StandaloneSurveyDetailMode.Edit &&
      (this.formData.status === SurveyStatus.Draft || this.formData.status === SurveyStatus.Unpublished)
    );
  }

  public get canPublish(): boolean {
    return this.formData.status === SurveyStatus.Unpublished || this.formData.status === SurveyStatus.Draft;
  }

  public get canUnpublish(): boolean {
    return this.formData.status === SurveyStatus.Published && this.formData.canUnpublishFormStandalone;
  }

  public get canShowArchiveButton(): boolean {
    return (
      this.isOwner &&
      (this.formData.ownerId === this.currentUser.extId || this.currentUser.hasAdministratorRoles()) &&
      (this.formData.status === SurveyStatus.Draft || this.formData.status === SurveyStatus.Unpublished)
    );
  }

  public get canShowDuplicateButton(): boolean {
    const threeYearsBefore = DateUtils.addYear(new Date(), -3);
    return (
      this.formData.archiveDate &&
      (this.formData.status === SurveyStatus.Archived && DateUtils.compareDate(new Date(this.formData.archiveDate), threeYearsBefore) >= 0)
    );
  }

  public get isSystemAdmin(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.SystemAdministrator);
  }

  public get isContentCreator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.ContentCreator);
  }

  public get isOwner(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.extId === this.formData.ownerId;
  }

  public get canShowEditButton(): boolean {
    return (
      this.mode === StandaloneSurveyDetailMode.View &&
      (this.formData.status === SurveyStatus.Draft || this.formData.status === SurveyStatus.Unpublished)
    );
  }

  public get isAllowAddCollaborator(): boolean {
    return this.isOwner && this.formData.status !== this.SurveyStatus.Archived;
  }

  private get isFormDataChanged(): boolean {
    const isFormSectionsQuestionsDifferent = Utils.isDifferent(this.savedFormSectionsQuestions, this.formSectionsQuestions);
    const isFormDataDifferent = Utils.isDifferent(this.savedFormData, this.formData);

    return isFormDataDifferent || isFormSectionsQuestionsDifferent;
  }

  public onTitleChange(title: string): void {
    this.formData.title = title;
  }

  public onSaveBtnClicked(status: SurveyStatus, isSave?: boolean): void {
    this.changeFormStatus(status, isSave);
  }

  public onArchiveBtnClicked(): void {
    this.formRepositoryPageService.archiveForm(this.formData.id, this.currentUser.extId).subscribe(_ => {
      this.showNotification(`${this.formData.title} is successfully archived`, NotificationType.Success);
      this.onBackAction();
    });
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
          this.changeFormStatus(SurveyStatus.Published);
        }
      );
    } else {
      this.changeFormStatus(SurveyStatus.Published);
    }
  }

  public onUnpublishClicked(): void {
    this.changeFormStatus(SurveyStatus.Unpublished);
  }

  public onDuplicateBtnClicked(): void {
    this.formRepositoryPageService.cloneForm(this.formData.id, `${this.translate('Copy of')} ${this.formData.title}`).subscribe(() => {
      this.showNotification(`${this.formData.title} is successfully duplicated`, NotificationType.Success);
      this.onBackAction();
    });
  }

  public onTransferOwnershipBtnClicked(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: TransferOwnershipDialogComponent });
    const component = dialogRef.content.instance as TransferOwnershipDialogComponent;
    component.userAcceptableRoles = [
      SystemRoleEnum.DivisionAdministrator,
      SystemRoleEnum.SchoolStaffDeveloper,
      SystemRoleEnum.SchoolAdministrator,
      SystemRoleEnum.BranchAdministrator,
      SystemRoleEnum.SystemAdministrator,
      SystemRoleEnum.DivisionTrainingCoordinator
    ];

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
    this.navigateTo(CCPMRoutePaths.StandaloneSurveyRepository, <IStandaloneSurveyEditorPageNavigationData>{});
  }

  public showWebPreviewer(mode: PreviewMode): void {
    this.previewMode = mode;
    this.isWebPreview = true;
    this.appToolbarService.hide();
    this.headerService.hide();
    this.navigationMenuService.hide();
    this.opalFooterService.hide();

    AppGlobal.standaloneSurveyIntegration.setFormId(this.formData.id);
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
      this.editorPageService.loadFormAndFormQuestionsData(revertChangeResult.newActiveId).subscribe();
    }
  }

  public reloadVersionTracking(): void {
    this.auditLogTab.loadData(false);
    this.auditLogTab.loadActiveVersion(false);
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
    this.changeModeStatus(SurveyStatus.Draft);
  }

  public onQuestionAreaClick(): void {
    if (
      this.formData.status === SurveyStatus.Published ||
      this.formData.status === SurveyStatus.Archived ||
      !this.currentUser.hasAdministratorRoles()
    ) {
      return;
    }
    this.changeModeStatus(SurveyStatus.Draft);
  }

  protected onInit(): void {
    this.currentUser = UserInfoModel.getMyUserInfo();
    this.navigationData = this.getNavigateData<IStandaloneSurveyEditorPageNavigationData>();
    this.updateDeeplink(`ccpm/lnaform/${this.navigationData.formId}`);
    this.mode = this.navigationData.mode ? this.navigationData.mode : StandaloneSurveyDetailMode.View;
    this.editModeService.initMode = this.mode;
    this.initDetailTitleSettings();
    this.initPreviewButtonOptions();
    this.initDraftStatusItems();
    this.initStatusItems();
    this.subcribeFormData();
    this.subcribeFormQuestionsData();
    this.editorPageService
      .loadFormAndFormQuestionsData(this.navigationData.formId)
      .pipe(
        catchError(err => {
          this.formAccessDenied(false);
          return of(err);
        })
      )
      .subscribe();
  }

  protected onDestroy(): void {
    this.unregisterFormAutoSaveScheduledTask(this.autoSaveFormScheduleTaskId);
    this.editorPageService.resetFormAutoSaveInformerSubjet();
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
    const autoSaveFormTask = new ScheduledTask(taskId, 10000, () => this.performAutoSaveFormAndQuestionForm(), false);
    this.moduleFacadeService.localScheduleService.register(autoSaveFormTask);
  }

  private unregisterFormAutoSaveScheduledTask(taskId: string): void {
    this.moduleFacadeService.localScheduleService.unregister(taskId);
  }

  private performAutoSaveFormAndQuestionForm(): void {
    if (!this.isSubmitting && this.previewMode !== 'Web' && this.previewMode !== 'Mobile' && this.isFormDataChanged) {
      this.encodeFormQuestionsTitle(this.formSectionsQuestions.formQuestions);
      this.editorPageService
        .updateFormData(
          this.formData,
          SurveyQuestionModel.removeUnsupportedQuestionTypes(this.formSectionsQuestions.formQuestions),
          this.formSectionsQuestions.formSections,
          true,
          false
        )
        .subscribe(() => {
          this.showNotification(this.translate('Survey saved successfully.'), NotificationType.Success);
        });
    }
  }

  private initStatusItems(): void {
    this.statusItems = [
      {
        text: this.translateCommon('Publish'),
        value: SurveyStatus.Published
      },
      {
        text: this.translateCommon('Unpublish'),
        value: SurveyStatus.Unpublished
      }
    ];
  }

  private initDraftStatusItems(): void {
    this.draftStatusItems = [
      {
        text: this.translateCommon('Draft'),
        value: SurveyStatus.Draft
      },
      {
        text: this.translateCommon('Publish'),
        value: SurveyStatus.Published
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
    this.encodeFormQuestionsTitle(this.formSectionsQuestions.formQuestions);
    this.editorPageService
      .changeStatusAndUpdateData(
        this.formData,
        SurveyQuestionModel.removeUnsupportedQuestionTypes(this.formSectionsQuestions.formQuestions),
        this.formSectionsQuestions.formSections,
        true,
        true
      )
      .subscribe(() => {
        this.showWebPreviewer(mode);
      });
  }

  private initDetailTitleSettings(): void {
    this.detailTitleSettings = new DetailTitleSettings();
    this.detailTitleSettings.titleValidators = StandaloneSurveyModel.titleValidators;
    if (this.navigationData.formId) {
      return;
    }
    this.detailTitleSettings.editModeEnabled = true;
  }

  private subcribeFormData(): void {
    this.editorPageService.formData$.pipe(this.untilDestroy()).subscribe(data => {
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
      this.isFormDataLoaded = true;
    });
  }

  private subcribeFormQuestionsData(): void {
    this.editorPageService.formQuestionsData$.pipe(this.untilDestroy()).subscribe(formSectionsQuestions => {
      this.savedFormSectionsQuestions.formQuestions = this.markQuestionsNoRequireAnswer(
        Utils.cloneDeep(formSectionsQuestions.formQuestions)
      );
      this.savedFormSectionsQuestions.formSections = Utils.cloneDeep(formSectionsQuestions.formSections);

      this.isFormQuestionsDataLoaded = true;

      const newFormQuestions = this.markQuestionsNoRequireAnswer(formSectionsQuestions.formQuestions);
      const newSectionsQuestionsViewModel = <StandaloneSurveySectionsQuestions>{
        formQuestions: this.decodeFormQuestionsTitle(newFormQuestions),
        formSections: formSectionsQuestions.formSections
      };

      this.formSectionsQuestions = newSectionsQuestionsViewModel;
    });
  }

  private markQuestionsNoRequireAnswer(formQuestions: SurveyQuestionModel[]): SurveyQuestionModel[] {
    formQuestions = formQuestions.map(fq => fq.markQuestionAsNoRequireAnswer());

    return formQuestions;
  }

  private async changeFormStatus(status: SurveyStatus, isSave?: boolean, isUpdateToNewVersion: boolean = false): Promise<void> {
    this.buildAdditionalInfoToFormModel();
    if (status === SurveyStatus.Draft || status === SurveyStatus.Archived || (await this.validateForm())) {
      const formRequest = Utils.cloneDeep(this.formData);
      formRequest.status = status ? status : this.formData.status;
      this.changeModeStatus(formRequest.status);
      this.markFormAsSubmitting();
      this.encodeFormQuestionsTitle(this.formSectionsQuestions.formQuestions);
      this.editorPageService
        .changeStatusAndUpdateData(
          formRequest,
          SurveyQuestionModel.removeUnsupportedQuestionTypes(this.formSectionsQuestions.formQuestions),
          this.formSectionsQuestions.formSections,
          status === SurveyStatus.Draft,
          true,
          isUpdateToNewVersion
        )
        .subscribe(() => {
          this.onLoadFormData();
          if (status === SurveyStatus.Draft) {
            if (!isSave) {
              this.showNotification('Survey is successfully marked as draft', NotificationType.Success);
              this.onBackAction();
            } else {
              this.showNotification('Survey is successfully saved', NotificationType.Success);
            }
          } else {
            this.onBackAction();
            if (status === SurveyStatus.Published) {
              this.showNotification('Survey is successfully published', NotificationType.Success);
            } else if (status === SurveyStatus.Unpublished) {
              this.showNotification('Survey is successfully unpublished', NotificationType.Success);
            }
          }
        });
      return;
    } else {
      await this.focusInvalidField();
    }
  }

  private buildAdditionalInfoToFormModel(): void {
    this.formData.isAllowedDisplayPollResult = this.additionalInfoTab.form.value.isAllowedDisplayPollResult || null;
    this.formData.sqRatingType = this.additionalInfoTab.form.value.sqRatingType || null;
    this.formData.startDate = this.additionalInfoTab.form.value.startDate || null;
    this.formData.endDate = this.additionalInfoTab.form.value.endDate || null;
    this.formData.archiveDate = this.additionalInfoTab.form.value.archiveDate
      ? DateUtils.setTimeToEndInDay(this.additionalInfoTab.form.value.archiveDate)
      : null;
  }

  private async validateForm(): Promise<boolean> {
    const isFormTitleValid = await this.detailTitleForm.validate();
    const isFormDetailValid = await this.formEditorPage.formEditor.validate();
    return isFormTitleValid && isFormDetailValid;
  }

  private async focusInvalidField(): Promise<void> {
    if (this.mode === StandaloneSurveyDetailMode.View) {
      this.mode = StandaloneSurveyDetailMode.Edit;
    }
    const isFormTitleValid = await this.detailTitleForm.validate();
    if (!isFormTitleValid) {
      this.detailTitleForm.enableEditTitle();
      setTimeout(() => this.detailTitleForm.validate(), 100);
      this.detailTitleForm.focusInput();
      return;
    }
    const isFormDetailValid = await this.formEditorPage.formEditor.validate();
    if (!isFormDetailValid) {
      const tabIndex = 0;
      this.formTabs.selectTab(tabIndex);
      setTimeout(() => this.formEditorPage.formEditor.validate());
      return;
    }
    const isAdditionalInfoTabValid = await this.additionalInfoTab.validate();
    if (!isAdditionalInfoTabValid) {
      const tabIndex = 1;
      this.formTabs.selectTab(tabIndex);
      setTimeout(() => this.additionalInfoTab.validate());
      return;
    }
  }

  private onBackAction(): void {
    this.navigateTo(CCPMRoutePaths.StandaloneSurveyRepository);
  }

  private changeModeStatus(formStatus: SurveyStatus): void {
    switch (formStatus) {
      case SurveyStatus.Draft:
        this.mode = StandaloneSurveyDetailMode.Edit;
        break;
      case SurveyStatus.Archived:
        this.mode = StandaloneSurveyDetailMode.View;
        break;
      default:
        break;
    }
  }

  private encodeFormQuestionsTitle(formQuestions: SurveyQuestionModel[]): SurveyQuestionModel[] {
    formQuestions = Utils.clone(formQuestions, newFormQuestions => {
      newFormQuestions.forEach(question => {
        question.questionTitle = new XmlEntities().encode(question.questionTitle);
      });
    });
    return formQuestions;
  }

  private decodeFormQuestionsTitle(formQuestions: SurveyQuestionModel[]): SurveyQuestionModel[] {
    formQuestions.forEach(question => {
      question.questionTitle = new XmlEntities().decode(question.questionTitle);
    });
    return formQuestions;
  }
}
