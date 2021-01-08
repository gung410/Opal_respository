import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { IMyDigitalContentSearchRequest } from '../dtos/my-digital-content-search-request.dto';
import { IMyLearningSearchRequest } from '../dtos/my-learning-search-request.dto';
import { ISearchFilterResultModel } from '../dtos/my-learning-search-result.dto';
import { Injectable } from '@angular/core';
import { LearnerRepositoryContext } from '../learner-repository-context';
import { MyDigitalContent } from '../models/my-digital-content.model';
import { MyDigitalContentApiService } from '../services/my-digital-content-backend.service';
import { MyDigitalContentSearchResult } from '../dtos/my-digital-content-search-result.dto';

@Injectable()
export class MyDigitalContentRepository extends BaseRepository<LearnerRepositoryContext> {
  constructor(context: LearnerRepositoryContext, private myDigitalContentApiService: MyDigitalContentApiService) {
    super(context);
  }

  public loadMyDigitalContentsBySearch(request: IMyLearningSearchRequest): Observable<ISearchFilterResultModel<MyDigitalContent>> {
    return this.processUpsertData(
      this.context.myDigitalContentSubject,
      implicitLoad => from(this.myDigitalContentApiService.getMyDigitalContentsBySearch(request, !implicitLoad)),
      'loadMyDigitalContentsBySearch',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.digitalContentId]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.digitalContentId
    );
  }

  public loadMyDigitalContents(request: IMyDigitalContentSearchRequest): Observable<MyDigitalContentSearchResult> {
    return this.processUpsertData(
      this.context.myDigitalContentSubject,
      implicitLoad => from(this.myDigitalContentApiService.getMyDigitalContents(request, !implicitLoad)),
      'loadMyDigitalContents',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.digitalContentId]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.digitalContentId
    );
  }

  public loadByDigitalContentIds(digitalContentIds: string[]): Observable<MyDigitalContent[]> {
    return this.processUpsertData(
      this.context.myDigitalContentSubject,
      implicitLoad => from(this.myDigitalContentApiService.getByDigitalContentIds(digitalContentIds, !implicitLoad)),
      'loadByDigitalContentIds',
      digitalContentIds,
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(a => repoData[a.digitalContentId]);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.digitalContentId
    );
  }

  public loadDigitalContentDetails(digitalContentId: string): Observable<MyDigitalContent> {
    return this.processUpsertData(
      this.context.myDigitalContentSubject,
      implicitLoad => from(this.myDigitalContentApiService.getDigitalContentDetails(digitalContentId, !implicitLoad)),
      'loadDigitalContentDetails',
      [digitalContentId],
      'implicitReload',
      (repoData, apiResult) => {
        return repoData[apiResult.digitalContentId];
      },
      apiResult => [apiResult],
      x => x.digitalContentId
    );
  }
}
