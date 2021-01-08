import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { Injectable } from '@angular/core';
import { QuestionBank } from '../models/question-bank';
import { QuestionBankApiService } from '../services/question-bank-api.service';
import { QuestionBankRepositoryContext } from '../question-bank-repository-context';
import { QuestionBankSearchResult } from '../dtos/question-bank-search-result';
import { QuestionType } from '@opal20/domain-api';

@Injectable()
export class QuestionBankRepository extends BaseRepository<QuestionBankRepositoryContext> {
  constructor(context: QuestionBankRepositoryContext, private questionBankApiService: QuestionBankApiService) {
    super(context);
  }

  public searchQuestionBanks(
    title: string | undefined = undefined,
    questionGroupIds: string[] = [],
    questionTypes: QuestionType[] = [],
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner: boolean = true
  ): Observable<QuestionBankSearchResult> {
    return this.processUpsertData<QuestionBank, QuestionBankSearchResult>(
      this.context.questionBanksSubject,
      implicitLoad =>
        from(
          this.questionBankApiService.searchQuestionBanks(
            {
              title: title,
              questionGroupIds: questionGroupIds,
              questionTypes: questionTypes,
              pagedInfo: {
                skipCount: skipCount,
                maxResultCount: maxResultCount
              }
            },
            showSpinner && !implicitLoad
          )
        ),
      'searchQuestionBanks',
      [title, questionGroupIds, questionTypes, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items.map(_ => new QuestionBank(_)),
      x => x.id
    );
  }
}
