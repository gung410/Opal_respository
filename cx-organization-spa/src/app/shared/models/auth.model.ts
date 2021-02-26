import { SystemRole } from 'app/core/models/system-role';
import { Department } from 'app/department-hierarchical/models/department.model';
import { GrantedType } from 'app/permissions/enum/granted-type.enum';
import { AccessRightsModel } from 'app/permissions/models/access-rights.model';
import { IPermissionDictionary } from '../components/component.abstract';
import { UserRoleEnum } from '../constants/user-roles.enum';
import { Header } from './header.model';
import { Identity } from './identity.model';

export class User {
  fullName?: string;
  emails?: string;
  id?: string;
  avatarUrl?: string;
  departmentId?: number;
  roles?: any[];
  systemRoles?: SystemRole[];
  identity: Identity;
  entityStatus: any;
  headerData?: Header;
  releaseDate?: string;
  userDepartment?: Department;
  topAccessibleDepartment?: Department;
  permissionDic?: IPermissionDictionary;
  constructor(identityClaims: any) {
    this.id = identityClaims.sub;
  }

  hasPermission(permissionKey: string): boolean {
    const permission: AccessRightsModel = this.permissionDic[permissionKey];

    return permission ? permission.grantedType === GrantedType.Allow : false;
  }

  hasAdminRole(): boolean {
    return this.systemRoles.some(
      (systemRole: any) =>
        systemRole.identity.extId === UserRoleEnum.UserAccountAdministrator ||
        systemRole.identity.extId === UserRoleEnum.OverallSystemAdministrator ||
        systemRole.identity.extId === UserRoleEnum.BranchAdmin ||
        systemRole.identity.extId === UserRoleEnum.SchoolAdmin ||
        systemRole.identity.extId === UserRoleEnum.DivisionAdmin
    );
  }

  hasSecondaryAdminRole(): boolean {
    return this.systemRoles.some(
      (systemRole: any) =>
        systemRole.identity.extId === UserRoleEnum.BranchAdmin ||
        systemRole.identity.extId === UserRoleEnum.SchoolAdmin ||
        systemRole.identity.extId === UserRoleEnum.DivisionAdmin
    );
  }

  hasUserAccountAdministrator(): boolean {
    return this.systemRoles.some(
      (systemRole: any) =>
        systemRole.identity.extId === UserRoleEnum.UserAccountAdministrator
    );
  }

  hasOverallSystemAdministrator(): boolean {
    return this.systemRoles.some(
      (systemRole: any) =>
        systemRole.identity.extId === UserRoleEnum.OverallSystemAdministrator
    );
  }
}

// tslint:disable-next-line:max-classes-per-file
export class SiteData {
  codeName: string;
  localizedData: [];
  menus: MenuItemAPI[] = [];
  releaseDate: string;

  constructor() {}
}

// tslint:disable-next-line:max-classes-per-file
export class MenuItemAPI {
  cssClass: string;
  localizedData: [];
  menuItems: MenuChildItemAPI[] = [];
  type: string;
  constructor() {}
}

// tslint:disable-next-line:max-classes-per-file
export class MenuChildItemAPI {
  cssClass: string;
  localizedData: [];
  path: string;
  childMenuItems: MenuChildItemAPI[] = [];
  openInNewTab: boolean = false;
  constructor() {}
}

// tslint:disable-next-line:max-classes-per-file
export class ErrorAPI {
  error: string;
  errorCode: string;
}
