import {
  CxNavbarItemModel,
  NotificationItem,
  AppsSwitcherItem,
} from '@conexus/cx-angular-common';

export class Header {
  public menus: Array<CxNavbarItemModel>;
  public notifications: Array<NotificationItem>;
  public applications: Array<AppsSwitcherItem>;
  public logo: any;
  public title: string;
  constructor() {}
}
