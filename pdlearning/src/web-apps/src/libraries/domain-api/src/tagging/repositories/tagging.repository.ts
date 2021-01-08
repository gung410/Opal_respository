import { map, take } from 'rxjs/operators';

import { BaseRepository } from '@opal20/infrastructure';
import { IQuerySearchTagRequest } from '../dtos/query-search-tag-request.dto';
import { ISaveResourceMetadataRequest } from '../dtos/save-resource-metadata-request.dto';
import { Injectable } from '@angular/core';
import { MetadataTagModel } from '../models/metadata-tag.model';
import { Observable } from 'rxjs';
import { QuerySearchTagResult } from '../dtos/query-search-tag-result.dto';
import { ResourceMetadataModel } from '../models/resource-metadata';
import { TaggingApiService } from '../services/tagging-api.service';
import { TaggingRepositoryContext } from '../tagging-repository-context';

@Injectable()
export class TaggingRepository extends BaseRepository<TaggingRepositoryContext> {
  constructor(context: TaggingRepositoryContext, private apiSvc: TaggingApiService) {
    super(context);
  }

  public loadMetadatasForResources(resourceIds: string[]): Observable<ResourceMetadataModel[]> {
    return this.processUpsertData(
      this.context.resourceMetadatasSubject,
      implicitLoad => this.apiSvc.getMetadatasForResources(resourceIds, !implicitLoad),
      'loadMetadatasForResources',
      resourceIds,
      'implicitReload',
      (repoData, apiResult) => resourceIds.map(id => repoData[id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.resourceId
    );
  }

  public loadAllMetaDataTags(showSpinner: boolean = false): Observable<MetadataTagModel[]> {
    return this.processUpsertData(
      this.context.metadataTagsSubject,
      implicitLoad => this.apiSvc.getAllMetaDataTags(implicitLoad || showSpinner),
      'loadAllMetaDataTags',
      null,
      'loadOnce',
      (repoData, apiResult) => apiResult.map(item => repoData[item.tagId]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.tagId
    );
  }

  public loadMetaDataTagsByIds(tagIds: string[]): Observable<MetadataTagModel[]> {
    return this.processUpsertData(
      this.context.metadataTagsSubject,
      implicitLoad => this.apiSvc.getMetaDataTagsByIds(tagIds, !implicitLoad),
      'loadMetaDataTagsByIds',
      [tagIds],
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.tagId]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.tagId
    );
  }

  public saveCourseMetadata(courseId: string, request: ISaveResourceMetadataRequest): Observable<void> {
    return this.apiSvc.saveCourseMetadata(courseId, request).pipe(
      map(_ => {
        this.loadMetadatasForResources([courseId])
          .pipe(take(1))
          .subscribe();
        return _;
      })
    );
  }

  public loadSearchTags(request: IQuerySearchTagRequest, showSpinner: boolean = true): Observable<QuerySearchTagResult> {
    return this.processUpsertData(
      this.context.searchTagsSubject,
      implicitLoad => this.apiSvc.querySearchTag(request, !implicitLoad && showSpinner),
      'loadSearchTags',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id
    );
  }

  public saveLearningPathMetadata(learningPathId: string, request: ISaveResourceMetadataRequest): Observable<void> {
    return this.apiSvc.saveLearningPathMetadata(learningPathId, request).pipe(
      map(_ => {
        this.loadMetadatasForResources([learningPathId])
          .pipe(take(1))
          .subscribe();
        return _;
      })
    );
  }
}
