import { AssessmentAnswer, IAssessmentAnswer } from '../models/assessment-answer.model';
import { BaseBackendService, CommonFacadeService, IFilter } from '@opal20/infrastructure';
import { INoOfAssessmentDoneInfo, NoOfAssessmentDoneInfo } from '../models/no-of-assessment-done-info.model';

import { Constant } from '@opal20/authentication';
import { ICreateAssessmentAnswerRequest } from '../dtos/create-assessment-answer-request';
import { ISaveAssessmentAnswerRequest } from '../dtos/save-assessment-answer-request';
import { ISearchAssessmentAnswerRequest } from '../../course/dtos/search-assessment-answer-request';
import { Injectable } from '@angular/core';
import { SearchAssessmentAnswerResult } from '../../course/dtos/search-assessment-answer-result';
import { map } from 'rxjs/operators';

@Injectable()
export class AssessmentAnswerApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getAssessmentAnswerById(
    id: string,
    participantAssignmentTrackId?: string,
    userId?: string,
    showSpinner?: boolean
  ): Promise<AssessmentAnswer> {
    const request = {
      id: id,
      participantAssignmentTrackId: participantAssignmentTrackId,
      userId: userId
    };
    return this.get<IAssessmentAnswer>(`/assessmentAnswer/byIdOrUser`, request, showSpinner)
      .pipe(map(data => new AssessmentAnswer(data)))
      .toPromise();
  }

  public createAssessmentAnswer(request: ICreateAssessmentAnswerRequest): Promise<AssessmentAnswer> {
    return this.post<ICreateAssessmentAnswerRequest, IAssessmentAnswer>(`/assessmentAnswer/createAssessmentAnswer`, request)
      .pipe(map(data => new AssessmentAnswer(data)))
      .toPromise();
  }

  public deleteAssessmentAnswer(id: string): Promise<void> {
    return this.delete<void>(`/assessmentAnswer/${id}`).toPromise();
  }

  public searchAssessmentAnswer(
    participantAssignmentTrackId?: string,
    userId?: string,
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Promise<SearchAssessmentAnswerResult> {
    const request: ISearchAssessmentAnswerRequest = {
      participantAssignmentTrackId: participantAssignmentTrackId,
      userId: userId,
      searchText: searchText,
      filter: filter,
      skipCount: skipCount,
      maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
    };
    return this.post<ISearchAssessmentAnswerRequest, SearchAssessmentAnswerResult>(
      `/assessmentAnswer/searchAssessmentAnswer`,
      request,
      showSpinner
    )
      .pipe(
        map(_ => {
          return _;
        })
      )
      .toPromise();
  }

  public saveAssessmentAnswer(request: ISaveAssessmentAnswerRequest, showSpinner?: boolean): Promise<AssessmentAnswer> {
    return this.post<ISaveAssessmentAnswerRequest, IAssessmentAnswer>(`/assessmentAnswer/save`, request, showSpinner)
      .pipe(map(data => new AssessmentAnswer(data)))
      .toPromise();
  }

  public getNoOfAssessmentDones(participantAssignmentTrackIds: string[], showSpinner?: boolean): Promise<NoOfAssessmentDoneInfo[]> {
    const request = {
      participantAssignmentTrackIds: participantAssignmentTrackIds
    };
    return this.get<INoOfAssessmentDoneInfo[]>('/assessmentAnswer/getNoOfAssessmentDones', request, showSpinner)
      .pipe(
        map(_ => {
          return _.map(p => new NoOfAssessmentDoneInfo(p));
        })
      )
      .toPromise();
  }
}
