import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { BrokenLinkModuleIdentifier } from '../dtos/broken-link-report-search-request';
import { BrokenLinkReport } from '../model/broken-link-report';
import { BrokenLinkReportApiService } from '../services/broken-link-report-api.service';
import { BrokenLinkReportRepositoryContext } from '../broken-link-report-repository-context';
import { BrokenLinkReportSearchResult } from '../dtos/broken-link-report-search-result';
import { Injectable } from '@angular/core';

@Injectable()
export class BrokenLinkReportRepository extends BaseRepository<BrokenLinkReportRepositoryContext> {
  constructor(context: BrokenLinkReportRepositoryContext, private brokenLinkReportApiService: BrokenLinkReportApiService) {
    super(context);
  }
  public searchBrokenLinkReport(
    originalObjectId: string,
    parentIds: string[],
    module: BrokenLinkModuleIdentifier,
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner: boolean = true
  ): Observable<BrokenLinkReportSearchResult> {
    return this.processUpsertData<BrokenLinkReport, BrokenLinkReportSearchResult>(
      this.context.contentsSubject,
      implicitLoad =>
        from(
          this.brokenLinkReportApiService.searchBrokenLinkReport(
            {
              originalObjectId: originalObjectId,
              parentIds: parentIds,
              module: module,
              pagedInfo: {
                skipCount: skipCount,
                maxResultCount: maxResultCount
              }
            },
            showSpinner
          )
        ),
      'searchBrokenLinkReport',
      [originalObjectId, parentIds, module, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items.map(_ => new BrokenLinkReport(_)),
      x => x.id
    );
  }
}
