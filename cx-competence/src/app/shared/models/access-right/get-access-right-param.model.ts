import { AccessRightModuleEnum } from './access-right-module.enum';

export class GetAccessRightParam {
  modules?: AccessRightModuleEnum[];
  actions?: string[];
  objectTypes?: string[];
  includeChildren?: boolean;
  constructor(getAccessRightParam: Partial<GetAccessRightParam>) {
    if (getAccessRightParam) {
      this.modules = getAccessRightParam.modules;
      this.actions = getAccessRightParam.actions;
      this.objectTypes = getAccessRightParam.objectTypes;
      this.includeChildren = getAccessRightParam.includeChildren;
    }
  }
}
