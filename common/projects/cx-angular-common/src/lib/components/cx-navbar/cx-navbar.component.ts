import { CxNavbarItemModel } from './models/cx-navbar-item-model';
import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  HostListener,
  AfterViewInit,
  ViewChild,
  ElementRef,
  ChangeDetectionStrategy,
  ChangeDetectorRef
} from '@angular/core';
import { Router, RouterEvent, NavigationEnd } from '@angular/router';

@Component({
  selector: 'cx-navbar',
  templateUrl: './cx-navbar.component.html',
  styleUrls: ['./cx-navbar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class CxNavbarComponent implements OnInit, AfterViewInit {
  @Input() listMenu: Array<CxNavbarItemModel>;
  @Output() clickItem: EventEmitter<any> = new EventEmitter();
  @ViewChild('scrollDiv') scrollableDiv: ElementRef;

  scrollable = false;
  isScrollAtStart = true;
  isScrollAtEnd = false;
  scrollStep = 200; // px;
  scrollDuration = 500; // px;

  constructor(
    private changeDetector: ChangeDetectorRef,
    private router: Router) {
    this.router.events.subscribe(this.activateMenuItem);
  }

  ngOnInit() {
    this.checkMenuIsActiveByRoute();
  }

  ngAfterViewInit(): void {
    this.checkScrollable();
  }

  checkScrollable() {
    if (!this.scrollableDiv || !this.scrollableDiv.nativeElement) {
      return;
    }

    const scrollDiv = this.scrollableDiv.nativeElement;
    this.scrollable = scrollDiv.offsetWidth < scrollDiv.scrollWidth;
    this.changeDetector.detectChanges();
  }

  checkMenuIsActiveByRoute() {
    const currentRoute = window.location.pathname;
    for (const menu of this.listMenu) {
      if (menu.children.length > 0) {
        if (
          menu.children.find(
            submenu =>
              currentRoute.indexOf(submenu.route) !== -1 ||
              submenu.route.indexOf(currentRoute) !== -1
          )
        ) {
          menu.isActive = true;
          this.changeDetector.detectChanges();
          return;
        }
      } else {
        if (
          menu.route.indexOf(currentRoute) !== -1 ||
          currentRoute.indexOf(menu.route) !== -1
        ) {
          menu.isActive = true;
          this.changeDetector.detectChanges();
          return;
        }
      }
    }
  }

  collapseOtherItems(item: any) {
    if (!item) {
      this.listMenu.forEach(x => {
        x.isCollapsed = true;
        x.children.forEach(child => {
          child.isCollapsed = true;
        });
      });
    } else {
      this.clickItem.emit(item);
    }
    this.changeDetector.detectChanges();
  }

  onScroll(event: any) {
    if (!event || !event.target) {
      return;
    }
    const scrollableDiv = event.target;
    const minScrollValue = 0;
    const maxScrollValue = scrollableDiv.scrollWidth - scrollableDiv.offsetWidth;

    this.isScrollAtStart = scrollableDiv.scrollLeft <= minScrollValue;
    this.isScrollAtEnd = scrollableDiv.scrollLeft >= maxScrollValue;
    this.resetCollapsed(this.listMenu);
  }

  @HostListener('window:scroll', [])
  windowScroll() {
    this.resetCollapsed(this.listMenu);
  }

  @HostListener('window:resize', [])
  onResize() {
    this.checkScrollable();
  }

  scrollRight = () => {
    if (
      !this.scrollableDiv ||
      !this.scrollableDiv.nativeElement ||
      this.isScrollAtEnd
    ) {
      return;
    }

    const scrollableDiv = this.scrollableDiv.nativeElement;
    const maxScrollValue = scrollableDiv.scrollWidth - scrollableDiv.offsetWidth;
    const expectedValue = scrollableDiv.scrollLeft + this.scrollStep;
    const actualValue = Math.min(maxScrollValue, expectedValue);

    this.scrollSmoothTo(scrollableDiv, actualValue, this.scrollDuration);
  }

  scrollLeft = () => {
    if (
      !this.scrollableDiv ||
      !this.scrollableDiv.nativeElement ||
      this.isScrollAtStart
    ) {
      return;
    }

    const scrollableDiv = this.scrollableDiv.nativeElement;
    const minScrollValue = 0;
    const expectedValue = scrollableDiv.scrollLeft - this.scrollStep;
    const actualValue = Math.max(minScrollValue, expectedValue);

    this.scrollSmoothTo(scrollableDiv, actualValue, this.scrollDuration);
  }

  private scrollSmoothTo(
    scrollElement: any,
    position: number,
    duration: number
  ) {
    if (!scrollElement) {
      return;
    }

    $(scrollElement).animate({ scrollLeft: position }, duration);
  }

  private resetCollapsed(menus: Array<CxNavbarItemModel>) {
    if (menus && menus.length > 0) {
      menus.forEach(menu => {
        menu.isCollapsed = true;
        this.resetCollapsed(menu.children);
      });
    }
  }

  private activateMenuItem = (navigationEvent: RouterEvent): void => {
    if (navigationEvent instanceof NavigationEnd) {
      // In order to prevent duplication of activating menu item.
      // The system should reset the active menu item before setting to the current activating menu item.
      this.resetActive(this.listMenu);
      this.setActiveMenuItem(navigationEvent.urlAfterRedirects);
    }
  }

  private setActiveMenuItem(navigatingUrl: string): void {
    for (const menuItem of this.listMenu) {
      if (menuItem.route && menuItem.matchUrl(navigatingUrl)) {
        menuItem.isActive = true;
      }
      if (menuItem.children && menuItem.children.length > 0) {
        for (const childMenuItem of menuItem.children) {
          if (childMenuItem.route && childMenuItem.matchUrl(navigatingUrl)) {
            childMenuItem.isActive = true;
            menuItem.isActive = true;
          }
        }
      }
    }
  }

  private resetActive(menus: Array<CxNavbarItemModel>) {
    menus.forEach(menu => {
      menu.isActive = false;
      this.resetActive(menu.children);
    });
  }
}
