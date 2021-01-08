import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { forkJoin, Observable } from 'rxjs';

import { SiteData } from 'app-models/auth.model';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import { AccessRightsRequest } from 'app/permissions/dtos/request-dtos/access-rights-request';
import { AccessRightsModel } from 'app/permissions/models/access-rights.model';
import {
  PagingResponseModel,
  UserManagement
} from 'app/user-accounts/models/user-management.model';
import { AppConstant } from '../app.constant';

@Injectable({
  providedIn: 'root'
})
export class AuthDataService {
  constructor(
    private httpHelpers: HttpHelpers,
    private departmentStoreService: DepartmentStoreService
  ) {}

  getDataFromCurrentUser(externalId: string): Promise<any> {
    return new Promise((resolve, reject) => {
      this.getSiteDataByCurrentUser().subscribe(
        (siteDataResp: SiteData) => {
          if (siteDataResp && siteDataResp.codeName === AppConstant.clientId) {
            const data = {
              siteData: siteDataResp,
              departmentId: undefined,
              roles: undefined,
              systemRoles: undefined,
              identity: undefined,
              entityStatus: undefined,
              avatarUrl: undefined,
              name: undefined,
              emails: undefined,
              userDepartment: undefined,
              topAccessibleDepartment: undefined
            };
            this.getVerifiedUserInfo(externalId).subscribe(
              (users: PagingResponseModel<UserManagement>) => {
                if (users && users.items && users.items[0]) {
                  const userInfo = users.items[0];
                  data.name = userInfo.firstName
                    ? userInfo.firstName
                    : undefined;
                  data.emails = userInfo.emailAddress
                    ? userInfo.emailAddress
                    : undefined;
                  data.departmentId = userInfo.departmentId
                    ? userInfo.departmentId
                    : undefined;
                  data.roles = userInfo.roles ? userInfo.roles : undefined;
                  data.systemRoles = userInfo.systemRoles
                    ? userInfo.systemRoles
                    : undefined;
                  data.identity = userInfo.identity
                    ? userInfo.identity
                    : undefined;
                  data.entityStatus = userInfo.entityStatus
                    ? userInfo.entityStatus
                    : undefined;
                  data.avatarUrl = userInfo.jsonDynamicAttributes
                    ? userInfo.jsonDynamicAttributes.avatarUrl
                    : undefined;

                  forkJoin(
                    this.departmentStoreService.getMyTopDepartment(),
                    this.departmentStoreService.getDepartmentById(
                      userInfo.departmentId
                    )
                  ).subscribe(([myTopDepartment, userDepartment]): any => {
                    data.userDepartment = userDepartment;
                    data.topAccessibleDepartment = myTopDepartment;

                    resolve(data);
                  });
                }
              },
              (_) => {
                resolve(data);
              }
            );
          } else {
            reject();
          }
        },
        (err) => {
          reject(err);
        }
      );
    });
  }

  getPermissionsForCurrentUser(
    accessRightsRequest: AccessRightsRequest
  ): Promise<AccessRightsModel[]> {
    return this.httpHelpers
      .get<AccessRightsModel[]>(
        `${AppConstant.api.portal}/me/AccessRights`,
        accessRightsRequest
      )
      .toPromise();
  }

  getMenusByUser(): Observable<unknown> {
    return this.httpHelpers.get(`${AppConstant.api.portal}/menus`, undefined, {
      avoidIntercepterCatchError: true
    });
  }

  private getSiteDataByCurrentUser(): Observable<SiteData> {
    return this.httpHelpers.get<SiteData>(
      `${AppConstant.api.portal}/sites`,
      undefined,
      { avoidIntercepterCatchError: true }
    );
  }

  private getVerifiedUserInfo(
    externalId: string
  ): Observable<PagingResponseModel<UserManagement>> {
    const body = {
      extIds: [externalId],
      getRoles: true,
      getDepartment: true,
      getGroups: true,
      pageIndex: 1,
      pageSize: 1
    };

    const url = `${AppConstant.api.organization}/usermanagement/get_users`;

    return this.httpHelpers.post(url, body, null, {
      avoidIntercepterCatchError: true
    });
  }
}
