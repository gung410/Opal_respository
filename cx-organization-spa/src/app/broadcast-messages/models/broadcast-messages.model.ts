import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { BroadcastMessageStatus } from 'app/shared/constants/broadcast-message-status.enum';
import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { SendMode } from 'app/shared/constants/send-mode.enum';
import { TargetUserType } from 'app/shared/constants/target-user-type.enum';
import { DayRepetition } from '../constant/day-repetition.enum';
import { MonthRepetition } from '../constant/month-repetition.enum';

// tslint:disable:max-classes-per-file
export class BroadcastMessagesFilterParams {
  searchText?: string;
  searchStatus?: BroadcastMessageStatus[];
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  orderType?: string;

  constructor(data?: Partial<BroadcastMessagesFilterParams>) {
    this.searchText = data.searchText;
    this.searchStatus = data.searchStatus;
    this.pageIndex = data.pageIndex;
    this.pageSize = data.pageSize;
    this.orderBy = data.orderBy;
    this.orderType = data.orderType;
  }
}

export class Recipients {
  departmentIds?: number[];
  roleIds?: number[];
  userIds?: string[];
  groupIds?: number[];
}

export interface IBroadcastMessageDto {
  broadcastMessageId?: string;
  title?: string;
  broadcastContent?: string;
  createdDate?: Date;
  lastUpdated?: Date;
  ownerId?: string;
  lastUpdatedBy?: string;
  recipients?: Recipients;
  validFromDate?: Date;
  validToDate?: Date;
  // status?: BroadcastMessageStatus;
  status?: string;
  targetUserType: TargetUserType;
  sendMode: SendMode;
  // Recurrence properties
  numberOfRecurrence?: number;
  recurrenceType: RecurrenceType;
  dayRepetitions?: DayRepetition[];
  monthRepetition?: MonthRepetition;
}

export class BroadcastMessagesDto implements IBroadcastMessageDto {
  static parseStatusToText(status: BroadcastMessageStatus): string {
    switch (status) {
      case BroadcastMessageStatus.None:
        return 'None';
      case BroadcastMessageStatus.Active:
        return 'Active';
      case BroadcastMessageStatus.Deactivate:
        return 'Deactivate';
      case BroadcastMessageStatus.Expired:
        return 'Expired';
      case BroadcastMessageStatus.NotSent:
        return 'Not Sent';
      default:
        break;
    }
  }

  static updateDisplayedStatus(
    broadcastMessageDto: BroadcastMessagesDto
  ): string {
    if (broadcastMessageDto.status === 'Deactivate') {
      return 'Deactivate';
    }
    const currentDateTime = new Date();
    if (
      DateTimeUtil.compareDate(
        currentDateTime,
        new Date(broadcastMessageDto.validFromDate)
      ) < 0
    ) {
      return 'Not Sent';
    }
    if (
      DateTimeUtil.compareDate(
        currentDateTime,
        new Date(broadcastMessageDto.validToDate)
      ) > 0
    ) {
      return 'Expired';
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
      return 'Active';
    }

    return 'None';
  }

  broadcastMessageId?: string;
  title?: string = '';
  broadcastContent?: string = '';
  createdDate?: Date;
  lastUpdated?: Date;
  ownerId?: string;
  lastUpdatedBy?: string;
  recipients?: Recipients = {
    departmentIds: [],
    userIds: [],
    roleIds: [],
    groupIds: []
  };
  validFromDate?: Date;
  validToDate?: Date;
  status?: string;
  targetUserType: TargetUserType;
  sendMode: SendMode;
  numberOfRecurrence?: number = 1;
  recurrenceType: RecurrenceType = RecurrenceType.Week;
  dayRepetitions?: DayRepetition[];
  monthRepetition?: MonthRepetition;

  constructor(data?: Partial<IBroadcastMessageDto>) {
    if (data == null) {
      return;
    }
    this.broadcastMessageId = data.broadcastMessageId;
    this.title = data.title;
    this.broadcastContent = data.broadcastContent;
    this.ownerId = data.ownerId;
    this.lastUpdatedBy = data.lastUpdatedBy;
    this.createdDate = data.createdDate;
    this.lastUpdated = data.lastUpdated;
    this.recipients = data.recipients;
    this.validFromDate = data.validFromDate;
    this.validToDate = data.validToDate;
    this.status = data.status;
    this.targetUserType = data.targetUserType;
    this.sendMode = data.sendMode;
    this.numberOfRecurrence = data.numberOfRecurrence;
    this.recurrenceType = data.recurrenceType;
    this.dayRepetitions = data.dayRepetitions;
    this.monthRepetition = data.monthRepetition;
  }
}
