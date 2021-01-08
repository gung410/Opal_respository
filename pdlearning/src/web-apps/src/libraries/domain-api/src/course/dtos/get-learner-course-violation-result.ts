import { ILearnerViolationCourseCriteria, LearnerViolationCourseCriteria } from '../models/learner-violation-course-criteria.model';

export interface IGetLearnerCourseViolationQueryResult {
  violationDetail?: ILearnerViolationCourseCriteria;
  isCourseCriteriaForClassRunActivated: boolean;
}

export class GetLearnerCourseViolationQueryResult implements IGetLearnerCourseViolationQueryResult {
  public violationDetail: LearnerViolationCourseCriteria | null;
  public isCourseCriteriaForClassRunActivated: boolean = false;

  constructor(data?: IGetLearnerCourseViolationQueryResult) {
    if (data) {
      this.violationDetail = data.violationDetail ? new LearnerViolationCourseCriteria(data.violationDetail) : null;
      this.isCourseCriteriaForClassRunActivated = data.isCourseCriteriaForClassRunActivated;
    }
  }
}
