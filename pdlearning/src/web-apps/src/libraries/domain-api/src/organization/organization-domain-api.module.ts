import { ModuleWithProviders, NgModule } from '@angular/core';

import { DepartmentApiService } from './services/department-api.service';
import { OrganizationApiService } from './services/organization-api.service';
import { OrganizationRepository } from './repositories/organization.repository';
import { OrganizationRepositoryContext } from './organization-repository-context';

@NgModule({
  providers: [DepartmentApiService, OrganizationRepository, OrganizationApiService]
})
export class OrganizationDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: OrganizationDomainApiModule,
      providers: [OrganizationRepositoryContext]
    };
  }
}
