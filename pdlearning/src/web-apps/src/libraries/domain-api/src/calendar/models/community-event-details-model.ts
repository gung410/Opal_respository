import { IPersonalEventModel, PersonalEventModel } from './personal-event-model';

import { CalendarCommunityEventPrivacy } from '../enums/calendar-community-event-privacy';

export interface ICommunityEventDetailsModel extends IPersonalEventModel {
  communityId: string;
  communityEventPrivacy: CalendarCommunityEventPrivacy;
}

export class CommunityEventDetailsModel extends PersonalEventModel implements ICommunityEventDetailsModel {
  public communityId: string;
  public communityEventPrivacy: CalendarCommunityEventPrivacy;
  public description: string;
  public id: string;
  public userId: string;
  public title: string;
  public createdBy: string;
  public createdAt: Date;
  public source: string;
  public sourceId: string;
  public startAt: Date;
  public endAt: Date;
  public isAllDay: boolean;

  constructor(data: ICommunityEventDetailsModel) {
    super(data);
    this.communityId = data.communityId;
    this.communityEventPrivacy = data.communityEventPrivacy;
  }
}
