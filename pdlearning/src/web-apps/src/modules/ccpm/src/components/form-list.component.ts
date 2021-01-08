import { AbstractControl, FormGroup, Validators } from '@angular/forms';
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
import {
  CCPM_PERMISSIONS,
  ContextMenuAction,
  FORM_STATUS_COLOR_MAP,
  FormDetailMode,
  FormEditorPageService,
  FormSearchTermService
} from '@opal20/domain-components';
import { CellClickEvent, GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Component, Input, SimpleChanges, ViewChild } from '@angular/core';
import {
  FormModel,
  FormQueryModeEnum,
  FormStatus,
  FormSurveyType,
  FormType,
  LearningContentApiService,
  SystemRoleEnum,
  TaggingApiService,
  UserInfoModel
} from '@opal20/domain-api';
import { FormRepositoryPageFormListData, FormRepositoryPageService } from '../services/form-repository-page.service';

import { Align } from '@progress/kendo-angular-popup';
import { CCPMRoutePaths } from '../ccpm.config';
import { ContextMenuEvent } from '@progress/kendo-angular-menu';
import { ContextMenuItem } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { IFormEditorPageNavigationData } from '../ccpm-navigation-data';
import { TransferOwnershipDialogComponent } from './dialogs/transfer-ownership-dialog.component';

@Component({
  selector: 'form-list',
  templateUrl: './form-list.component.html'
})
export class FormListComponent extends BasePageComponent {
  public statusColorMap = FORM_STATUS_COLOR_MAP;
  public filterItems: string[] = ['All', FormStatus.Draft, FormStatus.Published, FormStatus.Unpublished];
  public statusAcceptableArchive: FormStatus[] = [FormStatus.Draft, FormStatus.Rejected, FormStatus.Unpublished, FormStatus.Approved];
  public formType: typeof FormType = FormType;
  public formStatus: typeof FormStatus = FormStatus;
  public skip: number;
  public pageSize: number;
  public currentStatusFilter: string = 'All';
  public gridView: GridDataResult;
  public currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  public focusRow: number = -1;
  public formPermissionKeys = CCPM_PERMISSIONS;

  @Input() public contextMenuItems: ContextMenuItem[] = [];
  @Input() public filterStatus: FormStatus;
  @Input() public filterBySurveyTypes: FormSurveyType[];
  @Input() public excludeBySurveyTypes: FormSurveyType[];
  @Input() public set searchTerm(value: string | undefined) {
    if (this.initiated) {
      if (value === undefined || value === null) {
        return;
      }
      this._searchTerm = value;
      this.skip = 0;
      this.searchTermService.state.skip = 0;
      this.loadItems();
    }
  }

  @Input() public queryMode: FormQueryModeEnum = FormQueryModeEnum.All;
  @Input() public hiddenColumns: string[] = ['archivedByUser', 'archiveDate'];

  // The  properties are used for Menu Context in each form item on the list.
  public formGroup: CustomFormGroup | undefined;
  public popupAlign: Align = { horizontal: 'center', vertical: 'top' };
  @ViewChild('grid', { static: false })
  private gridComponent: GridComponent;
  private editedRowIndex: number;
  private _searchTerm: string | undefined;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private formRepositoryPageService: FormRepositoryPageService,
    public searchTermService: FormSearchTermService,
    private learningContentApiService: LearningContentApiService,
    protected formEditorPageService: FormEditorPageService,
    private taggingApiService: TaggingApiService
  ) {
    super(moduleFacadeService);
    this.initSearchData();
    this.formRepositoryPageService.formListData$.pipe(this.untilDestroy()).subscribe(data => {
      this.updateGridView(data);
    });
  }

  /************* Permission getters *************/

  public get isSystemAdmin(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.SystemAdministrator);
  }

  public get isAdmin(): boolean {
    return this.currentUser.hasAdministratorRoles();
  }

  public get isContentCreator(): boolean {
    return (
      this.currentUser.hasAdministratorRoles() ||
      this.currentUser.hasRole(SystemRoleEnum.ContentCreator, SystemRoleEnum.CourseContentCreator)
    );
  }

  public get isCourseFaciliator(): boolean {
    return this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.CourseFacilitator);
  }

  public get isApprovingOfficer(): boolean {
    if (!this.currentUser) {
      return false;
    }
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

  public onFormTitleInputKeydown(event: KeyboardEvent, form: FormModel, control: AbstractControl): void {
    if (event.keyCode !== KeyCode.Enter) {
      return;
    }
    this.checkAndSaveInlineInput(form, control);
  }

  public checkAndSaveInlineInput(form: FormModel, control: AbstractControl): void {
    if (control.valid) {
      if (form.title !== control.value) {
        this.formRepositoryPageService.renameForm(form.id, control.value).subscribe(_ => {
          this.closeFormInlineEditor();
          this.showNotification(`${form.title} is successfully renamed`, NotificationType.Success);
        });
      }
    } else {
      FormBuilderService.showError(this.moduleFacadeService.globalTranslator, control as CustomFormControl);
    }
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.searchForms();
  }

  public searchForms(): void {
    if (this.queryMode === FormQueryModeEnum.PendingApproval) {
      this.filterStatus = FormStatus.PendingApproval;
    } else if (this.currentStatusFilter !== FormQueryModeEnum.All) {
      this.filterStatus = FormStatus[this.currentStatusFilter];
    }
    if (this.queryMode === FormQueryModeEnum.PostCourseTemplate) {
      this.filterStatus = FormStatus.All;
    }
    this.loadItems();
  }

  public onFormItemClick(event: CellClickEvent): void {
    if (event.dataItem === undefined || (event.columnIndex !== 0 && event.columnIndex !== 1)) {
      return;
    }
    this.navigateToFormDetail({
      formId: event.dataItem.id,
      formStatus: undefined,
      mode: this.queryMode === 'PendingApproval' ? FormDetailMode.ForApprover : FormDetailMode.View
    });
  }

  public closeFormInlineEditor(rowIndex: number = this.editedRowIndex): void {
    this.gridComponent.closeRow(rowIndex);
    this.editedRowIndex = undefined;
    this.formGroup = undefined;
  }

  public editGridRow(rowIndex: number, dataItem: FormModel): void {
    this.closeFormInlineEditor();

    this.formGroup = new FormGroup({
      title: new CustomFormControl(dataItem.title, Validators.compose([Validators.required, Validators.maxLength(1000)]))
    });

    this.editedRowIndex = rowIndex;

    this.gridComponent.editRow(rowIndex, this.formGroup);
  }

  public getContextMenuByForm(dataItem: FormModel): ContextMenuItem[] {
    return this.contextMenuItems.filter(
      item =>
        // Check for menu context: Rename
        (item.id === ContextMenuAction.Rename && !dataItem.isSurveyTemplate && this.hasPermission(this.formPermissionKeys.RenameForm)) ||
        // Check for menu context: Duplicate
        (item.id === ContextMenuAction.Duplicate &&
          (!dataItem.isSurveyTemplate || (dataItem.isSurveyTemplate && this.hasPermission(this.formPermissionKeys.DuplicateForm))) &&
          this.canShowDuplicateMenuBtn(dataItem.status, dataItem.archiveDate)) ||
        // Check for menu context: Unpublish
        (item.id === ContextMenuAction.Unpublish &&
          dataItem.status === FormStatus.Published &&
          dataItem.canUnpublishFormStandalone &&
          (!dataItem.isSurveyTemplate && this.hasPermission(this.formPermissionKeys.UnPublishForm))) ||
        // Check for menu context: Publish
        (item.id === ContextMenuAction.Publish &&
          dataItem.status === FormStatus.Approved &&
          (!dataItem.isSurveyTemplate && this.hasPermission(this.formPermissionKeys.PublishForm))) ||
        // Check for menu context: Delete
        (item.id === ContextMenuAction.Delete &&
          (!dataItem.isSurveyTemplate &&
            (this.hasPermission(this.formPermissionKeys.DeleteForm) && (dataItem.ownerId === this.currentUser.extId || this.isAdmin)))) ||
        // Check for menu context: Transfer owner ship
        (item.id === ContextMenuAction.TransferOwnership &&
          !dataItem.isSurveyTemplate &&
          ((dataItem.ownerId === this.currentUser.extId || this.isAdmin) &&
            this.hasPermission(this.formPermissionKeys.TransferOwnerShipForm))) ||
        // Check for menu context: Archive
        (item.id === ContextMenuAction.Archive &&
          !dataItem.isSurveyTemplate &&
          ((dataItem.ownerId === this.currentUser.extId || this.isAdmin) && this.hasPermission(this.formPermissionKeys.ArchiveForm)) &&
          this.canArchive(dataItem.status))
    );
  }

  public canShowDuplicateMenuBtn(status: FormStatus, archiveDate: Date): boolean {
    const threeYearsAgo = DateUtils.addYear(new Date(), -3);
    return (
      status !== FormStatus.Archived || (status === FormStatus.Archived && DateUtils.compareDate(new Date(archiveDate), threeYearsAgo) >= 0)
    );
  }

  public onItemOptionSelect(contextMenuEmit: ContextMenuEvent, form: FormModel, rowIndex: number): void {
    switch (contextMenuEmit.item.id) {
      case 'duplicate':
        this.formRepositoryPageService.cloneForm(form.id, `${this.translate('Copy of')} ${form.title}`).subscribe(formWithQuestions => {
          this.taggingApiService.cloneResource(formWithQuestions.form.id, form.id).toPromise();
          this.showNotification(`${form.title} is successfully duplicated`, NotificationType.Success);
          this.loadItems();
        });
        break;
      case 'rename':
        this.editGridRow(rowIndex, form);
        break;
      case 'unpublish':
        this.formRepositoryPageService.changeFormStatusForm(form.id, FormStatus.Unpublished).subscribe(() => {
          this.showNotification(`${form.title} is successfully unpublished`, NotificationType.Success);
          this.loadItems();
        });
        break;
      case 'publish':
        if (form.isStandalone) {
          this.modalService.showConfirmMessage(
            new TranslationMessage(
              this.moduleFacadeService.globalTranslator,
              'You can not edit once this form is published. Please check your content before publishing.'
            ),
            () => {
              this.formRepositoryPageService.changeFormStatusForm(form.id, FormStatus.Published).subscribe(() => {
                this.showNotification(`${form.title} is successfully published`, NotificationType.Success);
                this.loadItems();
              });
            }
          );
        } else {
          this.formRepositoryPageService.changeFormStatusForm(form.id, FormStatus.Published).subscribe(() => {
            this.showNotification(`${form.title} is successfully published`, NotificationType.Success);
            this.loadItems();
          });
        }
        break;
      case 'delete':
        this.learningContentApiService.hasReferenceToResource(form.id, true).then(isFormInUsage => {
          if (!isFormInUsage) {
            const confirmMessage = form.canUnpublishFormStandalone
              ? 'Are you sure you want to permanently delete this item?'
              : 'This form is being used by learner. Are you sure you want to delete this form?';
            this.modalService.showConfirmMessage(new TranslationMessage(this.moduleFacadeService.globalTranslator, confirmMessage), () => {
              this.formRepositoryPageService.deleteForm(form.id).subscribe();
              this.showNotification(`${form.title} is successfully deleted`, NotificationType.Success);
            });
          } else {
            this.showNotification(this.translate('The form is in usage can not be deleted!'), 'warning');
          }
        });
        break;
      case 'transferOwnership':
        const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: TransferOwnershipDialogComponent });

        dialogRef.result.toPromise().then((newOwnerId: string) => {
          if (typeof newOwnerId === 'string') {
            this.formRepositoryPageService.transferOwnership(form.id, newOwnerId).subscribe(() => {
              this.showNotification('Ownership transferred successfully', NotificationType.Success);
              this.loadItems();
            });
          }
        });
        break;
      case 'archive':
        this.formRepositoryPageService.archiveForm(form.id, this.currentUser.extId).subscribe(() => {
          this.showNotification(`${form.title} is successfully archived`, NotificationType.Success);
          this.loadItems();
        });
        break;
      default:
        break;
    }
  }

  public getFormTypeTitle(form: FormModel): string {
    if (form.type !== FormType.Survey) {
      return form.type;
    }
    switch (form.surveyType) {
      case FormSurveyType.Standalone:
        return this.translate('Standalone');
      case FormSurveyType.PreCourse:
        return this.translate('Pre Course');
      case FormSurveyType.PostCourse:
        return this.translate('Post Course');
      case FormSurveyType.FollowUpPostCourse:
        return this.translate('Follow Up Post Course');
      case FormSurveyType.DuringCourse:
        return this.translate('During Course');
      default: {
        return this.translate('Survey');
      }
    }
  }

  public onContextMenuChange(rowIndex: number): void {
    this.focusRow = rowIndex;
  }

  public loadItems(): void {
    this.updateSearchTermService();
    this.formRepositoryPageService
      .loadFormListData(
        this.searchTermService.state.skip,
        this.pageSize,
        this.searchTermService.searchText,
        this.searchTermService.searchStatuses,
        false,
        null,
        this.filterBySurveyTypes,
        this.searchTermService.isSurveyTemplate,
        this.excludeBySurveyTypes
      )
      .subscribe();
  }

  protected onInit(): void {
    this.loadItems();
  }

  protected onChanges(changes: SimpleChanges): void {
    if (this.initiated) {
      if (!changes.filterStatus || changes.filterStatus.previousValue === changes.filterStatus.currentValue) {
        return;
      }
      this.loadItems();
    }
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return this.currentUser.permissionDic;
  }

  private updateGridView(data: FormRepositoryPageFormListData): void {
    this.gridView = {
      data: data.formList,
      total: data.totalCount
    };
  }

  private navigateToFormDetail(data: IFormEditorPageNavigationData): void {
    this.navigateTo(CCPMRoutePaths.FormDetail, data);
  }

  private updateSearchTermService(): void {
    this.searchTermService.state.take = this.pageSize;
    this.searchTermService.state.skip = this.skip;
    if (this.searchTermService.searchText !== this._searchTerm) {
      this.searchTermService.searchText = this._searchTerm;
      this.searchTermService.state.skip = 0;
      this.skip = 0;
    }
    if (!this.searchTermService.searchStatuses || this.checkStatusChanged()) {
      this.searchTermService.searchStatuses =
        this.filterStatus === undefined || this.filterStatus === FormStatus.All ? [] : [this.filterStatus];
      this.searchTermService.state.skip = 0;
      this.skip = 0;
    }
  }

  private checkStatusChanged(): boolean {
    return (
      !(this.searchTermService.searchStatuses.length === 0 && this.filterStatus === this.formStatus.All) &&
      this.searchTermService.searchStatuses[0] !== this.filterStatus
    );
  }

  private initSearchData(): void {
    this.pageSize = this.searchTermService.state.take;
    this.skip = this.searchTermService.state.skip;
    if (this.searchTermService.searchText && this._searchTerm === undefined) {
      this._searchTerm = this.searchTermService.searchText;
    }
    if (this.searchTermService.searchStatuses) {
      this.filterStatus = this.searchTermService.searchStatuses[0];
    }
  }

  private canArchive(status: FormStatus): boolean {
    return status && this.statusAcceptableArchive.includes(status);
  }
}
