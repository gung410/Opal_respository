import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { ManagementComponent } from './management.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ManagementComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([{ path: '', component: ManagementComponent }]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ManagementModule {}
