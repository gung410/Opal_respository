import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { BookmarkType } from '@opal20/domain-api';
import { CommunityItemModel } from '../models/community-item.model';
import { LearningActionService } from '../services/learning-action.service';

@Component({
  selector: 'learning-community-card',
  templateUrl: './learning-community-card.component.html'
})
export class LearningCommunityCardComponent extends BaseComponent {
  @Input()
  public communityCardItem: CommunityItemModel;
  @Input()
  public showBookmark: boolean = true;
  @Input()
  public showLongCard: boolean = false;

  constructor(protected moduleFacadeService: ModuleFacadeService, private learningActionService: LearningActionService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.learningActionService.bookmarkChanged.pipe(this.untilDestroy()).subscribe(bookmarkChanged => {
      if (this.communityCardItem.id === bookmarkChanged.itemId) {
        this.communityCardItem.isBookmark = bookmarkChanged.isBookmarked;
      }
    });
  }

  public onBookmarkChange(): void {
    this.communityCardItem.isBookmark
      ? this.learningActionService.unBookmark(this.communityCardItem.id, BookmarkType.Community)
      : this.learningActionService.bookmark(this.communityCardItem.id, BookmarkType.Community);
  }

  public onCommunityClick(): void {
    window.location.href = this.communityCardItem.urlDetail;
  }
}
