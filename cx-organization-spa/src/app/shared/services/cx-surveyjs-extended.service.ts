import { Injectable } from '@angular/core';
import {
  CxSurveyjsService,
  CxSurveyjsVariable
} from '@conexus/cx-angular-common';
import { User } from 'app-models/auth.model';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import {
  AssignRoleInOrgPermission,
  AssignRolePermission
} from 'app/user-accounts/assign-role-permission';
import * as _ from 'lodash';

import { DepartmentType } from 'app-models/department-type.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { SystemRoleHelpers } from 'app-utilities/system-role.helpers';
import { GetSystemRolesWithPermissionsRequest } from 'app/core/dtos/get-system-roles-with-permissions-request.dto';
import {
  Granted,
  SystemRolePermissionSubject
} from 'app/core/models/system-role-permission-subject.model';
import { SystemRoleSubjects } from 'app/core/models/system-role-subjects.model';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { Department } from 'app/department-hierarchical/models/department.model';
import { OrganizationUnitType } from 'app/department-hierarchical/models/organization-unit-type.enum';
import { archeType } from 'app/user-accounts/constants/arche-type.constant';
import { AppConstant } from '../app.constant';
import { findIndexCommon } from '../constants/common.const';
import { DesignationQueryParamEnum } from '../constants/designation-query-param.enum';
import { SurveyVariableEnum } from '../constants/survey-variable.enum';
import { UserRoleEnum } from '../constants/user-roles.enum';
import { Utils } from '../utilities/utils';

@Injectable({
  providedIn: 'root'
})
export class CxSurveyjsExtendedService {
  // tslint:disable-next-line:variable-name
  private currentUser_noPermissionRoles: number[];
  private userAssignRolePermission: number[];
  private currentUser: User;
  private allSystemRoles: SystemRoleSubjects[];

  constructor(
    private cxSurveyjsService: CxSurveyjsService,
    private departmentStoreService: DepartmentStoreService,
    private systemRolesDataService: SystemRolesDataService
  ) {}

  async setCurrentUserVariables(user: User): Promise<CxSurveyjsVariable[]> {
    const variables = {};
    this.currentUser = Utils.cloneDeep(user);

    this.allSystemRoles = await this.getAllSystemRoles();

    const systemRoleDic = Utils.toDictionary(
      this.allSystemRoles,
      (role) => role.id
    );

    const userAssignRoleIds: number[] = [];

    this.currentUser.systemRoles.forEach((systemRole) => {
      const systemRoleIds = this.allSystemRoles
        .filter((roleInfo) =>
          this.isSystemRolesVisibleToCurrentUser(
            systemRole.identity.id,
            roleInfo.systemRolePermissionSubjects
          )
        )
        .map((role) => role.id);
      userAssignRoleIds.push(...systemRoleIds);
    });

    this.userAssignRolePermission = Utils.distinct(userAssignRoleIds);

    const fullGrantedRoles: number[] = [];

    this.currentUser.systemRoles.forEach((systemRole) => {
      const systemRoleIds = this.userAssignRolePermission.filter((roleId) =>
        this.isSatisfiedGrantedBySystemRoleId(
          systemRole.identity.id,
          systemRoleDic[roleId].systemRolePermissionSubjects,
          Granted.Full
        )
      );
      fullGrantedRoles.push(...systemRoleIds);
    });

    variables[
      SurveyVariableEnum.currentUser_fullGrantedRoles
    ] = fullGrantedRoles;

    variables[
      SurveyVariableEnum.currentUser_systemRoles_ExId
    ] = !user.systemRoles
      ? []
      : user.systemRoles.map((role) => role.identity.extId);

    variables[
      SurveyVariableEnum.currentUser_visibleRolePermission
    ] = this.userAssignRolePermission;

    variables[SurveyVariableEnum.currentUser_extId] = user.id; // TODO: Change user.id to extid

    variables[SurveyVariableEnum.currentUser_departmentId] = user.departmentId;

    variables[
      SurveyVariableEnum.currentUser_isOverallSystemAdministrator
    ] = SystemRoleHelpers.hasSpecificRole(
      user.systemRoles,
      UserRoleEnum.OverallSystemAdministrator
    );

    variables[
      SurveyVariableEnum.currentUser_isUserAccountAdministrator
    ] = SystemRoleHelpers.hasSpecificRole(
      user.systemRoles,
      UserRoleEnum.UserAccountAdministrator
    );

    variables[
      SurveyVariableEnum.currentUser_isAdministrator
    ] = SystemRoleHelpers.hasAnyAdministratorRole(user.systemRoles);

    variables[
      SurveyVariableEnum.currentUser_isDivisionLearningCoordinatorOrSchoolStaffDeveloper
    ] = SystemRoleHelpers.hasEitherDivisionalLearningCoordinatorOrSchoolStaffDeveloper(
      user.systemRoles
    );

    variables[SurveyVariableEnum.currentDate] = DateTimeUtil.toDateString(
      new Date(),
      AppConstant.backendDateFormat
    );

    this.setCurrentUserDepartmentTypes(user.departmentId);
    this.cxSurveyjsService.setVariables(variables);

    return this.cxSurveyjsService.variables;
  }

  setCurrentDepartmentVariables(departmentId: number): void {
    this.departmentStoreService
      .getDepartmentById(departmentId)
      .subscribe((department) => {
        if (department) {
          const variables = {};
          variables[SurveyVariableEnum.currentDepartment_id] =
            department.identity.id;
          variables[SurveyVariableEnum.currentDepartment_name] =
            department.departmentName;
          variables[SurveyVariableEnum.currentDepartment_archetype] =
            department.identity.archetype;
          this.cxSurveyjsService.setVariables(variables);
        }
      });
    this.setCurrentDepartmentAvailableRolesByDepartmentId(departmentId);
  }

  setCurrentDepartmentAvailableRolesByDepartmentId(departmentId: number): void {
    this.departmentStoreService
      .getDepartmentTypesByDepartmentId(departmentId)
      .subscribe((departmentTypes) => {
        this.setCurrentDepartmentAvailableRoles(departmentTypes);
      });
  }

  setCurrentDepartmentAvailableRoles(departmentTypes: DepartmentType[]): void {
    const variables = {};
    variables[
      SurveyVariableEnum.currentDepartment_availableRoles
    ] = this.buildAvailableRolesByDepartmentTypes(departmentTypes);
    this.cxSurveyjsService.setVariables(variables);
  }

  setCurrentUserDepartmentTypes(departmentId: number): void {
    this.departmentStoreService
      .getDepartmentTypesByDepartmentId(departmentId)
      .subscribe((departmentTypes) => {
        const variables = {};
        variables[
          SurveyVariableEnum.currentUser_departmentTypes
        ] = departmentTypes ? departmentTypes.map((p) => p.identity.extId) : [];
        this.cxSurveyjsService.setVariables(variables);
      });
  }

  /**
   * Build the list of extIds of system roles which relevant for the list of department types.
   * @param departmentTypes The list of department types of the department.
   */
  buildAvailableRolesByDepartmentTypes(
    departmentTypes: DepartmentType[]
  ): string[] {
    let userAssignRolePermission = [];
    if (departmentTypes) {
      userAssignRolePermission = departmentTypes
        .filter(
          (x) =>
            x.identity.archetype === archeType.OrganizationalUnitType ||
            x.identity.archetype === archeType.Unknown
        )
        .map((type) => AssignRoleInOrgPermission[type.identity.extId])
        .reduce((arr1, arr2) => _.union(arr1, arr2), []);
    }
    userAssignRolePermission = _.union(
      AssignRoleInOrgPermission.default,
      userAssignRolePermission
    );

    return userAssignRolePermission;
  }

  buildCurrentObjectVariables(obj: any): CxSurveyjsVariable[] {
    const variables: CxSurveyjsVariable[] = [];
    if (obj.identity) {
      variables.push({
        name: SurveyVariableEnum.currentObject_id,
        value: obj.identity.id
      });
      variables.push({
        name: SurveyVariableEnum.currentObject_extId,
        value: obj.identity.extId
      });
    }
    if (obj.entityStatus) {
      variables.push({
        name: SurveyVariableEnum.currentObject_entityStatus,
        value: obj.entityStatus.statusId
      });
      variables.push({
        name: SurveyVariableEnum.currentObject_isExternallyMastered,
        value: obj.entityStatus.externallyMastered
      });
    }

    if (obj.systemRoles) {
      // tslint:disable-next-line:variable-name
      const currentObject_systemRoles: string[] = obj.systemRoles.map(
        (role) => role.identity.extId
      );

      variables.push({
        name: SurveyVariableEnum.currentObject_systemRoles,
        value: currentObject_systemRoles
      });

      const systemRoleIdsOfSelectedUser: number[] = obj.systemRoles.map(
        (role) => role.identity.id
      );

      variables.push({
        name: SurveyVariableEnum.systemRoleIdsOfSelectedUser,
        value: systemRoleIdsOfSelectedUser
      });

      this.currentUser_noPermissionRoles = Utils.differenceWith(
        systemRoleIdsOfSelectedUser,
        this.userAssignRolePermission
      );

      if (this.currentUser_noPermissionRoles) {
        variables.push({
          name: SurveyVariableEnum.currentUser_noPermissionRoles,
          value: this.currentUser_noPermissionRoles
        });
      }
    }

    return variables;
  }

  buildCurrentObjectDepartmentVariables(
    department: Department
  ): CxSurveyjsVariable[] {
    if (!department) {
      return [];
    }

    return [
      {
        name: SurveyVariableEnum.currentObject_departmentId,
        value: department.identity.id
      },
      {
        name: SurveyVariableEnum.currentObject_departmentArchetype,
        value: department.identity.archetype
      },
      {
        name: SurveyVariableEnum.currentObject_departmentName,
        value: department.departmentName
      }
    ];
  }

  buildCurrentObjectOrganizationUnitTypes(
    organizationUnitTypes: DepartmentType[]
  ): CxSurveyjsVariable[] {
    if (!organizationUnitTypes || !organizationUnitTypes.length) {
      return [];
    }
    const isSchoolDepartment =
      organizationUnitTypes &&
      organizationUnitTypes.findIndex(
        (orgType: DepartmentType) =>
          orgType.identity.extId === OrganizationUnitType.School.toLowerCase()
      ) > findIndexCommon.notFound;

    const designationQueryParam = isSchoolDepartment
      ? DesignationQueryParamEnum.SCHOOL.toString()
      : DesignationQueryParamEnum.HQ.toString();

    return [
      {
        name: SurveyVariableEnum.currentOrganisationUnit_types,
        value: designationQueryParam
      }
    ];
  }

  buildCurrentObjectDepartmentTypes(
    departmentTypes: DepartmentType[]
  ): CxSurveyjsVariable[] {
    if (!departmentTypes || !departmentTypes.length) {
      return [];
    }

    return [
      {
        name: SurveyVariableEnum.currentObject_departmentTypes,
        value: departmentTypes
          ? departmentTypes.map((p) => p.identity.extId)
          : []
      }
    ];
  }

  setAPIVariables(): void {
    const variables = {};
    variables[SurveyVariableEnum.organizationApi_BaseUrl] =
      AppConstant.api.organization;
    variables[SurveyVariableEnum.assessmentApi_BaseUrl] =
      AppConstant.api.assessment;
    variables[SurveyVariableEnum.learningCatalogApi_BaseUrl] =
      AppConstant.api.learningCatalog;
    variables[
      SurveyVariableEnum.learningCatalogApi_catalogentries_explorer_url
    ] = `${AppConstant.api.learningCatalog}/catalogentries/explorer`;
    this.cxSurveyjsService.setVariables(variables);
  }

  private getAllSystemRoles(): Promise<SystemRoleSubjects[]> {
    return this.systemRolesDataService
      .getSystemRolesWithPermissions(
        new GetSystemRolesWithPermissionsRequest({
          includeLocalizedData: true,
          includeSystemRolePermissionSubjects: true
        })
      )
      .toPromise();
  }

  private isSystemRolesVisibleToCurrentUser(
    systemRoleId: number,
    systemRolePermissionSubjects: SystemRolePermissionSubject[]
  ): boolean {
    return this.isSatisfiedGrantedBySystemRoleId(
      systemRoleId,
      systemRolePermissionSubjects,
      [Granted.Read, Granted.Full]
    );
  }

  private isSatisfiedGrantedBySystemRoleId(
    systemRoleId: number,
    systemRolePermissionSubjects: SystemRolePermissionSubject[],
    granted: Granted | [Granted.Read, Granted.Full]
  ): boolean {
    switch (granted.toString()) {
      case Granted.Full.toString():
        return systemRolePermissionSubjects.some(
          (systemRolePermissionSubject) =>
            systemRolePermissionSubject.id === systemRoleId &&
            systemRolePermissionSubject.granted === Granted.Full
        );
      case Granted.Read.toString():
        return systemRolePermissionSubjects.some(
          (systemRolePermissionSubject) =>
            systemRolePermissionSubject.id === systemRoleId &&
            systemRolePermissionSubject.granted === Granted.Read
        );
      case Granted.Deny.toString():
        return systemRolePermissionSubjects.some(
          (systemRolePermissionSubject) =>
            systemRolePermissionSubject.id === systemRoleId &&
            systemRolePermissionSubject.granted === Granted.Deny
        );
      case [Granted.Read, Granted.Full].toString():
        return systemRolePermissionSubjects.some(
          (systemRolePermissionSubject) =>
            systemRolePermissionSubject.id === systemRoleId &&
            systemRolePermissionSubject.granted !== Granted.Deny
        );
      default:
        return false;
    }
  }
}
