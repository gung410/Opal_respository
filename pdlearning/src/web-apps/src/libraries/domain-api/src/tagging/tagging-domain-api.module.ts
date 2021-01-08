import { Injector, ModuleWithProviders, NgModule } from '@angular/core';

import { CommunityTaggingApiService } from './services/community-tagging-api.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NoopHttpResponseInterceptor } from '@opal20/infrastructure';
import { TaggingApiService } from './services/tagging-api.service';
import { TaggingRepository } from './repositories/tagging.repository';
import { TaggingRepositoryContext } from './tagging-repository-context';

@NgModule({
  providers: [
    TaggingApiService,
    TaggingRepository,
    CommunityTaggingApiService,
    { provide: HTTP_INTERCEPTORS, useClass: NoopHttpResponseInterceptor, deps: [Injector], multi: true }
  ]
})
export class TaggingDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: TaggingDomainApiModule,
      providers: [TaggingRepositoryContext]
    };
  }
}
