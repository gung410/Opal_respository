import { BaseRepository, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { CoursePlanningCycle } from '../models/course-planning-cycle.model';
import { CoursePlanningCycleApiService } from '../services/course-planning-cycle-api.service';
import { CourseRepositoryContext } from '../course-repository-context';
import { ISaveCoursePlanningCycleRequest } from '../dtos/save-course-planning-cycle-request';
import { Injectable } from '@angular/core';
import { SearchCoursePlanningCycleResult } from '../dtos/search-course-planning-cycle-result';

@Injectable()
export class CoursePlanningCycleRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: CoursePlanningCycleApiService) {
    super(context);
  }

  public loadCoursePlanningCycleById(id: string): Observable<CoursePlanningCycle> {
    return this.processUpsertData(
      this.context.coursePlanningCycleSubject,
      implicitLoad => from(this.apiSvc.getCoursePlanningCycleById(id, !implicitLoad)),
      'loadCoursePlanningCycleById',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id];
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id,
      true,
      null,
      null,
      CoursePlanningCycle.optionalProps
    );
  }

  public loadCoursePlanningCycles(
    searchText: string = '',
    skipCount: number,
    maxResultCount: number
  ): Observable<SearchCoursePlanningCycleResult> {
    return this.processUpsertData(
      this.context.coursePlanningCycleSubject,
      implicitLoad => from(this.apiSvc.getCoursePlanningCycles(searchText, skipCount, maxResultCount, !implicitLoad)),
      'loadCoursePlanningCycles',
      [searchText, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true,
      null,
      null,
      CoursePlanningCycle.optionalProps
    );
  }

  public loadCoursePlanningCyclesByIds(ids: string[]): Observable<CoursePlanningCycle[]> {
    return this.processUpsertData(
      this.context.coursePlanningCycleSubject,
      implicitLoad => from(this.apiSvc.getCoursePlanningCyclesByIds(ids, !implicitLoad)),
      'loadCoursePlanningCyclesByIds',
      [ids],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id,
      true,
      null,
      null,
      CoursePlanningCycle.optionalProps
    );
  }

  public saveCoursePlanningCycle(request: ISaveCoursePlanningCycleRequest): Observable<CoursePlanningCycle> {
    return from(
      this.apiSvc.saveCoursePlanningCycle(request).then(coursePlanningCycle => {
        this.upsertData(
          this.context.coursePlanningCycleSubject,
          [new CoursePlanningCycle(Utils.cloneDeep(coursePlanningCycle))],
          item => item.id,
          true,
          null,
          CoursePlanningCycle.optionalProps
        );
        return coursePlanningCycle;
      })
    );
  }
}
