import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';

@Component({
  selector: 'preview-video-player',
  templateUrl: './preview-video-player.component.html'
})
export class PreviewVideoPlayerComponent implements AfterViewInit {
  @ViewChild('videoEl', { static: true })
  public videoEl: ElementRef;
  public videoSrc: string;

  @Input() public width: string = '100%';
  @Input() public height: string = 'auto';
  @Input() public set src(value: string) {
    this.videoSrc = value;
    setTimeout(() => {
      this.videoEl.nativeElement.load();
    }, 10);
  }

  public get styles(): Object {
    return {
      width: this.width,
      height: this.height,
      maxWidth: '600px'
    };
  }

  @Output() public videoLoadEvent = new EventEmitter();

  public handleVideoError(event: Event): void {
    this.videoLoadEvent.emit(event);
  }

  public ngAfterViewInit(): void {
    this.videoEl.nativeElement.addEventListener('contextmenu', (event: Event) => event.preventDefault());
  }
}
