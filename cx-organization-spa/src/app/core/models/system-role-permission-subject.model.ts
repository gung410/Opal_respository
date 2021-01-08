import { LocalizedDataItem } from 'app-models/localized-data-item.model';

export interface ISystemRolePermissionSubject {
  granted: Granted;
  id: number;
  exId?: string;
  isDefaultSystemRole?: boolean;
  localizedData?: LocalizedDataItem[];
}

export class SystemRolePermissionSubject
  implements ISystemRolePermissionSubject {
  granted: Granted;
  id: number;
  exId?: string;
  isDefaultSystemRole?: boolean;
  localizedData?: LocalizedDataItem[];

  constructor(data?: ISystemRolePermissionSubject) {
    if (!data) {
      return;
    }

    this.granted = data.granted;
    this.id = data.id;
    this.exId = data.exId;
    this.isDefaultSystemRole = data.isDefaultSystemRole;
    this.localizedData = data.localizedData;
  }
}

export enum Granted {
  Read = 'Read',
  Deny = 'Deny',
  Full = 'Full'
}
