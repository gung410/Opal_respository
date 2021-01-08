import { ISurveyQuestionAnswerStatisticsModel, SurveyQuestionAnswerStatisticsModel } from '../models/form-question-answer-statistics.model';

import { BaseStandaloneSurveyService } from './base-standalone-survey.service';
import { CommonFacadeService } from '@opal20/infrastructure';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class StandaloneSurveyQuestionAnswerService extends BaseStandaloneSurveyService {
  protected get apiUrl(): string {
    return AppGlobal.environment.lnaFormApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getQuestionStatistics(formQuestionId: string): Observable<SurveyQuestionAnswerStatisticsModel[]> {
    return this.get<ISurveyQuestionAnswerStatisticsModel[]>(`/survey-question-answers/question-ids/${formQuestionId}/statistics`);
  }
}
