import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { ContentApiService } from '../services/content-api.service';
import { ContentRepositoryContext } from '../content-repository-context';
import { DigitalContent } from '../models/digital-content';
import { DigitalContentQueryMode } from '../dtos/digital-content-query-mode.model';
import { DigitalContentSearchResult } from '../dtos/digital-content-search-result';
import { DigitalContentSortField } from '../dtos/digital-content-sort-field.model';
import { DigitalContentStatus } from '../../share/models/digital-content-status.enum';
import { Injectable } from '@angular/core';
import { SortDirection } from '../../share/dtos/sort-direction';
import { UserRepository } from '../../user/repositories/user.repository';

@Injectable()
export class ContentRepository extends BaseRepository<ContentRepositoryContext> {
  constructor(context: ContentRepositoryContext, private apiSvc: ContentApiService, private userRepository: UserRepository) {
    super(context);
  }

  public loadSearchDigitalContents(
    searchText: string = '',
    searchStatuses: DigitalContentStatus[] = [],
    skipCount: number = 0,
    maxResultCount: number = 10,
    queryMode: DigitalContentQueryMode = DigitalContentQueryMode.AllByCurrentUser,
    sortField: DigitalContentSortField = DigitalContentSortField.ChangedDate,
    sortDirection: SortDirection = SortDirection.Descending
  ): Observable<DigitalContentSearchResult> {
    return this.processUpsertData<DigitalContent, DigitalContentSearchResult>(
      this.context.contentsSubject,
      implicitLoad => {
        const obs = this.apiSvc.searchDigitalContent(
          {
            searchText: searchText,
            queryMode: queryMode,
            sortField: sortField,
            sortDirection: sortDirection,
            pagedInfo: {
              skipCount: skipCount,
              maxResultCount: maxResultCount
            },
            filterByStatus: searchStatuses ? searchStatuses : []
          },
          !implicitLoad
        );
        return from(obs);
      },
      'searchCourseList',
      [searchText, searchStatuses, skipCount, maxResultCount, queryMode],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id
    );
  }

  public loadDigitalContentById(id: string): Observable<DigitalContent> {
    return this.processUpsertData(
      this.context.contentsSubject,
      implicitLoad => from(this.apiSvc.getDigitalContent(id, !implicitLoad)),
      'loadDigitalContentById',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id];
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id,
      true,
      null,
      null,
      DigitalContent.optionalProps
    );
  }

  public loadDigitalContentByIds(ids: string[]): Observable<DigitalContent[]> {
    return this.processUpsertData(
      this.context.contentsSubject,
      implicitLoad => from(this.apiSvc.getDigitalContentByIds(ids, !implicitLoad)),
      'loadDigitalContentByIds',
      ids,
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(p => repoData[p.id]);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id
    );
  }
}
