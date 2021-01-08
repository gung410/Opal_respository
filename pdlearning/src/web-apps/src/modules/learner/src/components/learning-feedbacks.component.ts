import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigurationType, ILearningFeedbacksConfiguration } from '../models/learning-feedbacks.model';
import {
  ICreateUserReviewRequest,
  IUserReviewRequest,
  UserReviewApiService,
  UserReviewItemType,
  UserReviewModel
} from '@opal20/domain-api';

import { getLearnerReviewForm } from '../learner-utils';
import { isEmpty } from 'lodash-es';

@Component({
  selector: 'learning-feedbacks',
  templateUrl: './learning-feedbacks.component.html'
})
export class LearningFeedbacksComponent extends BaseFormComponent {
  @Input() public learningId: string;
  @Input() public learningTitle: string;
  @Input() public reviewType: UserReviewItemType;
  @Input() public configuration: ILearningFeedbacksConfiguration;
  @Input() public classRunId: string;
  @Input() public hasContentChanged: boolean;
  @Output() public submit: EventEmitter<UserReviewModel> = new EventEmitter<UserReviewModel>();
  public review: UserReviewModel | undefined;
  public rating: number;
  public reviewComment: string;

  public displayMode: boolean;
  constructor(protected moduleFacadeService: ModuleFacadeService, private userReviewApiService: UserReviewApiService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.loadMyReview();
  }

  public isShow(config: ConfigurationType): boolean {
    return this.configuration.isShow(config);
  }

  public onRating(rating: number): void {
    if (rating != null) {
      this.rating = rating;
    }
  }

  public onSubmit(): void {
    this.validate().then(val => {
      if (!val) {
        return;
      }
      this.submitMyReview();
    });
  }

  public onEditFeedback(): void {
    this.displayMode = false;
  }

  public onFinish(): void {
    this.submit.emit();
  }

  public get canSubmitReview(): boolean {
    return !this.displayMode;
  }

  public get isDigitalContentReviewType(): boolean {
    return this.reviewType === UserReviewItemType.DigitalContent;
  }

  public get isReviewExisted(): boolean {
    return this.review != null;
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return getLearnerReviewForm(this.configuration);
  }

  protected ignoreValidateForm(): boolean {
    return this.displayMode;
  }

  private loadMyReview(): void {
    const request: IUserReviewRequest = {
      itemId: this.learningId,
      itemType: this.reviewType
    };
    this.userReviewApiService.getMyReviews(request).then(_review => {
      if (!isEmpty(_review)) {
        this.review = _review;
        this.rating = this.review.rate;
        this.reviewComment = this.review.commentContent;
        this.displayMode = this.isReviewExisted;
      }
    });
  }

  private submitMyReview(): void {
    const userReviewRequest: ICreateUserReviewRequest = {
      itemId: this.learningId,
      rating: this.rating,
      commentContent: this.reviewComment,
      itemType: this.reviewType
    };
    if (this.classRunId != null) {
      userReviewRequest.classRunId = this.classRunId;
    }

    this.upsertReview(userReviewRequest).then(review => {
      this.submit.emit(review);
      this.displayMode = true;
    });
  }

  private upsertReview(userReviewRequest: ICreateUserReviewRequest): Promise<UserReviewModel> {
    return !this.isReviewExisted
      ? this.userReviewApiService.createMyReview(userReviewRequest)
      : this.userReviewApiService.updateMyReview(this.learningId, userReviewRequest).then(res => {
          const hasRatingAndReview =
            this.configuration.isShow(this.configuration.rating) && this.configuration.isShow(this.configuration.review);
          const successMessage = hasRatingAndReview ? 'Rating & Review updated successfully' : 'Feedback updated successfully';
          this.showNotification(this.translate(successMessage), 'success');
          return res;
        });
  }
}
