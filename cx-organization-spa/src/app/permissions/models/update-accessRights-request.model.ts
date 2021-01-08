import { AccessRightsModel } from './access-rights.model';

export interface IUpdateAccessRightsRequest {
  systemRoleId?: number;
  accessRights?: AccessRightsModel[];
}

export class UpdateAccessRightsRequest implements IUpdateAccessRightsRequest {
  systemRoleId?: number;
  accessRights?: AccessRightsModel[] = [];

  constructor(data?: Partial<IUpdateAccessRightsRequest>) {
    if (!data) {
      return;
    }

    this.systemRoleId = data.systemRoleId;
    this.accessRights = data.accessRights;
  }
}
