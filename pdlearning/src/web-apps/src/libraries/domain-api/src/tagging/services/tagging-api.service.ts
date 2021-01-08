import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { GetResourceWithMetadataResult, IGetResourceWithMetadataResult } from '../dtos/get-resource-with-meta-data-result.dto';
import { IMetadataTagModel, MetadataTagModel } from '../models/metadata-tag.model';
import { IQuerySearchTagResult, QuerySearchTagResult } from '../dtos/query-search-tag-result.dto';
import { IResourceModel, ResourceModel } from '../models/resource.model';
import { ISearchTag, SearchTag } from '../models/search-tag.model';

import { IGetResourceMetadataListResult } from '../dtos/get-resource-metadata-list-result.dto';
import { IQuerySearchTagRequest } from '../dtos/query-search-tag-request.dto';
import { ISaveResourceMetadataRequest } from '../dtos/save-resource-metadata-request.dto';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResourceMetadataModel } from '../models/resource-metadata';
import { map } from 'rxjs/operators';

@Injectable()
export class TaggingApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.taggingApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getAllMetaDataTags(showSpinner: boolean = true): Observable<MetadataTagModel[]> {
    return this.get<IMetadataTagModel[]>('/metadataTag', null, showSpinner).pipe(map(_ => _.map(__ => new MetadataTagModel(__))));
  }

  public getMetaDataTagsByIds(tagIds: string[], showSpinner: boolean = true): Observable<MetadataTagModel[]> {
    return this.post<string[], IMetadataTagModel[]>('/metadataTagByIds', tagIds, showSpinner).pipe(
      map(_ => _.map(__ => new MetadataTagModel(__)))
    );
  }

  public getResource(resourceId: string): Observable<ResourceModel | undefined> {
    return this.get<IResourceModel | undefined>(`/resource/${resourceId}`).pipe(map(_ => (_ ? new ResourceModel(_) : undefined)));
  }

  public cloneResource(resourceId: string, clonedFromResourceId: string): Observable<ResourceModel | undefined> {
    return this.post<unknown, IResourceModel | undefined>(`/resource/${resourceId}/cloned-from/${clonedFromResourceId}`, {}).pipe(
      map(_ => (_ ? new ResourceModel(_) : undefined))
    );
  }

  public saveCourseMetadata(courseId: string, request: ISaveResourceMetadataRequest, showSpinner?: boolean): Observable<void> {
    return this.post<ISaveResourceMetadataRequest, void>(`/courses/${courseId}/metadata`, request, showSpinner);
  }

  public saveContentMetadata(contentId: string, request: ISaveResourceMetadataRequest, showSpinner: boolean = true): Observable<void> {
    return this.post<ISaveResourceMetadataRequest, void>(`/contents/${contentId}/metadata`, request, showSpinner);
  }

  public saveFormMetadata(formId: string, request: ISaveResourceMetadataRequest, showSpinner: boolean = true): Observable<void> {
    return this.post<ISaveResourceMetadataRequest, void>(`/forms/${formId}/metadata`, request, showSpinner);
  }

  public saveLearningPathMetadata(
    learningPathId: string,
    request: ISaveResourceMetadataRequest,
    showSpinner: boolean = true
  ): Observable<void> {
    return this.post<ISaveResourceMetadataRequest, void>(`/learningPaths/${learningPathId}/metadata`, request, showSpinner);
  }

  public getCourseMetaData(courseId: string): Observable<GetResourceWithMetadataResult | undefined> {
    return this.get<IGetResourceWithMetadataResult | undefined>(`/courses/${courseId}/metadata`).pipe(
      map(p => (p !== undefined && p !== null ? new GetResourceWithMetadataResult(p) : undefined))
    );
  }

  public getMySuggestedCourseIds(userTags: string[], showSpinner: boolean = true): Promise<string[]> {
    return this.post<string[], string[]>(`/me/suggestion/courses`, userTags, showSpinner).toPromise();
  }

  public getMetadatasForResources(resourceIds: string[], showSpinner: boolean = true): Observable<ResourceMetadataModel[]> {
    return this.post<string[], IGetResourceMetadataListResult>(`/resource/resourceMetadataList`, resourceIds, showSpinner).pipe(
      map(p => {
        return p.items.map(_ => new ResourceMetadataModel(_));
      })
    );
  }

  public querySearchTag(request: IQuerySearchTagRequest, showSpinner?: boolean): Observable<QuerySearchTagResult> {
    return this.post<IQuerySearchTagRequest, IQuerySearchTagResult>(`/search-tag/search`, request, showSpinner).pipe(
      map(response => {
        return new QuerySearchTagResult(response);
      })
    );
  }

  public getSearchTagByNames(searchTags: string[], showSpinner?: boolean): Observable<SearchTag[]> {
    return this.post<string[], ISearchTag[]>(`/search-tag/get-by-names`, searchTags, showSpinner).pipe(
      map(response => response.map(searchTag => (searchTag ? new SearchTag(searchTag) : undefined)))
    );
  }
}
