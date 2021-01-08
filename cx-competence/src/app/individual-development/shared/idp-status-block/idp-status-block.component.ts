import { Component, Input, OnChanges } from '@angular/core';
import { AssessmentStatusInfo } from 'app-models/assessment.model';

enum PlanStatusClassEnum {
  NotStarted = 'not-started',
  Started = 'started',
  PendingForApproval = 'pending-for-approval',
  Approved = 'approved',
  Completed = 'completed',
  Rejected = 'rejected',
  ExternalRejected = 'rejected',
  ExternalPendingForApproval = 'pending-for-approval',
  InCompleted = 'incompleted',
}

@Component({
  selector: 'idp-status-block',
  templateUrl: './idp-status-block.component.html',
  styleUrls: ['./idp-status-block.component.scss'],
})
export class IdpStatusBlockComponent implements OnChanges {
  @Input() idpStatusInfo: AssessmentStatusInfo;
  statusName: string = 'N/A';
  statusClass: string = 'not-started';

  ngOnChanges(): void {
    if (this.idpStatusInfo) {
      this.statusName = this.idpStatusInfo.assessmentStatusName;
      const statusCode = this.idpStatusInfo.assessmentStatusCode;
      this.statusClass =
        PlanStatusClassEnum[statusCode] || PlanStatusClassEnum.NotStarted;
    }
  }
}
