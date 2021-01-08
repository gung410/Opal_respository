import { BaseComponent, ClipboardUtil, DomUtils, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BookmarkType,
  MyDigitalContentApiService,
  MyDigitalContentStatus,
  PublicUserInfo,
  UserInfoModel,
  UserReviewItemType
} from '@opal20/domain-api';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { LEARNER_PERMISSIONS, LearningPathSharingDialogComponent } from '@opal20/domain-components';
import { Observable, fromEvent, interval } from 'rxjs';
import { debounce, tap } from 'rxjs/operators';

import { Align } from '@progress/kendo-angular-popup';
import { ClassRunDataService } from '../services/class-run-data.service';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { DigitalContentDataService } from '../services/digital-content.service';
import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { ILearningFeedbacksConfiguration } from '../models/learning-feedbacks.model';
import { LearnerDetailMenu } from '../constants/learner-detail-menu';
import { LearningActionService } from '../services/learning-action.service';
import { LearningFeedbacksConfig } from '../constants/learning-feedbacks-configs';
import { LearningTrackingEventPayload } from '../user-activities-tracking/user-tracking.models';
import { LearningType } from '../models/learning-item.model';
import { MyDigitalContentDetail } from '../models/my-digital-content-detail.model';
import { MyLearningPathDataService } from '../services/my-learning-path-data.service';
import { OpalDialogService } from '@opal20/common-components';
import { TrackingSourceService } from '../user-activities-tracking/tracking-souce.service';
import { UserTrackingService } from '../user-activities-tracking/user-tracking.service';

const itemsPerPage: number = 3;
export class SubMenuType {
  public text: string;
  public section: ElementRef;
}
@Component({
  selector: 'learner-digital-content-detail',
  templateUrl: './learner-digital-content-detail.component.html',
  providers: [ClassRunDataService]
})
export class LearnerDigitalContentDetailComponent extends BaseComponent implements OnInit {
  @ViewChild('informationSection', { static: false })
  public set informationSectionElement(v: ElementRef) {
    this._informationSectionElement = v;
    this.initSection();
  }

  public get informationSectionElement(): ElementRef {
    return this._informationSectionElement;
  }

  @ViewChild('reviewSection', { static: false })
  public set reviewSectionElement(v: ElementRef) {
    this._reviewSectionElement = v;
    this.initSection();
  }

  public get reviewSectionElement(): ElementRef {
    return this._reviewSectionElement;
  }

  @Input()
  public digitalContentId: string | undefined;
  @Input()
  public canContinueTask: boolean = false;
  @Input()
  public canStartTask: boolean = false;

  @Output()
  public backClick: EventEmitter<void> = new EventEmitter<void>();

  public itemDetail: MyDigitalContentDetail;
  public originalId: string;
  public showLecturePlayer: boolean = false;
  public learningItem: DigitalContentItemModel;
  public currentActiveSectionNumber: number = 1;
  public scrollableParent: HTMLElement;
  public selectedLectureId: string | undefined;
  public MyDigitalContentStatus: typeof MyDigitalContentStatus = MyDigitalContentStatus;

  public reviewType: UserReviewItemType = UserReviewItemType.DigitalContent;
  public reviewConfig: ILearningFeedbacksConfiguration = LearningFeedbacksConfig.digitalContent;

  public fileType: string;
  public showMoreInfo: boolean = false;
  public selectSection: boolean = false;
  public visibleSections: SubMenuType[];
  public firstSection: string;
  public sharedUsers: PublicUserInfo[] = [];

  // Download / Like / Share / View
  public isLike: boolean = false;
  public totalDownload: number = 0;
  public totalLike: number = 0;
  public totalShare: number = 0;
  public totalView: number = 0;
  public popupAlign: Align = { horizontal: 'right', vertical: 'top' };
  public anchorAlign: Align = { horizontal: 'right', vertical: 'bottom' };

  private enableUserTracking: boolean = true;
  private sections: SubMenuType[];
  private _informationSectionElement: ElementRef;
  private _reviewSectionElement: ElementRef;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private digitalContentDataService: DigitalContentDataService,
    private myDigitalContentApiService: MyDigitalContentApiService,
    private learningActionService: LearningActionService,
    private elementRef: ElementRef,
    private opalDialogService: OpalDialogService,
    private myLearningPathDataService: MyLearningPathDataService,
    private trackingSourceSrv: TrackingSourceService,
    private userTrackingService: UserTrackingService
  ) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    super.ngOnInit();
    if (this.digitalContentId !== undefined) {
      this.loadDigitalContentDetails().subscribe();
      this.listenBookmarkChanged();
    }
  }

  public onBackClick(): void {
    this.backClick.emit();
  }

  public onAfterViewInit(): void {
    this.scrollableParent = DomUtils.findClosestVerticalScrollableParent(this.elementRef.nativeElement);
    if (this.scrollableParent === undefined) {
      return;
    }
    const parentScrollEvent: EventTarget = this.scrollableParent;

    fromEvent(parentScrollEvent, 'scroll')
      .pipe(
        this.untilDestroy(),
        debounce(() => interval(50))
      )
      .subscribe(() => this.onScroll());
  }

  public onSelectSection(mode: string): void {
    this.visibleSections.forEach((value, index) => {
      if (mode === value.text) {
        this.firstSection = mode;
        this.scrollTo(value.section.nativeElement, index + 1);
        return;
      }
    });
  }

  public initSection(): void {
    const sections = [
      { text: LearnerDetailMenu.Information, section: this.informationSectionElement },
      { text: LearnerDetailMenu.Review, section: this.reviewSectionElement }
    ];
    if (Utils.isDifferent(sections, this.sections)) {
      this.sections = sections;
      // Recreate sections to fix this issue: https://cxtech.atlassian.net/browse/OPX-3406
      this.visibleSections = this.sections.filter(p => p.section && p.section.nativeElement && !p.section.nativeElement.hidden);
      if (this.visibleSections && this.visibleSections.length > 0) {
        this.firstSection = this.visibleSections[0].text;
      }
    }
  }

  public onScroll(): void {
    if (this.selectSection) {
      this.selectSection = false;
      return;
    }

    if (this.scrollableParent.scrollTop === 0) {
      this.currentActiveSectionNumber = 1;
      return;
    }

    if (this.scrollableParent.scrollTop + this.scrollableParent.clientHeight > this.scrollableParent.scrollHeight - 50) {
      this.currentActiveSectionNumber = this.visibleSections.length;
      return;
    }

    const currentParentScrollPosition = this.scrollableParent.scrollTop;
    let currentActiveSection: number = 0;
    this.visibleSections.forEach((p, i) => {
      if (p.section !== undefined && p.section.nativeElement.offsetTop - 350 <= currentParentScrollPosition) {
        currentActiveSection = i + 1;
      }
    });
    this.firstSection = this.visibleSections[currentActiveSection - 1].text;
    this.currentActiveSectionNumber = currentActiveSection;
  }

  public scrollTo(el: HTMLElement, sectionNumber: number): void {
    if (el === undefined || this.scrollableParent === undefined) {
      return;
    }
    this.selectSection = true;
    this.scrollableParent.scrollTop = el.offsetTop - 300;
    setTimeout(() => (this.currentActiveSectionNumber = sectionNumber), 55);
  }

  public changeBookmark(): void {
    if (this.itemDetail === undefined) {
      return;
    }

    if (this.itemDetail.bookmarkInfo === undefined) {
      this.learningActionService.bookmark(this.originalId, BookmarkType.DigitalContent);
    } else {
      this.learningActionService.unBookmark(this.originalId, BookmarkType.DigitalContent);
    }
  }

  public startLearning(): void {
    this.setTrackingActivity(false);
    this.myDigitalContentApiService.enrollDigitalContent(this.originalId).then(_ => {
      this.loadDigitalContentDetails().subscribe(() => {
        this.showLecturePlayer = true;
      });
    });
  }

  public onContentPlayerBackClick(): void {
    this.setTrackingActivity(false);
    this.currentActiveSectionNumber = 1;
    if (this.scrollableParent !== undefined) {
      this.scrollableParent.scrollTop = 0;
    }
    this.hideLecturePlayer();
    this.selectedLectureId = undefined;
    this.loadDigitalContentDetails().subscribe();
  }

  public continue(): void {
    this.showLecturePlayer = true;
  }

  public learnAgain(): void {
    this.showLecturePlayer = true;
  }

  public onReviewShowMore(): void {
    this.onScroll();
  }

  public onContextMenuItemSelect(eventData: { id: string }): void {
    switch (eventData.id) {
      case 'share':
        this.openSharedDialog();
        break;
      case 'copy':
        this.changeCopyURL();
        break;
      case 'bookmarks':
        this.changeBookmark();
        break;
    }
  }

  public openSharedDialog(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(LearningPathSharingDialogComponent, {
      fetchUsersFn: this.fetchUsersFn,
      selectedUsers: this.sharedUsers,
      onShareFn: () => {
        this.userTrackingService.share(this.originalId, LearningType.DigitalContent, this.sharedUsers.map(user => user.id)).then(result => {
          this.totalShare = result.totalShare;

          const msg = this.moduleFacadeService.translator.translateCommon('Shared successfully');
          this.showNotification(msg);
        });
      }
    });

    dialogRef.result.subscribe(() => {
      this.sharedUsers = [];
      dialogRef.close();
    });
  }

  public fetchUsersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<PublicUserInfo[]> = (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => this.myLearningPathDataService.searchUsers({ searchText, maxResultCount, skipCount });

  public changeCopyURL(): void {
    ClipboardUtil.copyTextToClipboard(this.detailUrl);
    const bookmarkTypeMessage = this.moduleFacadeService.translator.translate('Copied successfully');
    this.showNotification(bookmarkTypeMessage);
  }

  public toggleLike(): void {
    this.userTrackingService.like(this.originalId, LearningType.DigitalContent, !this.isLike).then(result => {
      this.totalLike = result.totalLike;
      this.isLike = result.isLike;

      const msgCommon = (this.isLike ? 'Liked' : 'Like undone') + ' successfully';
      const msg = this.moduleFacadeService.translator.translateCommon(msgCommon);
      this.showNotification(msg);
    });
  }

  public get startLearningPermissionKey(): string {
    return LEARNER_PERMISSIONS.Action_StartLearning;
  }

  public get bookmarkPermissionKey(): string {
    return LEARNER_PERMISSIONS.Action_Bookmark;
  }

  public get hasBookmarkPermission(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_Bookmark);
  }

  public get hasLikeShareCopyPermission(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_DigitalContent_Like_Share_Copy);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }

  private listenBookmarkChanged(): void {
    this.learningActionService.bookmarkChanged.pipe(this.untilDestroy()).subscribe(bookmarkChanged => {
      if (bookmarkChanged.itemId === this.originalId) {
        this.itemDetail.bookmarkInfo = bookmarkChanged.isBookmarked ? bookmarkChanged.data : undefined;
      }
    });
  }

  private loadDigitalContentDetails(): Observable<MyDigitalContentDetail> {
    return this.digitalContentDataService.getDigitalContentDetail(this.digitalContentId).pipe(
      this.untilDestroy(),
      tap(response => {
        this.itemDetail = response;
        this.originalId = this.itemDetail.digitalContent.originalObjectId;
        this.learningItem = DigitalContentItemModel.createDigitalContentItemModel(this.itemDetail);
        if (this.learningItem.tags && this.learningItem.tags.length) {
          this.fileType = this.learningItem.tags[0];
        }
        this.viewTracking();
        this.getTrackingInfoByItemId();

        // task is triggered on home page
        this.triggerToStartOrContinueDigitalContentTask();
      })
    );
  }

  private viewTracking(): void {
    if (!this.enableUserTracking) {
      return;
    }
    this.trackingSourceSrv.eventTrack.next({
      eventName: 'LearningTracking',
      payload: <LearningTrackingEventPayload>{
        itemId: this.learningItem.originalId,
        trackingType: 'digitalContent',
        trackingAction: 'view'
      }
    });
  }

  public get detailUrl(): string {
    return `${AppGlobal.environment.appUrl}/learner/detail/${this.learningItem.type.toLocaleLowerCase()}/${this.learningItem.id}`;
  }

  private setTrackingActivity(enable: boolean): void {
    this.enableUserTracking = enable;
  }

  private triggerToStartOrContinueDigitalContentTask(): void {
    if (this.canStartTask) {
      this.startLearning();
    }

    if (this.canContinueTask) {
      this.continue();
    }
  }

  private hideLecturePlayer(): void {
    this.canStartTask = false;
    this.canContinueTask = false;
    this.showLecturePlayer = false;
  }

  private getTrackingInfoByItemId(): void {
    this.userTrackingService.getTrackingInfoByItemId(this.originalId, LearningType.DigitalContent).then(result => {
      this.totalDownload = result.totalDownload;
      this.totalLike = result.totalLike;
      this.totalShare = result.totalShare;
      this.totalView = result.totalView;
      this.isLike = result.isLike;
    });
  }
}
