import { BookmarkInfoModel, BookmarkType, IUserBookmarkRequest, MyBookmarkApiService } from '@opal20/domain-api';
import { ModuleFacadeService, NotificationType, TIME_HIDDEN_NOTIFICATION } from '@opal20/infrastructure';

import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { TrackingSourceService } from '../user-activities-tracking/tracking-souce.service';

@Injectable()
export class LearningActionService {
  public bookmarkChanged: Subject<BookmarkChangedEvent> = new Subject<BookmarkChangedEvent>();
  constructor(
    private myBookmarkApiService: MyBookmarkApiService,
    private moduleFacadeService: ModuleFacadeService,
    private trackingSourceSvr: TrackingSourceService
  ) {}

  public bookmark(itemId: string, type: BookmarkType): Promise<BookmarkInfoModel> {
    const request: IUserBookmarkRequest = {
      itemId: itemId,
      itemType: type
    };
    return this.myBookmarkApiService.createBookmark(request).then(bookmarkInfo => {
      this.trackingSourceSvr.eventTrack.next({
        eventName: 'BookmarkItem',
        payload: {
          isUnbookmark: false,
          ...request
        }
      });

      const bookmarkTypeMessage = this.moduleFacadeService.translator.translateCommon('Bookmarked successfully');
      this.showNotification(bookmarkTypeMessage);
      this.bookmarkChanged.next({ itemId: itemId, isBookmarked: true, data: bookmarkInfo });
      return bookmarkInfo;
    });
  }

  public unBookmark(itemId: string, type: BookmarkType): Promise<void> {
    const request: IUserBookmarkRequest = {
      itemId: itemId,
      itemType: type
    };
    return this.myBookmarkApiService.unbookmarkItem(request).then(() => {
      this.trackingSourceSvr.eventTrack.next({
        eventName: 'BookmarkItem',
        payload: {
          isUnbookmark: true,
          ...request
        }
      });

      const bookmarkTypeMessage = this.moduleFacadeService.translator.translateCommon('Unbookmarked successfully');
      this.showNotification(bookmarkTypeMessage);
      this.bookmarkChanged.next({ itemId: itemId, isBookmarked: false });
    });
  }

  public showNotification(content: string, type: NotificationType | string = NotificationType.Success): void {
    this.moduleFacadeService.notificationService.show({
      content: content,
      hideAfter: TIME_HIDDEN_NOTIFICATION,
      position: { horizontal: 'right', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type as ('none' | 'success' | 'warning' | 'error' | 'info'), icon: true }
    });
  }
}

export type BookmarkChangedEvent = { itemId: string; isBookmarked: boolean; data?: BookmarkInfoModel };
