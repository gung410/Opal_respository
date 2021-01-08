import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { IdpStatusCodeEnum } from 'app/individual-development/idp.constant';
import { PDPlanProcessClass } from 'app/shared/constants/pdplan-process-constant';
import { StaffListService } from 'app/staff/staff.container/staff-list.service';

@Component({
  selector: 'status-cell',
  template: `<div class="staff-table__progress-status">
    <div
      *ngIf="assessmentInfo && assessmentInfo.statusInfo"
      class="staff-table__items--info"
      data-toggle="tooltip"
      title="{{
        assessmentInfo && assessmentInfo.dueDate
          ? 'Complete by: ' + (assessmentInfo.dueDate | date: 'dd/MM/yyyy')
          : ''
      }}"
      (click)="onProgressClick()"
    >
      <idp-status-block
        [idpStatusInfo]="assessmentInfo.statusInfo"
      ></idp-status-block>
    </div>
  </div>`,
  // styleUrls: ['../staff-list.component.scss'],
})
export class PdPlanStatusRendererComponent implements ICellRendererAngularComp {
  public pDPlanProcessClass: string;
  public displayProcessText: string;
  public assessmentInfo: any;
  public userId: any;
  constructor(
    private router: Router,
    private staffListService: StaffListService
  ) {}

  // called on init
  public agInit(params: any): void {
    const info = params.value.assessmentInfo;
    if (!info || !info.statusInfo) {
      return;
    }
    this.assessmentInfo = info;
    this.userId = params.value.userId;
    this.initData();
  }
  // called when the cell is refreshed
  public refresh(params: any): boolean {
    const info = params.value.assessmentInfo;
    if (!info || !info.statusInfo) {
      return;
    }
    this.assessmentInfo = info;
    this.userId = params.value.userId;
    this.initData();

    return true;
  }

  public initData() {
    if (
      this.assessmentInfo.statusInfo.assessmentStatusCode ===
        IdpStatusCodeEnum.NotAdded ||
      this.assessmentInfo.statusInfo.assessmentStatusCode ===
        IdpStatusCodeEnum.NotStarted
    ) {
      this.pDPlanProcessClass = PDPlanProcessClass.NotStarted;
    } else {
      this.pDPlanProcessClass =
        PDPlanProcessClass[this.assessmentInfo.statusInfo.assessmentStatusCode];
    }
    this.displayProcessText =
      this.assessmentInfo && this.assessmentInfo.statusInfo.assessmentStatusName
        ? this.assessmentInfo.statusInfo.assessmentStatusName
        : '';
  }

  public onProgressClick() {
    if (!this.userId) {
      return;
    }
    this.staffListService.resetSearchValueSubject.next(true);
    this.router.navigate([`/employee/detail/${this.userId}`]);
  }
}
