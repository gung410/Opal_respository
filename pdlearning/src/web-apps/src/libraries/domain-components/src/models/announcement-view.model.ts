import { Announcement, IAnnouncement, PublicUserInfo } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface IAnnouncementViewModel extends IAnnouncement {
  selected: boolean;
  owner: PublicUserInfo;
}

// @dynamic
export class AnnouncementViewModel extends Announcement implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public owner: PublicUserInfo;
  public static createFromModel(
    announcement: Announcement,
    owner: PublicUserInfo,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {}
  ): AnnouncementViewModel {
    return new AnnouncementViewModel({
      ...announcement,
      owner: owner,
      selected: checkAll || selecteds[announcement.id]
    });
  }

  constructor(data?: IAnnouncementViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
      this.owner = data.owner;
    }
  }
}
