import { ModuleWithProviders, NgModule } from '@angular/core';

import { AccessRightApiService } from './services/access-right-api.services';
import { AccessRightComponentService } from './services/access-right-component.service';
import { AccessRightRepository } from './repositories/access-right.repository';
import { AccessRightRepositoryContext } from './access-right-repository-context';

@NgModule({
  providers: [AccessRightRepository, AccessRightComponentService, AccessRightApiService]
})
export class AccessRightDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: AccessRightDomainApiModule,
      providers: [AccessRightRepositoryContext]
    };
  }
}
