import {
  CourseCriteriaLearnerViolationDepartmentLevelType,
  ICourseCriteriaLearnerViolationDepartmentLevelType
} from './course-criteria-learner-violation-department-level-type.model';
import {
  CourseCriteriaLearnerViolationDepartmentUnitType,
  ICourseCriteriaLearnerViolationDepartmentUnitType
} from './course-criteria-learner-violation-department-unit-type.model';
import {
  CourseCriteriaLearnerViolationSpecificDepartment,
  ICourseCriteriaLearnerViolationSpecificDepartment
} from './course-criteria-learner-violation-specific-department.model';

import { CourseCriteriaLearnerViolationType } from './learner-violation-course-criteria-type.model';
import { DepartmentInfoModel } from '../../organization/models/department-info.model';
import { DepartmentLevelModel } from '../../organization/models/department-level.model';
import { TypeOfOrganization } from '../../share/models/type-of-organization.model';

export interface ICourseCriteriaLearnerViolationPlaceOfWork {
  departmentLevelTypes?: ICourseCriteriaLearnerViolationDepartmentLevelType[];
  departmentUnitTypes?: ICourseCriteriaLearnerViolationDepartmentUnitType[];
  specificDepartments?: ICourseCriteriaLearnerViolationSpecificDepartment[];
}

export class CourseCriteriaLearnerViolationPlaceOfWork implements ICourseCriteriaLearnerViolationPlaceOfWork {
  public departmentLevelTypes?: CourseCriteriaLearnerViolationDepartmentLevelType[] | null;
  public departmentUnitTypes?: CourseCriteriaLearnerViolationDepartmentUnitType[] | null;
  public specificDepartments?: CourseCriteriaLearnerViolationSpecificDepartment[] | null;
  constructor(data?: ICourseCriteriaLearnerViolationPlaceOfWork) {
    if (data != null) {
      this.departmentLevelTypes =
        data.departmentLevelTypes != null
          ? data.departmentLevelTypes.map(p => new CourseCriteriaLearnerViolationDepartmentLevelType(p))
          : null;
      this.departmentUnitTypes =
        data.departmentUnitTypes != null
          ? data.departmentUnitTypes.map(p => new CourseCriteriaLearnerViolationDepartmentUnitType(p))
          : null;
      this.specificDepartments =
        data.specificDepartments != null
          ? data.specificDepartments.map(p => new CourseCriteriaLearnerViolationSpecificDepartment(p))
          : null;
    }
  }

  public isCourseCriteriaViolated(): boolean {
    return (
      (this.departmentUnitTypes != null &&
        this.departmentUnitTypes.filter(p => p.violationType !== CourseCriteriaLearnerViolationType.NotViolate).length > 0) ||
      (this.departmentLevelTypes != null &&
        this.departmentLevelTypes.filter(p => p.violationType !== CourseCriteriaLearnerViolationType.NotViolate).length > 0) ||
      (this.specificDepartments != null &&
        this.specificDepartments.filter(p => p.violationType !== CourseCriteriaLearnerViolationType.NotViolate).length > 0)
    );
  }

  public isDepartmentUnitTypesViolated(): boolean {
    return (
      this.departmentUnitTypes != null &&
      this.departmentUnitTypes.filter(p => p.violationType !== CourseCriteriaLearnerViolationType.NotViolate).length > 0
    );
  }

  public isDepartmentLevelTypesViolated(): boolean {
    return (
      this.departmentLevelTypes != null &&
      this.departmentLevelTypes.filter(p => p.violationType !== CourseCriteriaLearnerViolationType.NotViolate).length > 0
    );
  }

  public isSpecificDepartmentsViolated(): boolean {
    return (
      this.specificDepartments != null &&
      this.specificDepartments.filter(p => p.violationType !== CourseCriteriaLearnerViolationType.NotViolate).length > 0
    );
  }
  public getPlaceOfWorkViolatedTypes(): CourseCriteriaLearnerViolationType[] {
    if (this.isDepartmentUnitTypesViolated()) {
      return this.departmentUnitTypes.map(p => p.violationType);
    } else if (this.isDepartmentLevelTypesViolated()) {
      return this.departmentLevelTypes.map(p => p.violationType);
    } else if (this.isSpecificDepartmentsViolated()) {
      return this.specificDepartments.map(p => p.violationType);
    } else {
      return [];
    }
  }

  public getDepartmentUnitTypesDisplayText(registerOrganizationUnitTypeDic: Dictionary<TypeOfOrganization>): string {
    return this.departmentUnitTypes
      .map(p => p.departmentUnitTypeId)
      .map(p => registerOrganizationUnitTypeDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
  }

  public getDepartmentLevelsDisplayText(registerOrganizationLevelDic: Dictionary<DepartmentLevelModel>): string {
    return this.departmentLevelTypes
      .map(p => p.departmentLevelTypeId)
      .map(p => registerOrganizationLevelDic[p])
      .filter(p => p != null)
      .map(p => p.departmentLevelName)
      .join(', ');
  }

  public getSpecificDepartmentssDisplayText(registerDepartmentsDic: Dictionary<DepartmentInfoModel>): string {
    return this.specificDepartments
      .map(p => p.departmentId)
      .map(p => registerDepartmentsDic[p])
      .filter(p => p != null)
      .map(p => p.departmentName)
      .join(', ');
  }
}
