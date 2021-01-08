import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { CourseReviewManagementModel } from 'app-models/course-review.model';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';

const itemsPerPage: number = 3;
@Component({
  selector: 'course-review',
  templateUrl: './course-review.component.html',
  styleUrls: ['./course-review.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourseReviewComponent implements OnInit {
  @Input() pdoDetail: PDOpportunityDetailModel;
  @Input() reviews: CourseReviewManagementModel;
  @Output() loadMoreReview: EventEmitter<number> = new EventEmitter();

  public pageNumber: number = 1;
  public showReviews: number = itemsPerPage;

  constructor() {}

  ngOnInit(): void {}

  public showMoreReviews(): void {
    this.increasePageNumber();
    this.loadMoreReview.emit(this.showReviews);
  }

  public showLessReviews(): void {
    this.resetPageNumber();
    this.reviews.items = this.reviews.items.slice(0, this.showReviews);
  }
  private increasePageNumber(): void {
    this.showReviews = this.showReviews + itemsPerPage;
  }
  private resetPageNumber(): void {
    this.showReviews = itemsPerPage;
  }
}
