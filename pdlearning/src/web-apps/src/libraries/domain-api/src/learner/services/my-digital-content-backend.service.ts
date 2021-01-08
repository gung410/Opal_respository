import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { IEnrollDigitalContentRequest, IMyDigitalContentRequest } from '../dtos/my-digital-content-backend-service.dto';
import { IMyDigitalContent, MyDigitalContent } from '../models/my-digital-content.model';

import { IMyDigitalContentSearchRequest } from '../dtos/my-digital-content-search-request.dto';
import { IMyLearningSearchRequest } from '../dtos/my-learning-search-request.dto';
import { ISearchFilterResultModel } from '../dtos/my-learning-search-result.dto';
import { Injectable } from '@angular/core';
import { MyDigitalContentSearchResult } from '../dtos/my-digital-content-search-result.dto';
import { map } from 'rxjs/operators';

@Injectable()
export class MyDigitalContentApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/me/digitalcontent';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getMyDigitalContentsBySearch(
    request: IMyLearningSearchRequest,
    showSpinner: boolean = true
  ): Promise<ISearchFilterResultModel<MyDigitalContent>> {
    const queryParams: IGetParams = {
      searchText: request.searchText,
      statusFilter: request.statusFilter,
      orderBy: 'CreatedDate desc',
      includeStatistic: request.includeStatistic,
      statisticsFilter: request.statisticsFilter,
      skipCount: request.skipCount,
      maxResultCount: request.maxResultCount
    };
    return this.post<IGetParams, ISearchFilterResultModel<IMyDigitalContent>>('/search', queryParams, showSpinner)
      .pipe(
        map(response => {
          response.items = response.items.map(p => new MyDigitalContent(p));
          return response as ISearchFilterResultModel<MyDigitalContent>;
        })
      )
      .toPromise();
  }

  public getMyDigitalContents(request: IMyDigitalContentSearchRequest, showSpinner: boolean = true): Promise<MyDigitalContentSearchResult> {
    const queryParams: IGetParams = {
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount,
      searchText: request.searchText,
      statusFilter: request.statusFilter,
      orderBy: request.orderBy,
      contentType: request.contentType
    };
    return this.post<IGetParams, MyDigitalContentSearchResult>(`/search`, queryParams, showSpinner)
      .pipe(map(response => new MyDigitalContentSearchResult(response)))
      .toPromise();
  }

  public getByDigitalContentIds(digitalContentIds: string[], showSpinner: boolean = true): Promise<MyDigitalContent[]> {
    const requestBody: object = {
      digitalContentIds: digitalContentIds
    };
    return this.post<object, IMyDigitalContent[]>('/getIds', requestBody, showSpinner)
      .pipe(map(result => result.map(p => new MyDigitalContent(p))))
      .toPromise();
  }

  public getDigitalContentDetails(digitalContentId: string, showSpinner: boolean = true): Promise<MyDigitalContent> {
    return this.get<IMyDigitalContent>(`/details/${digitalContentId}`, null, showSpinner)
      .pipe(map(result => new MyDigitalContent(result)))
      .toPromise();
  }

  public updateMyDigitalContent(request: IMyDigitalContentRequest): Promise<MyDigitalContent> {
    return this.post<IMyDigitalContentRequest, IMyDigitalContent>('/update', request)
      .pipe(map(result => new MyDigitalContent(result)))
      .toPromise();
  }

  public enrollDigitalContent(digitalContentId: string): Promise<MyDigitalContent> {
    const requestBody: IEnrollDigitalContentRequest = {
      digitalContentId: digitalContentId
    };
    return this.post<IEnrollDigitalContentRequest, IMyDigitalContent>('/enroll', requestBody)
      .pipe(map(result => new MyDigitalContent(result)))
      .toPromise();
  }
}
