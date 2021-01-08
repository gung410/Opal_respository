import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { IMyCoursesSearchRequest } from '../dtos/my-course-search-request.dto';
import { Injectable } from '@angular/core';
import { LearnerRepositoryContext } from '../learner-repository-context';
import { MyCourseApiService } from '../services/my-course-backend.service';
import { MyCourseResultModel } from '../models/my-course-result.model';
import { PagedCourseModelResult } from '../dtos/my-course-backend-service.dto';

@Injectable()
export class MyCourseRepository extends BaseRepository<LearnerRepositoryContext> {
  constructor(context: LearnerRepositoryContext, private myCourseApiService: MyCourseApiService) {
    super(context);
  }

  public loadMyCourses(request: IMyCoursesSearchRequest): Observable<PagedCourseModelResult> {
    return this.processUpsertData(
      this.context.myCourseResultSubject,
      implicitLoad => from(this.myCourseApiService.getMyCourses(request, !implicitLoad)),
      'loadMyCourses',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.courseId]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.courseId
    );
  }

  public loadMyCoursesByCourseIds(courseIds: string[]): Observable<MyCourseResultModel[]> {
    return this.processUpsertData(
      this.context.myCourseResultSubject,
      implicitLoad => from(this.myCourseApiService.getByCourseIds(courseIds, !implicitLoad)),
      'loadMyCoursesByCourseIds',
      courseIds,
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(a => repoData[a.courseId]);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.courseId
    );
  }
}
