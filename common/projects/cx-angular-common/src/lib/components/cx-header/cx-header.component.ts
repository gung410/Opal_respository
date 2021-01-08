import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  Output,
  ViewEncapsulation,
  ViewChild,
} from '@angular/core';
import { MediaObserver } from '@angular/flex-layout';

import { BaseComponent } from '../../abstracts/base.component';
import { CxAnimations } from '../../constants/cx-animation.constant';
import { AppsSwitcherItem } from './models/apps-switcher-item.model';
import { CurrentUser } from './models/current-user.model';
import { HeaderLogo } from './models/header-logo.model';
import { MultipleLanguages } from './models/multiple-languages.model';
import { NotificationItem } from './models/notification-item.model';
import { NotificationList } from './models/notification-list.model';
import { TopHeader } from './models/top-header.model';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';


@Component({
  selector: 'cx-header',
  templateUrl: './cx-header.component.html',
  styleUrls: ['./cx-header.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [
    CxAnimations.smoothAppendRemove
  ]
})

export class CxHeaderComponent extends BaseComponent {
  [x: string]: any;
  public height = 'auto';

  @Input() currentUser: CurrentUser;
  @Input() logo: HeaderLogo;
  @Input() topHeader: TopHeader;
  @Input() placeholderText: string;
  @Input() currentURL: string;
  @Input() multipleLanguages: MultipleLanguages = {
    notifications: 'Notifications',
    search: 'Search'
  };
  @Input() notificationList: NotificationList;
  @Input() isShowNotificationBell = false;
  @Input() applications: AppsSwitcherItem[];
  @Input() bellUrl: SafeResourceUrl;
  @Input() notificationContentUrl: SafeResourceUrl;

  @Output() searchOnSearchBox = new EventEmitter<string>();

  @Output() clickedNotification = new EventEmitter<NotificationItem>();
  @Output() clickedClearAllNotifications = new EventEmitter<void>();
  @Output() clickedClearNotification = new EventEmitter<NotificationItem>();
  @Output() clickedHamburger = new EventEmitter<string>();
  @Output() searchValueChange = new EventEmitter<string>();
  @Output() openNotificationPopUp = new EventEmitter<boolean>();

  @ViewChild('toggleNotification') toggleNotification: ElementRef<HTMLElement>;

  public isHoverNotificationsIcon: boolean;
  public isHoverAppsMenuIcon: boolean;
  public isHoverUserIcon: boolean;
  public isHoverNotificationItem: boolean;
  private _searchValue = '';

  public notificationItemTrackByFn: (index, item: NotificationItem) => {};
  @Input() get searchValue() {
    return this._searchValue;
  }
  set searchValue(val) {
    this._searchValue = val;
    this.searchValueChange.emit(val);
  }
  public toggleHamburger = false;
  private maxWidth = 768;

  constructor(changeDetectorRef: ChangeDetectorRef,
    elementRef: ElementRef,
    media: MediaObserver,
    private domSanitizer: DomSanitizer) {
    super(changeDetectorRef, elementRef, media);
  }

  ngOnInit() {
    super.ngOnInit();
    this.notificationItemTrackByFn = (index, item) => {
      return item.id;
    };
  }
  @HostListener('window:resize', [])
  onResize() {
    if (window.innerWidth > this.maxWidth) {
      // Reset flag toggleHamburger
      const listElement = document.getElementsByClassName('hamburger__open');
      if (listElement.length > 0) {
        this.toggleHamburger = false;
        this.changeDetectorRef.detectChanges();
      }
    }
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    this.receiveMessageFromIframe(event);
  }

  onSearch(searchTerm: string): void {
    this.searchOnSearchBox.emit(searchTerm);
  }

  onClickedNotification(notification: NotificationItem): void {
    this.clickedNotification.emit(notification);
  }

  onClearSearch() {
    this.searchValue = '';
    this.searchOnSearchBox.emit(this.searchValue);
  }

  onToggleHamburger() {
    this.toggleHamburger = !this.toggleHamburger;
    this.clickedHamburger.emit();
    this.changeDetectorRef.detectChanges();
  }

  checkIsCurrentURL(itemURL: string): boolean {
    // currentURL is passing from SPAs, e.g: https://www.uat.opal2.conexus.net/pdplanner/employee
    // appswitcher's itemURL are configured to home url for each web app, e.g: https://www.uat.opal2.conexus.net/pdplanner
    return this.currentURL.startsWith(itemURL);
  }

  openNotification(isOpen: boolean) {
    this.openNotificationPopUp.emit(isOpen);
  }

  onClickClearAllNotifications() {
    this.clickedClearAllNotifications.emit();
  }

  onClickClearNotification(notification: NotificationItem) {
    this.clickedClearNotification.emit(notification);
  }

  get devLocalEnv(): boolean {
    return location.hostname === 'localhost';
  }

  private receiveMessageFromIframe(event: MessageEvent): void {
    const data = event.data.paramsPopup;

    if (!data) {
      return;
    }

    if (!data) {
      return;
    }

    if (data.url) {
      this.showPopupNotifications(data.url);
    }

    if (data.height) {
      this.height = data.height;
    }
  }

  private showPopupNotifications(url) {
    this.notificationContentUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(url);

    const el: HTMLElement = this.toggleNotification.nativeElement;
    el.click();
  }

}
