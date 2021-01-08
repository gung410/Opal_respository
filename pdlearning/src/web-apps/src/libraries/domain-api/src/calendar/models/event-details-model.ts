import { IPersonalEventModel, PersonalEventModel } from './personal-event-model';

// TODO: Rename to IPersonnalEventDetailsModel
export interface IEventDetailsModel extends IPersonalEventModel {
  attendeeIds: string[];
}

export class EventDetailsModel extends PersonalEventModel implements IEventDetailsModel {
  public description: string;
  public id: string;
  public title: string;
  public createdBy: string;
  public createdAt: Date;
  public source: string;
  public sourceId: string;
  public startAt: Date;
  public endAt: Date;
  public isAllDay: boolean;
  public attendeeIds: string[];

  constructor(data?: IEventDetailsModel) {
    if (data != null) {
      super(data);
      this.description = data.description;
      this.attendeeIds = data.attendeeIds;
    }
  }
}
