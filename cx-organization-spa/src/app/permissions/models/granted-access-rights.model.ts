import { GrantedType } from '../enum/granted-type.enum';

export class GrantedAccessRightsModel {
  systemRoleId: number;
  accessRightId: number;
  grantedType: GrantedType;

  constructor(data?: GrantedAccessRightsModel) {
    if (!data) {
      return;
    }

    this.systemRoleId = data.systemRoleId;
    this.accessRightId = data.accessRightId;
    this.grantedType = data.grantedType;
  }
}
