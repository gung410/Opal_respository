import { ApprovalGroup } from '../models/approval-group.model';
import { EditUser } from '../models/edit-user.model';

// tslint:disable:max-classes-per-file
export class MemberApprovalGroupModel {
  approvalGroup: ApprovalGroup;
  previousMemberIds: number[];
  currentMemberIds: number[];
  constructor(data?: Partial<MemberApprovalGroupModel>) {
    if (!data) {
      return;
    }
    this.approvalGroup = data.approvalGroup ? data.approvalGroup : undefined;
    this.previousMemberIds = data.previousMemberIds
      ? data.previousMemberIds
      : [];
    this.currentMemberIds = data.currentMemberIds ? data.currentMemberIds : [];
  }
}

export class ApprovalInfoTabModel {
  primaryApprovalGroup: ApprovalGroup;
  alternateApprovalGroup: ApprovalGroup;
  memberOfPrimaryApprovalGroup: MemberApprovalGroupModel;
  memberOfAlternateApprovalGroup: MemberApprovalGroupModel;
  constructor(data?: Partial<ApprovalInfoTabModel>) {
    if (!data) {
      return;
    }
    this.primaryApprovalGroup = data.primaryApprovalGroup
      ? data.primaryApprovalGroup
      : undefined;
    this.alternateApprovalGroup = data.alternateApprovalGroup
      ? data.alternateApprovalGroup
      : undefined;
    this.memberOfPrimaryApprovalGroup = data.memberOfPrimaryApprovalGroup
      ? data.memberOfPrimaryApprovalGroup
      : undefined;
    this.memberOfAlternateApprovalGroup = data.memberOfAlternateApprovalGroup
      ? data.memberOfAlternateApprovalGroup
      : undefined;
  }
}

export class EditUserDialogSubmitModel {
  userData: EditUser;
  approvalData: ApprovalInfoTabModel;
  constructor(data?: Partial<EditUserDialogSubmitModel>) {
    if (!data) {
      this.approvalData = new ApprovalInfoTabModel();

      return;
    }
    this.userData = data.userData;
    this.approvalData = data.approvalData;
  }
}

export enum EditUserDialogModeEnum {
  Edit = 'edit',
  Create = 'create',
  View = 'view'
}
