import { Injectable } from '@angular/core';
import { SystemRole } from 'app/core/models/system-role';
import { findIndexCommon } from '../constants/common.const';
import { UserRoleEnum, UserRoleGroups } from '../constants/user-roles.enum';

@Injectable()
export class SystemRoleHelpers {
  static hasSpecificRole(
    userSystemRoles: SystemRole[],
    lookingRole: UserRoleEnum
  ): boolean {
    return (
      userSystemRoles.findIndex(
        (role) => lookingRole.toString() === role.identity.extId
      ) !== findIndexCommon.notFound
    );
  }

  static hasAnyAdministratorRole(userSystemRoles: SystemRole[]): boolean {
    return (
      userSystemRoles.findIndex((role) =>
        UserRoleGroups.Administrators.includes(role.identity.extId)
      ) !== findIndexCommon.notFound
    );
  }

  static hasEitherDivisionalLearningCoordinatorOrSchoolStaffDeveloper(
    userSystemRoles: SystemRole[]
  ): boolean {
    return (
      userSystemRoles.findIndex((role) =>
        UserRoleGroups.DivisionalLearningCoordinatorOrSchoolStaffDeveloper.includes(
          role.identity.extId
        )
      ) !== findIndexCommon.notFound
    );
  }
}
