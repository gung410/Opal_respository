import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewEncapsulation
} from '@angular/core';

import { AuthService } from 'app-auth/auth.service';

import { ShowHideColumnModel } from 'app-models/ag-grid-column.model';

import {
  AgGridConfigModel,
  ColumDefModel
} from 'app-models/ag-grid-config.model';

import { SortModel } from 'app-models/sort.model';

import { TranslateAdapterService } from 'app-services/translate-adapter.service';

import { SurveyUtils } from 'app-utilities/survey-utils';

import { BasePresentationComponent } from 'app/shared/components/component.abstract';

import { findIndexCommon } from 'app/shared/constants/common.const';

import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import * as _ from 'lodash';

import { GridApi } from '@ag-grid-community/core';

import { DateTimeUtil } from 'app-utilities/date-time-utils';

import { SystemRole } from 'app/core/models/system-role';

import { CommonHelpers } from 'app/shared/common.helpers';

import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';

import { AssignRolePermission } from '../assign-role-permission';

import {
  initUserActions,
  initUserActionsForCreateAccButton,
  initUserActionsForExportButton
} from '../models/user-action-mapping';

import { UserManagement } from '../models/user-management.model';

import { CheckingUserRolesService } from '../services/checking-user-roles.service';

import { ActionsModel } from '../user-actions/models/actions.model';

import { UserActionsModel } from '../user-actions/models/user-actions.model';

import { CellApprovingOfficerComponent } from './cell-components/cell-approving-officer/cell-approving-officer.component';

import { CellExpandableListComponent } from './cell-components/cell-expandable-list/cell-expandable-list.component';

import { CellUserInfoComponent } from './cell-components/cell-user-info/cell-user-info.component';

import { CellUserStatusComponent } from './cell-components/cell-user-status/cell-user-status.component';

import { USER_LIST_HEADER_CONST } from './models/user-list-header.const';

import { User } from 'app-models/auth.model';

import { SAM_PERMISSIONS } from 'app/shared/constants/sam-permission.constant';

import { Utils } from 'app-utilities/utils';

import { IUserAction } from '../models/user-action.model';

import { UserActionsService } from '../user-actions.service';

import { CellDropdownUserListActionsComponent } from './cell-components/cell-dropdown-user-list-actions/cell-dropdown-user-list-actions.component';

import { UserStatusDisplayActionModel } from './models/user-selected-status.model';

@Component({
  selector: 'user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent
  extends BasePresentationComponent
  implements OnInit, OnChanges {
  agGridConfig: AgGridConfigModel;
  currentSort: SortModel;
  userStatusDisplayAction: UserStatusDisplayActionModel;

  isVerticalToShowMenuAction: boolean = true;

  @Input() currentUser: User;
  @Input() showHideColumns: ShowHideColumnModel;
  @Input() isClearSelected: boolean = false;
  @Input() userItemsData: UserManagement[];
  @Input() currentUserRoles: SystemRole[];
  @Input() isCurrentUserSuperAdmin: boolean = false;
  @Input() isCurrentUserDivAdmin: boolean = false;
  @Input() isCurrentUserAccountAdmin: boolean = false;
  @Input() reloadUserAccountTab: boolean = false;
  @Input() tabLabel: string;

  @Output() editUser: EventEmitter<any> = new EventEmitter<any>();
  @Output() sortChange: EventEmitter<SortModel> = new EventEmitter<SortModel>();
  @Output()
  userActions: EventEmitter<UserActionsModel> = new EventEmitter<UserActionsModel>();
  @Output() userActionsForExportButton: EventEmitter<
    ActionsModel[]
  > = new EventEmitter<ActionsModel[]>();
  @Output() userActionsForCreateAccButton: EventEmitter<
    ActionsModel[]
  > = new EventEmitter<ActionsModel[]>();
  @Output() selectedUser: EventEmitter<UserManagement[]> = new EventEmitter<
    UserManagement[]
  >();
  @Output()
  singleAction: EventEmitter<ActionsModel> = new EventEmitter<ActionsModel>();
  @Output() columnShowHideDef: EventEmitter<ColumDefModel[]> = new EventEmitter<
    ColumDefModel[]
  >();
  @Output() gridApi: EventEmitter<GridApi> = new EventEmitter<GridApi>();

  private currentSelectedRows: number = 0;
  private authService: AuthService;
  private visibleRoles: string[] = [];

  private gridSetting: any;

  constructor(
    private translateAdapterService: TranslateAdapterService,
    private userActionsService: UserActionsService,
    changeDetectorRef: ChangeDetectorRef,
    authService: AuthService,
    private checkingUserRolesService: CheckingUserRolesService
  ) {
    super(changeDetectorRef);
    this.authService = authService;
  }

  @HostListener('window:resize') onResize(): void {
    if (!this.agGridConfig) {
      return;
    }

    const timeoutForResize = 300;

    setTimeout(
      () => {
        this.agGridConfig.gridApi.sizeColumnsToFit();
      },

      timeoutForResize
    );
  }

  ngOnInit(): void {
    this.currentSort = new SortModel({
      currentFieldSort: 'firstName',
      currentSortType: 'asc'
    });
    this.updateSystemRoles();

    this.initGridConfig('box-icon');

    if (this.userItemsData) {
      this.agGridConfig.rowData = this.getUserDataAfterUpdatingInvisibleRoles(
        this.userItemsData
      );
    }

    const theRight = this.checkingUserRolesService.hasRightToAccessReportingUser(
      this.authService.userData().getValue().systemRoles
    );
    this.userActions.emit(
      initUserActions(
        this.translateAdapterService,
        true,
        theRight,
        this.currentUser.hasPermission(
          SAM_PERMISSIONS.BasicUserAccountsManagement
        ),
        this.currentUser.hasPermission(SAM_PERMISSIONS.ExportUsers)
      )
    );
    this.userActionsForExportButton.emit(
      initUserActionsForExportButton(this.translateAdapterService, true)
    );

    this.userActionsForCreateAccButton.emit(
      initUserActionsForCreateAccButton(
        this.translateAdapterService,
        this.currentUser.hasPermission(SAM_PERMISSIONS.SingleUserCreation),
        this.currentUser.hasPermission(SAM_PERMISSIONS.MassUserCreation)
      )
    );

    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
    window.addEventListener('scroll', CommonHelpers.freezeAgGridScroll(), true);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.agGridConfig && this.userItemsData) {
      this.agGridConfig.rowData = this.getUserDataAfterUpdatingInvisibleRoles(
        this.userItemsData
      );
    }

    if (
      (changes.userItemsData &&
        changes.userItemsData.currentValue &&
        changes.userItemsData.previousValue &&
        changes.userItemsData.currentValue.length !==
          changes.userItemsData.previousValue.length) ||
      (changes.isClearSelected &&
        changes.isClearSelected.currentValue &&
        this.agGridConfig)
    ) {
      this.agGridConfig.selectedRows = [];
      this.agGridConfig.gridApi.deselectAll();
      this.changeDetectorRef.detectChanges();
    }

    if (
      this.agGridConfig &&
      changes.showHideColumns &&
      changes.showHideColumns.currentValue !==
        changes.showHideColumns.previousValue
    ) {
      this.changeSelectedColumn();
    }
  }

  initGridConfig(menuHeaderIconDefault: string): void {
    this.agGridConfig = new AgGridConfigModel({
      columnDefs: this.setColumnDef(menuHeaderIconDefault),
      frameworkComponents: {
        cellUserInfo: CellUserInfoComponent,
        cellExpandableList: CellExpandableListComponent,
        cellApprovingOfficer: CellApprovingOfficerComponent,
        cellUserStatus: CellUserStatusComponent,
        cellDropdownMenu: CellDropdownUserListActionsComponent
      },

      rowData: [],
      selectedRows: [],
      context: {
        componentParent: this
      },

      defaultColDef: {
        resizable: true
      }
    });
  }

  onSelectionChanged(event: any): void {
    // This block will be called whenever user click tab user-pending
    // Then we need to stop emit user-action to avoid duplicate emit
    if (this.isClearSelected) {
      this.isClearSelected = false;

      return;
    }

    const selectedRows = event.api.getSelectedRows();
    this.agGridConfig.selectedRows = selectedRows;
    const displayedRowCount: number = event.api.getDisplayedRowCount();

    const newSelectedRows = event.api.getSelectedRows().length;

    if (
      this.currentSelectedRows > 0 &&
      this.currentSelectedRows < displayedRowCount &&
      newSelectedRows === displayedRowCount &&
      this.currentSelectedRows !== displayedRowCount - 1
    ) {
      event.api.deselectAll();
    }

    this.currentSelectedRows = event.api.getSelectedRows().length;
    let actions: UserActionsModel = this.filterActionBasedOnStatus(
      selectedRows
    );

    actions = this.filterSatisfiedActionsByPermissions(actions);

    this.userActions.emit(actions);

    this.selectedUser.emit(selectedRows);
    this.changeDetectorRef.detectChanges();
  }

  setColumnDef(menuHeaderIcon: string): ColumDefModel[] {
    return [
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.Name.text
        ),
        field: USER_LIST_HEADER_CONST.Name.fieldName,
        colId: USER_LIST_HEADER_CONST.Name.colId,
        minWidth: 350,
        checkboxSelection: true,
        headerCheckboxSelection: true,
        sortable: true,
        cellStyle: {},

        cellRenderer: 'cellUserInfo',
        pinned: 'left',
        lockPinned: true
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.ServiceScheme.text
        ),
        field: USER_LIST_HEADER_CONST.ServiceScheme.fieldName,
        colId: USER_LIST_HEADER_CONST.ServiceScheme.colId,
        minWidth: 163,
        sortable: false,
        suppressMenu: true,
        autoHeight: false,
        valueFormatter: (params: any) => {
          return params && params.data && params.data.personnelGroups
            ? SurveyUtils.mapArrayLocalizedToArrayObject(
                params.data.personnelGroups,
                localStorage.getItem('language-code')
              ).join(', ')
            : '';
        },

        hide: false
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.OrganisationUnit.text
        ),
        field: USER_LIST_HEADER_CONST.OrganisationUnit.fieldName,
        colId: USER_LIST_HEADER_CONST.OrganisationUnit.colId,
        tooltipField: USER_LIST_HEADER_CONST.OrganisationUnit.fieldName,
        minWidth: 190,
        sortable: false,
        suppressMenu: true,
        cellStyle: {
          display: 'block',
          'line-height': '46px'
        },

        hide: false
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.GrowthModel.text
        ),
        field: USER_LIST_HEADER_CONST.GrowthModel.fieldName,
        colId: USER_LIST_HEADER_CONST.GrowthModel.colId,
        minWidth: 160,
        sortable: false,
        suppressMenu: true,
        cellStyle: {},

        cellRenderer: 'cellExpandableList',
        hide: false
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.DevelopmentalRole.text
        ),
        field: USER_LIST_HEADER_CONST.DevelopmentalRole.fieldName,
        colId: USER_LIST_HEADER_CONST.DevelopmentalRole.colId,
        minWidth: 191,
        sortable: false,
        suppressMenu: true,
        cellStyle: {},

        cellRenderer: 'cellExpandableList',
        hide: false
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.SystemRoles.text
        ),
        field: USER_LIST_HEADER_CONST.SystemRoles.fieldName,
        colId: USER_LIST_HEADER_CONST.SystemRoles.colId,
        minWidth: 180,
        sortable: false,
        suppressMenu: true,
        cellStyle: {},

        cellRendererParams: {},

        cellRenderer: 'cellExpandableList',
        hide: false
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.ApprovingOfficer.text
        ),
        field: USER_LIST_HEADER_CONST.ApprovingOfficer.fieldName,
        colId: USER_LIST_HEADER_CONST.ApprovingOfficer.colId,
        minWidth: 181,
        sortable: false,
        suppressMenu: true,
        cellStyle: {},

        cellRendererParams: {},

        cellRenderer: 'cellApprovingOfficer',
        hide: false
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.Status.text
        ),
        colId: USER_LIST_HEADER_CONST.Status.colId,
        cellRenderer: 'cellUserStatus',
        minWidth: 180,
        sortable: false,
        suppressMenu: true,
        cellStyle: {
          overflow: 'visible',
          'z-index': '9999'
        },

        cellRendererParams: {},

        hide: false,
        headerClass: 'grid-header-centered',
        cellClass: 'grid-cell-centered'
      }),
      new ColumDefModel({
        headerName: '',
        cellRenderer: 'cellDropdownMenu',
        maxWidth: 55,
        minWidth: 55,
        sortable: false,
        suppressMenu: true,
        pinned: 'right',
        cellStyle: {
          overflow: 'visible',
          'z-index': '9999',
          'padding-left': '10px'
        },

        cellRendererParams: {
          onClick: this.onActionClicked.bind(this)
        }
      })
    ];
  }

  onGridReady(params: any): void {
    this.gridSetting = params;

    this.emitColumnShowHideDefAndGridApi();
  }

  emitColumnShowHideDefAndGridApi() {
    if (!this.gridSetting) {
      return;
    }

    this.agGridConfig.gridApi = this.gridSetting.api;
    this.agGridConfig.gridColumnApi = this.gridSetting.columnApi;
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
        this.columnShowHideDef.emit(this.agGridConfig.columnShowHide);
      }
    }

    this.gridApi.emit(this.agGridConfig.gridApi);
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  changeSelectedColumn(): void {
    if (this.showHideColumns.column && this.showHideColumns.column.colId) {
      this.agGridConfig.gridColumnApi.setColumnVisible(
        this.showHideColumns.column.colId,
        this.showHideColumns.selected
      );
      this.agGridConfig.gridApi.sizeColumnsToFit();
    }
  }

  onSortChange(event: any): void {
    const sortModel = event.api.getSortModel().shift();

    if (sortModel) {
      this.currentSort.currentSortType = sortModel.sort;
      this.currentSort.currentFieldSort = sortModel.colId;
      this.sortChange.emit(this.currentSort);
    }
  }

  onEditUserClicked($event: any): void {
    this.editUser.emit($event.data);
  }

  onActionClicked($event: any): void {
    this.singleAction.emit($event);
  }

  onButtonGroupActionClick($event: ActionsModel): void {}

  openDialogToEnableColumns(): void {}

  isAllowToEditUser(): boolean {
    return this.currentUser.hasPermission(
      SAM_PERMISSIONS.BasicUserAccountsManagement
    );
  }

  filterSatisfiedListActionsByPermissions(
    actions: ActionsModel[]
  ): ActionsModel[] {
    const filteredActions: ActionsModel[] = [];

    let isAllowed: boolean;

    actions.forEach((action) => {
      switch (action.actionType) {
        case StatusActionTypeEnum.Suspend:
        case StatusActionTypeEnum.ResetPassword:
        case StatusActionTypeEnum.Edit:
        case StatusActionTypeEnum.Active:
        case StatusActionTypeEnum.Unlock:
        case StatusActionTypeEnum.SetApprovingOfficers:
        case StatusActionTypeEnum.SetExpirationDate:
        case StatusActionTypeEnum.AddToGroup:
          isAllowed = this.currentUser.hasPermission(
            SAM_PERMISSIONS.BasicUserAccountsManagement
          );
          break;
        case StatusActionTypeEnum.Archive:
        case StatusActionTypeEnum.Unarchive:
        case StatusActionTypeEnum.Delete:
        case StatusActionTypeEnum.ChangeUserPlaceOfWork:
          isAllowed = this.currentUser.hasPermission(
            SAM_PERMISSIONS.AdvancedUserAccountsManagement
          );
          break;
        case StatusActionTypeEnum.Export:
          isAllowed = this.currentUser.hasPermission(
            SAM_PERMISSIONS.ExportUsers
          );
          break;
        default:
          isAllowed = false;
          break;
      }

      if (isAllowed) {
        filteredActions.push(action);
      }
    });

    return filteredActions;
  }

  private filterSatisfiedActionsByPermissions(
    userActions: UserActionsModel
  ): UserActionsModel {
    const filterUserActions = Utils.cloneDeep(userActions);

    if (
      filterUserActions.listEssentialActions &&
      filterUserActions.listEssentialActions.length
    ) {
      filterUserActions.listEssentialActions = this.filterSatisfiedListActionsByPermissions(
        filterUserActions.listEssentialActions
      );
    }

    if (
      filterUserActions.listNonEssentialActions &&
      filterUserActions.listNonEssentialActions.length
    ) {
      filterUserActions.listNonEssentialActions = this.filterSatisfiedListActionsByPermissions(
        filterUserActions.listNonEssentialActions
      );
    }

    return filterUserActions;
  }

  private getUserDataAfterUpdatingInvisibleRoles(
    userData: UserManagement[]
  ): UserManagement[] {
    // TODO: OP-3826 will be fixed (do not change reference of userData)
    const clonedUserItemsData = userData;

    clonedUserItemsData.forEach((user) => {
      if (
        this.currentUser &&
        user &&
        this.currentUser.identity.extId === user.identity.extId
      ) {
        return;
      }

      // user.systemRoles =
      //   user.systemRoles &&
      //   user.systemRoles.filter((role: any) =>
      //     this.visibleRoles.includes(role.identity.extId)
      //   );
    });

    return clonedUserItemsData;
  }

  private updateSystemRoles(): void {
    const userAssignRolePermission = this.currentUserRoles
      .map((role) => AssignRolePermission[role.identity.extId])
      .reduce((arr1, arr2) => _.union(arr1, arr2), []);

    this.visibleRoles = userAssignRolePermission;
  }

  private getImmediatelyLanguage(columnName: string): string {
    return this.translateAdapterService.getValueImmediately(
      USER_LIST_HEADER_CONST.GroupKey.text + '.' + columnName
    );
  }

  private filterActionBasedOnStatus(
    selectedItems: UserManagement[]
  ): UserActionsModel {
    const uniqueStatusIdsOfSelectedUsers: string[] = _.uniqBy(
      selectedItems,
      'entityStatus.statusId'
    ).map((user: UserManagement) => {
      return user.entityStatus.statusId;
    });

    const userActions = this.userActionsService.getActions(
      'userList',
      this.currentUser
    );

    const actCorrectWithUserActionMapping = uniqueStatusIdsOfSelectedUsers.map(
      (statusOfSelectedUser: string) => {
        return userActions.filter((action: IUserAction) => {
          return (
            action.currentStatus.findIndex(
              (statusMapping: string) => statusMapping === statusOfSelectedUser
            ) > findIndexCommon.notFound &&
            action.targetAction !== StatusActionTypeEnum.Edit
          );
        });
      }
    );

    let uniqueAction = _.intersection(...actCorrectWithUserActionMapping);

    if (this.isCurrentUserSelected(selectedItems)) {
      uniqueAction = uniqueAction.filter((action) => {
        return (
          action.targetAction !== StatusActionTypeEnum.Suspend &&
          action.targetAction !== StatusActionTypeEnum.Archive &&
          action.targetAction !== StatusActionTypeEnum.Delete &&
          action.targetAction !== StatusActionTypeEnum.Unarchive
        );
      });
    }

    if (!this.isAllowToUnarchive(selectedItems)) {
      uniqueAction = uniqueAction.filter((action) => {
        return action.targetAction !== StatusActionTypeEnum.Unarchive;
      });
    }

    if (this.hasMOEUsers(selectedItems)) {
      uniqueAction = uniqueAction.filter((action) => {
        return action.targetAction !== StatusActionTypeEnum.Delete;
      });
    }

    const theRight = this.checkingUserRolesService.hasRightToAccessReportingUser(
      this.authService.userData().getValue().systemRoles
    );
    const userAction = initUserActions(
      this.translateAdapterService,
      true,
      theRight,
      this.currentUser.hasPermission(
        SAM_PERMISSIONS.BasicUserAccountsManagement
      ),
      this.currentUser.hasPermission(SAM_PERMISSIONS.ExportUsers)
    );

    userAction.listNonEssentialActions = uniqueAction.map((item: any) => {
      return new ActionsModel({
        text: this.translateAdapterService.getValueImmediately(
          `User_Account_Page.User_Context_Menu.${item.targetAction}`
        ),
        actionType: item.targetAction,
        allowActionSingle: item.allowActionSingle,
        icon: item.targetIcon,
        message: item.message
      });
    });

    return userAction;
  }

  private isCurrentUserSelected(selectedUsers: UserManagement[]): boolean {
    if (!selectedUsers) {
      return false;
    }

    return (
      selectedUsers.findIndex((user) => {
        return user.identity.extId === this.currentUser.identity.extId;
      }) > -1
    );
  }

  private isAllowToUnarchive(selectedUser: UserManagement[]): boolean {
    return selectedUser.every(
      (selectedUser: UserManagement) =>
        selectedUser.entityStatus.statusId === StatusTypeEnum.Archived.code &&
        (!selectedUser.entityStatus.expirationDate ||
          (selectedUser.entityStatus.expirationDate &&
            DateTimeUtil.compareDate(
              new Date(selectedUser.entityStatus.expirationDate),
              new Date(),
              false
            ) > 0))
    );
  }

  private hasMOEUsers(selectedUsers: UserManagement[]): boolean {
    return selectedUsers.some(
      (selectedUser: UserManagement) =>
        selectedUser.entityStatus.externallyMastered === true
    );
  }
}
