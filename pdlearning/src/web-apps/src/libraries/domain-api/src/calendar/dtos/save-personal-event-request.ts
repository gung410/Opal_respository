import { EventRepeatFrequency } from '../enums/event-repeat-frequency';

export class SavePersonalEventRequest {
  public id: string;
  public title: string;
  public description?: string;
  public startAt: Date;
  public endAt: Date;
  public isAllDay: boolean;
  public repeatFrequency?: EventRepeatFrequency;
  public repeatUntil?: Date;
  public attendeeIds?: string[];
}
