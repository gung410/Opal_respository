import {
  AppsSwitcherItem,
  CxNavbarItemModel,
  MultipleLanguages,
  NotificationItem
} from '@conexus/cx-angular-common';

export class Header {
  menus: CxNavbarItemModel[];
  notifications: NotificationItem[];
  applications: AppsSwitcherItem[];
  logo: any;
  title: string;
  multipleLanguages: MultipleLanguages;
}
