import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BatchJobsMonitoringComponent } from './batch-jobs-monitoring.component';

@NgModule({
  declarations: [BatchJobsMonitoringComponent],
  imports: [
    RouterModule.forChild([
      { path: '', component: BatchJobsMonitoringComponent }
    ])
  ],
  exports: [BatchJobsMonitoringComponent],
  entryComponents: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class BatchJobsMonitoringModule {}
