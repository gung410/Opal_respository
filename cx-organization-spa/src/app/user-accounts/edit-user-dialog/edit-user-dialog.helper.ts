import { Injectable } from '@angular/core';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { cloneDeep, isEmpty } from 'lodash';
import * as Survey from 'survey-angular';

import { CxSurveyJsUtil } from '@conexus/cx-angular-common';
import { organizationUnitLevelsConst } from 'app/department-hierarchical/models/department-level.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import { UserManagement } from '../models/user-management.model';
import { EditUserDialogModeEnum } from './edit-user-dialog.model';

const getArrayFromDropdownAndTagbox = (params: any[]) => {
  if (!params || params.length < 2) {
    return;
  }
  const param1 = params[0];
  const param2 = params[1];
  if (!isEmpty(param1)) {
    return Array.isArray(param1) ? [...param1] : [param1];
  }
  if (!isEmpty(param2)) {
    return Array.isArray(param2) ? [...param2] : [param2];
  }

  return [];
};

const DEPARTMENT_TYPES = {
  HQ: 'HQ_EAS',
  SCHOOL: 'SCHOOL_ADMIN_TEAM'
};

const getDesignationQueryParams = (params: any[]) => {
  if (!params || params.length < 3) {
    return;
  }
  const [careerPaths, personnelGroups, departmentTypes] = params;
  const queryParams = getArrayFromDropdownAndTagbox([
    careerPaths,
    personnelGroups
  ]);
  const isSchool =
    departmentTypes &&
    departmentTypes.includes(organizationUnitLevelsConst.School);
  const departmentTypeCode = isSchool
    ? DEPARTMENT_TYPES.SCHOOL
    : DEPARTMENT_TYPES.HQ;
  queryParams.push(departmentTypeCode);

  return queryParams;
};

Survey.FunctionFactory.Instance.register(
  'getArrayFromDropdownAndTagbox',
  getArrayFromDropdownAndTagbox
);

Survey.FunctionFactory.Instance.register(
  'getDesignationQueryParams',
  getDesignationQueryParams
);

const unregisterSameRequests = (obj: Survey.ChoicesRestfull, items: any) => {
  // tslint:disable-next-line: no-string-literal
  const res = Survey.ChoicesRestfull['sendingSameRequests'][obj['objHash']];
  if (!res) {
    return;
  }
  // tslint:disable-next-line:no-string-literal
  delete Survey.ChoicesRestfull['sendingSameRequests'][obj['objHash']];
  for (let i = 0; i < res.length; i++) {
    if (!!res[i].getResultCallback) {
      res[i].getResultCallback(items);
    }
  }
};
// tslint:disable-next-line:no-string-literal
Survey.ChoicesRestfull['unregisterSameRequests'] = unregisterSameRequests;

@Injectable()
export class EditUserDialogHelper {
  // This is requirement for default expiration date from OP-4074
  readonly defaultExpirationDate: string = '01/12/2099';
  user: UserManagement;
  currentUser: UserManagement;
  init(user?: UserManagement, currentUser?: UserManagement): void {
    this.user = user;
    this.currentUser = currentUser;
  }
  getDialogMode(): EditUserDialogModeEnum {
    if (!this.user) {
      return EditUserDialogModeEnum.Create;
    }
    if (this.user) {
      const isViewMode = this.isViewMode();

      return isViewMode
        ? EditUserDialogModeEnum.View
        : EditUserDialogModeEnum.Edit;
    }
  }

  processSurveyJsonData(jsonData: any): any {
    if (!jsonData) {
      return {};
    }
    const data = cloneDeep(jsonData);
    if (data.careerPaths) {
      data.careerPathsDropdown = data.careerPaths[0];
      data.careerPathsTagbox = data.careerPaths;

      delete data.careerPaths;
    }

    if (data.learningFrameworks) {
      data.learningFrameworksDropdown = data.learningFrameworks[0];
      data.learningFrameworksTagbox = data.learningFrameworks;

      delete data.learningFrameworks;
    }

    if (!data.expirationDate && !data.externallyMastered) {
      data.expirationDate = this.defaultExpirationDate;
    }

    if (!data.personalStorageSize) {
      const defaultPersonalStorageSize = 10;
      data.personalStorageSize = defaultPersonalStorageSize;
    }

    return data;
  }

  processSurveyResultData(data: any): any {
    if (!data) {
      return {};
    }
    data.careerPaths = getArrayFromDropdownAndTagbox([
      data.careerPathsDropdown,
      data.careerPathsTagbox
    ]);
    data.learningFrameworks = getArrayFromDropdownAndTagbox([
      data.learningFrameworksDropdown,
      data.learningFrameworksTagbox
    ]);

    data.personalStorageSize = data.personalStorageSize
      ? data.personalStorageSize.toString()
      : null;

    data.isStorageUnlimited =
      data.systemRoles &&
      data.systemRoles.findIndex(
        (role: any) =>
          role.identity.extId === UserRoleEnum.OverallSystemAdministrator
      ) !== findIndexCommon.notFound;

    return data;
  }

  /**
   * Add custom properties into the existing survey js form.
   * @param userForm The survey js form for adding/editing a user.
   * @param editingUser The editing user.
   * @param userAvatarUrl The avatar url of the editing user.
   */
  addCustomProperties(
    userForm: any,
    editingUser: UserManagement,
    userAvatarUrl: string
  ): void {
    const isCreatingNewUser = !editingUser;

    const avatarHTML = `<div *ngIf="isEditMode!==EditUserDialogMode.Create" class="user__avatar user__avatar--center">
    <img class="user__avatar-image" src="${userAvatarUrl}">
    </div>`;
    CxSurveyJsUtil.addProperty(userForm, 'avatar', 'html', avatarHTML);

    const activeDateConfig = {
      changeMonth: true,
      changeYear: true,
      yearRange: 'c:c+20',
      minDate: isCreatingNewUser ? '+0d' : ''
    };
    CxSurveyJsUtil.addProperty(
      userForm,
      'activeDate',
      'config',
      activeDateConfig
    );

    const expirationDateConfig = {
      changeMonth: true,
      changeYear: true,
      yearRange: 'c-50:c+50',
      minDate: isCreatingNewUser ? '+0d' : ''
    };
    CxSurveyJsUtil.addProperty(
      userForm,
      'expirationDate',
      'config',
      expirationDateConfig
    );

    this.handlePropertiesWhenEditUserNotOnBoarding(userForm, editingUser);
  }

  // MOE want admin can edit role event learner not onboarding,
  // meaning fields related to onboarding became not required if it doesn't have value
  private handlePropertiesWhenEditUserNotOnBoarding(
    userForm: any,
    editingUser: UserManagement
  ): void {
    CxSurveyJsUtil.addProperty(
      userForm,
      'personnelGroups',
      'isRequired',
      !editingUser || (editingUser && !isEmpty(editingUser.personnelGroups))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'careerPathsDropdown',
      'isRequired',
      !editingUser || (editingUser && !isEmpty(editingUser.careerPaths))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'careerPathsTagbox',
      'isRequired',
      !editingUser || (editingUser && !isEmpty(editingUser.careerPaths))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'developmentalRoles',
      'isRequired',
      !editingUser || (editingUser && !isEmpty(editingUser.developmentalRoles))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'learningFrameworksDropdown',
      'isRequired',
      !editingUser || (editingUser && !isEmpty(editingUser.learningFrameworks))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'learningFrameworksTagbox',
      'isRequired',
      !editingUser || (editingUser && !isEmpty(editingUser.learningFrameworks))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'teachingLevels',
      'isRequired',
      !editingUser ||
        (editingUser &&
          editingUser.jsonDynamicAttributes &&
          !isEmpty(editingUser.jsonDynamicAttributes.teachingLevels))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'teachingCourseOfStudy',
      'isRequired',
      !editingUser ||
        (editingUser &&
          editingUser.jsonDynamicAttributes &&
          !isEmpty(editingUser.jsonDynamicAttributes.teachingCourseOfStudy))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'teachingSubjects',
      'isRequired',
      !editingUser ||
        (editingUser &&
          editingUser.jsonDynamicAttributes &&
          !isEmpty(editingUser.jsonDynamicAttributes.teachingSubjects))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'cocurricularActivities',
      'isRequired',
      !editingUser ||
        (editingUser &&
          editingUser.jsonDynamicAttributes &&
          !isEmpty(editingUser.jsonDynamicAttributes.cocurricularActivities))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'roleSpecificProficiencies',
      'isRequired',
      !editingUser ||
        (editingUser &&
          editingUser.jsonDynamicAttributes &&
          !isEmpty(editingUser.jsonDynamicAttributes.roleSpecificProficiencies))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'jobFamily',
      'isRequired',
      !editingUser ||
        (editingUser &&
          editingUser.jsonDynamicAttributes &&
          !isEmpty(editingUser.jsonDynamicAttributes.jobFamilies))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'designation',
      'isRequired',
      !editingUser ||
        (editingUser &&
          editingUser.jsonDynamicAttributes &&
          !isEmpty(editingUser.jsonDynamicAttributes.designation))
    );

    CxSurveyJsUtil.addProperty(
      userForm,
      'professionalInterestArea',
      'isRequired',
      !editingUser ||
        (editingUser &&
          editingUser.jsonDynamicAttributes &&
          !isEmpty(editingUser.jsonDynamicAttributes.professionalInterestArea))
    );
  }

  private isViewMode(): boolean {
    if (!this.user && !this.user.entityStatus) {
      return false;
    }

    if (this.user.entityStatus.statusId === StatusTypeEnum.Deactive.code) {
      return true;
    }

    if (
      this.user.entityStatus.statusId === StatusTypeEnum.PendingApproval3rd.code
    ) {
      return !UserManagement.hasSystemAdminRole(this.currentUser.systemRoles);
    }

    if (
      this.user.entityStatus.statusId === StatusTypeEnum.PendingApproval2nd.code
    ) {
      return !(
        UserManagement.hasSystemAdminRole(this.currentUser.systemRoles) ||
        UserManagement.hasUserAccountAdminRole(this.currentUser.systemRoles)
      );
    }

    return false;
  }
}
