import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  ViewEncapsulation
} from '@angular/core';
import {
  CxColumnSortType,
  CxFormModal,
  CxGlobalLoaderService,
  CxSurveyjsVariable,
  CxTableIcon
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { AgGridConfigModel } from 'app-models/ag-grid-config.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { UserGroupHeaderConstant } from 'app/shared/constants/user-group-list.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { UserTypeEnum } from 'app/shared/constants/user-type.enum';
import { PagingResponseModel } from 'app/user-accounts/models/user-management.model';
import * as _ from 'lodash';

import { TranslateService } from '@ngx-translate/core';
import { DepartmentType } from 'app-models/department-type.model';
import { Identity } from 'app-models/identity.model';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import { Department } from 'app/department-hierarchical/models/department.model';
import { CommonHelpers } from 'app/shared/common.helpers';
import { UserActionsModel } from 'app/user-accounts/user-actions/models/user-actions.model';
import { forkJoin } from 'rxjs';
import { UserGroupFormJSON } from '../user-group-form';
import { UserGroupsDataService } from '../user-groups-data.service';
import { UserGroupsRemoveConfirmationComponent } from '../user-groups-remove-confirmation/user-groups-remove-confirmation.component';
import {
  MembershipDto,
  UserGroupDto,
  UserGroupFilterParams
} from '../user-groups.model';
import { CellDropdownMenuComponent } from './cell-dropdown-menu/cell-dropdown-menu.component';
import { UserGroupModifyFormComponent } from './user-group-modify-form/user-group-modify-form.component';
import { SAM_PERMISSIONS } from '../../shared/constants/sam-permission.constant';
import { ColDef } from 'ag-grid-community';

@Component({
  selector: 'user-group-list',
  templateUrl: './user-group-list.component.html',
  styleUrls: ['./user-group-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserGroupListComponent
  extends BaseScreenComponent
  implements OnInit {
  userDepartmentId: number; // The identifier of the department of the current logged-in user.
  currentDepartmentId: number = -1; // The identifier of the viewing-department.
  filterParams: UserGroupFilterParams;
  userGroupData: PagingResponseModel<UserGroupDto>;
  userGroupSelected: PagingResponseModel<UserGroupDto>;
  userGroupFormJSON: any = UserGroupFormJSON;
  currentSortType: CxColumnSortType = CxColumnSortType.ASCENDING;
  currentFieldSort: string = 'name';
  agGridConfig: AgGridConfigModel;
  tableIcon: CxTableIcon = new CxTableIcon({
    removeClass: 'material-icons remove-icon',
    moreActionsClass: 'material-icons more-vert'
  });
  removedGroups: any[] = [];
  notRemovedGroups: any[] = [];
  removedGroupIds: any[] = [];
  defaultPageSize: number = AppConstant.ItemPerPage;

  userActions: UserActionsModel;

  private selectedUserGroupId: number;
  private currentSelectedRows: number = 0;
  constructor(
    public authService: AuthService,
    private userGroupsDataService: UserGroupsDataService,
    private translateAdapterService: TranslateAdapterService,
    private formModal: CxFormModal,
    private ngbModal: NgbModal,
    private toastr: ToastrAdapterService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private translateService: TranslateService,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    this.subscription.add(
      this.authService.userData().subscribe((user) => {
        this.currentUser = user;
        if (this.currentUser) {
          this.userDepartmentId = this.currentUser.departmentId;
          if (this.userDepartmentId === undefined) {
            return;
          }
          this.currentDepartmentId = this.userDepartmentId;
          this.initFilterParams();
          this.getUserGroups();
        }
        this.changeDetectorRef.detectChanges();
      })
    );
    this.initGridConfig();

    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
    window.addEventListener('scroll', CommonHelpers.freezeAgGridScroll(), true);
    window.addEventListener('scroll', CommonHelpers.freezeMenuActions(), true);
  }

  get isCurrentAllowToActionOnUserGroup(): boolean {
    return this.currentUser.hasPermission(
      SAM_PERMISSIONS.CUDinUserGroupManagement
    );
  }

  initGridConfig(): void {
    this.agGridConfig = new AgGridConfigModel({
      columnDefs: this.setColumnDef(),
      frameworkComponents: {
        cellDropdownMenu: CellDropdownMenuComponent
      },
      rowData: [],
      selectedRows: [],
      context: { componentParent: this }
    });
  }

  onSelectionChanged(event: any): void {
    this.agGridConfig.selectedRows = event.api.getSelectedRows();
    const displayedRowCount = event.api.getDisplayedRowCount();

    const newSelectedRows = event.api.getSelectedRows().length;
    if (
      this.currentSelectedRows > 0 &&
      this.currentSelectedRows < displayedRowCount &&
      newSelectedRows === displayedRowCount
    ) {
      event.api.deselectAll();
    }
    this.currentSelectedRows = event.api.getSelectedRows().length;
    this.changeDetectorRef.detectChanges();
  }

  setColumnDef(): any {
    const colsDef: ColDef[] = [
      {
        headerName: this.getImmediatelyLanguage(
          UserGroupHeaderConstant.Name.text
        ),
        field: UserGroupHeaderConstant.Name.fieldName,
        colId: UserGroupHeaderConstant.Name.colId,
        minWidth: 200,
        checkboxSelection: this.isCurrentAllowToActionOnUserGroup,
        headerCheckboxSelection: this.isCurrentAllowToActionOnUserGroup,
        sortable: true
      },
      {
        headerName: this.getImmediatelyLanguage(
          UserGroupHeaderConstant.Description.text
        ),
        field: UserGroupHeaderConstant.Description.fieldName,
        colId: UserGroupHeaderConstant.Description.colId,
        minWidth: 200,
        sortable: true,
        suppressMenu: true
      },
      {
        headerName: this.getImmediatelyLanguage(
          UserGroupHeaderConstant.Members.text
        ),
        field: UserGroupHeaderConstant.Members.fieldName,
        colId: UserGroupHeaderConstant.Members.colId,
        minWidth: 200,
        sortable: false,
        suppressMenu: true
      }
    ];

    if (this.isCurrentAllowToActionOnUserGroup) {
      colsDef.push({
        headerName: '',
        cellRenderer: 'cellDropdownMenu',
        maxWidth: 50,
        minWidth: 50,
        sortable: false,
        suppressMenu: true,
        pinned: 'right',
        cellStyle: {
          overflow: 'visible',
          'padding-left': '10px'
        },
        cellRendererParams: {
          onClick: this.onClickDropdownMenuAction.bind(this),
          label: 'OnClick'
        }
      });
    }

    return colsDef;
  }

  onGridReady(params: any): void {
    this.agGridConfig.gridApi = params.api;
    this.agGridConfig.gridColumnApi = params.columnApi;
    this.agGridConfig.gridApi.setDomLayout('autoHeight');
    this.agGridConfig.gridApi.sizeColumnsToFit();
    if (
      this.agGridConfig.gridColumnApi &&
      this.agGridConfig.gridColumnApi.columnController
    ) {
      if (
        this.agGridConfig.columnShowHide &&
        this.agGridConfig.columnShowHide.length <= 0
      ) {
        this.agGridConfig.columnShowHide = this.agGridConfig.gridColumnApi.columnController.columnDefs;
        this.agGridConfig.columnShowHide.pop();
      }
    }
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  changeSelectedColumn(selected: any, column: any): void {
    if (column && column.colId) {
      this.agGridConfig.gridColumnApi.setColumnVisible(column.colId, selected);
      this.agGridConfig.gridApi.sizeColumnsToFit();
    }
  }

  onSortChange(event: any): void {
    const sortModel = event.api.getSortModel().shift();
    if (sortModel) {
      this.currentSortType =
        sortModel.sort === 'asc'
          ? CxColumnSortType.ASCENDING
          : CxColumnSortType.DESCENDING;
      this.currentFieldSort = sortModel.colId;
      this.getUserGroups();
    }
  }

  onUserGroupPageChange(pageIndex: number): void {
    this.filterParams.pageIndex = pageIndex;
    this.getUserGroups();
    window.scroll(0, 0);
  }

  onPageSizeChange(pageSize: number): void {
    if (Number(pageSize) > Number(this.filterParams.pageSize)) {
      this.filterParams.pageIndex = 1;
    }
    this.filterParams.pageSize = pageSize;
    this.getUserGroups();
    window.scroll(0, 0);
  }

  onSortTypeChange($event: {
    fieldSort: string;
    sortType: CxColumnSortType;
  }): void {
    this.currentSortType = $event.sortType;
    this.currentFieldSort = $event.fieldSort;
    this.getUserGroups();
  }

  onCreateUserGroupClicked(): void {
    if (this.formModal.hasOpenModals()) {
      return;
    }

    const modalRef = this.ngbModal.open(UserGroupModifyFormComponent, {
      size: 'lg',
      centered: true,
      keyboard: false,
      backdrop: 'static'
    });

    modalRef.result.then((data) => {
      const submitData = data;
      if (submitData) {
        this.cxGlobalLoaderService.showLoader();
        const newUserGroupDTO: Partial<UserGroupDto> = this.buildNewUserGroupDtoFromSubmittedForm(
          submitData
        );
        this.userGroupsDataService.createUserGroup(newUserGroupDTO).subscribe(
          (newCreatedUserGroup: UserGroupDto) => {
            // Embed member count to newCreatedUserGroup.
            newCreatedUserGroup.memberCount = submitData.addedUsers.length;
            // Push the new one on the top of the list.
            this.agGridConfig.rowData = [
              newCreatedUserGroup,
              ...this.userGroupData.items
            ];

            this.changeDetectorRef.detectChanges();
            this.toastr.success(
              `User group ${newCreatedUserGroup.name} has been created successfully`
            );

            if (submitData.addedUsers.length) {
              let newMembers: MembershipDto[] = submitData.addedUsers;
              newMembers = submitData.addedUsers.map((user: MembershipDto) => {
                return this.buildMemberDtoFromSubmittedForm(
                  user,
                  newCreatedUserGroup.identity.id
                );
              });

              this.userGroupsDataService
                .addMembersToUserGroup(
                  newCreatedUserGroup.identity.id,
                  newMembers
                )
                .subscribe(
                  (newCreatedMembers: any) => {
                    this.toastr.success(
                      `${newCreatedMembers.length} user account(s) has been added into group '${submitData.name}'`
                    );
                  },
                  (error: any) => {
                    this.toastr.error(
                      error.error.Message,
                      'An error occurred when adding user accounts to group.'
                    );
                  }
                );
            } else {
              this.getUserGroups();
            }
          },
          (error) => {
            this.toastr.error(
              error.error.Message,
              'An error occurred when creating new user group.'
            );
          },
          () => {
            this.cxGlobalLoaderService.hideLoader();
          }
        );
      }
    });
  }

  onClickDropdownMenuAction(event: any): void {
    const action = event.action;
    switch (action) {
      case 'edit':
        this.onEditUserGroupClicked(event);
        break;
      case 'delete':
        this.onDeleteUserGroupClicked(event.data);
        break;
      default:
        break;
    }
  }

  onEditUserGroupClicked(event: any): void {
    if (this.isLastColumn(event)) {
      return;
    }
    if (this.formModal.hasOpenModals()) {
      return;
    }

    const selectedUserGroup: UserGroupDto = event.data;

    this.selectedUserGroupId = selectedUserGroup.identity.id;

    const modalRef = this.ngbModal.open(UserGroupModifyFormComponent, {
      size: 'lg',
      centered: true,
      keyboard: false,
      backdrop: 'static'
    });

    modalRef.componentInstance.selectedUserGroupId = this.selectedUserGroupId;
    modalRef.componentInstance.name = selectedUserGroup.name;
    modalRef.componentInstance.description = selectedUserGroup.description;
    modalRef.result.then((data) => {
      const submitData = data;
      if (submitData) {
        this.cxGlobalLoaderService.showLoader();
        // Ensure the field 'Description' should always be presented in the payload even though the value is empty.
        // Otherwise the api won't update this field if the value is removed.

        const updateUserGroupDTO = selectedUserGroup;

        updateUserGroupDTO.name = submitData.name;
        updateUserGroupDTO.description = submitData.description;
        updateUserGroupDTO.entityStatus.lastUpdatedBy = this.currentUser.identity.id;

        this.userGroupsDataService
          .updateUserGroup(updateUserGroupDTO)
          .subscribe(
            (updatedUserGroup: UserGroupDto) => {
              this.toastr.success(
                `User group ${updatedUserGroup.name} has been updated successfully`
              );

              if (submitData.removedUsers.length) {
                this.cxGlobalLoaderService.showLoader();
                let removedMembers: MembershipDto[] = submitData.removedUsers;
                removedMembers = submitData.removedUsers.map(
                  (user: MembershipDto) => {
                    return this.buildMemberDtoFromSubmittedForm(
                      user,
                      updatedUserGroup.identity.id
                    );
                  }
                );

                if (removedMembers && removedMembers.length > 0) {
                  this.removeUserFromUserGroup(
                    updatedUserGroup,
                    removedMembers
                  );
                } else {
                  this.toastr.success(
                    `User group ${updatedUserGroup.name} has been updated successfully`
                  );
                }
              }

              if (submitData.addedUsers.length) {
                let newMembers: MembershipDto[] = submitData.addedUsers;
                newMembers = submitData.addedUsers.map(
                  (user: MembershipDto) => {
                    return this.buildMemberDtoFromSubmittedForm(
                      user,
                      updatedUserGroup.identity.id
                    );
                  }
                );

                this.userGroupsDataService
                  .addMembersToUserGroup(
                    updatedUserGroup.identity.id,
                    newMembers
                  )
                  .subscribe(
                    (newCreatedMembers: any) => {
                      this.getUserGroups();
                      this.toastr.success(
                        `${newCreatedMembers.length} user account(s) has been added into group '${submitData.name}'`
                      );
                    },
                    (error: any) => {
                      this.toastr.error(
                        error.error.Message,
                        'An error occurred when adding user accounts to group.'
                      );
                    }
                  );
              } else {
                this.getUserGroups();
              }
            },
            (error) => {
              this.toastr.error(
                error.error.Message,
                'An error occurred when updating user group.'
              );
            },
            () => {
              this.cxGlobalLoaderService.hideLoader();
            }
          );
      }
    });
  }

  isLastColumn(params: any): boolean {
    if (!params.columnApi) {
      return false;
    }
    const displayedColumns = params.columnApi.getAllDisplayedColumns();
    const thisIsLastColumn =
      displayedColumns[displayedColumns.length - 1] === params.column;

    return thisIsLastColumn;
  }

  async onDeleteUserGroupClicked(selectedRow: any = null): Promise<any> {
    this.removedGroups = [];
    this.notRemovedGroups = [];
    this.removedGroupIds = [];
    if (
      (this.agGridConfig.selectedRows &&
        this.agGridConfig.selectedRows.length > 0) ||
      selectedRow
    ) {
      if (!selectedRow) {
        for (const group of this.agGridConfig.selectedRows) {
          // Check if any members
          if (group.memberCount === undefined || group.memberCount === 0) {
            this.removedGroups.push(group.name);
            this.removedGroupIds.push(group.identity.id);
          } else {
            this.notRemovedGroups.push(group.name);
          }
        }
      } else {
        if (!selectedRow.memberCount) {
          this.removedGroups.push(selectedRow.name);
          this.removedGroupIds.push(selectedRow.identity.id);
        } else {
          this.notRemovedGroups.push(selectedRow.name);
        }
      }

      if (this.notRemovedGroups.length > 0) {
        this.toastr.warning(
          `${this.translateAdapterService.getValueImmediately(
            'User_Group_Page.Remove_User_Group.RemoveFailed'
          )}: ${this.notRemovedGroups.join(', ')}`
        );
      }
      if (this.removedGroupIds.length > 0) {
        const modalRef = this.ngbModal.open(
          UserGroupsRemoveConfirmationComponent,
          {
            size: 'lg',
            centered: true,
            backdrop: 'static'
          }
        );
        const message = this.translateService.instant(
          'User_Group_Page.Remove_User_Group.Confirmation'
        );

        const removeGroupConfirmDialogComponent = modalRef.componentInstance as UserGroupsRemoveConfirmationComponent;
        removeGroupConfirmDialogComponent.doneButtonText = this.translateService.instant(
          'Common.Button.Confirm'
        );
        removeGroupConfirmDialogComponent.message = message;
        removeGroupConfirmDialogComponent.removedGroups = this.removedGroups;

        const observer = this.subscription.add(
          removeGroupConfirmDialogComponent.done.subscribe(() => {
            forkJoin(
              this.removedGroupIds.map((groupId: number) => {
                return this.userGroupsDataService.deleteUserGroup(groupId);
              })
            ).subscribe(
              (responses: UserGroupDto[]) => {
                if (responses) {
                  this.removedGroupIds.forEach((deleteUserGroupId: any) => {
                    const index = this.findGroupIndexInGroupList(
                      deleteUserGroupId
                    );
                    if (index < 0) {
                      return;
                    }
                    this.userGroupData.items.splice(index, 1);
                  });
                  this.agGridConfig.rowData = [...this.userGroupData.items];
                  this.changeDetectorRef.detectChanges();
                  this.toastr.success(
                    `${this.translateAdapterService.getValueImmediately(
                      'User_Group_Page.Remove_User_Group.RemoveSuccess'
                    )}: ${this.removedGroups.join(', ')}`
                  );
                }
              },
              (error) => {
                this.toastr.error(
                  error.error.Message,
                  `${this.translateAdapterService.getValueImmediately(
                    'User_Group_Page.Remove_User_Group.Error'
                  )}`
                );
              }
            );

            this.agGridConfig.selectedRows = [];
            modalRef.close();
          })
        );

        modalRef.result.finally(() => {
          modalRef.close();
          observer.unsubscribe();
        });

        this.subscription.add(
          removeGroupConfirmDialogComponent.cancel.subscribe((data: any) => {
            modalRef.close();
          })
        );
      }
    }
  }

  removeUserFromUserGroup(
    selectedUserGroup: UserGroupDto,
    members: MembershipDto[]
  ): void {
    this.subscription.add(
      this.userGroupsDataService
        .deleteMember(this.selectedUserGroupId, members)
        .subscribe(
          (data) => {
            if (data) {
              selectedUserGroup.memberCount =
                selectedUserGroup.memberCount - members.length;

              this.getUserGroups();
              this.toastr.success(
                `User group ${selectedUserGroup.name} has been updated successfully`
              );
            }
          },
          (error) => {
            this.toastr.error(
              error.error.Message,
              'An error occurred when updating user group.'
            );
          }
        )
    );
  }

  private buildMemberDtoFromSubmittedForm(
    newMember: MembershipDto,
    userGroupId: number
  ): MembershipDto {
    const newMemberDto = new MembershipDto({ ...newMember });
    newMemberDto.identity = new Identity({
      ownerId: newMember.identity.ownerId,
      customerId: newMember.identity.customerId,
      archetype: UserTypeEnum.Unknown
    });
    newMemberDto.memberId = newMember.identity.id;
    newMemberDto.groupId = userGroupId;
    newMemberDto.createdBy = this.currentUser.identity.id;
    newMemberDto.entityStatus = {
      statusId: StatusTypeEnum.Active.code,
      statusReasonId: 'Active_None',
      lastUpdatedBy: this.currentUser.identity.id
    };

    return newMemberDto;
  }

  private findGroupIndexInGroupList(groupId: any): number {
    return this.userGroupData.items.findIndex(
      (group: UserGroupDto) => group.identity.id === groupId
    );
  }

  private initFilterParams(): void {
    this.filterParams = new UserGroupFilterParams();
    this.filterParams.departmentIds = [];
    this.filterParams.departmentIds.push(this.currentDepartmentId);
    this.filterParams.countActiveMembers = true;
    this.filterParams.pageSize = this.defaultPageSize;
  }

  private getUserGroups(): void {
    this.cxGlobalLoaderService.showLoader();
    this.filterParams.orderBy = `${this.currentFieldSort} ${
      this.currentSortType === CxColumnSortType.DESCENDING ? 'DESC' : ''
    }`;
    this.filterParams.countActiveMembers = true;

    this.userGroupsDataService.getUserGroups(this.filterParams).subscribe(
      (response) => {
        this.userGroupData = response;
        if (response) {
          this.agGridConfig.rowData = [];
          this.agGridConfig.rowData = response.items;
        }

        this.changeDetectorRef.detectChanges();
      },
      (error) => {
        this.toastr.error(
          error.error.Message,
          'An error occurred when getting user groups.'
        );
      },
      () => {
        this.cxGlobalLoaderService.hideLoader();
      }
    );
  }

  private buildNewUserGroupDtoFromSubmittedForm(
    submittedData: any
  ): UserGroupDto {
    const newUserGroupDto = new UserGroupDto({ ...submittedData });
    newUserGroupDto.userId = this.currentUser.identity.id;
    newUserGroupDto.departmentId = this.currentDepartmentId;
    newUserGroupDto.identity = {
      ownerId: AppConstant.ownerId,
      customerId: AppConstant.customerId,
      archetype: UserTypeEnum.UserPool
    };
    newUserGroupDto.entityStatus = {
      statusId: StatusTypeEnum.Active.code,
      statusReasonId: 'Unknown',
      lastUpdatedBy: this.currentUser.identity.id
    };

    return newUserGroupDto;
  }

  private updateUserGroupList(
    updatedUserGroupIndexInUserGroupData: number,
    updatedUserGroup: UserGroupDto
  ): void {
    const newUserGroupList = _.clone(this.userGroupData.items);
    newUserGroupList[updatedUserGroupIndexInUserGroupData] = updatedUserGroup;
    this.userGroupData.items = newUserGroupList;
    this.agGridConfig.rowData = newUserGroupList;
    this.changeDetectorRef.detectChanges();
  }

  private findRemovedMembers(oldList: any[], newList: any[]): any[] {
    if (newList && newList.length > 0) {
      const activeMemberIds = newList.map((m) => m.memberId);

      return oldList.filter((m) => activeMemberIds.indexOf(m.memberId) === -1);
    }
    // All members are removed so this function will return all the caching members.

    return oldList;
  }

  private getImmediatelyLanguage(columnName: string): string {
    return this.translateAdapterService.getValueImmediately(
      'User_Group_Page.TableHeader.' + columnName
    );
  }

  private buildSurveyVariablesForCreateNewUserGroup(
    currentDepartment: Department,
    currentDepartmentTypes: DepartmentType[]
  ): CxSurveyjsVariable[] {
    const surveyjsVariables = [
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.formDisplayMode,
        value: 'create'
      }),
      new CxSurveyjsVariable({
        name: 'formDisplayTabs',
        value: ['basic', 'members']
      })
    ];

    return _.union(
      surveyjsVariables,
      this.cxSurveyjsExtendedService.buildCurrentObjectDepartmentVariables(
        currentDepartment
      ),
      this.cxSurveyjsExtendedService.buildCurrentObjectDepartmentTypes(
        currentDepartmentTypes
      )
    );
  }

  private buildSurveyVariablesForEditUserGroup(
    userGroup: UserGroupDto,
    currentDepartment: Department,
    currentDepartmentTypes: DepartmentType[]
  ): CxSurveyjsVariable[] {
    const surveyjsVariables = [
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.formDisplayMode,
        value: 'edit'
      }),
      new CxSurveyjsVariable({ name: 'formDisplayTabs', value: ['basic'] })
    ];

    return _.union(
      surveyjsVariables,
      this.cxSurveyjsExtendedService.buildCurrentObjectVariables(userGroup),
      this.cxSurveyjsExtendedService.buildCurrentObjectDepartmentVariables(
        currentDepartment
      ),
      this.cxSurveyjsExtendedService.buildCurrentObjectDepartmentTypes(
        currentDepartmentTypes
      )
    );
  }
}
