import { SystemRole } from 'app/core/models/system-role';
import { findIndexCommon } from '../constants/common.const';
import { Identity } from './common.model';
import { Department } from './department-model';
import { Header } from './header.model';
import {
  DefaultHierarchyDepartment,
  MyTopHierarchyDepartment,
} from './my-top-hierarchy-department-model';
export class User {
  public id: number;
  public identity?: Identity;
  public fullName?: string;
  public emails?: string;
  public extId?: string;
  public avatarUrl?: string;
  public departmentId?: number;
  public headerData?: Header;
  public systemRoles?: SystemRole[];
  public jsonDynamicAttributes?: any;
  public releaseDate: string;
  userDepartment?: Department;
  topAccessibleDepartment?: MyTopHierarchyDepartment;
  defaultHierarchyDepartment?: DefaultHierarchyDepartment;
  /**
   * The list of action keys which the current logged-in user has permission.
   */
  permissions?: string[];
  constructor(identityClaims: any) {
    this.extId = identityClaims.sub;
    this.fullName = identityClaims.name;
    this.emails = identityClaims.emails;
    this.avatarUrl = identityClaims.avatarUrl;
  }

  /**
   * Checks whether the current logged-in user has permission to do something.
   * @param actionKey The action key. e.g: 'CompetenceSpa.OrganisationalDevelopment.StrategicThrusts.CUD'
   */
  hasPermission(actionKey: string): boolean {
    const lowerCaseActionKey = actionKey ? actionKey.toLowerCase() : null;

    return (
      lowerCaseActionKey &&
      this.permissions &&
      this.permissions.findIndex((p) => p === lowerCaseActionKey) >
        findIndexCommon.notFound
    );
  }
}

export class UserBasicInfo {
  fullName?: string;
  email?: string;
  identity?: Identity;
  avatarUrl?: string;
  constructor(userInfo: Partial<UserBasicInfo>) {
    if (userInfo) {
      this.fullName = userInfo.fullName;
      this.email = userInfo.email;
      this.identity = userInfo.identity;
      this.avatarUrl = userInfo.avatarUrl;
    }
  }
}

export class SiteData {
  public codeName: string;
  public localizedData: [];
  public menus: Array<MenuItemAPI> = [];
  public releaseDate: string;
  constructor() {}
}

export class MenuItemAPI {
  public cssClass: string;
  public localizedData: [];
  public menuItems: Array<MenuChildItemAPI> = [];
  public type: string;
  constructor() {}
}

export class MenuChildItemAPI {
  public cssClass: string;
  public localizedData: any;
  public path: string;
  public childMenuItems: Array<MenuChildItemAPI> = [];
  public isDefault: boolean;
  public openInNewTab: boolean = false;
  constructor() {}
}

export class ErrorAPI {
  error: string;
  errorCode: string;
}
