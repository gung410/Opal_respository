import { Injectable } from '@angular/core';
import { UserType } from 'app-models/user-type.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { SystemRole } from 'app/core/models/system-role';
import { emptyArray } from 'app/shared/common.constant';
import { findIndexCommon } from 'app/shared/constants/common.const';
import {
  JSON_DYNAMIC_FIELDS,
  JsonDynamicEnum
} from 'app/shared/constants/fields-json-dynamic.enum';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import * as _ from 'lodash';

import { DepartmentHierarchiesModel } from '@conexus/cx-angular-common';
import { User } from 'app-models/auth.model';
import { CrossSiteScriptingUtil } from 'app-utilities/cross-site-scripting.utils';
import { Department } from 'app/department-hierarchical/models/department.model';
import { AppConstant } from 'app/shared/app.constant';
import { EditUserDialogSubmitModel } from './edit-user-dialog/edit-user-dialog.model';
import { EditUser } from './models/edit-user.model';
import { USER_ACTION_MAPPING_CONST } from './models/user-action-mapping';
import { UserManagement } from './models/user-management.model';

@Injectable()
export class UserAccountsHelper {
  static isUserEdited(userInfoBefore: any, userInfoAfter: any): boolean {
    for (const key in userInfoAfter) {
      if (!_.isEqual(userInfoAfter[key], userInfoBefore[key])) {
        return true;
      }
    }

    return false;
  }

  /**
   * When the Organization Unit (Place of Work) is changed, then the removed system roles should be mapped into the new one.
   * e.g:
   *  - removedSystemRoles:   A, B1, C
   *  - availableSystemRoles: A, B2, D
   *  => result:              A, B2 =>  Rule:  Map B1 -> B2; Remove C.
   * @param removedSystemRoles The list of system roles which have been removed because it's only relevant for the old Organization Unit.
   * @param availableSystemRoles The list of system roles which should be displayed (available) in the form.
   */
  static findMappingRemovedSystemRoles(
    removedSystemRoles: UserType[],
    availableSystemRoles: UserType[]
  ): UserType[] {
    if (!removedSystemRoles || !availableSystemRoles) {
      return;
    }
    const adminRoleMapping = ['divisionadmin', 'branchadmin', 'schooladmin'];
    const otherRoleMapping = [
      'divisiontrainingcoordinator',
      'schooltrainingcoordinator'
    ];
    const newMappingSystemRoles = [];
    removedSystemRoles.forEach((oldSystemRole) => {
      if (adminRoleMapping.includes(oldSystemRole.identity.extId)) {
        const newMappingSystemRole = this.findMatchingSystemRole(
          adminRoleMapping,
          availableSystemRoles
        );
        if (newMappingSystemRole) {
          newMappingSystemRoles.push(newMappingSystemRole);
        }
      } else if (otherRoleMapping.includes(oldSystemRole.identity.extId)) {
        const newMappingSystemRole = this.findMatchingSystemRole(
          otherRoleMapping,
          availableSystemRoles
        );
        if (newMappingSystemRole) {
          newMappingSystemRoles.push(newMappingSystemRole);
        }
      }
    });

    return newMappingSystemRoles;
  }

  static findMatchingSystemRole(
    findingRoles: string[],
    availableSystemRoles: UserType[]
  ): UserType {
    return availableSystemRoles.find((availableSystemRole) =>
      findingRoles.includes(availableSystemRole.identity.extId)
    );
  }

  /**
   * Build the new system roles based on the current system roles of the user and the available system roles of the new Organization Unit.
   * e.g:
   *  - Current system roles:     A, B1, C
   *  - Available system roles:   A, B2, D
   *  => New system roles:        A, B2 => Rule:  Map B1 -> B2; Remove C.
   * @param currentUserSystemRoles The list of current system roles of the user (some may not available in the new Organization Unit).
   * @param availableSystemRoles The list of system roles which are available in the new Organization Unit.
   */
  static buildNewSystemRoles(
    currentUserSystemRoles: UserType[],
    availableSystemRoles: UserType[]
  ): UserType[] {
    if (!currentUserSystemRoles || !availableSystemRoles) {
      return currentUserSystemRoles;
    }

    const identityKey = 'identity.id';
    const removedSystemRoles = _.differenceBy(
      currentUserSystemRoles,
      availableSystemRoles,
      identityKey
    );
    const newSystemRoles = _.differenceBy(
      currentUserSystemRoles,
      removedSystemRoles,
      identityKey
    );

    const newMappingSystemRoles = this.findMappingRemovedSystemRoles(
      removedSystemRoles,
      availableSystemRoles
    );

    return _.union(newSystemRoles, newMappingSystemRoles);
  }

  /**
   * Get the list of accessible user action mappings based on the list of the user roles of the current logged-in user.
   * @param currentUserRoles The list of the user roles of the current logged-in user.
   */
  static getAccessibleUserActionMapping(currentUserRoles: SystemRole[]): any[] {
    const currentUserRoleExtIds = currentUserRoles.map(
      (role) => role.identity.extId
    );

    return USER_ACTION_MAPPING_CONST.filter((actionMapping: any) => {
      return (
        !actionMapping.allowedUserRoles ||
        (actionMapping.allowedUserRoles &&
          _.intersection(currentUserRoleExtIds, actionMapping.allowedUserRoles)
            .length > 0)
      );
    });
  }

  /**
   * Gets the default root department id based on the role of the current logged-in user.
   * @param currentUser The current logged-in user
   */
  static getDefaultRootDepartment(currentUser: User): number {
    return currentUser.topAccessibleDepartment
      ? currentUser.topAccessibleDepartment.identity.id
      : currentUser.departmentId;
  }

  static searchDepartments(
    departments: Department[],
    searchKey: string
  ): Department[] {
    if (!departments || !searchKey) {
      return departments;
    }

    return departments.filter(
      (department: Department) =>
        department.departmentName
          .trim()
          .toLowerCase()
          .indexOf(searchKey.trim().toLowerCase()) > findIndexCommon.notFound ||
        department.identity.extId
          .trim()
          .toLowerCase()
          .indexOf(searchKey.trim().toLowerCase()) > findIndexCommon.notFound
    );
  }

  /*
   * Gets the current/selected department in the department browser on the top left of the main screen.
   * It will find the matching item with the currentDepartmentId in the instance, otherwise it will pick the first item in the list.
   * @param departmentHierarchiesModel The current instance of DepartmentHierarchiesModel.
   */
  static getCurrentDepartment(
    departmentHierarchiesModel: DepartmentHierarchiesModel
  ): Department {
    if (
      !departmentHierarchiesModel ||
      !departmentHierarchiesModel.departments ||
      departmentHierarchiesModel.departments.length === 0
    ) {
      return null;
    }

    let currentDepartment = departmentHierarchiesModel.departments.find(
      (x: Department) =>
        x.identity.id === departmentHierarchiesModel.currentDepartmentId
    );
    if (!currentDepartment) {
      currentDepartment = departmentHierarchiesModel.departments[0];
    }

    return currentDepartment;
  }

  careerPaths: { [id: string]: any };
  developmentalRoles: { [id: string]: any };
  learningFrameworks: { [id: string]: any };
  personnelGroups: { [id: string]: any };

  /**
   * Too bad! will fine time to refactor
   */
  processEditUser(
    userBeingEdited: UserManagement,
    editedUser: EditUser,
    submittedResult: EditUserDialogSubmitModel,
    isEditNormalUser: boolean
  ): UserManagement {
    userBeingEdited.firstName = CrossSiteScriptingUtil.encodeHtmlEntity(
      editedUser.firstName
    );
    userBeingEdited.emailAddress = editedUser.emailAddress;
    if (editedUser.ssn && editedUser.ssn.length > 0) {
      userBeingEdited.ssn = editedUser.ssn;
    } else {
      delete userBeingEdited.ssn;
    }
    userBeingEdited.gender = editedUser.gender;
    if (editedUser.dateOfBirth) {
      // TODO: Fix this and sync the logic to other modules.
      userBeingEdited.dateOfBirth = DateTimeUtil.surveyToServerFormat(
        editedUser.dateOfBirth
      );
    }
    userBeingEdited.entityStatus.expirationDate = DateTimeUtil.surveyToEndDateLocalTimeISO(
      editedUser.expirationDate
    );
    userBeingEdited.entityStatus.activeDate = DateTimeUtil.surveyToDateLocalTimeISO(
      editedUser.activeDate
    );
    userBeingEdited.systemRoles = editedUser.systemRoles
      ? editedUser.systemRoles
      : emptyArray;

    userBeingEdited.careerPaths = this.addUserTypes(
      this.careerPaths,
      editedUser.careerPaths
    );
    userBeingEdited.learningFrameworks = this.addUserTypes(
      this.learningFrameworks,
      editedUser.learningFrameworks
    );

    const developmentalRoles =
      editedUser.developmentalRoles && editedUser.developmentalRoles.id;
    const personnelGroups =
      editedUser.personnelGroups && editedUser.personnelGroups.id;
    userBeingEdited.developmentalRoles = this.addUserTypes(
      this.developmentalRoles,
      developmentalRoles
    );
    userBeingEdited.personnelGroups = this.addUserTypes(
      this.personnelGroups,
      personnelGroups
    );

    userBeingEdited.jsonDynamicAttributes = userBeingEdited.jsonDynamicAttributes
      ? userBeingEdited.jsonDynamicAttributes
      : {};

    this.updateJsonDynamic(userBeingEdited, submittedResult.userData);

    userBeingEdited.jsonDynamicAttributes.signupReason =
      editedUser.signupReason;

    if (userBeingEdited.systemRoles) {
      const notExistLearnerRole =
        userBeingEdited.systemRoles.findIndex(
          (role: any) => role.identity.extId === UserRoleEnum.Learner
        ) === findIndexCommon.notFound;
      if (notExistLearnerRole) {
        if (
          userBeingEdited.jsonDynamicAttributes &&
          userBeingEdited.jsonDynamicAttributes.finishOnBoarding === false
        ) {
          delete userBeingEdited.jsonDynamicAttributes.finishOnBoarding;
        }
      } else {
        if (
          (isEditNormalUser &&
            userBeingEdited.entityStatus.statusId ===
              StatusTypeEnum.New.code) ||
          !isEditNormalUser
        ) {
          userBeingEdited.jsonDynamicAttributes.finishOnBoarding = false;
        }
      }
    }

    userBeingEdited.jsonDynamicAttributes.personalStorageSize = userBeingEdited
      .jsonDynamicAttributes.personalStorageSize
      ? userBeingEdited.jsonDynamicAttributes.personalStorageSize
      : null;

    userBeingEdited.jsonDynamicAttributes.isStorageUnlimited =
      userBeingEdited.systemRoles.findIndex(
        (role: any) =>
          role.identity.extId === UserRoleEnum.OverallSystemAdministrator
      ) !== findIndexCommon.notFound;

    return userBeingEdited;
  }

  private updateJsonDynamic(
    userBeingEdited: UserManagement,
    userData: EditUser
  ): void {
    const newJsonDynamic = { ...userBeingEdited.jsonDynamicAttributes };

    JSON_DYNAMIC_FIELDS.forEach((key) => {
      const oldJsonDynamicFieldData = newJsonDynamic[key];
      const newJsonDynamicFieldData = userData[key];

      const hasOldData = !_.isEmpty(oldJsonDynamicFieldData);
      const hasNewData = !_.isEmpty(newJsonDynamicFieldData);

      if (hasNewData) {
        newJsonDynamic[key] = newJsonDynamicFieldData;

        return;
      }

      if (hasOldData && !hasNewData) {
        delete newJsonDynamic[key];
      }
    });

    userBeingEdited.jsonDynamicAttributes = newJsonDynamic;
  }

  private addUserTypes(listUserTypes: any, submittedData: any): any[] {
    let userTypesResult = submittedData;
    if (!submittedData) {
      return [];
    }

    if (!(submittedData instanceof Array)) {
      userTypesResult = [userTypesResult];
    }

    return listUserTypes.filter((userType: any) => {
      return (
        userTypesResult.findIndex(
          (item: string) => item === userType.identity.extId
        ) > findIndexCommon.notFound
      );
    });
  }
}

export enum UserAccountTabEnum {
  UserAccounts = 'useraccounts',
  Pending1st = 'pending1st',
  Pending2nd = 'pending2nd',
  Pending3rd = 'pending3rd',
  UserOtherPlace = 'userotherplace'
}
