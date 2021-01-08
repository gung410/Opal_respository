import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  CLASSRUN_CANCELLATION_STATUS_COLOR_MAP,
  CLASSRUN_RESCHEDULE_STATUS_COLOR_MAP,
  CLASSRUN_STATUS_COLOR_MAP
} from './../../models/classrun-status-color-map.model';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import {
  ClassRun,
  ClassRunCancellationStatus,
  ClassRunRescheduleStatus,
  ClassRunStatus,
  ContentStatus,
  Course,
  CourseStatus,
  SearchClassRunType,
  UserInfoModel
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { CONTENT_STATUS_COLOR_MAP } from '../../models/content-status-color-map.model';
import { ClassRunViewModel } from './../../models/classrun-view.model';
import { ContextMenuAction } from '../../models/context-menu-action.model';
import { ContextMenuEmit } from '../../models/context-menu-emit.model';
import { ContextMenuItem } from '@opal20/common-components';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { IRowCallbackModel } from './../../models/row-callback.model';
import { ListClassRunGridComponentService } from '../../services/list-classrun-grid-component.service';

@Component({
  selector: 'list-classrun-grid',
  templateUrl: './list-classrun-grid.component.html'
})
export class ListClassRunGridComponent extends BaseGridComponent<ClassRunViewModel> {
  public get courseId(): string | undefined {
    return this._courseId;
  }

  @Input()
  public set courseId(v: string | undefined) {
    if (Utils.isDifferent(this._courseId, v)) {
      this._courseId = v;
    }
  }

  public get course(): Course | undefined {
    return this._course;
  }

  @Input()
  public set course(v: Course | undefined) {
    if (Utils.isDifferent(this._course, v)) {
      this._course = v;
    }
  }
  @Input() public forContent: boolean = false;
  @Input() public searchType: SearchClassRunType = SearchClassRunType.Owner;
  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<ClassRun>> = new EventEmitter();
  @Output() public loadedData: EventEmitter<GridDataResult> = new EventEmitter();
  @Output('viewClassRun') public viewClassRunEvent: EventEmitter<ClassRunViewModel> = new EventEmitter<ClassRunViewModel>();
  public contextMenuItemsForClassRun: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Publish,
      text: this.translateCommon('Publish'),
      icon: 'track-changes-accept'
    },
    {
      id: ContextMenuAction.Unpublish,
      text: this.translateCommon('Unpublish'),
      icon: 'track-changes-reject'
    },
    {
      id: ContextMenuAction.CancellationRequest,
      text: this.translateCommon('Cancel Class'),
      icon: 'cancel'
    },
    {
      id: ContextMenuAction.RescheduleRequest,
      text: this.translateCommon('Reschedule'),
      icon: 'clock'
    }
  ];

  public CourseStatus: typeof CourseStatus = CourseStatus;

  @Input()
  public set displayColumns(displayColumns: ListClassrunGridDisplayColumns[]) {
    this._displayColumns = displayColumns;
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, _ => true);
  }

  public SearchClassRunType: typeof SearchClassRunType = SearchClassRunType;
  public ClassRunStatus: typeof ClassRunStatus = ClassRunStatus;
  public query: Observable<unknown>;
  public loading: boolean;
  public dicDisplayColumns: Dictionary<boolean>;
  public get statusColorMap(): unknown {
    if (this.forContent) {
      return CONTENT_STATUS_COLOR_MAP;
    }
    return [CLASSRUN_STATUS_COLOR_MAP, CLASSRUN_RESCHEDULE_STATUS_COLOR_MAP, CLASSRUN_CANCELLATION_STATUS_COLOR_MAP];
  }

  private _displayColumns: ListClassrunGridDisplayColumns[] = [
    ListClassrunGridDisplayColumns.title,
    ListClassrunGridDisplayColumns.learningPeriod,
    ListClassrunGridDisplayColumns.applicationPeriod,
    ListClassrunGridDisplayColumns.owner,
    ListClassrunGridDisplayColumns.status,
    ListClassrunGridDisplayColumns.actions
  ];

  private _loadDataSub: Subscription = new Subscription();
  private _course: Course;
  private currentUser = UserInfoModel.getMyUserInfo();
  private _courseId: string;
  constructor(public moduleFacadeService: ModuleFacadeService, public listClassRunGridComponentService: ListClassRunGridComponentService) {
    super(moduleFacadeService);
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, _ => true);
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public getContextMenuByClassRun(rowData: ClassRun): ContextMenuItem[] {
    return this.contextMenuItemsForClassRun.filter(
      item =>
        (this.showUnpublish(rowData) && item.id === ContextMenuAction.Unpublish) ||
        (this.showPublish(rowData) && item.id === ContextMenuAction.Publish) ||
        (this.showCancellationRequest(rowData) && item.id === ContextMenuAction.CancellationRequest) ||
        (this.showRescheduleRequest(rowData) && item.id === ContextMenuAction.RescheduleRequest)
    );
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: ClassRun, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.listClassRunGridComponentService
      .loadClassRunsByCourseId(
        this.courseId,
        this.searchType,
        this.filter.search,
        this.filter.filter,
        false,
        false,
        this.state.skip,
        this.state.take,
        this.checkAll,
        () => this.selecteds,
        this.isDisplayContentStatus()
      )
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.loadedData.emit(this.gridData);
        this.updateSelectedsAndGridData();
      });
  }

  public onGridCellClick(event: CellClickEvent): void {
    if (event.dataItem instanceof ClassRunViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewClassRunEvent.emit(event.dataItem);
    }
  }

  public displayStatuses(dataItem: ClassRun): ContentStatus | (ClassRunStatus | ClassRunCancellationStatus | ClassRunRescheduleStatus)[] {
    return this.isDisplayContentStatus()
      ? dataItem.contentStatus
      : dataItem.status === ClassRunStatus.Cancelled
      ? [dataItem.status]
      : [dataItem.status, dataItem.rescheduleStatus, dataItem.cancellationStatus];
  }

  public isResubmit(dataItem: ClassRun): boolean {
    return this.isDisplayContentStatus() ? dataItem.isContentResubmit() : false;
  }

  public displayStatusPrefix(dataItem: ClassRun): string {
    if (this.isDisplayContentStatus() && ClassRun.isContentResubmit(dataItem, this.course)) {
      return '(Resubmit)';
    }
    if (dataItem.status === ClassRunStatus.Published && dataItem.rescheduled()) {
      return '(Rescheduled)';
    }
    return '';
  }

  public isDisplayContentStatus(): boolean {
    return (
      this.searchType !== SearchClassRunType.CancellationPending &&
      this.searchType !== SearchClassRunType.ReschedulePending &&
      this.forContent
    );
  }

  public canViewSelectItem(): boolean {
    return (
      this.searchType === SearchClassRunType.Owner ||
      this.searchType === SearchClassRunType.LearningManagement ||
      this.searchType === SearchClassRunType.CancellationPending ||
      this.searchType === SearchClassRunType.ReschedulePending
    );
  }

  public showCheckboxColumn(): boolean {
    return this.canViewSelectItem() && this.dicDisplayColumns[ListClassrunGridDisplayColumns.selected];
  }

  public showTitleColumn(): boolean {
    return this.dicDisplayColumns[ListClassrunGridDisplayColumns.title];
  }

  public showLearningPeriodColumn(): boolean {
    return this.dicDisplayColumns[ListClassrunGridDisplayColumns.learningPeriod];
  }

  public showApplicationPeriodColumn(): boolean {
    return this.dicDisplayColumns[ListClassrunGridDisplayColumns.applicationPeriod];
  }

  public showOwnerColumn(): boolean {
    return this.dicDisplayColumns[ListClassrunGridDisplayColumns.owner];
  }

  public showStatusColumn(): boolean {
    return this.dicDisplayColumns[ListClassrunGridDisplayColumns.status];
  }

  public showActionColumn(): boolean {
    return this.searchType === SearchClassRunType.Owner && this.dicDisplayColumns[ListClassrunGridDisplayColumns.actions];
  }

  public canShowActionColumn(dataItem: ClassRun): boolean {
    return dataItem.status !== ClassRunStatus.Cancelled && this.getContextMenuByClassRun(dataItem).length > 0;
  }

  protected onInit(): void {
    super.onInit();
  }

  private showCancellationRequest(classRun: ClassRun): boolean {
    return classRun.canCancelClass(this.course) && ClassRun.hasCancelClassPermission(this.course, this.currentUser);
  }

  private showRescheduleRequest(classRun: ClassRun): boolean {
    return classRun.canRescheduleClass(this.course) && ClassRun.hasRescheduleClassPermission(this.course, this.currentUser);
  }

  private showPublish(classRun: ClassRun): boolean {
    return classRun.canPublish(this.course) && ClassRun.hasPublishUnPublishPermission(this.course, this.currentUser);
  }

  private showUnpublish(classRun: ClassRun): boolean {
    return classRun.canUnpublish(this.course) && ClassRun.hasPublishUnPublishPermission(this.course, this.currentUser);
  }
}

export enum ListClassrunGridDisplayColumns {
  selected = 'selected',
  title = 'title',
  learningPeriod = 'learningPeriod',
  applicationPeriod = 'applicationPeriod',
  owner = 'owner',
  status = 'status',
  actions = 'actions'
}
