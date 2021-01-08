import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { CellClickEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { AssignmentApiService } from '@opal20/domain-api';
import { ContextMenuEmit } from './../../models/context-menu-emit.model';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { ListParticipantAssignmentTrackGridComponentService } from '../../services/list-participant-assignment-track-component.service';
import { PARTICIPANT_ASSIGNMENT_TRACK_STATUS_COLOR_MAP } from './../../models/participant-assignment-track-status-color-map.model';
import { ParticipantAssignmentTrackViewModel } from './../../models/participant-assignment-track-view.model';

@Component({
  selector: 'list-participant-assignment-track-grid',
  templateUrl: './list-assignment-track-grid.component.html'
})
export class ListParticipantAssignmentTrackGridComponent extends BaseGridComponent<ParticipantAssignmentTrackViewModel> {
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

  public get assignmentId(): string | undefined {
    return this._assignmentId;
  }

  @Input()
  public set assignmentId(v: string | undefined) {
    if (Utils.isDifferent(this._assignmentId, v)) {
      this._assignmentId = v;
    }
  }

  @Input() public indexActionColumn: number = null;

  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<ParticipantAssignmentTrackViewModel>> = new EventEmitter();
  @Output('viewAssignmentTrack') public viewAssignmentTrackEvent: EventEmitter<ParticipantAssignmentTrackViewModel> = new EventEmitter<
    ParticipantAssignmentTrackViewModel
  >();

  public query: Observable<unknown>;
  public loading: boolean;

  public get statusColorMap(): unknown {
    return PARTICIPANT_ASSIGNMENT_TRACK_STATUS_COLOR_MAP;
  }

  private _loadDataSub: Subscription = new Subscription();
  private _courseId: string;
  private _classRunId: string;
  private _assignmentId: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public assignmentApiService: AssignmentApiService,
    public listParticipantAssignmentTrackGridComponentService: ListParticipantAssignmentTrackGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: ParticipantAssignmentTrackViewModel, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public onGridCellClick(event: CellClickEvent): void {
    // columnIndex isn't action column
    if (
      event.dataItem instanceof ParticipantAssignmentTrackViewModel &&
      (this.indexActionColumn == null || event.columnIndex !== this.indexActionColumn)
    ) {
      this.viewAssignmentTrackEvent.emit(event.dataItem);
    }
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.listParticipantAssignmentTrackGridComponentService
      .loadParticipantAssignmentTracks(
        this.courseId,
        this.classRunId,
        this.assignmentId,
        this.filter.search,
        this.filter.filter,
        null,
        this.state.skip,
        this.state.take,
        false,
        () => this.selecteds,
        true
      )
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.updateSelectedsAndGridData();
      });
  }

  protected onInit(): void {
    super.onInit();
  }
}
