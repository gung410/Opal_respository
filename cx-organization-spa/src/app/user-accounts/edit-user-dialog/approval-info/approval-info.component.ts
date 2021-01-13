import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';
import {
  CxGlobalLoaderService,
  CxObjectUtil
} from '@conexus/cx-angular-common';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { BasePresentationComponent } from 'app/shared/components/component.abstract';
import { PeoplePickerEventModel } from 'app/shared/components/people-picker/people-picker.model';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { ApprovalGroupTypeEnum } from 'app/user-accounts/constants/approval-group.enum';
import {
  ApprovalGroup,
  ApprovalGroupQueryModel
} from 'app/user-accounts/models/approval-group.model';
import {
  PagingResponseModel,
  UserManagement,
  UserManagementQueryModel
} from 'app/user-accounts/models/user-management.model';
import { ApprovalDataService } from 'app/user-accounts/services/approval-data.service';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';

import { DepartmentType } from 'app-models/department-type.model';
import { UserType } from 'app-models/user-type.model';
import { organizationUnitLevelsConst } from 'app/department-hierarchical/models/department-level.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { isEmpty, without } from 'lodash';
import {
  ApprovalInfoTabModel,
  MemberApprovalGroupModel
} from '../edit-user-dialog.model';
@Component({
  selector: 'approval-info',
  templateUrl: './approval-info.component.html',
  styleUrls: ['./approval-info.component.scss']
})
export class ApprovalInfoComponent
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

  get selectedMembersOfPrimaryGroup(): UserManagement[] {
    return this._selectedMembersOfPrimaryGroup;
  }
  set selectedMembersOfPrimaryGroup(users: UserManagement[]) {
    if (!users) {
      return;
    }
    this._selectedMembersOfPrimaryGroup = users;
    this.approvalData.memberOfPrimaryApprovalGroup.currentMemberIds = this._selectedMembersOfPrimaryGroup.map(
      (mem: UserManagement) => mem.identity.id
    );
  }

  get selectedMembersOfAlternateGroup(): UserManagement[] {
    return this._selectedMembersOfAlternateGroup;
  }
  set selectedMembersOfAlternateGroup(users: UserManagement[]) {
    this._selectedMembersOfAlternateGroup = users;
    this.approvalData.memberOfAlternateApprovalGroup.currentMemberIds = this._selectedMembersOfAlternateGroup.map(
      (mem: UserManagement) => mem.identity.id
    );
  }
  getPropertyValue: any = CxObjectUtil.getPropertyValue;
  /**
   * The editing user this could be null if you're trying to process multi-users.
   */
  @Input() user?: UserManagement;
  /**
   * All of system roles from selected user(s)
   */
  @Input() systemRoles?: UserType[];
  /**
   * The identifier of the department which the user belonging to.
   */
  @Input() userDepartmentId?: number;
  /**
   * Whether the primary approving officer is a required field or not.
   */
  @Input() requirePrimaryApprovingOfficer: boolean;
  /**
   * The department types of the user department.
   */
  @Input() departmentTypes: DepartmentType[];
  @Input() approvalData: ApprovalInfoTabModel;
  @Output()
  approvalDataChange: EventEmitter<ApprovalInfoTabModel> = new EventEmitter();
  @Input() disabled: boolean = false;

  numberOfItemsBeforeFetchingMoreUser: number = 10;
  isReportingOfficer: boolean;
  isSchoolDepartment: boolean;

  allPrimaryApprovalGroups: ApprovalGroup[];
  allAlternateApprovalGroups: ApprovalGroup[];
  allVisiblePrimaryApprovalGroups: ApprovalGroup[];
  allVisibleAlternateApprovalGroups: ApprovalGroup[];

  visibleMembersForPrimaryGroup: UserManagement[];
  visibleMembersForAlternateGroup: UserManagement[];

  isLoadAllMemberForPrimaryGroup: boolean;
  isLoadAllMemberForAlternateGroup: boolean;
  stopLoadingPrimaryGroups: boolean;
  stopLoadingAlternateGroups: boolean;

  loading: boolean;

  private _primarySelectedGroup: ApprovalGroup;
  private _alternateSelectedGroup: ApprovalGroup;
  private userPageSize: number = 20;
  private allMembersForPrimaryGroup: UserManagement[] = [];
  private allMembersForAlternateGroup: UserManagement[] = [];
  private _selectedMembersOfPrimaryGroup: UserManagement[] = [];
  private _selectedMembersOfAlternateGroup: UserManagement[] = [];
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private approvalDataService: ApprovalDataService,
    private userAccountService: UserAccountsDataService,
    private toastrService: ToastrAdapterService,
    private globalLoader: CxGlobalLoaderService
  ) {
    super(changeDetectorRef);
  }

  approvalMemberComparator: (
    userA: UserManagement,
    userB: UserManagement
  ) => boolean = (userA: UserManagement, userB: UserManagement) =>
    userA.identity.id === userB.identity.id;

  approvalGroupComparator: (
    groupA: ApprovalGroup,
    groupB: ApprovalGroup
  ) => boolean = (groupA: UserManagement, groupB: UserManagement) =>
    groupA.identity.id === groupB.identity.id;

  ngOnInit(): void {
    this.checkSchoolDepartment();
    this.isReportingOfficer =
      this.user &&
      this.user.systemRoles &&
      this.user.systemRoles.findIndex(
        (role: any) => role.identity.extId === UserRoleEnum.ReportingOfficer
      ) > findIndexCommon.notFound;
    if (this.isReportingOfficer) {
      this.approvalData.memberOfPrimaryApprovalGroup = new MemberApprovalGroupModel();
      this.approvalData.memberOfAlternateApprovalGroup = new MemberApprovalGroupModel();
    }
    this.globalLoader.showLoader();
    const initData = this.isReportingOfficer
      ? [this.initApprovalGroup(), this.initApprovalMembers()]
      : [this.initApprovalGroup()];
    Promise.all(initData).finally(() => {
      this.globalLoader.hideLoader();
      this.changeDetectorRef.detectChanges();
    });
  }

  initApprovalGroup(): Promise<void[]> {
    return Promise.all([
      this.loadSelectedGroups(),
      this.loadMorePrimaryGroups(new PeoplePickerEventModel()),
      this.loadMoreAlternateGroups(new PeoplePickerEventModel())
    ]);
  }

  initApprovalMembers(): Promise<void[]> {
    return Promise.all([
      this.loadMoreMembersForPrimaryGroup(new PeoplePickerEventModel()),
      this.loadMoreMembersForAlternateGroup(new PeoplePickerEventModel()),
      this.loadEmployeesForApprovalData()
    ]);
  }

  // BEGIN LOAD APPROVAL GROUP MEMBERS
  loadMoreMembersForPrimaryGroup(event: PeoplePickerEventModel): Promise<void> {
    if (event.pageIndex === 1) {
      this.isLoadAllMemberForPrimaryGroup = false;
    }
    const queryParams = new UserManagementQueryModel({
      parentDepartmentId: [this.getUserDepartmentId()],
      pageIndex: event.pageIndex,
      pageSize: this.userPageSize,
      searchKey: event.key,
      getRoles: false,
      getDeapartments: true,
      getUserGroups: false,
      isCrossOrganizationalUnit: false
    });

    return this.userAccountService
      .getUsers(queryParams)
      .toPromise()
      .then(
        (response: PagingResponseModel<UserManagement>) => {
          if (response.items.length < this.userPageSize) {
            this.isLoadAllMemberForPrimaryGroup = true;
          }
          this.allMembersForPrimaryGroup =
            event.pageIndex !== 1
              ? [...this.allMembersForPrimaryGroup, ...response.items]
              : response.items;
          this.updateVisibleMembersForPrimaryGroup();
        },
        (error: any) => {
          this.isLoadAllMemberForPrimaryGroup = true;
          this.toastrService.error(
            'Loading more users failed',
            'Error message'
          );
        }
      );
  }

  loadMoreMembersForAlternateGroup(
    event: PeoplePickerEventModel
  ): Promise<void> {
    if (event.pageIndex === 1) {
      this.isLoadAllMemberForAlternateGroup = false;
    }
    const queryParams = new UserManagementQueryModel({
      parentDepartmentId: [this.getUserDepartmentId()],
      pageIndex: event.pageIndex,
      pageSize: this.userPageSize,
      searchKey: event.key,
      getRoles: false,
      getDeapartments: true,
      getUserGroups: false,
      isCrossOrganizationalUnit: false
    });

    if (!this.isSchoolDepartment) {
      queryParams.filterOnSubDepartment = true;
    }

    return this.userAccountService
      .getUsers(queryParams)
      .toPromise()
      .then(
        (response: PagingResponseModel<UserManagement>) => {
          if (response.items.length < this.userPageSize) {
            this.isLoadAllMemberForAlternateGroup = true;
          }
          this.allMembersForAlternateGroup =
            event.pageIndex !== 1
              ? [...this.allMembersForAlternateGroup, ...response.items]
              : [...response.items];
          this.updateVisibleMembersForAlternateGroup();
        },
        (error: any) => {
          this.isLoadAllMemberForPrimaryGroup = true;
          this.toastrService.error(
            'Loading more users failed',
            'Error message'
          );
        }
      );
  }

  get hasLearnerRole(): boolean {
    const selectedUserSystemRole: UserType[] = this.user
      ? this.user.systemRoles
      : [];

    return this.systemRoles
      ? UserManagement.hasLearnerRole(this.systemRoles)
      : UserManagement.hasLearnerRole(selectedUserSystemRole);
  }

  updateVisibleMembersForAlternateGroup(): void {
    const selectedMemberOfPrimaryGroupIds = this.selectedMembersOfPrimaryGroup.map(
      (member: UserManagement) => member.identity.id
    );
    this.visibleMembersForAlternateGroup = [
      ...this.allMembersForAlternateGroup.filter((member: UserManagement) => {
        const memberId = member.identity.id;
        return (
          memberId !== this.user.identity.id &&
          !selectedMemberOfPrimaryGroupIds.includes(memberId)
        );
      })
    ];
    this.changeDetectorRef.detectChanges();
  }

  updateVisibleMembersForPrimaryGroup(): void {
    const selectedMemberOfAlternateGroupIds = this.selectedMembersOfAlternateGroup.map(
      (member: UserManagement) => member.identity.id
    );
    this.visibleMembersForPrimaryGroup = [
      ...this.allMembersForPrimaryGroup.filter((member: UserManagement) => {
        const memberId = member.identity.id;
        return (
          memberId !== this.user.identity.id &&
          !selectedMemberOfAlternateGroupIds.includes(memberId)
        );
      })
    ];
    this.changeDetectorRef.detectChanges();
  }

  // END LOAD APPROVAL GROUP MEMBERS

  // BEGIN LOAD APPROVAL GROUPS

  loadSelectedGroups(): Promise<void> {
    const selectedPrimaryGroup: ApprovalGroup =
      this.user &&
      this.user.groups &&
      this.user.groups.find(
        (g: any) => g.type === ApprovalGroupTypeEnum.PrimaryApprovalGroup
      );
    const selectedPrimaryGroupId: number =
      selectedPrimaryGroup && selectedPrimaryGroup.identity.id;
    const selectedAlternateGroup: ApprovalGroup =
      this.user &&
      this.user.groups &&
      this.user.groups.find(
        (g: any) => g.type === ApprovalGroupTypeEnum.AlternativeApprovalGroup
      );
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
      approvalGroupIds
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
          this.changeDetectorRef.detectChanges();
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
    handleResults: any
  ): Promise<void> {
    const queryParams = new ApprovalGroupQueryModel({
      assigneeDepartmentId: this.getUserDepartmentId(),
      isCrossOrganizationalUnit: false,
      searchInSameDepartment: true,
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
        this.stopLoadingPrimaryGroups = true;

        if (event.pageIndex === 1) {
          this.allPrimaryApprovalGroups = [];
          this.allVisiblePrimaryApprovalGroups = [];
        }

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
        this.stopLoadingAlternateGroups = true;

        if (event.pageIndex === 1) {
          this.allAlternateApprovalGroups = [];
          this.allVisibleAlternateApprovalGroups = [];
        }
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
          return this.filterCurrentApprovalGroups(approvalGroup);
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

  onSelectPrimaryApprovalGroup(selectedGroup: ApprovalGroup): void {
    this.approvalData.primaryApprovalGroup = selectedGroup;
    this.primarySelectedGroup = selectedGroup;
  }

  onSelectAlternateApprovalGroup(selectedGroup: ApprovalGroup): void {
    this.approvalData.alternateApprovalGroup = selectedGroup;
    this.alternateSelectedGroup = selectedGroup;
  }
  // END LOAD APPROVAL GROUP MEMBERS

  private loadEmployeesForApprovalData(): Promise<void> {
    return this.approvalDataService
      .getApprovalGroupsUserApprovesFor(this.user.identity.id)
      .toPromise()
      .then(
        (approvalGroups: ApprovalGroup[]) => {
          if (!approvalGroups || approvalGroups.length === 0) {
            return;
          }
          const primaryGroup = approvalGroups.find(
            (group: ApprovalGroup) =>
              group.type === ApprovalGroupTypeEnum.PrimaryApprovalGroup
          );
          const alternateGroup = approvalGroups.find(
            (group: ApprovalGroup) =>
              group.type === ApprovalGroupTypeEnum.AlternativeApprovalGroup
          );
          if (primaryGroup) {
            this.approvalData.memberOfPrimaryApprovalGroup.approvalGroup = primaryGroup;
            this.approvalDataService
              .getApprovalGroupMembers(primaryGroup.identity.id)
              .toPromise()
              .then(
                (employees: UserManagement[]) => {
                  if (!employees || employees.length === 0) {
                    return;
                  }
                  this.approvalData.memberOfPrimaryApprovalGroup.previousMemberIds = employees.map(
                    (emp: UserManagement) => emp.identity.id
                  );
                  this.selectedMembersOfPrimaryGroup = employees;
                },
                (error: HttpErrorResponse) => {
                  this.toastrService.error(
                    'Loading approval employees info failed'
                  );
                }
              );
          }
          if (alternateGroup) {
            this.approvalData.memberOfAlternateApprovalGroup.approvalGroup = alternateGroup;
            this.approvalDataService
              .getApprovalGroupMembers(alternateGroup.identity.id)
              .toPromise()
              .then(
                (employees: UserManagement[]) => {
                  if (!employees || employees.length === 0) {
                    return;
                  }
                  this.approvalData.memberOfAlternateApprovalGroup.previousMemberIds = employees.map(
                    (emp: UserManagement) => emp.identity.id
                  );
                  this.selectedMembersOfAlternateGroup = employees;
                },
                (error: HttpErrorResponse) => {
                  this.toastrService.error(
                    'Loading approval employees info failed'
                  );
                }
              );
          }
        },
        (error: HttpErrorResponse) => {
          this.toastrService.error('Loading approval employees info failed');
        }
      );
  }

  private checkSchoolDepartment(): void {
    this.isSchoolDepartment =
      this.departmentTypes &&
      this.departmentTypes.length > 0 &&
      this.departmentTypes.findIndex(
        (type) => type.identity.extId === organizationUnitLevelsConst.School
      ) > findIndexCommon.notFound;
  }

  private getUserDepartmentId(): number {
    return !this.user || (this.user && this.userDepartmentId)
      ? this.userDepartmentId
      : this.user.departmentId;
  }
  private filterCurrentApprovalGroups(approvalGroup: ApprovalGroup): boolean {
    const approverId = approvalGroup.approverId;
    if (this.primarySelectedGroup) {
      if (this.alternateSelectedGroup) {
        return (
          !this.user &&
          approverId !== this.alternateSelectedGroup.approverId &&
          approverId !== this.primarySelectedGroup.approverId
        );
      }
      return !this.user && approverId !== this.primarySelectedGroup.approverId;
    }
    if (this.alternateSelectedGroup) {
      return (
        !this.user && approverId !== this.alternateSelectedGroup.approverId
      );
    }

    return !this.user;
  }
}
