import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AssignedPDOResultModel } from 'app-models/mpj/assign-pdo.model';
import { CourseDetailModalComponent } from 'app/approval-page/modals/course-detail-modal/course-detail-modal.component';
import { isEmpty } from 'lodash';
import {
  ApprovalClassRunModel,
  ApprovalCourseModel,
  RegistrationModel,
} from '../models/class-registration.model';

@Component({
  selector: 'course-cell',
  template: `
    <div *ngIf="courseModel" class="course-cell">
      <div
        class="course-cell__infos course-cell-infos"
        title="{{ courseModel.name }}"
      >
        <div class="course-cell-infos__name" (click)="showPopupCourseDetail()">
          <span>{{ courseModel.name }}</span>
        </div>
      </div>
    </div>
    <div *ngIf="!courseModel">N/A</div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class CourseCellRendererComponent implements ICellRendererAngularComp {
  courseModel: ApprovalCourseModel;
  classrunModel: ApprovalClassRunModel;
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
    modalRef.classrunModel = this.classrunModel;
    modalRef.close.subscribe(() => ngbModal.close(false));
  }

  processParam(param: any): void {
    if (!param || isEmpty(param.value)) {
      return;
    }

    this.courseModel = param.value;

    const registrationModel: AssignedPDOResultModel | RegistrationModel =
      param.data;

    if (param.data instanceof AssignedPDOResultModel) {
      this.resultId = (registrationModel as AssignedPDOResultModel).id;
    }

    this.classrunModel = registrationModel.classRun;
  }
}
