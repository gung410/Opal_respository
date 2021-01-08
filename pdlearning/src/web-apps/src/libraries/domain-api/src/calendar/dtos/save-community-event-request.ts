import { EventRepeatFrequency } from '../enums/event-repeat-frequency';

export class SaveCommunityEventRequest {
  public communityId: string;
  public id: string;
  public calendarEventSource: string;
  public title: string;
  public description?: string;
  public startAt: Date;
  public endAt: Date;
  public isAllDay: boolean;
  public communityEventPrivacy: string;
  public repeatFrequency?: EventRepeatFrequency;
  public repeatUntil?: Date;
}
