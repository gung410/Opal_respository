import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  ViewEncapsulation
} from '@angular/core';
import { BroadcastMessagesDto } from './../models/broadcast-messages.model';

import { BaseSmartComponent } from 'app/shared/components/component.abstract';

import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';

@Component({
  selector: 'broadcast-message-status',
  templateUrl: './broadcast-message-status.component.html',
  styleUrls: ['./broadcast-message-status.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class BroadcastMessageStatusComponent extends BaseSmartComponent {
  get statusClass(): string {
    if (!this.broadcastMessage) {
      return;
    }

    const displayedStatus = this.getDisplayedStatus(this.broadcastMessage);

    return this.getStatusClass(displayedStatus);
  }

  get status(): string {
    if (!this.broadcastMessage) {
      return;
    }

    switch (this.getDisplayedStatus(this.broadcastMessage)) {
      case 'ACTIVE':
        return 'Active';
      case 'DEACTIVATED':
        return 'Deactivated';
      case 'EXPIRED':
        return 'Expired';
      case 'NOT SENT':
        return 'Not Sent';
      default:
        return '';
    }
  }

  @Input() broadcastMessage: BroadcastMessagesDto;
  statusHistoricalData: any = null;
  statusTypeEnum: any = StatusTypeEnum;

  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }

  getStatusClass(broadcastMessageStatus: string): string {
    let strClass = '';

    switch (broadcastMessageStatus) {
      case 'ACTIVE':
        strClass = 'success-status';
        break;
      case 'DEACTIVATED':
        strClass = 'danger-status';
        break;
      case 'EXPIRED':
        strClass = 'disabled-status';
        break;
      case 'NOT SENT':
        strClass = 'warning-status';
        break;
      default:
        strClass = '';
        break;
    }

    return strClass;
  }

  private getDisplayedStatus(
    broadcastMessageDto: BroadcastMessagesDto
  ): string {
    if (broadcastMessageDto.status === 'Deactivate') {
      return 'DEACTIVATED';
    }
    const currentDateTime = new Date();
    if (
      DateTimeUtil.compareDate(
        currentDateTime,
        new Date(broadcastMessageDto.validFromDate)
      ) < 0
    ) {
      return 'NOT SENT';
    }
    if (
      DateTimeUtil.compareDate(
        currentDateTime,
        new Date(broadcastMessageDto.validToDate)
      ) > 0
    ) {
      return 'EXPIRED';
    }
    if (
      DateTimeUtil.compareDate(
        currentDateTime,
        new Date(broadcastMessageDto.validFromDate)
      ) > 0 &&
      DateTimeUtil.compareDate(
        currentDateTime,
        new Date(broadcastMessageDto.validToDate)
      ) < 0
    ) {
      return 'ACTIVE';
    }

    return 'None';
  }
}
