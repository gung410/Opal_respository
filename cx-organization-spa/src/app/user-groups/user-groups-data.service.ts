import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { PagingResponseModel } from 'app/user-accounts/models/user-management.model';
import { Observable } from 'rxjs';

import {
  MembershipDto,
  UserGroupDto,
  UserGroupFilterParams
} from './user-groups.model';

@Injectable({
  providedIn: 'root'
})
export class UserGroupsDataService {
  constructor(private httpHelper: HttpHelpers) {}

  getUserGroups(
    filterParamModel: UserGroupFilterParams
  ): Observable<PagingResponseModel<UserGroupDto>> {
    return this.httpHelper.get<PagingResponseModel<UserGroupDto>>(
      this.getUserGroupUrl(),
      filterParamModel
    );
  }

  createUserGroup(userGroupDTO: UserGroupDto): Observable<UserGroupDto> {
    return this.httpHelper.post(this.getUserGroupUrl(), userGroupDTO);
  }

  updateUserGroup(userGroupDTO: UserGroupDto): Observable<UserGroupDto> {
    return this.httpHelper.put(
      `${this.getUserGroupUrl()}/${userGroupDTO.identity.id}`,
      userGroupDTO
    );
  }

  deleteUserGroup(userPoolId: number): Observable<UserGroupDto> {
    return this.httpHelper.delete(
      `${this.getUserGroupUrl()}/${userPoolId}`,
      userPoolId
    );
  }

  addMembersToUserGroup(
    userPoolId: number,
    members: MembershipDto[]
  ): Observable<MembershipDto> {
    return this.httpHelper.post(
      `${this.getUserGroupUrl()}/${userPoolId}/members`,
      members
    );
  }

  getMembers(userGroupId: number): Observable<any[]> {
    return this.httpHelper.get<any[]>(
      `${this.getUserGroupUrl()}/${userGroupId}/members`
    );
  }

  deleteMember(userPoolId: number, members: MembershipDto[]): Observable<any> {
    return this.httpHelper.delete(
      `${this.getUserGroupUrl()}/${userPoolId}/members`,
      members
    );
  }

  private getUserGroupUrl(): string {
    return `${AppConstant.api.organization}/userpools`;
  }
}
