import { ObjectType } from '../../enum/object-type.enum';

export class AccessRightsMatrixModelRequest {
  objectTypes?: ObjectType[];
  parentAccessRightIds?: number[];
  accessRightIds?: number[];
  systemRoleIds?: number[];
  actions?: string[];
  includeChildren?: boolean;
  includeAccessRights?: boolean;
  includeSystemRoles?: boolean;
  includeLocalizedData?: boolean;

  constructor(data: Partial<AccessRightsMatrixModelRequest>) {
    if (!data) {
      return;
    }

    this.objectTypes = data.objectTypes;
    this.parentAccessRightIds = data.parentAccessRightIds;
    this.accessRightIds = data.accessRightIds;
    this.systemRoleIds = data.systemRoleIds;
    this.actions = data.actions;
    this.includeChildren = data.includeChildren;
    this.includeAccessRights = data.includeAccessRights;
    this.includeSystemRoles = data.includeSystemRoles;
    this.includeLocalizedData = data.includeLocalizedData;
  }
}
