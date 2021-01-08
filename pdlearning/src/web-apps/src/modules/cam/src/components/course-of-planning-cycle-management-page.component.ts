import { BasePageComponent, ComponentType, IGridFilter, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { ButtonAction, OpalDialogService } from '@opal20/common-components';
import {
  CAMRoutePaths,
  CAMTabConfiguration,
  CourseCriteriaDetailMode,
  CourseDetailMode,
  CourseFilterComponent,
  CourseFilterModel,
  CoursePlanningCycleDetailMode,
  CourseViewModel,
  ListCourseGridComponentService,
  NavigationPageService,
  RouterPageInput,
  SelectCourseDialogComponent,
  SelectCourseModel
} from '@opal20/domain-components';
import { Component, HostBinding, Input } from '@angular/core';
import {
  Course,
  CoursePlanningCycle,
  CoursePlanningCycleStatus,
  CourseRepository,
  SearchCourseType,
  UserInfoModel
} from '@opal20/domain-api';

import { CoursePlanningCycleDetailPageInput } from '../models/course-planning-cycle-detail-input.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Observable } from 'rxjs';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { map } from 'rxjs/operators';

@Component({
  selector: 'course-of-planning-cycle-management-page',
  templateUrl: './course-of-planning-cycle-management-page.component.html'
})
export class CourseOfPlanningCycleManagementPageComponent extends BasePageComponent {
  public filterPopupContent: ComponentType<CourseFilterComponent> = CourseFilterComponent;
  public searchText: string = '';
  public filterData: CourseFilterModel = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public gridData: GridDataResult;

  public get coursePlanningCycle(): CoursePlanningCycle | undefined {
    return this._coursePlanningCycle;
  }

  public get showMessage(): boolean {
    return this.showNotStartedMessage() || this.showEndedMessage();
  }

  @Input() public stickyDependElement: HTMLElement;

  @Input()
  public set coursePlanningCycle(v: CoursePlanningCycle | undefined) {
    if (Utils.isDifferent(this._coursePlanningCycle, v)) {
      this._coursePlanningCycle = v;
    }
  }

  @Input() public coursePlanningCycleDetailMode: CoursePlanningCycleDetailMode;

  public get coursePlanningCycleDetailPageInput():
    | RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration>
    | undefined {
    return this._coursePlanningCycleDetailPageInput;
  }

  @Input() public set coursePlanningCycleDetailPageInput(
    coursePlanningCycleDetailPageInput:
      | RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration>
      | undefined
  ) {
    this._coursePlanningCycleDetailPageInput = coursePlanningCycleDetailPageInput;
  }

  public actionBtnGroup: ButtonAction<unknown>[] = [
    {
      id: 'publish',
      text: this.translateCommon('Publish')
    },
    {
      id: 'unpublish',
      text: this.translateCommon('Unpublish')
    },
    {
      id: 'approve',
      text: this.translateCommon('Approve')
    },
    {
      id: 'reject',
      text: this.translateCommon('Reject')
    }
  ];
  public createCourseModes: Array<unknown> = [
    {
      id: CourseDetailMode.NewCourse,
      text: this.translate('New Course')
    },
    {
      id: CourseDetailMode.Recurring,
      text: this.translate('Recurring')
    },
    {
      id: CourseDetailMode.Edit,
      text: this.translate('Duplicate')
    }
  ];
  public SearchCourseType: typeof SearchCourseType = SearchCourseType;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  private _coursePlanningCycle: CoursePlanningCycle;
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  public get selectedTab(): CAMTabConfiguration {
    return this.coursePlanningCycleDetailPageInput.subActiveTab != null
      ? this.coursePlanningCycleDetailPageInput.subActiveTab
      : CAMTabConfiguration.AllCoursesOfPlanningCycleTab;
  }
  private _coursePlanningCycleDetailPageInput:
    | RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration>
    | undefined;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private opalDialogService: OpalDialogService,
    public navigationPageService: NavigationPageService,
    private listCourseGridComponentService: ListCourseGridComponentService,
    private courseRepository: CourseRepository
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public canCreateCourse(): boolean {
    return (
      this.currentUser &&
      Course.haveCreateCoursePermission(this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.AllCoursesOfPlanningCycleTab &&
      this.coursePlanningCycle.status === CoursePlanningCycleStatus.InProgress
    );
  }

  public showNotStartedMessage(): boolean {
    return this.coursePlanningCycle.status === CoursePlanningCycleStatus.NotStarted;
  }

  public showEndedMessage(): boolean {
    return this.coursePlanningCycle.status === CoursePlanningCycleStatus.Completed;
  }

  public onTabSelected(event: SelectEvent): void {
    this.coursePlanningCycleDetailPageInput.subActiveTab = courseOfPlanningCycleManagementPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.coursePlanningCycleDetailPageInput);
  }

  public canViewPendingApproval(): boolean {
    return this.currentUser && Course.haveApproveCoursePermission(this.currentUser);
  }

  public canViewPendingVerify(): boolean {
    return this.currentUser && CoursePlanningCycle.canVerifyCourse(this.currentUser);
  }

  public checkSearchCourseHasDataFnCreator(searchCourseType: SearchCourseType): () => Observable<boolean> {
    return () => {
      return this.listCourseGridComponentService
        .loadCourses(null, null, searchCourseType, null, 0, 0, false, false, null, this.coursePlanningCycleDetailPageInput.data.id)
        .pipe(
          map(data => {
            return data.total > 0;
          })
        );
    };
  }

  public onCreateCourse(mode: string): void {
    // TODO: Switch case for mode
    if (mode === CourseDetailMode.Recurring) {
      const dialogRef: DialogRef = this.opalDialogService.openDialogRef(SelectCourseDialogComponent);
      this.subscribe(dialogRef.result, (data: SelectCourseModel) => {
        if (data.id) {
          this.navigationPageService.navigateTo(
            CAMRoutePaths.CourseDetailPage,
            {
              activeTab: CAMTabConfiguration.CourseInfoTab,
              subActiveTab: CAMTabConfiguration.AllClassRunsTab,
              data: {
                mode: CourseDetailMode.Recurring,
                id: data.id,
                coursePlanningCycleId: this.coursePlanningCycle.id
              }
            },
            this.coursePlanningCycleDetailPageInput
          );
        }
      });
    } else if (mode === CourseDetailMode.Edit) {
      const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
        SelectCourseDialogComponent,
        SelectCourseDialogComponent.selectToCloneCoursesConfig()
      );
      this.subscribe(dialogRef.result, (data: SelectCourseModel) => {
        if (data.id) {
          this.courseRepository
            .cloneCourse({ id: data.id, fromCoursePlanning: true })
            .pipe(this.untilDestroy())
            .subscribe(course => {
              this.navigationPageService.navigateTo(
                CAMRoutePaths.CourseDetailPage,
                {
                  activeTab: CAMTabConfiguration.CourseInfoTab,
                  data: {
                    mode: CourseDetailMode.Edit,
                    id: course.id,
                    coursePlanningCycleId: this.coursePlanningCycle.id,
                    courseCriteriaMode: CourseCriteriaDetailMode.View
                  }
                },
                this.coursePlanningCycleDetailPageInput
              );
            });
        }
      });
    } else {
      this.navigationPageService.navigateTo(
        CAMRoutePaths.CourseDetailPage,
        {
          activeTab: CAMTabConfiguration.CourseInfoTab,
          subActiveTab: CAMTabConfiguration.AllClassRunsTab,
          data: {
            mode: CourseDetailMode.NewCourse,
            coursePlanningCycleId: this.coursePlanningCycle.id
          }
        },
        this.coursePlanningCycleDetailPageInput
      );
    }
  }

  public onViewCourse(data: CourseViewModel, searchType: SearchCourseType, activeTab: CAMTabConfiguration, mode: CourseDetailMode): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.CourseDetailPage,
      {
        activeTab: CAMTabConfiguration.CourseInfoTab,
        subActiveTab: CAMTabConfiguration.AllClassRunsTab,
        data: {
          mode: mode,
          id: data.id,
          coursePlanningCycleId: this.coursePlanningCycle.id
        }
      },
      this.coursePlanningCycleDetailPageInput
    );
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public onApplyFilter(data: CourseFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }
}

export const courseOfPlanningCycleManagementPageTabIndexMap = {
  0: CAMTabConfiguration.AllCoursesOfPlanningCycleTab,
  1: CAMTabConfiguration.CoursePendingApprovalOfPlanningCycleTab,
  2: CAMTabConfiguration.CoursePendingVerifyOfPlanningCycleTab
};
