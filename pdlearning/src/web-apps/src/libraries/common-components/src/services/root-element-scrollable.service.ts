import { Observable, Subject } from 'rxjs';

import { Injectable } from '@angular/core';

@Injectable()
export class RootElementScrollableService {
  private _onScroll$: Subject<HTMLElement> = new Subject();
  private currentElement: HTMLElement;

  public get onScroll$(): Observable<HTMLElement> {
    return this._onScroll$.asObservable();
  }

  public emitScroll(htmlElement: HTMLElement): void {
    this.currentElement = htmlElement;
    this._onScroll$.next(htmlElement);
  }

  public scrollToTop(): void {
    if (this.currentElement != null) {
      this.currentElement.scrollTo({
        top: 0,
        behavior: 'smooth'
      });
    }
  }
}
