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
import { ColDef } from 'ag-grid-community';
import { AgGridConfigModel } from 'app-models/ag-grid-config.model';
import { SortModel } from 'app-models/sort.model';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { BasePresentationComponent } from 'app/shared/components/component.abstract';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';

import { User } from 'app-models/auth.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { SystemRole } from 'app/core/models/system-role';
import { AppConstant } from 'app/shared/app.constant';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import * as _ from 'lodash';
import { SAM_PERMISSIONS } from '../../shared/constants/sam-permission.constant';
import { Utils } from '../../shared/utilities/utils';
import { USER_ACTION_MAPPING_CONST } from '../models/user-action-mapping';
import { UserManagement } from '../models/user-management.model';
import { ActionsModel } from '../user-actions/models/actions.model';
import { UserActionsModel } from '../user-actions/models/user-actions.model';
import { CellUserInfoComponent } from '../user-list/cell-components/cell-user-info/cell-user-info.component';
import { CellUserStatusComponent } from '../user-list/cell-components/cell-user-status/cell-user-status.component';
import { UserStatusDisplayActionModel } from '../user-list/models/user-selected-status.model';
import { CellDropdownOtherPlaceOfWorkActionsComponent } from './cell-components/cell-dropdown-other-place-actions.component';

@Component({
  selector: 'user-other-place-list',
  templateUrl: './user-other-place-list.component.html',
  styleUrls: ['./user-other-place-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserOtherPlaceListComponent
  extends BasePresentationComponent
  implements OnInit, OnChanges {
  @Input() set isClearSelected(newValue: boolean) {
    this._isClearSelectedValue = newValue;
    if (this._isClearSelectedValue && this.agGridConfig) {
      this.agGridConfig.selectedRows = [];
      this.agGridConfig.gridApi.deselectAll();
      this.changeDetectorRef.detectChanges();
    }
  }
  get isClearSelected(): boolean {
    return this._isClearSelectedValue;
  }
  @Input() currentUser: User;
  @Input() currentUserRoles: SystemRole[];
  @Input() tabLabel: string;
  @Input() userItemsData: UserManagement[];
  @Output() editUser: EventEmitter<any> = new EventEmitter<any>();
  @Output() sortChange: EventEmitter<SortModel> = new EventEmitter<SortModel>();
  @Output()
  userActions: EventEmitter<UserActionsModel> = new EventEmitter<UserActionsModel>();
  @Output() selectedUser: EventEmitter<UserManagement[]> = new EventEmitter<
    UserManagement[]
  >();
  @Output()
  singleAction: EventEmitter<ActionsModel> = new EventEmitter<ActionsModel>();
  agGridConfig: AgGridConfigModel;
  currentSort: SortModel;
  userStatusDisplayAction: UserStatusDisplayActionModel;
  isOtherPlaceOfWorkTab: boolean = true;
  private _isClearSelectedValue: boolean;
  private currentSelectedRows: number = 0;

  constructor(
    private translateAdapterService: TranslateAdapterService,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);
  }

  @HostListener('window:resize')
  onResize(): void {
    if (!this.agGridConfig) {
      return;
    }
    const timeoutForResize = 300;
    setTimeout(() => {
      this.agGridConfig.gridApi.sizeColumnsToFit();
    }, timeoutForResize);
  }

  ngOnInit(): void {
    this.currentSort = new SortModel({
      currentFieldSort: 'firstName',
      currentSortType: 'asc'
    });

    this.initGridConfig();
    if (this.userItemsData && this.userItemsData) {
      this.agGridConfig.rowData = this.userItemsData;
    }

    const hasPermissionToCreateOrgUnit = this.currentUser.hasPermission(
      SAM_PERMISSIONS.CreateOrganisationUnitInOtherPlaceOfWork
    );

    const actions = new UserActionsModel();
    actions.listEssentialActions = hasPermissionToCreateOrgUnit
      ? [this.getCreateOrgUnitAction()]
      : [];

    this.userActions.emit(actions);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.agGridConfig && this.userItemsData) {
      this.agGridConfig.rowData = this.userItemsData;
      this.agGridConfig.gridApi.sizeColumnsToFit();
    }
    if (
      (changes.userItemsData &&
        changes.userItemsData.currentValue &&
        changes.userItemsData.previousValue &&
        changes.userItemsData.currentValue.length !==
          changes.userItemsData.previousValue.length) ||
      (changes.isClearSelected &&
        changes.isClearSelected.currentValue &&
        changes.isClearSelected.previousValue &&
        this.agGridConfig)
    ) {
      this.agGridConfig.selectedRows = [];
      this.agGridConfig.gridApi.deselectAll();
      this.changeDetectorRef.detectChanges();
    }
  }

  initGridConfig(): void {
    const frameworkComponents = {
      cellUserInfo: CellUserInfoComponent,
      cellUserStatus: CellUserStatusComponent,
      cellDropdownMenu: CellDropdownOtherPlaceOfWorkActionsComponent
    };

    this.agGridConfig = new AgGridConfigModel({
      columnDefs: this.setColumnDef(),
      frameworkComponents,
      rowData: [],
      selectedRows: [],
      context: { componentParent: this },
      defaultColDef: {
        resizable: true
      }
    });
  }

  onSelectionChanged(event: any): void {
    this.agGridConfig.selectedRows = event.api.getSelectedRows();
    const displayedRowCount: number = event.api.getDisplayedRowCount();
    const newSelectedRows = event.api.getSelectedRows().length;
    // This block will be called whenever user click tab user-list
    // Then we need to stop emit user-action to avoid duplicate emit
    if (this.isClearSelected && newSelectedRows === 0) {
      this.isClearSelected = false;

      return;
    }

    if (
      this.currentSelectedRows > 0 &&
      this.currentSelectedRows < displayedRowCount &&
      newSelectedRows === displayedRowCount &&
      this.currentSelectedRows !== displayedRowCount - 1
    ) {
      event.api.deselectAll();
    }
    const createNewOrgUnitAction = this.getCreateOrgUnitAction();
    let actions = new UserActionsModel({
      listEssentialActions: [createNewOrgUnitAction]
    });
    const selectedRows = this.agGridConfig.selectedRows;
    this.currentSelectedRows = selectedRows.length;

    if (this.currentSelectedRows > 0) {
      const changePlaceOfWorkAction = this.getChangePlaceOfWorkAction();
      const rejectAction = this.getRejectAction();
      const isHasPendingUser = this.checkIsPendingUsers(selectedRows);
      actions.listNonEssentialActions = isHasPendingUser
        ? [rejectAction, changePlaceOfWorkAction]
        : [changePlaceOfWorkAction];
    }

    actions = this.filterSatisfiedActionsByPermissions(actions);

    this.userActions.emit(actions);
    this.selectedUser.emit(this.agGridConfig.selectedRows);
    this.changeDetectorRef.detectChanges();
  }

  isAllowToEditUser(): boolean {
    return this.currentUser.hasPermission(SAM_PERMISSIONS.EditOtherPlaceOfWork);
  }

  onGridReady(params: any): void {
    this.agGridConfig.gridApi = params.api;
    this.agGridConfig.gridColumnApi = params.columnApi;
    this.agGridConfig.gridApi.setDomLayout('autoHeight');
    this.agGridConfig.gridApi.sizeColumnsToFit();
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
      this.currentSort.currentSortType = sortModel.sort;
      this.currentSort.currentFieldSort = sortModel.colId;
      this.sortChange.emit(this.currentSort);
    }
  }

  onEditUserClicked($event: any): void {
    this.editUser.emit($event.data);
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

  private filterSatisfiedListActionsByPermissions(
    actions: ActionsModel[]
  ): ActionsModel[] {
    const filteredActions: ActionsModel[] = [];

    let isAllowed: boolean;

    actions.forEach((action) => {
      switch (action.actionType) {
        case StatusActionTypeEnum.CreateNewOrgUnit:
          isAllowed = this.currentUser.hasPermission(
            SAM_PERMISSIONS.CreateOrganisationUnitInOtherPlaceOfWork
          );
          break;
        case StatusActionTypeEnum.Reject:
          isAllowed = this.currentUser.hasPermission(
            SAM_PERMISSIONS.RejectInOtherPlaceOfWork
          );
          break;
        case StatusActionTypeEnum.ChangeUserPlaceOfWork:
          isAllowed = this.currentUser.hasPermission(
            SAM_PERMISSIONS.ChangePlaceOfWorkInOtherPlaceOfWork
          );
          break;
        default:
          isAllowed = true;
          break;
      }

      if (isAllowed) {
        filteredActions.push(action);
      }
    });

    return filteredActions;
  }

  private getChangePlaceOfWorkAction(): ActionsModel {
    const changePlaceOfWorkActionMapped = USER_ACTION_MAPPING_CONST.find(
      (action) =>
        action.targetAction === StatusActionTypeEnum.ChangeUserPlaceOfWork
    );
    const changePlaceOfWorkActionTarget =
      changePlaceOfWorkActionMapped.targetAction;
    const changePlaceOfWorkActionText = this.getActionText(
      changePlaceOfWorkActionTarget
    );

    return new ActionsModel({
      actionType: changePlaceOfWorkActionMapped.targetAction,
      icon: changePlaceOfWorkActionMapped.targetIcon,
      message: changePlaceOfWorkActionMapped.message,
      text: changePlaceOfWorkActionText
    });
  }

  private getCreateOrgUnitAction(): ActionsModel {
    const createNewOrgUnitActionMapped = USER_ACTION_MAPPING_CONST.find(
      (action) => action.targetAction === StatusActionTypeEnum.CreateNewOrgUnit
    );
    const createNewOrgUnitActionTarget =
      createNewOrgUnitActionMapped.targetAction;
    const createNewOrgUnitActionText = this.getActionText(
      createNewOrgUnitActionTarget
    );

    return new ActionsModel({
      actionType: createNewOrgUnitActionMapped.targetAction,
      icon: 'icon-add',
      message: createNewOrgUnitActionMapped.message,
      text: createNewOrgUnitActionText
    });
  }

  private getRejectAction(): ActionsModel {
    const rejectActionMapped = USER_ACTION_MAPPING_CONST.find(
      (action) => action.targetAction === StatusActionTypeEnum.Reject
    );

    const rejectActionTarget = rejectActionMapped.targetAction;
    const rejectActionText = this.getActionText(rejectActionTarget);

    return new ActionsModel({
      actionType: rejectActionMapped.targetAction,
      icon: rejectActionMapped.targetIcon,
      message: rejectActionMapped.message,
      text: rejectActionText
    });
  }

  private getActionText(targetAction: string): string {
    return this.translateAdapterService.getValueImmediately(
      `User_Account_Page.User_Context_Menu.${targetAction}`
    );
  }

  private setColumnDef(): ColDef[] {
    return [
      {
        headerName: 'Name',
        field: 'firstName',
        minWidth: 300,
        sortable: true,
        suppressSizeToFit: false,
        checkboxSelection: true,
        headerCheckboxSelection: true,
        cellRenderer: 'cellUserInfo'
      },
      {
        headerName: 'Status',
        cellRenderer: 'cellUserStatus',
        minWidth: 200,
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
      },
      {
        headerName: 'Place of Work Name',
        cellRenderer: this.getOrgUnitNameFromAgGridParam,
        minWidth: 300,
        suppressSizeToFit: false,
        suppressMenu: true
      },
      {
        headerName: 'Place of Work Address',
        cellRenderer: this.getOrgUnitAddressFromAgGridParam,
        minWidth: 300,
        suppressSizeToFit: false,
        suppressMenu: true
      },
      {
        headerName: 'Application Date',
        minWidth: 300,
        suppressSizeToFit: false,
        suppressMenu: true,
        cellRenderer: (eventData) => {
          const user = eventData.data;
          const applicationDate = user
            ? DateTimeUtil.toDateString(
                eventData.data.created,
                AppConstant.fullDateTimeFormat
              )
            : 'N/A';

          return applicationDate;
        }
      },
      {
        headerName: '',
        cellRenderer: 'cellDropdownMenu',
        maxWidth: 50,
        minWidth: 50,
        sortable: false,
        suppressSizeToFit: false,
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
      }
    ];
  }

  private onActionClicked($event: any): void {
    this.singleAction.emit($event);
  }

  private getOrgUnitAddressFromAgGridParam(param: {
    data: UserManagement;
  }): string {
    if (!param || !param.data || !param.data.jsonDynamicAttributes) {
      return;
    }

    return (
      param.data.jsonDynamicAttributes.manualOrganizationUnitAddress || 'N/A'
    );
  }

  private getOrgUnitNameFromAgGridParam(param: {
    data: UserManagement;
  }): string {
    if (!param || !param.data || !param.data.jsonDynamicAttributes) {
      return;
    }

    return param.data.jsonDynamicAttributes.manualOrganizationUnitName || 'N/A';
  }

  private checkIsPendingUsers(selectedItems: UserManagement[]): boolean {
    const rejectStatuses = [
      StatusTypeEnum.PendingApproval1st.code,
      StatusTypeEnum.PendingApproval2nd.code,
      StatusTypeEnum.PendingApproval3rd.code
    ];

    const pendingUser = selectedItems.find(
      (user: UserManagement) =>
        user.entityStatus && rejectStatuses.includes(user.entityStatus.statusId)
    );

    return !!pendingUser;
  }
}
