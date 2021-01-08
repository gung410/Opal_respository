import { EntityStatus } from 'app-models/entity-status.model';
import { Identity } from 'app-models/identity.model';
import { Period } from 'app-models/period.model';

// tslint:disable:max-classes-per-file
export class UserGroupFilterParams {
  userPoolIds?: number[];
  departmentIds?: number[];
  poolOwnerUserIds?: number[];
  memberUserIds?: number[];
  statusEnums?: string[];
  countActiveMembers?: boolean;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;

  constructor(data?: UserGroupFilterParams) {
    if (data == null) {
      return;
    }
    this.userPoolIds = data.userPoolIds;
    this.departmentIds = data.departmentIds;
    this.poolOwnerUserIds = data.poolOwnerUserIds;
    this.memberUserIds = data.memberUserIds;
    this.statusEnums = data.statusEnums;
    this.countActiveMembers = data.countActiveMembers;
    this.pageIndex = data.pageIndex;
    this.pageSize = data.pageSize;
    this.orderBy = data.orderBy;
  }
}

export class UserGroupDto {
  departmentId?: number;
  userId?: number;
  name?: string;
  type?: string;
  description?: string;
  identity?: Identity;
  entityStatus?: EntityStatus;
  referrerToken?: string;
  referrerResource?: string;
  memberCount?: number;
  dynamicAttributes?: any[];
  users?: any[];
  constructor(data?: Partial<UserGroupDto>) {
    if (!data) {
      return;
    }
    this.departmentId = data.departmentId;
    this.userId = data.userId;
    this.name = data.name;
    this.description = data.description;
    this.memberCount = data.memberCount;
    this.identity = data.identity;
    this.entityStatus = data.entityStatus;
    this.dynamicAttributes = data.dynamicAttributes
      ? data.dynamicAttributes
      : [];
    this.users = data.users ? data.users : [];
  }
}

export class MembershipDto {
  validTo: string;
  validFrom: string;
  memberRoleId: number;
  created: string;
  createdBy: number;
  role: string;
  groupId: number;
  memberId: number;
  displayName: string;
  period: Period;
  referrerToken: string;
  referrerResource: string;
  referrerArchetypeId: number;
  periodId: number;
  identity: Identity;
  dynamicAttributes: any[];
  entityStatus: EntityStatus;
  constructor(data?: Partial<MembershipDto>) {
    if (!data) {
      return;
    }
    this.validTo = data.validTo;
    this.validFrom = data.validFrom;
    this.memberRoleId = data.memberRoleId;
    this.created = data.created;
    this.createdBy = data.createdBy;
    this.role = data.role;
    this.groupId = data.groupId;
    this.memberId = data.memberId;
    this.displayName = data.displayName;
    this.period = data.period;
    this.referrerToken = data.referrerToken;
    this.referrerResource = data.referrerResource;
    this.referrerArchetypeId = data.referrerArchetypeId;
    this.periodId = data.periodId;
    this.identity = data.identity;
    this.dynamicAttributes = data.dynamicAttributes;
    this.entityStatus = data.entityStatus;
  }
}
