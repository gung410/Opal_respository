import { IUserAccessSharingModel, UserAccessSharingModel } from './user-access-sharing-model';

export interface ICalendarAccessSharingsResult {
  totalCount: number;
  items: IUserAccessSharingModel[];
}

export class CalendarAccessSharingsResult implements ICalendarAccessSharingsResult {
  public totalCount: number;
  public items: IUserAccessSharingModel[];

  constructor(data: ICalendarAccessSharingsResult) {
    this.totalCount = data.totalCount;
    this.items = data.items.map(p => new UserAccessSharingModel(p));
  }
}
