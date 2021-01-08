import { Identity } from 'app-models/common.model';

export class UserGroupModel {
  name: string;
  identity: Identity;
  memberCount: number;
  description: string;
  departmentId: number;
  constructor(userGroupDTO: UserGroupDTO) {
    if (!userGroupDTO) {
      return;
    }
    this.identity = userGroupDTO.identity;
    this.departmentId = userGroupDTO.departmentId;
    this.memberCount = userGroupDTO.memberCount;
    this.name = userGroupDTO.name;
    this.description = userGroupDTO.description;
  }
}

export class UserGroupDTO {
  name: string;
  identity: Identity;
  memberCount: number;
  description: string;
  departmentId: number;
}

export interface GetUserGroupParamDTO {
  departmentIds: number;
  countActiveMembers: boolean;
  orderBy: string;
}
