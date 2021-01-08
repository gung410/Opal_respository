import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, TemplateRef, ViewChild } from '@angular/core';
import { UserReviewApiService, UserReviewItemType, UserReviewModel } from '@opal20/domain-api';

import { ILearningFeedbacksConfiguration } from '../../models/learning-feedbacks.model';
import { LearnerReviewEditingDialogComponent } from './learner-review-editing-dialog.component';
import { OpalDialogService } from '@opal20/common-components';

const itemsPerPage: number = 3;
@Component({
  selector: 'learner-review-list',
  templateUrl: './learner-review-list.component.html'
})
export class LearnerReviewListComponent extends BaseComponent {
  @ViewChild('reviewEditingTemplate', { static: false })
  public reviewEditingTemplate: TemplateRef<unknown>;

  @ViewChild(LearnerReviewEditingDialogComponent, { static: false })
  public learnerReviewEditingDialogComponent: LearnerReviewEditingDialogComponent;

  @Input() public itemId: string;
  @Input() public reviewType: UserReviewItemType;
  @Input() public feedbackConfig: ILearningFeedbacksConfiguration;
  @Output() public showMore: EventEmitter<void> = new EventEmitter<void>();

  public reviews: UserReviewModel[] = [];
  public totalReviews: number = 0;
  public averageRating: number;
  public showReviews: number = itemsPerPage;
  public pageNumber: number = 1;

  public editingReview: UserReviewModel;
  constructor(
    moduleFacadeService: ModuleFacadeService,
    private userReviewApiService: UserReviewApiService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.loadCourseReviews();
  }

  public showLessReviews(): void {
    this.resetPageNumber();
    this.reviews = this.reviews.slice(0, this.showReviews);
  }

  public showMoreReviews(): void {
    this.increasePageNumber();
    this.loadCourseReviews(this.showReviews, this.showReviews * (this.pageNumber - 1));
  }

  public onEditClicked(review: UserReviewModel): void {
    this.editingReview = review;
    this.opalDialogService
      .openConfirmDialog({
        bodyTemplate: this.reviewEditingTemplate,
        confirmTitle: 'Edit Feedback',
        yesBtnText: 'Save',
        noBtnText: 'Cancel',
        confirmMsg: undefined,
        confirmRequest: () => this.confirmUpdateReview(),
        confirmRequestNotShowFailedMsg: true,
        confirmRequestNotShowSuccessMsg: true
      })
      .toPromise();
  }

  public get showRating(): boolean {
    return this.feedbackConfig.isShow(this.feedbackConfig.rating);
  }

  private confirmUpdateReview(): Promise<[string[], undefined] | [undefined, unknown]> {
    const confirmRequestFail: [string[], undefined] = [['error'], undefined];
    const confirmRequestSuccess: [undefined, unknown] = [undefined, undefined];
    return this.learnerReviewEditingDialogComponent
      .submit()
      .then(p => {
        this.onReviewUpdatedSucessfully(p);
        return confirmRequestSuccess;
      })
      .catch(() => confirmRequestFail);
  }

  private onReviewUpdatedSucessfully(review: UserReviewModel): void {
    const index = this.reviews.findIndex(p => p.id === review.id);
    if (index === -1) {
      return;
    }
    this.reCalculateRating(this.reviews[index], review);
    this.reviews.splice(index, 1);
    this.reviews.unshift(review);
  }

  private reCalculateRating(oldReview: UserReviewModel, newReview: UserReviewModel): void {
    const rateChangedPercentage = Math.round(((newReview.rate - oldReview.rate) / this.totalReviews) * 10) / 10;
    this.averageRating += rateChangedPercentage;
  }

  private loadCourseReviews(itemCount: number = 3, skipCount: number = 0): void {
    this.userReviewApiService
      .getReviews({
        itemId: this.itemId,
        itemTypeFilter: [this.reviewType],
        orderBy: 'CreatedDate desc',
        skipCount: skipCount,
        maxResultCount: itemCount
      })
      .then(reviews => {
        this.reviews = this.reviews.concat(reviews.items);
        this.totalReviews = reviews.totalCount;
        this.averageRating = reviews.rating;
      });
  }

  private resetPageNumber(): void {
    this.pageNumber = 1;
  }

  private increasePageNumber(): void {
    this.pageNumber = this.pageNumber + 1;
  }
}
