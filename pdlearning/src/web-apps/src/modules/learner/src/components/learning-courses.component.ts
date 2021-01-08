import { Component, Input } from '@angular/core';
import { IMyCoursesSearchRequest, LearningCourseType, MyLearningStatisticType } from '@opal20/domain-api';

import { CourseDataService } from '../services/course-data.service';
import { LearningMicrolearningComponent } from './learning-microlearning.component';
import { ModuleFacadeService } from '@opal20/infrastructure';
import { MyLearningSearchDataService } from '../services/my-learning-search.service';
import { MyLearningTab } from '@opal20/domain-components';

@Component({
  selector: 'learning-courses',
  templateUrl: './learning-microlearning.component.html'
})
export class LearningCoursesComponent extends LearningMicrolearningComponent {
  @Input()
  public searchingText: string;

  @Input()
  public isSearchingWithText: boolean;

  public filterStatuses = FilterCourseStatus;
  public currentMyStatusFilter: string = FilterCourseStatus.Registered;
  public currentActiveTab: MyLearningTab = MyLearningTab.Courses;

  constructor(
    moduleFacadeService: ModuleFacadeService,
    courseDataService: CourseDataService,
    myLearningSearchDataService: MyLearningSearchDataService
  ) {
    super(moduleFacadeService, courseDataService, myLearningSearchDataService);
  }

  public onQuickStatusFilterChanged(event: string): void {
    this.currentMyStatusFilter = event as FilterCourseStatus;
    this.triggerDataChange(true);
  }

  protected getRequestSearchMyCourses(maxResultCount: number, skipCount: number): IMyCoursesSearchRequest {
    const requestSearchMyCourses: IMyCoursesSearchRequest = {
      maxResultCount,
      skipCount,
      searchText: undefined,
      orderBy: this.currentMyStatusFilter === FilterCourseStatus.Completed ? this.completedOrderBy : this.inProgressOrderBy,
      courseType: LearningCourseType.FaceToFace
    };
    switch (this.currentMyStatusFilter) {
      case FilterCourseStatus.Registered:
        requestSearchMyCourses.statusFilter = MyLearningStatisticType.Registered;
        break;
      case FilterCourseStatus.Upcoming:
        requestSearchMyCourses.statusFilter = MyLearningStatisticType.Upcoming;
        break;
      case FilterCourseStatus.InProgress:
        requestSearchMyCourses.statusFilter = MyLearningStatisticType.InProgress;
        break;
      case FilterCourseStatus.Completed:
        requestSearchMyCourses.statusFilter = MyLearningStatisticType.Completed;
        break;
    }
    return requestSearchMyCourses;
  }
}

enum FilterCourseStatus {
  Registered = 'Registered',
  Upcoming = 'Upcoming',
  InProgress = 'InProgress',
  Completed = 'Completed'
}
