import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { FormQuestionAnswerStatisticsModel, IFormQuestionAnswerStatisticsModel } from '../models/form-question-answer-statistics.model';

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class FormQuestionAnswerService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.formApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getQuestionStatistics(formQuestionId: string): Observable<FormQuestionAnswerStatisticsModel[]> {
    return this.get<IFormQuestionAnswerStatisticsModel[]>(`/form-question-answers/question-ids/${formQuestionId}/statistics`);
  }
}
