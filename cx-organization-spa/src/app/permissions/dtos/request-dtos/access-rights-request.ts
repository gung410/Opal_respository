import { ModuleType } from 'app/permissions/enum/module-type.enum';
import { ObjectType } from '../../enum/object-type.enum';

export class AccessRightsRequest {
  objectTypes?: ObjectType[];
  modules?: ModuleType[];
  parentAccessRightIds?: number[];
  accessRightIds?: number[];
  systemRoleIds?: number[];
  actions?: string[];
  includeChildren?: boolean;
  includeLocalizedData?: boolean;

  constructor(data: Partial<AccessRightsRequest>) {
    if (!data) {
      return;
    }

    this.objectTypes = data.objectTypes;
    this.parentAccessRightIds = data.parentAccessRightIds;
    this.accessRightIds = data.accessRightIds;
    this.systemRoleIds = data.systemRoleIds;
    this.actions = data.actions;
    this.modules = data.modules;
    this.includeChildren = data.includeChildren;
    this.includeLocalizedData = data.includeLocalizedData;
  }
}
