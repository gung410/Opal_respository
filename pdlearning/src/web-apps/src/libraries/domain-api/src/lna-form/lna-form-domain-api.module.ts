import { ModuleWithProviders, NgModule } from '@angular/core';

import { LnaFormSectionRepositoryContext } from './form-section-repository-context';
import { StandaloneSurveyAnswerApiService } from './services/form-answer.service';
import { StandaloneSurveyApiService } from './services/form.service';
import { StandaloneSurveyQuestionAnswerService } from './services/form-question-answer.service';
import { StandaloneSurveyRepository } from '././repositories/form.repository';
import { StandaloneSurveyRepositoryContext } from './form-repository-context';
import { StandaloneSurveySectionApiService } from './services/form-section-api.services';

@NgModule({
  providers: [
    StandaloneSurveyApiService,
    StandaloneSurveyAnswerApiService,
    StandaloneSurveyRepository,
    StandaloneSurveyQuestionAnswerService,
    StandaloneSurveySectionApiService
  ]
})
export class LnaFormDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: LnaFormDomainApiModule,
      providers: [StandaloneSurveyRepositoryContext, LnaFormSectionRepositoryContext]
    };
  }
}
