import { BaseRepository, Utils } from '@opal20/infrastructure';
import { ICatalogSearchRequest, ICatalogSearchV2Request } from '../../learning-catalog/dtos/catalog-search.request';

import { ICatalogSearchResult } from '../../learning-catalog/models/catalog-search-results.model';
import { ICatalogSuggestionRequest } from '../../learning-catalog/dtos/catalog-recommendation.request';
import { INewlyAddedCoursesRequest } from '../../learning-catalog/dtos/catalog-get-recent-courses.request';
import { Injectable } from '@angular/core';
import { LearningCatalogueRepositoryContext } from '../learning-catalogue-repository-context';
import { Observable } from 'rxjs';
import { PDCatalogueApiService } from '../../learning-catalog/services/pd-catalog-backend.service';

const DATE_FIELD_NAMES = ['expiredDate', 'startDate'];

@Injectable()
export class CatalogueRepository extends BaseRepository<LearningCatalogueRepositoryContext> {
  constructor(context: LearningCatalogueRepositoryContext, private pdCatalogueApiService: PDCatalogueApiService) {
    super(context);
  }

  public search(request: ICatalogSearchRequest, showSpinner: boolean = true): Observable<ICatalogSearchResult> {
    const payload = this.processCataloguePayload(request);
    return this.processUpsertData(
      this.context.catalogueResourceSubject,
      implicitLoad => this.pdCatalogueApiService.search(request, !implicitLoad),
      'search',
      [payload],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.resources = apiResult.resources.map(p => repoData[p.id]);
        return apiResult;
      },
      apiResult => apiResult.resources,
      x => x.id
    );
  }

  public searchV2(request: ICatalogSearchV2Request, showSpinner: boolean = true): Observable<ICatalogSearchResult> {
    const payload = this.processCataloguePayloadV2(request);
    return this.processUpsertData(
      this.context.catalogueResourceSubject,
      implicitLoad => this.pdCatalogueApiService.searchV2(request, !implicitLoad),
      'search',
      [payload],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.resources = apiResult.resources.map(p => repoData[p.id]);
        return apiResult;
      },
      apiResult => apiResult.resources,
      x => x.id
    );
  }

  public loadNewlyAddedCourses(request: INewlyAddedCoursesRequest, showSpinner: boolean = true): Observable<ICatalogSearchResult> {
    const payload = this.processCataloguePayload(request);
    return this.processUpsertData(
      this.context.catalogueResourceSubject,
      implicitLoad => this.pdCatalogueApiService.getNewlyAddedCourses(request, !implicitLoad),
      'loadNewlyAddedCourses',
      [payload],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.resources = apiResult.resources.map(p => repoData[p.id]);
        return apiResult;
      },
      apiResult => apiResult.resources,
      x => x.id
    );
  }

  public loadSuggestedLearningItems(request: ICatalogSuggestionRequest, showSpinner: boolean = true): Observable<ICatalogSearchResult> {
    return this.processUpsertData(
      this.context.catalogueResourceSubject,
      implicitLoad => this.pdCatalogueApiService.getSuggestedLearningItems(request, !implicitLoad),
      'loadSuggestedLearningItems',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.resources = apiResult.resources.map(p => repoData[p.id]);
        return apiResult;
      },
      apiResult => apiResult.resources,
      x => x.id
    );
  }

  /**
   * To remove the date in request
   * Because the date time generate differently any time so cannot cache in the repository.
   */
  private processCataloguePayload<T extends ICatalogSearchRequest | INewlyAddedCoursesRequest | ICatalogSuggestionRequest>(request: T): T {
    const payload = Utils.cloneDeep(request);
    DATE_FIELD_NAMES.forEach(fieldName => {
      this.removeSearchCriteriaDateTime(payload, fieldName);
    });
    return payload;
  }

  private processCataloguePayloadV2(request: ICatalogSearchV2Request): ICatalogSearchV2Request {
    const payload = Utils.cloneDeep(request);
    Object.values(payload.filters).forEach(filters => {
      filters.forEach(filter => {
        if (DATE_FIELD_NAMES.includes(filter.fieldName)) {
          filter.values[0] = filter.values[0].split(' ')[0];
        }
      });
    });
    return payload;
  }

  private removeSearchCriteriaDateTime(
    payload: ICatalogSearchRequest | INewlyAddedCoursesRequest | ICatalogSuggestionRequest,
    fieldName: string
  ): void {
    // searchCriteria date field has format like ['...', '13/10/2020 03:38:32', '...']
    // so we get the second index to remove the time of the date time value.
    const dateField = payload.searchCriteria[fieldName] && payload.searchCriteria[fieldName][1];
    if (dateField) {
      payload.searchCriteria[fieldName][1] = dateField.split(' ')[0];
    }
  }
}
