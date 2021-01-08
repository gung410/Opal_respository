import { Injectable } from '@angular/core';
import { SystemRole } from 'app/core/models/system-role';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';

@Injectable()
export class CheckingUserRolesService {
  constructor() {}

  hasRightToAccessReportingUser(systemRoles: any[]): boolean {
    return (
      systemRoles.findIndex((role: SystemRole) => {
        return (
          role.identity.extId === UserRoleEnum.OverallSystemAdministrator ||
          role.identity.extId === UserRoleEnum.UserAccountAdministrator ||
          role.identity.extId === UserRoleEnum.BranchAdmin ||
          role.identity.extId === UserRoleEnum.SchoolAdmin ||
          role.identity.extId === UserRoleEnum.DivisionAdmin ||
          role.identity.extId === UserRoleEnum.DivisionalLearningCoordinator ||
          role.identity.extId === UserRoleEnum.SchoolStaffDeveloper
        );
      }) > findIndexCommon.notFound
    );
  }
}
