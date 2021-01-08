import { Component, Input, OnInit } from '@angular/core';
import { CourseReviewModel } from 'app-models/course-review.model';

@Component({
  selector: 'course-review-item',
  templateUrl: './course-review-item.component.html',
  styleUrls: ['./course-review-item.component.scss'],
})
export class CourseReviewItemComponent implements OnInit {
  @Input() review: CourseReviewModel;
  @Input() public hasRating: boolean = true;

  public wordsCount: number = 0;
  public partialComment: string | undefined;
  public showingMore: boolean = false;

  constructor() {}

  ngOnInit(): void {
    this.handleCommentDisplay();
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
    let word50thMatch: RegExpExecArray | undefined;
    while (i <= 50) {
      const wordMatch = reg.exec(this.review.commentContent);
      if (i === 50) {
        word50thMatch = wordMatch;
      }
      i++;
    }

    if (word50thMatch) {
      this.partialComment = this.review.commentContent.slice(
        0,
        word50thMatch.index + word50thMatch[0].length + 1
      );
    }
  }
}
