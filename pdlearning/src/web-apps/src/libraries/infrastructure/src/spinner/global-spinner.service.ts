import { DomUtils as DOMUtils } from '../utils/dom.utils';
import { Injectable } from '@angular/core';
import { SpinnerService } from './spinner.service';

@Injectable()
export class GlobalSpinnerService extends SpinnerService {
  private _hasGlobalSpinner: boolean;

  constructor() {
    super();
    this.onSignal$.subscribe(() => (this._hasGlobalSpinner = true));
    this.offSignal$.subscribe(() => (this._hasGlobalSpinner = false));

    /**
     * Prevent keydown event when global spinner is showing.
     */
    document.addEventListener('keydown', event => this.handleGlobalSpinnerEvent(event), true);
    document.addEventListener('mousedown', event => this.handleGlobalSpinnerEvent(event));
  }

  private handleGlobalSpinnerEvent(event: KeyboardEvent | MouseEvent): void {
    if (this._hasGlobalSpinner === true) {
      DOMUtils.preventDefaultEvent(event);
    }
  }
}
