import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { SendMode } from 'app/shared/constants/send-mode.enum';
import { TargetUserType } from 'app/shared/constants/target-user-type.enum';
import { DayRepetition } from '../constant/day-repetition.enum';
import { MonthRepetition } from '../constant/month-repetition.enum';
import { Recipients } from '../models/broadcast-messages.model';

export interface IUpdateBroadcastMessageRequest {
  broadcastMessageId?: string;
  title?: string;
  broadcastContent?: string;
  createdDate?: Date;
  lastUpdated?: Date;
  lastUpdatedBy?: string;
  recipients?: Recipients;
  validFromDate?: Date;
  validToDate?: Date;
  status?: string;
  targetUserType: TargetUserType;
  sendMode: SendMode;
  numberOfRecurrence?: number;
  recurrenceType: RecurrenceType;
  dayRepetitions?: DayRepetition[];
  monthRepetition?: MonthRepetition;
}

export class UpdateBroadcastMessageRequest
  implements IUpdateBroadcastMessageRequest {
  broadcastMessageId?: string;
  title?: string;
  broadcastContent?: string;
  createdDate?: Date;
  lastUpdated?: Date;
  lastUpdatedBy?: string;
  recipients?: Recipients;
  validFromDate?: Date;
  validToDate?: Date;
  status?: string;
  targetUserType: TargetUserType;
  sendMode: SendMode;
  numberOfRecurrence?: number;
  recurrenceType: RecurrenceType;
  dayRepetitions?: DayRepetition[];
  monthRepetition?: MonthRepetition;

  constructor(data?: Partial<IUpdateBroadcastMessageRequest>) {
    if (data == null) {
      return;
    }

    this.broadcastMessageId = data.broadcastMessageId;
    this.title = data.title;
    this.broadcastContent = data.broadcastContent;
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
