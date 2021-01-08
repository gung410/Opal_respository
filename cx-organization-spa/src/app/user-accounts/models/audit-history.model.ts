import { AuditActionType } from '../constants/user-field-mapping.constant';
import { ApprovalGroupTypeEnum } from '../constants/approval-group.enum';

export class AuditHistory {
  id: string;
  actionUserInfo: ActionUserInfo;
  actionType: AuditActionType;
  date: any;
  payload: any;
  title: string;
  info: string;
  showSubInfo: boolean;
  constructor(data?: Partial<AuditHistory>) {
    if (!data) {
      this.actionUserInfo = new ActionUserInfo();

      return;
    }
    this.actionUserInfo = data.actionUserInfo
      ? data.actionUserInfo
      : new ActionUserInfo();
    this.actionType = data.actionType;
    this.id = data.id;
    this.date = data.date;
    this.payload = data.payload;
    this.title = data.title;
    this.info = data.info;
    this.showSubInfo = data.showSubInfo;
  }
}

// tslint:disable:max-classes-per-file
export class ActionUserInfo {
  extId: string;
  firstName: string;
  avatarUrl: string;
  constructor(data?: Partial<ActionUserInfo>) {
    if (!data) {
      return;
    }
    this.extId = data.extId;
    this.firstName = data.firstName;
    this.avatarUrl = data.avatarUrl;
  }
}

export class DatahubEventModel {
  executor: AuditLogExecutor;
  approvalGroupInfo: AuditLogApprovalInfo;
  type: any;
  version: any;
  id: any;
  created: any;
  routing: DatahubRoutingModel;
  payload: DatahubPayloadModel;
}

export class AuditLogApprovalInfo {
  appprovalGroupId: number;
  fullName: string;
  type: ApprovalGroupTypeEnum;
}

export class AuditLogExecutor {
  extId: string;
  fullName: string;
  avatarUrl: string;
}

export class DatahubRoutingModel {
  action: any;
  actionVersion: any;
  entity: any;
  entityId: any;
}

export class DatahubPayloadModel {
  identity: DatahubIdentityModel;
  references: DatahubReferenceModel;
  body: DatahubBodyModel;
}

export class DatahubBodyModel {
  userData: any;
  departmentId: any;
  departmentArcheTypeId: any;
  userId: any;
  userArcheTypeId: any;
  memberData: any;
  userGroupId: number;
}

export class DatahubReferenceModel {
  externalId: any;
  correlationId: any;
  commandId: any;
  eventId: any;
}

export class DatahubIdentityModel {
  clientId: any;
  customerId: any;
  sourceIp: any;
  userId: any;
  onBehalfOfUser: any;
}
