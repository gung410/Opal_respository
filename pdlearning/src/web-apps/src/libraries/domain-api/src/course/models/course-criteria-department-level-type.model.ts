export interface ICourseCriteriaDepartmentLevelType {
  departmentLevelTypeId: string;
  maxParticipant?: number;
}

export class CourseCriteriaDepartmentLevelType implements ICourseCriteriaDepartmentLevelType {
  public departmentLevelTypeId: string;
  public maxParticipant?: number;
  constructor(data?: ICourseCriteriaDepartmentLevelType) {
    if (data == null) {
      return;
    }
    this.departmentLevelTypeId = data.departmentLevelTypeId;
    this.maxParticipant = data.maxParticipant;
  }
}
