import { INewsfeedBaseModel, NewsfeedType } from './newsfeed.model';

export interface IUserPostFeedModel extends INewsfeedBaseModel {
  userId: string;
  postId: string;
  postContent: string;
  postedBy: string;
  createdDate: Date;
  changedDate: Date;
  postForward?: IUserPostFeedModel;
  url: string;
}

export class UserPostFeedModel implements IUserPostFeedModel {
  public id: string;
  public type: NewsfeedType;
  public userId: string;
  public postId: string;
  public postContent: string;
  public postedBy: string;
  public createdDate: Date;
  public changedDate: Date;
  public postForward?: UserPostFeedModel;
  public url: string;
  constructor(data?: IUserPostFeedModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.type = data.type;
    this.userId = data.userId;
    this.postId = data.postId;
    this.postContent = data.postContent;
    this.postedBy = data.postedBy;
    this.createdDate = new Date(data.createdDate);
    this.changedDate = data.changedDate ? new Date(data.changedDate) : undefined;
    this.url = data.url;
    this.postForward = this.createPostForward(data.postForward);
  }

  private createPostForward(postForward: IUserPostFeedModel): UserPostFeedModel {
    if (postForward == null) {
      return;
    }
    switch (postForward.type) {
      case NewsfeedType.CommunityPost:
        return new CommunityFeedModel(postForward as ICommunityFeedModel);
      case NewsfeedType.Post:
      default:
        return new UserPostFeedModel(postForward);
    }
  }
}

export interface ICommunityFeedModel extends IUserPostFeedModel {
  communityName: string;
  communityThumbnailUrl: string;
  description: string;
}

export class CommunityFeedModel extends UserPostFeedModel implements ICommunityFeedModel {
  public id: string;
  public type: NewsfeedType;
  public userId: string;
  public postId: string;
  public postContent: string;
  public postedBy: string;
  public communityName: string;
  public communityThumbnailUrl: string;
  public createdDate: Date;
  public changedDate: Date;
  public description: string;
  constructor(data?: ICommunityFeedModel) {
    super(data);
    if (data == null) {
      return;
    }
    this.communityName = data.communityName;
    this.communityThumbnailUrl = data.communityThumbnailUrl;
    this.description = data.description;
  }
}
