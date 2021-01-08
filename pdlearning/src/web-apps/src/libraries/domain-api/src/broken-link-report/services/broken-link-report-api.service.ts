import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { BrokenLinkCheckResult, BrokenLinkReportSearchResult } from '../dtos/broken-link-report-search-result';
import { ICheckBrokenLinkRequest, IReportBrokenLinkRequest } from '../dtos/report-broken-link-request';

import { IBrokenLinkReportSearchRequest } from '../dtos/broken-link-report-search-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

Injectable();
export class BrokenLinkReportApiService extends BaseBackendService {
  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.brokenLinkApiUrl;
  }

  public searchBrokenLinkReport(
    request: IBrokenLinkReportSearchRequest,
    showSpinner: boolean = true
  ): Promise<BrokenLinkReportSearchResult> {
    return this.get<BrokenLinkReportSearchResult>('/broken-links/search', request, showSpinner)
      .pipe(map(_ => new BrokenLinkReportSearchResult(_)))
      .toPromise();
  }

  public reportBrokenLink(request: IReportBrokenLinkRequest, showSpinner?: boolean): Promise<void> {
    return this.post<IReportBrokenLinkRequest, void>('/broken-links/report', request, showSpinner).toPromise();
  }

  public checkBrokenLink(request: ICheckBrokenLinkRequest, showSpinner?: boolean): Observable<BrokenLinkCheckResult> {
    return this.post<ICheckBrokenLinkRequest, BrokenLinkCheckResult>('/broken-links/check', request, showSpinner).pipe(
      map(_ => new BrokenLinkCheckResult(_))
    );
  }
}
