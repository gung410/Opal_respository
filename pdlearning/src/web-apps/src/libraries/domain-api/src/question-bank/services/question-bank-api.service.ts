import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { IQuestionBank, QuestionBank } from '../models/question-bank';
import { IQuestionBankSearchResult, QuestionBankSearchResult } from '../dtos/question-bank-search-result';
import { IQuestionGroupSearchResult, QuestionGroupSearchResult } from '../dtos/question-group-search-result';

import { IQuestionBankSearchRequest } from '../dtos/question-bank-search-request';
import { IQuestionGroupSearchRequest } from '../dtos/question-group-search-request';
import { ISaveQuestionBankRequest } from '../dtos/save-question-bank-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class QuestionBankApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.formApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public createQuestionBank(request: ISaveQuestionBankRequest): Promise<QuestionBank> {
    return this.post<ISaveQuestionBankRequest, IQuestionBank>(`/question-bank/create`, request)
      .pipe(map(data => new QuestionBank(data)))
      .toPromise();
  }

  public updateQuestionBank(request: ISaveQuestionBankRequest): Promise<QuestionBank> {
    return this.put<ISaveQuestionBankRequest, IQuestionBank>(`/question-bank/update`, request)
      .pipe(map(data => new QuestionBank(data)))
      .toPromise();
  }

  public searchQuestionBanks(request: IQuestionBankSearchRequest, showSpinner?: boolean): Promise<QuestionBankSearchResult> {
    return this.post<IQuestionBankSearchRequest, IQuestionBankSearchResult>('/question-bank/search', request, showSpinner)
      .pipe(map(_ => new QuestionBankSearchResult(_)))
      .toPromise();
  }

  public searchQuestionGroups(request: IQuestionGroupSearchRequest, showSpinner?: boolean): Observable<QuestionGroupSearchResult> {
    return this.post<IQuestionGroupSearchRequest, IQuestionGroupSearchResult>(
      '/question-bank/question-group/search',
      request,
      showSpinner
    ).pipe(map(_ => new QuestionGroupSearchResult(_)));
  }

  public deleteQuestionBank(id: string): Promise<void> {
    return this.delete<void>(`/question-bank/${id}`).toPromise();
  }
}
