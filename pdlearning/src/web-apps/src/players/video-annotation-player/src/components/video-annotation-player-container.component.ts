import { AuthDataService, AuthService, OAuthService } from '@opal20/authentication';
import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding, NgZone } from '@angular/core';

import { VideoAnnotationMode } from '@opal20/domain-components';

@Component({
  selector: 'video-annotation-player-container',
  templateUrl: './video-annotation-player-container.component.html'
})
export class VideoAnnotationPlayerContainerComponent extends BaseComponent {
  public videoId: string;
  public videoUrl: string;
  public fileType: string;
  public fileExtension: string;
  public ownerId: string;
  public mode: VideoAnnotationMode;
  public onSavedCallback: () => void;

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
    AppGlobal.videoAnnotationPlayerIntergrations.init = (
      accessToken: string,
      ownerId: string,
      videoId: string,
      videoUrl: string,
      fileType: string,
      fileExtension: string,
      mode: 'edit' | 'view' | 'learn',
      onSavedCallback?: () => void
    ) => {
      this.ngZone.run(() => {
        if (accessToken) {
          let extId: string;
          this.oAuthService.skipSubjectCheck = true;
          Promise.resolve()
            .then(() => this.authSvc.setAccessToken(accessToken))
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
              this.videoId = videoId;
              this.videoUrl = videoUrl;
              this.fileType = fileType;
              this.fileExtension = fileExtension;
              this.ownerId = ownerId;
              this.mode = this.getMode(mode);
              this.onSavedCallback = onSavedCallback;
            });
        }
      });
    };
    AppGlobal.videoAnnotationPlayerIntergrations.changeMode = (mode: 'edit' | 'view' | 'learn') => {
      this.ngZone.run(() => {
        this.mode = this.getMode(mode);
      });
    };
  }

  private getMode(mode: string): VideoAnnotationMode {
    switch (mode) {
      case 'edit':
        return VideoAnnotationMode.Management;
      case 'learn':
        return VideoAnnotationMode.Learn;
      case 'view':
      default:
        return VideoAnnotationMode.View;
    }
  }
}
