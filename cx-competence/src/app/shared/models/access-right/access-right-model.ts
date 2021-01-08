import { AccessRightModuleEnum } from './access-right-module.enum';
import { GrantedTypeEnum } from './granted-type.enum';

export class AccessRight {
  id: number;
  action?: string;
  module: AccessRightModuleEnum;
  grantedType: GrantedTypeEnum | string;
  parentId?: number;
}
