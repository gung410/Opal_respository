import { ModuleWithProviders, NgModule } from '@angular/core';

import { VersionTrackingApiService } from './services/version-tracking-api.services';
import { VersionTrackingComponentService } from './services/version-tracking-component.service';
import { VersionTrackingRepository } from './repositories/version-tracking.repository';
import { VersionTrackingRepositoryContext } from './version-tracking-repository-context';

@NgModule({
  providers: [VersionTrackingRepository, VersionTrackingComponentService, VersionTrackingApiService]
})
export class VersionTrackingDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: VersionTrackingDomainApiModule,
      providers: [VersionTrackingRepositoryContext]
    };
  }
}
