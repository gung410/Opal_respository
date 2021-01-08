import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { FileType } from '../models/file-type.enum';
import { Injectable } from '@angular/core';
import { PersonalFileRepositoryContext } from '../personal-file-repository-context';
import { PersonalFileSortField } from '../models/personal-file-sort-item.model';
import { PersonalSpaceApiService } from './personal-space.service';
import { SearchPersonalFileRequest } from './../dtos/search-personal-file-request';
import { SearchPersonalFileResponse } from '../dtos/search-personal-file-request';
import { SortDirection } from './../../share/dtos/sort-direction';

@Injectable()
export class PersonalFileRepository extends BaseRepository<PersonalFileRepositoryContext> {
  constructor(context: PersonalFileRepositoryContext, private apiSvc: PersonalSpaceApiService) {
    super(context);
  }

  public loadPersonalFiles(
    searchText: string = '',
    skipCount: number = 0,
    maxResultCount: number = 25,
    filterByExtensions: string[],
    filterByType: FileType[],
    sortDirection?: SortDirection,
    sortBy?: PersonalFileSortField,
    showSpinner?: boolean
  ): Observable<SearchPersonalFileResponse> {
    const request = <SearchPersonalFileRequest>{
      pagedInfo: {
        skipCount: skipCount,
        maxResultCount: maxResultCount
      },
      searchText: searchText,
      sortDirection: sortDirection ? sortDirection : SortDirection.Descending,
      sortBy: sortBy ? sortBy : PersonalFileSortField.CreatedDate,
      filterByExtensions: filterByExtensions ? filterByExtensions : [],
      filterByType: filterByType ? filterByType : [FileType.All]
    };

    return this.processUpsertData(
      this.context.personalFileSubject,
      implicitLoad => from(this.apiSvc.searchPersonalFiles(request, !implicitLoad && showSpinner)),
      'searchCourses',
      [searchText, sortDirection, sortBy, filterByType, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id
    );
  }
}
