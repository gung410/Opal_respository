import { Observable, from } from 'rxjs';

import { BadgeRepositoryContext } from '../badge-repository-context';
import { BaseRepository } from '@opal20/infrastructure';
import { IAwardBadgeRequest } from '../dtos/award-badge-request';
import { Injectable } from '@angular/core';
import { SearchTopBadgeUserStatisticResult } from '../dtos/search-top-badge-user-statistic-result';
import { YearlyUserStatisticApiService } from './../services/yearly-user-statistic-api.service';

@Injectable()
export class YearlyUserStatisticRepository extends BaseRepository<BadgeRepositoryContext> {
  constructor(context: BadgeRepositoryContext, private apiSvc: YearlyUserStatisticApiService) {
    super(context);
  }

  public searchTopBadgeUserStatistics(
    badgeId: string,
    searchText: string = '',
    skipCount: number = 0,
    maxResultCount: number = 10
  ): Observable<SearchTopBadgeUserStatisticResult> {
    return this.processUpsertData(
      this.context.yearlyUserStatisticsSubject,
      implicitLoad => from(this.apiSvc.searchTopBadgeUserStatistics(badgeId, searchText, skipCount, maxResultCount, !implicitLoad)),
      'badges/searchTopBadgeUserStatistics',
      [searchText, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true
    );
  }

  public awardBadge(request: IAwardBadgeRequest): Observable<void> {
    return from(
      this.apiSvc.awardBadge(request).then(_ => {
        this.processRefreshData('searchYearlyUserStatistics');
      })
    );
  }
}
