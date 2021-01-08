import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppsSwitcherItem } from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { AuthDataService } from 'app-auth/auth-data.service';
import { AuthService } from 'app-auth/auth.service';
import { MenuItemAPI, SiteData, User } from 'app-models/auth.model';
import { AppConstant } from 'app/shared/app.constant';

@Component({
  selector: 'error-page',
  templateUrl: './error-page.component.html',
  styleUrls: ['./error-page.component.scss'],
})
export class ErrorPageComponent implements OnInit {
  errorMessageTitle: string;
  errorMessageDescription: string;
  moduleMessageDescription: string;
  errorImage: string = 'assets/images/error-sorry.png';
  opalLogo: string = 'assets/images/opal-logo.png';
  errorMessageAction: string;
  isForbidden: boolean;
  currentUser: User;
  listModule: AppsSwitcherItem[];
  showLogoutButton: boolean = true;
  translateData: object;
  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private authDataService: AuthDataService,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params) => {
      setTimeout(() => {
        this.processData(params);
      });
    });
  }

  processData = (params: any) => {
    const errorId = params.id;
    const descriptionPath = 'ErrorPage.SomeTrouble';
    const tryAgainPath = 'ErrorPage.TryAgain';
    const sitePermissionPath = 'ErrorPage.SitePermission';
    const pagePermissionPath = 'ErrorPage.PagePermission';
    const pendingUserDescriptionPath = 'ErrorPage.PendingUserDescription';

    const descriptionText =
      'It looks like our systems are having some trouble at the moment.';
    const tryAgainText = 'Try again';
    const sitePermissionText = "You don't have permission to access this site.";
    const pagePermissionText = "You don't have permission to access this page.";
    const pendingUserDescriptionText =
      'Your account is not activated yet, please wait for approval.';

    const descriptionTranslate = this.translateService.instant(descriptionPath);
    const tryAgainTranslate = this.translateService.instant(tryAgainPath);
    const sitePermissionTranslate = this.translateService.instant(
      sitePermissionPath
    );
    const pagePermissionTranslate = this.translateService.instant(
      pagePermissionPath
    );
    const pendingUserDescriptionTranslate = this.translateService.instant(
      pendingUserDescriptionPath
    );

    this.errorMessageDescription =
      descriptionTranslate === descriptionPath
        ? descriptionText
        : descriptionTranslate;
    this.errorMessageAction =
      tryAgainTranslate === tryAgainPath ? tryAgainText : tryAgainTranslate;
    this.moduleMessageDescription = '';
    this.isForbidden = false;
    this.errorMessageDescription += this.isInIframe
      ? ' Please refresh the page and try again.'
      : '';

    switch (errorId) {
      case 'forbidden-user':
        this.listModule = new Array<AppsSwitcherItem>();
        this.errorMessageDescription =
          sitePermissionTranslate === sitePermissionPath
            ? sitePermissionText
            : sitePermissionTranslate;
        this.isForbidden = true;
        this.errorMessageAction = '';
        this.currentUser = this.authService.User;
        this.getApps();
        break;
      case 'pending-user': // For pending user
        this.errorMessageDescription =
          sitePermissionTranslate === sitePermissionPath
            ? sitePermissionText
            : sitePermissionTranslate;
        this.isForbidden = true;
        this.errorMessageAction = '';
        this.currentUser = this.authService.User;
        this.moduleMessageDescription =
          pendingUserDescriptionTranslate === pendingUserDescriptionPath
            ? pendingUserDescriptionText
            : pendingUserDescriptionTranslate;
        break;
      case 'access-denied':
        this.errorMessageDescription =
          pagePermissionTranslate === pagePermissionPath
            ? pagePermissionText
            : pagePermissionTranslate;
        this.isForbidden = true;
        this.errorMessageAction = '';
        this.currentUser = this.authService.User;
        this.showLogoutButton = false;
        break;
      default:
        break;
    }
  };

  tryAgain(): void {
    document.location.href = '/';
  }

  onLogout(): void {
    this.authService.logout();
  }

  onAppClick(url: string): void {
    if (url) {
      document.location.href = url;
    }
  }

  get isInIframe(): boolean {
    try {
      return window.self !== window.top;
    } catch (e) {
      return true;
    }
  }

  get canLogOut(): boolean {
    const hasValidIdToken = this.authService.hasValidIdToken();

    return hasValidIdToken && this.showLogoutButton && !this.isInIframe;
  }

  private getApps(): void {
    this.authDataService.getMenusByUser().subscribe(
      (listApiMenuItem: MenuItemAPI[]) => {
        if (listApiMenuItem && listApiMenuItem.length > 0) {
          const siteData = new SiteData();
          siteData.menus = listApiMenuItem;
          this.listModule = this.authService.getApplicationFromSiteData(
            siteData
          );
          const moduleDescriptionTranslate = this.translateService.instant(
            'ErrorPage.ModuleDescription'
          );
          this.moduleMessageDescription =
            moduleDescriptionTranslate === 'ErrorPage.ModuleDescription'
              ? 'Please navigate to one of the following apps:'
              : moduleDescriptionTranslate;
        }
      },
      () => {
        return;
      }
    );
  }
}
