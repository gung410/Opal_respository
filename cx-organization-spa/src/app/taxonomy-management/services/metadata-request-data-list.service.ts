import { Injectable } from '@angular/core';
import { PagedResultDto } from 'app-models/paged-result.dto';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { IGetMetadataSuggestionsRequest } from '../dtos/get-metadata-suggestion-request.dto';
import { TaxonomyRequestItem } from '../models/taxonomy-request-item.model';
import { TaxonomyRequest } from '../models/taxonomy-request.model';
import { TaxonomyRequestViewModel } from '../models/taxonomy-request.viewmodel';
import { MetadataRequestApiService } from './metadata-request-api.services';

@Injectable()
export class MetadataRequestDataListSerivce {
  constructor(private metadataRequestApiService: MetadataRequestApiService) {}

  getMetadataRequestList(
    metadataSuggestionsRequest: IGetMetadataSuggestionsRequest
  ): Observable<PagedResultDto<TaxonomyRequestViewModel>> {
    return this.metadataRequestApiService
      .getMetadataRequests(metadataSuggestionsRequest)
      .pipe(
        map((metadataRequestsRes) => {
          const { totalCount, items } = metadataRequestsRes;
          let parsedMetadataRequestListViewModel: TaxonomyRequestViewModel[] = [];

          items.forEach((metadataRequest: TaxonomyRequest) => {
            parsedMetadataRequestListViewModel = parsedMetadataRequestListViewModel.concat(
              this.parseMetadataRequestListViewModel(metadataRequest)
            );
          });

          return new PagedResultDto<TaxonomyRequestViewModel>({
            items: parsedMetadataRequestListViewModel,
            totalCount
          });
        })
      );
  }

  private parseMetadataRequestListViewModel(
    metadataRequest: TaxonomyRequest
  ): TaxonomyRequestViewModel[] {
    const metadataRequestListViewModel: TaxonomyRequestViewModel[] = [];
    metadataRequest.taxonomyRequestItems.forEach(
      (item: TaxonomyRequestItem) => {
        const metadataRequestViewModel = new TaxonomyRequestViewModel(
          metadataRequest
        );

        const metadaNameBasedOnPathName = item.pathName
          ? item.pathName.split(' / ')
          : '';

        metadataRequestViewModel.taxonomyRequestItemId = item.id;
        metadataRequestViewModel.nodeId = item.nodeId;
        metadataRequestViewModel.metadataType = item.metadataType;
        metadataRequestViewModel.metadataName = item.metadataName
          ? item.metadataName
          : metadaNameBasedOnPathName[metadaNameBasedOnPathName.length - 1];
        metadataRequestViewModel.oldMetadataName = item.oldMetadataName;
        metadataRequestViewModel.pathName = item.pathName;
        metadataRequestViewModel.pathIds = item.pathIds;
        metadataRequestViewModel.reason = item.reason;
        metadataRequestViewModel.abbreviation = item.abbreviation;
        metadataRequestViewModel.type = item.type;

        metadataRequestListViewModel.push(metadataRequestViewModel);
      }
    );

    return metadataRequestListViewModel;
  }
}
