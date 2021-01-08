export interface ITrackingModel {
  itemId: string;
  itemType: string;
  isLike: boolean;
  totalLike: number;
  totalShare: number;
  totalView: number;
  totalDownload: number;
}

export class TrackingModel implements ITrackingModel {
  public itemId: string;
  public itemType: string;
  public isLike: boolean;
  public totalLike: number;
  public totalShare: number;
  public totalView: number;
  public totalDownload: number;
  constructor(data?: ITrackingModel) {
    if (data) {
      this.itemId = data.itemId;
    }
    this.itemType = data.itemType;
    this.isLike = data.isLike;
    this.totalLike = data.totalLike;
    this.totalShare = data.totalShare;
    this.totalView = data.totalView;
    this.totalDownload = data.totalDownload;
  }
}
