import { BaseRepository, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, from } from 'rxjs';
import { filter, map, tap, withLatestFrom } from 'rxjs/operators';

import { IGetNewsfeedRequest } from '../dtos/get-newsfeed-request.dto';
import { Injectable } from '@angular/core';
import { NewsfeedApiService } from '../services/newsfeed.service';
import { NewsfeedRepositoryContext } from '../newsfeed-repository-context';
import { NewsfeedResult } from '../dtos/newsfeed-result.dto';

@Injectable()
export class NewsfeedRepository extends BaseRepository<NewsfeedRepositoryContext> {
  constructor(private newsfeedApiService: NewsfeedApiService, context: NewsfeedRepositoryContext) {
    super(context);
  }

  public loadNewsFeedsLazy(): Observable<NewsfeedResult> {
    return combineLatest(this.context.newsfeedSubject.asObservable(), this.context.loadedNewsfeedIdsResultSubject.asObservable()).pipe(
      withLatestFrom(this.context.currentNewsfeedRequestSubject.asObservable()),
      filter(([[newsfeedsResult, loadedNewsfeedIdsResult], currentRequest]) => {
        const result = loadedNewsfeedIdsResult[LOADED_NEWS_FEED_DEFAULT_KEY];
        const newsfeedIds = result ? result.items : [];
        // to make sure the repo has included all the data from result by ids
        const hasNewsfeedData = newsfeedIds.findIndex(p => newsfeedsResult[p] == null) === -1;
        return currentRequest != null && result != null && hasNewsfeedData;
      }),
      map(([[newsfeedsResult, loadedNewsfeedIdsResult], currentRequest]) => {
        const newFeedsIdsResult = loadedNewsfeedIdsResult[LOADED_NEWS_FEED_DEFAULT_KEY];

        const result = new NewsfeedResult();

        result.totalCount = newFeedsIdsResult.totalCount;
        result.items = newFeedsIdsResult.items
          .slice(0, currentRequest.skipCount + currentRequest.maxResultCount)
          .map(id => newsfeedsResult[id]);

        return result;
      })
    );
  }

  public loadNewsFeeds(request: IGetNewsfeedRequest): Observable<NewsfeedResult> {
    this.context.currentNewsfeedRequestSubject.next(request);
    return this.processUpsertData(
      this.context.newsfeedSubject,
      implicitLoad => from(this.newsfeedApiService.getNewsfeeds(request, !implicitLoad)),
      'loadNewsfeeds',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(p => repoData[p.id]);
        return apiResult;
      },
      apiResult => apiResult.items,
      item => item.id
    ).pipe(
      tap(response => {
        this.updateLoadedNewsfeeds(response, request);
      })
    );
  }

  private updateLoadedNewsfeeds(result: NewsfeedResult, request: IGetNewsfeedRequest): void {
    const loadedIdsResult = Utils.cloneDeep(this.context.loadedNewsfeedIdsResultSubject.value);
    const currentLoadedNewsfeed = loadedIdsResult[LOADED_NEWS_FEED_DEFAULT_KEY];

    const newIds = result.items.map(p => p.id);
    let currentIds = currentLoadedNewsfeed ? currentLoadedNewsfeed.items.slice() : [];

    if (currentIds.length) {
      newIds.forEach((p, index) => {
        currentIds[request.skipCount + index] = p;
      });
    } else {
      currentIds = newIds;
    }

    loadedIdsResult[LOADED_NEWS_FEED_DEFAULT_KEY] = { items: currentIds, totalCount: result.totalCount };
    if (!Utils.isDifferent(currentLoadedNewsfeed, loadedIdsResult[LOADED_NEWS_FEED_DEFAULT_KEY])) {
      return;
    }

    this.context.loadedNewsfeedIdsResultSubject.next(loadedIdsResult);
  }
}

/**
 * This can be used as hashed request to store multiple loaded newsfeeds
 */
const LOADED_NEWS_FEED_DEFAULT_KEY: string = 'default';
