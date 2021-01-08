import { Injectable } from '@angular/core';
import { ApprovalGroup } from 'app-models/approval-group.model';
import {
  GetUserGroupParamDTO,
  UserGroupDTO,
} from 'app-models/user-group.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { APIConstant, AppConstant } from 'app/shared/app.constant';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GroupStoreService {
  constructor(private httpHelper: HttpHelpers) {}

  /**
   * Get approval groups by the owner of the groups.
   * @param approverId The identity of the current logged-in user who is the owner of the approval groups.
   */
  getApprovalGroups(approverId: number): Observable<ApprovalGroup[]> {
    return this.httpHelper.get<ApprovalGroup[]>(
      `${AppConstant.api.organization}/approvalgroups`,
      {
        approverIds: approverId,
      }
    );
  }

  /**
   * Gets user groups by department identifier.
   * @param departmentId The department identifier which the user groups belonging to.
   * @param countActiveMembers Indicating the result should include the counting number of the member in each group.
   * @param orderBy The sorting field. Default is "name".
   */
  getUserGroupsByDepartmentId(
    departmentId: number,
    countActiveMembers: boolean = true,
    orderBy: string = 'name'
  ): Observable<PagingResponseModel<UserGroupDTO>> {
    const param: GetUserGroupParamDTO = {
      departmentIds: departmentId,
      countActiveMembers,
      orderBy,
    };
    const url = APIConstant.GET_USER_GROUP;

    return this.httpHelper.get<PagingResponseModel<UserGroupDTO>>(url, param);
  }
}
