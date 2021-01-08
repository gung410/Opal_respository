import { ModuleWithProviders, NgModule } from '@angular/core';

import { NewsfeedApiService } from './services/newsfeed.service';
import { NewsfeedRepository } from './repositories/newsfeed.repository';
import { NewsfeedRepositoryContext } from './newsfeed-repository-context';

@NgModule({
  providers: [NewsfeedRepositoryContext, NewsfeedApiService, NewsfeedRepository]
})
export class NewsfeedDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: NewsfeedDomainApiModule,
      providers: []
    };
  }
}
