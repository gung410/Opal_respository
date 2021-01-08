import { CourseReviewViewModel } from '@opal20/domain-components';

export interface IFeedbackModel {
  feedbackGridDataResult: OpalGridDataResult<CourseReviewViewModel>;
  rating: number;
}
