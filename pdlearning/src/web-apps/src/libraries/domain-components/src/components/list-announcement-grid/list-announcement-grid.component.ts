import { Announcement, AnnouncementStatus } from '@opal20/domain-api';
import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ContextMenuItem, OpalDialogService } from '@opal20/common-components';
import { Observable, Subscription } from 'rxjs';

import { ANNOUNCEMENT_STATUS_COLOR_MAP } from './../../models/announcement-status-color-map.model';
import { AnnouncementDetailDialogComponent } from '../announcement-detail-dialog/announcement-detail-dialog.component';
import { AnnouncementViewModel } from './../../models/announcement-view.model';
import { ContextMenuAction } from './../../models/context-menu-action.model';
import { ContextMenuEmit } from './../../models/context-menu-emit.model';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { ListAnnouncementGridComponentService } from './../../services/list-announcement-grid-component.service';

@Component({
  selector: 'list-announcement-grid',
  templateUrl: './list-announcement-grid.component.html'
})
export class ListAnnouncementGridComponent extends BaseGridComponent<AnnouncementViewModel> {
  public get courseId(): string | undefined {
    return this._courseId;
  }

  @Input()
  public set courseId(v: string | undefined) {
    if (Utils.isDifferent(this._courseId, v)) {
      this._courseId = v;
    }
  }

  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
    }
  }

  @Input() public indexActionColumn: number = null;
  @Output() public loadedData: EventEmitter<GridDataResult> = new EventEmitter();
  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<AnnouncementViewModel>> = new EventEmitter();
  @Output('viewAnnouncement') public viewAnnouncementEvent: EventEmitter<AnnouncementViewModel> = new EventEmitter<AnnouncementViewModel>();

  public query: Observable<unknown>;
  public loading: boolean;
  public statusColorMap = ANNOUNCEMENT_STATUS_COLOR_MAP;

  public contextMenuItemsForAnnouncement: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Send,
      text: this.translateCommon('Send now'),
      icon: ''
    },
    {
      id: ContextMenuAction.Cancel,
      text: this.translateCommon('Cancel'),
      icon: ''
    }
  ];

  private _loadDataSub: Subscription = new Subscription();
  private _classRunId: string;
  private _courseId: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    public listAnnouncementGridComponentService: ListAnnouncementGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.listAnnouncementGridComponentService
      .loadAnnouncements(
        this.courseId,
        this.classRunId,
        this.filter.filter,
        this.state.skip,
        this.state.take,
        this.checkAll,
        () => this.selecteds
      )
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.loadedData.emit(this.gridData);
        this.updateSelectedsAndGridData();
      });
  }

  public canShowActionColumn(dataItem: Announcement): boolean {
    return dataItem.status === AnnouncementStatus.Scheduled;
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: AnnouncementViewModel, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public onViewAnnouncement(dataItem: Announcement): void {
    this.opalDialogService.openDialogRef(
      AnnouncementDetailDialogComponent,
      {
        announcement: dataItem
      },
      {
        width: '800px',
        height: '600px'
      }
    );
  }

  public onGridCellClick(event: CellClickEvent): void {
    // columnIndex isn't action column
    if (
      event.dataItem instanceof AnnouncementViewModel &&
      (this.indexActionColumn == null || event.columnIndex !== this.indexActionColumn)
    ) {
      this.viewAnnouncementEvent.emit(event.dataItem);
    }
  }

  protected onInit(): void {
    super.onInit();
  }
}
