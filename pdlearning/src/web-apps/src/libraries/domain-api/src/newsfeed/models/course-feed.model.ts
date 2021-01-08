import { INewsfeedBaseModel, NewsFeedUpdateInfo, NewsfeedType } from './newsfeed.model';

export interface ICourseFeedModel extends INewsfeedBaseModel {
  courseId: string;
  userId: string;
  createdDate: Date;
  thumbnailUrl: string;
  courseName: string;
  updateInfo: NewsFeedUpdateInfo;
  url: string;
}

export class CourseFeedModel implements ICourseFeedModel {
  public id: string;
  public type: NewsfeedType;
  public courseId: string;
  public userId: string;
  public createdDate: Date;
  public thumbnailUrl: string;
  public courseName: string;
  public updateInfo: NewsFeedUpdateInfo;
  public url: string;
  constructor(data?: ICourseFeedModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.type = data.type;
    this.courseId = data.courseId;
    this.userId = data.userId;
    this.createdDate = new Date(data.createdDate);
    this.thumbnailUrl = data.thumbnailUrl;
    this.courseName = data.courseName;
    this.updateInfo = data.updateInfo;
    this.url = data.url;
  }
}
