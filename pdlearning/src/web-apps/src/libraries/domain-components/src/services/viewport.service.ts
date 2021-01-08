import { Observable, fromEvent } from 'rxjs';
import { debounceTime, map } from 'rxjs/operators';

export class OpalViewportService {
  public get resize$(): Observable<number> {
    return fromEvent(window, 'resize').pipe(
      debounceTime(1000),
      map(e => window.innerWidth)
    );
  }

  public setViewPortDesktopMode(): void {
    const viewPortEl = document.querySelector('meta[name=viewport]');
    if (window.innerWidth < 1024) {
      viewPortEl.setAttribute('content', 'width=1024');
    } else if (window.innerWidth > 1024) {
      viewPortEl.setAttribute('content', 'width=device-width, initial-scale=1.0');
    }
  }
}
