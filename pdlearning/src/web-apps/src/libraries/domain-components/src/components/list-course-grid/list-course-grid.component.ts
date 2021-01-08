import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { COURSE_IN_COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP, COURSE_STATUS_COLOR_MAP } from './../../models/course-status-color-map.model';
import { CellClickEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ContentStatus, CourseStatus, SearchCourseType, UserInfoModel } from '@opal20/domain-api';
import { Observable, Subscription } from 'rxjs';

import { CONTENT_STATUS_COLOR_MAP } from '../../models/content-status-color-map.model';
import { ContextMenuAction } from '../../models/context-menu-action.model';
import { ContextMenuEmit } from './../../models/context-menu-emit.model';
import { ContextMenuItem } from '@opal20/common-components';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { CourseViewModel } from './../../models/course-view.model';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { ListCourseGridComponentService } from '../../services/list-course-grid-component.service';

@Component({
  selector: 'list-course-grid',
  templateUrl: './list-course-grid.component.html'
})
export class ListCourseGridComponent extends BaseGridComponent<CourseViewModel> {
  @Input() public searchType: SearchCourseType = SearchCourseType.Owner;
  @Input() public set ignoreColumns(ignoreColumns: ListCourseGridDisplayColumns[]) {
    this._ignoreColumns = ignoreColumns;
    if (this.initiated) {
      this.updateIgnoreColumnsDict();
    }
  }
  @Input() public contextMenuItems: ContextMenuItem[] = [];
  @Input() public coursePlanningCycleId?: string = null;
  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<CourseViewModel>> = new EventEmitter();
  public SearchCourseType: typeof SearchCourseType = SearchCourseType;
  public CourseStatus: typeof CourseStatus = CourseStatus;
  public dicIgnoreColumns: Dictionary<boolean> = {};

  @Output('viewCourse') public viewCourseEvent: EventEmitter<CourseViewModel> = new EventEmitter<CourseViewModel>();

  public checkCourseContent: boolean = true;
  public query: Observable<unknown>;
  public loading: boolean;

  private _loadDataSub: Subscription = new Subscription();
  private currentUser = UserInfoModel.getMyUserInfo();
  private _ignoreColumns: ListCourseGridDisplayColumns[] = [
    ListCourseGridDisplayColumns.archivedBy,
    ListCourseGridDisplayColumns.archivedDate
  ];

  constructor(public moduleFacadeService: ModuleFacadeService, public listCourseGridComponentService: ListCourseGridComponentService) {
    super(moduleFacadeService);
  }

  public getStatusColorMap(course: CourseViewModel): unknown {
    return this.showContentStatus()
      ? CONTENT_STATUS_COLOR_MAP
      : course && course.coursePlanningCycleId
      ? COURSE_IN_COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP
      : COURSE_STATUS_COLOR_MAP;
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public getContextMenuByCourse(rowData: CourseViewModel): ContextMenuItem[] {
    return this.contextMenuItems.filter(
      item =>
        ((rowData.status === CourseStatus.Approved || rowData.status === CourseStatus.Unpublished) &&
          item.id === ContextMenuAction.Publish &&
          rowData.canPublishCourse() &&
          rowData.hasPublishCoursePermission(this.currentUser)) ||
        (rowData.status === CourseStatus.Published &&
          item.id === ContextMenuAction.Unpublish &&
          rowData.canUnpublishCourse(this.currentUser)) ||
        (rowData.canDeleteCourse(this.currentUser) && item.id === ContextMenuAction.Delete) ||
        (rowData.canTransferOwnershipCourse(this.currentUser) && item.id === ContextMenuAction.TransferOwnership) ||
        (rowData.canArchiveCourse(this.currentUser) && item.id === ContextMenuAction.Archive)
    );
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: CourseViewModel, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.listCourseGridComponentService
      .loadCourses(
        this.filter.search,
        this.filter.filter,
        this.searchType,
        null,
        this.state.skip,
        this.state.take,
        this.checkAll,
        this.checkCourseContent,
        () => this.selecteds,
        this.coursePlanningCycleId
      )
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.updateSelectedsAndGridData();
      });
  }

  public onGridCellClick(event: CellClickEvent): void {
    // columnIndex isn't action column
    if (event.dataItem instanceof CourseViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewCourseEvent.emit(event.dataItem);
    }
  }

  public showContentStatus(): boolean {
    return this.searchType === SearchCourseType.LearningManagement || this.searchType === SearchCourseType.ContentApprover;
  }

  public displayStatus(item: CourseViewModel): CourseStatus | ContentStatus {
    if (
      this.searchType === SearchCourseType.Approver ||
      this.searchType === SearchCourseType.ClassApprover ||
      this.searchType === SearchCourseType.RegistrationApprover ||
      this.searchType === SearchCourseType.ContentApprover
    ) {
      return this.showContentStatus() ? ContentStatus.PendingApproval : CourseStatus.PendingApproval;
    }
    return this.showContentStatus() ? item.contentStatus : item.status;
  }

  public isResubmit(dataItem: CourseViewModel): boolean {
    return this.showContentStatus() ? dataItem.isContentResubmit() : dataItem.isResubmit();
  }

  public showCheckboxColumn(): boolean {
    return (
      (this.searchType === SearchCourseType.LearningManagement || this.searchType === SearchCourseType.Owner) &&
      !this.dicIgnoreColumns[ListCourseGridDisplayColumns.selected]
    );
  }

  public showTitleColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.title];
  }

  public showCreatedDateColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.createdDate];
  }

  public showInPlanningPeriod(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.period];
  }

  public showPDActivityTypeColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.pdActivityType];
  }

  public showLearningModeColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.learningMode];
  }

  public showOwnerColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.owner];
  }

  public showArchivedByColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.archivedBy];
  }

  public showArchivedDateColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.archivedDate];
  }

  public showStatusColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.status];
  }

  public showActionColumn(): boolean {
    return !this.dicIgnoreColumns[ListCourseGridDisplayColumns.actions];
  }

  protected onInit(): void {
    super.onInit();
    this.updateIgnoreColumnsDict();
  }

  private updateIgnoreColumnsDict(): void {
    this.dicIgnoreColumns = Utils.toDictionarySelect(this._ignoreColumns, p => p, p => true);
  }
}

export enum ListCourseGridDisplayColumns {
  selected = 'selected',
  title = 'title',
  createdDate = 'createdDate',
  period = 'period',
  pdActivityType = 'pdActivityType',
  learningMode = 'learningMode',
  owner = 'owner',
  archivedBy = 'archivedBy',
  archivedDate = 'archivedDate',
  status = 'status',
  actions = 'actions'
}
