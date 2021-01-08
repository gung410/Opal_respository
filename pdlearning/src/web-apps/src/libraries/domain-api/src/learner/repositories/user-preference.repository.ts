import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { IUpdateUserPreferenceRequest } from '../dtos/user-preferences.dto';
import { Injectable } from '@angular/core';
import { LearnerRepositoryContext } from '../learner-repository-context';
import { UserPreferenceAPIService } from '../services/user-preferences-backend.service';
import { UserPreferenceModel } from '../models/user-preferences.model';

@Injectable()
export class UserPreferenceRepository extends BaseRepository<LearnerRepositoryContext> {
  constructor(private userPreferenceApi: UserPreferenceAPIService, context: LearnerRepositoryContext) {
    super(context);
  }

  public loadUserPreferences(): Observable<UserPreferenceModel[]> {
    const request = [];
    return this.processUpsertData(
      this.context.userPreferenceSubject,
      implicitLoad => from(this.userPreferenceApi.getAllUserPreferences(request)),
      'loadUserPreferences',
      request,
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(a => repoData[a.id]);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id,
      true,
      null,
      null
    );
  }

  public updateUserPreferences(needUpdateUPs: UserPreferenceModel[]): Observable<void> {
    const request = needUpdateUPs.map(u => <IUpdateUserPreferenceRequest>{ key: u.key, valueString: JSON.stringify(u.value) });
    return from(
      this.userPreferenceApi.updateUserPreferences(request).then(() => {
        this.processRefreshData('loadUserPreferences', []);
      })
    );
  }
}
