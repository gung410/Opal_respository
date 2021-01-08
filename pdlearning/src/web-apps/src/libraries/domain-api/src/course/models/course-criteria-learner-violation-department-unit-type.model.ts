import { CourseCriteriaLearnerViolationType } from './learner-violation-course-criteria-type.model';

export interface ICourseCriteriaLearnerViolationDepartmentUnitType {
  departmentUnitTypeId: string;
  maxParticipant?: number;
  violationType: CourseCriteriaLearnerViolationType;
}

export class CourseCriteriaLearnerViolationDepartmentUnitType implements ICourseCriteriaLearnerViolationDepartmentUnitType {
  public departmentUnitTypeId: string;
  public maxParticipant?: number = null;
  public violationType: CourseCriteriaLearnerViolationType = CourseCriteriaLearnerViolationType.NotViolate;
  constructor(data?: ICourseCriteriaLearnerViolationDepartmentUnitType) {
    if (data != null) {
      this.departmentUnitTypeId = data.departmentUnitTypeId;
      this.maxParticipant = data.maxParticipant;
      this.violationType = data.violationType;
    }
  }
}
