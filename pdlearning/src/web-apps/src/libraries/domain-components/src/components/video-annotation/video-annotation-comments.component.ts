import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import {
  IVideoCommentCreateRequest,
  IVideoCommentSearchRequest,
  IVideoCommentUpdateRequest,
  PublicUserInfo,
  UserInfoModel,
  UserRepository,
  VideoComment,
  VideoCommentApiService,
  VideoCommentOrderBy,
  VideoCommentSourceType
} from '@opal20/domain-api';
import { VideoAnnotationCommentInfo, VideoAnnotationMode } from '../../view-models/video-annotation-view.model';

const FIRST_PAGE_SIZE = 2;
const PAGE_SIZE = 10;
type SortingType = { displayText: string; value: string };
@Component({
  selector: 'video-annotation-comments',
  templateUrl: './video-annotation-comments.component.html'
})
export class VideoAnnotationCommentsComponent extends BaseComponent {
  @ViewChild('commentList', { static: false })
  public commentList: ElementRef;

  @Input() public mode: VideoAnnotationMode = VideoAnnotationMode.Learn;

  @Input() public videoId: string;
  @Input() public set videoDuration(v: number) {
    if (Utils.isDifferent(v, this._videoDuration)) {
      this._videoDuration = v;
      this.updateLimitTimeRangeOfView();
    }
  }
  public get videoDuration(): number {
    return this._videoDuration;
  }

  @Input() public currentVideoTime: number;
  @Input() public commentInfo: VideoAnnotationCommentInfo;

  @Output() public timestampClick: EventEmitter<number> = new EventEmitter<number>();

  public sortingList: SortingType[] = [
    { displayText: 'Comment Time', value: VideoCommentOrderBy.CreatedDate },
    { displayText: 'Video Time', value: VideoCommentOrderBy.VideoTime }
  ];

  public comments: VideoComment[] = [];
  public totalCount: number = 0;
  public userDict: Dictionary<PublicUserInfo> = {};
  public currentUserId: string;

  public editingComment: VideoComment;
  public editingCommentIndex: number;

  public videoMinLengthAsDate: Date;
  public videoMaxLengthAsDate: Date;

  private _videoDuration: number;
  private _editingTimestamp: Date = new Date();
  private request: IVideoCommentSearchRequest;
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private videoCommentApiService: VideoCommentApiService,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.currentUserId = UserInfoModel.getMyUserInfo().id;
    this.initRequest();
    this.initComments();
  }

  public set editingTimestamp(value: Date) {
    if (value === this._editingTimestamp) {
      return;
    }
    const validTime = this.getValidTime(value);
    if (value !== validTime) {
      this.editingTimestamp = validTime;
      return;
    }
    this._editingTimestamp = validTime;
    this.editingComment.videoTime = this._editingTimestamp.getTime() / 1000;
  }

  public get editingTimestamp(): Date {
    return this._editingTimestamp;
  }

  public set currentSorting(v: VideoCommentOrderBy) {
    if (this.request.orderBy !== v) {
      this.request.orderBy = v;
      (this.request.orderType = this.getOrderTypeByOrderBy(this.request.orderBy)), this.initComments();
    }
  }

  public get currentSorting(): VideoCommentOrderBy {
    return this.request.orderBy;
  }

  public onVideoTimestampClicked(comment: VideoComment): void {
    this.timestampClick.emit(comment.videoTime);
  }

  public onViewOlderCommentsClicked(): void {
    this.loadMore();
  }

  public onSubmit(textbox: HTMLTextAreaElement): void {
    this.createComment(textbox.value).then(() => {
      textbox.value = '';
    });
  }

  public onEditClicked(comment: VideoComment, index: number): void {
    this.editingComment = Utils.cloneDeep(comment);
    this.editingCommentIndex = index;
    this.editingTimestamp.setTime(this.editingComment.videoTime * 1000);
    const isLastItem = index + 1 === this.comments.length;
    if (isLastItem) {
      // to focus the whole comment
      this.scrollToEnd();
    }
  }

  public onDeleteClicked(): void {
    this.videoCommentApiService.deleteVideoComment(this.editingComment.id).then(() => {
      this.comments.splice(this.editingCommentIndex, 1);
      this.resetEditing();
    });
  }

  public onCancelClicked(): void {
    this.resetEditing();
  }

  public onSaveClicked(): void {
    this.updateComment().then(() => {
      this.resetEditing();
    });
  }

  public isMine(comment: VideoComment): boolean {
    return comment.userId === this.currentUserId;
  }

  public get isLearnMode(): boolean {
    return this.mode === VideoAnnotationMode.Learn;
  }

  public isTextEmpty(value: string): boolean {
    return value.trim().length === 0;
  }

  private resetEditing(): void {
    this.editingCommentIndex = null;
    this.editingComment = null;
  }

  private loadMore(): void {
    this.request.skipCount += this.request.maxResultCount;
    this.request.maxResultCount = PAGE_SIZE;
    this.videoCommentApiService.getVideoComments(this.request).then(result => {
      result.items.forEach(comment => {
        const isExisted = this.comments.findIndex(p => p.id === comment.id) > -1;
        if (!isExisted) {
          this.comments.push(comment);
        }
      });
      this.sortComments();
      this.getUsers();
    });
  }

  private initComments(): void {
    this.request.skipCount = 0;
    this.request.maxResultCount = FIRST_PAGE_SIZE;
    this.videoCommentApiService.getVideoComments(this.request).then(result => {
      this.totalCount = result.totalCount;
      this.comments = result.items;
      this.sortComments();
      this.getUsers();
      this.scrollToEnd();
    });
  }

  private scrollToEnd(): void {
    setTimeout(() => {
      this.commentList.nativeElement.scrollTop = this.commentList.nativeElement.scrollHeight;
    });
  }

  private createComment(content: string): Promise<void> {
    const request: IVideoCommentCreateRequest = {
      content: content,
      videoId: this.videoId,
      videoTime: this.currentVideoTime,
      objectId: this.commentInfo && this.commentInfo.objectId ? this.commentInfo.objectId : null,
      originalObjectId: this.commentInfo && this.commentInfo.originalObjectId ? this.commentInfo.originalObjectId : null,
      sourceType: this.commentInfo ? this.commentInfo.sourceType : VideoCommentSourceType.CCPM
    };
    return this.videoCommentApiService.createVideoComment(request).then(p => {
      this.comments.push(p);
      this.scrollToEnd();
    });
  }

  private updateComment(): Promise<void> {
    const request: IVideoCommentUpdateRequest = {
      id: this.comments[this.editingCommentIndex].id,
      content: this.editingComment.content,
      videoTime: this.editingComment.videoTime
    };
    return this.videoCommentApiService.updateVideoComment(request).then(result => {
      this.comments[this.editingCommentIndex] = result;
    });
  }

  private getUsers(): void {
    const userIds = [...this.comments.map(p => p.userId), this.currentUserId];
    const missingUserIds = userIds.filter(userId => this.userDict[userId] == null);
    if (!missingUserIds.length) {
      return;
    }
    this.userRepository
      .loadPublicUserInfoList({ userIds: Utils.uniq(missingUserIds) })
      .pipe(this.untilDestroy())
      .subscribe(newUsers => {
        const newUserDict = Utils.toDictionary(newUsers, _ => _.extId);
        this.userDict = { ...this.userDict, ...newUserDict };
      });
  }

  /**
   * @returns the value of time range if the time is invalid. Otherwise return itself.
   */
  private getValidTime(time: Date): Date {
    const dateTemp = new Date();
    if (time > this.videoMaxLengthAsDate) {
      dateTemp.setTime(this.videoMaxLengthAsDate.getTime());
      return dateTemp;
    }
    if (time < this.videoMinLengthAsDate) {
      dateTemp.setTime(this.videoMinLengthAsDate.getTime());
      return dateTemp;
    }
    return time;
  }

  private updateLimitTimeRangeOfView(): void {
    const minDate = new Date();
    minDate.setTime(0);
    this.videoMinLengthAsDate = minDate;

    const maxDate = new Date();
    maxDate.setTime(this.videoDuration * 1000);
    this.videoMaxLengthAsDate = maxDate;
  }

  private initRequest(): void {
    this.request = {
      objectId: this.commentInfo && this.commentInfo.objectId ? this.commentInfo.objectId : null,
      originalObjectId: this.commentInfo && this.commentInfo.originalObjectId ? this.commentInfo.originalObjectId : null,
      sourceType: this.commentInfo ? this.commentInfo.sourceType : VideoCommentSourceType.CCPM,
      videoId: this.videoId,
      skipCount: 0,
      maxResultCount: FIRST_PAGE_SIZE,
      orderType: this.getOrderTypeByOrderBy(VideoCommentOrderBy.CreatedDate),
      orderBy: VideoCommentOrderBy.CreatedDate
    };
  }

  private sortComments(): void {
    switch (this.request.orderBy) {
      case VideoCommentOrderBy.CreatedDate:
        this.comments.sort((c1, c2) => c1.createdDate.getTime() - c2.createdDate.getTime());
        break;
      case VideoCommentOrderBy.VideoTime:
        this.comments.sort((c1, c2) => c2.videoTime - c1.videoTime);
        break;
    }
  }

  private getOrderTypeByOrderBy(orderBy: VideoCommentOrderBy): 'ASC' | 'DESC' {
    return orderBy === VideoCommentOrderBy.VideoTime ? 'ASC' : 'DESC';
  }
}
