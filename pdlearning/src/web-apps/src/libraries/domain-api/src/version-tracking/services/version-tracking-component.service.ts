import { Observable, of } from 'rxjs';

import { GridDataResult } from '@progress/kendo-angular-grid';
import { Injectable } from '@angular/core';
import { UserRepository } from '../../user/repositories/user.repository';
import { Utils } from '@opal20/infrastructure';
import { VersionTrackingRepository } from '../repositories/version-tracking.repository';
import { VersionTrackingViewModel } from '../models/version-tracking-view-model';
import { switchMap } from 'rxjs/operators';

@Injectable()
export class VersionTrackingComponentService {
  constructor(private userRepository: UserRepository, private versionTrackingRepository: VersionTrackingRepository) {}

  public loadVersionTrackings(
    originalObjectId: string,
    skipCount: number,
    maxResultCount: number,
    showSpinner: boolean = true
  ): Observable<GridDataResult> {
    return this.versionTrackingRepository.searchVersionTrackings(originalObjectId, skipCount, maxResultCount, showSpinner).pipe(
      switchMap(versionTrackingSearchResult => {
        if (versionTrackingSearchResult.totalCount === 0) {
          return of(null);
        }
        return this.userRepository
          .loadPublicUserInfoList({ userIds: Utils.uniq(versionTrackingSearchResult.items.map(_ => _.changedByUserId)) }, showSpinner)
          .pipe(
            switchMap(usersList => {
              const userDic = Utils.toDictionary(usersList, p => p.id);
              const vmResult = <GridDataResult>{
                data: versionTrackingSearchResult.items.map(_ => VersionTrackingViewModel.createFromModel(_, userDic[_.changedByUserId])),
                total: versionTrackingSearchResult.totalCount
              };
              return of(vmResult);
            })
          );
      })
    );
  }
}
