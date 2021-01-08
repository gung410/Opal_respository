import { Identity } from './common.model';
import { LocalizedData } from './localized-data.model';
import { EntityStatus } from './entity-status.model';

export class DepartmentType {
  public identity: Identity;
  public entityStatus: EntityStatus;
  public localizedData: LocalizedData[];
  constructor(data?: any) {
    if (!data) {
      return;
    }
    this.identity = data.identity;
    this.entityStatus = data.entityStatus;
    this.localizedData = data.localizedData;
  }
}
