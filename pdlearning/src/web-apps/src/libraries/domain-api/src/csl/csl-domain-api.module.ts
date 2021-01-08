import { ModuleWithProviders, NgModule } from '@angular/core';

import { CollaborativeSocialLearningApiService } from './services/csl-backend.service';
import { CslRepository } from './repositories/csl.repository';
import { CslRepositoryContext } from './csl-repository-context';

@NgModule({
  providers: [CslRepositoryContext, CollaborativeSocialLearningApiService, CslRepository]
})
export class CollaborativeSocialLearningApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: CollaborativeSocialLearningApiModule,
      providers: []
    };
  }
}
