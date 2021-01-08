import { CourseCriteriaLearnerViolationType } from './learner-violation-course-criteria-type.model';

export interface ICourseCriteriaLearnerViolationSpecificDepartment {
  departmentId: number;
  maxParticipant?: number;
  violationType: CourseCriteriaLearnerViolationType;
}

export class CourseCriteriaLearnerViolationSpecificDepartment implements ICourseCriteriaLearnerViolationSpecificDepartment {
  public departmentId: number;
  public maxParticipant?: number = null;
  public violationType: CourseCriteriaLearnerViolationType = CourseCriteriaLearnerViolationType.NotViolate;
  constructor(data?: ICourseCriteriaLearnerViolationSpecificDepartment) {
    if (data != null) {
      this.departmentId = data.departmentId;
      this.maxParticipant = data.maxParticipant;
      this.violationType = data.violationType;
    }
  }
}
