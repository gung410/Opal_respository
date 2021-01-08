import { APP_BASE_HREF, CommonModule, PlatformLocation } from '@angular/common';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { AuthenticationError, InfrastructureModule, ShellModule } from '@opal20/infrastructure';
import { DomainComponentsModule, PLAYER_LOCAL_STORAGE_KEYS } from '@opal20/domain-components';
import { cloudfrontCheckingKey, redirectModuleKey } from './app.config';

import { AppComponent } from './app.component';
import { AuthService } from '@opal20/authentication';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { CommonComponentsModule } from '@opal20/common-components';
import { CustomAppShell } from './custom-app-shell';
import { DomainApiModule } from '@opal20/domain-api';
import { NgIdleKeepaliveModule } from '@ng-idle/keepalive';

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    ShellModule,
    InfrastructureModule.forRoot(),
    DomainComponentsModule.forRoot(),
    CommonComponentsModule.forRoot(),
    DomainApiModule.forRoot(),
    NgIdleKeepaliveModule
  ],
  declarations: [AppComponent, CustomAppShell],
  entryComponents: [CustomAppShell],
  exports: [CustomAppShell],
  bootstrap: [AppComponent],
  providers: [
    {
      provide: APP_BASE_HREF,
      useFactory: (pl: PlatformLocation) => pl.getBaseHrefFromDOM(),
      deps: [PlatformLocation]
    },
    {
      provide: APP_INITIALIZER,
      useFactory: authenticate,
      deps: [AuthService, APP_BASE_HREF],
      multi: true
    }
  ]
})
export class AppModule {}

export function verifyIgnorePath(): boolean {
  const currentPath = location.pathname;
  const ignoredPaths = AppGlobal.environment.authConfig.ignoredPaths;

  const isIgnoreAuthPath = ignoredPaths.includes(currentPath);
  const isStartWithIgnorePath = ignoredPaths.findIndex(item => currentPath.startsWith(item)) !== -1;

  return isIgnoreAuthPath || isStartWithIgnorePath;
}

export function authenticate(authService: AuthService, baseHref: string): () => Promise<void> {
  const moduleId: string = AppGlobal.getModuleIdFromUrl(baseHref);
  /**
   * The issue is single sign-on issue between multiple spa such as SAM, Learner, PDPM...
   * This bug was found as 08 March 2020.
   * Please don't blame us because of the following line of code.
   *
   * https://cxtech.atlassian.net/browse/OP-5216
   * The issue is signle sign-out between mulitple tabs.
   * When we open another tab, It will remove the 'session_state' then the checkSession will be errored. Then it stop the session checking.
   * The related tabs won't know when the others signout.
   * Will consider to add 'session_state' to whitelist after having a better solution for storing jwt stuffs.
   */
  const whitelistLocalStorage: string[] = ['nonce', 'PKCI_verifier', ...PLAYER_LOCAL_STORAGE_KEYS];

  if (location.pathname !== `${baseHref}index.html` && !localStorage.getItem(cloudfrontCheckingKey)) {
    Object.keys(localStorage).forEach(key => {
      if (key === redirectModuleKey || key === cloudfrontCheckingKey || whitelistLocalStorage.indexOf(key) > -1) {
        return;
      } else {
        localStorage.removeItem(key);
      }
    });
  }

  const isIgnorePath = verifyIgnorePath();
  if (isIgnorePath) {
    authService.oAuthStorageService.switchToMemoryStorage();
    return () => Promise.resolve();
  }

  return () =>
    Promise.resolve()
      .then(() => authService.loadDiscoveryDocumentAndTryLogin(AppGlobal.environment.authConfig.redirectUri))
      .then(data => (AppGlobal.user = data))
      .then(() => authService.login())
      .then(() => {
        if (location.pathname === `${baseHref}index.html` && !localStorage.getItem(cloudfrontCheckingKey)) {
          const form: HTMLFormElement = document.querySelector('#cloudfront-form');
          form.action = `${AppGlobal.environment.cloudfrontUrl}/api/cloudfront/signin?returnUrl=${encodeURIComponent(location.href)}`;

          const input: HTMLInputElement = document.querySelector('#token');
          input.value = authService.getAccessToken();

          localStorage.setItem(cloudfrontCheckingKey, '.');

          form.submit();
        } else {
          localStorage.removeItem(cloudfrontCheckingKey);
          authService.oAuthStorageService.switchToMemoryStorage();
        }

        return Promise.resolve();
      })
      .then(() => authService.loadModulePermissions())
      .then(permissions => {
        AppGlobal.permissions = permissions;
        return Promise.resolve();
      })
      .catch(() => {
        if (moduleId) {
          localStorage.setItem(redirectModuleKey, moduleId);
        }

        if (authService.isLogged) {
          return Promise.resolve();
        }

        throw new AuthenticationError('Unexpected authentication error occurs.');
      });
}
