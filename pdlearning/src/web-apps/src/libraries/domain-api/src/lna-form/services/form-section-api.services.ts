import { ISurveySection, SurveySection } from '../models/form-section';

import { BaseStandaloneSurveyService } from './base-standalone-survey.service';
import { CommonFacadeService } from '@opal20/infrastructure';
import { ICreateSurveySectionRequest } from '../dtos/create-form-section-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class StandaloneSurveySectionApiService extends BaseStandaloneSurveyService {
  protected get apiUrl(): string {
    return AppGlobal.environment.lnaFormApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public saveSurveySection(request: ICreateSurveySectionRequest): Promise<SurveySection> {
    return this.post<ICreateSurveySectionRequest, ISurveySection>(`/survey-sections`, request)
      .pipe(map(data => new SurveySection(data)))
      .toPromise();
  }

  public getSurveySectionsByFormId(formId: string): Promise<SurveySection[]> {
    return this.get<ISurveySection[]>(`/survey-sections/survey-id/${formId}`)
      .pipe(map(listSection => listSection.map(section => new SurveySection(section))))
      .toPromise();
  }
}
