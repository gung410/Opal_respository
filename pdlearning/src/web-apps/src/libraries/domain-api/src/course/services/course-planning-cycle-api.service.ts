import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { CoursePlanningCycle, ICoursePlanningCycle } from '../models/course-planning-cycle.model';

import { Constant } from '@opal20/authentication';
import { ISaveCoursePlanningCycleRequest } from './../dtos/save-course-planning-cycle-request';
import { Injectable } from '@angular/core';
import { SearchCoursePlanningCycleResult } from './../dtos/search-course-planning-cycle-result';
import { map } from 'rxjs/operators';

@Injectable()
export class CoursePlanningCycleApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getCoursePlanningCycleById(id: string, showSpinner?: boolean): Promise<CoursePlanningCycle> {
    return this.get<ICoursePlanningCycle>(`/coursePlanningCycle/${id}`, null, showSpinner)
      .pipe(map(data => new CoursePlanningCycle(data)))
      .toPromise();
  }

  public getCoursePlanningCycles(
    searchText: string = '',
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner?: boolean
  ): Promise<SearchCoursePlanningCycleResult> {
    return this.get<SearchCoursePlanningCycleResult>(
      '/coursePlanningCycle/search',
      {
        searchText,
        skipCount,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
      },
      showSpinner
    )
      .pipe(map(_ => new SearchCoursePlanningCycleResult(_)))
      .toPromise();
  }

  public getCoursePlanningCyclesByIds(ids: string[], showSpinner?: boolean): Promise<CoursePlanningCycle[]> {
    return this.post<string[], CoursePlanningCycle[]>(`/coursePlanningCycle/getByIds`, ids, showSpinner)
      .pipe(map(data => data.map(a => new CoursePlanningCycle(a))))
      .toPromise();
  }

  public saveCoursePlanningCycle(request: ISaveCoursePlanningCycleRequest): Promise<CoursePlanningCycle> {
    return this.post<ISaveCoursePlanningCycleRequest, ICoursePlanningCycle>(`/coursePlanningCycle/save`, request)
      .pipe(map(data => new CoursePlanningCycle(data)))
      .toPromise();
  }
}
