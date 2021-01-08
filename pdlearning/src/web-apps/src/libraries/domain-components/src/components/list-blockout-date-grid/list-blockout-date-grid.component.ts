import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { BlockoutDateModel, UserInfoModel } from '@opal20/domain-api';
import { CellClickEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ContextMenuItem, OpalDialogService } from '@opal20/common-components';
import { Subscription, of } from 'rxjs';

import { BLOCKOUT_DATE_STATUS_COLOR_MAP } from './../../models/blockout-date-status-color-map.model';
import { BlockoutDateViewModel } from './../../models/blockout-date-view.model';
import { ContextMenuAction } from './../../models/context-menu-action.model';
import { ContextMenuEmit } from './../../models/context-menu-emit.model';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { IRowCallbackModel } from './../../models/row-callback.model';
import { ListBlockoutDateGridComponentService } from './../../services/list-blockout-date-grid-component.service';

@Component({
  selector: 'list-blockout-date-grid',
  templateUrl: './list-blockout-date-grid.component.html'
})
export class ListBlockoutDateGridComponent extends BaseGridComponent<BlockoutDateViewModel> {
  public loading: boolean;
  @Input() public coursePlanningCycleId?: string = null;
  @Input() public availableBlockOutDates: BlockoutDateViewModel[] = null;
  @Input()
  public set displayColumns(displayColumns: ListBlockoutDateGridDisplayColumns[]) {
    this._displayColumns = displayColumns;
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, _ => true);
  }
  @Input() public contextMenuItems: ContextMenuItem[] = [];
  @Output('viewBlockoutDate')
  public viewBlockoutDateEvent: EventEmitter<BlockoutDateViewModel> = new EventEmitter<BlockoutDateViewModel>();
  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<BlockoutDateViewModel>> = new EventEmitter();

  public get displayColumns(): ListBlockoutDateGridDisplayColumns[] {
    return this._displayColumns;
  }
  public dicDisplayColumns: Dictionary<boolean> = {};
  private _displayColumns: ListBlockoutDateGridDisplayColumns[] = [
    ListBlockoutDateGridDisplayColumns.title,
    ListBlockoutDateGridDisplayColumns.description,
    ListBlockoutDateGridDisplayColumns.startDateTime,
    ListBlockoutDateGridDisplayColumns.endDateTime,
    ListBlockoutDateGridDisplayColumns.serviceSchemes,
    ListBlockoutDateGridDisplayColumns.actions
  ];
  private _loadDataSub: Subscription = new Subscription();
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    public listBlockoutDateGridComponentService: ListBlockoutDateGridComponentService
  ) {
    super(moduleFacadeService);
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, _ => true);
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = (this.availableBlockOutDates != null
      ? of(<OpalGridDataResult<BlockoutDateViewModel>>{
          data: this.availableBlockOutDates,
          total: this.availableBlockOutDates.length
        })
      : this.listBlockoutDateGridComponentService.loadBlockoutDates(
          this.filter.search,
          this.filter.filter,
          this.state.skip,
          this.state.take,
          this.checkAll,
          this.coursePlanningCycleId,
          () => this.selecteds
        )
    )
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.updateSelectedsAndGridData();
      });
  }

  public getContextMenuByBlockoutDate(rowData: BlockoutDateViewModel): ContextMenuItem[] {
    return this.contextMenuItems.filter(item => item.id === ContextMenuAction.Delete && rowData.isConfirmed === false);
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: BlockoutDateViewModel, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public get statusColorMap(): unknown {
    return BLOCKOUT_DATE_STATUS_COLOR_MAP;
  }

  public onGridCellClick(event: CellClickEvent): void {
    if (event.dataItem instanceof BlockoutDateViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewBlockoutDateEvent.emit(event.dataItem);
    }
  }

  public canShowTitle(): boolean {
    return this.dicDisplayColumns[ListBlockoutDateGridDisplayColumns.title];
  }

  public canShowDescription(): boolean {
    return this.dicDisplayColumns[ListBlockoutDateGridDisplayColumns.description];
  }

  public canShowStartDateTime(): boolean {
    return this.dicDisplayColumns[ListBlockoutDateGridDisplayColumns.startDateTime];
  }

  public canShowEndDateTime(): boolean {
    return this.dicDisplayColumns[ListBlockoutDateGridDisplayColumns.endDateTime];
  }

  public canShowServiceSchemes(): boolean {
    return this.dicDisplayColumns[ListBlockoutDateGridDisplayColumns.serviceSchemes];
  }

  public canShowAction(): boolean {
    return this.dicDisplayColumns[ListBlockoutDateGridDisplayColumns.actions];
  }

  public canShowActionContextMenu(dataItem: BlockoutDateViewModel): boolean {
    return (
      this.getContextMenuByBlockoutDate(dataItem).length !== 0 &&
      dataItem.isConfirmed === false &&
      BlockoutDateModel.haveCudPermission(this.currentUser)
    );
  }

  protected onInit(): void {
    super.onInit();
  }
}

export enum ListBlockoutDateGridDisplayColumns {
  title = 'title',
  description = 'description',
  startDateTime = 'startDateTime',
  endDateTime = 'endDateTime',
  serviceSchemes = 'serviceSchemes',
  actions = 'actions'
}
