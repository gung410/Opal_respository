import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'app/shared/shared.module';

import { ManagementComponent } from './management.component';

@NgModule({
  declarations: [ManagementComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([{ path: '', component: ManagementComponent }])
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ManagementModule {}
