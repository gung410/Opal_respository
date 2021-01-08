import { IEntityStatusModel, IIdentityModel } from '../../share/models/identity.model';

import { ILocalizedData } from './../../share/models/localized-data.model';

export interface IDepartmentLevelModel {
  entityStatus: IEntityStatusModel;
  identity: IIdentityModel;
  localizedData: ILocalizedData[];
}

export class DepartmentLevelModel {
  public id: number = 0;
  public extId: string = '';
  public departmentLevelName: string = '';

  constructor(data?: IDepartmentLevelModel) {
    if (data != null) {
      this.id = data.identity.id;
      this.extId = data.identity.extId;
      this.departmentLevelName =
        data.localizedData != null && data.localizedData.length > 0 && data.localizedData[0].fields.length > 0
          ? data.localizedData[0].fields[0].localizedText
          : '';
    }
  }
}
