import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { switchMap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { Observable, of } from 'rxjs';

import {
  ApprovalGroup,
  ApprovalGroupQueryModel
} from '../models/approval-group.model';
import {
  UserManagement,
  UserManagementQueryModel
} from '../models/user-management.model';

@Injectable()
export class ApprovalDataService {
  constructor(
    private httpHelper: HttpHelpers,
    private userAccountDataService: UserAccountsDataService
  ) {}
  getApprovalGroupsUserApprovesFor(
    userId: number
  ): Observable<ApprovalGroup[]> {
    return this.httpHelper.get<ApprovalGroup[]>(
      `${AppConstant.api.organization}/approvalgroups`,
      {
        approverIds: userId.toString(),
        pageIndex: '1',
        pageSize: '2'
      }
    );
  }

  getApprovalGroupMembers(
    approvalGroupId: number
  ): Observable<UserManagement[]> {
    return this.httpHelper.get<UserManagement[]>(
      `${AppConstant.api.organization}/approvalgroups/${approvalGroupId}/members`
    );
  }

  getApprovalGroups(
    params: ApprovalGroupQueryModel
  ): Observable<ApprovalGroup[]> {
    return this.httpHelper.get<ApprovalGroup[]>(
      `${AppConstant.api.organization}/approvalgroups`,
      params
    );
  }
}
