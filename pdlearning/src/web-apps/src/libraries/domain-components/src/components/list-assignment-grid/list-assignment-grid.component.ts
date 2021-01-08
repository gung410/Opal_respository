import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { CellClickEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable, Subscription, combineLatest } from 'rxjs';

import { AssignmentType } from '@opal20/domain-api';
import { AssignmentViewModel } from './../../models/assignment-view.model';
import { ContextMenuEmit } from './../../models/context-menu-emit.model';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { ListAssignmentGridComponentService } from './../../services/list-assignment-grid-component.service';
import { OpalDialogService } from '@opal20/common-components';

@Component({
  selector: 'list-assignment-grid',
  templateUrl: './list-assignment-grid.component.html'
})
export class ListAssignmentGridComponent extends BaseGridComponent<AssignmentViewModel> {
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

  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<AssignmentViewModel>> = new EventEmitter();
  @Output('viewAssignment') public viewAssignmentEvent: EventEmitter<AssignmentViewModel> = new EventEmitter<AssignmentViewModel>();

  public query: Observable<unknown>;
  public loading: boolean;

  private _loadDataSub: Subscription = new Subscription();
  private _courseId: string;
  private _classRunId: string;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    private listAssignmentGridComponentService: ListAssignmentGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: AssignmentViewModel, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = combineLatest(this.getCourseAssignments(), this.getClassRunAssignments())
      .pipe(this.untilDestroy())
      .subscribe(([courseAssignment, classrunAssignment]) => {
        this.gridData =
          classrunAssignment != null && classrunAssignment.total !== 0
            ? { data: classrunAssignment.data, total: classrunAssignment.total }
            : { data: courseAssignment.data, total: courseAssignment.total };
        this.updateSelectedsAndGridData();
      });
  }

  public onGridCellClick(event: CellClickEvent): void {
    // columnIndex isn't action column
    if (event.dataItem instanceof AssignmentViewModel && (this.indexActionColumn == null || event.columnIndex !== this.indexActionColumn)) {
      this.viewAssignmentEvent.emit(event.dataItem);
    }
  }

  protected onInit(): void {
    super.onInit();
  }

  private getCourseAssignments(): Observable<OpalGridDataResult<AssignmentViewModel>> {
    return this.listAssignmentGridComponentService
      .loadAssignments(this.courseId, null, AssignmentType.Quiz, this.state.skip, this.state.take, this.checkAll, () => this.selecteds)
      .pipe(this.untilDestroy());
  }

  private getClassRunAssignments(): Observable<OpalGridDataResult<AssignmentViewModel>> {
    return this.listAssignmentGridComponentService
      .loadAssignments(
        this.courseId,
        this.classRunId,
        AssignmentType.Quiz,
        this.state.skip,
        this.state.take,
        this.checkAll,
        () => this.selecteds
      )
      .pipe(this.untilDestroy());
  }
}
