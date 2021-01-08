import { BaseRepository, IFilter } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { CourseRepositoryContext } from '../course-repository-context';
import { IAssignAssignmentRequest } from '../dtos/assign-assignment-request';
import { IMarkScoreForQuizQuestionAnswerRequest } from '../dtos/mark-score-for-quiz-question-answer-request';
import { ISaveAssignmentQuizAnswerRequest } from '../dtos/save-assignment-quiz-answer-request';
import { Injectable } from '@angular/core';
import { ParticipantAssignmentTrack } from '../models/participant-assignment-track.model';
import { ParticipantAssignmentTrackApiService } from '../services/participant-assignment-track-api.service';
import { ParticipantAssignmentTrackResult } from '../dtos/search-participant-assignment-track-result';

@Injectable()
export class ParticipantAssignmentTrackRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: ParticipantAssignmentTrackApiService) {
    super(context);
  }
  public loadParticipantAssignmentTracks(
    courseId: string,
    classRunId: string,
    assignmentId: string,
    searchText: string = '',
    filter: IFilter = null,
    registrationIds: string[],
    skipCount: number = 0,
    maxResultCount: number = 10,
    includeQuizAssignmentFormAnswer: boolean = false
  ): Observable<ParticipantAssignmentTrackResult> {
    return this.processUpsertData(
      this.context.participantAssignmentTrackSubject,
      implicitLoad =>
        from(
          this.apiSvc.getParticipantAssignmentTracks(
            courseId,
            classRunId,
            assignmentId,
            searchText,
            filter,
            registrationIds,
            skipCount,
            maxResultCount,
            includeQuizAssignmentFormAnswer,
            !implicitLoad
          )
        ),
      'loadParticipantAssignmentTracks',
      [courseId, classRunId, assignmentId, searchText, filter, registrationIds, skipCount, maxResultCount, includeQuizAssignmentFormAnswer],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      null,
      null,
      null,
      ParticipantAssignmentTrack.optionalProps
    );
  }

  public getParticipantAssignmentTrackById(id: string): Observable<ParticipantAssignmentTrack> {
    return this.processUpsertData(
      this.context.participantAssignmentTrackSubject,
      implicitLoad => from(this.apiSvc.getParticipantAssignmentTrackById(id, !implicitLoad)),
      'getParticipantAssignmentTrackById',
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
      ParticipantAssignmentTrack.optionalProps
    );
  }

  public assignAssignment(request: IAssignAssignmentRequest): Observable<ParticipantAssignmentTrackResult> {
    return from(
      this.apiSvc.assignAssignment(request).then(result => {
        this.processRefreshData('loadParticipantAssignmentTracks');
        return result;
      })
    );
  }

  public markScoreManuallyForQuizAssignmentQuestionAnswer(
    request: IMarkScoreForQuizQuestionAnswerRequest
  ): Observable<ParticipantAssignmentTrack> {
    return from(
      this.apiSvc.markScoreManuallyForQuizAssignmentQuestionAnswer(request).then(result => {
        this.upsertData(
          this.context.participantAssignmentTrackSubject,
          [result],
          p => p.id,
          true,
          null,
          ParticipantAssignmentTrack.optionalProps
        );
        return result;
      })
    );
  }

  public saveAssignmentQuizAnswer(request: ISaveAssignmentQuizAnswerRequest): Observable<ParticipantAssignmentTrack> {
    return from(
      this.apiSvc.saveAssignmentQuizAnswer(request).then(result => {
        this.upsertData(
          this.context.participantAssignmentTrackSubject,
          [result],
          p => p.id,
          true,
          null,
          ParticipantAssignmentTrack.optionalProps
        );
        return result;
      })
    );
  }
}
