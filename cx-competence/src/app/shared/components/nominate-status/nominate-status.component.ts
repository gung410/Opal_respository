import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  AssignedPDOResultModel,
  DepartmentAssignPDOResultModel,
  GroupAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';

@Component({
  selector: 'nominate-status',
  templateUrl: './nominate-status.component.html',
  styleUrls: ['./nominate-status.component.scss'],
})
export class NominateStatusComponent implements OnInit {
  @Input() nominateItem: AssignedPDOResultModel;
  @Input() isExternalPDO: boolean;
  @Output()
  openDetailNominateStatus: EventEmitter<NominateStatusCodeEnum> = new EventEmitter<NominateStatusCodeEnum>();

  nominateStatusCode: typeof NominateStatusCodeEnum = NominateStatusCodeEnum;

  statusName:
    | 'Pending approval'
    | 'Pending CA confirmation '
    | 'On Waitlist'
    | 'Not Nominated'
    | 'Confirmed'
    | 'Approved by CAO' // Use for external PDO only
    | 'Rejected'
    | 'Rejected by CA'
    | 'Withdrawn'
    | 'Rejected by learner'
    | 'Nomination unsuccessful'
    | 'Not nominated';

  statusClass:
    | 'nominate-status-pending'
    | 'nominate-status-approved'
    | 'nominate-status-rejected'
    | 'nominate-status-notnominated';

  confirmedStatusName: 'confirmed' | 'approved by CAO' = 'confirmed';
  statusSubName: string;
  totalLearner: number = 0;
  totalApproved: number = 0;
  totalPendingLv1: number = 0;
  totalPendingLv2: number = 0;
  totalPendingLv3: number = 0;
  totalRejectedLv1: number = 0;
  totalRejectedLv2: number = 0;
  totalRejectedLv3: number = 0;
  totalRejectedLv4: number = 0;
  totalRejectedLv5: number = 0;
  unAssignedLearner: number = 0;
  isGroupOrDepartment: boolean = false;
  groupId: number;
  groupType: 'group' | 'department';

  constructor() {}

  ngOnInit(): void {
    this.processData();
  }

  objectiveName(count: number): string {
    return count > 1 ? 'learners' : 'learner';
  }

  nominateStatusClick(status: NominateStatusCodeEnum): void {
    this.openDetailNominateStatus.emit(status);
  }

  getTargetAttr(): string {
    if (!this.groupId) {
      return '';
    }

    return `nominate-${this.groupType}-members-${this.groupId}`;
  }

  private processData(): void {
    if (!this.nominateItem) {
      return;
    }

    this.confirmedStatusName = this.isExternalPDO
      ? 'approved by CAO'
      : 'confirmed';

    if (this.nominateItem.status) {
      switch (this.nominateItem.status.assessmentStatusCode) {
        case NominateStatusCodeEnum.PendingForApproval:
          this.statusClass = 'nominate-status-pending';
          this.statusName = 'Pending approval';
          break;
        case NominateStatusCodeEnum.PendingForApproval2nd:
          this.statusClass = 'nominate-status-pending';
          this.statusName = 'Pending CA confirmation ';
          this.statusSubName = 'Approved by CAO';
          break;
        case NominateStatusCodeEnum.PendingForApproval3rd:
          this.statusClass = 'nominate-status-pending';
          this.statusName = 'On Waitlist';
          break;
        case NominateStatusCodeEnum.Approved:
          this.statusClass = 'nominate-status-approved';
          this.statusName = this.isExternalPDO
            ? 'Approved by CAO'
            : 'Confirmed';
          break;
        case NominateStatusCodeEnum.Rejected:
          this.statusClass = 'nominate-status-rejected';
          this.statusName = 'Rejected';
          break;
        case NominateStatusCodeEnum.Rejected2nd:
          this.statusClass = 'nominate-status-rejected';
          this.statusName = 'Rejected by CA';
          break;
        case NominateStatusCodeEnum.Rejected3nd:
          this.statusClass = 'nominate-status-rejected';
          this.statusName = 'Withdrawn';
          break;
        case NominateStatusCodeEnum.Rejected4th:
          this.statusClass = 'nominate-status-rejected';
          this.statusName = 'Rejected by learner';
          break;
        case NominateStatusCodeEnum.Rejected5th:
          this.statusClass = 'nominate-status-rejected';
          this.statusName = 'Nomination unsuccessful';
          break;
        case NominateStatusCodeEnum.NotNominated:
          this.statusClass = 'nominate-status-notnominated';
          this.statusName = 'Not nominated';
          break;
        default:
          break;
      }
    }

    if (this.nominateItem instanceof GroupAssignPDOResultModel) {
      const groupResult = this.nominateItem as GroupAssignPDOResultModel;
      const groupDetail = groupResult ? groupResult.group : undefined;
      if (groupDetail) {
        this.groupId = groupDetail.id;
        this.groupType = 'group';

        this.isGroupOrDepartment = true;
        this.totalLearner = groupDetail.totalLearner || 0;
        this.totalApproved = groupDetail.totalApproved || 0;
        this.totalPendingLv1 = groupDetail.totalPendingLv1 || 0;
        this.totalPendingLv2 = groupDetail.totalPendingLv2 || 0;
        this.totalPendingLv3 = groupDetail.totalPendingLv3 || 0;
        this.totalRejectedLv1 = groupDetail.totalRejectedLv1 || 0;
        this.totalRejectedLv2 = groupDetail.totalRejectedLv2 || 0;
        this.totalRejectedLv3 = groupDetail.totalRejectedLv3 || 0;
        this.totalRejectedLv4 = groupDetail.totalRejectedLv4 || 0;
        this.totalRejectedLv5 = groupDetail.totalRejectedLv5 || 0;
        this.unAssignedLearner =
          this.totalLearner -
          this.totalApproved -
          this.totalPendingLv1 -
          this.totalPendingLv2 -
          this.totalPendingLv3 -
          this.totalRejectedLv1 -
          this.totalRejectedLv2 -
          this.totalRejectedLv3 -
          this.totalRejectedLv4 -
          this.totalRejectedLv5;
      }
    } else if (this.nominateItem instanceof DepartmentAssignPDOResultModel) {
      const departmentResult = this
        .nominateItem as DepartmentAssignPDOResultModel;
      const departmentDetail = departmentResult
        ? departmentResult.department
        : undefined;
      if (departmentDetail) {
        this.groupId = departmentDetail.id;
        this.groupType = 'department';

        this.isGroupOrDepartment = true;
        this.totalLearner = departmentDetail.totalLearner || 0;
        this.totalApproved = departmentDetail.totalApproved || 0;
        this.totalPendingLv1 = departmentDetail.totalPendingLv1 || 0;
        this.totalPendingLv2 = departmentDetail.totalPendingLv2 || 0;
        this.totalPendingLv3 = departmentDetail.totalPendingLv3 || 0;
        this.totalRejectedLv1 = departmentDetail.totalRejectedLv1 || 0;
        this.totalRejectedLv2 = departmentDetail.totalRejectedLv2 || 0;
        this.totalRejectedLv3 = departmentDetail.totalRejectedLv3 || 0;
        this.totalRejectedLv4 = departmentDetail.totalRejectedLv4 || 0;
        this.totalRejectedLv5 = departmentDetail.totalRejectedLv5 || 0;
        this.unAssignedLearner =
          this.totalLearner -
          this.totalApproved -
          this.totalPendingLv1 -
          this.totalPendingLv2 -
          this.totalPendingLv3 -
          this.totalRejectedLv1 -
          this.totalRejectedLv2 -
          this.totalRejectedLv3 -
          this.totalRejectedLv4 -
          this.totalRejectedLv5;
      }
    }
  }
}
