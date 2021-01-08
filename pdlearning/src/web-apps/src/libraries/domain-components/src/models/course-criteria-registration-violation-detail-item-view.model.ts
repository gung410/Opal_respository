export interface ICourseCriteriaRegistrationViolationDetailItemViewModel {
  criteria: string;
  learnerProfile: string;
  courseCriteria: string;
  reason: string;
}

export class CourseCriteriaRegistrationViolationDetailItemViewModel implements ICourseCriteriaRegistrationViolationDetailItemViewModel {
  public criteria: string;
  public learnerProfile: string;
  public courseCriteria: string;
  public reason: string;

  public static create(
    criteriaFieldDisplayText: string,
    learnerProfileDisplayText: string,
    courseCriteriaDisplayText: string,
    reasonDisplayText: string
  ): CourseCriteriaRegistrationViolationDetailItemViewModel {
    return new CourseCriteriaRegistrationViolationDetailItemViewModel({
      criteria: criteriaFieldDisplayText,
      learnerProfile: learnerProfileDisplayText,
      courseCriteria: courseCriteriaDisplayText,
      reason: reasonDisplayText
    });
  }

  constructor(data?: ICourseCriteriaRegistrationViolationDetailItemViewModel) {
    if (data != null) {
      this.criteria = data.criteria;
      this.learnerProfile = data.learnerProfile;
      this.courseCriteria = data.courseCriteria;
      this.reason = data.reason;
    }
  }
}
