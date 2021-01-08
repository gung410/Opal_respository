export interface IDigitalBadgeModel {
  itemId: string;
  itemType: string;
  title: string;
  sharedByUsers: string[];
  thumbnailUrl: string;
  tagIds: string[];
  skipCount: number;
  maxResultCount: number;
  completedDate: Date;
}

export class DigitalBadgeModel implements IDigitalBadgeModel {
  public itemId: string;
  public itemType: string;
  public title: string;
  public sharedByUsers: string[];
  public tagIds: string[];
  public thumbnailUrl: string;
  public skipCount: number;
  public maxResultCount: number;
  public completedDate: Date;

  constructor(data?: IDigitalBadgeModel) {
    if (data) {
      this.itemId = data.itemId;
      this.itemType = data.itemType;
      this.title = data.title;
      this.sharedByUsers = data.sharedByUsers;
      this.thumbnailUrl = data.thumbnailUrl;
      this.tagIds = data.tagIds;
      this.skipCount = data.skipCount;
      this.maxResultCount = data.maxResultCount;
      this.completedDate = data.completedDate;
    }
  }
}
