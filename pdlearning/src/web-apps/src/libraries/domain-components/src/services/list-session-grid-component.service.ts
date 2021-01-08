import { SearchSessionResult, SearchSessionType, SessionRepository } from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SessionViewModel } from '../models/session-view.model';
import { map } from 'rxjs/operators';

@Injectable()
export class ListSessionGridComponentService {
  constructor(private sessionRepository: SessionRepository) {}

  public loadSessionsByClassRunId(
    classRunId: string = undefined,
    searchType: SearchSessionType = SearchSessionType.Owner,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<SessionViewModel>> {
    return this.progressSessions(
      this.sessionRepository.loadSessionsByClassRunId(classRunId, searchType, skipCount, maxResultCount),
      checkAll,
      selectedsFn
    );
  }

  private progressSessions(
    sessionObs: Observable<SearchSessionResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<SessionViewModel>> {
    return sessionObs.pipe(
      map(searchSessionResult => {
        return <OpalGridDataResult<SessionViewModel>>{
          data: searchSessionResult.items.map(session => {
            return SessionViewModel.createFromModel(session, checkAll, selectedsFn != null ? selectedsFn() : {});
          }),
          total: searchSessionResult.totalCount
        };
      })
    );
  }
}
