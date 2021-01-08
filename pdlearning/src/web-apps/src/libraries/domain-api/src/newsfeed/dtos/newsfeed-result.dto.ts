import { CommunityFeedModel, ICommunityFeedModel, IUserPostFeedModel, UserPostFeedModel } from '../models/user-post-feed.model';
import { CourseFeedModel, ICourseFeedModel } from '../models/course-feed.model';
import { INewsfeed, Newsfeed, NewsfeedType } from '../models/newsfeed.model';

import { IPagedResultDto } from '../../share/dtos/paged-result.dto';

export interface INewsfeedResult extends IPagedResultDto<INewsfeed> {}

export class NewsfeedResult implements INewsfeedResult {
  public totalCount: number = 0;
  public items: Newsfeed[] = [];
  constructor(data?: INewsfeedResult) {
    if (data == null) {
      return;
    }
    this.totalCount = data.totalCount ? data.totalCount : 0;
    this.items = data.items ? data.items.map(_ => this.createNewsfeed(_)) : [];
  }

  private createNewsfeed(dto: INewsfeed): Newsfeed {
    switch (dto.type) {
      case NewsfeedType.Course:
        return new CourseFeedModel(dto as ICourseFeedModel);
      case NewsfeedType.Post:
        return new UserPostFeedModel(dto as IUserPostFeedModel);
      case NewsfeedType.CommunityPost:
        return new CommunityFeedModel(dto as ICommunityFeedModel);
    }
  }
}
