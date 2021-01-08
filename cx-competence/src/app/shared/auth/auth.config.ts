// This api will come in the next version
import { environment } from 'environments/environment';
import { AuthConfig } from '@conexus/cx-angular-common';
export const authConfig: AuthConfig = {
  strictDiscoveryDocumentValidation: false,
  requireHttps: false,
  responseType: 'code',
  // Url of the Identity Provider
  issuer: environment.issuer,
  logoutUrl: environment.issuer + '/connect/endsession',

  postLogoutRedirectUri: environment.VirtualPath
    ? `${window.location.origin}/${environment.VirtualPath}`
    : window.location.origin,
  // URL of the SPA to redirect the user to after login
  redirectUri: environment.VirtualPath
    ? `${window.location.origin}/${environment.VirtualPath}/index.html`
    : window.location.origin + '/index.html',
  // URL of the SPA to redirect the user after silent refresh
  silentRefreshRedirectUri: environment.VirtualPath
    ? `${window.location.origin}/${environment.VirtualPath}/silent-refresh.html`
    : window.location.origin + '/silent-refresh.html',

  // The SPA's id. The SPA is registered with this id at the auth-server
  clientId: environment.clientId,

  // set the scope for the permissions the client should request
  // The first three are defined by OIDC. The 4th is a usecase-specific one
  scope: 'profile cxprofile openid cxDomainInternalApi userManagement',
  disablePKCE: false,
  showDebugInformation: !environment.production,
  sessionChecksEnabled: true,
  sessionCheckIntervall: 2000,
  skipIssuerCheck: true,
};
