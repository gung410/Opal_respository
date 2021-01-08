import { BaseBackendService, CommonFacadeService, IFilter } from '@opal20/infrastructure';
import { IParticipantAssignmentTrack, ParticipantAssignmentTrack } from '../models/participant-assignment-track.model';
import {
  ISearchParticipantAssignmentTrackResult,
  ParticipantAssignmentTrackResult
} from '../dtos/search-participant-assignment-track-result';

import { Constant } from '@opal20/authentication';
import { IAssignAssignmentRequest } from '../dtos/assign-assignment-request';
import { IMarkScoreForQuizQuestionAnswerRequest } from '../dtos/mark-score-for-quiz-question-answer-request';
import { ISaveAssignmentQuizAnswerRequest } from '../dtos/save-assignment-quiz-answer-request';
import { ISearchParticipantAssignmentTrackRequest } from './../dtos/search-participant-assignment-track-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class ParticipantAssignmentTrackApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getParticipantAssignmentTracks(
    courseId: string,
    classRunId: string,
    assignmentId: string,
    searchText: string = '',
    filter: IFilter = null,
    registrationIds: string[],
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    includeQuizAssignmentFormAnswer: boolean = false,
    showSpinner?: boolean
  ): Promise<ParticipantAssignmentTrackResult> {
    const request = {
      courseId: courseId,
      classRunId: classRunId,
      assignmentId: assignmentId,
      searchText: searchText,
      filter: filter,
      registrationIds: registrationIds,
      skipCount: skipCount,
      maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount,
      includeQuizAssignmentFormAnswer
    };
    return this.post<ISearchParticipantAssignmentTrackRequest, ParticipantAssignmentTrackResult>(
      '/participantAssignmentTrack/getParticipantAssignmentTracks',
      request,
      showSpinner
    )
      .pipe(
        map(_ => {
          return new ParticipantAssignmentTrackResult(_);
        })
      )
      .toPromise();
  }

  public getParticipantAssignmentTrackByAssignmentId(assignmentId: string, showSpinner?: boolean): Promise<ParticipantAssignmentTrack[]> {
    const request = {
      assignmentId: assignmentId
    };
    return this.get<IParticipantAssignmentTrack[]>('/participantAssignmentTrack/getByAssignmentId', request, showSpinner)
      .pipe(map(data => (data ? data.map(p => new ParticipantAssignmentTrack(p)) : [])))
      .toPromise();
  }

  public getParticipantAssignmentTrackById(id: string, showSpinner?: boolean): Promise<ParticipantAssignmentTrack> {
    return this.get<IParticipantAssignmentTrack>(`/participantAssignmentTrack/${id}`, null, showSpinner)
      .pipe(map(data => new ParticipantAssignmentTrack(data)))
      .toPromise();
  }

  public getParticipantAssignmentTrackByIds(
    participantAssignmentTrackIds: string[],
    showSpinner?: boolean
  ): Promise<ParticipantAssignmentTrack[]> {
    return this.post<string[], IParticipantAssignmentTrack[]>(
      `/participantAssignmentTrack/getByIds`,
      participantAssignmentTrackIds,
      showSpinner
    )
      .pipe(map(data => (data ? data.map(p => new ParticipantAssignmentTrack(p)) : [])))
      .toPromise();
  }

  public markScoreManuallyForQuizAssignmentQuestionAnswer(
    request: IMarkScoreForQuizQuestionAnswerRequest,
    showSpinner?: boolean
  ): Promise<ParticipantAssignmentTrack> {
    return this.post<IMarkScoreForQuizQuestionAnswerRequest, IParticipantAssignmentTrack>(
      `/participantAssignmentTrack/markScore`,
      request,
      showSpinner
    )
      .pipe(map(data => new ParticipantAssignmentTrack(data)))
      .toPromise();
  }

  public saveAssignmentQuizAnswer(request: ISaveAssignmentQuizAnswerRequest, showSpinner?: boolean): Promise<ParticipantAssignmentTrack> {
    return this.post<ISaveAssignmentQuizAnswerRequest, IParticipantAssignmentTrack>(
      `/participantAssignmentTrack/saveAssignmentQuizAnswer`,
      request,
      showSpinner
    )
      .pipe(map(data => new ParticipantAssignmentTrack(data)))
      .toPromise();
  }

  public assignAssignment(request: IAssignAssignmentRequest): Promise<ParticipantAssignmentTrackResult> {
    return this.post<IAssignAssignmentRequest, ISearchParticipantAssignmentTrackResult>(
      `/participantAssignmentTrack/assignAssignment`,
      request
    )
      .pipe(map(data => new ParticipantAssignmentTrackResult(data)))
      .toPromise();
  }
}
