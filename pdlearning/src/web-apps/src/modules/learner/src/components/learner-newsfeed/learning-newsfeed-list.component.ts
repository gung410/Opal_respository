import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  CommunityFeedModel,
  CourseFeedModel,
  IGetNewsfeedRequest,
  Newsfeed,
  NewsfeedRepository,
  NewsfeedType,
  PublicUserInfo,
  UserPostFeedModel,
  UserRepository
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Observable, Subscription, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';

const pageSize = 10;
@Component({
  selector: 'learning-newsfeed-list',
  templateUrl: './learning-newsfeed-list.component.html'
})
export class LearningNewsFeedListComponent extends BaseComponent implements OnInit {
  @Input()
  public isShowMore: boolean = false;

  @Output()
  public newsFeedClick: EventEmitter<Newsfeed> = new EventEmitter<Newsfeed>();
  @Output()
  public loadMoreNewsFeedClick: EventEmitter<void> = new EventEmitter<void>();

  public newsfeedData$: Observable<Newsfeed[]>;
  public newsfeedDataSubscription: Subscription = new Subscription();
  public users$: Observable<Dictionary<PublicUserInfo>>;

  public totalNewsfeed: number;
  public newsfeedLength: number;

  public NewsfeedType = NewsfeedType;

  private userIds: string[] = [];
  private requestPayload: IGetNewsfeedRequest = { skipCount: 0, maxResultCount: pageSize };
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private newsfeedRepository: NewsfeedRepository,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
  }

  public trackByFn(index: number, item: Newsfeed): string {
    return item.id;
  }

  public onInit(): void {
    this.newsfeedData$ = this.newsfeedRepository.loadNewsFeedsLazy().pipe(
      tap(res => {
        this.fetchUsersFromNewsFeeds(res.items);
        this.totalNewsfeed = res.totalCount;
        this.newsfeedLength = res.items.length;
      }),
      map(res => res.items)
    );
    this.fetchData();
  }

  public onNewsFeedClicked(event: Newsfeed): void {
    this.newsFeedClick.emit(event);
  }

  public loadMoreClicked(): void {
    // this.requestPayload.skipCount += pageSize;
    // this.fetchData();
    this.loadMoreNewsFeedClick.emit();
  }

  public fetchData(): void {
    this.newsfeedDataSubscription.unsubscribe();
    this.newsfeedDataSubscription = this.newsfeedRepository.loadNewsFeeds(this.requestPayload).subscribe();
  }

  public get isEmpty(): boolean {
    return this.totalNewsfeed != null && this.totalNewsfeed === 0;
  }

  public get showLoadMore(): boolean {
    return this.totalNewsfeed > this.newsfeedLength;
  }

  private fetchUsersFromNewsFeeds(newsFeeds: Newsfeed[]): void {
    const userIds = this.getListOfUserIds(newsFeeds);
    if (!Utils.isDifferent(userIds.sort(), this.userIds.sort())) {
      return;
    }
    this.userIds = userIds;
    const users$: Observable<PublicUserInfo[]> = userIds.length ? this.userRepository.loadPublicUserInfoList({ userIds }, false) : of([]);
    this.users$ = users$.pipe(map(users => Utils.toDictionary(users, p => p.extId)));
  }

  private getListOfUserIds(newsFeeds: Newsfeed[]): string[] {
    const userIds = newsFeeds.map(newsfeed => {
      switch (newsfeed.constructor) {
        case CourseFeedModel:
          return (newsfeed as CourseFeedModel).userId;
        case CommunityFeedModel:
          return (newsfeed as CommunityFeedModel).postedBy;
        case UserPostFeedModel:
          return (newsfeed as UserPostFeedModel).postedBy;
      }
    });

    const ownerPostIds = newsFeeds.map(newsfeed => {
      switch (newsfeed.constructor) {
        case CommunityFeedModel:
          const communityPostFeed = newsfeed as CommunityFeedModel;
          return communityPostFeed.postForward && communityPostFeed.postForward.postedBy;
        case UserPostFeedModel:
          const userPostFeed = newsfeed as CommunityFeedModel;
          return userPostFeed.postForward && userPostFeed.postForward.postedBy;
      }
    });

    return Utils.uniq(userIds.concat(ownerPostIds));
  }
}
