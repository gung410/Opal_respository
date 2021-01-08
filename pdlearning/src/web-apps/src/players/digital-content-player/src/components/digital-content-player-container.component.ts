import { AuthDataService, AuthService, OAuthService } from '@opal20/authentication';
import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding, NgZone } from '@angular/core';
import {
  PLAYER_ACCESS_TOKEN_KEY,
  PLAYER_CLASSRUN_ID_KEY,
  PLAYER_CONTENT_ID_KEY,
  PLAYER_DISABLE_FULLSCREEN_KEY,
  PLAYER_DISPLAY_MODE_KEY,
  PLAYER_FULLSCREEN_CALLBACK_KEY,
  PLAYER_LECTURE_ID_KEY,
  PLAYER_MY_LECTURE_ID_KEY
} from '@opal20/domain-components';

@Component({
  selector: 'digital-content-player-container',
  templateUrl: './digital-content-player-container.component.html'
})
export class DigitalContentPlayerContainerComponent extends BaseComponent {
  public disableFullscreen: boolean = false;
  public fullscreenCallback: (isFullScreen: boolean) => void;
  public classRunId: string;
  public lectureId: string;
  public myLectureId: string;
  public digitalContentId: string;
  public displayMode: 'preview' | 'learn' = 'preview';

  @HostBinding('style.width')
  public width: string = '100%';
  @HostBinding('style.height')
  public height: string = '100%';

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private ngZone: NgZone,
    private authSvc: AuthService,
    private oAuthService: OAuthService,
    private authDataService: AuthDataService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    AppGlobal.digitalContentPlayerIntergrations.init = (
      accessToken: string,
      digitalContentId: string,
      lectureId: string,
      classRunId: string,
      myLectureId: string,
      displayMode: 'preview' | 'learn',
      disableFullscreen: boolean,
      fullscreenCallback: (isFullscreen: boolean) => void
    ) => {
      this.ngZone.run(() => {
        this.authSvc.setAccessToken(accessToken);
        localStorage.setItem(PLAYER_ACCESS_TOKEN_KEY, accessToken);
        localStorage.setItem(PLAYER_CONTENT_ID_KEY, digitalContentId);
        localStorage.setItem(PLAYER_LECTURE_ID_KEY, lectureId);
        localStorage.setItem(PLAYER_CLASSRUN_ID_KEY, classRunId);
        localStorage.setItem(PLAYER_MY_LECTURE_ID_KEY, myLectureId);
        localStorage.setItem(PLAYER_DISPLAY_MODE_KEY, displayMode);
        if (disableFullscreen) {
          localStorage.setItem(PLAYER_DISABLE_FULLSCREEN_KEY, disableFullscreen.toString());
        }
        if (fullscreenCallback) {
          localStorage.setItem(PLAYER_FULLSCREEN_CALLBACK_KEY, fullscreenCallback.toString());
        }
        this.signinCloudfront();
      });
    };

    const playerAccessToken: string = localStorage.getItem(PLAYER_ACCESS_TOKEN_KEY);

    if (playerAccessToken) {
      let extId: string;
      this.oAuthService.skipSubjectCheck = true;
      Promise.resolve()
        .then(() => this.authSvc.setAccessToken(playerAccessToken))
        .then(() => this.oAuthService.loadDiscoveryDocument())
        .then(() => this.oAuthService.loadUserProfile())
        .then(userProfile => {
          // tslint:disable:no-string-literal
          extId = userProfile['sub'];
          return this.authDataService.getUserProfileAsync(extId);
        })
        .then(user => {
          AppGlobal.user = user;
          AppGlobal.user['extId'] = extId;
        })
        .then(() => {
          this.digitalContentId = localStorage.getItem(PLAYER_CONTENT_ID_KEY);
          this.lectureId = localStorage.getItem(PLAYER_LECTURE_ID_KEY);
          this.classRunId = localStorage.getItem(PLAYER_CLASSRUN_ID_KEY);
          this.myLectureId = localStorage.getItem(PLAYER_MY_LECTURE_ID_KEY);
          this.displayMode = localStorage.getItem(PLAYER_DISPLAY_MODE_KEY) as 'preview' | 'learn';
          const disableFullscreenString = localStorage.getItem(PLAYER_DISABLE_FULLSCREEN_KEY);
          const fullscreenCallbackString = localStorage.getItem(PLAYER_FULLSCREEN_CALLBACK_KEY);
          this.disableFullscreen = disableFullscreenString && disableFullscreenString === 'true';
          // tslint:disable-next-line:no-eval
          this.fullscreenCallback = fullscreenCallbackString && eval(fullscreenCallbackString);
          localStorage.removeItem(PLAYER_ACCESS_TOKEN_KEY);
          localStorage.removeItem(PLAYER_CONTENT_ID_KEY);
          localStorage.removeItem(PLAYER_LECTURE_ID_KEY);
          localStorage.removeItem(PLAYER_CLASSRUN_ID_KEY);
          localStorage.removeItem(PLAYER_MY_LECTURE_ID_KEY);
          localStorage.removeItem(PLAYER_DISPLAY_MODE_KEY);
          localStorage.removeItem(PLAYER_DISABLE_FULLSCREEN_KEY);
          localStorage.removeItem(PLAYER_FULLSCREEN_CALLBACK_KEY);
        });
    }
  }

  private signinCloudfront(): void {
    const form: HTMLFormElement = document.querySelector('#cloudfront-form');
    form.action = [
      `${AppGlobal.environment.cloudfrontUrl}/api/cloudfront/signin`,
      `?returnUrl=${AppGlobal.environment.appUrl}/digital-content-player?redirect=true`
    ].join('');

    const input: HTMLInputElement = document.querySelector('#token');
    input.value = this.authSvc.getAccessToken();

    form.submit();
  }
}
