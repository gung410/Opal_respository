import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import {
  AppsSwitcherItem,
  CxFooterData,
  CxNavbarItemModel,
  JwksValidationHandler,
  OAuthService,
  OAuthStorage,
  OAuthSuccessEvent
} from '@conexus/cx-angular-common';
import { environment } from 'app-environments/environment';
import {
  MenuChildItemAPI,
  MenuItemAPI,
  SiteData,
  User
} from 'app-models/auth.model';
import { Header } from 'app-models/header.model';
import { NotificationService } from 'app-services/notification.service';
import { BehaviorSubject, Subject } from 'rxjs';

import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ModuleType } from 'app/permissions/enum/module-type.enum';
import { AccessRightsModel } from 'app/permissions/models/access-rights.model';
import {
  AppConstant,
  OAuthEventConstant,
  RouteConstant
} from '../app.constant';
import { IPermissionDictionary } from '../components/component.abstract';
import { AuthDataService } from './auth-data.service';
import { authConfig } from './auth.config';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  get User(): User {
    const identityClaims = this.authAdapterService.getIdentityClaims();
    if (!identityClaims) {
      return undefined;
    }

    return new User(identityClaims);
  }

  isAuthenticating: Subject<boolean> = new Subject();
  private userSubject: BehaviorSubject<User> = new BehaviorSubject(null);

  constructor(
    private router: Router,
    private authAdapterService: OAuthService,
    private oAuthStorageService: OAuthStorage,
    private authDataService: AuthDataService,
    private notificationService: NotificationService,
    private translateAdapterService: TranslateAdapterService
  ) {
    this.authAdapterService.configure(authConfig);
    this.authAdapterService.setStorage(this.oAuthStorageService);
    this.authAdapterService.tokenValidationHandler = new JwksValidationHandler();
    this.isAuthenticating.next(false);
  }

  navigateToExternalSite(baseUrl: string, path: string): void {
    this.setReturnUrl(window.location.pathname);
    window.location.href = `${baseUrl}/${path}?returnUrl=${encodeURIComponent(
      window.location.origin
    )}`;
  }

  logout(avoidUnsubscribeNotification?: boolean): void {
    const currentUser = this.userData().getValue();
    if (
      currentUser &&
      currentUser.identity &&
      currentUser.identity.extId &&
      !avoidUnsubscribeNotification
    ) {
      this.notificationService
        .unsubscribeUserFromNotificationSource(currentUser.identity.extId)
        .finally(() => {
          this.logoutSPA();
        })
        .catch((error) => {
          console.error('Unsubscribe user from notification failed', error);
          this.logoutSPA();
        });
    } else {
      this.logoutSPA();
    }
  }

  getAccessToken(): string {
    return this.authAdapterService.getAccessToken();
  }

  loadDiscoveryDocumentAndTryLogin(): Promise<any> {
    return new Promise((resolve, reject) => {
      this.authAdapterService
        .loadDiscoveryDocumentAndTryLogin()
        .then((_) => {
          const currentUser = this.User;
          if (currentUser === undefined) {
            this.isAuthenticating.next(false);
            if (!this.hasValidAccessToken()) {
              this.login();
            }

            return reject();
          }

          if (currentUser && currentUser.headerData) {
            this.isAuthenticating.next(false);

            return resolve(currentUser);
          }

          Promise.all([
            this.authDataService.getDataFromCurrentUser(currentUser.id),
            this.getPermissionsForCurrentUser()
          ]).then(
            ([userInfo, permissions]) => {
              currentUser.headerData = this.processHeaderData(
                userInfo.siteData
              );
              currentUser.departmentId = userInfo.departmentId;
              currentUser.roles = userInfo.roles;
              currentUser.systemRoles = userInfo.systemRoles;
              currentUser.identity = userInfo.identity;
              currentUser.entityStatus = userInfo.entityStatus;
              currentUser.avatarUrl = userInfo.avatarUrl;
              currentUser.fullName = userInfo.name;
              currentUser.emails = userInfo.emails;
              currentUser.releaseDate =
                userInfo.siteData && userInfo.siteData.releaseDate;
              currentUser.userDepartment = userInfo.userDepartment;
              currentUser.topAccessibleDepartment =
                userInfo.topAccessibleDepartment;
              currentUser.permissionDic = this.getPermissionDic(permissions);
              // console.info(JSON.stringify(currentUser.permissionDic));
              this.userSubject.next(currentUser);
              this.isAuthenticating.next(false);

              return resolve(currentUser);
            },
            (err) => {
              this.isAuthenticating.next(false);

              return reject(err);
            }
          );
        })
        .catch((err) => {
          this.isAuthenticating.next(false);
          reject(err);
        });
    });
  }

  getPermissionsForCurrentUser(): Promise<AccessRightsModel[]> {
    return this.authDataService
      .getPermissionsForCurrentUser({ modules: [ModuleType.OrganizationSpa] })
      .then((data) => Promise.resolve(data));
  }

  getPermissionDic(accessRights: AccessRightsModel[]): IPermissionDictionary {
    return (
      accessRights.reduce(
        (
          acc: { [actionKey: string]: AccessRightsModel },
          value: AccessRightsModel
        ) => {
          if (value.action) {
            acc[value.action] = value;
          }

          return acc;
        },
        {}
      ) || {}
    );
  }

  processHeaderData(siteData: SiteData): Header {
    const header = new Header();
    header.logo = environment.site.logo;
    header.title = environment.site.title;
    if (!siteData) {
      return;
    }
    // Get menu data
    header.menus = this.getMenuItemFromSiteData(siteData);
    // Get application
    header.applications = this.getApplicationFromSiteData(siteData);

    return header;
  }

  getApplicationFromSiteData(siteData: SiteData): AppsSwitcherItem[] {
    if (siteData && siteData.menus && siteData.menus.length > 0) {
      const listAPIMenu = siteData.menus;
      for (const apiMenuItem of listAPIMenu) {
        if (apiMenuItem.type === AppConstant.menuType.APP_SWITCHER) {
          const listAPIMenuItem = apiMenuItem.menuItems;

          return this.getAppSwitcherFromMenuItem(listAPIMenuItem);
        }
      }
    }

    return undefined;
  }

  login(): void {
    const returnUrl = localStorage.getItem(RouteConstant.RETURN_URL);
    this.clearStorages();
    this.setReturnUrl(returnUrl);
    this.authAdapterService.initCodeFlow();
  }

  hasValidAccessToken(): boolean {
    return this.authAdapterService.hasValidAccessToken();
  }

  hasValidIdToken(): boolean {
    return this.authAdapterService.hasValidIdToken();
  }

  setReturnUrl(returnUrl: string): void {
    localStorage.setItem(RouteConstant.RETURN_URL, returnUrl);
  }

  checkIsMenuRoute(routeURL: string): boolean {
    const menusRouteURL = AppConstant.siteURL.menus;
    for (const key of Object.getOwnPropertyNames(menusRouteURL)) {
      const menuRoute = `/${menusRouteURL[key]}`;
      if (routeURL.includes(menuRoute)) {
        return true;
      }
    }

    return false;
  }

  hasRightToAccessMenu(routeURL: string): boolean {
    const currentUser = this.userSubject.getValue();
    if (currentUser && currentUser.headerData && currentUser.headerData.menus) {
      const listMenuItem = currentUser.headerData.menus;
      for (const menuItem of listMenuItem) {
        if (menuItem.route && routeURL.includes(menuItem.route)) {
          return true;
        }
        if (menuItem.children && menuItem.children.length > 0) {
          for (const childMenuItem of menuItem.children) {
            if (childMenuItem.route && routeURL.includes(childMenuItem.route)) {
              return true;
            }
          }
        }
      }
    }

    return false;
  }

  userData(): BehaviorSubject<User> {
    return this.userSubject;
  }

  userDataInfo(): User {
    return this.userSubject.getValue();
  }

  configureWithNewConfigApi(params: object = {}): void {
    this.authAdapterService.setupAutomaticSilentRefresh(params);
    this.authAdapterService.events.subscribe((e: OAuthSuccessEvent) => {
      if (!e || !e.type) {
        return;
      }
      switch (e.type) {
        case OAuthEventConstant.TOKEN_EXPIRES:
          console.error('token_expires');
          break;
        case OAuthEventConstant.SESSION_ERROR:
          console.error('system cannot check session');
          break;
        case OAuthEventConstant.TOKEN_RECIEVED:
          this.authAdapterService.clearHashAfterLogin = false;
          break;
        case OAuthEventConstant.DOCIMENT_LOAD_FAILED:
          console.error('load document fail: ', JSON.stringify(e));
          break;
        case OAuthEventConstant.TOKEN_ERROR:
          console.error('token error', JSON.stringify(e));
          this.router.navigate([RouteConstant.ERROR_COMMON]);
          break;
        case OAuthEventConstant.SESSION_TERMINATED:
        case OAuthEventConstant.CODE_ERROR:
          console.error('Session terminated', JSON.stringify(e));
          this.logout(true);
          break;
        default:
          break;
      }
    });
  }

  /** Clears everything in the session storage and the local storage. */
  clearStorages(): void {
    sessionStorage.clear();
    localStorage.clear();
  }

  getFooterData(releaseDate: string): CxFooterData {
    const copyRightYear = new Date().getFullYear();
    const vulnerabilityText = this.translateAdapterService.getValueImmediately(
      'Footer.ReportVulnerability'
    );
    const privacyStatementText = this.translateAdapterService.getValueImmediately(
      'Footer.PrivacyStatement'
    );
    const termsOfUseText = this.translateAdapterService.getValueImmediately(
      'Footer.TermsOfUse'
    );
    const copyrightText = this.translateAdapterService.getValueImmediately(
      'Footer.CopyRight',
      { year: copyRightYear }
    );
    const rightReservedText = this.translateAdapterService.getValueImmediately(
      'Footer.AllRightReserved'
    );
    const lastUpdatedText = this.translateAdapterService.getValueImmediately(
      'Footer.LastUpdated'
    );
    const dateReleaseText = lastUpdatedText + ': ' + releaseDate;

    return {
      vulnerabilityText,
      privacyStatementText,
      termsOfUseText,
      copyrightText,
      rightReservedText,
      dateReleaseText,
      vulnerabilityUrl: environment.site.footer.vulnerabilityUrl,
      privacyStatementUrl: environment.site.footer.privacyStatementUrl,
      termsOfUseUrl: environment.site.footer.termsOfUseUrl
    };
  }

  private logoutSPA(): void {
    const idToken = encodeURIComponent(this.authAdapterService.getIdToken());
    this.clearStorages();
    if (!idToken || idToken === '' || idToken === 'null') {
      this.router.navigate(['/' + AppConstant.siteURL.login]);
    } else {
      const logoutUrl = `${authConfig.logoutUrl}?id_token_hint=${idToken}&post_logout_redirect_uri=${authConfig.postLogoutRedirectUri}`;

      this.authAdapterService.logOut(true);
      location.href = logoutUrl;
    }
  }

  private getAppSwitcherFromMenuItem(
    listAPIMenuItem: MenuChildItemAPI[]
  ): AppsSwitcherItem[] {
    const arrayApp = new Array<AppsSwitcherItem>();
    if (listAPIMenuItem && listAPIMenuItem.length > 0) {
      listAPIMenuItem.forEach((item) => {
        const appItem = new AppsSwitcherItem();
        appItem.label = this.getPropLocalizedData(item.localizedData, 'Name');
        appItem.logoUrl = item.cssClass;
        appItem.mainUrl = item.path;
        appItem.openNewTab = item.openInNewTab;
        arrayApp.push(appItem);
      });
    }

    return arrayApp;
  }

  private getMenuItemFromSiteData(siteData: SiteData): CxNavbarItemModel[] {
    if (siteData && siteData.menus && siteData.menus.length > 0) {
      for (const listMenuAPI of siteData.menus) {
        if (listMenuAPI.type === AppConstant.menuType.MENU_ITEM) {
          return this.convertListMenuItemFromAPI(listMenuAPI);
        }
      }
    }

    return undefined;
  }

  private convertListMenuItemFromAPI(
    menuAPI: MenuItemAPI
  ): CxNavbarItemModel[] {
    const listMenu = new Array<CxNavbarItemModel>();
    const listMenuAPI = menuAPI.menuItems;
    if (listMenuAPI && listMenuAPI.length > 0) {
      for (const menuItemAPI of listMenuAPI) {
        const menuItem = this.convertMenuItemFromAPI(menuItemAPI);
        listMenu.push(menuItem);
      }
    }

    return listMenu;
  }

  private convertMenuItemFromAPI(
    apiMenuItem: MenuChildItemAPI
  ): CxNavbarItemModel {
    const menuItem = new CxNavbarItemModel();
    menuItem.content = this.getPropLocalizedData(
      apiMenuItem.localizedData,
      'Name'
    );
    if (apiMenuItem.cssClass) {
      menuItem.icon = apiMenuItem.cssClass;
      menuItem.iconActive = apiMenuItem.cssClass + '-selected';
    }
    if (apiMenuItem.path) {
      menuItem.route = apiMenuItem.path;
    } else {
      if (apiMenuItem.childMenuItems && apiMenuItem.childMenuItems.length > 0) {
        menuItem.children = this.getChildrenFromAPIMenuItem(
          apiMenuItem.childMenuItems
        );
      }
    }

    return menuItem;
  }

  private getChildrenFromAPIMenuItem(
    childAPIMenuItem: MenuChildItemAPI[]
  ): CxNavbarItemModel[] {
    const listChildMenuItem = new Array<CxNavbarItemModel>();
    childAPIMenuItem.forEach((item) => {
      const childMenuItem = new CxNavbarItemModel();
      childMenuItem.content = this.getPropLocalizedData(
        item.localizedData,
        'Name'
      );
      childMenuItem.route = item.path;
      listChildMenuItem.push(childMenuItem);
    });

    return listChildMenuItem;
  }

  private getPropLocalizedData(
    localizedDataArray: any,
    propName: string
  ): string {
    for (const localizedDataItem of localizedDataArray) {
      const fields = localizedDataItem.fields;
      for (const field of fields) {
        if (field.name === propName) {
          return field.localizedText;
        }
      }
    }

    return undefined;
  }
}
