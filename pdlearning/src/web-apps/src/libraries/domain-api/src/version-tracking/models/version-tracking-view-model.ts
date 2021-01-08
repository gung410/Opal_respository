import { IVersionTracking, VersionTracking } from './version-tracking';

import { PublicUserInfo } from '../../share/models/user-info.model';

export interface IVersionTrackingViewModel extends IVersionTracking {
  changedByUser: PublicUserInfo;
}

export class VersionTrackingViewModel extends VersionTracking {
  public type: string;
  public changedByUser: PublicUserInfo;
  public static createFromModel(versionTracking: IVersionTracking, changedByUser: PublicUserInfo): VersionTrackingViewModel {
    return new VersionTrackingViewModel({
      ...versionTracking,
      changedByUser: changedByUser
    });
  }

  constructor(data?: IVersionTrackingViewModel) {
    super(data);
    if (data != null) {
      this.changedByUser = data.changedByUser;
    }
  }
}
