import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import { User } from 'app-models/auth.model';
import { StartingHierarchyDepartment } from './starting-hierarchy-department.enum';

export class CxPeoplePickerDialogHelper {
  static initFilterParams(
    currentUser: User,
    defaultPageSize: number,
    startingHierarchyDepartment: StartingHierarchyDepartment,
    specifyStartingHierarchyDepartmentId?: number
  ): FilterParamModel {
    return new FilterParamModel({
      pageSize: defaultPageSize,
      departmentIds: this.buildDepartmentFilter(
        currentUser,
        startingHierarchyDepartment,
        specifyStartingHierarchyDepartmentId
      ),
    });
  }

  static buildDepartmentFilter(
    currentUser: User,
    startingHierarchyDepartment: StartingHierarchyDepartment,
    specifyStartingHierarchyDepartmentId?: number
  ): number[] {
    if (
      startingHierarchyDepartment ===
        StartingHierarchyDepartment.SpecifiedDepartment &&
      specifyStartingHierarchyDepartmentId > 0
    ) {
      return [specifyStartingHierarchyDepartmentId];
    }
    const result = [];
    switch (startingHierarchyDepartment) {
      case StartingHierarchyDepartment.TopAccessible:
        result.push(currentUser.topAccessibleDepartment.identity.id);
        break;
      case StartingHierarchyDepartment.DefaultSelected:
        result.push(
          currentUser.topAccessibleDepartment.defaultHierarchyDepartment
            .departmentId
        );
        break;
      default:
        result.push(currentUser.departmentId);
        break;
    }

    return result;
  }

  static checkDisableSearchButton(surveyData: any): boolean {
    return (
      !this.hasFilterOnSearchKey(surveyData) &&
      !this.hasFilterOnServiceSchemes(surveyData)
    );
  }

  static hasFilterOnSearchKey(surveyData: any): boolean {
    return (
      surveyData && surveyData.searchKey && surveyData.searchKey.length > 0
    );
  }

  static hasFilterOnServiceSchemes(surveyData: any): boolean {
    return (
      surveyData &&
      surveyData.serviceSchemes &&
      surveyData.serviceSchemes.length > 0
    );
  }
}
