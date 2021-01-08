import { FormStatus, FormSurveyType, FormType } from '../models/form.model';
import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { Constant } from '@opal20/authentication';
import { FormApiService } from './../services/form.service';
import { FormRepositoryContext } from '../form-repository-context';
import { Injectable } from '@angular/core';
import { SearchFormResponse } from '../dtos/search-form-request';

@Injectable()
export class FormRepository extends BaseRepository<FormRepositoryContext> {
  constructor(context: FormRepositoryContext, private formApiService: FormApiService) {
    super(context);
  }

  public searchForm(
    filterByStatus: FormStatus[] = [FormStatus.Published],
    filterByType: FormType = FormType.Survey,
    filterBySurveyTypes: FormSurveyType[] = [],
    skipCount: number = 0,
    maxResultCount: number = Constant.MAX_ITEMS_PER_REQUEST,
    searchFormTitle: string | undefined = undefined,
    includeFormForImportToCourse: boolean = false
  ): Observable<SearchFormResponse> {
    return this.processUpsertData(
      this.context.formListDataSubject,
      implicitLoad =>
        from(
          this.formApiService.searchForm(
            skipCount,
            maxResultCount,
            searchFormTitle,
            filterByStatus,
            includeFormForImportToCourse,
            filterByType,
            filterBySurveyTypes,
            !implicitLoad
          )
        ),
      'searchForm',
      [skipCount, maxResultCount, searchFormTitle, filterByStatus, includeFormForImportToCourse, filterByType, filterBySurveyTypes],
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
