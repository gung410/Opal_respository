import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { ICatalogSearchRequest, ICatalogSearchV2Request } from '../dtos/catalog-search.request';

import { ICatalogSearchResult } from '../models/catalog-search-results.model';
import { ICatalogSuggestionRequest } from '../dtos/catalog-recommendation.request';
import { INewlyAddedCoursesRequest } from '../dtos/catalog-get-recent-courses.request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class PDCatalogueApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learningCatalogUrl + '/api/PDCatalogue/';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public search(request: ICatalogSearchRequest, showSpinner: boolean = true): Observable<ICatalogSearchResult> {
    return this.post<ICatalogSearchRequest, ICatalogSearchResult>('search', request, showSpinner);
  }

  public searchV2(request: ICatalogSearchV2Request, showSpinner: boolean = true): Observable<ICatalogSearchResult> {
    return this.post<ICatalogSearchV2Request, ICatalogSearchResult>('searchv2', request, showSpinner);
  }

  public getNewlyAddedCourses(request: INewlyAddedCoursesRequest, showSpinner: boolean = true): Observable<ICatalogSearchResult> {
    return this.post<INewlyAddedCoursesRequest, ICatalogSearchResult>('newlyResourceAdded ', request, showSpinner);
  }

  public getSuggestedLearningItems(request: ICatalogSuggestionRequest, showSpinner: boolean = true): Observable<ICatalogSearchResult> {
    return this.post<ICatalogSuggestionRequest, ICatalogSearchResult>('search/recommendationv2', request, showSpinner);
  }
}
