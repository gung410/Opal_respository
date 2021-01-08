import { FormParticipantForm, FormParticipantRepository, FormParticipantViewModel, UserRepository } from '@opal20/domain-api';
import { Observable, of } from 'rxjs';

import { GridDataResult } from '@progress/kendo-angular-grid';
import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';
import { switchMap } from 'rxjs/operators';

@Injectable()
export class FormParticipantListService {
  constructor(private userRepository: UserRepository, private formParticipantRepository: FormParticipantRepository) {}

  public loadFormParticipants(
    formOriginalObjectId: string,
    skipCount: number,
    maxResultCount: number,
    showSpinner: boolean = true
  ): Observable<GridDataResult> {
    return this.formParticipantRepository.searchFormParticipants(formOriginalObjectId, skipCount, maxResultCount, showSpinner).pipe(
      switchMap(formParticipantSearchResult => {
        if (formParticipantSearchResult.totalCount === 0) {
          return of(null);
        }
        return this.userRepository
          .loadPublicUserInfoList({ userIds: Utils.uniq(formParticipantSearchResult.items.map(_ => _.userId)) }, showSpinner)
          .pipe(
            switchMap(usersList => {
              const userDic = Utils.toDictionary(usersList, p => p.id);
              const vmResult = <GridDataResult>{
                data: formParticipantSearchResult.items.map(_ => FormParticipantViewModel.createFromParticipantModel(_, userDic[_.userId])),
                total: formParticipantSearchResult.totalCount
              };
              return of(vmResult);
            })
          );
      })
    );
  }

  public getFormParticipantsByFormIds(formIds: string[], showSpinner: boolean = true): Observable<FormParticipantForm[]> {
    return this.formParticipantRepository.getFormParticipantsByFormIds(formIds, showSpinner);
  }
}
