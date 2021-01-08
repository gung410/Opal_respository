import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AssessmentInfo } from 'app-models/assessment-info.model';
import { IdpStatusCodeEnum } from 'app/individual-development/idp.constant';

@Component({
  selector: 'lna-completion-rate-cell',
  templateUrl: './lna-completion-rate-renderer.component.html',
  styleUrls: ['./lna-completion-rate-renderer.component.scss'],
})
export class LnaCompletionRateRendererComponent
  implements ICellRendererAngularComp {
  public assessmentInfo: AssessmentInfo;
  public isAssignedLNA: boolean;
  public completionRate: number = 0;
  constructor(private router: Router) {}

  // called on init
  public agInit(params: any): void {
    this.initData(params);
  }

  // called when the cell is refreshed
  public refresh(params: any): boolean {
    return this.initData(params);
  }

  private initData(params: any): boolean {
    const info = params.value;

    if (!info) {
      return;
    }

    this.completionRate = info.completionRate || 0;
    this.isAssignedLNA =
      info.statusInfo.assessmentStatusCode !== IdpStatusCodeEnum.NotAdded;
    return true;
  }
}
