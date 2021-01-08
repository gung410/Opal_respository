import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding, NgZone } from '@angular/core';
import {
  PLAYER_ACCESS_TOKEN_KEY,
  PLAYER_CONTENT_ID_KEY,
  PLAYER_DISPLAY_MODE_KEY,
  PLAYER_MY_LECTURE_ID_KEY
} from '@opal20/domain-components';

import { AuthService } from '@opal20/authentication';

@Component({
  selector: 'scorm-player-container',
  templateUrl: './scorm-player-container.component.html'
})
export class ScormPlayerContainerComponent extends BaseComponent {
  public digitalContentId: string;
  public myLectureId: string;
  public displayMode: 'preview' | 'learn' = 'preview';
  public onScormFinish: () => void;

  @HostBinding('style.width')
  public width: string = '100%';
  @HostBinding('style.height')
  public height: string = '100%';

  constructor(protected moduleFacadeService: ModuleFacadeService, private ngZone: NgZone, private authSvc: AuthService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    AppGlobal.scormPlayerIntegrations.init = (
      accessToken: string,
      digitalContentId: string,
      myLectureId: string,
      displayMode: 'preview' | 'learn',
      onScormFinish?: () => void
    ) => {
      this.ngZone.run(() => {
        this.authSvc.setAccessToken(accessToken);
        localStorage.setItem(PLAYER_ACCESS_TOKEN_KEY, accessToken);
        localStorage.setItem(PLAYER_CONTENT_ID_KEY, digitalContentId);
        localStorage.setItem(PLAYER_MY_LECTURE_ID_KEY, myLectureId);
        localStorage.setItem(PLAYER_DISPLAY_MODE_KEY, displayMode);
        this.signinCloudfront();
        this.onScormFinish = onScormFinish;
      });
    };
    AppGlobal.scormPlayerIntegrations.registerFinishHandler = (onScormFinish: () => void) => {
      this.onScormFinish = onScormFinish;
    };

    const playerAccessToken: string = localStorage.getItem(PLAYER_ACCESS_TOKEN_KEY);

    if (playerAccessToken) {
      this.authSvc.setAccessToken(playerAccessToken);
      this.digitalContentId = localStorage.getItem(PLAYER_CONTENT_ID_KEY);
      this.myLectureId = localStorage.getItem(PLAYER_MY_LECTURE_ID_KEY);
      this.displayMode = localStorage.getItem(PLAYER_DISPLAY_MODE_KEY) as 'preview' | 'learn';
      localStorage.removeItem(PLAYER_ACCESS_TOKEN_KEY);
      localStorage.removeItem(PLAYER_CONTENT_ID_KEY);
      localStorage.removeItem(PLAYER_MY_LECTURE_ID_KEY);
      localStorage.removeItem(PLAYER_DISPLAY_MODE_KEY);
    }
  }

  private signinCloudfront(): void {
    const form: HTMLFormElement = document.querySelector('#cloudfront-form');
    form.action = [
      `${AppGlobal.environment.cloudfrontUrl}/api/cloudfront/signin`,
      `?returnUrl=${AppGlobal.environment.appUrl}/scorm-player?redirect=true`
    ].join('');

    const input: HTMLInputElement = document.querySelector('#token');
    input.value = this.authSvc.getAccessToken();

    form.submit();
  }
}
