import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { IUserPreferenceModel, UserPreferenceModel } from '../models/user-preferences.model';

import { IUpdateUserPreferenceRequest } from '../dtos/user-preferences.dto';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class UserPreferenceAPIService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/userPreferences';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getAllUserPreferences(keys?: string[]): Promise<UserPreferenceModel[]> {
    return this.post<string[], IUserPreferenceModel[]>(`/get`, keys)
      .pipe(
        map(results => {
          if (results && results instanceof Array) {
            return results.map(_ => new UserPreferenceModel(_));
          }

          return [];
        })
      )
      .toPromise();
  }

  public updateUserPreferences(request: IUpdateUserPreferenceRequest[]): Promise<void> {
    return this.put<IUpdateUserPreferenceRequest[], void>('', request).toPromise();
  }
}
