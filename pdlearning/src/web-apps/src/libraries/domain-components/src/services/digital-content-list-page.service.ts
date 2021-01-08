import {
  ContentRepository,
  DigitalContentQueryMode,
  DigitalContentSortField,
  DigitalContentStatus,
  SortDirection,
  TaggingRepository,
  UserRepository
} from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { DigitalContentViewModel } from '../models/digital-content-view.model';
import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';

@Injectable()
export class DigitalContentListPageService {
  constructor(
    private userRepository: UserRepository,
    private contentRepository: ContentRepository,
    private taggingRepository: TaggingRepository
  ) {}

  public loadGridDigitalContent(
    searchText: string,
    searchStatuses: DigitalContentStatus[],
    skipCount: number,
    maxResultCount: number,
    queryMode: DigitalContentQueryMode,
    sortfield: DigitalContentSortField,
    sortDirection: SortDirection
  ): Observable<OpalGridDataResult<DigitalContentViewModel>> {
    return this.contentRepository
      .loadSearchDigitalContents(searchText, searchStatuses, skipCount, maxResultCount, queryMode, sortfield, sortDirection)
      .pipe(
        switchMap(digitalContentSearchResult => {
          if (digitalContentSearchResult.totalCount === 0) {
            return of(<OpalGridDataResult<DigitalContentViewModel>>{
              data: [],
              total: digitalContentSearchResult.totalCount
            });
          }

          return combineLatest(
            this.taggingRepository.loadMetadatasForResources(digitalContentSearchResult.items.map(_ => _.id)),
            this.userRepository.loadPublicUserInfoList({ userIds: Utils.uniq(digitalContentSearchResult.items.map(_ => _.ownerId)) })
          ).pipe(
            map(([contentMetadatas, publicUsers]) => {
              const contentMetadatasDic = Utils.toDictionary(contentMetadatas, p => p.resourceId);
              const publicUsersDicById = Utils.toDictionary(publicUsers, p => p.id);
              return <OpalGridDataResult<DigitalContentViewModel>>{
                data: digitalContentSearchResult.items.map(_ =>
                  DigitalContentViewModel.createFromModel(
                    _,
                    contentMetadatasDic[_.id],
                    publicUsersDicById[_.ownerId],
                    publicUsersDicById[_.archivedBy]
                  )
                ),
                total: digitalContentSearchResult.totalCount
              };
            })
          );
        })
      );
  }
}
