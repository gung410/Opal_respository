import { ModuleWithProviders, NgModule } from '@angular/core';

import { WebinarApiService } from './services/webinar-api.service';

@NgModule({
  providers: [WebinarApiService]
})
export class WebinarDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: WebinarDomainApiModule,
      providers: []
    };
  }
}
