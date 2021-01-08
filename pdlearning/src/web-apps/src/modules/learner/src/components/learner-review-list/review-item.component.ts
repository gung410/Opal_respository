import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { UserReviewModel } from '@opal20/domain-api';

@Component({
  selector: 'review-item',
  templateUrl: './review-item.component.html'
})
export class ReviewItemComponent extends BaseComponent implements OnInit {
  @Input() public review: UserReviewModel;
  @Input() public hasRating: boolean = true;
  @Input() public canEdit: boolean = true;

  @Output() public edit: EventEmitter<UserReviewModel> = new EventEmitter<UserReviewModel>();

  public wordsCount: number = 0;
  public partialComment: string | undefined;
  public showingMore: boolean = false;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    this.handleCommentDisplay();
  }

  public onEditCliked(): void {
    this.edit.emit(this.review);
  }

  private handleCommentDisplay(): void {
    if (this.review === undefined || this.review.commentContent === undefined) {
      return;
    }
    const reg: RegExp = /\S+/g;

    const wordMatching = this.review.commentContent.match(reg);
    this.wordsCount = wordMatching ? wordMatching.length : 0;

    // Use Regex exec to find index of the 50th word
    let i = 1;
    let word50thMatch: RegExpExecArray | undefined = undefined;
    while (i <= 50) {
      const wordMatch = reg.exec(this.review.commentContent);
      if (i === 50) {
        word50thMatch = wordMatch;
      }
      i++;
    }

    if (word50thMatch) {
      this.partialComment = this.review.commentContent.slice(0, word50thMatch.index + word50thMatch[0].length + 1);
    }
  }
}
