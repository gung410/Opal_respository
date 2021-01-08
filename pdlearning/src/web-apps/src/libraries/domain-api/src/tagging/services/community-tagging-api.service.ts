import { BaseBackendService, CommonFacadeService, InterceptorRegistry, InterceptorType } from '@opal20/infrastructure';
import { GetResourceWithMetadataResult, IGetResourceWithMetadataResult } from '../dtos/get-resource-with-meta-data-result.dto';

import { ISaveResourceMetadataRequest } from '../dtos/save-resource-metadata-request.dto';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class CommunityTaggingApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.taggingApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public saveCommunityMetadata(communityId: string, request: ISaveResourceMetadataRequest, showSpinner?: boolean): Observable<void> {
    return this.post<ISaveResourceMetadataRequest, void>(`/community/${communityId}/metadata`, request, showSpinner);
  }

  public getCommunityMetaData(communityId: string): Observable<GetResourceWithMetadataResult | undefined> {
    return this.get<IGetResourceWithMetadataResult | undefined>(`/community/${communityId}/metadata`).pipe(
      map(p => (p !== undefined && p !== null ? new GetResourceWithMetadataResult(p) : undefined))
    );
  }

  /**
   * Override this method to set or replace interceptors for current service scope.
   */
  protected onFilterInterceptors(registry: InterceptorRegistry): InterceptorRegistry {
    return registry.replace(InterceptorType.HttpResponse, { key: 'NOOP_HTTP_RESPONSE_INTERCEPTOR', type: InterceptorType.HttpResponse });
  }
}
