import { Injectable } from '@angular/core';
import { SystemRole } from 'app/core/models/system-role';
import { UserRoleEnum } from '../constants/user-roles.enum';
import { findIndexCommon } from '../constants/common.const';

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

  static hasAnyRole(
    userSystemRoles: SystemRole[],
    lookingRoles: (UserRoleEnum | string)[],
    acceptLookingRolesEmpty: boolean = false
  ): boolean {
    if (lookingRoles && lookingRoles.length === 0) {
      return acceptLookingRolesEmpty;
    }

    const lookingRoleExtIds =
      typeof lookingRoles[0] === 'string'
        ? lookingRoles
        : lookingRoles.map((r) => r.toString());

    return (
      userSystemRoles.findIndex((role) =>
        lookingRoleExtIds.includes(role.identity.extId)
      ) !== findIndexCommon.notFound
    );
  }
}
