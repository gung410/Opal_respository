import { CourseCriteriaDepartmentLevelType, ICourseCriteriaDepartmentLevelType } from './course-criteria-department-level-type.model';
import { CourseCriteriaDepartmentUnitType, ICourseCriteriaDepartmentUnitType } from './course-criteria-department-unit-type.model';
import { CourseCriteriaSpecificDepartment, ICourseCriteriaSpecificDepartment } from './course-criteria-specific-department.model';

export interface ICourseCriteriaPlaceOfWork {
  type?: CourseCriteriaPlaceOfWorkType;
  departmentUnitTypes?: ICourseCriteriaDepartmentUnitType[];
  departmentLevelTypes?: ICourseCriteriaDepartmentLevelType[];
  specificDepartments?: ICourseCriteriaSpecificDepartment[];
}

export enum CourseCriteriaPlaceOfWorkType {
  DepartmentUnitTypes = 'DepartmentUnitTypes',
  DepartmentLevelTypes = 'DepartmentLevelTypes',
  SpecificDepartments = 'SpecificDepartments'
}

export class CourseCriteriaPlaceOfWork implements ICourseCriteriaPlaceOfWork {
  public type: CourseCriteriaPlaceOfWorkType = CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes;
  public departmentUnitTypes: CourseCriteriaDepartmentUnitType[] = [];
  public departmentLevelTypes: CourseCriteriaDepartmentLevelType[] = [];
  public specificDepartments: CourseCriteriaSpecificDepartment[] = [];
  constructor(data?: ICourseCriteriaPlaceOfWork) {
    if (data == null) {
      return;
    }
    this.type = data.type != null ? data.type : this.type;
    this.departmentUnitTypes = data.departmentUnitTypes ? data.departmentUnitTypes.map(p => new CourseCriteriaDepartmentUnitType(p)) : [];
    this.departmentLevelTypes = data.departmentLevelTypes
      ? data.departmentLevelTypes.map(p => new CourseCriteriaDepartmentLevelType(p))
      : [];
    this.specificDepartments = data.specificDepartments ? data.specificDepartments.map(p => new CourseCriteriaSpecificDepartment(p)) : [];
  }
}
