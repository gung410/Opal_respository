import { ModuleWithProviders, NgModule } from '@angular/core';

import { QuestionBankApiService } from './services/question-bank-api.service';
import { QuestionBankRepository } from './repositories/question-bank.repository';
import { QuestionBankRepositoryContext } from './question-bank-repository-context';

@NgModule({
  providers: [QuestionBankApiService, QuestionBankRepository]
})
export class QuestionBankDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: QuestionBankDomainApiModule,
      providers: [QuestionBankRepositoryContext]
    };
  }
}
