import { ModuleWithProviders, NgModule } from '@angular/core';

import { AssessmentApiService } from './services/assessment-api.service';
import { AssessmentRepository } from './repositories/assessment.repository';
import { FormAnswerApiService } from './services/form-answer.service';
import { FormApiService } from './services/form.service';
import { FormQuestionAnswerService } from './services/form-question-answer.service';
import { FormRepository } from '././repositories/form.repository';
import { FormRepositoryContext } from './form-repository-context';

@NgModule({
  providers: [FormApiService, FormAnswerApiService, FormRepository, FormQuestionAnswerService, AssessmentApiService, AssessmentRepository]
})
export class FormDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: FormDomainApiModule,
      providers: [FormRepositoryContext]
    };
  }
}
