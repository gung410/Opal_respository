import { AuthConfig } from '@conexus/cx-angular-common';
import { environment } from 'environments/environment';

export const authConfig: AuthConfig = {
  issuer: environment.issuer,

  // URL of the SPA to redirect the user to after login
  redirectUri: window.location.origin + '/index.html',
  logoutUrl: environment.issuer + '/connect/endsession',
  postLogoutRedirectUri: window.location.origin,

  // URL of the SPA to redirect the user after silent refresh
  silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',

  // The SPA's id. The SPA is registered with this id at the auth-server
  clientId: environment.clientId,
  responseType: 'code',
  // set the scope for the permissions the client should request
  // The first three are defined by OIDC. The 4th is a usecase-specific one
  scope: 'profile cxprofile openid cxDomainInternalApi userManagement',
  disablePKCE: false,
  showDebugInformation: true,
  sessionChecksEnabled: true,
  sessionCheckIntervall: 2 * 1000,
  skipIssuerCheck: true
};
