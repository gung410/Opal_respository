import { CourseCriteriaLearnerViolationType } from './learner-violation-course-criteria-type.model';

export interface ICourseCriteriaLearnerViolationAccountType {
  accountType: CourseCriteriaLearnerViolationAccountTypeEnum;
  violationType: CourseCriteriaLearnerViolationType;
}

export class CourseCriteriaLearnerViolationAccountType implements ICourseCriteriaLearnerViolationAccountType {
  public accountType: CourseCriteriaLearnerViolationAccountTypeEnum = CourseCriteriaLearnerViolationAccountTypeEnum.AllLearners;
  public violationType: CourseCriteriaLearnerViolationType;
  constructor(data?: ICourseCriteriaLearnerViolationAccountType) {
    if (data != null) {
      this.accountType = data.accountType;
      this.violationType = data.violationType;
    }
  }

  public isCourseCriteriaViolated(): boolean {
    return this.violationType !== CourseCriteriaLearnerViolationType.NotViolate;
  }
}

export enum CourseCriteriaLearnerViolationAccountTypeEnum {
  AllLearners = 'AllLearners',
  MOELearners = 'MOELearners',
  ExternalLearners = 'ExternalLearners'
}
