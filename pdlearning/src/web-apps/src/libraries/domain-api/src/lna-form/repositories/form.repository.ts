import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { Constant } from '@opal20/authentication';
import { Injectable } from '@angular/core';
import { SearchSurveyResponse } from '../dtos/search-form-request';
import { StandaloneSurveyApiService } from './../services/form.service';
import { StandaloneSurveyRepositoryContext } from '../form-repository-context';
import { SurveyStatus } from '../models/lna-form.model';

@Injectable()
export class StandaloneSurveyRepository extends BaseRepository<StandaloneSurveyRepositoryContext> {
  constructor(context: StandaloneSurveyRepositoryContext, private formApiService: StandaloneSurveyApiService) {
    super(context);
  }

  public searchForm(
    filterByStatus: SurveyStatus[] = [SurveyStatus.Published],
    skipCount: number = 0,
    maxResultCount: number = Constant.MAX_ITEMS_PER_REQUEST,
    searchFormTitle: string | undefined = undefined,
    includeFormForImportToCourse: boolean = false
  ): Observable<SearchSurveyResponse> {
    return this.processUpsertData(
      this.context.formListDataSubject,
      implicitLoad =>
        from(
          this.formApiService.searchSurvey(
            skipCount,
            maxResultCount,
            searchFormTitle,
            filterByStatus,
            includeFormForImportToCourse,
            !implicitLoad
          )
        ),
      'searchForm',
      [skipCount, maxResultCount, searchFormTitle, filterByStatus, includeFormForImportToCourse],
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
