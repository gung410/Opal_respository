import { AccessRightsModel } from './access-rights.model';

export class AccessRightsLevel extends AccessRightsModel {
  level: number;

  constructor(data?: AccessRightsLevel) {
    super(data);

    this.level = data.level;
  }
}
