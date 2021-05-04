import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  ViewEncapsulation
} from '@angular/core';
import { GridApi } from 'ag-grid-community';
import {
  AgGridConfigModel,
  ColumDefModel
} from 'app-models/ag-grid-config.model';
import { User } from 'app-models/auth.model';
import { SortModel } from 'app-models/sort.model';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { SystemRole } from 'app/core/models/system-role';
import { CommonHelpers } from 'app/shared/common.helpers';
import { BasePresentationComponent } from 'app/shared/components/component.abstract';
import { initTaxonomyUserActions } from 'app/user-accounts/models/user-action-mapping';
import { UserManagement } from 'app/user-accounts/models/user-management.model';
import { UserStatusDisplayActionModel } from 'app/user-accounts/user-list/models/user-selected-status.model';
import { Utils } from '../../shared/utilities/utils';
import { CellDropdownMetadataListActionsComponent } from '../cell-components/cell-dropdown-metadata-list-actions/cell-dropdown-metadata-list-actions.component';
import { CellRequestStatusComponent } from '../cell-components/cell-request-status/cell-request-status.component';
import { CellRequestedDateComponent } from '../cell-components/cell-requested-date/cell-requested-date.component';
import { TaxonomyCellUserInfoComponent } from '../cell-components/cell-user-info/taxonomy-cell-user-info.component';
import { TAXONOMY_LIST_HEADER_CONST } from '../constant/taxonomy-list-header.const';
import { TaxonomyRequestStatusLabel } from '../constant/taxonomy-request-status-label.enum';
import { TaxonomyRequestStatus } from '../constant/taxonomy-request-status.enum';
import { TaxonomyActionToolbarModel } from '../models/actions.model';
import { TaxonomyRequest } from '../models/taxonomy-request.model';
import { TaxonomyRequestViewModel } from '../models/taxonomy-request.viewmodel';
import { SAM_PERMISSIONS } from '../../shared/constants/sam-permission.constant';

@Component({
  selector: 'taxonomy-management-list',
  templateUrl: './taxonomy-management-list.component.html',
  styleUrls: ['./taxonomy-management-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaxonomyManagementListComponent
  extends BasePresentationComponent
  implements OnInit, OnChanges {
  agGridConfig: AgGridConfigModel;
  currentSort: SortModel;
  userStatusDisplayAction: UserStatusDisplayActionModel;

  isVerticalToShowMenuAction: boolean = true;

  @Input() currentUser: User;
  @Input() taxonomyItemsData: TaxonomyRequestViewModel[];
  @Input() taxonomyRequestedByUserInfo: UserManagement[];
  @Input() currentUserRoles: SystemRole[];
  @Input() currentTab: TaxonomyRequestStatusLabel;
  @Input() searchText: string;

  @Output()
  editMetadataRequest: EventEmitter<TaxonomyRequestViewModel> = new EventEmitter<TaxonomyRequestViewModel>();

  @Output() gridApi: EventEmitter<GridApi> = new EventEmitter<GridApi>();
  @Output()
  taxonomyActions: EventEmitter<TaxonomyActionToolbarModel> = new EventEmitter<TaxonomyActionToolbarModel>();
  @Output() singleAction: EventEmitter<any> = new EventEmitter<any>();
  @Output() selectedMetadatas: EventEmitter<
    TaxonomyRequest[]
  > = new EventEmitter<TaxonomyRequest[]>();
  @Output() sortChange: EventEmitter<SortModel> = new EventEmitter<SortModel>();

  private gridSetting: any;

  private currentSelectedRows: number = 0;

  constructor(
    private translateAdapterService: TranslateAdapterService,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    this.initGridConfig('box-icon');

    this.agGridConfig.rowData = this.taxonomyItemsData;

    this.taxonomyActions.emit(
      initTaxonomyUserActions(
        this.translateAdapterService,
        this.currentTab,
        this.checkIfHasPermissionToApprove(),
        this.checkIfHasPermissionToReject(),
        this.checkIfHasPermissionToComplete()
      )
    );

    this.addScrollEventListeners();
  }

  ngOnChanges(): void {
    if (!this.agGridConfig) {
      return;
    }
    this.agGridConfig.rowData = this.taxonomyItemsData;

    if (this.agGridConfig.gridApi) {
      this.agGridConfig.gridApi.sizeColumnsToFit();
    }
  }

  onGridReady(params: any): void {
    this.gridSetting = params;

    this.emitColumnShowHideDefAndGridApi();
  }

  onSortChange(event: any): void {
    const sortModel = event.api.getSortModel().shift();

    if (sortModel) {
      this.currentSort.currentSortType = sortModel.sort;
      this.currentSort.currentFieldSort = sortModel.colId;
      this.sortChange.emit(this.currentSort);
    }
  }

  emitColumnShowHideDefAndGridApi(): void {
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
      }
    }

    this.gridApi.emit(this.agGridConfig.gridApi);
  }

  initGridConfig(menuHeaderIconDefault: string): void {
    this.agGridConfig = new AgGridConfigModel({
      columnDefs: this.setColumnDef(menuHeaderIconDefault),
      frameworkComponents: {
        cellUserInfo: TaxonomyCellUserInfoComponent,
        cellRequestStatus: CellRequestStatusComponent,
        cellRequestedDate: CellRequestedDateComponent,
        cellDropdownMetadataListActions: CellDropdownMetadataListActionsComponent
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

  onCellClick($event: any): void {
    if ($event == null || $event.data == null || this.isLastColumn($event)) {
      return;
    }

    const rowData = Utils.cloneDeep($event.data) as TaxonomyRequestViewModel;

    this.editMetadataRequest.emit(rowData);
  }

  onActionClicked($event: any): void {
    this.singleAction.emit($event);
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  onSelectionChanged(event: any): void {
    // This block will be called whenever user click another tab
    // Then we need to stop emit taxonomy-action to avoid duplicate emit

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
    const actions: TaxonomyActionToolbarModel = initTaxonomyUserActions(
      this.translateAdapterService,
      this.currentTab,
      this.checkIfHasPermissionToApprove(),
      this.checkIfHasPermissionToReject(),
      this.checkIfHasPermissionToComplete()
    );

    this.taxonomyActions.emit(actions);

    this.selectedMetadatas.emit(selectedRows);
    this.changeDetectorRef.detectChanges();
  }

  setColumnDef(menuHeaderIcon: string): ColumDefModel[] {
    const nonAcceptedRequestStatus = [
      TaxonomyRequestStatus.RejectLevel1,
      TaxonomyRequestStatus.RejectLevel2,
      TaxonomyRequestStatus.Completed
    ];
    const representRequestStatus = this.taxonomyItemsData[0]
      ? this.taxonomyItemsData[0].status
      : null;

    const columnDef = [
      new ColumDefModel({
        field: TAXONOMY_LIST_HEADER_CONST.CustomMetadataRequestId.fieldName,
        colId: TAXONOMY_LIST_HEADER_CONST.CustomMetadataRequestId.colId,
        hide: true
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          TAXONOMY_LIST_HEADER_CONST.Title.text
        ),
        field: TAXONOMY_LIST_HEADER_CONST.Title.fieldName,
        colId: TAXONOMY_LIST_HEADER_CONST.Title.colId,
        minWidth: 200,
        checkboxSelection:
          !nonAcceptedRequestStatus.includes(representRequestStatus) &&
          (this.checkIfHasPermissionToApprove() ||
            this.checkIfHasPermissionToReject() ||
            this.checkIfHasPermissionToComplete()),
        headerCheckboxSelection:
          !nonAcceptedRequestStatus.includes(representRequestStatus) &&
          (this.checkIfHasPermissionToApprove() ||
            this.checkIfHasPermissionToReject() ||
            this.checkIfHasPermissionToComplete()),
        sortable: true,
        cellStyle: {}
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          TAXONOMY_LIST_HEADER_CONST.TypeOfMetadata.text
        ),
        field: TAXONOMY_LIST_HEADER_CONST.TypeOfMetadata.fieldName,
        colId: TAXONOMY_LIST_HEADER_CONST.TypeOfMetadata.colId,
        tooltipField: TAXONOMY_LIST_HEADER_CONST.TypeOfMetadata.fieldName,
        minWidth: 200,
        sortable: false,
        suppressMenu: true,
        autoHeight: false,

        hide: false
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          TAXONOMY_LIST_HEADER_CONST.Action.text
        ),
        field: TAXONOMY_LIST_HEADER_CONST.Action.fieldName,
        colId: TAXONOMY_LIST_HEADER_CONST.Action.colId,
        minWidth: 60,
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
          TAXONOMY_LIST_HEADER_CONST.RequestedBy.text
        ),
        field: TAXONOMY_LIST_HEADER_CONST.RequestedBy.fieldName,
        colId: TAXONOMY_LIST_HEADER_CONST.RequestedBy.colId,
        minWidth: 250,
        sortable: false,
        suppressMenu: true,
        cellStyle: {},
        hide: false,
        cellRenderer: 'cellUserInfo',
        cellRendererParams: {
          metadataRequestedByUserInfo: this.taxonomyRequestedByUserInfo
        }
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          TAXONOMY_LIST_HEADER_CONST.RequestedDate.text
        ),
        field: TAXONOMY_LIST_HEADER_CONST.RequestedDate.fieldName,
        colId: TAXONOMY_LIST_HEADER_CONST.RequestedDate.colId,
        cellRenderer: 'cellRequestedDate',
        minWidth: 160,
        sortable: false,
        suppressMenu: true,
        cellRendererParams: {}
      }),
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          TAXONOMY_LIST_HEADER_CONST.Status.text
        ),
        field: TAXONOMY_LIST_HEADER_CONST.Status.fieldName,
        colId: TAXONOMY_LIST_HEADER_CONST.Status.colId,
        cellRenderer: 'cellRequestStatus',
        minWidth: 180,
        sortable: false,
        suppressMenu: true,
        hide: false,
        cellRendererParams: {}
      })
    ];

    if (
      !nonAcceptedRequestStatus.includes(representRequestStatus) &&
      (this.hasPermissionToEdit() ||
        this.checkIfHasPermissionToApprove() ||
        this.checkIfHasPermissionToReject() ||
        this.checkIfHasPermissionToComplete())
    ) {
      columnDef.push(
        new ColumDefModel({
          headerName: '',
          cellRenderer: 'cellDropdownMetadataListActions',
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
            hasPermissionToEdit: this.hasPermissionToEdit(),
            hasPermissionToApprove: this.checkIfHasPermissionToApprove(),
            hasPermissionToReject: this.checkIfHasPermissionToReject(),
            hasPermissionToComplete: this.checkIfHasPermissionToComplete(),
            onClick: this.onActionClicked.bind(this)
          }
        })
      );
    }

    return columnDef;
  }

  private hasPermissionToEdit(): boolean {
    switch (this.currentTab) {
      case TaxonomyRequestStatusLabel.PendingLevel1:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataEditPending1st
        );
      case TaxonomyRequestStatusLabel.PendingLevel2:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataEditPending2nd
        );
      default:
        return false;
    }
  }

  private addScrollEventListeners(): void {
    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
    window.addEventListener('scroll', CommonHelpers.freezeAgGridScroll(), true);
    window.addEventListener('scroll', CommonHelpers.freezeMenuActions(), true);
  }

  private getImmediatelyLanguage(columnName: string): string {
    return this.translateAdapterService.getValueImmediately(
      TAXONOMY_LIST_HEADER_CONST.GroupKey.text + '.' + columnName
    );
  }

  private isLastColumn(params: any): boolean {
    if (!params.columnApi) {
      return false;
    }
    const displayedColumns = params.columnApi.getAllDisplayedColumns();

    return displayedColumns[displayedColumns.length - 1] === params.column;
  }

  private checkIfHasPermissionToApprove(): boolean {
    switch (this.currentTab) {
      case TaxonomyRequestStatusLabel.PendingLevel1:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataApprovePending1st
        );
      case TaxonomyRequestStatusLabel.PendingLevel2:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataApprovePending2nd
        );
      default:
        return false;
    }
  }

  private checkIfHasPermissionToReject(): boolean {
    switch (this.currentTab) {
      case TaxonomyRequestStatusLabel.PendingLevel1:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataRejectPending1st
        );
      case TaxonomyRequestStatusLabel.PendingLevel2:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataRejectPending2nd
        );
      default:
        return false;
    }
  }

  private checkIfHasPermissionToComplete(): boolean {
    return (
      this.currentTab === TaxonomyRequestStatusLabel.Approved &&
      this.currentUser.hasPermission(
        SAM_PERMISSIONS.MetadataMarkAsCompleteInApprovedList
      )
    );
  }
}
