import * as videoJS from 'video.js';

import { AmazonS3UploaderService, BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';

import { PlatformHelper } from '../../utils/platform.utils';
import { VideojsPlayerCustom } from '../../models/videojs-player-custom.model';
import { registerVideoJsChaptersPlugin } from './videojs-chapter-plugin';
// tslint:disable:no-string-literal
const videojs = videoJS.default;
@Component({
  selector: 'videojs-player',
  templateUrl: './videojs-player.component.html'
})
export class VideojsPlayerComponent extends BaseComponent {
  @ViewChild('videoEl', { static: true })
  public videoEl: ElementRef;

  @Input() public options: videojs.VideoJsPlayerOptions;
  @Input() public extension: string;

  @Input() public disableFullscreen: boolean = false;
  @Input() public fullscreenCallback: (isFullScreen: boolean) => void;

  @Output() public videoErrorEvent: EventEmitter<Event> = new EventEmitter<Event>();
  @Output() public videojsReady: EventEmitter<VideojsPlayerCustom> = new EventEmitter<VideojsPlayerCustom>();
  @Output() public videoTimeUpdate: EventEmitter<number> = new EventEmitter<number>();
  @Output() public duration: EventEmitter<number> = new EventEmitter<number>();

  public player: VideojsPlayerCustom;
  public canShowMessageForOgvIos: boolean;

  constructor(moduleFacadeService: ModuleFacadeService, private uploaderService: AmazonS3UploaderService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.loadVideo();
    this.checkMessageForDevice();
  }

  public onAfterViewInit(): void {
    this.videoEl.nativeElement.addEventListener('contextmenu', (event: Event) => event.preventDefault());
  }

  public onDestroy(): void {
    this.disposeVideojs();
  }

  public handleVideoError(): void {
    this.loadVideoSourceFromS3
      .bind(this)()
      .then(src => {
        this.player.src({ ...this.options.sources[0], src });
      });
  }

  private loadVideo(): void {
    if (PlatformHelper.isIOSDevice()) {
      this.loadVideoSourceFromS3().then(src => {
        this.options.sources[0].src = src;
        this.setupVideojs();
      });
      return;
    }
    this.setupVideojs();
  }

  private loadVideoSourceFromS3(): Promise<string> {
    const src = this.options.sources[0].src;
    const isCloudfrontStorage = src && src.indexOf != null && src.indexOf(AppGlobal.environment.cloudfrontUrl) > -1;
    if (isCloudfrontStorage) {
      const originSrcUrl = src.replace(`${AppGlobal.environment.cloudfrontUrl}/`, '');
      return this.uploaderService.getFile(originSrcUrl).then(resp => {
        return resp;
      });
    }
    return Promise.resolve(src);
  }

  private setupVideojs(): void {
    this.player = videojs(this.videoEl.nativeElement, this.options, () => {
      this.player.load();
      this.videojsReady.emit(this.player);
      this.listenTimeUpdate();
      this.listenVideoDuration();
      this.listenFullscreen();
      this.updateFullscreenAbility();
    }) as VideojsPlayerCustom;
  }

  private listenVideoDuration(): void {
    this.videoEl.nativeElement.onloadeddata = () => {
      const duration = Math.floor(this.videoEl.nativeElement.duration);
      this.duration.emit(duration);
    };
  }

  private listenTimeUpdate(): void {
    this.player.on('timeupdate', () => {
      this.videoTimeUpdate.emit(this.player.currentTime());
    });
  }

  private listenFullscreen(): void {
    if (!this.fullscreenCallback) {
      return;
    }
    PlatformHelper.setMobileViewport();
    if (!this.disableFullscreen) {
      this.player.on('fullscreenchange', () => {
        this.fullscreenCallback(this.player.isFullscreen());
      });
    }
    this.player.on('enterFullWindow', () => {
      this.fullscreenCallback(true);
    });
    this.player.on('exitFullWindow', () => {
      this.fullscreenCallback(false);
    });
  }

  private updateFullscreenAbility(): void {
    if (!this.disableFullscreen) {
      return;
    }
    delete this.player['fsApi_'].exitFullscreen;
    delete this.player['fsApi_'].requestFullscreen;
    this.player['tech_'].supportsFullScreen = () => false;
  }

  private disposeVideojs(): void {
    if (this.player) {
      this.player.dispose();
    }
  }

  private checkMessageForDevice(): void {
    const isIosDevice = PlatformHelper.isIOSDevice();

    this.canShowMessageForOgvIos = this.extension === 'ogv' && isIosDevice;
  }
}

export function registerVideojsPlugins(): void {
  videojs.registerPlugin('chapters', registerVideoJsChaptersPlugin);
}
