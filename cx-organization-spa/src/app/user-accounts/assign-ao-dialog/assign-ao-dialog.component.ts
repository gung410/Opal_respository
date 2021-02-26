import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation
} from '@angular/core';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService,
  CxObjectUtil
} from '@conexus/cx-angular-common';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { SurveyUtils } from 'app-utilities/survey-utils';
import { organizationUnitLevelsConst } from 'app/department-hierarchical/models/department-level.model';
import { Department } from 'app/department-hierarchical/models/department.model';
import { BasePresentationComponent } from 'app/shared/components/component.abstract';
import { PeoplePickerEventModel } from 'app/shared/components/people-picker/people-picker.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { isEmpty, without } from 'lodash';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserType } from 'app-models/user-type.model';
import { ApprovalGroupTypeEnum } from '../constants/approval-group.enum';
import {
  ApprovalGroup,
  ApprovalGroupQueryModel
} from '../models/approval-group.model';
import { UserManagement } from '../models/user-management.model';
import { ApprovalDataService } from '../services/approval-data.service';

@Component({
  selector: 'assign-ao-dialog',
  templateUrl: './assign-ao-dialog.component.html',
  styleUrls: ['./assign-ao-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AssignAODialogComponent
  extends BasePresentationComponent
  implements OnInit {
  get primarySelectedGroup(): ApprovalGroup {
    return this._primarySelectedGroup;
  }
  set primarySelectedGroup(primarySelectedGroup: ApprovalGroup) {
    this._primarySelectedGroup = primarySelectedGroup;
  }

  get alternateSelectedGroup(): ApprovalGroup {
    return this._alternateSelectedGroup;
  }
  set alternateSelectedGroup(alternateSelectedGroup: ApprovalGroup) {
    this._alternateSelectedGroup = alternateSelectedGroup;
  }

  get hasLearnerRole(): boolean {
    const systemRoles: UserType[] = [];
    this.itemsSelected.map((user) =>
      user.systemRoles.map((role) => {
        systemRoles.push(role);
      })
    );

    return UserManagement.hasLearnerRole(systemRoles);
  }
  getPropertyValue: any = CxObjectUtil.getPropertyValue;
  @Input() department: Department;
  @Input() itemsSelected: UserManagement[];
  @Output() done: EventEmitter<any> = new EventEmitter();
  @Output() cancel: EventEmitter<any> = new EventEmitter();
  allPrimaryApprovalGroups: ApprovalGroup[] = [];
  allAlternateApprovalGroups: ApprovalGroup[] = [];
  allVisiblePrimaryApprovalGroups: ApprovalGroup[] = [];
  allVisibleAlternateApprovalGroups: ApprovalGroup[] = [];
  numberOfItemsBeforeFetchingMoreUser: number = 10;
  stopLoadingPrimaryGroups: boolean;
  stopLoadingAlternateGroups: boolean;

  loading: boolean;
  private selectedUserIds: number[] = [];
  private _primarySelectedGroup: ApprovalGroup;
  private _alternateSelectedGroup: ApprovalGroup;
  constructor(
    private translateAdapterService: TranslateAdapterService,
    private toastrService: ToastrAdapterService,
    private approvalDataService: ApprovalDataService,
    private globalLoader: CxGlobalLoaderService,
    protected changeDetectorRef: ChangeDetectorRef,
    public ngbModal: NgbModal
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    if (!this.validateInput()) {
      this.onCancel();
    } else {
      this.selectedUserIds = this.itemsSelected.map((item) => item.identity.id);
      this.globalLoader.showLoader();
      Promise.all([
        this.loadSelectedGroups(),
        this.loadMorePrimaryGroups(new PeoplePickerEventModel()),
        this.loadMoreAlternateGroups(new PeoplePickerEventModel())
      ]).finally(() => {
        this.globalLoader.hideLoader();
        this.changeDetectorRef.detectChanges();
      });
    }
  }

  onDone(): void {
    if (!(!this.primarySelectedGroup && !this.alternateSelectedGroup)) {
      this.proceedToApproveUser();

      return;
    }

    if (this.isDuplicatePrimaryAndAlternateGroup()) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'User_Account_Page.User_Edit_Dialog.Approval_Info_Tab.Duplicate_Primary_Alternate_Officer'
        ),
        'Alert'
      );

      return;
    }

    this.showNonAssignAOConfirmationDialog(
      () => {
        this.proceedToApproveUser();
      },
      'Confirm',
      'You have not assigned Approving Officer/Alternate Approving Officer for this user. Proceed?'
    );
  }

  onCancel(): void {
    this.cancel.emit();
  }

  getAvatar(email: string): string {
    return SurveyUtils.getAvatarFromEmail(email);
  }

  loadSelectedGroups(): Promise<void> {
    if (this.itemsSelected.length !== 1 || !this.itemsSelected[0].groups) {
      return Promise.resolve();
    }
    const selectedPrimaryGroup = this.itemsSelected[0].groups.find(
      (g) => g.type === ApprovalGroupTypeEnum.PrimaryApprovalGroup
    );

    const selectedAlternateGroup = this.itemsSelected[0].groups.find(
      (g) => g.type === ApprovalGroupTypeEnum.AlternativeApprovalGroup
    );

    const selectedPrimaryGroupId: number =
      selectedPrimaryGroup && selectedPrimaryGroup.identity.id;

    const selectedAlternateGroupId: number =
      selectedAlternateGroup && selectedAlternateGroup.identity.id;

    const approvalGroupIds = without(
      [selectedPrimaryGroupId, selectedAlternateGroupId],
      undefined
    );

    if (isEmpty(approvalGroupIds)) {
      return Promise.resolve();
    }

    const queryParams = new ApprovalGroupQueryModel({
      statusEnums: [StatusTypeEnum.Active.code],
      searchInSameDepartment: true,
      isCrossOrganizationalUnit: true,
      approvalGroupIds,
      assigneeDepartmentId:
        this.itemsSelected && this.itemsSelected[0]
          ? this.itemsSelected[0].departmentId
          : null
    });

    return this.approvalDataService
      .getApprovalGroups(queryParams)
      .toPromise()
      .then(
        (approvalGroups: ApprovalGroup[]) => {
          if (!approvalGroups || approvalGroups.length === 0) {
            return;
          }
          this.primarySelectedGroup = approvalGroups.find(
            (g: any) => g.identity.id === selectedPrimaryGroupId
          );
          this.alternateSelectedGroup = approvalGroups.find(
            (g: any) => g.identity.id === selectedAlternateGroupId
          );
        },
        (error: HttpErrorResponse) => {
          this.toastrService.error('Loading Approver info failed');
        }
      );
  }

  loadAllActiveApprovalGroups(
    type: ApprovalGroupTypeEnum,
    pageIndex: number,
    searchKey: string,
    handleResults: (approvalGroups: ApprovalGroup[]) => void
  ): Promise<void> {
    const queryParams = new ApprovalGroupQueryModel({
      assigneeDepartmentId: this.itemsSelected[0].departmentId,
      searchInSameDepartment: true,
      isCrossOrganizationalUnit: true,
      statusEnums: [StatusTypeEnum.Active.code],
      pageIndex,
      searchKey,
      groupTypes: [type],
      pageSize: this.numberOfItemsBeforeFetchingMoreUser
    });

    return this.approvalDataService
      .getApprovalGroups(queryParams)
      .toPromise()
      .then(
        (approvalGroups: ApprovalGroup[]) => {
          handleResults(approvalGroups);
        },
        (error: HttpErrorResponse) => {
          this.toastrService.error('Loading Approver info failed');
        }
      );
  }

  loadMorePrimaryGroups(event: PeoplePickerEventModel): Promise<void> {
    const handleResults = (approvalGroups: ApprovalGroup[]): void => {
      this.stopLoadingPrimaryGroups = false;
      if (!approvalGroups || approvalGroups.length === 0) {
        if (event.pageIndex === 1) {
          this.allPrimaryApprovalGroups = [];
          this.allVisiblePrimaryApprovalGroups = [];
        }
        this.stopLoadingPrimaryGroups = true;
        this.changeDetectorRef.detectChanges();

        return;
      }
      this.allPrimaryApprovalGroups =
        event.pageIndex !== 1
          ? [...this.allPrimaryApprovalGroups, ...approvalGroups]
          : [...approvalGroups];
      this.updateAvailablePrimaryGroups();
      this.changeDetectorRef.detectChanges();
    };

    return this.loadAllActiveApprovalGroups(
      ApprovalGroupTypeEnum.PrimaryApprovalGroup,
      event.pageIndex,
      event.key,
      handleResults
    );
  }

  loadMoreAlternateGroups(event: PeoplePickerEventModel): Promise<void> {
    const handleResults = (approvalGroups: ApprovalGroup[]): void => {
      this.stopLoadingAlternateGroups = false;
      if (!approvalGroups || approvalGroups.length === 0) {
        if (event.pageIndex === 1) {
          this.allAlternateApprovalGroups = [];
          this.allVisibleAlternateApprovalGroups = [];
        }
        this.stopLoadingAlternateGroups = true;
        this.changeDetectorRef.detectChanges();

        return;
      }
      this.allAlternateApprovalGroups =
        event.pageIndex !== 1
          ? [...this.allAlternateApprovalGroups, ...approvalGroups]
          : [...approvalGroups];
      this.updateAvailableAlternateGroups();
      this.changeDetectorRef.detectChanges();
    };

    return this.loadAllActiveApprovalGroups(
      ApprovalGroupTypeEnum.AlternativeApprovalGroup,
      event.pageIndex,
      event.key,
      handleResults
    );
  }

  updateApprovalGroups(
    approvalGroups: ApprovalGroup[],
    selectedGroups: ApprovalGroup[]
  ): ApprovalGroup[] {
    let approvalGroupResults: ApprovalGroup[] = [];
    if (selectedGroups) {
      approvalGroupResults = [
        ...approvalGroups.filter((approvalGroup: ApprovalGroup) => {
          const approverId = approvalGroup.approverId;

          return (
            !this.selectedUserIds.includes(approverId) &&
            !selectedGroups.some(
              (selectedGroup: ApprovalGroup) =>
                selectedGroup && selectedGroup.approverId === approverId
            )
          );
        })
      ];
    }

    return approvalGroupResults;
  }

  updateAvailablePrimaryGroups(): void {
    this.allVisiblePrimaryApprovalGroups = this.updateApprovalGroups(
      this.allPrimaryApprovalGroups,
      [this.alternateSelectedGroup, this.primarySelectedGroup]
    );
  }

  updateAvailableAlternateGroups(): void {
    this.allVisibleAlternateApprovalGroups = this.updateApprovalGroups(
      this.allAlternateApprovalGroups,
      [this.alternateSelectedGroup, this.primarySelectedGroup]
    );
  }

  private proceedToApproveUser(): void {
    this.done.emit({
      itemsSelected: this.itemsSelected,
      primaryApprovalGroup: this.primarySelectedGroup,
      alternateApprovalGroup: this.alternateSelectedGroup
    });
  }

  private isDuplicatePrimaryAndAlternateGroup(): boolean {
    if (this.primarySelectedGroup && !this.alternateSelectedGroup) {
      return this.checkDuplicatePrimaryAndAlternateGroup(
        this.primarySelectedGroup,
        this.allAlternateApprovalGroups
      );
    } else if (!this.primarySelectedGroup && this.alternateSelectedGroup) {
      return this.checkDuplicatePrimaryAndAlternateGroup(
        this.alternateSelectedGroup,
        this.allPrimaryApprovalGroups
      );
    }

    return false;
  }

  private checkDuplicatePrimaryAndAlternateGroup(
    selectedApprovalGroup: ApprovalGroup,
    allApprovalGroup: ApprovalGroup[]
  ): boolean {
    const existApprovalGroups = this.itemsSelected.map((user: any) => {
      const userApprovalGroup =
        user.groups &&
        user.groups.find(
          (group: any) =>
            group.type !== selectedApprovalGroup.type &&
            group.type !== 'Default'
        );
      const approvalGroup = userApprovalGroup
        ? allApprovalGroup.find(
            (group: any) => group.identity.id === userApprovalGroup.identity.id
          )
        : null;

      return approvalGroup ? approvalGroup.approverId : null;
    });
    const existSelectedGroup =
      existApprovalGroups &&
      existApprovalGroups.find(
        (approverId: number) => approverId === selectedApprovalGroup.approverId
      );
    if (existSelectedGroup) {
      return true;
    }

    return false;
  }

  private isSchoolDepartment(): boolean {
    if (
      !this.department ||
      !this.department.organizationalUnitTypes ||
      !this.department.organizationalUnitTypes.length
    ) {
      return false;
    }

    return (
      this.department.organizationalUnitTypes.findIndex(
        (type) => type.identity.extId === organizationUnitLevelsConst.School
      ) > findIndexCommon.notFound
    );
  }

  private validateInput(): boolean {
    if (!this.itemsSelected || !this.itemsSelected.length) {
      return false;
    }
    const departmentId = this.itemsSelected[0].departmentId;
    const isMultipleDepartments =
      this.itemsSelected.findIndex(
        (item) => item.departmentId !== departmentId
      ) > findIndexCommon.notFound;
    if (isMultipleDepartments) {
      this.toastrService.warning(
        'You cannot set Approving Officer for those who have different Organisations.'
      );

      return false;
    }

    return true;
  }

  private showNonAssignAOConfirmationDialog(
    onConfirmed: () => void,
    headerName: string,
    content: string
  ): void {
    const cxConfirmationDialogModalRef = this.ngbModal.open(
      CxConfirmationDialogComponent,
      {
        size: 'sm',
        centered: true
      }
    );

    const cxConfirmationDialogModal = cxConfirmationDialogModalRef.componentInstance as CxConfirmationDialogComponent;
    cxConfirmationDialogModal.showConfirmButton = true;
    cxConfirmationDialogModal.showCloseButton = true;
    cxConfirmationDialogModal.confirmButtonText = 'Confirm';
    cxConfirmationDialogModal.cancelButtonText = 'Cancel';
    cxConfirmationDialogModal.header = headerName;
    cxConfirmationDialogModal.content = content;

    cxConfirmationDialogModal.confirm.subscribe(() => {
      onConfirmed();
      cxConfirmationDialogModalRef.close();
    });
    cxConfirmationDialogModal.cancel.subscribe(() => {
      cxConfirmationDialogModalRef.close();
    });
  }
}
