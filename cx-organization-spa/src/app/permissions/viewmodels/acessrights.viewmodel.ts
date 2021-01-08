import { AccessRightsModel } from '../models/access-rights.model';

export class AccessRightsViewModel extends AccessRightsModel {
  dataName?: string;

  constructor(data?: AccessRightsModel) {
    super(data);
    if (!data) {
      return;
    }

    this.dataName = data.localizedData[0].fields[0].localizedText;
  }
}
