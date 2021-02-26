import { Injectable } from '@angular/core';
import { PagedResultDto } from 'app-models/paged-result.dto';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApproveTaxonomyRequest } from '../dtos/approve-taxonomy-request.dto';
import { IGetMetadataSuggestionsRequest } from '../dtos/get-metadata-suggestion-request.dto';
import { RejectTaxonomyRequest } from '../dtos/reject-taxonomy-request.dto';
import { ITaxonomyCreationRequest } from '../dtos/taxonomy-creation-request.dto';
import { IUpdateTaxonomyRequestItemRequest } from '../dtos/update-taxonomy-request-Item-request.dto';
import { UpdateTaxonomyRequestRequest } from '../dtos/update-taxonomy-request-request.dto';
import {
  IMetadataTagModel,
  MetadataTagModel
} from '../models/metadata-tag.model';
import { TaxonomyRequestItem } from '../models/taxonomy-request-item.model';
import { TaxonomyRequest } from '../models/taxonomy-request.model';

@Injectable()
export class MetadataRequestApiService {
  private readonly TAXONOMY_API_URL: string = `${AppConstant.api.tagging}/taxonomyRequest`;
  // private readonly TAXONOMY_API_URL: string =
  //   'http://localhost:5005/api/taxonomyRequest';
  private readonly TAGGING_API_URL: string = `${AppConstant.api.tagging}/metadataTag`;
  private readonly LEARNING_CATALOG_API_URL: string = `${AppConstant.api.learningCatalog}`;

  constructor(private httpHelper: HttpHelpers) {}

  getMetadataRequests(
    metadataSuggestionsRequest: IGetMetadataSuggestionsRequest
  ): Observable<PagedResultDto<TaxonomyRequest>> {
    return this.httpHelper.post<PagedResultDto<TaxonomyRequest>>(
      `${this.TAXONOMY_API_URL}/get`,
      metadataSuggestionsRequest
    );
  }

  getMetadataRequestById(
    taxonomyRequestId: string
  ): Observable<TaxonomyRequest> {
    return this.httpHelper.get<TaxonomyRequest>(
      `${this.TAXONOMY_API_URL}/${taxonomyRequestId}`
    );
  }

  createMetadataRequest(
    taxonomyCreationRequest: ITaxonomyCreationRequest
  ): Observable<TaxonomyRequest> {
    return this.httpHelper.post(
      `${this.TAXONOMY_API_URL}`,
      taxonomyCreationRequest
    );
  }

  updateMetadata(
    updateTaxonomyRequestRequest: UpdateTaxonomyRequestRequest
  ): Observable<TaxonomyRequest> {
    return this.httpHelper.put(
      `${this.TAXONOMY_API_URL}/request`,
      updateTaxonomyRequestRequest
    );
  }

  approveTaxonomyRequest(
    approveTaxonomyRequest: ApproveTaxonomyRequest
  ): Observable<TaxonomyRequest> {
    return this.httpHelper.post(
      `${this.TAXONOMY_API_URL}/approve`,
      approveTaxonomyRequest
    );
  }

  rejectTaxonomyRequest(
    rejectTaxonomyRequest: RejectTaxonomyRequest
  ): Observable<TaxonomyRequest> {
    return this.httpHelper.post(
      `${this.TAXONOMY_API_URL}/reject`,
      rejectTaxonomyRequest
    );
  }

  closeTaxonomyRequestById(
    taxonomyRequestId: string
  ): Observable<TaxonomyRequest> {
    return this.httpHelper.post(
      `${this.TAXONOMY_API_URL}/close`,
      taxonomyRequestId
    );
  }

  // TAGGING
  getAllMetaDataTags(): Promise<MetadataTagModel[]> {
    return this.httpHelper
      .get<IMetadataTagModel[]>(this.TAGGING_API_URL)
      .pipe(map((_) => _.map((__) => new MetadataTagModel(__))))
      .toPromise();
  }

  getMetadataType(): Observable<any> {
    return this.httpHelper.get(`${this.LEARNING_CATALOG_API_URL}/enumerations`);
  }
}
