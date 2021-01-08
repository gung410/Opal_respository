import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AssessmentInfo } from 'app-models/assessment-info.model';
import { PDPlanProcessClass } from 'app/shared/constants/pdplan-process-constant';
import { StaffListService } from 'app/staff/staff.container/staff-list.service';

@Component({
  selector: 'status-cell',
  templateUrl: './lna-status-renderer.component.html',
  styleUrls: ['./lna-status-renderer.component.scss'],
})
export class LNAStatusRendererComponent implements ICellRendererAngularComp {
  public assessmentInfo: AssessmentInfo;
  public userId: number;
  public pDPlanProcessClass: string;
  public displayProcessText: string;
  constructor(
    private router: Router,
    private staffListService: StaffListService
  ) {}

  // called on init
  public agInit(params: any): void {
    this.initData(params);
  }

  // called when the cell is refreshed
  public refresh(params: any): boolean {
    return this.initData(params);
  }

  public onProgressClick(): void {
    if (!this.userId) {
      return;
    }
    this.staffListService.resetSearchValueSubject.next(true);
    this.router.navigate([`/employee/detail/${this.userId}`]);
  }

  private initData(params: any): boolean {
    const info = params.value.assessmentInfo;

    if (!info || !info.statusInfo) {
      return;
    }
    this.assessmentInfo = info;
    this.userId = params.value.userId;
    this.pDPlanProcessClass =
      PDPlanProcessClass[this.assessmentInfo.statusInfo.assessmentStatusCode];
    this.displayProcessText =
      this.assessmentInfo && this.assessmentInfo.statusInfo.assessmentStatusName
        ? this.assessmentInfo.statusInfo.assessmentStatusName
        : '';

    return true;
  }
}
