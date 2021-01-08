import {
  ChangeDetectorRef,
  Component,
  OnDestroy,
  ViewEncapsulation,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CxGlobalLoaderService,
  CxNavbarItemModel,
} from '@conexus/cx-angular-common';
import { authConfig } from 'app-auth/auth.config';
import { AuthService } from 'app-auth/auth.service';
import { environment } from 'app-environments/environment';
import { DefaultMenuParam } from 'app-models/default-menu-param';
import { RouteConfigService } from 'app-services/route-config.service';
import { VariableConstant } from 'app/shared/app.constant';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';

@Component({
  template: '',
  styles: [],
  encapsulation: ViewEncapsulation.None,
})
export class LoginComponent extends BaseSmartComponent implements OnDestroy {
  constructor(
    private router: Router,
    private authService: AuthService,
    private routeConfigService: RouteConfigService,
    private globalLoader: CxGlobalLoaderService,
    private route: ActivatedRoute,
    protected changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);
    this.globalLoader.showLoader();
  }

  ngOnInit(): void {
    //skip take-off page when switching app
    const switchingAppParam = this.route.snapshot.queryParamMap.get(
      'switching'
    );
    //skip login page by configuration
    const skipLoginParam = this.route.snapshot.queryParamMap.get('skip');
    if (switchingAppParam === 'true' || skipLoginParam === 'true') {
      this.login();

      return;
    }
    const returnUrl = localStorage.getItem(VariableConstant.RETURN_URL);
    if (this.authService.hasValidAccessToken() === true) {
      if (!!returnUrl) {
        // Found the return url, then navigate to it.
        this.router.navigate([returnUrl]);

        return;
      } else {
        // Find the default route based on the access of the user.
        const currentUser = this.authService.userData().getValue();
        const currentMenus = currentUser.headerData.menus;
        const defaultRoute = this.findDefaultPage(currentMenus);
        if (defaultRoute) {
          this.router.navigateByUrl(defaultRoute);
        }

        return;
      }
    }
    const returnUrlPathMap = localStorage.getItem(
      VariableConstant.RETURN_URL_PATH_MAP
    );

    if (
      returnUrl != null &&
      this.routeConfigService.SendInRedirectUrlRoutePaths.indexOf(
        returnUrlPathMap
      ) >= 0
    ) {
      authConfig.redirectUri = environment.VirtualPath
        ? `${window.location.origin}/${environment.VirtualPath}${returnUrl}`
        : window.location.origin + returnUrl;
    } else {
      authConfig.redirectUri = environment.VirtualPath
        ? `${window.location.origin}/${environment.VirtualPath}/index.html`
        : window.location.origin + '/index.html';
    }
  }

  ngOnDestroy() {
    this.globalLoader.hideLoader();
  }

  login(): void {
    this.globalLoader.showLoader();
    this.authService.login();
  }

  private findDefaultPage(currentMenus: CxNavbarItemModel[]): string {
    const defaultPage = '';
    if (currentMenus && currentMenus.length > 0) {
      const params: DefaultMenuParam = new DefaultMenuParam();

      for (const menu of currentMenus) {
        this.findDefaultOnMenu(menu, params);
        if (params.defaultMenu) {
          return params.defaultMenu.route;
        }
        for (const childMenu of menu.children) {
          this.findDefaultOnMenu(childMenu, params);
          if (params.defaultMenu) {
            return params.defaultMenu.route;
          }
        }
      }

      return params.firstValidMenu.route;
    }

    return defaultPage;
  }

  private findDefaultOnMenu(
    menu: CxNavbarItemModel,
    dataParams: DefaultMenuParam
  ): void {
    if (!dataParams.firstValidMenu && menu.route && menu.route !== '') {
      dataParams.firstValidMenu = menu;
    }

    if (menu.isDefault === true && !dataParams.defaultMenu) {
      dataParams.defaultMenu = menu;
    }
  }
}
