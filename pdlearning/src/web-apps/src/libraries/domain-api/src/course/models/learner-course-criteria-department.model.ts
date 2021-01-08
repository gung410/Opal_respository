export interface ILearnerCourseCriteriaDepartment {
  departmentId: number;
  departmentUnitTypeId: string;
  departmentLevelTypeId: number;
}

export class LearnerCourseCriteriaDepartment implements ILearnerCourseCriteriaDepartment {
  public departmentId: number = -1;
  public departmentUnitTypeId: string = '';
  public departmentLevelTypeId: number = -1;
  constructor(data?: ILearnerCourseCriteriaDepartment) {
    if (data != null) {
      this.departmentId = data.departmentId;
      this.departmentUnitTypeId = data.departmentUnitTypeId;
      this.departmentLevelTypeId = data.departmentLevelTypeId;
    }
  }
}
