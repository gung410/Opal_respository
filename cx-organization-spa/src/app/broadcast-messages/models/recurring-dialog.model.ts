import { DAY_OF_WEEK } from 'app/shared/constants/broadcast-message-status.constant';
import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import * as moment from 'moment';

export interface IRecurringDialogModel {
  type: RecurrenceType;
  startWeekDay: string;
  endWeekDay: string;
  startDay: number;
  endDay: number;
  startMonth: number;
  endMonth: number;
  recurringStartDate: Date;
  recurringEndDate: Date;
  recurringStartTime: string;
  recurringEndTime: string;
  startAt: string;
  endAt: string;
}

export class RecurringDialogModel implements IRecurringDialogModel {
  type: RecurrenceType = RecurrenceType.None;
  startWeekDay: string;
  endWeekDay: string;
  startDay: number;
  endDay: number;
  startMonth: number;
  endMonth: number;
  recurringStartDate: Date;
  recurringEndDate: Date;
  recurringStartTime: string;
  recurringEndTime: string;
  startAt: string;
  endAt: string;
  constructor(data?: Partial<IRecurringDialogModel>) {
    if (!data) {
      return;
    }
    this.type = data.type;
    this.startWeekDay = data.startWeekDay;
    this.endWeekDay = data.endWeekDay;
    this.startDay = data.startDay;
    this.endDay = data.endDay;
    this.startMonth = data.startMonth;
    this.endMonth = data.endMonth;
    this.recurringStartDate = data.recurringStartDate;
    this.recurringEndDate = data.recurringEndDate;
    this.recurringStartTime = data.recurringStartTime;
    this.recurringEndTime = data.recurringEndTime;
    this.startAt = data.startAt;
    this.endAt = data.endAt;
  }
}
