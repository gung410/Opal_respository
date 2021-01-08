import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { ISearchTopBadgeUserStatisticResult, SearchTopBadgeUserStatisticResult } from '../dtos/search-top-badge-user-statistic-result';

import { Constant } from '@opal20/authentication';
import { IAwardBadgeRequest } from '../dtos/award-badge-request';
import { ISearchTopBadgeUserStatisticRequest } from '../dtos/search-top-badge-user-statistic-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
@Injectable()
export class YearlyUserStatisticApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.badgeApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public searchTopBadgeUserStatistics(
    badgeId: string,
    searchText: string = '',
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Promise<SearchTopBadgeUserStatisticResult> {
    return this.post<ISearchTopBadgeUserStatisticRequest, ISearchTopBadgeUserStatisticResult>(
      '/badges/searchTopBadgeUserStatistics',
      {
        badgeId,
        searchText,
        skipCount,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
      },
      showSpinner
    )
      .pipe(map(_ => new SearchTopBadgeUserStatisticResult(_)))
      .toPromise();
  }

  public awardBadge(request: IAwardBadgeRequest): Promise<void> {
    return this.put<IAwardBadgeRequest, void>(``, request).toPromise();
  }
}
