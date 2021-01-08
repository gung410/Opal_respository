import { BaseRepository, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { Constant } from '@opal20/authentication';
import { CourseRepositoryContext } from '../course-repository-context';
import { IChangeLearningMethodRequest } from '../dtos/change-learning-method-request';
import { ISaveSessionRequest } from '../dtos/save-session-request';
import { Injectable } from '@angular/core';
import { SearchSessionResult } from '../dtos/search-session-result';
import { SearchSessionType } from '../models/search-session-type.model';
import { Session } from '../models/session.model';
import { SessionApiService } from '../services/session-api.service';
import { UpcomingSession } from '../models/upcoming-session.model';

@Injectable()
export class SessionRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: SessionApiService) {
    super(context);
  }

  public loadSessionById(id: string): Observable<Session> {
    return this.processUpsertData(
      this.context.sessionSubject,
      implicitLoad => from(this.apiSvc.getSessionById(id, !implicitLoad)),
      'loadSessionById',
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
      Session.optionalProps
    );
  }

  public loadSessionCodeById(id: string): Observable<Session> {
    return this.processUpsertData(
      this.context.sessionSubject,
      implicitLoad => from(this.apiSvc.getSessionCodeById(id, !implicitLoad)),
      'loadSessionCodeById',
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
      Session.optionalProps
    );
  }

  public loadSessionsByClassRunId(
    classRunId: string,
    searchType: SearchSessionType = SearchSessionType.Owner,
    skipCount: number = 0,
    maxResultCount: number = Constant.MAX_ITEMS_PER_REQUEST,
    showSpinner: boolean = true
  ): Observable<SearchSessionResult> {
    return this.processUpsertData(
      this.context.sessionSubject,
      implicitLoad =>
        from(this.apiSvc.getSessionsByClassRunId(classRunId, searchType, skipCount, maxResultCount, !implicitLoad && showSpinner)),
      'loadSessionsByClassRunId',
      [classRunId, searchType, skipCount, maxResultCount],
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
      Session.optionalProps
    );
  }

  public saveSession(request: ISaveSessionRequest): Observable<Session> {
    return from(
      this.apiSvc.saveSession(request).then(session => {
        this.upsertData(
          this.context.sessionSubject,
          [new Session(Utils.cloneDeep(request.data))],
          item => item.id,
          true,
          null,
          Session.optionalProps
        );
        return session;
      })
    );
  }

  public changeLearningMethod(request: IChangeLearningMethodRequest): Promise<void> {
    return this.apiSvc.changeLearningMethod(request).then(_ => {
      this.upsertData(
        this.context.sessionSubject,
        [<Partial<Session>>{ id: request.id, learningMethod: request.learningMethod }],
        item => item.id
      );
    });
  }

  public deleteSession(id: string): Promise<void> {
    return this.apiSvc.deleteSession(id).then(_ => {
      this.processRefreshData('loadSessionsByClassRunId');
    });
  }

  public loadUpcomingSessionsByClassRunIds(classRunIds: string[]): Observable<UpcomingSession[]> {
    return this.processUpsertData(
      this.context.upcomingSessionSubject,
      implicitLoad => from(this.apiSvc.getUpcomingSessionsByClassRunIds(classRunIds, !implicitLoad)),
      'loadUpcomingSessionsByClassRunIds',
      [classRunIds],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(p => repoData[p.classRunId]);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.classRunId
    );
  }

  public loadMaxMinutesCanJoinWebinarEarly(): Observable<number> {
    const settingId = 'loadMaxMinutesCanJoinWebinarEarly';
    return this.processUpsertData(
      this.context.appSettingsSubject,
      implicitLoad => from(this.apiSvc.getMaxMinutesCanJoinWebinarEarly(!implicitLoad)),
      'loadMaxMinutesCanJoinWebinarEarly',
      [],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = <number>repoData[settingId];
        return apiResult;
      },
      apiResult => [apiResult],
      x => settingId
    );
  }
}
