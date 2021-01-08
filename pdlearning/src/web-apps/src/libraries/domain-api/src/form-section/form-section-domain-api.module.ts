import { ModuleWithProviders, NgModule } from '@angular/core';

import { FormSectionApiService } from './services/form-section-api.services';
import { FormSectionRepositoryContext } from './form-section-repository-context';

@NgModule({
  providers: [FormSectionApiService]
})
export class FormSectionDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: FormSectionDomainApiModule,
      providers: [FormSectionRepositoryContext]
    };
  }
}
