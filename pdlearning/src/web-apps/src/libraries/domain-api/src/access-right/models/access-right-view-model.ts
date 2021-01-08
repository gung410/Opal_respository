import { AccessRight, IAccessRight } from './access-right';

import { IPublicUserInfo } from '../../share/models/user-info.model';

export interface IAccessRightViewModel extends IAccessRight {
  collaborator: IPublicUserInfo;
}

export class AccessRightViewModel extends AccessRight {
  public type: string;
  public collaborator: IPublicUserInfo;
  public static createFromModel(accessRight: IAccessRight, collaborator: IPublicUserInfo): AccessRightViewModel {
    return new AccessRightViewModel({
      ...accessRight,
      collaborator: collaborator
    });
  }

  constructor(data?: IAccessRightViewModel) {
    super(data);
    if (data != null) {
      this.collaborator = data.collaborator;
    }
  }
}
