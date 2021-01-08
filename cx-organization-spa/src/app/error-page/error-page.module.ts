import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'app/shared/shared.module';

import { ErrorPageComponent } from './error-page.component';

@NgModule({
  declarations: [ErrorPageComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([{ path: '', component: ErrorPageComponent }])
  ]
})
export class ErrorPageModule {}
