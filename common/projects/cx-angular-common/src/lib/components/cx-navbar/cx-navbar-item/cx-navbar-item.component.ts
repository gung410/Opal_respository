import { uniqueId } from 'lodash';
import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectorRef,
  ElementRef
} from '@angular/core';
import { CxNavbarItemModel } from '../models/cx-navbar-item-model';
import { BaseComponent } from '../../../abstracts/base.component';
import { MediaObserver } from '@angular/flex-layout';

@Component({
  selector: 'cx-navbar-item',
  templateUrl: './cx-navbar-item.component.html',
  styleUrls: ['./cx-navbar-item.component.scss']
})
export class CxNavbarItemComponent extends BaseComponent {
  @Input() navbarItem: CxNavbarItemModel;
  @Input() listMenu: Array<CxNavbarItemModel>;
  @Input() level: number = 1;
  @Output() itemExpand = new EventEmitter();
  public mouseOvered = false;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    elementRef: ElementRef,
    media: MediaObserver
  ) {
    super(changeDetectorRef, elementRef, media);
  }

  uniqueMenuParentId = 'menu-parent-' + uniqueId();
  uniqueSubMenuId = 'menu-sub-' + uniqueId();

  ngOnInit() {
    super.ngOnInit();
  }

  expand() {
    const previousIsCollapsed = this.navbarItem.isCollapsed;
    this.resetCollapsed(this.listMenu);
    this.navbarItem.isCollapsed = !previousIsCollapsed;
    if (!this.navbarItem.isCollapsed && this.navbarItem.children && this.navbarItem.children.length <= 0) {
        this.resetActive(this.listMenu);
        this.setPropertyForParent(this.listMenu, true, this.navbarItem.route, 'isActive');
        this.navbarItem.isActive = true;
        this.setPropertyForParent(this.listMenu, true, this.navbarItem.route, 'isCollapsed');
        this.itemExpand.emit(this.navbarItem);
    } else {
      if (this.level !== 1) {
        this.setPropertyForParent(this.listMenu, false, this.navbarItem.route, 'isCollapsed');
        this.navbarItem.isCollapsed = !previousIsCollapsed;
      }

      if (this.level === 1 && !this.navbarItem.isCollapsed ) {
        this.alignPositionSubItem();
      }
    }
  }

  childClick(item: any) {
    this.itemExpand.emit(item);
  }

  resetActive(menus: Array<CxNavbarItemModel>) {
    menus.forEach(menu => {
      menu.isActive = false;
      this.resetActive(menu.children);
    });
  }

  resetCollapsed(menus: Array<CxNavbarItemModel>) {
    menus.forEach(menu => {
      menu.isCollapsed = true;
      this.resetCollapsed(menu.children);
    });
  }

  findParent(menus: Array<CxNavbarItemModel>, content: string) {
    if (menus) {
      return menus.find(x => x.content === content);
    }
    this.listMenu.forEach(element => {
      return this.findParent(element.children, content);
    });
    return undefined;
  }

  setPropertyForParent(menus: Array<CxNavbarItemModel>, target: any, navBarItemRoute: string, property: string) {
    if (menus) {
      const element = menus.find(x => x.route === navBarItemRoute);
      if (element)  {
        element[property] = target;
        return true;
      } else {
        for (const menu of menus) {
          if (this.setPropertyForParent(menu.children, target, navBarItemRoute, property)) {
            menu[property] = target;
            return true;
          }
        }
      }
    } else {
      return false;
    }
  }

  private alignPositionSubItem() {
    const parentMenu = $('#' + this.uniqueMenuParentId);
    const subMenu = $('#' + this.uniqueSubMenuId);
    if (!parentMenu || !subMenu) {
      return;
    }
    const parentMenuOffset = parentMenu.offset();
    const windowScrollTop = $(window).scrollTop();
    const subMenuTop = parentMenuOffset.top + parentMenu.height() - windowScrollTop;
    const subMenuLeft = parentMenuOffset.left;
    subMenu.css({top: subMenuTop, left: subMenuLeft, position: 'fixed'});
  }
}
