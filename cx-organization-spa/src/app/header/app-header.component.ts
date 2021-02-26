import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  Output,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Event, NavigationEnd, Router } from '@angular/router';
import {
  CxNavbarItemModel,
  NotificationItem,
  NotificationList
} from '@conexus/cx-angular-common';
import { environment } from 'app-environments/environment';
import { User } from 'app-models/auth.model';
import { GlobalKeySearchStoreService } from 'app/core/store-services/search-key-store.service';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { Subscription } from 'rxjs';
import { MenuRouteEnum } from './constant/menu-route-enum';

@Component({
  selector: 'app-header',
  templateUrl: './app-header.component.html',
  styleUrls: ['./app-header.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppHeaderComponent
  extends BaseSmartComponent
  implements OnDestroy {
  @Input() userData: User;
  @Input() notifications: NotificationList;
  @Input() isShowNotificationBell: boolean = false;
  @Output() signOut: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() editProfile: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() clickSettings: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() clickSupport: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output()
  clickNotificationItem: EventEmitter<NotificationItem> = new EventEmitter<NotificationItem>();
  @Output()
  openNotificationPopup: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() changePassword: EventEmitter<boolean> = new EventEmitter();

  isHeaderAtTop: boolean = true;
  toggleNavObj: object = {};
  currentSite: string;
  currentSearchText: string = '';
  notificationList: NotificationList;
  notificationBellUrl: SafeResourceUrl;
  isHideSearchBar: boolean = true;

  private previousPath: string;
  private routerEventSubscription: Subscription = new Subscription();
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private router: Router,
    private searchKeyGlobalStoreService: GlobalKeySearchStoreService,
    private domSanitizer: DomSanitizer
  ) {
    super(changeDetectorRef);
    // angular-common library are checking active menu item by currentSite.startWith(menuUrl),
    //  passing pathname also incase SPA deploy as virtualpath
    this.currentSite =
      `${location.protocol}//${location.hostname}${location.pathname}` ||
      undefined;
    this.previousPath = this.getUrlPath(this.router.url);
    this.routerEventSubscription = this.router.events.subscribe(
      (event: Event) => {
        if (!(event instanceof NavigationEnd)) {
          return;
        }

        this.clearUniversalSearch();

        this.checkToHideSearchBar(event.url);
        this.generatePlaceholderText(event.url);

        const currentNavigatedPath = this.getUrlPath(event.url);
        this.previousPath = currentNavigatedPath;
        this.changeDetectorRef.detectChanges();
      }
    );
  }

  ngAfterViewInit(): void {
    this.notificationBellUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(
      environment.notification.bellUrl
    );

    this.checkToHideSearchBar(this.router.url);
    this.generatePlaceholderText(this.router.url);
  }

  ngOnDestroy(): void {
    this.routerEventSubscription.unsubscribe();
  }

  onLogout(): void {
    this.signOut.emit();
  }

  onEditProfile(): void {
    this.editProfile.emit();
  }

  onChangePassword(): void {
    this.changePassword.emit();
  }

  onSearch(searchKey: string): void {
    this.searchKeyGlobalStoreService.edit({ isSearch: true, searchKey });
  }

  onClickSettings(): void {
    this.clickSettings.emit();
  }

  onClickSupport(): void {
    this.clickSupport.emit();
  }

  onClickNotification(notification: NotificationItem): void {
    this.clickNotificationItem.emit(notification);
  }

  onOpenNotificationPopUp(isOpen: boolean): void {
    this.openNotificationPopup.emit(isOpen);
  }

  onClickClearNotification(notificationItem: NotificationItem): void {
    // TODO: handle
  }

  onClickClearAllNotification(): void {
    // TODO: handle
  }

  onClickedHamburger(): void {
    // TODO: handle hamburger button when implement for mobile
  }

  onMenuItemClicked(menuItem: CxNavbarItemModel): void {
    this.checkToHideSearchBar(menuItem.route);
    this.generatePlaceholderText(menuItem.route);
  }

  generatePlaceholderText(menuRouteString: string): string {
    menuRouteString = menuRouteString ? menuRouteString : this.router.url;
    switch (menuRouteString) {
      case MenuRouteEnum.UserAccounts:
        return 'Search in User Management';
      case MenuRouteEnum.Organization:
        return 'Search in Organisation Management';
      case MenuRouteEnum.TaxonomyManagement:
        return 'Search in Metadata Management';
      default:
        return '';
    }
  }

  private getUrlPath(url: string): string {
    return url.split('?')[0];
  }

  private clearUniversalSearch(): void {
    this.currentSearchText = '';
    this.searchKeyGlobalStoreService.edit({
      isSearch: false,
      searchKey: this.currentSearchText
    });
  }

  private checkToHideSearchBar(url: string): void {
    const allowToShowSearchBarRoute = [
      MenuRouteEnum.UserAccounts.toString(),
      MenuRouteEnum.Organization.toString(),
      MenuRouteEnum.TaxonomyManagement.toString()
    ];
    this.isHideSearchBar = !allowToShowSearchBarRoute.includes(url);
  }
}
