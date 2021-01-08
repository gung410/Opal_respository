import { Observable, of } from 'rxjs';

import { BrokenLinkModuleIdentifier } from '../dtos/broken-link-report-search-request';
import { BrokenLinkReportRepository } from '../repository/broken-link-report-repository';
import { BrokenLinkReportViewModel } from '../model/broken-link-report-view-model';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Injectable } from '@angular/core';
import { UserRepository } from '../../user/repositories/user.repository';
import { Utils } from '@opal20/infrastructure';
import { switchMap } from 'rxjs/operators';

Injectable();
export class BrokenLinkReportComponentService {
  constructor(private userRepository: UserRepository, private brokenLinkReportRepository: BrokenLinkReportRepository) {}

  public loadBrokenLinkReport(
    originalObjectId: string,
    parentIds: string[],
    module: BrokenLinkModuleIdentifier,
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner: boolean = true
  ): Observable<GridDataResult> {
    return this.brokenLinkReportRepository
      .searchBrokenLinkReport(originalObjectId, parentIds, module, skipCount, maxResultCount, showSpinner)
      .pipe(
        switchMap(brokenLinkReportSearchResult => {
          if (brokenLinkReportSearchResult.totalCount === 0) {
            return of(null);
          }
          return this.userRepository
            .loadPublicUserInfoList({ userIds: Utils.uniq(brokenLinkReportSearchResult.items.map(_ => _.reportBy)) }, showSpinner)
            .pipe(
              switchMap(usersList => {
                const userDic = Utils.toDictionary(usersList, p => p.id);
                const vmResult = <GridDataResult>{
                  data: brokenLinkReportSearchResult.items.map(_ => BrokenLinkReportViewModel.createViewModel(_, userDic[_.reportBy])),
                  total: brokenLinkReportSearchResult.totalCount
                };
                return of(vmResult);
              })
            );
        })
      );
  }
}
