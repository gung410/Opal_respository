import {
  Component,
  EventEmitter,
  Input,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { LearnerAssignPDOResultModel } from 'app-models/mpj/assign-pdo.model';
import { PDOAddType } from 'app-models/mpj/pdo-action-item.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { AdhocNominationService } from 'app-services/idp/assign-pdo/adhoc-nomination.service';
import { AssignPDOService } from 'app-services/idp/assign-pdo/assign-pdo.service';
import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';
import { OdpApprovalPageService } from '../services/odp-approval-page.services';

@Component({
  selector: 'pending-learner-listing-modal',
  templateUrl: './pending-learner-listing-modal.component.html',
  styles: [
    `
      .pending-learner-listing-modal .modal-dialog {
        max-width: 1000px;
      }
      .pending-learner-listing-modal .cx-dialog-template--fixed {
        height: initial;
      }
    `,
  ],
  encapsulation: ViewEncapsulation.None,
})
export class PendingLearnerListingModalComponent {
  @Output() groupId: number;
  @Input() courseId: string;
  @Input() classRunId: string;
  @Input() departmentId: number;
  @Input() resultId: string | number;
  @Input() timestamp: string;
  @Input() nominationData: 'Group' | 'Department' | 'MassNomination';
  @Input() nominationType: 'adhoc-nominated' | 'nominated';

  pageSize: number;
  pageIndex: number;

  @Output() cancel: EventEmitter<void> = new EventEmitter<void>();

  learnerPagingData: PagingResponseModel<LearnerAssignPDOResultModel>;

  constructor(
    private globalLoader: CxGlobalLoaderService,
    private adhocNominationService: AdhocNominationService,
    private assignPDOService: AssignPDOService,
    private odpApprovalPageService: OdpApprovalPageService
  ) {}

  async loadLearners(
    pageIndex: number = 1,
    pageSize: number = 5
  ): Promise<void> {
    this.pageSize = pageSize;
    this.pageIndex = pageIndex;

    this.globalLoader.showLoader();
    if (this.nominationType === 'adhoc-nominated') {
      switch (this.nominationData) {
        case 'Group':
          this.learnerPagingData = await this.adhocNominationService.getLearnerAdhocNominationOfGroupAsync(
            this.departmentId,
            this.groupId,
            this.courseId,
            this.classRunId,
            NominateStatusCodeEnum.PendingForApproval,
            this.timestamp,
            pageIndex,
            pageSize
          );
          break;

        case 'Department':
          this.learnerPagingData = await this.adhocNominationService.getLearnerAdhocNominationOfDepartmentAsync(
            this.departmentId,
            this.courseId,
            this.classRunId,
            NominateStatusCodeEnum.PendingForApproval,
            this.timestamp,
            pageIndex,
            pageSize
          );
          break;

        case 'MassNomination':
          this.learnerPagingData = await this.adhocNominationService.getAdhocMassNominationLearnerAsync(
            this.departmentId,
            this.resultId,
            NominateStatusCodeEnum.PendingForApproval,
            this.timestamp,
            pageIndex,
            pageSize
          );
          break;
        default:
          break;
      }
    }

    if (this.nominationType === 'nominated') {
      switch (this.nominationData) {
        case 'Group':
          this.learnerPagingData = await this.assignPDOService.getLearnerAssignedPDOForGroupAsync(
            this.departmentId,
            PDOAddType.Nominated,
            this.groupId,
            this.timestamp,
            this.courseId,
            this.classRunId,
            NominateStatusCodeEnum.PendingForApproval,
            pageIndex,
            pageSize,
            false
          );
          break;

        case 'Department':
          this.learnerPagingData = await this.assignPDOService.getLearnerAssignedPDOForDepartmentAsync(
            this.departmentId,
            PDOAddType.Nominated,
            this.timestamp,
            this.courseId,
            this.classRunId,
            NominateStatusCodeEnum.PendingForApproval,
            pageIndex,
            pageSize,
            false
          );
          break;

        case 'MassNomination':
          this.learnerPagingData = await this.odpApprovalPageService.getMassNominationLearnerAsync(
            this.departmentId,
            this.resultId,
            NominateStatusCodeEnum.PendingForApproval,
            this.timestamp,
            pageIndex,
            pageSize
          );
          break;

        default:
          break;
      }
    }

    this.globalLoader.hideLoader();
  }

  onPagingChanged(pagingInfo: { pageSize: number; pageIndex: number }): void {
    this.loadLearners(pagingInfo.pageIndex, pagingInfo.pageSize);
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
