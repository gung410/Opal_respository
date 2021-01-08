import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { IRevertVersionTrackingResult, RevertVersionTrackingResult } from '../../version-tracking/dtos/version-tracking-revert-result';
import { IVersionTracking, VersionTracking } from '../models/version-tracking';
import { IVersionTrackingSearchResult, VersionTrackingSearchResult } from '../../version-tracking/dtos/version-tracking-search-result';

import { IVersionTrackingRevertRequest } from '../../version-tracking/dtos/version-tracking-revert-request';
import { IVersionTrackingSearchRequest } from '../../version-tracking/dtos/version-tracking-search-request';
import { Injectable } from '@angular/core';
import { VersionTrackingType } from '../models/version-tracking-type';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class VersionTrackingApiService extends BaseBackendService {
  protected serviceType: VersionTrackingType = VersionTrackingType.DigitalContent;

  protected get apiUrl(): string {
    switch (this.serviceType) {
      case VersionTrackingType.DigitalContent:
        return AppGlobal.environment.contentApiUrl + '/content';
      case VersionTrackingType.Form:
        return AppGlobal.environment.formApiUrl + '/form';
      case VersionTrackingType.LnaForm:
        return AppGlobal.environment.lnaFormApiUrl + '/form';
      default:
        return AppGlobal.environment.apiUrl;
    }
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public initApiService(serviceType: VersionTrackingType): void {
    this.serviceType = serviceType;
  }

  public searchVersionTrackings(request: IVersionTrackingSearchRequest, showSpinner: boolean = true): Promise<VersionTrackingSearchResult> {
    return this.post<IVersionTrackingSearchRequest, IVersionTrackingSearchResult>('/versioning/getVersionsByObjectId', request, showSpinner)
      .pipe(map(_ => new VersionTrackingSearchResult(_)))
      .toPromise();
  }

  public getRevertableVersions(originalObjectId: string, showSpinner: boolean = true): Promise<IVersionTracking[]> {
    return this.get<VersionTracking[]>(`/versioning/getRevertableVersions/${originalObjectId}`, null, showSpinner)
      .pipe(map(versionTrackings => versionTrackings.map(vt => new VersionTracking(vt))))
      .toPromise();
  }

  public getActiveVersion(originalObjectId: string, showSpinner: boolean = true): Promise<IVersionTracking> {
    return this.get<VersionTracking>(`/versioning/activeVersion/${originalObjectId}`, null, showSpinner)
      .pipe(map(vt => new VersionTracking(vt)))
      .toPromise();
  }

  public revertVersion(request: IVersionTrackingRevertRequest, showSpinner?: boolean): Promise<IRevertVersionTrackingResult> {
    return this.post<IVersionTrackingRevertRequest, IRevertVersionTrackingResult>('/versioning/revertVersion', request, showSpinner)
      .pipe(map(_ => new RevertVersionTrackingResult(_)))
      .toPromise();
  }
}
