import { AppConstant, Constant } from '../app.constant';
import { BehaviorSubject, Subject } from 'rxjs';
import { Inject, Injectable } from '@angular/core';
import { MenuChildItemAPI, SiteData, User } from '../models/auth.model';

import { APP_BASE_HREF } from '@angular/common';
import { AppInfoService } from '@opal20/infrastructure';
import { AuthDataService } from './auth-data.service';
import { Header } from '../models/header.model';
import { JwksValidationHandler } from '../helpers/jwks-validation-handler';
import { OAuthService } from './oauth-service';
import { OAuthStorage } from '../models/types';
import { PermissionService } from './permission.service';

// tslint:disable:all

/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject: BehaviorSubject<User> = new BehaviorSubject(null);
  public isAuthenticating: Subject<boolean> = new Subject();
  public static readonly ORDERED_REDIRECT_MODULES: Dictionary<number> = {
    ['learner']: 1,
    ['cam']: 2,
    ['lmm']: 3,
    ['ccpm']: 4
  };
  public isAuthented: boolean = false;
  private authConfig: any;

  constructor(
    private authAdapterService: OAuthService,
    public oAuthStorageService: OAuthStorage,
    private authDataService: AuthDataService,
    private appInfoService: AppInfoService,
    private permisionService: PermissionService,
    @Inject(APP_BASE_HREF) private baseHref: string
  ) {
    this.authConfig = AppGlobal.environment.authConfig;
    this.authAdapterService.configure(this.authConfig);
    this.authAdapterService.setStorage(this.oAuthStorageService);
    this.authAdapterService.tokenValidationHandler = new JwksValidationHandler();
    this.isAuthenticating.next(false);

    this.isAuthented = false;

    // TODO: Will open this when found better solution for storing jwt stuffs.
    // https://cxtech.atlassian.net/browse/OP-5216
    // The issue is signle sign-out between mulitple tabs.
    // if (AppGlobal.environment.production) {
    //   this.configureWithNewConfigApi();
    // }

    AppGlobal.logoutFn = this.logout.bind(this);
  }

  public navigateToExternalSite(url: string) {
    sessionStorage.setItem(AppConstant.sessionVariable.redirectToExternalSite, 'true');
    sessionStorage.setItem(AppConstant.sessionVariable.workingSite, window.location.pathname);
    window.location.href = `${url}?returnUrl=${this.authConfig.redirectUri}`;
  }

  public backToWorkingSpaceIfHavingExternalNavigation() {
    const redirectToExternalSite = sessionStorage.getItem(AppConstant.sessionVariable.redirectToExternalSite);
    const workingSite = sessionStorage.getItem(AppConstant.sessionVariable.workingSite);
    if (redirectToExternalSite === 'true') {
      sessionStorage.removeItem(AppConstant.sessionVariable.redirectToExternalSite);
      sessionStorage.removeItem(AppConstant.sessionVariable.workingSite);
      // this.router.navigate([workingSite]);
    }
  }

  public logout(avoidUnsubscribeNotification?: boolean): void {
    // if user permission denied, currentUser has data
    // else currentUser=undefiend (when user click browser's back button)

    // Comment the below business logic because affected deeplink

    // const currentUser = this.userData().getValue();
    // if (currentUser) {
    //   // ex: development-cxid-opal-idp.csc.cxs.cloud/app/learner --> get /learner
    //   const path = window.location.href.split('/').slice(-1)[0];
    //   // To check and announce if user is denied
    //   const hasRight =
    //     currentUser.siteData.menus[0].menuItems.findIndex(element => {
    //       return element.path.includes(path);
    //     }) > -1;
    //   if (!hasRight) {
    //     // Redirect to first module in case current user do not permission to access current module
    //     if (currentUser.siteData.menus[0].menuItems.length) {
    //       let menuItemsOrdered = Utils.orderBy(
    //         currentUser.siteData.menus[0].menuItems,
    //         p => AuthService.ORDERED_REDIRECT_MODULES[p.path.split('/').pop()] || MAX_INT
    //       );

    //       // "https://www.development.opal2.conexus.net/app/cam" => pathId = cam
    //       let firstPathId = menuItemsOrdered[0].path.split('/').pop();
    //       window.location.href = `${location.origin}${this.baseHref}${firstPathId}`;
    //       return;
    //     }
    //     alert('You do not have permissions to access this page.');
    //   }
    // }
    this.logoutSPA();
  }

  public get User(): User {
    const identityClaims = this.authAdapterService.getIdentityClaims();
    if (!identityClaims) {
      return undefined;
    }
    return new User(identityClaims);
  }

  public get isLogged(): boolean {
    return (
      this.authAdapterService.getAccessToken() !== undefined &&
      this.authAdapterService.getAccessToken() !== null &&
      this.authAdapterService.getAccessToken() !== '' &&
      this.User !== undefined
    );
  }

  public getAccessToken() {
    return this.authAdapterService.getAccessToken();
  }

  public setAccessToken(token: string): void {
    this.oAuthStorageService.setItem(AppConstant.sessionVariable.accessToken, token);
    this.appInfoService.setAccessToken(token);
  }

  public clearAccessToken(): void {
    this.oAuthStorageService.removeItem(AppConstant.sessionVariable.accessToken);
  }

  public loadDiscoveryDocumentAndTryLogin(redirectUrl?: string): Promise<any> {
    return new Promise((resolve, reject) => {
      this.authAdapterService
        .loadDiscoveryDocumentAndTryLogin(null, redirectUrl)
        .then(res => {
          const currentUser = this.User;
          if (!currentUser) {
            this.isAuthenticating.next(false);
            resolve();

            return;
          }

          if (currentUser.headerData) {
            this.isAuthenticating.next(false);
            resolve(currentUser);

            return;
          }

          this.appInfoService.setAccessToken(this.getAccessToken());
          this.authDataService.getDataFromCurrentUser(currentUser.extId).then(
            (resp: {
              siteData: SiteData;
              departmentId: number;
              systemRoles: any;
              id: number;
              identity: any;
              avatarUrl: string;
              approvingOfficerGroups: [{}];
              fullName: string;
            }) => {
              // const headerData = this.processHeaderData(resp.siteData);
              // currentUser.headerData = headerData;
              currentUser.siteData = resp.siteData;
              currentUser.departmentId = resp.departmentId;
              currentUser.systemRoles = resp.systemRoles;
              currentUser.id = resp.id;
              currentUser.identity = resp.identity;
              currentUser.avatarUrl = resp.avatarUrl;
              currentUser.approvingOfficerGroups = resp.approvingOfficerGroups;
              currentUser.fullName = resp.fullName;
              this.isAuthenticating.next(false);
              this.userSubject.next(currentUser);
              this.isAuthented = true;
              resolve(currentUser);
            },
            err => {
              if (err && err.errorCode === Constant.CANNOT_GET_USER_INFO) {
                // this.router.navigate([RouteConstant.ERROR_COMMON]);
              }
              this.isAuthenticating.next(false);
              this.isAuthented = false;
              reject(err);
            }
          );
        })
        .catch(err => {
          // this.router.navigate([RouteConstant.ERROR_COMMON]);
          this.isAuthenticating.next(false);
          reject(err);
        });
    });
  }

  public loadModulePermissions(): Promise<IModulePermission[]> {
    return this.permisionService
      .getModulePermissions({
        modules: [
          'CommunitySite',
          'CompetenceSpa',
          'CourseAdminManagement',
          'CourseContentSite',
          'LearnerSite',
          'LearningManagement',
          'OrganizationSpa',
          'ReportingAndAnalytics',
          'Webinar'
        ]
        // You can use additional parameters to get full detail permission
        // includeChildren: true,
        // includeLocalizedData: true
      })
      .then(data => Promise.resolve(data))
      .catch(err => Promise.reject(err));
  }

  public login(): Promise<void> {
    if (this.isAuthented) {
      return Promise.resolve();
    }

    sessionStorage.clear();
    localStorage.clear();

    return this.authAdapterService.initCodeFlow();
  }

  public hasValidAccessToken() {
    return this.authAdapterService.hasValidAccessToken();
  }

  public checkIsMenuRoute(routeURL: string): boolean {
    const menusRouteURL = AppConstant.siteURL.menus;
    for (const key of Object.getOwnPropertyNames(menusRouteURL)) {
      const menuRoute = `/${menusRouteURL[key]}`;
      if (routeURL.includes(menuRoute)) {
        return true;
      }
    }
    return false;
  }

  public hasRightToAccessMenu(routeURL: string): boolean {
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

  public userData(): BehaviorSubject<User> {
    return this.userSubject;
  }

  public configureWithNewConfigApi(params: object = {}): void {
    this.authAdapterService.setupAutomaticSilentRefresh(params);
    const tokenReceived = 'token_received';
    const tokenExpires = 'token_expires';
    const sessionTerminarted = 'session_terminated';
    const codeError = 'code_error';
    const sessionError = 'session_error';
    const documentLoaded = 'discovery_document_loaded';
    this.authAdapterService.events.subscribe(event => {
      switch (event.type) {
        case sessionTerminarted:
        case codeError:
        case sessionError:
          this.logout(true);
          break;
        case tokenReceived:
          this.authAdapterService.clearHashAfterLogin = false;
          break;
        case documentLoaded:
          this.authAdapterService.issuer = this.authConfig.issuer;
          break;
      }
    });
  }

  public processHeaderData(siteData: SiteData): Header {
    const header = new Header();
    header.title = 'LMM';
    if (!siteData) {
      return;
    }
    // Get menu data
    header.menus = this.getMenuItemFromSiteData(siteData);
    // Get application
    header.applications = this.getApplicationFromSiteData(siteData);

    return header;
  }

  public getApplicationFromSiteData(siteData: SiteData): Array<any> {
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

  public refreshToken(): void {
    this.authAdapterService.refreshTokenSilently();
  }

  public triggerRefreshTokenLogic(): void {
    this.authAdapterService.triggerRefreshTokenLogic();
  }

  private logoutSPA(): void {
    const idToken = encodeURIComponent(this.authAdapterService.getIdToken());
    sessionStorage.clear();
    localStorage.clear();
    if (!idToken || idToken === '' || idToken === 'null') {
      this.login();
    } else {
      const logoutUrl = `${this.authConfig.logoutUrl}?id_token_hint=${idToken}&post_logout_redirect_uri=${this.authConfig.postLogoutRedirectUri}`;

      this.userSubject.next(null);
      this.authAdapterService.logOut(true);
      location.href = logoutUrl;
    }
  }

  private getAppSwitcherFromMenuItem(listAPIMenuItem: Array<any>): Array<any> {
    const arrayApp = new Array<any>();
    if (listAPIMenuItem && listAPIMenuItem.length > 0) {
      listAPIMenuItem.forEach(item => {
        const appItem = {
          label: this.getPropLocalizedData(item.localizedData, 'Name'),
          logoUrl: item.cssClass,
          mainUrl: item.path
        };

        arrayApp.push(appItem);
      });
    }
    return arrayApp;
  }

  private getMenuItemFromSiteData(siteData: SiteData): Array<any> {
    if (siteData && siteData.menus && siteData.menus.length > 0) {
      for (const listMenuAPI of siteData.menus) {
        if (listMenuAPI.type === AppConstant.menuType.MENU_ITEM) {
          return this.convertListMenuItemFromAPI(listMenuAPI);
        }
      }
    }
    return undefined;
  }

  private convertListMenuItemFromAPI(menuAPI: any): Array<any> {
    const listMenu = new Array<any>();
    const listMenuAPI = menuAPI.menuItems;
    if (listMenuAPI && listMenuAPI.length > 0) {
      for (const menuItemAPI of listMenuAPI) {
        const menuItem = this.convertMenuItemFromAPI(menuItemAPI);
        if (menuItem) {
          listMenu.push(menuItem);
        }
      }
    }
    return listMenu;
  }

  private convertMenuItemFromAPI(apiMenuItem: any): any {
    const menuItem = new CxNavbarItemModel();
    const isEmptyChildren = !(apiMenuItem.childMenuItems && apiMenuItem.childMenuItems.length > 0);
    const isNullPath = !apiMenuItem.path || apiMenuItem.path === '';

    if (isEmptyChildren && isNullPath) {
      return;
    }
    menuItem.content = this.getPropLocalizedData(apiMenuItem.localizedData, 'Name');
    if (apiMenuItem.cssClass) {
      menuItem.icon = apiMenuItem.cssClass;
      menuItem.iconActive = apiMenuItem.cssClass + '-selected';
    }
    if (apiMenuItem.path && apiMenuItem.path !== '') {
      menuItem.route = apiMenuItem.path;
      menuItem.isDefault = apiMenuItem.isDefault;
    } else {
      if (apiMenuItem.childMenuItems && apiMenuItem.childMenuItems.length > 0) {
        menuItem.children = this.getChildrenFromAPIMenuItem(apiMenuItem.childMenuItems);
      }
    }
    return menuItem;
  }

  private getChildrenFromAPIMenuItem(childAPIMenuItem: Array<MenuChildItemAPI>): Array<any> {
    const listChildMenuItem = new Array<any>();
    childAPIMenuItem.forEach(item => {
      const isEmptyChildren = !(item.childMenuItems && item.childMenuItems.length > 0);
      const isNullPath = !item.path || item.path === '';
      if (isEmptyChildren && isNullPath) {
        return;
      }
      const childMenuItem = {
        content: this.getPropLocalizedData(item.localizedData, 'Name'),
        route: item.path,
        isDefault: item.isDefault
      };
      listChildMenuItem.push(childMenuItem);
    });
    return listChildMenuItem;
  }

  private getPropLocalizedData(localizedDataArray: any, propName: string): string {
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

export declare class CxNavbarItemModel {
  id: string;
  content: string;
  children: Array<CxNavbarItemModel>;
  isCollapsed: boolean;
  route: string;
  icon: string;
  iconActive: string;
  isActive: boolean;
  isDefault: boolean;
  constructor(data?: Partial<CxNavbarItemModel>);
}
