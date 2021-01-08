import * as moment_ from 'moment';

import { AbstractControl, Validators } from '@angular/forms';
import {
  BasePageComponent,
  CustomFormControl,
  CustomFormGroup,
  DateUtils,
  FormBuilderService,
  KeyCode,
  ModuleFacadeService,
  NotificationType,
  TranslationMessage
} from '@opal20/infrastructure';
import { CellClickEvent, GridComponent, GridDataResult, PageChangeEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, HostBinding, Input, Output, SimpleChanges, ViewChild } from '@angular/core';
import {
  ContentApiService,
  DIGITAL_CONTENT_SORT_ITEMS,
  DigitalContent,
  DigitalContentQueryMode,
  DigitalContentSortField,
  DigitalContentStatus,
  IDigitalContent,
  LearningContentApiService,
  SortDirection,
  SystemRoleEnum,
  TaggingApiService,
  UserInfoModel
} from '@opal20/domain-api';

import { CCPM_PERMISSIONS } from '../../module-constants/ccpm/ccpm-permission.constant';
import { ContextMenuAction } from '../../models/context-menu-action.model';
import { ContextMenuItem } from '@opal20/common-components';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { DIGITAL_CONTENT_STATUS_MAPPING } from '../../models/digital-content-status.model';
import { DigitalContentContextMenuEmit } from '../../models/digital-content-context-menu-emit.model';
import { DigitalContentListPageService } from '../../services/digital-content-list-page.service';
import { DigitalContentSearchTermService } from '../../services/digital-content-search-term.service';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { Observable } from 'rxjs';

const moment = moment_;

@Component({
  selector: 'list-digital-content-page',
  templateUrl: './list-digital-content-page.component.html',
  styleUrls: ['./list-digital-content-page.component.scss']
})
export class ListDigitalContentPageComponent extends BasePageComponent {
  @Input() public filterStatus: DigitalContentStatus = DigitalContentStatus.All;

  @Input() public set search(v: string | undefined) {
    this._search = v;
    if (this.initiated) {
      this.loadData();
    }
  }
  public get search(): string | undefined {
    return this._search;
  }

  @Input() public set sortModeId(v: number) {
    this._sortModeId = v;
    this.loadData();
  }
  public get sortModeId(): number {
    return this._sortModeId;
  }

  @Input() public queryMode: DigitalContentQueryMode = DigitalContentQueryMode.AllByCurrentUser;
  @Input() public contextMenuItems: ContextMenuItem[] = [];
  @Input() public hiddenColumns: string[] = ['archivedByUser', 'archiveDate'];

  @Output() public selectedContextMenu: EventEmitter<DigitalContentContextMenuEmit> = new EventEmitter();
  @Output() public itemClicked: EventEmitter<unknown> = new EventEmitter();

  public gridData: GridDataResult;
  public queryModeApproved: DigitalContentQueryMode = DigitalContentQueryMode.Approved;
  public state: PageChangeEvent;
  public checkAll: boolean = false;
  public query: Observable<unknown>;
  public loading: boolean;
  public digitalContentStatusMapping = DIGITAL_CONTENT_STATUS_MAPPING;
  public selecteds = {};
  public formGroup: CustomFormGroup | undefined;
  public editedRowIndex: number | undefined;
  public focusRow: number = -1;
  @ViewChild('grid', { static: false })
  public grid: GridComponent;
  private docType: string[] = ['docx', 'xlsx', 'pptx'];
  private videoType: string[] = ['mp4', 'm4v', 'ogv'];
  private pictureType: string[] = ['jpeg', 'jpg', 'gif', 'png', 'svg'];
  private audioType: string[] = ['mp3', 'ogg'];
  private _search: string | undefined;
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  private _sortModeId: number = 1;
  private _sortField: DigitalContentSortField;
  private _sortDirection: SortDirection;
  private statusAcceptableArchive: DigitalContentStatus[] = [
    DigitalContentStatus.Draft,
    DigitalContentStatus.Rejected,
    DigitalContentStatus.Unpublished,
    DigitalContentStatus.Approved
  ];

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public digitalContentListPageService: DigitalContentListPageService,
    public searchTermService: DigitalContentSearchTermService,
    private contentApiService: ContentApiService,
    private taggingBackendService: TaggingApiService,
    private learningContentApiService: LearningContentApiService
  ) {
    super(moduleFacadeService);
    this.initSearchData();
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  @HostBinding('class.home-module')
  public getContentClass(): boolean {
    return true;
  }

  /************* Permission getters *************/

  public get isAdmin(): boolean {
    return this.currentUser.hasAdministratorRoles();
  }

  public get isContentCreator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.ContentCreator);
  }

  public get isCoursesContentCreator(): boolean {
    if (!this.currentUser) {
      return false;
    }
    return (
      this.currentUser.hasAdministratorRoles() ||
      this.currentUser.hasRole(SystemRoleEnum.ContentCreator, SystemRoleEnum.CourseContentCreator)
    );
  }

  public get isCoursesFacilitator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.CourseFacilitator);
  }

  public get isApprovingOfficer(): boolean {
    return (
      this.currentUser.hasAdministratorRoles() ||
      this.currentUser.hasRole(SystemRoleEnum.MOEHQContentApprovingOfficer, SystemRoleEnum.SchoolContentApprovingOfficer)
    );
  }

  public get isCreatorAndFacilitator(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.CourseFacilitator, SystemRoleEnum.CourseContentCreator, SystemRoleEnum.ContentCreator);
  }

  /************* End Permission getters *************/

  public isHidden(columnName: string): boolean {
    return this.hiddenColumns.indexOf(columnName) > -1;
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public getIconUrl(fileExtension: string): string {
    const contentType = this.getContentType(fileExtension);
    let iconFileName;
    switch (contentType) {
      case 'Document': {
        iconFileName = 'document';
        break;
      }
      case 'PDF': {
        iconFileName = 'pdf';
        break;
      }
      case 'Video': {
        iconFileName = 'video';
        break;
      }
      case 'Picture': {
        iconFileName = 'picture-file';
        break;
      }
      case 'Audio': {
        iconFileName = 'document';
        break;
      }
      case 'Scorm': {
        iconFileName = 'scorm';
        break;
      }
      default: {
        iconFileName = 'document';
        break;
      }
    }
    return 'assets/images/icons/sm/' + iconFileName + '.svg';
  }

  public getContentType(extension: string): string {
    if (this.docType.includes(extension)) {
      return 'Document';
    } else if (extension === 'pdf') {
      return 'PDF';
    } else if (this.videoType.includes(extension)) {
      return 'Video';
    } else if (this.audioType.includes(extension)) {
      return 'Audio';
    } else if (this.pictureType.includes(extension)) {
      return 'Picture';
    } else if (extension === 'zip') {
      return 'Scorm';
    } else {
      return 'Document';
    }
  }

  public onPageChange(state: PageChangeEvent): void {
    this.state = state;
    this.loadData();
  }

  public getContextMenuByDigitalContent(content: DigitalContent): ContextMenuItem[] {
    return this.contextMenuItems.filter(
      item =>
        // Check for menu context: Rename
        (item.id === ContextMenuAction.Rename &&
          content.status !== DigitalContentStatus.Archived &&
          this.hasPermission(CCPM_PERMISSIONS.RenameContent)) ||
        // Check for menu context: Duplicate
        (item.id === ContextMenuAction.Duplicate &&
          this.canShowDuplicateMenuBtn(content.status, content.archiveDate) &&
          this.hasPermission(CCPM_PERMISSIONS.DuplicateContent)) ||
        // Check for menu context: Unpublish
        (item.id === ContextMenuAction.Unpublish &&
          content.status === DigitalContentStatus.Published &&
          this.hasPermission(CCPM_PERMISSIONS.UnPublishContent)) ||
        // Check for menu context: Publish
        (item.id === ContextMenuAction.Publish &&
          content.status === DigitalContentStatus.Approved &&
          this.hasPermission(CCPM_PERMISSIONS.PublishContent)) ||
        // Check for menu context: Delete
        (item.id === ContextMenuAction.Delete &&
          (content.ownerId === this.currentUser.extId || this.isAdmin) &&
          content.status !== DigitalContentStatus.Archived &&
          this.hasPermission(CCPM_PERMISSIONS.DeleteContent)) ||
        // Check for menu context: Transfer owner ship
        (item.id === ContextMenuAction.TransferOwnership &&
          (content.ownerId === this.currentUser.extId || this.isAdmin) &&
          this.hasPermission(CCPM_PERMISSIONS.TransferOwnerShipContent)) ||
        // Check for menu context: Archive
        (item.id === ContextMenuAction.Archive &&
          (content.ownerId === this.currentUser.extId || this.isAdmin) &&
          this.canArchive(content.status) &&
          this.hasPermission(CCPM_PERMISSIONS.ArchiveContent))
    );
  }

  public canShowDuplicateMenuBtn(status: DigitalContentStatus, archiveDate: Date): boolean {
    const threeYearsAgo = DateUtils.addYear(new Date(), -3);
    return (
      status !== DigitalContentStatus.Archived ||
      (status === DigitalContentStatus.Archived && DateUtils.compareDate(new Date(archiveDate), threeYearsAgo) >= 0)
    );
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: DigitalContent, rowIndex: number): void {
    this.selectedContextMenu.emit(new DigitalContentContextMenuEmit(event, dataItem, rowIndex));
  }

  public loadData(): void {
    this.updateSearchTermService();
    this.digitalContentListPageService
      .loadGridDigitalContent(
        this.search,
        this.searchTermService.searchStatuses,
        this.state.skip,
        this.state.take,
        this.queryMode,
        this._sortField,
        this._sortDirection
      )
      .subscribe(data => {
        this.gridData = data;
      });
  }

  public onItemClick(event: CellClickEvent): void {
    if (event.dataItem === undefined || (event.columnIndex !== 0 && event.columnIndex !== 1)) {
      return;
    }
    this.itemClicked.emit(event.dataItem.id);
  }

  public onGridTitleInputKeydown(event: KeyboardEvent, item: IDigitalContent, formControl: AbstractControl): void {
    if (event.keyCode !== KeyCode.Enter) {
      return;
    }
    this.checkAndSaveInlineInput(item, formControl);
  }

  public checkAndSaveInlineInput(item: IDigitalContent, formControl: AbstractControl): void {
    if (formControl.valid) {
      if (item.title !== formControl.value) {
        this.contentApiService
          .renameDigitalContent({
            id: item.id,
            title: formControl.value
          })
          .then(() => {
            this.closeGridInlineEditor();
            this.showNotification(`${item.title} is successfully renamed`, NotificationType.Success);
            this.loadData();
          });
      }
    } else {
      FormBuilderService.showError(this.moduleFacadeService.globalTranslator, formControl as CustomFormControl);
    }
  }

  public closeGridInlineEditor(rowIndex: number = this.editedRowIndex): void {
    this.grid.closeRow(rowIndex);
    this.editedRowIndex = undefined;
    this.formGroup = undefined;
  }

  public editGridRow(rowIndex: number, item: IDigitalContent): void {
    this.closeGridInlineEditor();

    this.formGroup = new CustomFormGroup({
      title: new CustomFormControl(item.title, Validators.compose([Validators.required, Validators.maxLength(255)]))
    });

    this.editedRowIndex = rowIndex;

    this.grid.editRow(rowIndex, this.formGroup);
  }

  public duplicateContent(id: string, contentName: string): void {
    this.contentApiService.duplicateDigitalContent(id).then(_ => {
      this.taggingBackendService.cloneResource(_.id, id).toPromise();
      this.showNotification(`${contentName} is successfully duplicated`, NotificationType.Success);
      this.loadData();
    });
  }

  public deleteContent(id: string, contentName?: string): void {
    this.learningContentApiService.hasReferenceToResource(id, false).then(isDigitalContentInUsage => {
      if (!isDigitalContentInUsage) {
        this.modalService.showConfirmMessage(
          new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Are you sure you want to permanently delete this item?'),
          () => {
            this.contentApiService
              .deleteDigitalContent(id)
              .then(() => this.loadData())
              .then(() => {
                this.showNotification(`${contentName} is successfully deleted`, NotificationType.Success);
              });
          }
        );
      } else {
        this.showNotification(this.translate('The digital content is in usage can not be deleted!'), NotificationType.Warning);
      }
    });
  }

  public isExpired(expiredDate: Date | null | undefined): boolean {
    if (!expiredDate) {
      return false;
    }

    return moment().isSameOrAfter(expiredDate);
  }

  public onContextMenuChange(rowIndex: number): void {
    this.focusRow = rowIndex;
  }

  public getAllowDownloadDisplay(isAllow: boolean): string {
    return isAllow ? this.translate('Allow download') : '';
  }

  protected onInit(): void {
    this.loadData();
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return this.currentUser.permissionDic;
  }

  protected onChanges(changes: SimpleChanges): void {
    if (!changes.filterStatus || changes.filterStatus.previousValue === changes.filterStatus.currentValue) {
      return;
    }
    if (this.initiated) {
      this.loadData();
    }
  }

  private checkStatusChanged(): boolean {
    return (
      !(this.searchTermService.searchStatuses.length === 0 && this.filterStatus === DigitalContentStatus.All) &&
      this.searchTermService.searchStatuses[0] !== this.filterStatus
    );
  }

  private initSearchData(): void {
    this.state = this.searchTermService.state;
    if (this.searchTermService.searchStatuses) {
      this.filterStatus = this.searchTermService.searchStatuses[0];
    }
  }

  private updateSearchTermService(): void {
    this.searchTermService.state = this.state;
    if (this.searchTermService.searchText !== this.search) {
      this.searchTermService.searchText = this.search;
      this.searchTermService.state.skip = 0;
      this.state.skip = 0;
    }

    if (this.searchTermService.queryMode === DigitalContentQueryMode.PendingApproval) {
      this.searchTermService.searchStatuses = [DigitalContentStatus.PendingForApproval];
    } else if (!this.searchTermService.searchStatuses || this.checkStatusChanged()) {
      this.searchTermService.searchStatuses =
        this.filterStatus === undefined || this.filterStatus === DigitalContentStatus.All ? [] : [this.filterStatus];
      this.searchTermService.state.skip = 0;
      this.state.skip = 0;
    }

    const sortItemSelected = DIGITAL_CONTENT_SORT_ITEMS.find(x => x.id === this._sortModeId);
    this._sortField = sortItemSelected.sortMode.sortField;
    this._sortDirection = sortItemSelected.sortMode.sortDirection;
  }

  private canArchive(status: DigitalContentStatus): boolean {
    return status && this.statusAcceptableArchive.includes(status);
  }
}
