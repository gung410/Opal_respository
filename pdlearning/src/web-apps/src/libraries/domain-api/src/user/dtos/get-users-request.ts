import { OrganizationUnitLevelEnum } from '../../organization/models/department-info.model';
import { SystemRoleEnum } from '../../share/models/user-info.model';

export interface IGetUsersRequest {
  userIds?: string[];
  parentDepartmentId?: number[];
  jsonDynamicData?: string[];
  inRoles?: SystemRoleEnum[];
  filterOnSubDepartment?: boolean;
  pageSize: number;
  pageIndex: number;
  searchKey?: string;
  departmentExtIds?: OrganizationUnitLevelEnum[];
  userEntityStatuses?: string[];
  systemRolePermissions?: string[];
}
