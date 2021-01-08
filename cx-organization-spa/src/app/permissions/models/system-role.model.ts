import { LocalizedDataItem } from 'app-models/localized-data-item.model';
import { Utils } from '../../shared/utilities/utils';

export class SystemRoleModel {
  id: number; //Read-only
  extId: string; //Read-only
  isDefaultSystemRole: boolean; //Default system roles(System admin, user account admin,..) will be true
  localizedData: LocalizedDataItem[];

  constructor(data?: SystemRoleModel) {
    if (!data) {
      return;
    }

    this.id = data.id;
    this.extId = data.extId;
    this.isDefaultSystemRole = data.isDefaultSystemRole;
    this.localizedData = Utils.cloneDeep(data.localizedData);
  }
}
