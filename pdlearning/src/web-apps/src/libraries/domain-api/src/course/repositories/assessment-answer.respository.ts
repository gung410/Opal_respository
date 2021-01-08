import { BaseRepository, IFilter, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { AssessmentAnswer } from '../models/assessment-answer.model';
import { AssessmentAnswerApiService } from '../services/assessment-answer-api.service';
import { CourseRepositoryContext } from '../course-repository-context';
import { ICreateAssessmentAnswerRequest } from '../dtos/create-assessment-answer-request';
import { ISaveAssessmentAnswerRequest } from '../dtos/save-assessment-answer-request';
import { Injectable } from '@angular/core';
import { NoOfAssessmentDoneInfo } from './../models/no-of-assessment-done-info.model';
import { SearchAssessmentAnswerResult } from '../dtos/search-assessment-answer-result';

@Injectable()
export class AssessmentAnswerRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: AssessmentAnswerApiService) {
    super(context);
  }

  public loadAssessmentAnswerById(id: string, participantAssignmentTrackId?: string, userId?: string): Observable<AssessmentAnswer> {
    return this.processUpsertData(
      this.context.assessmentAnswerSubject,
      implicitLoad => from(this.apiSvc.getAssessmentAnswerById(id, participantAssignmentTrackId, userId, !implicitLoad)),
      'loadAssessmentAnswerById',
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

  public saveAssessmentAnswer(request: ISaveAssessmentAnswerRequest): Observable<AssessmentAnswer> {
    return from(
      this.apiSvc.saveAssessmentAnswer(request).then(result => {
        this.upsertData(this.context.assessmentAnswerSubject, [result], p => p.id, true);
        return result;
      })
    );
  }

  public loadNoOfAssessmentDone(participantAssignmentTrackIds: string[]): Observable<NoOfAssessmentDoneInfo[]> {
    return this.processUpsertData(
      this.context.noOfAssessmentDoneInfoTrackingSubject,
      implicitLoad => from(this.apiSvc.getNoOfAssessmentDones(participantAssignmentTrackIds, !implicitLoad)),
      'loadNoOfAssessmentDone',
      [participantAssignmentTrackIds],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.participantAssignmentTrackId]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.participantAssignmentTrackId
    );
  }

  public createAssessmentAnswer(request: ICreateAssessmentAnswerRequest): Observable<AssessmentAnswer> {
    return from(
      this.apiSvc.createAssessmentAnswer(request).then(assessment => {
        this.upsertData(this.context.assessmentAnswerSubject, [Utils.cloneDeep(assessment)], item => item.id, true);
        this.processRefreshData('searchAssessmentAnswer');
        return assessment;
      })
    );
  }

  public deleteAssessmentAnswer(id: string, participantAssignmentTrackId?: string): Promise<void> {
    return this.apiSvc.deleteAssessmentAnswer(id).then(_ => {
      this.processRefreshData('searchAssessmentAnswer');
    });
  }

  public loadSearchAssessmentAnswer(
    participantAssignmentTrackId?: string,
    userId?: string,
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Observable<SearchAssessmentAnswerResult> {
    return this.processUpsertData(
      this.context.assessmentAnswerSubject,
      implicitLoad =>
        from(
          this.apiSvc.searchAssessmentAnswer(
            participantAssignmentTrackId,
            userId,
            searchText,
            filter,
            skipCount,
            maxResultCount,
            !implicitLoad && showSpinner
          )
        ),
      'searchAssessmentAnswer',
      [participantAssignmentTrackId, userId, searchText, filter, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true
    );
  }
}
