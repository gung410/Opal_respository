import { ISaveSurveyAnswer, IUpdateSurveyAnswerRequest } from '../dtos/update-form-answer-request';
import { ISurveyAnswerModel, SurveyAnswerModel } from '../models/form-answer.model';

import { BaseStandaloneSurveyService } from './base-standalone-survey.service';
import { CommonFacadeService } from '@opal20/infrastructure';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class StandaloneSurveyAnswerApiService extends BaseStandaloneSurveyService {
  protected get apiUrl(): string {
    return AppGlobal.environment.lnaFormApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public saveFormAnswer(requestDto: ISaveSurveyAnswer): Observable<SurveyAnswerModel> {
    return this.post<ISaveSurveyAnswer, ISurveyAnswerModel>('/survey-answers', requestDto).pipe(
      map(savedFormAnswer => new SurveyAnswerModel(savedFormAnswer))
    );
  }

  public updateFormAnswer(requestDto: IUpdateSurveyAnswerRequest): Observable<SurveyAnswerModel> {
    return this.post<IUpdateSurveyAnswerRequest, ISurveyAnswerModel>(`/survey-answers/update`, requestDto).pipe(
      map(formAnswer => new SurveyAnswerModel(formAnswer))
    );
  }

  public getByFormId(formId: string, resourceId?: string, userId?: string): Observable<SurveyAnswerModel[]> {
    return this.get<ISurveyAnswerModel[]>(`/survey-answers/survey-ids/${formId}`, {
      resourceId: resourceId,
      userId: userId
    }).pipe(map(formAnswer => formAnswer.map(p => new SurveyAnswerModel(p))));
  }
}
