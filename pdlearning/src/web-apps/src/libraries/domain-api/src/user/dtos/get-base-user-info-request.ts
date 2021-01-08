import { SystemRoleEnum } from '../../share/models/user-info.model';

export interface IBaseUserInfoRequest {
  userIds?: string[];
  extIds?: string[];
  emails?: string[];
  entityStatuses?: string[];
  departmentIds?: number[];
  getFullIdentity?: boolean;
  getEntityStatus?: boolean;
  userTypeExtIds?: SystemRoleEnum[];
  pageSize: number;
  pageIndex: number;
  searchKey?: string;
  orderBy?: string;
  systemRolePermissions?: string[];
}
