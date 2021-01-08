import { EntityStatus } from './entity-status.model';
import { Identity } from './identity.model';
import { LocalizedDataItem } from './localized-data-item.model';

export class DepartmentType {
  identity: Identity;
  entityStatus: EntityStatus;
  localizedData: LocalizedDataItem[];
  constructor(data?: any) {
    if (!data) {
      return;
    }
    this.identity = data.identity;
    this.entityStatus = data.entityStatus;
    this.localizedData = data.localizedData;
  }
}
