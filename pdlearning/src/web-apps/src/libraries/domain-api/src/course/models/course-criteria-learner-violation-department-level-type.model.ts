import { CourseCriteriaLearnerViolationType } from './learner-violation-course-criteria-type.model';

export interface ICourseCriteriaLearnerViolationDepartmentLevelType {
  departmentLevelTypeId: string;
  maxParticipant?: number;
  violationType: CourseCriteriaLearnerViolationType;
}

export class CourseCriteriaLearnerViolationDepartmentLevelType implements ICourseCriteriaLearnerViolationDepartmentLevelType {
  public departmentLevelTypeId: string;
  public maxParticipant?: number = null;
  public violationType: CourseCriteriaLearnerViolationType = CourseCriteriaLearnerViolationType.NotViolate;
  constructor(data?: ICourseCriteriaLearnerViolationDepartmentLevelType) {
    if (data != null) {
      this.departmentLevelTypeId = data.departmentLevelTypeId;
      this.maxParticipant = data.maxParticipant;
      this.violationType = data.violationType;
    }
  }
}
