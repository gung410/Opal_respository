import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { BookmarkType, MyDigitalContentStatus } from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { LearningActionService } from '../services/learning-action.service';
import { MY_DIGITAL_CONTENT_STATUS_DISPLAY_MAP } from '../constants/learning-card-status.constant';

@Component({
  selector: 'digital-content-long-card',
  templateUrl: './digital-content-long-card.component.html'
})
export class DigitalContentLongCardComponent extends BaseComponent {
  @Input()
  public learningCardItem: DigitalContentItemModel;
  @Input()
  public showBookmark: boolean = true;
  @Input()
  public displayOnlyMode: boolean = false;

  @Output()
  public learningCardClick: EventEmitter<DigitalContentItemModel> = new EventEmitter<DigitalContentItemModel>();

  public MyDigitalContentStatus: typeof MyDigitalContentStatus = MyDigitalContentStatus;

  constructor(protected moduleFacadeService: ModuleFacadeService, protected learningActionService: LearningActionService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    if (!this.learningCardItem) {
      return;
    }
    this.learningActionService.bookmarkChanged.pipe(this.untilDestroy()).subscribe(bookmarkChanged => {
      if (this.learningCardItem.originalId === bookmarkChanged.itemId) {
        this.learningCardItem.isBookmark = bookmarkChanged.isBookmarked;
      }
    });
  }

  public onBookmarkChange(): void {
    this.learningCardItem.isBookmark
      ? this.learningActionService.unBookmark(this.learningCardItem.originalId, BookmarkType.DigitalContent)
      : this.learningActionService.bookmark(this.learningCardItem.originalId, BookmarkType.DigitalContent);
  }

  public onDigitalContentCardClick(): void {
    this.learningCardClick.emit(this.learningCardItem);
  }

  public get learningStatusText(): string {
    return MY_DIGITAL_CONTENT_STATUS_DISPLAY_MAP.get(this.learningCardItem.learningStatus);
  }
}
