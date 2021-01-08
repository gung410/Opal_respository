import { BehaviorSubject, Observable, Subject, defer } from 'rxjs';
import { distinctUntilChanged, finalize, map, switchMap, take, takeUntil } from 'rxjs/operators';

import { Utils } from '../utils/utils';

export type RepoLoadStrategy = 'loadOnce' | 'implicitReload' | 'explicitReload';

export abstract class BaseRepository<TContext extends BaseRepositoryContext> {
  constructor(protected context: TContext) {}

  protected maxNumberOfCacheItemPerRequest(): number {
    return 10;
  }

  protected processUpsertData<TModel, TApiResult>(
    repoDataSubject: BehaviorSubject<Dictionary<TModel>>,
    apiRequestFn: (implicitLoad: boolean) => Observable<TApiResult>,
    requestName: string,
    requestPayload: unknown[],
    strategy: RepoLoadStrategy,
    finalResultBuilder: (repoData: Dictionary<TModel>, apiResult: TApiResult) => TApiResult,
    modelDataExtracter: (apiResult: TApiResult) => TModel[],
    modelIdFn: (item: TModel) => string | number,
    replaceItem: boolean = true,
    asRequest?: boolean,
    refreshRelatedReqs?: { requestName: string; requestPartialPayload: unknown }[],
    optionalProps: (keyof TModel)[] = []
  ): Observable<TApiResult> {
    const requestId = this.buildRequestId(requestName, requestPayload);
    const stopRefreshNotifier$ = new Subject();
    const refreshDataFn = () => {
      apiRequestFn(true)
        .pipe(takeUntil(stopRefreshNotifier$))
        .subscribe(apiResult => {
          this.updateNewRequestData<TModel, TApiResult>(
            requestId,
            apiResult,
            repoDataSubject,
            modelDataExtracter,
            modelIdFn,
            replaceItem,
            optionalProps
          );
          if (refreshRelatedReqs != null) {
            refreshRelatedReqs.forEach(p => this.processRefreshData(p.requestName, p.requestPartialPayload));
          }
        });
    };
    const returnDataObsFn = () => {
      return defer(() => {
        if (this.context.loadedRequestSubscriberCountDic[requestId] != null) {
          this.context.loadedRequestSubscriberCountDic[requestId] = this.context.loadedRequestSubscriberCountDic[requestId] + 1;
        } else {
          this.context.loadedRequestSubscriberCountDic[requestId] = 1;
        }

        let resultObs = repoDataSubject.asObservable().pipe(
          map(repoData => {
            const cachedRequestData = <TApiResult>this.context.loadedRequestDataDic[requestId];
            return finalResultBuilder(repoData, Utils.cloneDeep(cachedRequestData));
          }),
          distinctUntilChanged((x, y) => Utils.isEqual(x, y)),
          map(x => Utils.cloneDeep(x))
        );
        if (asRequest) {
          resultObs = resultObs.pipe(take(1));
        }
        return resultObs.pipe(
          finalize(() => {
            stopRefreshNotifier$.next();
            this.context.loadedRequestSubscriberCountDic[requestId] = this.context.loadedRequestSubscriberCountDic[requestId] - 1;
            this.clearRequestDataCache(requestName);
          })
        );
      });
    };

    this.context.loadedRequestRefreshFnDic[requestId] = refreshDataFn;

    const cachedRequestApiResult = this.context.loadedRequestDataDic[requestId];
    if (cachedRequestApiResult == null || strategy === 'explicitReload' || (asRequest && strategy !== 'loadOnce')) {
      return apiRequestFn(false).pipe(
        switchMap(apiResult => {
          this.updateNewRequestData<TModel, TApiResult>(
            requestId,
            apiResult,
            repoDataSubject,
            modelDataExtracter,
            modelIdFn,
            replaceItem,
            optionalProps
          );
          if (refreshRelatedReqs != null) {
            refreshRelatedReqs.forEach(p => this.processRefreshData(p.requestName, p.requestPartialPayload));
          }
          return returnDataObsFn();
        })
      );
    }
    if (strategy === 'implicitReload') {
      refreshDataFn();
      return returnDataObsFn();
    }

    return returnDataObsFn();
  }

  protected processRefreshData(requestName: string, requestPartialPayload?: unknown): void {
    const requestId = this.buildRequestId(requestName, requestPartialPayload);
    const requestIdPrefix = requestId.endsWith(']') ? requestId.slice(0, requestId.length - 1) : requestId;
    Object.keys(this.context.loadedRequestRefreshFnDic).forEach(key => {
      if (key.startsWith(requestIdPrefix)) {
        this.context.loadedRequestRefreshFnDic[key]();
      }
    });
  }

  protected processClearRefreshDataRequest(requestName: string, requestPartialPayload?: unknown): void {
    const requestId = this.buildRequestId(requestName, requestPartialPayload);
    const requestIdPrefix = requestId.endsWith(']') ? requestId.slice(0, requestId.length - 1) : requestId;
    Object.keys(this.context.loadedRequestRefreshFnDic).forEach(key => {
      if (key.startsWith(requestIdPrefix)) {
        delete this.context.loadedRequestRefreshFnDic[key];
      }
    });
  }

  protected upsertData<TModel>(
    dataSubject: BehaviorSubject<Dictionary<TModel>>,
    data: (TModel | Partial<TModel>)[],
    modelIdFn: (item: TModel) => string | number,
    replaceItem: boolean = false,
    onDataChanged?: (newState: Dictionary<TModel>) => void,
    optionalProps: (keyof TModel)[] = []
  ): Dictionary<TModel> {
    return Utils.upsertDic(
      dataSubject.getValue(),
      data,
      modelIdFn,
      x => x,
      null,
      null,
      replaceItem,
      onDataChanged ? onDataChanged : x => dataSubject.next(x),
      optionalProps
    );
  }

  private updateCachedRequestData<TModel, TApiResult>(requestId: string, apiResult: TApiResult): boolean {
    if (Utils.isDifferent(this.context.loadedRequestDataDic[requestId], apiResult)) {
      this.context.loadedRequestDataDic[requestId] = Utils.cloneDeep(apiResult);
      return true;
    }
    return false;
  }

  private buildRequestId(requestName: string, requestPayload: unknown): string {
    return `${requestName}${requestPayload != null ? '_' + JSON.stringify(requestPayload) : ''}`;
  }

  private updateNewRequestData<TModel, TApiResult>(
    requestId: string,
    apiResult: TApiResult,
    repoDataSubject: BehaviorSubject<Dictionary<TModel>>,
    modelDataExtracter: (apiResult: TApiResult) => TModel[],
    modelIdFn: (item: TModel) => string | number,
    replaceItem: boolean,
    optionalProps: (keyof TModel)[] = []
  ): void {
    let hasChanged = this.updateCachedRequestData<TModel, TApiResult>(requestId, apiResult);
    const newData = this.upsertData(
      repoDataSubject,
      modelDataExtracter(apiResult),
      modelIdFn,
      replaceItem,
      x => (hasChanged = true),
      optionalProps
    );
    if (hasChanged) {
      repoDataSubject.next(newData);
    }
  }

  private clearRequestDataCache(requestName: string): void {
    const noSubscriberRequests = Object.keys(this.context.loadedRequestDataDic).filter(
      key => key.startsWith(requestName) && this.context.loadedRequestSubscriberCountDic[key] <= 0
    );

    while (noSubscriberRequests.length > this.maxNumberOfCacheItemPerRequest()) {
      const oldestRequestKey = noSubscriberRequests.shift();
      delete this.context.loadedRequestDataDic[oldestRequestKey];
      delete this.context.loadedRequestDataDic[oldestRequestKey];
      delete this.context.loadedRequestDataDic[oldestRequestKey];
    }
  }
}

export abstract class BaseRepositoryContext {
  public loadedRequestDataDic: Dictionary<unknown> = {};
  public loadedRequestRefreshFnDic: Dictionary<() => void> = {};
  public loadedRequestSubscriberCountDic: Dictionary<number> = {};
}
