import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { AssessmentStatusInfo } from 'app-models/assessment.model';

@Component({
  selector: 'assessment-status-indicator',
  template: `
    <idp-status-block [idpStatusInfo]="assessmentStatusInfo"></idp-status-block>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class AssessmentStatusCellRendererComponent
  implements ICellRendererAngularComp {
  assessmentStatusInfo: AssessmentStatusInfo;

  constructor() {}

  // called on init
  agInit(param: any): boolean {
    this.processParam(param);

    return true;
  }

  // called when the cell is refreshed
  refresh(param: any): boolean {
    this.processParam(param);

    return true;
  }

  private processParam(param: any): void {
    if (!param) {
      return;
    }

    this.assessmentStatusInfo = param.value;
  }
}
