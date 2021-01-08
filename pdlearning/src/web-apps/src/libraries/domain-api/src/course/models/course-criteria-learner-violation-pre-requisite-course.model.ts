import { CourseCriteriaLearnerViolationType } from './learner-violation-course-criteria-type.model';

export interface ICourseCriteriaLearnerViolationPreRequisiteCourse {
  courseId: string;
  violationType: CourseCriteriaLearnerViolationType;
}

export class CourseCriteriaLearnerViolationPreRequisiteCourse implements ICourseCriteriaLearnerViolationPreRequisiteCourse {
  public courseId: string;
  public violationType: CourseCriteriaLearnerViolationType = CourseCriteriaLearnerViolationType.NotViolate;
  constructor(data?: ICourseCriteriaLearnerViolationPreRequisiteCourse) {
    if (data != null) {
      this.courseId = data.courseId;
      this.violationType = data.violationType;
    }
  }

  public isCourseCriteriaViolated(): boolean {
    return this.violationType !== CourseCriteriaLearnerViolationType.NotViolate;
  }
}
