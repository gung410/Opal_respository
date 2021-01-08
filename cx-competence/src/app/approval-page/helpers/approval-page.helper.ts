import { User } from 'app-models/auth.model';
import { ChangeStatusNominateRequestPayload } from 'app-models/mpj/assign-pdo.model';
import {
  ApprovalTargetEnum,
  ChangeNominationStatusTargetEnum,
} from '../models/approval.enum';
import {
  ApprovalConfirmMessage,
  ApprovalConstants,
} from './approval-page.constant';

export class ApprovalPageHelper {
  static getApproveConfirmMessage(approvalTarget: ApprovalTargetEnum): string {
    return ApprovalConfirmMessage.APPROVE_CONFIRM_MESSAGE_MAP[approvalTarget];
  }

  static getRejectConfirmMessage(approvalTarget: ApprovalTargetEnum): string {
    return ApprovalConfirmMessage.REJECT_CONFIRM_MESSAGE_MAP[approvalTarget];
  }

  static buildChangeNominationStatusPayload(
    resultIds: number[],
    changeNominationStatusTarget: ChangeNominationStatusTargetEnum
  ): ChangeStatusNominateRequestPayload {
    const targetStatus =
      ApprovalConstants.CHANGE_NOMINATION_STATUS_TARGET_MAP[
        changeNominationStatusTarget
      ];

    return {
      target: changeNominationStatusTarget,
      resultIds,
      changePDOpportunityStatus: targetStatus,
    };
  }

  static hasReviewPermisson(
    target: ApprovalTargetEnum,
    currentUser: User
  ): boolean {
    if (!target || !currentUser) {
      return false;
    }
    const requiredPermisson =
      ApprovalConstants.APPROVAL_TARGET_ACTIONKEY_MAP[target];

    return currentUser.hasPermission(requiredPermisson);
  }
}
