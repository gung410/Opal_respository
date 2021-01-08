import { BaseBackendService, CommonFacadeService, IFilter } from '@opal20/infrastructure';
import { BlockoutDateModel, IBlockoutDateModel } from '../models/blockout-date.model';
import { GetBlockoutDateDependenciesModel, IGetBlockoutDateDependenciesModel } from '../models/get-blockout-date-dependencies-model';

import { Constant } from '@opal20/authentication';
import { IConfirmBlockoutDateRequest } from '../dtos/confirm-blockout-date-request';
import { IGetBlockoutDateDependenciesRequest } from '../dtos/get-blockout-date-dependencies-request';
import { ISaveBlockoutDateRequest } from './../dtos/save-blockout-date-request';
import { ISearchBlockoutDateRequest } from './../dtos/search-blockout-date-request';
import { Injectable } from '@angular/core';
import { SearchBlockoutDateResult } from './../dtos/search-blockout-date-result';
import { map } from 'rxjs/operators';

@Injectable()
export class BlockoutDateService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getBlockoutDateById(id: string, showSpinner?: boolean): Promise<BlockoutDateModel> {
    return this.get<IBlockoutDateModel>(`/blockoutDate/${id}`, null, showSpinner)
      .pipe(map(data => new BlockoutDateModel(data)))
      .toPromise();
  }

  public searchBlockoutDates(
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number | null = 10,
    coursePlanningCycleId: string,
    showSpinner?: boolean
  ): Promise<SearchBlockoutDateResult> {
    const request = {
      searchText: searchText,
      filter: filter,
      skipCount: skipCount,
      maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount,
      coursePlanningCycleId: coursePlanningCycleId
    };
    return this.post<ISearchBlockoutDateRequest, SearchBlockoutDateResult>('/blockoutDate/search', request, showSpinner)
      .pipe(
        map(_ => {
          return new SearchBlockoutDateResult(_);
        })
      )
      .toPromise();
  }

  public getBlockoutDateDependencies(
    request: IGetBlockoutDateDependenciesRequest,
    showSpinner?: boolean
  ): Promise<GetBlockoutDateDependenciesModel> {
    return this.post<IGetBlockoutDateDependenciesRequest, IGetBlockoutDateDependenciesModel>(
      `/blockoutDate/getBlockoutDateDependencies`,
      request,
      showSpinner
    )
      .pipe(map(data => new GetBlockoutDateDependenciesModel(data)))
      .toPromise();
  }

  public saveBlockoutDate(request: ISaveBlockoutDateRequest): Promise<BlockoutDateModel> {
    return this.post<ISaveBlockoutDateRequest, IBlockoutDateModel>(`/blockoutDate/save`, request)
      .pipe(map(data => new BlockoutDateModel(data)))
      .toPromise();
  }

  public confirmBlockoutDate(request: IConfirmBlockoutDateRequest): Promise<void> {
    return this.put<IConfirmBlockoutDateRequest, void>(`/blockoutDate/confirmBlockoutDate`, request).toPromise();
  }

  public deleteBlockoutDate(id: string): Promise<void> {
    return this.delete<void>(`/blockoutDate/${id}`).toPromise();
  }
}
