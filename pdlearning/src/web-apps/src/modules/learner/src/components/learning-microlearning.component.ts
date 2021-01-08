import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { ILearningItemModel, LearningItemModel } from '../models/learning-item.model';
import { IMyCoursesSearchRequest, LearningCourseType, MyCourseSearchFilterResultModel, MyLearningStatisticType } from '@opal20/domain-api';
import { MyLearningSearchFilterResult, SearchFilterModel } from '../models/my-learning-search-filter-model';

import { CourseDataService } from '../services/course-data.service';
import { LearningCardListComponent } from './learning-card-list.component';
import { MyLearningSearchDataService } from '../services/my-learning-search.service';
import { MyLearningTab } from '@opal20/domain-components';
import { Observable } from 'rxjs';
import { SEARCH_FILTER_TYPE_INITIALIZE } from '../constants/my-learning-tab.enum';
import { StatusFilterModel } from '../models/status-filter.model';
import { map } from 'rxjs/operators';

@Component({
  selector: 'learning-microlearning',
  templateUrl: './learning-microlearning.component.html'
})
export class LearningMicrolearningComponent extends BaseComponent {
  @Input()
  public searchingText: string;

  @Input()
  public isSearchingWithText: boolean;

  @Output()
  public learningCardClick: EventEmitter<ILearningItemModel> = new EventEmitter<ILearningItemModel>();

  public statusFilter: StatusFilterModel[];
  public currentMyStatusFilter: string = FilterMicrolearningStatus.InProgress;
  public filterByType: SearchFilterModel;

  public filterResult$: Observable<MyCourseSearchFilterResultModel>;

  protected filterStatuses: {} = FilterMicrolearningStatus;
  protected currentActiveTab: MyLearningTab = MyLearningTab.Microlearning;

  @ViewChild('myCoursesList', { static: false })
  protected myCoursesListComponent: LearningCardListComponent;
  protected inProgressOrderBy: string = 'LastLogin DESC';
  protected completedOrderBy: string = 'CompletedDate DESC';

  private mapFilterStatusToMyCourseStatus: Map<string, MyLearningStatisticType> = new Map([
    [FilterMicrolearningStatus.InProgress.toString(), MyLearningStatisticType.InProgress],
    [FilterMicrolearningStatus.Completed.toString(), MyLearningStatisticType.Completed]
  ]);
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected courseDataService: CourseDataService,
    protected myLearningSearchDataService: MyLearningSearchDataService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.statusFilter = this.getMyCourseStatusFilter();
    this.filterByType = this.initSearchFilterType(this.currentActiveTab);
  }

  public getMyCoursesCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: LearningItemModel[];
  }> {
    const requestSearchMyCourses = this.getRequestSearchMyCourses(maxResultCount, skipCount);
    return this.courseDataService.getMyCourses(requestSearchMyCourses).pipe(
      map(result => {
        const items = result.items.map(p => new LearningItemModel(p));
        return {
          total: result.totalCount,
          items: items
        };
      })
    );
  }

  public onQuickStatusFilterChanged(event: string): void {
    this.currentMyStatusFilter = event as FilterMicrolearningStatus;
    this.triggerDataChange(true);
  }

  public onLearningCardClick(event: ILearningItemModel): void {
    if (event instanceof LearningItemModel) {
      this.learningCardClick.emit(event);
      return;
    }
  }

  public triggerDataChange(fromPage1: boolean = false): void {
    this.myCoursesListComponent.triggerDataChange(fromPage1);
  }

  public getPagedSearchFilterCallBack(searchModel: SearchFilterModel): Observable<MyLearningSearchFilterResult<LearningItemModel>> {
    return this.myLearningSearchDataService.searchCourses(searchModel);
  }

  protected getRequestSearchMyCourses(maxResultCount: number, skipCount: number): IMyCoursesSearchRequest {
    const requestSearchMyCourses: IMyCoursesSearchRequest = {
      maxResultCount,
      skipCount,
      searchText: this.searchingText,
      orderBy: this.currentMyStatusFilter === FilterMicrolearningStatus.Completed ? this.completedOrderBy : this.inProgressOrderBy,
      statusFilter: this.mapFilterStatusToMyCourseStatus.get(this.currentMyStatusFilter),
      courseType: LearningCourseType.Microlearning
    };
    return requestSearchMyCourses;
  }

  protected getMyCourseStatusFilter(): StatusFilterModel[] {
    return Object.keys(this.filterStatuses).map(status => {
      return new StatusFilterModel({
        status: status,
        displayText: this.translate(status)
      });
    });
  }

  private initSearchFilterType(tab: MyLearningTab): SearchFilterModel {
    return SEARCH_FILTER_TYPE_INITIALIZE.get(tab);
  }
}

enum FilterMicrolearningStatus {
  InProgress = 'InProgress',
  Completed = 'Completed'
}
