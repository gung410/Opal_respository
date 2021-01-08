import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { ConfigurationType, ILearningFeedbacksConfiguration } from '../../models/learning-feedbacks.model';
import { IUpdateUserReviewRequest, UserReviewApiService, UserReviewModel } from '@opal20/domain-api';

import { getLearnerReviewForm } from '../../learner-utils';

@Component({
  selector: 'learner-review-editing-dialog',
  templateUrl: './learner-review-editing-dialog.component.html'
})
export class LearnerReviewEditingDialogComponent extends BaseFormComponent {
  @Input()
  public set review(value: UserReviewModel) {
    this._review = value;
    this.rating = this.review.rate;
    this.reviewComment = this.review.commentContent;
  }
  public get review(): UserReviewModel {
    return this._review;
  }

  @Input() public configuration: ILearningFeedbacksConfiguration;

  public rating: number;
  public reviewComment: string;
  public displayMode: boolean = false;
  private _review: UserReviewModel;
  constructor(public moduleFacadeService: ModuleFacadeService, private userReviewApiService: UserReviewApiService) {
    super(moduleFacadeService);
  }

  public isShow(config: ConfigurationType): boolean {
    return this.configuration.isShow(config);
  }

  public onRating(rating: number): void {
    if (rating != null) {
      this.rating = rating;
    }
  }

  public submit(): Promise<UserReviewModel> {
    return this.validate().then(val => {
      if (!val) {
        return Promise.reject();
      }
      return this.submitMyReview();
    });
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return getLearnerReviewForm(this.configuration);
  }

  protected ignoreValidateForm(): boolean {
    return this.displayMode;
  }

  private submitMyReview(): Promise<UserReviewModel> {
    const userReviewRequest: IUpdateUserReviewRequest = {
      rating: this.rating,
      commentContent: this.reviewComment,
      itemType: this.review.itemType
    };

    return this.userReviewApiService.updateMyReview(this.review.itemId, userReviewRequest).then(res => {
      const hasRatingAndReview =
        this.configuration.isShow(this.configuration.rating) && this.configuration.isShow(this.configuration.review);
      const successMessage = hasRatingAndReview ? 'Rating & Review updated successfully' : 'Feedback updated successfully';
      this.showNotification(this.translate(successMessage), 'success');
      return res;
    });
  }
}
