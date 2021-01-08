import {
  AccessRightType,
  BrokenLinkModuleIdentifier,
  CommentApiService,
  CommentServiceType,
  ContentApiService,
  DigitalContent,
  DigitalContentStatus,
  DigitalContentType,
  DigitalLearningContentRequest,
  DigitalUploadContentRequest,
  ICopyright,
  IDigitalContent,
  IDigitalContentRequest,
  IRevertVersionTrackingResult,
  MetadataId,
  ResourceModel,
  ResourceType,
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
  ContextMenuAction,
  CopyrightFormComponent,
  DIGITAL_CONTENT_STATUS_MAPPING,
  DigitalContentDetailMode,
  DigitalContentDetailViewModel,
  HeaderService,
  MetadataEditorComponent,
  MetadataEditorService,
  NavigationMenuService,
  OpalFooterService
} from '@opal20/domain-components';
import {
  BaseFormComponent,
  DateUtils,
  IFormBuilderDefinition,
  ModuleFacadeService,
  NotificationType,
  ScheduledTask,
  Utils
} from '@opal20/infrastructure';
import { Component, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { DetailTitleComponent, DetailTitleSettings } from '@opal20/common-components';
import { IDigitalContentDetailNavigationData, IDigitalContentRepositoryNavigationData } from '../ccpm-navigation-data';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import { Subscription, from } from 'rxjs';

import { AccessRightTabComponent } from './tabs/access-right-tab/access-right-tab.component';
import { AuditLogTabComponent } from './tabs/audit-log-tab/audit-log-tab.component';
import { CCPMRoutePaths } from '../ccpm.config';
import { Constants } from '../constants/ccpm.common.constant';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { DigitalAdditionalInformationTabComponent } from './digital-additional-information-tab.component';
import { DigitalUploadContentEditorComponent } from './digital-upload-content-editor-page.component';
import { TransferOwnershipDialogComponent } from './dialogs/transfer-ownership-dialog.component';
import { XmlEntities } from 'html-entities';

type PreviewMode = 'Mobile' | 'Web' | 'None';
@Component({
  selector: 'app-digital-content-detail-page',
  templateUrl: './digital-content-detail-page.component.html',
  encapsulation: ViewEncapsulation.None
})
export class DigitalContentDetailPageComponent extends BaseFormComponent {
  public get commentTabInput(): CommentTabInput {
    return {
      originalObjectId: this.contentViewModel.originalObjectId,
      commentServiceType: CommentServiceType.DigitalContent,
      hasReply: true
    };
  }
  public digitalContentStatusMapping = DIGITAL_CONTENT_STATUS_MAPPING;
  public previewMode: PreviewMode = 'None';
  public uploadedContent: string = DigitalContentType.UploadedContent;
  public learningContent: string = DigitalContentType.LearningContent;
  public detailTitleSettings: DetailTitleSettings;
  public previewOptions: unknown[];
  public digitalContentStatus: typeof DigitalContentStatus = DigitalContentStatus;
  public ContextMenuAction: typeof ContextMenuAction = ContextMenuAction;
  public versionTrackingType: VersionTrackingType = VersionTrackingType.DigitalContent;
  public accessRightType: AccessRightType = AccessRightType.DigitalContent;
  public commentContentType: CommentServiceType = CommentServiceType.DigitalContent;
  public contentViewModel: DigitalContentDetailViewModel = new DigitalContentDetailViewModel(new DigitalContent());

  public mode: DigitalContentDetailMode = DigitalContentDetailMode.Create;
  // Broken link properties
  public brokenLinkModule: BrokenLinkModuleIdentifier = BrokenLinkModuleIdentifier.Content;
  public ccpmPermissions = CCPM_PERMISSIONS;

  public dialogRejectRef: DialogRef;
  public dialogApprovalRef: DialogRef;

  public readonly MAX_LENGTH_TITLE: number = Constants.MAX_LENGTH_DIGITAL_CONTENT_TITLE;

  private copyrightForm: CopyrightFormComponent;
  private metadataEditor: MetadataEditorComponent;
  private isSubmitForApproval: boolean = false;
  private originalResourceMetadata: ResourceModel;
  private originalSaveDigitalContentRequest: IDigitalContentRequest;
  private autoSaveScheduleTaskId: string = 'auto-save-digital-content';
  private loadContentInfoSub: Subscription = new Subscription();
  private contentModeBeforePreview: DigitalContentDetailMode;

  private users: UserInfoModel[] = [];

  @ViewChild(DetailTitleComponent, { static: false })
  private detailTitleForm: DetailTitleComponent;

  @ViewChild(TabStripComponent, { static: false })
  private tabstrip: TabStripComponent;

  @ViewChild(AuditLogTabComponent, { static: false })
  private auditLogTab: AuditLogTabComponent;

  @ViewChild(AccessRightTabComponent, { static: false })
  private accessRightTab: AccessRightTabComponent;

  @ViewChild('dialogRejectCommentTemplate', { static: false })
  private dialogRejectCommentTemplate: TemplateRef<unknown>;

  @ViewChild('dialogApprovalCommentTemplate', { static: false })
  private dialogApprovalCommentTemplate: TemplateRef<unknown>;

  @ViewChild(BrokenLinkReportTabComponent, { static: false })
  private brokenLinkReportTab: BrokenLinkReportTabComponent;

  @ViewChild(DigitalAdditionalInformationTabComponent, { static: false })
  private additionalInfoTab: DigitalAdditionalInformationTabComponent;

  @ViewChild(DigitalUploadContentEditorComponent, { static: false })
  private digitalUploadContentEditorComponent: DigitalUploadContentEditorComponent;

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private contentApiService: ContentApiService,
    private headerService: HeaderService,
    private navigationMenuService: NavigationMenuService,
    private opalFooterService: OpalFooterService,
    private appToolbarService: AppToolbarService,
    private metadataEditorSvc: MetadataEditorService,
    private commentApiService: CommentApiService,
    private taggingBackendService: TaggingApiService
  ) {
    super(moduleFacadeService);
    this.initDefaultPdOpportunityType();
    this.initDetailTitleSettings();
    this.initPreviewOptions();
    this.registerDigitalContentAutoSaveScheduledTask(this.autoSaveScheduleTaskId);
  }

  public get canShowEditButton(): boolean {
    return (
      this.mode === DigitalContentDetailMode.View &&
      (this.contentViewModel.status === DigitalContentStatus.Draft ||
        this.contentViewModel.status === DigitalContentStatus.Rejected ||
        this.contentViewModel.status === DigitalContentStatus.Unpublished ||
        this.contentViewModel.status === DigitalContentStatus.Approved)
    );
  }

  public get canSubmitDigitalContent(): boolean {
    return (
      (this.mode === DigitalContentDetailMode.Edit ||
        this.mode === DigitalContentDetailMode.Create ||
        this.mode === DigitalContentDetailMode.View) &&
      (this.contentViewModel.status === DigitalContentStatus.Draft || this.contentViewModel.status === DigitalContentStatus.Rejected)
    );
  }

  public get canShowArchiveButton(): boolean {
    return (
      this.isOwner &&
      this.mode !== DigitalContentDetailMode.ForApprover &&
      (this.contentViewModel.status === DigitalContentStatus.Draft ||
        this.contentViewModel.status === DigitalContentStatus.Rejected ||
        this.contentViewModel.status === DigitalContentStatus.Unpublished ||
        this.contentViewModel.status === DigitalContentStatus.Approved)
    );
  }

  public get canDuplicateDigitalContent(): boolean {
    return (
      this.contentViewModel.archiveDate &&
      (this.contentViewModel.status === DigitalContentStatus.Archived &&
        DateUtils.compareDate(new Date(this.contentViewModel.archiveDate), DateUtils.addYear(new Date(), -3)) >= 0)
    );
  }

  public get isContentCreator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.ContentCreator);
  }

  public get isCourseContentCreator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.CourseContentCreator);
  }

  public get isOwner(): boolean {
    return this.currentUser.extId === this.contentViewModel.data.ownerId;
  }

  public get isAllowAddCollaborator(): boolean {
    return (
      this.contentViewModel.status !== DigitalContentStatus.Archived &&
      (this.currentUser.hasAdministratorRoles() || this.currentUser.extId === this.contentViewModel.data.ownerId)
    );
  }

  public get isCourseFacilitator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.CourseFacilitator);
  }

  public get isApprovingOfficer(): boolean {
    return (
      this.currentUser.hasAdministratorRoles() ||
      this.currentUser.hasRole(SystemRoleEnum.MOEHQContentApprovingOfficer, SystemRoleEnum.SchoolContentApprovingOfficer)
    );
  }

  public get canApprovalDigitalContent(): boolean {
    return this.mode === DigitalContentDetailMode.ForApprover && this.contentViewModel.status === DigitalContentStatus.PendingForApproval;
  }

  public get canPublishDigitalContent(): boolean {
    return this.mode !== DigitalContentDetailMode.ForApprover && this.contentViewModel.status === DigitalContentStatus.Approved;
  }

  public get canUnpublishDigitalContent(): boolean {
    return this.mode !== DigitalContentDetailMode.ForApprover && this.contentViewModel.status === DigitalContentStatus.Published;
  }

  public get canSaveDigitalContent(): boolean {
    return (
      (this.mode === DigitalContentDetailMode.Edit || this.mode === DigitalContentDetailMode.Create) &&
      (this.contentViewModel.status === DigitalContentStatus.Draft ||
        this.contentViewModel.status === DigitalContentStatus.Rejected ||
        this.contentViewModel.status === DigitalContentStatus.Unpublished ||
        this.contentViewModel.status === DigitalContentStatus.Approved)
    );
  }

  public get canShowMarkAsReadyToUseButton(): boolean {
    return this.contentViewModel.status === DigitalContentStatus.Draft;
  }

  public get canShowMarkAsDraftButton(): boolean {
    return this.contentViewModel.status === DigitalContentStatus.ReadyToUse;
  }

  public createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        editor: {
          defaultValue: '',
          validators: []
        }
      }
    };
  }

  public initData(initId?: string): void {
    this.loadContentInfoSub.unsubscribe();
    const data: IDigitalContentDetailNavigationData = this.getNavigateData<IDigitalContentDetailNavigationData>();
    const id = initId || data.id;
    this.mode = data.mode ? data.mode : DigitalContentDetailMode.View;
    this.updateDeeplink(`ccpm/content/${id}`);
    this.loadContentInfoSub = from(this.contentApiService.getDigitalContent(id))
      .pipe(this.untilDestroy())
      .subscribe(
        content => {
          if (content.isArchived || !content) {
            this.contentAccessDenied();
            return;
          }

          this.contentViewModel = new DigitalContentDetailViewModel(content);
          this.patchInitialFormValue({
            editor: this.contentViewModel.htmlContent
          });
          this.originalSaveDigitalContentRequest = this.buildSaveDigitalContentRequest(
            this.contentViewModel.data,
            this.contentViewModel.data.title
          );
          this.metadataEditorSvc.setResourceInfo(id, ResourceType.Content);
        },
        err => {
          this.contentAccessDenied(false);
        }
      );
  }

  public async onBackAction(): Promise<void> {
    if (this.contentViewModel) {
      await this.performAutoSaveDigitalContent();
      this.metadataEditorSvc.resetMetadataSubjects();
    }
    this.navigateTo(CCPMRoutePaths.DigitalContentRepository, <IDigitalContentRepositoryNavigationData>{});
  }

  public contentAccessDenied(showError: boolean = true): void {
    this.navigateTo(CCPMRoutePaths.DigitalContentRepository, <IDigitalContentRepositoryNavigationData>{});
    if (showError) {
      this.moduleFacadeService.modalService.showErrorMessage(this.translate('You do not have the permission to access this content.'));
    }
  }

  public onPreviewerChange(mode: PreviewMode): void {
    this.showWebPreviewer();
  }

  public onLoad(controls: { copyrightForm: CopyrightFormComponent; metadataEditor: MetadataEditorComponent }): void {
    this.metadataEditor = controls.metadataEditor;
    this.copyrightForm = controls.copyrightForm;
  }

  public onRejectClick(): void {
    this.dialogRejectRef = this.moduleFacadeService.dialogService.open({
      content: this.dialogRejectCommentTemplate,
      width: 700
    });
  }

  public async onSubmitForStatus(status: DigitalContentStatus): Promise<void> {
    const isValid = await this.additionalCanSaveCheck();
    if (isValid) {
      const showSpinner = true;
      this.isSubmitForApproval = status === DigitalContentStatus.PendingForApproval ? true : this.isSubmitForApproval;
      this.changeModeStatus(status);
      const request: IDigitalContentRequest = this.buildSaveDigitalContentRequest();
      await this.contentApiService.updateDigitalContent(request, showSpinner);
      await this.metadataEditorSvc
        .saveCurrentResourceMetadataForm(this.contentViewModel.id, ResourceType.Content, true, showSpinner, true)
        .toPromise()
        .then(() => {
          if (status === DigitalContentStatus.ReadyToUse) {
            this.showNotification(`${this.contentViewModel.title} is successfully marked as ready`, NotificationType.Success);
          } else if (status === DigitalContentStatus.PendingForApproval) {
            this.showNotification(`${this.contentViewModel.title} is successfully pending for approval`, NotificationType.Success);
          }
        });
      await this.contentApiService.changeApprovalStatus({
        id: this.contentViewModel.id,
        status: status
      });
      this.onBackAction();
    } else if (this.mode === DigitalContentDetailMode.View) {
      this.mode = DigitalContentDetailMode.Edit;
    }
  }

  public async saveCommentAndRejectClick(comment: string): Promise<void> {
    const showSpinner = true;
    this.isSubmitForApproval = true;
    if (comment) {
      await this.commentApiService.saveComment(
        {
          content: comment,
          objectId: this.contentViewModel.originalObjectId
        },
        showSpinner
      );
    }

    this.dialogRejectRef.close();
    await this.contentApiService
      .changeApprovalStatus({
        id: this.contentViewModel.id,
        status: DigitalContentStatus.Rejected,
        comment: comment
      })
      .then(() => {
        this.showNotification(`${this.contentViewModel.title} is successfully rejected`, NotificationType.Success);
      });

    this.onBackAction();
  }

  public async onApprovalClick(): Promise<void> {
    const isValid = await this.additionalCanSaveCheck();
    if (isValid) {
      this.dialogApprovalRef = this.moduleFacadeService.dialogService.open({
        content: this.dialogApprovalCommentTemplate,
        width: 700
      });
    }
  }

  public async saveCommentAndApproveClick(comment: string): Promise<void> {
    const showSpinner = true;
    this.isSubmitForApproval = true;
    if (comment) {
      await this.commentApiService.saveComment(
        {
          content: comment,
          objectId: this.contentViewModel.originalObjectId
        },
        showSpinner
      );
    }

    this.dialogApprovalRef.close();
    const request: IDigitalContentRequest = this.buildSaveDigitalContentRequest();
    await this.contentApiService.updateDigitalContent(request, showSpinner);
    await this.metadataEditorSvc
      .saveCurrentResourceMetadataForm(this.contentViewModel.id, ResourceType.Content, true, showSpinner, true)
      .toPromise();
    await this.contentApiService
      .changeApprovalStatus({
        id: this.contentViewModel.id,
        status: DigitalContentStatus.Approved,
        comment: comment
      })
      .then(() => {
        this.showNotification(`${this.contentViewModel.title} is successfully approved`, NotificationType.Success);
      });

    this.onBackAction();
  }

  /**
   * Workaround for demo 27 March 2020
   * This issue will fix on next sprint.
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
        case 6:
          this.brokenLinkReportTab.loadData(true);
          break;
      }
    }
  }

  public onEditButtonClick(): void {
    this.changeModeStatus(DigitalContentStatus.Draft);
  }

  public onDuplicateBtnClicked(): void {
    this.contentApiService.duplicateDigitalContent(this.contentViewModel.id).then(_ => {
      this.taggingBackendService.cloneResource(_.id, this.contentViewModel.id).toPromise();
      this.showNotification(`${this.contentViewModel.title} is successfully duplicated`, NotificationType.Success);
      this.onBackAction();
    });
  }

  public onTransferOwnershipBtnClicked(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: TransferOwnershipDialogComponent });
    const configurationPopup = dialogRef.content.instance as TransferOwnershipDialogComponent;
    configurationPopup.userAcceptableRoles = this.getUserAcceptTableRolesForTransfer(this.contentViewModel.status);

    dialogRef.result.toPromise().then((newOwnerId: string) => {
      if (typeof newOwnerId === 'string') {
        this.onTransferOwnerShip(this.contentViewModel.id, newOwnerId);
      }
    });
  }

  public onTransferOwnerShip(id: string, newOwnerId: string): void {
    this.contentApiService
      .transferOwnerShip({
        objectId: id,
        newOwnerId: newOwnerId
      })
      .then(() => {
        this.showNotification('Ownership transferred successfully.', NotificationType.Success);
        this.onBackAction();
      });
  }

  public getUserAcceptTableRolesForTransfer(status: DigitalContentStatus): SystemRoleEnum[] {
    switch (status) {
      case DigitalContentStatus.ReadyToUse: {
        return [SystemRoleEnum.CourseContentCreator];
      }
      case DigitalContentStatus.PendingForApproval:
      case DigitalContentStatus.Approved:
      case DigitalContentStatus.Rejected:
      case DigitalContentStatus.Published:
      case DigitalContentStatus.Unpublished: {
        return [SystemRoleEnum.ContentCreator];
      }
      default:
        break;
    }
  }

  public onRevertData(revertChangeResult: IRevertVersionTrackingResult): void {
    if (revertChangeResult && revertChangeResult.isSuccess) {
      this.initData(revertChangeResult.newActiveId);
    }
  }

  public onArchiveBtnClicked(): void {
    this.contentApiService
      .archiveContent({
        objectId: this.contentViewModel.id,
        archiveByUserId: this.currentUser.extId
      })
      .subscribe(() => {
        this.showNotification(`${this.contentViewModel.title} is successfully archived`, NotificationType.Success);
        this.onBackAction();
      });
  }

  public changeStatus(action: ContextMenuAction): void {
    let status = DigitalContentStatus.Draft;
    switch (action) {
      case ContextMenuAction.Approve:
        status = DigitalContentStatus.Approved;
        break;
      case ContextMenuAction.Reject:
        status = DigitalContentStatus.Rejected;
        break;
      case ContextMenuAction.Publish:
        status = DigitalContentStatus.Published;
        break;
      case ContextMenuAction.Unpublish:
        status = DigitalContentStatus.Unpublished;
        break;
      default:
        break;
    }

    if (status === DigitalContentStatus.Draft) {
      return;
    }

    this.contentApiService.changeApprovalStatus({ id: this.contentViewModel.id, status: status }).then(() => {
      switch (action) {
        case ContextMenuAction.Approve:
          this.showNotification(`${this.contentViewModel.title} is successfully approved`, NotificationType.Success);
          break;
        case ContextMenuAction.Reject:
          this.showNotification(`${this.contentViewModel.title} is successfully rejected`, NotificationType.Success);
          break;
        case ContextMenuAction.Publish:
          this.showNotification(`${this.contentViewModel.title} is successfully published`, NotificationType.Success);
          break;
        case ContextMenuAction.Unpublish:
          this.showNotification(`${this.contentViewModel.title} is successfully unpublished`, NotificationType.Success);
          break;
        default:
          break;
      }

      this.onBackAction();
    });
  }

  public additionalCanSaveCheck(controls?: string[]): Promise<boolean> {
    const digitalUploadContentEditorValidate: Promise<boolean> = this.digitalUploadContentEditorComponent
      ? this.digitalUploadContentEditorComponent.validate()
      : Promise.resolve(true);
    /**
     * Workaround for demo 27 March 2020
     * This issue will fix on next sprint.
     */
    return Promise.all([
      this.copyrightForm.validate(),
      this.metadataEditor.validate(),
      this.additionalInfoTab.validate(),
      this.detailTitleForm.validate(),
      digitalUploadContentEditorValidate
    ]).then(result => {
      if ((!result[0] || !result[1]) && !this.tabstrip.tabs.toArray()[1].active) {
        this.onTabSelect();

        this.tabstrip.selectTab(1);
        setTimeout(() => this.validate(), 0);
      }
      if (!result[3]) {
        this.detailTitleForm.enableEditTitle();
        setTimeout(() => this.detailTitleForm.validate(), 100);
      }
      return result.findIndex(p => !p) < 0;
    });
  }

  public saveInBackground(): Promise<IDigitalContent> {
    const showSpinner = false;
    const request: IDigitalContentRequest = this.buildSaveDigitalContentRequest();
    request.isAutoSave = true;
    return this.contentApiService
      .updateDigitalContent(request, showSpinner)
      .then(content =>
        Promise.resolve()
          .then(() =>
            this.metadataEditorSvc
              .saveCurrentResourceMetadataForm(this.contentViewModel.id, ResourceType.Content, true, showSpinner, true)
              .toPromise()
          )
          .then(() => content)
      )
      .then(content => {
        this.showNotification(this.translate('Digital content saved successfully.'), NotificationType.Success);
        return content;
      });
  }

  public onSaveDigitalContent(digitalContentStatus?: DigitalContentStatus, isSave?: boolean, showSpinner: boolean = true): void {
    this.contentViewModel.status = digitalContentStatus ? digitalContentStatus : this.contentViewModel.status;
    const request: IDigitalContentRequest = this.buildSaveDigitalContentRequest();
    this.contentApiService
      .updateDigitalContent(request, showSpinner)
      .then(content => {
        this.contentViewModel.updatedata(content);
        this.changeModeStatus(this.contentViewModel.status);
        this.patchInitialFormValue({
          editor: this.contentViewModel.htmlContent,
          description: this.contentViewModel.description
        });
      })
      .then(_ =>
        this.metadataEditorSvc
          .saveCurrentResourceMetadataForm(this.contentViewModel.id, ResourceType.Content, !this.isSubmitForApproval, showSpinner, false)
          .toPromise()
      )
      .then(() => {
        const contentStatus = this.contentViewModel.status;
        if (!isSave) {
          switch (contentStatus) {
            case DigitalContentStatus.Draft:
              this.showNotification(`${this.contentViewModel.title} is successfully marked as draft`, NotificationType.Success);
              break;
          }
        } else {
          this.showNotification(`${this.contentViewModel.title} is successfully saved`, NotificationType.Success);
        }
      })
      .then(() => {
        this.auditLogTab.loadData(false);
        this.auditLogTab.loadActiveVersion(false);
      });
  }

  public showWebPreviewer(): void {
    this.onSaveDigitalContent();
    this.contentModeBeforePreview = this.mode;
    this.appToolbarService.hide();
    this.headerService.hide();
    this.navigationMenuService.hide();
    this.opalFooterService.hide();
  }

  public hideWebOrMobilePreviewer(): void {
    this.mode = this.contentModeBeforePreview;
    this.headerService.show();
    this.appToolbarService.show();
    this.navigationMenuService.show();
    this.opalFooterService.show();
    this.previewMode = 'None';
  }

  public async performAutoSaveDigitalContent(): Promise<void> {
    if (
      this.mode === DigitalContentDetailMode.Edit &&
      this.previewMode !== 'Web' &&
      this.previewMode !== 'Mobile' &&
      this.isFormDataChanged
    ) {
      const currentSaveDigitalContentRequest: IDigitalContentRequest = this.buildSaveDigitalContentRequest();
      this.originalSaveDigitalContentRequest = currentSaveDigitalContentRequest;
      await this.saveInBackground();
    }
  }

  public onRejectCommentCancel(): void {
    this.dialogRejectRef.close();
  }

  public onApprovalCommentCancel(): void {
    this.dialogApprovalRef.close();
  }

  protected onInit(): void {
    this.subscribeResourceData();
  }

  protected onDestroy(): void {
    this.clearNavigateData();
    this.unregisterDigitalContentAutoSaveScheduledTask(this.autoSaveScheduleTaskId);
    this.metadataEditorSvc.resetDigitalContentAutoSaveInformerSubjet();
  }

  private registerDigitalContentAutoSaveScheduledTask(taskId: string): void {
    const autoSaveDigitalContentTask = new ScheduledTask(
      taskId,
      Constants.AUTOSAVE_INTERVALTIME,
      () => this.performAutoSaveDigitalContent(),
      false
    );
    this.moduleFacadeService.localScheduleService.register(autoSaveDigitalContentTask);
  }

  private unregisterDigitalContentAutoSaveScheduledTask(taskId: string): void {
    this.moduleFacadeService.localScheduleService.unregister(taskId);
  }

  private get isFormDataChanged(): boolean {
    const currentSaveDigitalContentRequest: IDigitalContentRequest = this.buildSaveDigitalContentRequest();
    const isDifferentSaveDigitalContentRequest = Utils.isDifferent(
      this.originalSaveDigitalContentRequest,
      currentSaveDigitalContentRequest
    );
    const isDifferentResourceMetadata = Utils.isDifferent(this.originalResourceMetadata, this.metadataEditorSvc.currentResource);
    return isDifferentSaveDigitalContentRequest || isDifferentResourceMetadata;
  }

  private buildSaveDigitalContentRequest(copyRightDataSource?: ICopyright, titleDataSource?: string): IDigitalContentRequest {
    const copyRightData: ICopyright = copyRightDataSource ? copyRightDataSource : this.copyrightForm.getCopyrightData();
    // TODO: Refactor model bellow
    const model: IDigitalContent = {
      ...{ id: this.contentViewModel.id, type: this.contentViewModel.type, status: this.contentViewModel.status },
      title: titleDataSource ? titleDataSource : this.detailTitleForm.title,
      description: new XmlEntities().encode(this.additionalInfoTab.form.value.description),
      ...copyRightData,
      htmlContent: new XmlEntities().encode(this.form.value.editor),

      alternativeApprovingOfficerId: this.additionalInfoTab.form.value.alternativeApprovingOfficerId || null,
      primaryApprovingOfficerId: this.additionalInfoTab.form.value.primaryApprovingOfficerId || null,
      isAutoPublish: this.additionalInfoTab.form.value.isAutoPublish,

      archiveDate: this.additionalInfoTab.form.value.archiveDate
        ? DateUtils.setTimeToEndInDay(this.additionalInfoTab.form.value.archiveDate)
        : null,

      autoPublishDate: this.additionalInfoTab.form.value.autoPublishDate
        ? DateUtils.setTimeToEndInDay(this.additionalInfoTab.form.value.autoPublishDate)
        : null,

      fileName: this.contentViewModel.data.fileName,
      fileExtension: this.contentViewModel.data.fileExtension,
      fileSize: this.contentViewModel.data.fileSize,
      fileType: this.contentViewModel.data.fileType,
      fileLocation: this.contentViewModel.data.fileLocation,
      fileDuration: this.contentViewModel.data.fileDuration,
      chapters: Utils.cloneDeep(this.contentViewModel.data.chapters)
    };
    let request: IDigitalContentRequest;

    if (model.type === DigitalContentType.LearningContent) {
      request = new DigitalLearningContentRequest(model);
    } else if (model.type === DigitalContentType.UploadedContent) {
      request = new DigitalUploadContentRequest(model);
    }

    request.isSubmitForApproval = this.isSubmitForApproval;
    return request;
  }

  private subscribeResourceData(): void {
    this.metadataEditorSvc.resource$.pipe(this.untilDestroy()).subscribe(resource => {
      this.originalResourceMetadata = Utils.cloneDeep(resource);
    });
  }

  private initDefaultPdOpportunityType(): void {
    this.metadataEditorSvc.resourceMetadataForm$.pipe(this.untilDestroy()).subscribe(respone => {
      if (respone && !this.metadataEditorSvc.currentResource.tags.includes(MetadataId.WebPageLearningFromPDResource)) {
        this.metadataEditorSvc.currentResource.tags.push(MetadataId.WebPageLearningFromPDResource);
      }
    });
  }

  private initDetailTitleSettings(): void {
    this.detailTitleSettings = new DetailTitleSettings();
    this.detailTitleSettings.titleValidators = DigitalContent.titleValidators;
    this.detailTitleSettings.editModeEnabled = false;
  }

  private initPreviewOptions(): void {
    this.previewOptions = [
      {
        text: this.translate('Mobile'),
        click: () => {
          this.previewMode = 'Mobile';
          this.mode = DigitalContentDetailMode.View;
        }
      },
      {
        text: this.translate('Web'),
        click: () => {
          this.previewMode = 'Web';
          this.mode = DigitalContentDetailMode.View;
        }
      }
    ];
  }

  private changeModeStatus(digitalContentStatus: DigitalContentStatus): void {
    switch (digitalContentStatus) {
      case DigitalContentStatus.Draft:
        this.mode = DigitalContentDetailMode.Edit;
        break;
      case DigitalContentStatus.ReadyToUse:
      case DigitalContentStatus.PendingForApproval:
        this.mode = DigitalContentDetailMode.View;
        break;
      default:
        break;
    }
  }
}
