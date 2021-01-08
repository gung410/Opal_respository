import { LocalizedDataItem } from 'app-models/localized-data-item.model';
import { AccessRightsModel } from 'app/permissions/models/access-rights.model';
import { SystemRolePermissionSubject } from '../models/system-role-permission-subject.model';

export class CreateSystemRoleRequest {
  isDefaultSystemRole?: boolean;
  localizedData?: LocalizedDataItem[];
  systemRoleTemplates?: number[];
  accessRights?: AccessRightsModel[];
  systemRolePermissionSubjects?: SystemRolePermissionSubject[];

  constructor(data?: CreateSystemRoleRequest) {
    if (!data) {
      return;
    }

    this.isDefaultSystemRole = data.isDefaultSystemRole;
    this.localizedData = data.localizedData;
    this.systemRoleTemplates = data.systemRoleTemplates;
    this.accessRights = data.accessRights;
    this.systemRolePermissionSubjects = data.systemRolePermissionSubjects;
  }
}
