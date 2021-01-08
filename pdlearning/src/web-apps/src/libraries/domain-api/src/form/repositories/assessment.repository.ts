import { Observable, from } from 'rxjs';

import { Assessment } from '../models/assessment.model';
import { AssessmentApiService } from '../services/assessment-api.service';
import { BaseRepository } from '@opal20/infrastructure';
import { Constant } from '@opal20/authentication';
import { FormRepositoryContext } from './../form-repository-context';
import { Injectable } from '@angular/core';
import { SearchAssessmentResult } from '../dtos/search-assessment-result';

@Injectable()
export class AssessmentRepository extends BaseRepository<FormRepositoryContext> {
  constructor(context: FormRepositoryContext, private apiSvc: AssessmentApiService) {
    super(context);
  }

  public loadAssessments(
    skipCount: number = 0,
    maxResultCount: number = Constant.MAX_ITEMS_PER_REQUEST,
    showSpinner: boolean = true
  ): Observable<SearchAssessmentResult> {
    return this.processUpsertData(
      this.context.assessmentSubject,
      implicitLoad => from(this.apiSvc.searchAssessments(skipCount, maxResultCount, !implicitLoad && showSpinner)),
      'loadAssessments',
      [skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id
    );
  }

  public loadAssessmentById(id: string): Observable<Assessment> {
    return this.processUpsertData(
      this.context.assessmentSubject,
      implicitLoad => from(this.apiSvc.getAssessmentById(id, !implicitLoad)),
      'loadAssessmentById',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id];
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id
    );
  }
}
