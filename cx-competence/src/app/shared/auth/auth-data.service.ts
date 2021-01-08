import { Injectable } from '@angular/core';
import { SiteData, User } from 'app-models/auth.model';
import { AccessRightService } from 'app-services/access-right.service';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import { forkJoin } from 'rxjs';
import { AppConstant, Constant } from '../app.constant';

@Injectable({
  providedIn: 'root',
})
export class AuthDataService {
  constructor(
    private http: HttpHelpers,
    private departmentStoreService: DepartmentStoreService,
    private accessRightService: AccessRightService
  ) {}

  getDataFromCurrentUser(): Promise<any> {
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
        userDepartment: undefined,
        topAccessibleDepartment: undefined,
        defaultHierarchyDepartment: undefined,
        permissions: undefined,
      };
      const userPermissions = await this.accessRightService
        .getMyPermissions()
        .toPromise();
      data.permissions = userPermissions;

      const userInfo = await this.getUserProfileAsync();
      if (!userInfo) {
        reject({
          errorCode: Constant.CANNOT_GET_USER_INFO,
        });

        return;
      }
      data.departmentId = userInfo.departmentId;
      data.systemRoles = userInfo.systemRoles;
      data.id = userInfo.identity ? userInfo.identity.id : undefined;
      data.identity = userInfo.identity;
      data.avatarUrl = userInfo.jsonDynamicAttributes
        ? userInfo.jsonDynamicAttributes.avatarUrl
        : undefined;

      forkJoin([
        this.departmentStoreService.getMyTopDepartment(),
        this.departmentStoreService.getDepartmentById(userInfo.departmentId),
      ]).subscribe(([myTopDepartment, userDepartment]): any => {
        data.userDepartment = userDepartment;
        data.topAccessibleDepartment = myTopDepartment;
        data.defaultHierarchyDepartment =
          myTopDepartment.defaultHierarchyDepartment;

        resolve(data);
      });
    });
  }

  getMenusByUser(): any {
    return this.http.get(`${AppConstant.api.portal}/menus`, undefined, {
      avoidIntercepterCatchError: true,
    });
  }

  async getUserProfileAsync(): Promise<User> {
    const user = await this.getListUserProfileAsync();
    if (user) {
      return user;
    }

    return;
  }

  async getListUserProfileAsync(): Promise<User> {
    const response = await this.http.getAsync<User>(
      `${AppConstant.api.organization}/usermanagement/me?getRoles=true`,
      null,
      { avoidIntercepterCatchError: true }
    );

    if (response.error) {
      console.error('Get user profile error');

      return;
    }

    return response.data;
  }

  async getSiteDataByCurrentUser(): Promise<SiteData> {
    const response = await this.http.getAsync<SiteData>(
      `${AppConstant.api.portal}/sites`,
      undefined,
      { avoidIntercepterCatchError: true }
    );
    if (response.error) {
      console.error('Cannot get site data');

      return;
    }

    return response.data;
  }
}
