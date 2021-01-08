import { NgModule } from '@angular/core';
import { ErrorPageComponent } from './error-page.component';
import { SharedModule } from 'app/shared/shared.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ErrorPageComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([{ path: '', component: ErrorPageComponent }]),
  ],
})
export class ErrorPageModule {}
