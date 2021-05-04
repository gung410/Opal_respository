import { UserTypeEnum } from 'app/shared/constants/user-type.enum';

import { EntityStatus } from '../../shared/models/entity-status.model';
import { Identity } from '../../shared/models/identity.model';
import { ApprovalGroupTypeEnum } from '../constants/approval-group.enum';

// tslint:disable:max-classes-per-file
export class ApprovalGroup {
  departmentId: number;
  approverId: number;
  name: string;
  email: string;
  type: ApprovalGroupTypeEnum;
  description: string;
  referrerToken: string;
  referrerResource: string;
  identity: Identity;
  entityStatus: EntityStatus;
  jsonDynamicAttributes: any;
  constructor(data?: Partial<ApprovalGroup>) {
    if (!data) {
      this.identity = new Identity();
      this.identity.archetype = UserTypeEnum.ApprovalGroup;
      this.entityStatus = new EntityStatus();

      return;
    }
    this.approverId = data.approverId;
    this.departmentId = data.departmentId;
    this.name = data.name ? data.name : '';
    this.type = data.type
      ? data.type
      : ApprovalGroupTypeEnum.PrimaryApprovalGroup;
    this.description = data.description ? data.description : '';
    this.referrerToken = data.referrerToken ? data.referrerToken : '';
    this.referrerResource = data.referrerResource ? data.referrerResource : '';
    this.jsonDynamicAttributes = data.jsonDynamicAttributes
      ? data.jsonDynamicAttributes
      : '';
    this.identity = data.identity ? data.identity : new Identity();
    if (!this.identity.archetype) {
      this.identity.archetype = UserTypeEnum.ApprovalGroup;
    }
    this.entityStatus = data.entityStatus
      ? data.entityStatus
      : new EntityStatus();
  }
}

export class GetApprovalGroupModel {
  parentDepartmentId: number;
  approvalGroupIds: number[] = [];
  approverIds: number[] = [];
  employeeIds: number[] = [];
  statusEnums: string[];
  groupTypes: string[];
  extIds: string[];
  lastUpdatedBefore: string;
  lastUpdatedAfter: string;
  selectIdentity: boolean;
  pageIndex: number;
  pageSize: number;
  orderBy: string;
}
export class ApprovalGroupQueryModel {
  parentDepartmentId: number;
  approvalGroupIds: number[] = [];
  approverIds: number[] = [];
  employeeIds: number[] = [];
  statusEnums: string[] = [];
  groupTypes: string[] = [];
  extIds: string[] = [];
  lastUpdatedBefore: any;
  lastUpdatedAfter: any;
  selectIdentity: boolean;
  pageIndex: number;
  pageSize: number;
  orderBy: string;
  searchKey: string;
  searchInSameDepartment: boolean;
  searchFromDepartmentToTop: boolean;
  assigneeDepartmentId: number;
  constructor(data?: Partial<ApprovalGroupQueryModel>) {
    if (!data) {
      return;
    }
    this.parentDepartmentId = data.parentDepartmentId;
    this.approvalGroupIds = data.approvalGroupIds;
    this.approverIds = data.approverIds;
    this.employeeIds = data.employeeIds;
    this.statusEnums = data.statusEnums;
    this.groupTypes = data.groupTypes;
    this.extIds = data.extIds;
    this.lastUpdatedBefore = data.lastUpdatedBefore;
    this.lastUpdatedAfter = data.lastUpdatedAfter;
    this.selectIdentity = data.selectIdentity;
    this.pageIndex = data.pageIndex;
    this.pageSize = data.pageSize;
    this.orderBy = data.orderBy;
    this.searchKey = data.searchKey;
    this.searchInSameDepartment = data.searchInSameDepartment;
    this.searchFromDepartmentToTop = data.searchFromDepartmentToTop;
    this.assigneeDepartmentId = data.assigneeDepartmentId;
  }
}
