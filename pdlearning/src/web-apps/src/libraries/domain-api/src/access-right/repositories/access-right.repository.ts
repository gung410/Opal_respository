import { Observable, from } from 'rxjs';

import { AccessRight } from '../models/access-right';
import { AccessRightApiService } from '../services/access-right-api.services';
import { AccessRightRepositoryContext } from '../access-right-repository-context';
import { AccessRightSearchResult } from '../dtos/access-right-search-result';
import { BaseRepository } from '@opal20/infrastructure';
import { Injectable } from '@angular/core';

@Injectable()
export class AccessRightRepository extends BaseRepository<AccessRightRepositoryContext> {
  constructor(context: AccessRightRepositoryContext, private versionTrackingApiService: AccessRightApiService) {
    super(context);
  }

  public searchCollaborators(
    originalObjectId: string = '',
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner: boolean = true
  ): Observable<AccessRightSearchResult> {
    return this.processUpsertData<AccessRight, AccessRightSearchResult>(
      this.context.contentsSubject,
      implicitLoad =>
        from(
          this.versionTrackingApiService.searchAccessRights(
            {
              originalObjectId: originalObjectId,
              pagedInfo: {
                skipCount: skipCount,
                maxResultCount: maxResultCount
              }
            },
            showSpinner && !implicitLoad
          )
        ),
      'searchCollaborators',
      [originalObjectId, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items.map(_ => new AccessRight(_)),
      x => x.id
    );
  }
}
