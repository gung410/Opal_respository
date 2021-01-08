import { Component, Input } from '@angular/core';
import { ApprovalClassRunModel } from 'app/approval-page/models/class-registration.model';

@Component({
  selector: 'overall-classrun-info',
  templateUrl: './overall-classrun-info.component.html',
})
export class OverallClassrunInfoComponent {
  @Input() classRunInfo: ApprovalClassRunModel;
  constructor() {}
}
