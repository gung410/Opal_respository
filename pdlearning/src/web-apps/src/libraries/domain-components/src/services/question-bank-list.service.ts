import { Observable, of } from 'rxjs';
import { QuestionBankRepository, QuestionBankViewModel, QuestionType } from '@opal20/domain-api';

import { GridDataResult } from '@progress/kendo-angular-grid';
import { Injectable } from '@angular/core';
import { switchMap } from 'rxjs/operators';

@Injectable()
export class QuestionBankListService {
  constructor(private questionBankRepository: QuestionBankRepository) {}

  public loadQuestionBanks(
    title: string | undefined,
    questionGroupIds: string[] = [],
    questionTypes: QuestionType[] = [],
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner: boolean = true
  ): Observable<GridDataResult> {
    return this.questionBankRepository
      .searchQuestionBanks(title, questionGroupIds, questionTypes, skipCount, maxResultCount, showSpinner)
      .pipe(
        switchMap(questionBankSearchResult => {
          if (questionBankSearchResult.totalCount === 0) {
            return of(null);
          }
          const vmResult = <GridDataResult>{
            data: questionBankSearchResult.items.map(_ => new QuestionBankViewModel(_)),
            total: questionBankSearchResult.totalCount
          };
          return of(vmResult);
        })
      );
  }
}
