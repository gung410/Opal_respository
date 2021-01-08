import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'audio-player',
  templateUrl: './audio-player.component.html'
})
export class AudioPlayerComponent extends BaseComponent {
  @Input() public src: string;
  @Input() public extension: string;

  public canShowMessageForOggIos: boolean;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    const isIosDevice = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);

    this.canShowMessageForOggIos = this.extension === 'ogg' && isIosDevice;
  }
}
