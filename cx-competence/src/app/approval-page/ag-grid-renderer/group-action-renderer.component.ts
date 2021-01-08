import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { ChangeDetectorRef, Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { PendingLearnerListingModalComponent } from '../modals/pending-learner-listing-modal.component';
import {
  ApprovalGroupModel,
  IApprovalUnitModel,
} from '../models/class-registration.model';

@Component({
  selector: 'group-action',
  template: `
    <div class="group-action">
      <a (click)="openListLearnerModal()" href="javascript:;">View learners</a>
    </div>
  `,
  styleUrls: [],
})
export class GroupActionRendererComponent
  extends BaseComponent
  implements ICellRendererAngularComp {
  approvalInfoModel: IApprovalUnitModel;
  courseId: string;
  classRunId: string;

  resultId: string | number;
  timestamp: string;
  departmentId: number;
  nominationData: 'Group' | 'Department' | 'MassNomination';
  nominationType: 'adhoc-nominated' | 'nominated';

  constructor(
    private globalModal: NgbModal,
    private authService: AuthService,
    protected changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);
  }

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

  openListLearnerModal(): void {
    const modalRef = this.globalModal.open(
      PendingLearnerListingModalComponent,
      {
        centered: true,
        size: 'lg',
        windowClass: 'pending-learner-listing-modal',
      }
    );

    const modalPendingLearnerIns = modalRef.componentInstance as PendingLearnerListingModalComponent;

    modalPendingLearnerIns.nominationData = this.nominationData;
    modalPendingLearnerIns.nominationType = this.nominationType;
    if (this.approvalInfoModel instanceof ApprovalGroupModel) {
      modalPendingLearnerIns.groupId = this.approvalInfoModel.id;
    }

    modalPendingLearnerIns.courseId = this.courseId;
    modalPendingLearnerIns.classRunId = this.classRunId;
    modalPendingLearnerIns.resultId = this.resultId;
    modalPendingLearnerIns.departmentId = this.authService.userDepartmentId;
    modalPendingLearnerIns.timestamp = this.timestamp;
    modalPendingLearnerIns.loadLearners();

    this.subscriptionAdder = modalPendingLearnerIns.cancel.subscribe(() => {
      modalRef.close();
    });
  }

  processParam(param: any): void {
    // tslint:disable: no-unsafe-any
    if (param.value) {
      this.nominationType = param.value.type;
      this.nominationData = param.value.nominationType;

      if (this.nominationType === 'adhoc-nominated') {
        switch (param.value.nominationType) {
          case 'Department':
            this.courseId = param.data.course.id;
            this.classRunId = param.data.classRun.id;
            this.approvalInfoModel = param.data.department;
            this.departmentId = this.approvalInfoModel.id;
            this.timestamp = param.data.timestamp;
            break;

          case 'Group':
            this.courseId = param.data.course.id;
            this.classRunId = param.data.classRun.id;
            this.approvalInfoModel = param.data.group;
            this.departmentId = this.approvalInfoModel.id;
            this.timestamp = param.data.timestamp;
            break;

          case 'MassNomination':
            this.departmentId = param.data.objectiveInfo.identity.id;
            this.resultId = param.data.resultIdentity.id;
            this.timestamp = param.data.timestamp;
            break;
          default:
            break;
        }
      }
      if (this.nominationType === 'nominated') {
        switch (param.value.nominationType) {
          case 'Department':
            this.courseId = param.data.course.id;
            this.classRunId = param.data.classRun.id;
            this.approvalInfoModel = param.data.department;
            this.departmentId = this.approvalInfoModel.id;
            this.timestamp = param.data.timestamp;
            break;

          case 'Group':
            this.courseId = param.data.course.id;
            this.classRunId = param.data.classRun.id;
            this.approvalInfoModel = param.data.group;
            this.departmentId = this.approvalInfoModel.id;
            this.timestamp = param.data.timestamp;
            break;
          case 'MassNomination':
            this.departmentId = param.data.objectiveInfo.identity.id;
            this.resultId = param.data.resultIdentity.id;
            this.timestamp = param.data.timestamp;
            break;
          default:
            break;
        }
      }
    }
  }
}
