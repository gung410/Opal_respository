export interface CourseReviewModelDTO {
  id?: string;
  userId: string;
  courseId: string;
  parentCommentId?: string | undefined;
  version?: string;
  sectionId?: string | undefined;
  lectureId?: string | undefined;
  itemName?: string | undefined;
  userFullName: string;
  commentTitle?: string | undefined;
  commentContent?: string | undefined;
  rate: number;
  createdDate?: Date;
  createdBy?: string;
  changedDate?: Date | undefined;
  changedBy?: string | undefined;
}

export class CourseReviewModel {
  public id: string;
  public userId: string;
  public courseId: string;
  public parentCommentId?: string | undefined;
  public version?: string | undefined;
  public sectionId?: string | undefined;
  public lectureId?: string | undefined;
  public itemName?: string | undefined;
  public userFullName: string;
  public commentTitle?: string | undefined;
  public commentContent?: string | undefined;
  public rate: number;
  public createdDate: Date;
  public createdBy?: string;
  public changedDate?: Date | undefined;
  public changedBy?: string | undefined;

  constructor(data?: CourseReviewModelDTO) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.courseId = data.courseId;
    this.parentCommentId = data.parentCommentId;
    this.version = data.version;
    this.sectionId = data.sectionId;
    this.lectureId = data.lectureId;
    this.itemName = data.itemName;
    this.userFullName = data.userFullName;
    this.commentTitle = data.commentTitle;
    this.commentContent = data.commentContent;
    this.rate = data.rate;
    this.createdDate = new Date(data.createdDate);
    this.createdBy = data.createdBy;
    this.changedDate =
      data.changedDate !== undefined ? new Date(data.changedDate) : undefined;
    this.changedBy = data.changedBy;
  }
}

export class CourseReviewManagementModel {
  public items: CourseReviewModel[];
  public rating?: number;
  public totalCount?: number;

  constructor(
    data?: CourseReviewModel[],
    rating?: number,
    totalCount?: number
  ) {
    if (data == null) {
      return;
    }
    this.items = data;
    this.rating = rating ? rating : 0;
    this.totalCount = totalCount ? totalCount : 0;
  }
}

export class ReviewPagingResponse {
  public rating?: number;
  public totalCount?: number;
  constructor(data?: Partial<ReviewPagingResponse>) {
    if (!data) {
      return;
    }
    this.rating = data.rating ? data.rating : 0;
    this.totalCount = data.totalCount ? data.totalCount : 0;
  }
}

export class ReviewPagingResponseModel<T> extends ReviewPagingResponse {
  public items?: T[];
  constructor(data?: Partial<ReviewPagingResponseModel<T>>) {
    if (!data) {
      return;
    }
    super(data);
    this.items = data.items;
  }
}
