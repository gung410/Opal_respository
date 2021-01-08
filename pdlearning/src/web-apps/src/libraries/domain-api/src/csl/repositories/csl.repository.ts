import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { CSLCommunityResults } from '../dtos/csl-community-list-result.model';
import { CollaborativeSocialLearningApiService } from '../services/csl-backend.service';
import { CslRepositoryContext } from '../csl-repository-context';
import { ICommunityRequest } from '../models/csl-community.model';
import { Injectable } from '@angular/core';

@Injectable()
export class CslRepository extends BaseRepository<CslRepositoryContext> {
  constructor(context: CslRepositoryContext, private cslService: CollaborativeSocialLearningApiService) {
    super(context);
  }

  public loadAllItemsCommunity(userId: string, request: ICommunityRequest): Observable<CSLCommunityResults> {
    return this.processUpsertData(
      this.context.communityResultSubject,
      implicitLoad => from(this.cslService.getAllItemsCommunity(userId, request, !implicitLoad)),
      'loadAllItemsCommunity',
      [userId, request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.guid]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.guid
    );
  }

  public loadCommunityByIds(communityIds: string[]): Observable<CSLCommunityResults> {
    return this.processUpsertData(
      this.context.communityResultSubject,
      implicitLoad => from(this.cslService.getCommunityByIds(communityIds, !implicitLoad)),
      'loadCommunityByIds',
      communityIds,
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.guid]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.guid
    );
  }
}
