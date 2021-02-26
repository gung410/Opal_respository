import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  SimpleChanges,
  ViewEncapsulation
} from '@angular/core';
import { Router } from '@angular/router';
import { CxTableIcon } from '@conexus/cx-angular-common';
import { ColDef } from 'ag-grid-community';
import { AgGridConfigModel } from 'app-models/ag-grid-config.model';
import { SortModel } from 'app-models/sort.model';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { StringUtil } from 'app-utilities/string-utils';
import { CommonHelpers } from 'app/shared/common.helpers';
import { BasePresentationComponent } from 'app/shared/components/component.abstract';
import { BroadcastMessagesGridHeader } from 'app/shared/constants/broadcast-message-grid-header.enum';
import { UserManagement } from 'app/user-accounts/models/user-management.model';
import { CellBroadcastMessageUserInfoComponent } from '../cell-components/cell-broadcast-message-user-info/cell-broadcast-message-user-info.component';
import { CellBroadcastMessageStatusComponent } from '../cell-components/cell-user-status/cell-broadcast-message-status.component';
import { ActionsItemModel } from '../events/action-items.model';
import {
  BroadcastMessagesDto,
  BroadcastMessagesFilterParams
} from '../models/broadcast-messages.model';
import { BroadcastMessageViewModel } from '../models/broadcast-messages.view.model';
import { BROADCAST_MESSAGE_STATUS } from './../../shared/constants/broadcast-message-status.constant';
import { CellBroadcastDropdownMenuComponent } from './cell-dropdown-menu/cell-broadcast-dropdown-menu.component';

@Component({
  selector: 'broadcast-messages-list',
  templateUrl: './broadcast-messages-list.component.html',
  styleUrls: ['./broadcast-messages-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BroadcastMessagesListComponent
  extends BasePresentationComponent
  implements OnInit {
  @Input() broadcastMessagesData: BroadcastMessagesDto[] = [];
  @Input() broadcastMessageOwners: UserManagement[] = [];
  @Input() isCurrentUserAllowToMakeActions: boolean;

  userDepartmentId: number;
  currentDepartmentId: number = -1;
  currentSort: SortModel = new SortModel();
  filterParams: BroadcastMessagesFilterParams;
  agGridConfig: AgGridConfigModel;
  tableIcon: CxTableIcon = new CxTableIcon({
    removeClass: 'material-icons remove-icon',
    moreActionsClass: 'material-icons more-vert'
  });

  @Output() singleAction: EventEmitter<
    ActionsItemModel<BroadcastMessageViewModel>
  > = new EventEmitter<ActionsItemModel<BroadcastMessageViewModel>>();

  @Output()
  editBroadcastMessages: EventEmitter<BroadcastMessageViewModel> = new EventEmitter<BroadcastMessageViewModel>();

  @Output() sortChange: EventEmitter<SortModel> = new EventEmitter<SortModel>();

  constructor(
    private translateAdapterService: TranslateAdapterService,
    private router: Router,
    private toastrService: ToastrAdapterService,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    this.initGridConfig();
    this.initGridData();

    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
    window.addEventListener('scroll', CommonHelpers.freezeAgGridScroll(), true);
    window.addEventListener('scroll', CommonHelpers.freezeMenuActions(), true);
  }

  onGridReady(params: any): void {
    if (!params) {
      return;
    }
    this.agGridConfig.gridApi = params.api;
    this.agGridConfig.gridColumnApi = params.columnApi;
    this.agGridConfig.gridApi.setDomLayout('autoHeight');
    this.agGridConfig.gridApi.sizeColumnsToFit();
    if (
      this.agGridConfig.gridColumnApi &&
      this.agGridConfig.gridColumnApi.columnController &&
      this.agGridConfig.columnShowHide &&
      this.agGridConfig.columnShowHide.length <= 0
    ) {
      this.agGridConfig.columnShowHide = this.agGridConfig.gridColumnApi.columnController.columnDefs;
      this.agGridConfig.columnShowHide.pop();
    }
  }

  onSortChange(event: any): void {
    const sortModel = event.api.getSortModel().shift();
    if (sortModel) {
      this.currentSort.currentSortType = StringUtil.capitalize(sortModel.sort);
      this.currentSort.currentFieldSort = StringUtil.capitalize(
        sortModel.colId
      );
      this.sortChange.emit(this.currentSort);
    }
  }

  createNewBroadcastMessage(): void {
    this.router.navigate(['/broadcast-messages/detail/', '']);
  }

  onEditBroadcastMessagesClicked($event: any): void {
    if (!this.isCurrentUserAllowToMakeActions) {
      return;
    }

    const broadcastMessageVM: BroadcastMessageViewModel = $event
      ? $event.data
      : null;

    if (
      this.isLastColumn($event) ||
      (broadcastMessageVM &&
        (broadcastMessageVM.status === BROADCAST_MESSAGE_STATUS.DEACTIVATE ||
          this.isBroadcastMessageExpired(broadcastMessageVM)))
    ) {
      this.toastrService.warning(
        'Can not edit deactivate or expired message.',
        'Alert'
      );

      return;
    }

    this.editBroadcastMessages.emit(broadcastMessageVM);
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      this.agGridConfig &&
      changes.broadcastMessagesData.currentValue !==
        changes.broadcastMessagesData.previousValue
    ) {
      this.agGridConfig.rowData = this.broadcastMessagesData;
      this.changeDetectorRef.detectChanges();
    }
  }

  private isLastColumn(params: any): boolean {
    if (!params.columnApi) {
      return false;
    }
    const displayedColumns = params.columnApi.getAllDisplayedColumns();

    return displayedColumns[displayedColumns.length - 1] === params.column;
  }

  private isBroadcastMessageExpired(
    broadcastMessageVM: BroadcastMessageViewModel
  ): boolean {
    return (
      DateTimeUtil.compareDate(
        broadcastMessageVM.validToDate,
        new Date(),
        true
      ) < 0
    );
  }

  private setColumnDef(): ColDef[] {
    const colsDef: ColDef[] = [
      {
        headerName: this.getImmediatelyLanguage(
          BroadcastMessagesGridHeader.BroadcastMessageId.text
        ),
        field: BroadcastMessagesGridHeader.BroadcastMessageId.fieldName,
        colId: BroadcastMessagesGridHeader.BroadcastMessageId.colId,
        hide: true
      },
      {
        headerName: this.getImmediatelyLanguage(
          BroadcastMessagesGridHeader.Title.text
        ),
        field: BroadcastMessagesGridHeader.Title.fieldName,
        colId: BroadcastMessagesGridHeader.Title.colId,
        minWidth: 200,
        sortable: true,
        tooltipField: BroadcastMessagesGridHeader.Title.fieldName,
        comparator: this.titleComparator.bind(this)
      },
      {
        headerName: this.getImmediatelyLanguage(
          BroadcastMessagesGridHeader.createdDate.text
        ),
        field: BroadcastMessagesGridHeader.createdDate.fieldName,
        colId: BroadcastMessagesGridHeader.createdDate.colId,
        minWidth: 200,
        sortable: true,
        suppressMenu: true,
        comparator: this.createdDateComparator.bind(this)
      },
      {
        headerName: this.getImmediatelyLanguage(
          BroadcastMessagesGridHeader.CreatedBy.text
        ),
        field: BroadcastMessagesGridHeader.CreatedBy.fieldName,
        colId: BroadcastMessagesGridHeader.CreatedBy.colId,
        minWidth: 150,
        sortable: false,
        suppressMenu: true,
        cellStyle: {},
        cellRenderer: 'cellBroadcastMessageUserInfo',
        cellRendererParams: {
          broadcastMessageOwners: this.broadcastMessageOwners
        },
        lockPinned: true
      },
      {
        headerName: this.getImmediatelyLanguage(
          BroadcastMessagesGridHeader.Recipients.text
        ),
        field: BroadcastMessagesGridHeader.Recipients.fieldName,
        colId: BroadcastMessagesGridHeader.Recipients.colId,
        minWidth: 200,
        sortable: false,
        suppressMenu: true
      },
      {
        headerName: this.getImmediatelyLanguage(
          BroadcastMessagesGridHeader.ValidFromDate.text
        ),
        field: BroadcastMessagesGridHeader.ValidFromDate.fieldName,
        colId: BroadcastMessagesGridHeader.ValidFromDate.colId,
        minWidth: 200,
        sortable: false,
        suppressMenu: true
      },
      {
        headerName: this.getImmediatelyLanguage(
          BroadcastMessagesGridHeader.ValidToDate.text
        ),
        field: BroadcastMessagesGridHeader.ValidToDate.fieldName,
        colId: BroadcastMessagesGridHeader.ValidToDate.colId,
        minWidth: 200,
        sortable: false,
        suppressMenu: true
      },
      {
        headerName: this.getImmediatelyLanguage(
          BroadcastMessagesGridHeader.Status.text
        ),
        field: BroadcastMessagesGridHeader.Status.fieldName,
        colId: BroadcastMessagesGridHeader.Status.colId,
        minWidth: 200,
        sortable: false,
        suppressMenu: true,
        cellRenderer: 'cellBroadcastMessageStatus',
        cellStyle: {
          overflow: 'visible',
          'z-index': '9999'
        },

        cellRendererParams: {},

        hide: false,
        headerClass: 'grid-header-centered',
        cellClass: 'grid-cell-centered'
      }
    ];

    if (this.isCurrentUserAllowToMakeActions) {
      colsDef.push({
        headerName: '',
        cellRenderer: 'cellDropdownMenu',
        minWidth: 50,
        maxWidth: 50,
        sortable: false,
        suppressSizeToFit: false,
        suppressMenu: true,
        pinned: 'right',
        cellStyle: {
          overflow: 'visible',
          'z-index': '9999',
          'padding-left': '10px',
          'max-width': '50px'
        },
        cellRendererParams: {
          onClick: this.onActionClicked.bind(this)
        }
      });
    }

    return colsDef;
  }

  private titleComparator = (title: string, comparedTitle: string): number => {
    return StringUtil.compareStringsCaseSensitive(title, comparedTitle);
  };

  private createdDateComparator(date: Date, comparedDate: Date): number {
    const dateNumber = DateTimeUtil.convertDateToComparableNumber(date);
    const comparedDateNumber = DateTimeUtil.convertDateToComparableNumber(
      comparedDate
    );
    if (!(dateNumber && comparedDateNumber)) {
      return 0;
    }
    if (!dateNumber) {
      return -1;
    }
    if (!comparedDateNumber) {
      return 1;
    }

    return dateNumber - comparedDateNumber;
  }

  private initGridConfig(): void {
    this.agGridConfig = new AgGridConfigModel({
      columnDefs: this.setColumnDef(),
      frameworkComponents: {
        cellDropdownMenu: CellBroadcastDropdownMenuComponent,
        cellBroadcastMessageStatus: CellBroadcastMessageStatusComponent,
        cellBroadcastMessageUserInfo: CellBroadcastMessageUserInfoComponent
      },
      rowData: [],
      selectedRows: [],
      context: { componentParent: this },
      defaultColDef: {
        resizable: true
      }
    });
  }

  private onActionClicked(
    $event: ActionsItemModel<BroadcastMessageViewModel>
  ): void {
    this.singleAction.emit($event);
  }

  private getImmediatelyLanguage(columnName: string): string {
    return this.translateAdapterService.getValueImmediately(
      'Broadcast_Message_Page.TableHeader.' + columnName
    );
  }

  private initGridData(): void {
    this.agGridConfig.rowData = this.broadcastMessagesData;
    this.changeDetectorRef.detectChanges();
  }
}
