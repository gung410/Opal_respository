import { LocalizedDataItem } from 'app-models/localized-data-item.model';
import { SystemRolePermissionSubject } from '../models/system-role-permission-subject.model';

export class UpdateSystemRoleInfoRequest {
  id: number;
  extId?: string;
  systemRolePermissionSubjects: SystemRolePermissionSubject[];
  localizedData: LocalizedDataItem[];

  constructor(data?: UpdateSystemRoleInfoRequest) {
    if (!data) {
      return;
    }

    this.id = data.id;
    this.extId = data.extId;
    this.systemRolePermissionSubjects = data.systemRolePermissionSubjects;
  }
}
