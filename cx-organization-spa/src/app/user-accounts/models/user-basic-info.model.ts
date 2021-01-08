import { EntityStatus } from 'app-models/entity-status.model';
import { Identity } from 'app-models/identity.model';

export interface IGetUserBasicInfoParameters {
  userIds?: string[];
  extIds?: string[];
}

export interface UserBasicInfo {
  departmentId: number;
  departmentName: string;
  avatarUrl: string;
  emailAddress: string;
  firstName: string;
  lastName: string;
  userCxId: string;
  fullName: string;
  identity: Identity;
  dynamicAttributes: any[];
  entityStatus: EntityStatus;
}
