import { ModuleWithProviders, NgModule } from '@angular/core';

import { PersonalFileRepository } from './services/personal-file.repository';
import { PersonalFileRepositoryContext } from './personal-file-repository-context';
import { PersonalSpaceApiService } from './services/personal-space.service';

@NgModule({
  providers: [PersonalSpaceApiService, PersonalFileRepository]
})
export class PersonalSpaceApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: PersonalSpaceApiModule,
      providers: [PersonalFileRepositoryContext]
    };
  }
}
