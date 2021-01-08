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
import {
  AgGridConfigModel,
  ColumDefModel
} from 'app-models/ag-grid-config.model';
import { SortModel } from 'app-models/sort.model';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { AppConstant } from 'app/shared/app.constant';
import { CellDropdownActionComponent } from 'app/shared/components/cell-dropdown-action/cell-dropdown-action.component';
import { BasePresentationComponent } from 'app/shared/components/component.abstract';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import * as _ from 'lodash';
import * as moment from 'moment';

import { GridApi } from '@ag-grid-community/core';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { SystemRole } from 'app/core/models/system-role';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import {
  initPendingUserActions,
  initUserActions
} from '../models/user-action-mapping';
import { UserManagement } from '../models/user-management.model';
import { UserAccountsHelper } from '../user-accounts.helper';
import { ActionsModel } from '../user-actions/models/actions.model';
import { UserActionsModel } from '../user-actions/models/user-actions.model';
import { CellUserStatusComponent } from '../user-list/cell-components/cell-user-status/cell-user-status.component';
import { UserStatusDisplayActionModel } from '../user-list/models/user-selected-status.model';
import { CellUserPendingInfoComponent } from './cell-components/cell-user-pending-info/cell-user-pending-info.component';
import { USER_PENDING_LIST_HEADER_CONST } from './models/user-pending-list.const';
import { CommonHelpers } from 'app/shared/common.helpers';

@Component({
  selector: 'user-pending-list',
  templateUrl: './user-pending-list.component.html',
  styleUrls: ['./user-pending-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserPendingListComponent
  extends BasePresentationComponent
  implements OnInit, OnChanges {
  agGridConfig: AgGridConfigModel;
  currentSort: SortModel;
  userStatusDisplayAction: UserStatusDisplayActionModel;
  get isClearSelected(): any {
    return this._isClearSelectedlue;
  }
  @Input() set isClearSelected(newValue: any) {
    this._isClearSelectedlue = newValue;
    if (this._isClearSelectedlue && this.agGridConfig) {
      this.agGridConfig.selectedRows = [];
      this.agGridConfig.gridApi.deselectAll();
      this.changeDetectorRef.detectChanges();
    }
  }
  @Input() currentUserRoles: SystemRole[];
  @Input() isCurrentUserDivAdmin: boolean;
  @Input() isCurrentUserAccountAdmin: boolean;
  @Input() isCurrentUserSuperAdmin: boolean;
  @Input() viewOnly: boolean;
  @Input() userItemsData: UserManagement[];
  @Input() reloadPendingTab: boolean = false;

  @Output() editUser: EventEmitter<any> = new EventEmitter<any>();
  @Output() sortChange: EventEmitter<SortModel> = new EventEmitter<SortModel>();
  @Output()
  userActions: EventEmitter<UserActionsModel> = new EventEmitter<UserActionsModel>();
  @Output() selectedUser: EventEmitter<UserManagement[]> = new EventEmitter<
    UserManagement[]
  >();
  @Output()
  singleAction: EventEmitter<ActionsModel> = new EventEmitter<ActionsModel>();
  @Output() columnShowHideDef: EventEmitter<ColumDefModel[]> = new EventEmitter<
    ColumDefModel[]
  >();
  @Output() gridApi: EventEmitter<GridApi> = new EventEmitter<GridApi>();
  private _isClearSelectedlue: any;
  private userActionMapping: any;
  private currentSelectedRows: number = 0;

  private gridSetting: any;

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
    let frameworkComponents = {};
    if (this.viewOnly) {
      frameworkComponents = {
        cellUserInfo: CellUserPendingInfoComponent,
        cellUserStatus: CellUserStatusComponent
      };
    } else {
      frameworkComponents = {
        cellUserInfo: CellUserPendingInfoComponent,
        cellUserStatus: CellUserStatusComponent,
        cellDropdownMenu: CellDropdownActionComponent
      };
    }

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

    this.currentSelectedRows = event.api.getSelectedRows().length;
    const actions: UserActionsModel = this.filterActionBasedOnStatus(
      this.agGridConfig.selectedRows
    );

    if (this.currentSelectedRows > 0) {
      actions.listEssentialActions = initPendingUserActions(
        this.translateAdapterService,
        this.isCurrentUserDivAdmin
      );
      this.userActions.emit(actions);
    } else {
      this.userActions.emit(null);
    }

    this.selectedUser.emit(this.agGridConfig.selectedRows);
    this.changeDetectorRef.detectChanges();
  }

  setColumnDef(): ColumDefModel[] {
    const columnDefs = [
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_PENDING_LIST_HEADER_CONST.Name.text
        ),
        field: USER_PENDING_LIST_HEADER_CONST.Name.fieldName,
        colId: USER_PENDING_LIST_HEADER_CONST.Name.colId,
        minWidth: 300,
        checkboxSelection: !this.viewOnly,
        headerCheckboxSelection: !this.viewOnly,
        sortable: true,
        cellStyle: {},
        suppressSizeToFit: false,
        cellRenderer: 'cellUserInfo'
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_PENDING_LIST_HEADER_CONST.Email.text
        ),
        field: USER_PENDING_LIST_HEADER_CONST.Email.fieldName,
        colId: USER_PENDING_LIST_HEADER_CONST.Email.colId,
        minWidth: 300,
        sortable: true,
        suppressSizeToFit: false,
        suppressMenu: true
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_PENDING_LIST_HEADER_CONST.DepartmentName.text
        ),
        field: USER_PENDING_LIST_HEADER_CONST.DepartmentName.fieldName,
        colId: USER_PENDING_LIST_HEADER_CONST.DepartmentName.colId,
        minWidth: 200,
        sortable: true,
        suppressSizeToFit: false,
        suppressMenu: true
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_PENDING_LIST_HEADER_CONST.Created.text
        ),
        field: USER_PENDING_LIST_HEADER_CONST.Created.fieldName,
        colId: USER_PENDING_LIST_HEADER_CONST.Created.colId,
        minWidth: 200,
        sortable: true,
        suppressSizeToFit: false,
        suppressMenu: true,
        valueFormatter: (params: any) => {
          return params && params.data && params.data.created
            ? moment(params.data.created).format(AppConstant.fullDateTimeFormat)
            : '';
        }
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_PENDING_LIST_HEADER_CONST.Status.text
        ),
        cellRenderer: 'cellUserStatus',
        minWidth: 200,
        sortable: true,
        suppressSizeToFit: false,
        suppressMenu: true,
        cellStyle: { overflow: 'visible', 'z-index': '9999' },
        cellRendererParams: {},
        headerClass: 'grid-header-centered',
        cellClass: 'grid-cell-centered'
      })
    ];

    if (!this.viewOnly) {
      columnDefs.push(
        new ColumDefModel({
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
          colId: USER_PENDING_LIST_HEADER_CONST.Action.colId,
          cellRendererParams: {
            onClick: this.onActionClicked.bind(this)
          }
        })
      );
    }

    return columnDefs;
  }

  onGridReady(params: any): void {
    this.gridSetting = params;

    this.agGridConfig.gridApi = this.gridSetting.api;
    this.agGridConfig.gridColumnApi = this.gridSetting.columnApi;
    this.agGridConfig.gridApi.setDomLayout('autoHeight');
    this.agGridConfig.gridApi.sizeColumnsToFit();
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  changeSelectedColumn(show: boolean, column: any): void {
    if (column && column.colId) {
      this.agGridConfig.gridColumnApi.setColumnVisible(column.colId, show);
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
    if (
      $event.action.actionType === StatusActionTypeEnum.Accept &&
      $event.item.entityStatus.statusId ===
        StatusTypeEnum.PendingApproval1st.code
    ) {
      $event.action.message = '';
    }
    this.singleAction.emit($event);
  }

  private getImmediatelyLanguage(columnName: string): string {
    return this.translateAdapterService.getValueImmediately(
      USER_PENDING_LIST_HEADER_CONST.GroupKey.text + '.' + columnName
    );
  }

  private filterActionBasedOnStatus(
    selectedItems: UserManagement[]
  ): UserActionsModel {
    const uniqueStatusOfUser = _.uniqBy(
      selectedItems,
      'entityStatus.statusId'
    ).map((user: UserManagement) => {
      return user.entityStatus.statusId;
    });

    this.userActionMapping = UserAccountsHelper.getAccessibleUserActionMapping(
      this.currentUserRoles
    );

    const actCorrectWithUserActionMapping = uniqueStatusOfUser.map(
      (status: string) => {
        return this.userActionMapping
          .filter((item: any) => {
            return (
              item.currentStatus.findIndex(
                (stt: string) => stt.toString() === status
              ) > findIndexCommon.notFound &&
              item.targetAction !== StatusActionTypeEnum.Edit
            );
          })
          .map((stt: any) => stt.targetAction);
      }
    );

    const actionsModel = _.intersection(...actCorrectWithUserActionMapping);

    return new UserActionsModel({
      listNonEssentialActions: this.getUserAction(actionsModel, true),
      listSpecifyActions: initUserActions(this.translateAdapterService)
        .listSpecifyActions
    });
  }

  private getUserAction(
    actionsModel: any,
    isGetSimpleAction: boolean = false
  ): any {
    const userActionList = [];
    if (actionsModel && !actionsModel.length) {
      if (!this.viewOnly) {
        this.changeSelectedColumn(
          this.agGridConfig.selectedRows.length <= 1,
          USER_PENDING_LIST_HEADER_CONST.Action
        );
      }

      return;
    }

    actionsModel.forEach((targetAction: string) => {
      const actionsMapped = this.userActionMapping.find((item: any) => {
        return item.targetAction === targetAction;
      });

      if (actionsMapped) {
        const action = new ActionsModel({
          actionType: actionsMapped.targetAction,
          icon: actionsMapped.targetIcon,
          message: actionsMapped.message
        });
        if (!this.viewOnly) {
          this.changeSelectedColumn(
            this.agGridConfig.selectedRows.length <= 1,
            USER_PENDING_LIST_HEADER_CONST.Action
          );
        }
        if (actionsMapped.targetAction === StatusActionTypeEnum.Accept) {
          action.text = this.translateAdapterService.getValueImmediately(
            `User_Account_Page.User_Context_Menu.${
              this.isCurrentUserDivAdmin ? 'Endorse' : 'Approve'
            }`
          );
        } else {
          action.text = this.translateAdapterService.getValueImmediately(
            `User_Account_Page.User_Context_Menu.${actionsMapped.targetAction}`
          );
        }

        if (isGetSimpleAction) {
          if (
            actionsMapped.targetAction !== StatusActionTypeEnum.Accept &&
            actionsMapped.targetAction !== StatusActionTypeEnum.Reject
          ) {
            userActionList.push(action);
          }
        }
      }
    });

    return userActionList;
  }
}
