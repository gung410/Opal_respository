import { SharingType } from '../dtos/user-sharing-request.dto';

export interface IUserSharing {
  id: string;
  itemId: string;
  itemType: SharingType;
  createdBy: string;
  createdDate: Date;
  users: IUserSharingDetail[];
}

export interface IUserSharingDetail {
  id?: string;
  userId: string;
  userSharingId: string;
  isFollowing?: boolean;
}

export class UserSharing implements IUserSharing {
  public id: string;
  public itemId: string;
  public itemType: SharingType;
  public createdBy: string;
  public createdDate: Date;
  public users: UserSharingDetail[];
  constructor(data?: IUserSharing) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.itemId = data.itemId;
    this.itemType = data.itemType;
    this.createdBy = data.createdBy;
    this.createdDate = data.createdDate ? new Date(data.createdDate) : new Date();
    this.users = data.users ? data.users.map(c => new UserSharingDetail(c)) : [];
  }
}

export class UserSharingDetail implements IUserSharingDetail {
  public id: string;
  public userId: string;
  public userSharingId: string;
  public isFollowing?: boolean;
  constructor(data?: IUserSharingDetail) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.userSharingId = data.userSharingId;
    this.isFollowing = data.isFollowing;
  }
}
