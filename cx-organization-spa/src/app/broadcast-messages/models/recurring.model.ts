import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { Validation } from './validation.model';

export interface IRecurringModel {
  type: RecurrenceType;
  fromDate: Date;
  toDate: Date;
  recurringStartDate: Date;
  recurringEndDate: Date;
  startAt: string;
  endAt: string;
}
export class RecurringModel implements IRecurringModel {
  type: RecurrenceType = RecurrenceType.None;
  fromDate: Date = new Date();
  toDate: Date = new Date();
  recurringStartDate: Date = new Date();
  recurringEndDate: Date = new Date();
  startAt: string;
  endAt: string;
  constructor(data?: Partial<IRecurringModel>) {
    if (!data) {
      return;
    }
    this.type = data.type;
    this.fromDate = data.fromDate;
    this.toDate = data.toDate;
    this.recurringStartDate = data.recurringStartDate;
    this.recurringEndDate = data.recurringEndDate;
    this.startAt = data.startAt;
    this.endAt = data.endAt;
  }
}

export interface IValidation {
  isValid(): boolean;
  resetError(): void;
}

// tslint:disable-next-line:max-classes-per-file
export class DateTimeValidation implements IValidation {
  month: Validation = new Validation();
  date: Validation = new Validation();
  time: Validation = new Validation();

  isValid(): boolean {
    return !(this.month.isError && this.date.isError && this.month.isError);
  }

  resetError(): void {
    this.month.isError = this.date.isError = this.time.isError = false;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class WeeklyValidation implements IValidation {
  weekday: Validation = new Validation();
  time: Validation = new Validation();

  isValid(): boolean {
    return !(this.weekday.isError || this.time.isError);
  }

  resetError(): void {
    this.weekday.isError = this.time.isError = false;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class RecurringDateValidation implements IValidation {
  date: Validation = new Validation();
  time: Validation = new Validation();
  inPast: Validation = new Validation();

  isValid(): boolean {
    return !(this.date.isError || this.time.isError || this.inPast.isError);
  }

  resetError(): void {
    this.date.isError = this.time.isError = this.inPast.isError = false;
  }
}
