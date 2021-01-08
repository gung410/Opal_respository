import { PublicUserInfo, UserInfoModel } from '../../share/models/user-info.model';

import { UserReviewItemType } from '../../content/models/user-review-item-type.enum';

export interface IUserReviewModel {
  id?: string;
  userId: string;
  itemId: string;
  classRunId?: string;
  itemType: UserReviewItemType;
  parentCommentId?: string | undefined;
  version?: string;
  itemName?: string | undefined;
  userFullName?: string;
  commentTitle?: string | undefined;
  commentContent?: string | undefined;
  rate?: number;
  createdDate?: Date;
  createdBy?: string;
  changedDate?: Date | undefined;
  changedBy?: string | undefined;
}

export class UserReviewModel implements IUserReviewModel {
  public id: string;
  public userId: string;
  public itemId: string;
  public classRunId?: string;
  public itemType: UserReviewItemType;
  public parentCommentId?: string | undefined;
  public version?: string | undefined;
  public itemName?: string | undefined;
  public userFullName?: string;
  public commentTitle?: string | undefined;
  public commentContent?: string | undefined;
  public rate?: number;
  public createdDate: Date;
  public createdBy?: string;
  public changedDate?: Date | undefined;
  public changedBy?: string | undefined;
  public user: PublicUserInfo;

  constructor(data?: IUserReviewModel) {
    if (data == null || data === null) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.itemId = data.itemId;
    this.classRunId = data.classRunId;
    this.itemType = data.itemType;
    this.parentCommentId = data.parentCommentId;
    this.version = data.version;
    this.itemName = data.itemName;
    this.userFullName = data.userFullName;
    this.commentTitle = data.commentTitle;
    this.commentContent = data.commentContent;
    this.rate = data.rate;
    this.createdDate = new Date(data.createdDate);
    this.createdBy = data.createdBy;
    this.changedDate = data.changedDate !== undefined ? new Date(data.changedDate) : undefined;
    this.changedBy = data.changedBy;
  }

  public get isMine(): boolean {
    return this.userId === UserInfoModel.getMyUserInfo().extId;
  }
}
