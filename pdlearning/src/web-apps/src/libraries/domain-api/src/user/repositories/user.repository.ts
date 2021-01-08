import { BaseUserInfo, PublicUserInfo, UserInfoModel } from '../../share/models/user-info.model';
import { Observable, of } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { BaseUserInfoWithCheckMoreData } from '../dtos/get-user-result.dto';
import { IBaseUserInfoRequest } from '../dtos/get-base-user-info-request';
import { IGetUsersRequest } from '../dtos/get-users-request';
import { Injectable } from '@angular/core';
import { PublicUserRequest } from '../dtos/get-public-user-info.dto';
import { UserApiService } from './../services/user-api.service';
import { UserRepositoryContext } from '../user-repository-context';
import { map } from 'rxjs/operators';

@Injectable()
export class UserRepository extends BaseRepository<UserRepositoryContext> {
  constructor(context: UserRepositoryContext, private apiSvc: UserApiService) {
    super(context);
  }

  public loadUserInfoList(
    request: IGetUsersRequest,
    asRequest?: boolean,
    additionalStatuses: string[] = [],
    showSpinner: boolean = true
  ): Observable<UserInfoModel[]> {
    return this.processUpsertData(
      this.context.usersSubject,
      implicitLoad => this.apiSvc.getUserInfoList(request, additionalStatuses, !implicitLoad && showSpinner),
      'loadUserInfoList',
      [request],
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.id,
      null,
      asRequest
    );
  }

  public loadMOEOfficers(parentDepartmentIds: number[], asRequest?: boolean, showSpinner: boolean = true): Observable<UserInfoModel[]> {
    return this.loadUserInfoList({ parentDepartmentId: parentDepartmentIds, pageIndex: 0, pageSize: 0 }, asRequest, [], showSpinner).pipe(
      map(_ => {
        try {
          return [UserInfoModel.getMyUserInfo()].concat(_);
        } catch (error) {
          return _;
        }
      })
    );
  }

  public loadPublicUserInfoList(request: PublicUserRequest, showSpinner: boolean = true): Observable<PublicUserInfo[]> {
    return this.processUpsertData(
      this.context.publicUsersSubject,
      implicitLoad => (request.userIds.length > 0 ? this.apiSvc.getPublicUserInfoList(request, !implicitLoad && showSpinner) : of([])),
      'loadPublicUserInfoList',
      [request],
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadBaseUserInfoList(request: IBaseUserInfoRequest, showSpinner: boolean = true): Observable<BaseUserInfo[]> {
    return this.processUpsertData(
      this.context.baseUserInfoSubject,
      implicitLoad => this.apiSvc.getBaseUserInfoList(request, !implicitLoad && showSpinner),
      'loadBaseUserInfoList',
      [
        request.userIds,
        request.extIds,
        request.emails,
        request.entityStatuses,
        request.departmentIds,
        request.getFullIdentity,
        request.getEntityStatus,
        request.userTypeExtIds,
        request.pageSize,
        request.pageIndex,
        request.searchKey,
        request.orderBy
      ],
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadBaseUserInfoListResult(request: IBaseUserInfoRequest, showSpinner: boolean = true): Observable<BaseUserInfoWithCheckMoreData> {
    return this.processUpsertData(
      this.context.baseUserInfoResultSubject,
      implicitLoad => this.apiSvc.getBaseUserInfoListResult(request, !implicitLoad && showSpinner),
      'loadBaseUserInfoList',
      [
        request.userIds,
        request.extIds,
        request.emails,
        request.entityStatuses,
        request.departmentIds,
        request.getFullIdentity,
        request.getEntityStatus,
        request.userTypeExtIds,
        request.pageSize,
        request.pageIndex,
        request.searchKey,
        request.orderBy
      ],
      'implicitReload',
      (repoData, apiResult) => apiResult,
      apiResult => apiResult.items,
      x => x.id,
      null
    );
  }
}
