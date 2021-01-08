import { ModuleWithProviders, NgModule } from '@angular/core';

import { CatalogueRepository } from './repositories/catalogue.repository';
import { LearningCatalogueRepositoryContext } from './learning-catalogue-repository-context';
import { PDCatalogueApiService } from './services/pd-catalog-backend.service';

@NgModule({
  providers: [PDCatalogueApiService]
})
export class LearningCatalogueDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: LearningCatalogueDomainApiModule,
      providers: [LearningCatalogueRepositoryContext, CatalogueRepository]
    };
  }
}
