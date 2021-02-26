import { Component } from '@angular/core';
import { environment } from 'app-environments/environment';

@Component({
  selector: 'batch-jobs-monitoring',
  templateUrl: './batch-jobs-monitoring.component.html',
  styleUrls: ['./batch-jobs-monitoring.component.scss']
})
export class BatchJobsMonitoringComponent {
  baseProfileAppUrl: string = environment.baseProfileAppUrl;
}
