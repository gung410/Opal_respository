import { BookmarkInfoModel, IBookmarkInfoModel } from './bookmark-info.model';
import { IMyDigitalContentInfo, MyDigitalContentInfo } from './my-digital-content-info.model';

export interface IMyDigitalContent {
  /**
   * Original Object Id
   */
  digitalContentId: string;
  rating: number;
  reviewsCount: number;
  bookmarkInfo?: IBookmarkInfoModel | undefined;
  myDigitalContent?: IMyDigitalContentInfo | undefined;
  viewsCount: number;
  downloadsCount: number;
}

export class MyDigitalContent implements IMyDigitalContent {
  /**
   * Original Object Id
   */
  public digitalContentId: string;
  public rating: number;
  public reviewsCount: number;
  public bookmarkInfo?: BookmarkInfoModel;
  public myDigitalContent?: MyDigitalContentInfo;
  public viewsCount: number;
  public downloadsCount: number;
  constructor(data?: IMyDigitalContent) {
    if (!data) {
      return;
    }
    this.digitalContentId = data.digitalContentId;
    this.rating = data.rating;
    this.reviewsCount = data.reviewsCount;
    this.bookmarkInfo = data.bookmarkInfo !== undefined ? new BookmarkInfoModel(data.bookmarkInfo) : undefined;
    this.myDigitalContent = data.myDigitalContent !== undefined ? new MyDigitalContentInfo(data.myDigitalContent) : undefined;
    this.viewsCount = data.viewsCount;
    this.downloadsCount = data.downloadsCount;
  }
}
