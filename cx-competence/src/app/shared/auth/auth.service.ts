import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import {
  AppsSwitcherItem,
  CxNavbarItemModel,
  JwksValidationHandler,
  OAuthService,
  OAuthStorage,
  OAuthSuccessEvent,
} from '@conexus/cx-angular-common';
import { environment } from 'app-environments/environment';
import {
  MenuChildItemAPI,
  MenuItemAPI,
  SiteData,
  User,
} from 'app-models/auth.model';
import { Identity } from 'app-models/common.model';
import { Department } from 'app-models/department-model';
import { Header } from 'app-models/header.model';
import {
  DefaultHierarchyDepartment,
  MyTopHierarchyDepartment,
} from 'app-models/my-top-hierarchy-department-model';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { isEmpty } from 'lodash';
import { BehaviorSubject, Subject } from 'rxjs';
import {
  AppConstant,
  Constant,
  OAuthEventConstant,
  RouteConstant,
  VariableConstant,
} from '../app.constant';
import { AuthDataService } from './auth-data.service';
import { authConfig } from './auth.config';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  isAuthenticating: Subject<boolean> = new Subject();
  accessTokenSubject: Subject<string> = new Subject();
  private userSubject: BehaviorSubject<User> = new BehaviorSubject(null);

  constructor(
    private router: Router,
    private authAdapterService: OAuthService,
    private oAuthStorageService: OAuthStorage,
    private authDataService: AuthDataService
  ) {
    if (!this.isInIframe) {
      this.authAdapterService.configure(authConfig);
      this.authAdapterService.setStorage(this.oAuthStorageService);
      this.authAdapterService.tokenValidationHandler = new JwksValidationHandler();
      this.isAuthenticating.next(false);
    }
  }

  navigateToExternalSite(baseUrl: string, path: string): void {
    const navigateQueryString = encodeURIComponent(
      `${window.location.origin}${
        environment.VirtualPath && environment.VirtualPath !== ''
          ? '/' + environment.VirtualPath
          : ''
      }`
    );
    window.location.href = `${baseUrl}/${path}?returnUrl=${navigateQueryString}`;
  }

  logout(): void {
    // Prevent logout if current page is MPJ using at Learner Module
    const isHandledForIframe = this.checkAndHandleIfInAIframe();
    if (isHandledForIframe) {
      return;
    }

    this.logoutSPA();
  }

  get User(): User {
    const identityClaims = this.authAdapterService.getIdentityClaims();
    if (!identityClaims) {
      return undefined;
    }

    return new User(identityClaims);
  }

  getAccessToken(): string {
    return this.authAdapterService.getAccessToken();
  }

  setAccessToken(token: string): void {
    this.oAuthStorageService.setItem(
      AppConstant.sessionVariable.accessToken,
      token
    );
  }

  clearAccessToken(): void {
    this.oAuthStorageService.removeItem(
      AppConstant.sessionVariable.accessToken
    );
  }

  loadDiscoveryDocumentAndTryLogin(): Promise<any> {
    return new Promise((resolve, reject) => {
      this.authAdapterService
        .loadDiscoveryDocumentAndTryLogin()
        .then((res) => {
          const currentUser = this.User;
          if (!currentUser) {
            this.isAuthenticating.next(false);
            if (!this.hasValidAccessToken()) {
              this.login();
            }

            return reject();
          }

          if (currentUser.headerData) {
            this.isAuthenticating.next(false);

            return resolve(currentUser);
          }

          this.authDataService.getDataFromCurrentUser().then(
            (resp: {
              siteData: SiteData;
              departmentId: number;
              systemRoles: any;
              id: number;
              identity: Identity;
              avatarUrl: string;
              userDepartment?: Department;
              topAccessibleDepartment?: MyTopHierarchyDepartment;
              defaultHierarchyDepartment?: DefaultHierarchyDepartment;
              permissions: string[];
            }) => {
              const headerData = this.processHeaderData(resp.siteData);
              currentUser.headerData = headerData;
              currentUser.departmentId = resp.departmentId;
              currentUser.systemRoles = resp.systemRoles;
              currentUser.id = resp.id;
              currentUser.identity = resp.identity;
              currentUser.avatarUrl = resp.avatarUrl;
              currentUser.releaseDate =
                resp.siteData && resp.siteData.releaseDate;
              currentUser.userDepartment = resp.userDepartment;
              currentUser.topAccessibleDepartment =
                resp.topAccessibleDepartment;
              currentUser.defaultHierarchyDepartment =
                resp.defaultHierarchyDepartment;
              currentUser.permissions = resp.permissions;
              this.isAuthenticating.next(false);
              this.userSubject.next(currentUser);

              return resolve(currentUser);
            },
            (err) => {
              if (err && err.errorCode === Constant.CANNOT_GET_USER_INFO) {
                this.router.navigate([RouteConstant.ERROR_COMMON]);
              }
              this.isAuthenticating.next(false);

              return reject(err);
            }
          );
        })
        .catch((err) => {
          this.router.navigate([RouteConstant.ERROR_COMMON]);
          this.isAuthenticating.next(false);

          return reject(err);
        });
    });
  }

  login(): void {
    sessionStorage.clear();
    const returnUrl = localStorage.getItem(VariableConstant.RETURN_URL);
    const nonce = localStorage.getItem('nonce');
    const pkceverifier = localStorage.getItem('PKCI_verifier');
    localStorage.clear();
    //keeping the nonce and PKCI_verifier in localStorage
    if (!authConfig.disablePKCE && nonce && pkceverifier) {
      localStorage.setItem('nonce', nonce);
      localStorage.setItem('PKCI_verifier', pkceverifier);
    }
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
    localStorage.setItem(VariableConstant.RETURN_URL, returnUrl);
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
    if (
      !currentUser ||
      !currentUser.headerData ||
      !currentUser.headerData.menus
    ) {
      return false;
    }

    const listFlattenMenuItem = ObjectUtilities.flattenArray(
      currentUser.headerData.menus,
      undefined,
      true,
      undefined
    );

    const strategicThrustRoute = `/${AppConstant.siteURL.menus.strategicThrusts}`;
    if (routeURL.includes(strategicThrustRoute)) {
      return listFlattenMenuItem.some(
        (menuItem) =>
          menuItem.route && menuItem.route.includes(strategicThrustRoute)
      );
    }

    return listFlattenMenuItem.some(
      (menuItem) => menuItem.route && menuItem.matchUrl(routeURL)
    );
  }

  userData(): BehaviorSubject<User> {
    return this.userSubject;
  }

  updateUser(user: User): void {
    this.userSubject.next(user);
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
          this.accessTokenSubject.next(this.getAccessToken());
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
          if (environment.production) {
            console.error('Session terminated', JSON.stringify(e));
            this.logout();
          }
          break;
        default:
          break;
      }
    });
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

  getApplicationFromSiteData(siteData: SiteData): Array<AppsSwitcherItem> {
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

  get userDepartmentId(): number {
    const userData = this.userData().getValue();
    if (userData) {
      return userData.departmentId;
    }
  }

  private logoutSPA(): void {
    const idToken = this.authAdapterService.getIdToken();
    sessionStorage.clear();
    localStorage.clear();
    if (isEmpty(idToken)) {
      this.router.navigate(['/' + AppConstant.siteURL.login]);
    } else {
      const idTokenEncoded = encodeURIComponent(idToken);
      const logoutUrl =
        `${authConfig.logoutUrl}` +
        `?id_token_hint=${idTokenEncoded}` +
        `&post_logout_redirect_uri=${authConfig.postLogoutRedirectUri}`;
      this.authAdapterService.logOut(true);
      location.href = logoutUrl;
    }
  }

  private getAppSwitcherFromMenuItem(
    listAPIMenuItem: Array<MenuChildItemAPI>
  ): Array<AppsSwitcherItem> {
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

  private getMenuItemFromSiteData(
    siteData: SiteData
  ): Array<CxNavbarItemModel> {
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
  ): Array<CxNavbarItemModel> {
    const listMenu = new Array<CxNavbarItemModel>();
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

  private convertMenuItemFromAPI(
    apiMenuItem: MenuChildItemAPI
  ): CxNavbarItemModel {
    const menuItem = new CxNavbarItemModel();
    const isEmptyChildren = !(
      apiMenuItem.childMenuItems && apiMenuItem.childMenuItems.length > 0
    );
    const isNullPath = !apiMenuItem.path || apiMenuItem.path === '';

    if (isEmptyChildren && isNullPath) {
      return;
    }
    menuItem.content = this.getPropLocalizedData(
      apiMenuItem.localizedData,
      'Name'
    );
    if (apiMenuItem.cssClass) {
      menuItem.icon = apiMenuItem.cssClass;
      menuItem.iconActive = apiMenuItem.cssClass + '-selected';
    }
    if (apiMenuItem.path && apiMenuItem.path !== '') {
      menuItem.route = apiMenuItem.path;
      menuItem.isDefault = apiMenuItem.isDefault;
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
    childAPIMenuItem: Array<MenuChildItemAPI>
  ): Array<CxNavbarItemModel> {
    const listChildMenuItem = new Array<CxNavbarItemModel>();
    childAPIMenuItem.forEach((item) => {
      const isEmptyChildren = !(
        item.childMenuItems && item.childMenuItems.length > 0
      );
      const isNullPath = !item.path || item.path === '';
      if (isEmptyChildren && isNullPath) {
        return;
      }
      const childMenuItem = new CxNavbarItemModel();
      childMenuItem.content = this.getPropLocalizedData(
        item.localizedData,
        'Name'
      );
      childMenuItem.route = item.path;
      childMenuItem.isDefault = item.isDefault;
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

  private checkAndHandleIfInAIframe(): boolean {
    if (!this.isInIframe) {
      return false;
    }
    this.router.navigate([RouteConstant.ERROR_COMMON]);

    return true;
  }

  private get isInIframe(): boolean {
    try {
      return window.self !== window.top;
    } catch (e) {
      return true;
    }
  }
}
