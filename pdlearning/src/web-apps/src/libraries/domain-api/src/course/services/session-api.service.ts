import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { ISession, Session } from '../models/session.model';
import { IUpcomingSession, UpcomingSession } from '../models/upcoming-session.model';

import { CheckExistedSessionFieldRequest } from '../dtos/check-existed-session-field-request';
import { Constant } from '@opal20/authentication';
import { IChangeLearningMethodRequest } from './../dtos/change-learning-method-request';
import { ISaveSessionRequest } from './../dtos/save-session-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchSessionResult } from '../dtos/search-session-result';
import { SearchSessionType } from '../models/search-session-type.model';
import { map } from 'rxjs/operators';

@Injectable()
export class SessionApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public saveSession(request: ISaveSessionRequest): Promise<Session> {
    return this.post<ISaveSessionRequest, ISession>(`/session/save`, request)
      .pipe(map(data => new Session(data)))
      .toPromise();
  }

  public changeLearningMethod(request: IChangeLearningMethodRequest): Promise<Session> {
    return this.post<IChangeLearningMethodRequest, ISession>(`/session/changeLearningMethod`, request)
      .pipe(map(data => new Session(data)))
      .toPromise();
  }
  public getSessionById(id: string, showSpinner?: boolean): Promise<Session> {
    return this.get<ISession>(`/session/${id}`, null, showSpinner)
      .pipe(map(data => new Session(data)))
      .toPromise();
  }
  public getSessionCodeById(id: string, showSpinner?: boolean): Promise<Session> {
    return this.get<ISession>(`/session/${id}/code`, null, showSpinner)
      .pipe(map(data => new Session(data)))
      .toPromise();
  }
  public getSessionsByIds(classRunIds: string[]): Observable<Session[]> {
    return this.post<string[], ISession[]>(`/session/byClassRunIds`, classRunIds).pipe(map(result => result.map(_ => new Session(_))));
  }
  public getSessionsByClassRunId(
    classRunId: string,
    searchType?: SearchSessionType,
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner?: boolean
  ): Promise<SearchSessionResult> {
    return this.get<SearchSessionResult>(
      `/session/byClassRunId`,
      {
        classRunId,
        skipCount,
        searchType,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
      },
      showSpinner
    )
      .pipe(
        map(_ => {
          return new SearchSessionResult(_);
        })
      )
      .toPromise();
  }

  public checkExistedSessionField(request: CheckExistedSessionFieldRequest, showSpinner?: boolean): Promise<boolean> {
    return this.post<CheckExistedSessionFieldRequest, boolean>(`/session/checkExistedSessionField`, request, showSpinner).toPromise();
  }

  public deleteSession(id: string): Promise<void> {
    return this.delete<void>(`/session/${id}`).toPromise();
  }

  public getUpcomingSessionsByClassRunIds(classRunIds: string[], showSpinner?: boolean): Promise<UpcomingSession[]> {
    return this.post<string[], IUpcomingSession[]>(`/session/getUpcomingSessionByClassRunIds`, classRunIds, showSpinner)
      .pipe(map(response => response.map(_ => new UpcomingSession(_))))
      .toPromise();
  }

  public getMaxMinutesCanJoinWebinarEarly(showSpinner?: boolean): Promise<number> {
    return this.get<number>('/session/maxMinutesCanJoinWebinarEarly', null, showSpinner)
      .pipe(map(data => data))
      .toPromise();
  }
}
