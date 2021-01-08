import { DateUtils } from '@opal20/infrastructure';
import { EventRepeatFrequency } from '../enums/event-repeat-frequency';

export interface IPersonalEventModel {
  id: string;
  title: string;
  createdBy: string;
  source: string;
  sourceId: string;
  createdAt: Date;
  startAt: Date;
  endAt: Date;
  isAllDay: boolean;
  description: string;
  type: string;
  repeatFrequency: string;
  repeatUntil: Date;
  eventId: string;
  recurrenceRule: string;
}

export class PersonalEventModel implements IPersonalEventModel {
  public id: string;
  public title: string;
  public createdBy: string;
  public createdAt: Date;
  public source: string;
  public sourceId: string;
  public startAt: Date;
  public endAt: Date;
  public isAllDay: boolean;
  public startTimezone?: string;
  public endTimezone?: string;
  public description: string;
  public type: string;
  public repeatFrequency: string;
  public repeatUntil: Date;
  public eventId: string;
  public recurrenceRule: string;

  /*
   * Count repeat days from start date until repeat end date.
   */
  private get repeatCount(): number {
    return this.repeatUntil ? DateUtils.countDay(this.startAt, this.repeatUntil) + 1 : null;
  }
  private get isDailyEvent(): boolean {
    return this.repeatFrequency === EventRepeatFrequency.Daily;
  }

  constructor(data: IPersonalEventModel) {
    this.id = data.id;
    this.eventId = data.id;
    this.title = data.title;
    this.createdBy = data.createdBy;
    this.createdAt = new Date(data.createdAt);
    this.source = data.source;
    this.sourceId = data.sourceId;
    this.startAt = new Date(data.startAt);
    this.endAt = data.isAllDay ? DateUtils.setTimeToEndInDay(new Date(data.endAt)) : new Date(data.endAt);
    this.isAllDay = data.isAllDay;
    this.startTimezone = null;
    this.endTimezone = null;
    this.description = data.description;
    this.type = data.type;
    this.repeatFrequency = data.repeatFrequency;
    this.repeatUntil = data.repeatUntil ? new Date(data.repeatUntil) : null;
    this.recurrenceRule = this.getRecurrenceRule();
  }
  private getRecurrenceRule(): string {
    return this.isDailyEvent ? `FREQ=${this.repeatFrequency.toUpperCase()};COUNT=${this.repeatCount};` : null;
  }
}
