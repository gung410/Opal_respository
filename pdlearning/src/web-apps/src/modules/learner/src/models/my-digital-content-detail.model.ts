import {
  BookmarkInfoModel,
  DigitalContent,
  IDigitalContent,
  IMyDigitalContent,
  IPagedResultDto,
  MyDigitalContentInfo
} from '@opal20/domain-api';

export interface IMyDigitalContentDetail {
  digitalContentId: string;
  rating: number;
  reviewsCount: number;
  createdDate?: Date | undefined;
  bookmarkInfo?: BookmarkInfoModel;
  myDigitalContent?: MyDigitalContentInfo;
  digitalContent: DigitalContent;
  viewsCount: number;
  downloadsCount: number;
}

export class MyDigitalContentDetail implements IMyDigitalContentDetail {
  public digitalContentId: string;
  public rating: number;
  public reviewsCount: number;
  public createdDate?: Date | undefined;
  public bookmarkInfo?: BookmarkInfoModel;
  public myDigitalContent?: MyDigitalContentInfo;
  public digitalContent: DigitalContent;
  public viewsCount: number;
  public downloadsCount: number;
  public static createMyDigitalContentDetail(myDc: IMyDigitalContent, dc: IDigitalContent): MyDigitalContentDetail {
    return new MyDigitalContentDetail({
      digitalContentId: myDc !== undefined ? myDc.digitalContentId : dc.id,
      rating: myDc !== undefined ? myDc.rating : 0,
      reviewsCount: myDc !== undefined ? myDc.reviewsCount : 0,
      createdDate: dc !== undefined && dc.createdDate ? new Date(dc.createdDate) : undefined,
      bookmarkInfo: myDc !== undefined && myDc.bookmarkInfo !== undefined ? new BookmarkInfoModel(myDc.bookmarkInfo) : undefined,
      myDigitalContent:
        myDc !== undefined && myDc.myDigitalContent !== undefined ? new MyDigitalContentInfo(myDc.myDigitalContent) : undefined,
      digitalContent: dc !== undefined ? new DigitalContent(dc) : undefined,
      viewsCount: myDc.viewsCount,
      downloadsCount: myDc.downloadsCount
    });
  }
  constructor(data?: IMyDigitalContentDetail) {
    if (!data) {
      return;
    }
    this.createdDate = data.createdDate ? new Date(data.createdDate) : undefined;
    this.digitalContentId = data.digitalContentId;
    this.rating = data.rating;
    this.reviewsCount = data.reviewsCount;
    this.bookmarkInfo = data.bookmarkInfo !== undefined ? new BookmarkInfoModel(data.bookmarkInfo) : undefined;
    this.myDigitalContent = data.myDigitalContent !== undefined ? new MyDigitalContentInfo(data.myDigitalContent) : undefined;
    this.digitalContent = data.digitalContent !== undefined ? new DigitalContent(data.digitalContent) : undefined;
    this.viewsCount = data.viewsCount;
    this.downloadsCount = data.downloadsCount;
  }
}

export class PagedMyDigitalContentDetailResult implements IPagedResultDto<MyDigitalContentDetail> {
  public items: MyDigitalContentDetail[] = [];
  public totalCount: number = 0;
  constructor(data?: IPagedResultDto<MyDigitalContentDetail>) {
    if (!data) {
      return;
    }
    this.totalCount = data.totalCount;
    this.items = data.items ? data.items : [];
  }
}
