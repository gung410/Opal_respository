import { Utils } from '../../shared/utilities/utils';
import { AccessRightsModel } from './access-rights.model';
import { GrantedAccessRightsModel } from './granted-access-rights.model';
import { SystemRoleModel } from './system-role.model';

export class AccessRightsMatrixModel {
  systemRoles: SystemRoleModel[];
  accessRights: AccessRightsModel[];
  grantedAccessRights: GrantedAccessRightsModel[];

  constructor(data?: AccessRightsMatrixModel) {
    if (!data) {
      return;
    }

    this.accessRights = Utils.cloneDeep(data.accessRights);
    this.systemRoles = Utils.cloneDeep(data.systemRoles);
    this.grantedAccessRights = Utils.cloneDeep(data.grantedAccessRights);
  }
}
