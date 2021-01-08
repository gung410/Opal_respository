import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { Injectable } from '@angular/core';
import { VersionTracking } from '../models/version-tracking';
import { VersionTrackingApiService } from '../services/version-tracking-api.services';
import { VersionTrackingRepositoryContext } from '../version-tracking-repository-context';
import { VersionTrackingSearchResult } from '../dtos/version-tracking-search-result';

@Injectable()
export class VersionTrackingRepository extends BaseRepository<VersionTrackingRepositoryContext> {
  constructor(context: VersionTrackingRepositoryContext, private versionTrackingApiService: VersionTrackingApiService) {
    super(context);
  }

  public searchVersionTrackings(
    originalObjectId: string = '',
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner: boolean = true
  ): Observable<VersionTrackingSearchResult> {
    return this.processUpsertData<VersionTracking, VersionTrackingSearchResult>(
      this.context.contentsSubject,
      implicitLoad =>
        from(
          this.versionTrackingApiService.searchVersionTrackings(
            {
              originalObjectId: originalObjectId,
              pagedInfo: {
                skipCount: skipCount,
                maxResultCount: maxResultCount
              }
            },
            showSpinner
          )
        ),
      'searchVersionTracking',
      [originalObjectId, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items.map(_ => new VersionTracking(_)),
      x => x.id
    );
  }
}
