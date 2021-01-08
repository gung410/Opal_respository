import { BaseRepository, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { CourseCriteria } from './../models/course-criteria.model';
import { CourseCriteriaApiService } from '../services/course-criteria-api.service';
import { CourseRepositoryContext } from '../course-repository-context';
import { ISaveCourseCriteriaRequest } from './../dtos/save-course-criteria-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class CourseCriteriaRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: CourseCriteriaApiService) {
    super(context);
  }

  public loadCourseCriteria(id: string): Observable<CourseCriteria | null> {
    return this.processUpsertData(
      this.context.courseCriteriaSubject,
      implicitLoad => from(this.apiSvc.getCourseCriteria(id, !implicitLoad)).pipe(map(p => (p ? p : new CourseCriteria({ id: id })))),
      'loadCourseCriteria',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult != null ? repoData[apiResult.id] : null;
        return apiResult;
      },
      apiResult => (apiResult != null ? [apiResult] : []),
      x => x.id,
      true,
      null,
      null
    );
  }

  public saveCourseCriteria(request: ISaveCourseCriteriaRequest): Observable<CourseCriteria> {
    return from(
      this.apiSvc.saveCourseCriteria(request).then(courseCriteria => {
        this.upsertData(
          this.context.courseCriteriaSubject,
          [new CourseCriteria(Utils.cloneDeep(courseCriteria))],
          item => item.id,
          true,
          null
        );
        return courseCriteria;
      })
    );
  }
}
