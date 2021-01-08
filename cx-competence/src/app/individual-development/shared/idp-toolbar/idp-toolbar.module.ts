import { NgModule } from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { IdpToolbarComponent } from './idp-toolbar.component';

@NgModule({
  declarations: [IdpToolbarComponent],
  exports: [IdpToolbarComponent],
  imports: [SharedModule, CxCommonModule],
})
export class IdpToolbarModule {}
