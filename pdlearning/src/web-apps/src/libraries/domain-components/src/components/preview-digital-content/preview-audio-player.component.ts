import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';

@Component({
  selector: 'preview-audio-player',
  templateUrl: './preview-audio-player.component.html'
})
export class PreviewAudioPlayerComponent implements AfterViewInit {
  @Input() public src: string;

  @Output() public audioLoadEvent = new EventEmitter();

  @ViewChild('audioEl', { static: true })
  public audioEl: ElementRef;

  public handleAudioError(event: Event): void {
    this.audioLoadEvent.emit(event);
  }

  public ngAfterViewInit(): void {
    this.audioEl.nativeElement.addEventListener('contextmenu', (event: Event) => event.preventDefault());
  }
}
