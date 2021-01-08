import { ModuleWithProviders, NgModule } from '@angular/core';

import { FormParticipantApiService } from './services/form-participant-api.service';
import { FormParticipantRepository } from './repositories/form-participant.repository';
import { FormParticipantRepositoryContext } from './form-participant-repository-context';

@NgModule({
  providers: [FormParticipantRepository]
})
export class FormParticipantDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: FormParticipantDomainApiModule,
      providers: [FormParticipantRepositoryContext, FormParticipantApiService]
    };
  }
}
