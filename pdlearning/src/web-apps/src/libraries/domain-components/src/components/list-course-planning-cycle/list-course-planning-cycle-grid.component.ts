import { BaseGridComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Output } from '@angular/core';
import { CoursePlanningCycle, UserInfoModel } from '@opal20/domain-api';
import { Observable, Subscription } from 'rxjs';

import { COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP } from './../../models/course-planning-cycle-status-color-map.model';
import { CellClickEvent } from '@progress/kendo-angular-grid';
import { CoursePlanningCycleViewModel } from '../../models/course-planning-cycle-view.model';
import { ListCoursePlanningCycleGridComponentService } from '../../services/list-course-planning-cycle-grid-component.service';

@Component({
  selector: 'list-course-planning-cycle-grid',
  templateUrl: './list-course-planning-cycle-grid.component.html'
})
export class ListCoursePlanningCycleGridComponent extends BaseGridComponent<CoursePlanningCycleViewModel> {
  @Output('viewCoursePlanningCycle')
  public viewCoursePlanningCycleEvent: EventEmitter<CoursePlanningCycleViewModel> = new EventEmitter<CoursePlanningCycleViewModel>();

  public checkCourseContent: boolean = true;
  public query: Observable<unknown>;
  public loading: boolean;

  private _loadDataSub: Subscription = new Subscription();
  private currentUser = UserInfoModel.getMyUserInfo();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private listCoursePlanningCycleGridSvc: ListCoursePlanningCycleGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.listCoursePlanningCycleGridSvc
      .loadCoursePlanningCycles(this.filter.search, this.state.skip, this.state.take, this.checkAll, () => this.selecteds)
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.updateSelectedsAndGridData();
      });
  }

  public canShowNumberOfCourses(): boolean {
    return CoursePlanningCycle.canVerifyCourse(this.currentUser);
  }

  public onGridCellClick(event: CellClickEvent): void {
    if (event.dataItem instanceof CoursePlanningCycleViewModel) {
      this.viewCoursePlanningCycleEvent.emit(event.dataItem);
    }
  }

  public get statusColorMap(): unknown {
    return COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP;
  }

  protected onInit(): void {
    super.onInit();
  }
}
