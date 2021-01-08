import { ISession, Session, UserInfoModel } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface ISessionViewModel extends ISession {
  selected: boolean;
}

// @dynamic
export class SessionViewModel extends Session implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public facilitators: UserInfoModel[];
  public static createFromModel(session: Session, checkAll: boolean = false, selecteds: Dictionary<boolean> = {}): SessionViewModel {
    return new SessionViewModel({
      ...session,
      selected: checkAll || selecteds[session.id]
    });
  }

  constructor(data?: ISessionViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
    }
  }
}
