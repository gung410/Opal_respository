import { Announcement, IAnnouncement } from '../models/announcement.model';

export interface ISearchAnnouncementResult {
  items: IAnnouncement[];
  totalCount: number;
}

export class SearchAnnouncementResult implements ISearchAnnouncementResult {
  public items: Announcement[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchAnnouncementResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new Announcement(item));
    this.totalCount = data.totalCount;
  }
}
