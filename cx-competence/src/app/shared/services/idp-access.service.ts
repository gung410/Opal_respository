import { Injectable } from '@angular/core';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { ApprovalGroup } from 'app-models/approval-group.model';
import { User } from 'app-models/auth.model';
import { environment } from 'app-environments/environment';

@Injectable({
  providedIn: 'root',
})
export class IdpAccessService {
  constructor() {}

  /**
   * Determines whether the current logged-in user (who is not the reporting officer)
   * has valid role to be able to approve or reject the LNA/PD Plan of the staff.
   * @param currentLoggedInUser The current logged-in user that is looking at the staff detail.
   */
  userCanDecideApproval(currentLoggedInUser: User): boolean {
    return (
      currentLoggedInUser.systemRoles.findIndex((role) =>
        environment.lnaResult.roleCanDecideApprovalExtIds.includes(
          role.identity.extId
        )
      ) > -1
    );
  }

  /**
   * Determines whether the staff is under of any approving officer or not.
   * @param staff The staff that is being view.
   */
  staffUnderAnyApprovingOfficers(staff: Staff): boolean {
    return staff.approvalGroups && staff.approvalGroups.length > 0;
  }

  /**
   * Determines whether the current logged-in user is either primary approving officer or alternative approving officer of the staff.
   * @param currentLoggedInUserApprovalGroups The approval groups of the current logged-in user who is looking at the staff detail.
   * @param staff The staff that is being view.
   */
  userIsApprovingOfficerOfStaff(
    currentLoggedInUserApprovalGroups: ApprovalGroup[],
    staff: Staff
  ): boolean {
    if (!currentLoggedInUserApprovalGroups) {
      return false;
    }

    return (
      staff.approvalGroups.find(
        (p) =>
          currentLoggedInUserApprovalGroups.find(
            (g) => g.identity.extId === p.identity.extId
          ) !== undefined
      ) !== undefined
    );
  }
}
