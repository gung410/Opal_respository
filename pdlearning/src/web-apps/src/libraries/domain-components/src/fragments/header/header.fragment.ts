import { ChangeDetectorRef, Component, ElementRef, HostListener, Inject, Renderer2, ViewChild, ViewContainerRef } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

import { APP_BASE_HREF } from '@angular/common';
import { Align } from '@progress/kendo-angular-popup';
import { AuthService } from '@opal20/authentication';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { Fragment } from '@opal20/infrastructure';
import { FragmentPosition } from '../fragment-position';
import { HeaderService } from './header.service';

@Component({
  selector: 'header-fragment',
  templateUrl: './header.fragment.html'
})
export class HeaderFragment extends Fragment {
  public modules: IRegistrationModule[] = AppGlobal.accessibleModules;
  public userActionItems: IDataItem[] = [
    {
      text: 'Change Password',
      value: 'password'
    },
    {
      text: 'Profile',
      value: 'profile'
    },
    {
      text: 'Sign Out',
      value: 'signout'
    }
  ];
  public router: Router = AppGlobal.router;
  public moduleName: string = 'Dashboard';
  public openDropdown: boolean = false;
  public avatarUrl: string | undefined;
  public appGlobal = AppGlobal;
  public notificationHeight: string = 'auto';
  public notificationUrl: SafeResourceUrl;
  public notificationContentUrl: SafeResourceUrl;
  public notificationAnchorAlign: Align = { horizontal: 'center', vertical: 'bottom' };
  public notificationPopupAlign: Align = { horizontal: 'center', vertical: 'top' };
  public showNotification: boolean = false;
  public enableBellNotification: boolean = AppGlobal.environment.enableBellNotification;

  @ViewChild('notificationAnchor', { static: true })
  public notificationAnchor: ElementRef;
  @ViewChild('notificationPopup', { static: true, read: ElementRef })
  public notificationPopup: ElementRef;

  protected position: string = FragmentPosition.Header;

  @ViewChild('titleContent', { read: ViewContainerRef, static: true })
  private titleContent!: ViewContainerRef;

  constructor(
    @Inject(APP_BASE_HREF) private baseHref: string,
    protected renderer: Renderer2,
    protected changeDetectorRef: ChangeDetectorRef,
    protected elementRef: ElementRef,
    public headerService: HeaderService,
    public authService: AuthService,
    private sanitizer: DomSanitizer
  ) {
    super(renderer, changeDetectorRef, elementRef);

    this.headerService.init(this);
    this.avatarUrl = (AppGlobal.user ? AppGlobal.user.avatarUrl : undefined) || './assets/images/others/default-avatar.png';
    this.notificationUrl = this.sanitizer.bypassSecurityTrustResourceUrl(AppGlobal.environment.notificationUrl);
  }

  public attachTitleView(element: HTMLElement): void {
    this.renderer.addClass(this.titleContent.element.nativeElement, 'has-content');
    this.registerView(this.titleContent.element.nativeElement, element);
  }

  public detachTitleView(): void {
    this.renderer.removeClass(this.titleContent.element.nativeElement, 'has-content');
    this.removeChildNodes(this.titleContent.element.nativeElement);
  }

  public onSelect(event: ContextMenuSelectEvent): void {
    const moduleId = event.item.data.id;
    const navigateToModule: () => void = event.item.data.navigateToModule;

    if (navigateToModule) {
      navigateToModule();
    } else {
      this.router.setRoute(moduleId);
    }
  }

  public onSelectUserAction(event: ContextMenuSelectEvent): void {
    const actionId = event.item.data.value;

    switch (actionId) {
      case 'password':
        this.authService.navigateToExternalSite(AppGlobal.environment.authConfig.changePasswordUrl);
        break;
      case 'profile':
        this.authService.navigateToExternalSite(AppGlobal.environment.authConfig.profileUrl);
        break;
      case 'signout':
        this.authService.logout();
        break;
      default:
        break;
    }
  }

  public navigateToModuleHomePage(): void {
    const moduleId: string = AppGlobal.getModuleIdFromUrl(this.baseHref);
    this.router.setRoute(`${this.baseHref}${moduleId}`);
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    const data = event.data && event.data.paramsPopup;

    if (!data) {
      return;
    }

    if (data.url) {
      this.notificationContentUrl = this.sanitizer.bypassSecurityTrustResourceUrl(data.url);
    }

    if (data.height) {
      this.notificationHeight = data.height;
    }

    this.toggleNotification(true);
  }

  @HostListener('document:click', ['$event'])
  public onDocumentClick(event: MouseEvent): void {
    if (
      this.enableBellNotification &&
      !(
        (this.notificationAnchor && this.notificationAnchor.nativeElement.contains(event.target)) ||
        (this.notificationPopup ? this.notificationPopup.nativeElement.contains(event.target) : false)
      )
    ) {
      this.toggleNotification(false);
    }
  }

  public toggleNotification(show: boolean | undefined = undefined): void {
    if (show === undefined) {
      this.showNotification = !this.showNotification;
    } else {
      this.showNotification = show;
    }
  }
}
