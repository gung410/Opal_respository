import { CommunityFeedModel, ICommunityFeedModel, IUserPostFeedModel, UserPostFeedModel } from './user-post-feed.model';
import { CourseFeedModel, ICourseFeedModel } from './course-feed.model';

export interface INewsfeedBaseModel {
  id: string;
  type: NewsfeedType;
}

export enum NewsfeedType {
  Course = 'PdpmSuggestCourseFeed',
  Post = 'UserPostFeed',
  CommunityPost = 'CommunityPostFeed'
}

export enum NewsFeedUpdateInfo {
  Info = 'CourseInfoUpdated',
  Content = 'CourseContentUpdated',
  Suggested = 'CourseSuggestedToUser'
}

export type IPostNewsfeed = ICommunityFeedModel | IUserPostFeedModel;
export type PostNewsfeed = CommunityFeedModel | UserPostFeedModel;

export type INewsfeed = IPostNewsfeed | ICourseFeedModel;
export type Newsfeed = PostNewsfeed | CourseFeedModel;
