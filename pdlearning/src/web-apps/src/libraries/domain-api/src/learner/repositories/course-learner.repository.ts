import { IPageUserReviewRequest, PagedUserReviewModelResult } from '../dtos/user-review-backend-service.dto';
import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { Injectable } from '@angular/core';
import { LearnerRepositoryContext } from '../learner-repository-context';
import { UserReviewApiService } from '../services/user-review-backend.service';

@Injectable()
export class CourseLearnerRepository extends BaseRepository<LearnerRepositoryContext> {
  constructor(context: LearnerRepositoryContext, private userReviewApiService: UserReviewApiService) {
    super(context);
  }

  public loadReviews(request: IPageUserReviewRequest): Observable<PagedUserReviewModelResult> {
    return this.processUpsertData(
      this.context.userReviewSubject,
      implicitLoad => from(this.userReviewApiService.getReviews(request)),
      'loadReviewsByCourse',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.id]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true,
      null,
      null
    );
  }
}
