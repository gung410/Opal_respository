import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { BookmarkType } from '@opal20/domain-api';
import { LearnerLearningPathModel } from '../models/learning-path.model';
import { LearningActionService } from '../services/learning-action.service';

@Component({
  selector: 'learning-path-card',
  templateUrl: './learning-path-card.component.html'
})
export class LearningPathCardComponent extends BaseComponent {
  @Input()
  public learningCardItem: LearnerLearningPathModel;
  @Input()
  public showBookmark: boolean = true;
  @Input()
  public displayOnlyMode: boolean = false;

  @Output()
  public learningCardClick: EventEmitter<LearnerLearningPathModel> = new EventEmitter<LearnerLearningPathModel>();

  constructor(protected moduleFacadeService: ModuleFacadeService, private learningActionService: LearningActionService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.learningActionService.bookmarkChanged.pipe(this.untilDestroy()).subscribe(bookmarkChanged => {
      if (this.learningCardItem.id === bookmarkChanged.itemId) {
        this.learningCardItem.isBookmark = bookmarkChanged.isBookmarked;
      }
    });
  }

  public onLearningCardClick(): void {
    this.learningCardClick.emit(this.learningCardItem);
  }
  public onBookmarkChange(): void {
    const bookmarkType = this.learningCardItem.fromLMM === true ? BookmarkType.LearningPathLMM : BookmarkType.LearningPath;
    this.learningCardItem.isBookmark
      ? this.learningActionService.unBookmark(this.learningCardItem.id, bookmarkType)
      : this.learningActionService.bookmark(this.learningCardItem.id, bookmarkType);
  }
}
