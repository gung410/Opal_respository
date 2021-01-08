import { AppConstant, Constant } from '../app.constant';
import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';

import { HttpHelpers } from '../helpers/httpHelpers';
import { Injectable } from '@angular/core';

// tslint:disable:all

/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
@Injectable({ providedIn: 'root' })
export class AuthDataService extends BaseBackendService {
  constructor(protected commonFacadeService: CommonFacadeService, private http: HttpHelpers) {
    super(commonFacadeService);
  }
  getDataFromCurrentUser(externalId: string): Promise<any> {
    return new Promise(async (resolve, reject) => {
      const siteData = await this.getSiteDataByCurrentUser();
      if (!siteData) {
        reject();

        return;
      }
      const data = {
        siteData,
        departmentId: undefined,
        systemRoles: undefined,
        id: undefined,
        identity: undefined,
        avatarUrl: undefined,
        approvingOfficerGroups: undefined,
        fullName: undefined
      };

      const userInfo = await this.getUserProfileAsync(externalId);
      if (!userInfo) {
        reject({
          errorCode: Constant.CANNOT_GET_USER_INFO
        });

        return;
      }
      data.departmentId = userInfo.departmentId;
      data.systemRoles = userInfo.systemRoles;
      data.id = userInfo.identity.id;
      data.identity = userInfo.identity;
      data.avatarUrl = userInfo.jsonDynamicAttributes.avatarUrl;
      data.approvingOfficerGroups = userInfo.groups ? userInfo.groups : [];
      data.fullName = userInfo.firstName;
      resolve(data);
    });
  }

  getMenusByUser(): any {
    return this.get(`${AppConstant.api.portal}/menus`, undefined);
  }

  loginWithToken(returnUrl: string) {
    return new Promise(async (resolve, reject) => {
      const url = `https://idm.uat.opal2.conexus.net/api/tokens/passwordlessurl?returnUrl=${returnUrl}`;

      const response = await this.get(url);
      if (!response) {
        reject();

        return;
      }

      resolve(response);
    });
  }

  async getUserProfileAsync(externalId: string): Promise<any> {
    const params = { extIds: externalId };
    const users = await this.getListUserProfileAsync(params);
    if (users && users.length > 0) {
      return users[0];
    }

    return;
  }

  async getListUserProfileAsync(filterParams: any): Promise<any[]> {
    const response = await this.get<any>(
      `${AppGlobal.environment.authConfig.organizationUrl}/usermanagement/users`,
      filterParams
    ).toPromise();

    if (response.error) {
      console.error('Get list user profile error');

      return;
    }

    if (!response.items || !response.items.length) {
      return [];
    }

    return response.items;
  }

  private async getSiteDataByCurrentUser(): Promise<any> {
    return this.get<any>(AppGlobal.environment.authConfig.sitesUrl).toPromise();
  }
}
