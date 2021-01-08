import { LocalizedDataItem } from 'app-models/localized-data-item.model';
import { SystemRolePermissionSubject } from './system-role-permission-subject.model';

export interface ISystemRoleSubjects {
  id: number;
  extId?: string;
  systemRolePermissionSubjects?: SystemRolePermissionSubject[];
  isDefaultSystemRole?: boolean;
  localizedData?: LocalizedDataItem[];
}

export class SystemRoleSubjects implements ISystemRoleSubjects {
  id: number;
  extId?: string;
  isDefaultSystemRole?: boolean;
  systemRolePermissionSubjects?: SystemRolePermissionSubject[];
  localizedData?: LocalizedDataItem[];

  constructor(data?: ISystemRoleSubjects) {
    if (!data) {
      return;
    }

    this.id = data.id;
    this.extId = data.extId;
    this.isDefaultSystemRole = data.isDefaultSystemRole;
    this.localizedData = data.localizedData;
    this.systemRolePermissionSubjects = data.systemRolePermissionSubjects;
  }
}
