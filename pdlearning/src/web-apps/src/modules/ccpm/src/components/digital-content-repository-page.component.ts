import { BasePageComponent, CustomFormGroup, ModuleFacadeService, NotificationType, UploadParameters } from '@opal20/infrastructure';
import {
  CCPM_PERMISSIONS,
  ContextMenuAction,
  ContextMenuEmit,
  DigitalContentDetailMode,
  DigitalContentSearchTermService,
  ListDigitalContentPageComponent,
  PersonalFileDialogComponent
} from '@opal20/domain-components';
import { Component, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  ContentApiService,
  CopyrightLicenseTerritory,
  CopyrightLicenseType,
  CopyrightOwnership,
  DIGITAL_CONTENT_QUERY_MODE_LABEL,
  DIGITAL_CONTENT_SORT_ITEMS,
  DigitalContentQueryMode,
  DigitalContentStatus,
  DigitalLearningContentRequest,
  DigitalUploadContentRequest,
  IDigitalContent,
  SystemRoleEnum,
  UserInfoModel
} from '@opal20/domain-api';
import { ContextMenuItem, FileUploaderUtils, OpalDialogService } from '@opal20/common-components';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';

import { CCPMRoutePaths } from '../ccpm.config';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { IDigitalContentDetailNavigationData } from '../ccpm-navigation-data';
import { TransferOwnershipDialogComponent } from './dialogs/transfer-ownership-dialog.component';

@Component({
  selector: 'digital-content-repository-page',
  templateUrl: './digital-content-repository-page.component.html',
  encapsulation: ViewEncapsulation.None
})
export class DigitalContentRepositoryPageComponent extends BasePageComponent {
  public textSearch: string = '';
  public submitSearch: string = '';
  public formGroup: CustomFormGroup | undefined;
  public queryModePendingApproval: DigitalContentQueryMode = DigitalContentQueryMode.PendingApproval;
  public queryModeAllByCurrentUser: DigitalContentQueryMode = DigitalContentQueryMode.AllByCurrentUser;
  public queryModeArchived: DigitalContentQueryMode = DigitalContentQueryMode.Archived;

  public digitalContentSortDisplayItems = DIGITAL_CONTENT_SORT_ITEMS;
  public ccpmPermissions = CCPM_PERMISSIONS;

  public contextMenuItemsForFilesTab: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Publish,
      text: this.translateCommon('Publish'),
      icon: 'check'
    },
    {
      id: ContextMenuAction.Unpublish,
      text: this.translateCommon('Unpublish'),
      icon: 'cancel'
    },
    {
      id: ContextMenuAction.Rename,
      text: this.translateCommon('Rename'),
      icon: 'edit'
    },
    {
      id: ContextMenuAction.Duplicate,
      text: this.translateCommon('Duplicate'),
      icon: 'aggregate-fields'
    },
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    },
    {
      id: ContextMenuAction.TransferOwnership,
      text: this.translateCommon('Transfer Ownership'),
      icon: 'user'
    },
    {
      id: ContextMenuAction.Archive,
      text: this.translateCommon('Archive'),
      icon: 'select-box'
    }
  ];

  public contextMenuItemsForApprovalTab: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Publish,
      text: this.translateCommon('Publish'),
      icon: 'check'
    },
    {
      id: ContextMenuAction.Unpublish,
      text: this.translateCommon('Unpublish'),
      icon: 'cancel'
    },
    {
      id: ContextMenuAction.Rename,
      text: this.translateCommon('Rename'),
      icon: 'edit'
    },
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    }
  ];
  public digitalContentFilterStatus: DigitalContentStatus = DigitalContentStatus.All;

  public filterItems: IDataItem[] = [
    {
      text: this.translateCommon('All'),
      value: DigitalContentStatus.All
    },
    {
      text: this.translateCommon('Approved'),
      value: DigitalContentStatus.Approved
    },
    {
      text: this.translateCommon('Draft'),
      value: DigitalContentStatus.Draft
    },
    {
      text: this.translateCommon('Expired'),
      value: DigitalContentStatus.Expired
    },
    {
      text: this.translateCommon('Pending Approval'),
      value: DigitalContentStatus.PendingForApproval
    },
    {
      text: this.translateCommon('Published'),
      value: DigitalContentStatus.Published
    },
    {
      text: this.translateCommon('Ready For Use'),
      value: DigitalContentStatus.ReadyToUse
    },
    {
      text: this.translateCommon('Rejected'),
      value: DigitalContentStatus.Rejected
    },
    {
      text: this.translateCommon('Unpublished'),
      value: DigitalContentStatus.Unpublished
    }
  ];

  public contextMenuItemsForArchivedTab: ContextMenuItem[] = [
    {
      id: ContextMenuAction.TransferOwnership,
      text: this.translateCommon('Transfer Ownership'),
      icon: 'user'
    },
    {
      id: ContextMenuAction.Duplicate,
      text: this.translateCommon('Duplicate'),
      icon: 'aggregate-fields'
    }
  ];

  public digitalContentQueryModeLabel = DIGITAL_CONTENT_QUERY_MODE_LABEL;
  public digitalContentQueryMode = DigitalContentQueryMode;

  public get isContentCreator(): boolean {
    if (!this.currentUser) {
      return false;
    }
    return (
      this.currentUser.hasAdministratorRoles() ||
      this.currentUser.hasRole(SystemRoleEnum.ContentCreator, SystemRoleEnum.CourseContentCreator)
    );
  }

  public get isCourseFacilitator(): boolean {
    if (!this.currentUser) {
      return false;
    }
    return this.currentUser.hasRole(SystemRoleEnum.CourseFacilitator);
  }

  @ViewChild('digitalContentTabstrip', { static: true })
  private digitalContentTabstripEl: TabStripComponent;
  @ViewChild(ListDigitalContentPageComponent, { static: false })
  private digitalContentList: ListDigitalContentPageComponent;
  private fakeModel: IDigitalContent = {
    type: 'LearningContent',
    attributionElements: [],
    title: 'Draft',
    description: '',
    ownership: CopyrightOwnership.MoeOwned,
    licenseType: CopyrightLicenseType.Perpetual,
    termsOfUse: '',
    startDate: null,
    expiredDate: null,
    publisher: null,
    acknowledgementAndCredit: null,
    remarks: null,
    licenseTerritory: CopyrightLicenseTerritory.Singapore,
    isAllowReusable: false,
    isAllowDownload: false,
    isAllowModification: false,
    isAutoPublish: true
  };

  public get canViewPendingApprovalList(): boolean {
    return (
      this.currentUser &&
      (this.currentUser.hasAdministratorRoles() ||
        this.currentUser.hasRole(SystemRoleEnum.MOEHQContentApprovingOfficer, SystemRoleEnum.SchoolContentApprovingOfficer))
    );
  }

  public get canViewArchivalList(): boolean {
    return (
      this.currentUser &&
      (this.currentUser.hasAdministratorRoles() ||
        this.currentUser.hasRole(SystemRoleEnum.CourseContentCreator, SystemRoleEnum.ContentCreator, SystemRoleEnum.CourseFacilitator))
    );
  }

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  private uploadContentModel: IDigitalContent = {
    title: 'Draft',
    type: 'UploadedContent',
    ownership: CopyrightOwnership.MoeOwned,
    licenseType: CopyrightLicenseType.Perpetual,
    termsOfUse: '',
    startDate: null,
    expiredDate: null,
    publisher: null,
    acknowledgementAndCredit: null,
    remarks: null,
    licenseTerritory: CopyrightLicenseTerritory.Singapore,
    isAllowReusable: false,
    isAllowDownload: false,
    isAllowModification: false,
    attributionElements: [],
    isAutoPublish: true
  };

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private opalDialogService: OpalDialogService,
    private contentApiService: ContentApiService,
    public searchTermService: DigitalContentSearchTermService,
    private digitalContentBackendService: ContentApiService
  ) {
    super(moduleFacadeService);
    this.initTextSearch();
  }

  public onInit(): void {
    this.updateDeeplink('ccpm');
  }

  public openBatchUpload(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(PersonalFileDialogComponent);
    const configurationPopup = dialogRef.content.instance as PersonalFileDialogComponent;
    configurationPopup.uploadFolder = 'digital-contents';
    configurationPopup.icon = 'assets/images/icons/add-file.svg';
    configurationPopup.settings.extensions = FileUploaderUtils.uploadContentAllowedExtensions;
    dialogRef.result.subscribe((result: UploadParameters[]) => {
      if (result && result.length > 0) {
        if (result.length === 1) {
          const fileUploadModel = this.createFileUploadModel(result[0]);
          this.digitalContentBackendService.createDigitalContent(new DigitalUploadContentRequest(fileUploadModel), true).then(data => {
            this.navigateTo(CCPMRoutePaths.DigitalContentDetailPage, <IDigitalContentDetailNavigationData>{
              id: data.id,
              mode: DigitalContentDetailMode.Edit
            });
          });
        } else {
          this.processCreateContents(result).then(() => {
            this.digitalContentList.loadData();
          });
        }
      }
    });
  }

  public onCreateNewButtonClick(): void {
    this.contentApiService.createDigitalContent(new DigitalLearningContentRequest(this.fakeModel)).then(_ => {
      this.navigateTo(CCPMRoutePaths.DigitalContentDetailPage, <IDigitalContentDetailNavigationData>{
        id: _.id,
        mode: DigitalContentDetailMode.Edit
      });
    });
  }

  public onSubmitSearch(): void {
    this.submitSearch = this.textSearch.slice();
    if (this.searchTermService.searchText !== this.submitSearch) {
      this.searchTermService.searchText = this.submitSearch;
      this.searchTermService.state.skip = 0;
    }
  }

  public digitalContentMenuItemSelected(contextMenuEmit: ContextMenuEmit<IDigitalContent>): void {
    switch (contextMenuEmit.event.item.id) {
      case 'rename':
        this.digitalContentList.editGridRow(contextMenuEmit.rowIndex, contextMenuEmit.dataItem);
        break;
      case 'delete':
        this.digitalContentList.deleteContent(contextMenuEmit.dataItem.id, contextMenuEmit.dataItem.title);
        break;
      case 'duplicate':
        this.digitalContentList.duplicateContent(contextMenuEmit.dataItem.id, contextMenuEmit.dataItem.title);
        break;
      case 'publish':
        this.contentApiService
          .changeApprovalStatus({
            id: contextMenuEmit.dataItem.id,
            status: DigitalContentStatus.Published
          })
          .then(() => {
            this.showNotification(`${contextMenuEmit.dataItem.title} is successfully published`, NotificationType.Success);
            this.digitalContentList.loadData();
          });
        break;
      case 'unpublish':
        this.contentApiService
          .changeApprovalStatus({
            id: contextMenuEmit.dataItem.id,
            status: DigitalContentStatus.Unpublished
          })
          .then(() => {
            this.showNotification(`${contextMenuEmit.dataItem.title} is successfully unpublished`, NotificationType.Success);
            this.digitalContentList.loadData();
          });
        break;
      case 'transferOwnership':
        const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: TransferOwnershipDialogComponent });
        const configurationPopup = dialogRef.content.instance as TransferOwnershipDialogComponent;
        configurationPopup.userAcceptableRoles = this.getUserAcceptTableRolesForTransfer(contextMenuEmit.dataItem.status);

        dialogRef.result.toPromise().then((newOwnerId: string) => {
          if (typeof newOwnerId === 'string') {
            this.onTransferOwnerShip(contextMenuEmit.dataItem.id, newOwnerId);
          }
        });
        break;
      case 'archive':
        this.contentApiService
          .changeApprovalStatus({
            id: contextMenuEmit.dataItem.id,
            status: DigitalContentStatus.Archived
          })
          .then(() => {
            this.showNotification(`${contextMenuEmit.dataItem.title} is successfully archived`, NotificationType.Success);
            this.digitalContentList.loadData();
          });
      default:
        break;
    }
  }

  public onTransferOwnerShip(id: string, newOwnerId: string): void {
    this.contentApiService
      .transferOwnerShip({
        objectId: id,
        newOwnerId: newOwnerId
      })
      .then(() => {
        this.showNotification('Ownership transferred successfully.', NotificationType.Success);
        this.digitalContentList.loadData();
      });
  }

  public onViewDigitalContent(id: string): void {
    this.navigateTo(CCPMRoutePaths.DigitalContentDetailPage, <IDigitalContentDetailNavigationData>{
      id: id,
      mode: DigitalContentDetailMode.View
    });
  }

  public onViewPendingApproveContent(id: string): void {
    this.navigateTo(CCPMRoutePaths.DigitalContentDetailPage, <IDigitalContentDetailNavigationData>{
      id: id,
      mode: DigitalContentDetailMode.ForApprover
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

  public get canShowFilterToolbar(): boolean {
    if (!this.digitalContentTabstripEl.tabs) {
      return false;
    }

    return this.digitalContentTabstripEl.tabs.some(
      tab =>
        tab.title === this.translateCommon(this.digitalContentQueryModeLabel.get(DigitalContentQueryMode.AllByCurrentUser)) &&
        tab.active === true
    );
  }

  public onDigitalContentTabSelect(tabSelectedEvent: SelectEvent): void {
    switch (tabSelectedEvent.title) {
      case this.translateCommon(this.digitalContentQueryModeLabel.get(DigitalContentQueryMode.PendingApproval)):
        this.searchTermService.queryMode = DigitalContentQueryMode.PendingApproval;
        this.searchTermService.searchStatuses = [DigitalContentStatus.PendingForApproval];
        break;
      case this.translateCommon(this.digitalContentQueryModeLabel.get(DigitalContentQueryMode.Archived)):
        this.searchTermService.queryMode = DigitalContentQueryMode.Archived;
        this.searchTermService.searchStatuses = [DigitalContentStatus.Archived];
        this.digitalContentFilterStatus = DigitalContentStatus.Archived;
        break;
      default:
        this.searchTermService.queryMode = DigitalContentQueryMode.AllByCurrentUser;
        this.digitalContentFilterStatus = DigitalContentStatus.All;
    }
  }

  public onOpalSelectControlFocus(): void {
    const event = document.createEvent('MouseEvents');
    event.initEvent('mousedown', true, true);
    document.dispatchEvent(event);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return this.currentUser.permissionDic;
  }

  private initTextSearch(): void {
    if (this.searchTermService.searchText) {
      this.textSearch = this.searchTermService.searchText;
      this.onSubmitSearch();
    }
    this.digitalContentFilterStatus = this.searchTermService.searchStatuses
      ? this.searchTermService.searchStatuses[0]
      : DigitalContentStatus.All;

    if (this.hasPermission(CCPM_PERMISSIONS.ViewListContent)) {
      this.searchTermService.queryMode = DigitalContentQueryMode.AllByCurrentUser;
      return;
    } else if (this.hasPermission(CCPM_PERMISSIONS.ViewListSubmittedContent)) {
      this.searchTermService.queryMode = DigitalContentQueryMode.PendingApproval;
      return;
    } else {
      this.searchTermService.queryMode = DigitalContentQueryMode.Archived;
    }
  }

  private async createContent(fileParameter: UploadParameters): Promise<void | IDigitalContent> {
    return Promise.resolve()
      .then(() => {
        return this.createFileUploadModel(fileParameter);
      })
      .then((fileUploadModel: IDigitalContent) =>
        this.digitalContentBackendService.createDigitalContent(new DigitalUploadContentRequest(fileUploadModel), true)
      );
  }

  private createFileUploadModel(fileParameter: UploadParameters): IDigitalContent {
    const fileUploadModel: IDigitalContent = Object.assign({}, this.uploadContentModel);
    fileUploadModel.title = fileParameter.file ? fileParameter.file.name : fileParameter.fileName;
    fileUploadModel.fileExtension = fileParameter.fileExtension;
    fileUploadModel.fileName = fileParameter.file ? fileParameter.file.name : fileParameter.fileName;
    fileUploadModel.fileLocation = fileParameter.fileLocation;
    fileUploadModel.fileSize = fileParameter.file ? fileParameter.file.size : fileParameter.fileSize;
    fileUploadModel.fileType = fileParameter.mineType;
    return fileUploadModel;
  }

  private processCreateContents(fileParameters: UploadParameters[]): Promise<void | (void | IDigitalContent)[]> {
    return Promise.all(fileParameters.map(parameters => this.createContent(parameters))).catch(error => {
      this.modalService.showErrorMessage(error || 'Create content(s) failed');
    });
  }
}
