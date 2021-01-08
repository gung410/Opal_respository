import { CourseCriteriaLearnerViolationType } from './learner-violation-course-criteria-type.model';

export interface ICourseCriteriaLearnerViolationTaggingMetadata {
  tagId: string;
  maxParticipant?: number;
  violationType: CourseCriteriaLearnerViolationType;
}

export class CourseCriteriaLearnerViolationTaggingMetadata implements ICourseCriteriaLearnerViolationTaggingMetadata {
  public tagId: string;
  public maxParticipant: number = null;
  public violationType: CourseCriteriaLearnerViolationType = CourseCriteriaLearnerViolationType.NotViolate;
  constructor(data?: ICourseCriteriaLearnerViolationTaggingMetadata) {
    if (data != null) {
      this.tagId = data.tagId;
      this.maxParticipant = data.maxParticipant;
      this.violationType = data.violationType;
    }
  }

  public isCourseCriteriaViolated(): boolean {
    return this.violationType !== CourseCriteriaLearnerViolationType.NotViolate;
  }
}
