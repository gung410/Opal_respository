import { BaseComponent, LocalTranslatorService, ModuleFacadeService } from '@opal20/infrastructure';
import { CommunityFeedModel, NewsfeedType, PublicUserInfo, UserPostFeedModel } from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'learning-newsfeed',
  templateUrl: './learning-newsfeed.component.html'
})
export class LearningNewsFeedComponent extends BaseComponent {
  @Input()
  public newsfeed: CommunityFeedModel | UserPostFeedModel;

  @Input()
  public users: Dictionary<PublicUserInfo> = {};

  @Output()
  public newsFeedClick: EventEmitter<CommunityFeedModel | UserPostFeedModel> = new EventEmitter<CommunityFeedModel | UserPostFeedModel>();

  public isReadMoreEnabled: boolean = false;

  constructor(public translator: LocalTranslatorService, protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onNewsFeedClicked(): void {
    this.newsFeedClick.emit(this.newsfeed);
  }

  public onReadMoreClicked(): void {
    this.isReadMoreEnabled = true;
  }

  public onPostClicked(): void {
    // Navigate to the community wall if the post type is CommunityPost
    if (this.newsfeed.type === NewsfeedType.CommunityPost) {
      window.location.href = this.newsfeed.url;
    } else {
      // Navigate to the current user's dashboard in CSL
      window.location.href = CSL_USER_DASHBOARD_URL;
    }
  }

  public get showWallInfo(): boolean {
    return this.newsfeed.constructor === CommunityFeedModel;
  }

  public get wallName(): string {
    return this.wallInfo.name;
  }

  public get wallThumbnailUrl(): string {
    return this.wallInfo.thumbnailUrl;
  }

  public get wallDescription(): string {
    return this.wallInfo.description;
  }

  public get postedByName(): string {
    const user = this.users && this.users[this.newsfeed.postedBy];
    return user ? user.fullName : 'Anonymous';
  }

  public get postedUserAvatar(): string {
    const user = this.users && this.users[this.newsfeed.postedBy];
    return user && user.avatarUrl ? user.avatarUrl : DEFAULT_USER_AVATAR;
  }

  private get wallInfo(): WallInfo {
    switch (this.newsfeed.constructor) {
      case CommunityFeedModel:
        const communityModel = this.newsfeed as CommunityFeedModel;
        return communityModel
          ? {
              name: communityModel.communityName,
              thumbnailUrl: communityModel.communityThumbnailUrl ? communityModel.communityThumbnailUrl : DEFAULT_COMMUNITY_AVATAR,
              description: communityModel.description
            }
          : {
              name: 'Anonymous Group',
              thumbnailUrl: DEFAULT_COMMUNITY_AVATAR
            };
      case UserPostFeedModel:
        const userId = (this.newsfeed as UserPostFeedModel).userId;
        const user = this.users && this.users[userId];
        return user
          ? {
              name: user.fullName,
              thumbnailUrl: user.avatarUrl ? user.avatarUrl : DEFAULT_USER_AVATAR
            }
          : {
              name: 'Anonymous',
              thumbnailUrl: DEFAULT_USER_AVATAR
            };
    }
  }
}

const DEFAULT_COMMUNITY_AVATAR = 'assets/images/course/img-community-default.svg';
const DEFAULT_USER_AVATAR = 'assets/images/others/default-avatar.png';
const CSL_USER_DASHBOARD_URL = '/csl/dashboard';

type WallInfo = { name: string; thumbnailUrl: string; description?: string };
