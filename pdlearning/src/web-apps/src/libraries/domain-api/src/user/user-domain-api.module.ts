import { ModuleWithProviders, NgModule } from '@angular/core';

import { LearningCatalogApiService } from './services/learning-catalog-api.service';
import { LearningCatalogRepository } from './repositories/learning-catalog.repository';
import { UserApiService } from './services/user-api.service';
import { UserRepository } from './repositories/user.repository';
import { UserRepositoryContext } from './user-repository-context';

@NgModule({
  providers: [UserApiService, UserRepository, LearningCatalogApiService, LearningCatalogRepository]
})
export class UserDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: UserDomainApiModule,
      providers: [UserRepositoryContext]
    };
  }
}
