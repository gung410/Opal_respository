export interface ICourseCriteriaDepartmentUnitType {
  departmentUnitTypeId: string;
  maxParticipant?: number;
}

export class CourseCriteriaDepartmentUnitType implements ICourseCriteriaDepartmentUnitType {
  public departmentUnitTypeId: string;
  public maxParticipant?: number;
  constructor(data?: ICourseCriteriaDepartmentUnitType) {
    if (data == null) {
      return;
    }
    this.departmentUnitTypeId = data.departmentUnitTypeId;
    this.maxParticipant = data.maxParticipant;
  }
}
