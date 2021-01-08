import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'video-player',
  templateUrl: './video-player.component.html'
})
export class VideoPlayerComponent extends BaseComponent {
  @Input() public width: string = 'auto';
  @Input() public height: string = 'auto';
  @Input() public src: string;
  @Input() public extension: string;

  public canShowMessageForOgvIos: boolean;

  public get styles(): Object {
    return {
      width: this.width,
      height: this.height,
      'max-width': '100%'
    };
  }

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    const isIosDevice = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);

    this.canShowMessageForOgvIos = this.extension === 'ogv' && isIosDevice;
  }
}
