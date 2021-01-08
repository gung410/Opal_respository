import { AuthService, SiteData, User } from '@opal20/authentication';
import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { HeaderService, OpalFooterService } from '@opal20/domain-components';

import { AppsSwitcherItem } from '../../models/app-switcher.model';
import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'error-page',
  templateUrl: './error-page.component.html'
})
export class ErrorPageComponent extends BaseComponent {
  public errorMessageTitle: string;
  public errorMessageDescription: string;
  public moduleMessageDescription: string;
  public errorImage: string = 'assets/images/others/error-sorry.png';
  public moduleLogoPath: string = 'assets/images/logos/opal-logo.png';
  public defaultAvatar = 'assets/images/others/default-avatar.png';
  public errorMessageAction: string;
  public currentUser: User;
  public listModule: AppsSwitcherItem[];
  public showLogoutButton: boolean = true;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private authService: AuthService,
    private translateService: TranslateService,
    private opalFooterService: OpalFooterService,
    private headerService: HeaderService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.opalFooterService.hide();
    this.headerService.hide();
    this.currentUser = AppGlobal.user;
    this.processData();
  }

  public onDestroy(): void {
    this.opalFooterService.show();
    this.headerService.show();
  }

  public tryAgain(): void {
    document.location.href = '/';
  }

  public onLogout(): void {
    this.authService.logout();
  }

  public onAppClick(url: string): void {
    if (url) {
      window.location.href = url;
    }
  }

  private processData(): void {
    const descriptionText = 'It looks like our systems are having some trouble at the moment.';
    const tryAgainText = 'Try again';
    const sitePermissionText = `You don't have permission to access this site.`;
    const pagePermissionText = `You don't have permission to access this page.`;
    const pendingUserDescriptionText = 'Your account is not activated yet, please wait for approval.';

    this.errorMessageDescription = descriptionText;
    this.errorMessageAction = tryAgainText;
    this.moduleMessageDescription = '';
    this.errorMessageDescription += this.isInIframe ? ' Please refresh the page and try again.' : '';

    this.listModule = new Array<AppsSwitcherItem>();
    this.errorMessageDescription = sitePermissionText;
    this.errorMessageAction = '';
    this.getApps();

    // TODO: cloned from PDPM, not using now, but keep it for future.
    // switch (errorId) {
    //   case 'forbidden-user':
    //     this.listModule = new Array<AppsSwitcherItem>();
    //     this.errorMessageDescription = sitePermissionText;
    //     this.errorMessageAction = '';
    //     this.getApps();
    //     break;
    //   case 'pending-user': // For pending user
    //     this.errorMessageDescription = sitePermissionText;
    //     this.errorMessageAction = '';
    //     this.moduleMessageDescription = pendingUserDescriptionText;
    //     break;
    //   case 'access-denied':
    //     this.errorMessageDescription = pagePermissionText;
    //     this.errorMessageAction = '';
    //     this.showLogoutButton = false;
    //     break;
    //   default:
    //     this.listModule = new Array<AppsSwitcherItem>();
    //     this.errorMessageDescription = sitePermissionText;
    //     this.errorMessageAction = '';
    //     this.getApps();
    //     break;
    // }
  }

  get isInIframe(): boolean {
    try {
      return window.self !== window.top;
    } catch (e) {
      return true;
    }
  }

  get canLogOut(): boolean {
    const hasValidIdToken = this.authService.hasValidAccessToken();

    return hasValidIdToken && this.showLogoutButton && !this.isInIframe;
  }

  private getApps(): void {
    if (!AppGlobal.user.siteData) {
      return;
    }
    const listApiMenuItem = AppGlobal.user.siteData.menus;
    if (listApiMenuItem && listApiMenuItem.length > 0) {
      const siteData = new SiteData();
      siteData.menus = listApiMenuItem;
      this.listModule = this.authService.getApplicationFromSiteData(siteData);
      const moduleDescriptionTranslate = this.translateService.instant('ErrorPage.ModuleDescription');
      this.moduleMessageDescription =
        moduleDescriptionTranslate === 'ErrorPage.ModuleDescription'
          ? 'Please navigate to one of the following apps:'
          : moduleDescriptionTranslate;
    }
  }
}
