import { Observable, of } from 'rxjs';

import { AccessRightApiService } from './access-right-api.services';
import { AccessRightRepository } from '../repositories/access-right.repository';
import { AccessRightViewModel } from '../models/access-right-view-model';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Injectable } from '@angular/core';
import { UserRepository } from '../../user/repositories/user.repository';
import { Utils } from '@opal20/infrastructure';
import { switchMap } from 'rxjs/operators';

@Injectable()
export class AccessRightComponentService {
  constructor(
    private userRepository: UserRepository,
    private accessRightRepository: AccessRightRepository,
    private versionTrackingApiService: AccessRightApiService
  ) {}

  public loadCollaborators(
    originalObjectId: string,
    skipCount: number,
    maxResultCount: number,
    showSpinner: boolean = true
  ): Observable<GridDataResult> {
    return this.accessRightRepository.searchCollaborators(originalObjectId, skipCount, maxResultCount, showSpinner).pipe(
      switchMap(accessRightSearchResult => {
        if (accessRightSearchResult.totalCount === 0) {
          return of(null);
        }
        return this.userRepository
          .loadPublicUserInfoList({ userIds: Utils.uniq(accessRightSearchResult.items.map(_ => _.userId)) }, showSpinner)
          .pipe(
            switchMap(usersList => {
              const userDic = Utils.toDictionary(usersList, p => p.id);
              const vmResult = <GridDataResult>{
                data: accessRightSearchResult.items.map(_ => AccessRightViewModel.createFromModel(_, userDic[_.userId])),
                total: accessRightSearchResult.totalCount
              };
              return of(vmResult);
            })
          );
      })
    );
  }

  public loadAllCollaboratorsIds(originalObjectId: string): Observable<string[]> {
    return this.versionTrackingApiService.getAllListAccessRightId({ originalObjectId: originalObjectId });
  }
}
