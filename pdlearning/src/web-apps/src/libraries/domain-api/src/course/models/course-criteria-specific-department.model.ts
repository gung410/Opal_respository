export interface ICourseCriteriaSpecificDepartment {
  departmentId: number;
  maxParticipant?: number;
}

export class CourseCriteriaSpecificDepartment implements ICourseCriteriaSpecificDepartment {
  public departmentId: number = -1;
  public maxParticipant?: number;
  constructor(data?: ICourseCriteriaSpecificDepartment) {
    if (data == null) {
      return;
    }
    this.departmentId = data.departmentId;
    this.maxParticipant = data.maxParticipant;
  }
}
