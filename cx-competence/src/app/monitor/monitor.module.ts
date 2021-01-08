import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MonitorComponent } from './monitor.component';
import { SharedModule } from 'app/shared/shared.module';
import { RouterModule } from '@angular/router';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiDetailComponent } from './api-detail/api-detail.component';

@NgModule({
  declarations: [MonitorComponent, ApiDetailComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([{ path: '', component: MonitorComponent }]),
  ],
  entryComponents: [ApiDetailComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class MonitorModule {}
