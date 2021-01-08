import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { BaseUserInfo, IPublicUserInfo, PublicUserInfo, UserInfoModel } from '../../share/models/user-info.model';
import { BaseUserInfoWithCheckMoreData, IBaseUserInfoResult, IUserInfoListResult } from '../dtos/get-user-result.dto';

import { AuthService } from '@opal20/authentication';
import { IBaseUserInfoRequest } from '../dtos/get-base-user-info-request';
import { IGetUsersRequest } from '../dtos/get-users-request';
import { IUserTagModel } from '../models/user-tag.model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PublicUserRequest } from '../dtos/get-public-user-info.dto';
import { map } from 'rxjs/operators';

@Injectable()
export class UserApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.authConfig.organizationUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService, private authService: AuthService) {
    super(commonFacadeService);
  }

  public getUserTags(showSpinner: boolean = true): Promise<string[]> {
    const userId = this.authService.User.extId;
    return this.get<IUserTagModel>(`/userInfo/${userId}`, undefined, showSpinner)
      .pipe(map(result => result.tagIds))
      .toPromise();
  }

  public getUserInfoList(
    request: IGetUsersRequest,
    additionalStatuses: string[] = [],
    showSpinner: boolean = true
  ): Observable<UserInfoModel[]> {
    const defaultStatuses = request.userEntityStatuses ? request.userEntityStatuses : ['Active', 'New'];
    return this.post<unknown, IUserInfoListResult>(
      `/usermanagement/get_users`,
      {
        extIds: request.userIds,
        parentDepartmentId: request.parentDepartmentId,
        jsonDynamicData: request.jsonDynamicData ? request.jsonDynamicData : ['avatarUrl'],
        userEntityStatuses: additionalStatuses ? additionalStatuses.concat(defaultStatuses) : defaultStatuses,
        getRoles: true,
        pageSize: request.pageSize,
        pageIndex: request.pageIndex,
        multiUserTypeExtIdFilters: request.inRoles ? [request.inRoles] : null,
        getGroups: true,
        filterOnSubDepartment: request.filterOnSubDepartment,
        searchKey: request.searchKey,
        departmentExtIds: request.departmentExtIds ? request.departmentExtIds : null,
        systemRolePermissions: request.systemRolePermissions
      },
      showSpinner
    ).pipe(map(result => (result && result.items ? result.items.map(_ => new UserInfoModel(_)) : [])));
  }

  public getPublicUserInfoList(request: PublicUserRequest, showSpinner: boolean = true): Observable<PublicUserInfo[]> {
    return this.post<unknown, IPublicUserInfo[]>(`/userInfo/public`, { userCxIds: request.userIds }, showSpinner).pipe(
      map(users => (users ? users.map(user => new PublicUserInfo(user)) : []))
    );
  }

  public getBaseUserInfoList(request: IBaseUserInfoRequest, showSpinner: boolean = true): Observable<BaseUserInfo[]> {
    return this.post<unknown, IBaseUserInfoResult>(`/userinfo/basic`, request, showSpinner).pipe(
      map(result => (result && result.items ? result.items.map(_ => new BaseUserInfo(_)) : []))
    );
  }

  public getBaseUserInfoListResult(request: IBaseUserInfoRequest, showSpinner: boolean = true): Observable<BaseUserInfoWithCheckMoreData> {
    return this.post<unknown, IBaseUserInfoResult>(`/userinfo/basic`, request, showSpinner).pipe(
      map(_ => new BaseUserInfoWithCheckMoreData(_))
    );
  }
}
