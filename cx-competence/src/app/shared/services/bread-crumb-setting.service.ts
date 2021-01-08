import { Injectable, Output, EventEmitter, Directive } from '@angular/core';
import { CxNavbarItemModel } from '@conexus/cx-angular-common';
import { CxBreadCrumbItem } from 'app-models/breadcrumb.model';

@Directive()
@Injectable()
/**
 * This service class will check and load all the routes have preload configure to tre
 * and also add the routes which have sendInReturnUrlOidc to the RouteConfigService to use when
 * redirect to IDP for login
 */
export class BreadcrumbSettingService {
  @Output() changeBreadcrumbEvent: EventEmitter<any> = new EventEmitter();
  paths = [];
  currentBreadcrumb = [];
  constructor() {}

  public changeBreadcrumb(result: any) {
    this.changeBreadcrumbEvent.emit(result);
  }

  public mapRouteToBreadcrumb(
    menus: Array<CxNavbarItemModel>,
    route: string,
    param?: string
  ): CxBreadCrumbItem[] {
    this.paths = [];
    const currentBreadcrumb: CxBreadCrumbItem[] = [];
    menus.forEach((menu) => {
      this.getPaths(menu.children, menu.content);
    });
    const currentRoute = this.paths.find((e) => route.includes(e.route));
    if (currentRoute && currentRoute.displayString) {
      const items = currentRoute.displayString.split('/');
      items.forEach((item) => {
        if (item !== currentRoute.finalString) {
          currentBreadcrumb.push(new CxBreadCrumbItem(item, ''));
        } else {
          currentBreadcrumb.push(
            new CxBreadCrumbItem(item, currentRoute.route)
          );
        }
      });
    }
    if (param) {
      currentBreadcrumb.push(new CxBreadCrumbItem(param, ''));
    }
    this.currentBreadcrumb = currentBreadcrumb;

    return this.currentBreadcrumb;
  }

  private getPaths(menus: Array<CxNavbarItemModel>, previousString?: string) {
    for (const menu of menus) {
      if (menu.children && menu.children.length > 0) {
        this.getPaths(menu.children, `${previousString}/${menu.content}`);
      } else {
        this.paths.push({
          displayString: `${previousString}/${menu.content}`,
          route: menu.route,
          finalString: menu.content,
        });
      }
    }
  }
}
