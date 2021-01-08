import { Header } from './header.model';

// tslint:disable:all

/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export class User {
  public id: number;
  public identity?: any;
  public fullName?: string;
  public emails?: string;
  public extId?: string;
  public avatarUrl?: string;
  public siteData?: SiteData;
  public departmentId?: number;
  public headerData?: Header;
  public systemRoles?: any[];
  public jsonDynamicAttributes?: any;
  public approvingOfficerGroups?: [{}];
  constructor(identityClaims: any) {
    this.extId = identityClaims.sub;
    this.fullName = identityClaims.name;
    this.emails = identityClaims.emails;
    this.avatarUrl = identityClaims.avatarUrl;
  }
}

export class UserBasicInfo {
  name: string;
  email: string;
  identity: any;
  avatarUrl: string;
  constructor(userInfo: Partial<UserBasicInfo>) {
    if (userInfo) {
      this.name = userInfo.name;
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
  public localizedData: [];
  public path: string;
  public childMenuItems: Array<MenuChildItemAPI> = [];
  public isDefault: boolean;
  constructor() {}
}

export class ErrorAPI {
  error: string;
  errorCode: string;
}
