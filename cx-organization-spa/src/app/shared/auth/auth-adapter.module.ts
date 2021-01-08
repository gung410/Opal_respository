import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { OAuthModule, OAuthStorage } from '@conexus/cx-angular-common';
import { InMemoryStorage } from './memory.storage';

export function createDefaultStorage(): OAuthStorage {
  return new InMemoryStorage();
}

@NgModule({
  imports: [CommonModule, OAuthModule.forRoot()],
  providers: [{ provide: OAuthStorage, useFactory: createDefaultStorage }],
  declarations: [],
  exports: []
})
export class OAuthAdapterModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: OAuthAdapterModule
    };
  }
}
