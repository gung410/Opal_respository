import { ModuleWithProviders, NgModule } from '@angular/core';

import { IdpRepository } from './repositories/idp.repository';
import { IdpRepositoryContext } from './cx-competence-repository-context';
import { IndividualDevelopmentPlanApiService } from './services/idp-backend.service';

@NgModule({
  providers: [IdpRepositoryContext, IndividualDevelopmentPlanApiService, IdpRepository]
})
export class CxCompetenceDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: CxCompetenceDomainApiModule,
      providers: []
    };
  }
}
