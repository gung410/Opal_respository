import { CoursePlanningCycleRepository, SearchCoursePlanningCycleResult } from '@opal20/domain-api';

import { CoursePlanningCycleViewModel } from '../models/course-planning-cycle-view.model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class ListCoursePlanningCycleGridComponentService {
  constructor(private coursePlanningCycleRepository: CoursePlanningCycleRepository) {}

  public loadCoursePlanningCycles(
    searchText: string = '',
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<CoursePlanningCycleViewModel>> {
    return this.progressCoursePlanningCycles(
      this.coursePlanningCycleRepository.loadCoursePlanningCycles(searchText, skipCount, maxResultCount),
      checkAll,
      selectedsFn
    );
  }

  private progressCoursePlanningCycles(
    coursePlanningCycleObs: Observable<SearchCoursePlanningCycleResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<CoursePlanningCycleViewModel>> {
    return coursePlanningCycleObs.pipe(
      map(searchCoursePlanningCycleResult => {
        return <OpalGridDataResult<CoursePlanningCycleViewModel>>{
          data: searchCoursePlanningCycleResult.items.map(_ =>
            CoursePlanningCycleViewModel.createFromModel(_, checkAll, selectedsFn != null ? selectedsFn() : {})
          ),
          total: searchCoursePlanningCycleResult.totalCount
        };
      })
    );
  }
}
