import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AssignedPDOResultModel } from 'app-models/mpj/assign-pdo.model';
import { CourseDetailTabEnum } from 'app-models/session.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { isEmpty } from 'lodash';
import { CourseDetailModalComponent } from '../modals/course-detail-modal/course-detail-modal.component';
import {
  ApprovalClassRunModel,
  ApprovalCourseModel,
  RegistrationModel,
} from '../models/class-registration.model';

@Component({
  selector: 'class-run-cell',
  template: `
    <div *ngIf="classRunModel" class="class-run-cell">
      <div
        class="class-run-cell__infos class-run-cell-infos"
        title="{{ classRunModel.name }}"
      >
        <div
          class="class-run-cell-infos__name"
          (click)="showPopupCourseDetail()"
        >
          <span>{{ classRunModel.name }}</span>
        </div>
        <div *ngIf="classRunModel.startDate" class="class-run-cell-infos__date">
          Start date: {{ getDateString(classRunModel.startDate) }}
        </div>
        <div *ngIf="classRunModel.endDate" class="class-run-cell-infos__date">
          End date: {{ getDateString(classRunModel.endDate) }}
        </div>
      </div>
      <!-- <a class="class-run-cell__hiddenLink" routerLink="{{detailLink}}"></a> -->
    </div>
    <div *ngIf="!classRunModel">N/A</div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class ClassRunCellRendererComponent implements ICellRendererAngularComp {
  classRunModel: ApprovalClassRunModel;
  courseModel: ApprovalCourseModel;
  detailLink: string;
  resultId: number;
  constructor(private ngbModal: NgbModal) {}

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

  getDateString(date: string): string {
    return DateTimeUtil.toDateString(date);
  }

  getTimeFromNow(date: string): string {
    return DateTimeUtil.getTimeFromNow(date);
  }

  async showPopupCourseDetail(): Promise<void> {
    const ngbModal = this.ngbModal.open(CourseDetailModalComponent, {
      centered: true,
      size: 'lg',
      windowClass: 'mobile-dialog-slide-right',
    });

    const modalRef = ngbModal.componentInstance as CourseDetailModalComponent;
    modalRef.resultId = this.resultId;
    modalRef.courseId = this.courseModel.id;
    modalRef.isExternalPDO = this.courseModel.isExternalPDO;
    modalRef.classrunModel = this.classRunModel;
    modalRef.selectedTab = CourseDetailTabEnum.ClassRun;
    modalRef.close.subscribe(() => ngbModal.close(false));
  }

  private processParam(param: any): void {
    if (!param || isEmpty(param.value)) {
      return;
    }
    this.classRunModel = param.value;

    const registrationModel: AssignedPDOResultModel | RegistrationModel =
      param.data;

    if (param.data instanceof AssignedPDOResultModel) {
      this.resultId = (registrationModel as AssignedPDOResultModel).id;
    }

    this.courseModel = registrationModel.course;
  }
}
