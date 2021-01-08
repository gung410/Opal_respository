import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { AuthService } from 'app-auth/auth.service';
import { AppConstant } from '../app.constant';

@Injectable()
export class AuthGuardService implements CanActivate {
  constructor(private router: Router, private authService: AuthService) {}

  /**
   * Checks whether the navigating route is allowed or not.
   * If not, then store the navigating route into local storage for further usage (returning after logging in).
   * @param route The activated route snapshot
   * @param state The router state snapshot
   */
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    // For not item route url
    if (!this.authService.checkIsMenuRoute(state.url)) {
      // Seems like this would never reach. TODO: Check and remove it.
      return true;
    }

    const loginRoute = '/' + AppConstant.siteURL.login;
    const hasValidAccessToken = this.authService.hasValidAccessToken();
    if (!hasValidAccessToken) {
      // Stores the navigating route into local storage for further usage (returning after logging in).
      this.authService.setReturnUrl(state.url);
      let redirectPath = loginRoute;

      // redirect to session timeout page if user idle session is timeout
      const isUserIdleSessionTimeout = sessionStorage.getItem(
        AppConstant.sessionVariable.sessionTimeout
      );
      if (isUserIdleSessionTimeout === 'true') {
        redirectPath = '/' + AppConstant.siteURL.sessionTimeout;
        sessionStorage.removeItem(AppConstant.sessionVariable.sessionTimeout);
      }

      // Incase we the returnUrl which SPA app have sent to the IDM
      // for redirect back after login success is difference with /index.html
      // We need to append the id_token and access_token fragment to login component for handling Authentication user
      this.router.navigate([redirectPath], {
        queryParams: state.root.queryParams,
      });

      return false;
    }

    // Check right to access menu
    if (!this.authService.hasRightToAccessMenu(state.url)) {
      this.router.navigateByUrl('/');

      return false;
    }

    return true;
  }
}
