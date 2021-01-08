import { LearningNeedsCompletionRate } from 'app-models/mpj/idp.model';
import { IdpStatusCodeEnum } from 'app/individual-development/idp.constant';

export class LearningNeedHelpers {
  static readonly LEARNING_NEED_UNSUBMITTED_STATUS_CODE: IdpStatusCodeEnum[] = [
    IdpStatusCodeEnum.NotAdded,
    IdpStatusCodeEnum.NotStarted,
    IdpStatusCodeEnum.Started,
    IdpStatusCodeEnum.Rejected,
  ];

  static readonly LEARNING_NEED_SUBMITTED_STATUS_CODE: IdpStatusCodeEnum[] = [
    IdpStatusCodeEnum.PendingForApproval,
    IdpStatusCodeEnum.Approved,
    IdpStatusCodeEnum.Rejected,
    IdpStatusCodeEnum.Completed,
  ];

  public static ProcessCompletionRateByStep(currentStep: number): number {
    switch (currentStep) {
      case 1: {
        return LearningNeedsCompletionRate.Started;
      }
      case 2: {
        return LearningNeedsCompletionRate.CareerAspiration;
      }
      case 3: {
        return LearningNeedsCompletionRate.Competencies;
      }
      case 4: {
        return LearningNeedsCompletionRate.PrioritiesOfLearningAreas;
      }
      default: {
        return;
      }
    }
  }
}
