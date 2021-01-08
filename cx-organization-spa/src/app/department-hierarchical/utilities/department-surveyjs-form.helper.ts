import {
  CxSurveyjsFormModalOptions,
  CxSurveyjsVariable
} from '@conexus/cx-angular-common';
import { DepartmentType } from 'app-models/department-type.model';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';

import {
  organizationUnitLevelFlowConst,
  organizationUnitLevelsConst
} from '../models/department-level.model';

export class DepartmentSurveyJSFormHelper {
  openAddDepartmentSurveyJSForm(
    parentObject: any,
    departmentTypesList: DepartmentType[]
  ): any {
    const organizationUnitLevel = organizationUnitLevelFlowConst.filter(
      // In current, MOE partner doesn't have type, which mean organizationUnitType doesn't exist
      // therefore, type of it will be undefined
      (item) =>
        item.parentIdIdentity ===
        (parentObject.organizationUnitType
          ? parentObject.organizationUnitType[0].identity.extId
          : parentObject.organizationUnitType)
    );

    if (organizationUnitLevel && organizationUnitLevel.length === 0) {
      return null;
    }
    const childOrganizationUnitType = departmentTypesList.filter(
      (item) =>
        item.identity.extId === organizationUnitLevel[0].childrenIdIdentity
    );

    if (childOrganizationUnitType && childOrganizationUnitType.length === 0) {
      return null;
    }
    const departmentUnitType =
      childOrganizationUnitType && childOrganizationUnitType.length > 0
        ? childOrganizationUnitType.map((item) => item.identity.extId)
        : '';
    const isSchoolDepartment = departmentUnitType.includes(
      organizationUnitLevelsConst.School
    );
    const isZoneDepartment = departmentUnitType.includes(
      organizationUnitLevelsConst.Branch_Zone
    );
    const isClusterDepartment = departmentUnitType.includes(
      organizationUnitLevelsConst.Cluster
    );
    const isBranchDepartment = departmentUnitType.includes(
      organizationUnitLevelsConst.Branch_Zone
    );

    const surveyjsVariables = [
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.currentObject_isExternallyMastered,
        value: false
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.formDisplayMode,
        value: 'add'
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.isSchoolDepartment,
        value: isSchoolDepartment
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.isClusterDepartment,
        value: isClusterDepartment
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.isZoneDepartment,
        value: isZoneDepartment
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.isBranchDepartment,
        value: isBranchDepartment
      })
    ];
    const options = {
      showModalHeader: true,
      fixedButtonsFooter: true,
      modalHeaderText: 'Add new Organisation Unit',
      cancelName: 'Cancel',
      submitName: 'Ok',
      variables: surveyjsVariables
    } as CxSurveyjsFormModalOptions;

    const dataJson: any = {
      parentDepartmentId: parentObject.identity.id,
      'identity.ownerId': parentObject.identity.ownerId,
      'identity.customerId': parentObject.identity.customerId,
      'identity.archetype': parentObject.identity.archetype,
      organizationalUnitTypes: childOrganizationUnitType,
      'entityStatus.externallyMastered': false
    };

    return { options, dataJson };
  }
}
