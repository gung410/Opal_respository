import { EventEmitter, Injectable } from '@angular/core';

import { INavigationMenuItem } from '../../models/navigation-menu.model';

@Injectable()
export class NavigationMenuService {
  public items: INavigationMenuItem[] = [];
  public onHide: EventEmitter<void> = new EventEmitter();
  public onShow: EventEmitter<void> = new EventEmitter();
  public onChange: EventEmitter<INavigationMenuItem> = new EventEmitter();

  public init<TNavigateParam>(
    navigateToFn: (menuId: string, parameters: TNavigateParam | null, skipLocationChange: boolean) => void,
    items: INavigationMenuItem[],
    navigationParameterFn?: (item: INavigationMenuItem) => TNavigateParam,
    navigateSkipUrlLocationChange: boolean = true
  ): void {
    const menus = [];
    items.forEach(item => {
      menus.push({
        ...item,
        onClick: (menuItem: INavigationMenuItem) =>
          this.onMenuItemClick(navigateToFn, menuItem, navigationParameterFn, navigateSkipUrlLocationChange)
      });
    });
    this.items = menus;
  }

  public activate(id: string): void {
    this.items.forEach(item => (item.isActivated = item.id === id));
  }

  public find(id: string): INavigationMenuItem {
    return this.items.find(item => item.id === id);
  }

  public hide(): void {
    this.onHide.next();
  }

  public show(): void {
    this.onShow.next();
  }

  public onMenuItemClick<TNavigateParam>(
    navigateToFn: (menuId: string, parameters: TNavigateParam | null, skipLocationChange: boolean) => void,
    menu: INavigationMenuItem,
    navigationParameterFn?: (item: INavigationMenuItem) => TNavigateParam,
    navigateSkipUrlLocationChange: boolean = true
  ): void {
    // To prevent navigating to the current route, we check the activated route before navigating.
    // https://cxtech.atlassian.net/browse/OPX-1705
    if (this.items.find(item => item.id === menu.id).isActivated === true) {
      return;
    }
    this.activate(menu.id);
    navigateToFn(menu.id, navigationParameterFn ? navigationParameterFn(menu) : undefined, navigateSkipUrlLocationChange);
    this.onChange.next(menu);
  }
}
