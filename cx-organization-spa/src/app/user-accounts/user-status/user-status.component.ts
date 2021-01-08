import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  ViewEncapsulation
} from '@angular/core';

import { BaseSmartComponent } from 'app/shared/components/component.abstract';

import {
  StatusReasonTypeConstant,
  StatusTypeEnum
} from 'app/shared/constants/user-status-type.enum';

import { UserManagement } from '../models/user-management.model';

import { UserAccountsDataService } from '../user-accounts-data.service';

@Component({
  selector: 'user-status',
  templateUrl: './user-status.component.html',
  styleUrls: ['./user-status.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class UserStatusComponent extends BaseSmartComponent {
  get statusReason(): string {
    if (this.employee) {
      return this.getStatusReason();
    }
  }

  get statusClass(): string {
    if (this.employee) {
      return this.getStatusClass();
    }
  }

  get status(): string {
    if (this.employee) {
      return this.getStatusText();
    }
  }

  @Input() employee: UserManagement;
  statusHistoricalData: any = null;
  statusTypeEnum: any = StatusTypeEnum;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private userAccountsDataService: UserAccountsDataService
  ) {
    super(changeDetectorRef);
  }

  getStatusClass(): string {
    let strClass = '';

    switch (this.employee.entityStatus.statusId) {
      case StatusTypeEnum.New.code:
      case StatusTypeEnum.Active.code:
        strClass = 'success-status';
        break;
      case StatusTypeEnum.Rejected.code:
      case StatusTypeEnum.Deactive.code:
      case StatusTypeEnum.Inactive.code:
      case StatusTypeEnum.IdentityServerLocked.code:
      case StatusTypeEnum.Archived.code:
        strClass = 'danger-status';
        break;
      case StatusTypeEnum.PendingApproval1st.code:
      case StatusTypeEnum.PendingApproval2nd.code:
      case StatusTypeEnum.PendingApproval3rd.code:
        strClass = 'danger-status';
        break;
      default:
        strClass = '';
        break;
    }

    strClass += this.employee.entityStatus.externallyMastered
      ? ' user-status__status--employee'
      : ' user-status__status--non-employee';

    return strClass;
  }

  getStatusText(): string {
    for (const key of Object.keys(StatusTypeEnum)) {
      if (this.employee.entityStatus.statusId === StatusTypeEnum[key].code) {
        return StatusTypeEnum[key].text;
      }
    }

    return '';
  }

  getStatusReason(): string {
    if (
      this.employee.entityStatus.statusReasonId ===
        StatusReasonTypeConstant.ManuallyArchived.code ||
      this.employee.entityStatus.statusReasonId ===
        StatusReasonTypeConstant.Archived_ManuallyArchived.code
    ) {
      return '';
    }

    for (const key of Object.keys(StatusReasonTypeConstant)) {
      if (
        this.employee.entityStatus.statusReasonId ===
        StatusReasonTypeConstant[key].code
      ) {
        return StatusReasonTypeConstant[key].text;
      }
    }

    return '';
  }

  onStatusClicked(): void {
    this.renderStatusHistoricalData();
  }

  private renderStatusHistoricalData(): void {
    this.subscription.add(
      this.userAccountsDataService
        .getStatusHistoricalData(this.employee.identity.id)
        .subscribe((data) => {
          if (data) {
            this.statusHistoricalData = data;
            this.changeDetectorRef.detectChanges();
          }
        })
    );
  }
}
