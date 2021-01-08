import { Directive, ElementRef, Renderer2 } from '@angular/core';
import { SpinnerStyle, SpinnerType } from './spinner.models';

import { GlobalSpinnerService } from './global-spinner.service';
import { SpinnerService } from './spinner.service';
import { Subscribable } from '../subscribable';

@Directive({
  selector: '[spinner]'
})
export class SpinnerDirective extends Subscribable {
  private _style: SpinnerStyle = 'primary';
  private _element: HTMLElement;
  private _spinner: HTMLElement;
  private _showType: SpinnerType = '';

  constructor(
    private elementRef: ElementRef,
    private renderer: Renderer2,
    private spinnerService: SpinnerService,
    private globalSpinnerSerivce: GlobalSpinnerService
  ) {
    super();

    this._element = this.elementRef.nativeElement;
    this.subscribe(this.spinnerService.onSignal$, this.onShow.bind(this));
    this.subscribe(this.spinnerService.offSignal$, this.onHide.bind(this));
    this.subscribe(this.globalSpinnerSerivce.onSignal$, this.onShowGlobal.bind(this));
    this.subscribe(this.globalSpinnerSerivce.offSignal$, this.onHideGlobal.bind(this));
  }

  private generateSpinnerTemplate(): HTMLElement {
    const template: string = `
      <div class="spinner__container">
        <div class="spinner__part part-1 animation scale">
          <div class="animation rotate">
            <div class="hightlight"></div>
          </div>
        </div>
        <div class="spinner__part part-2 animation scale">
          <div class="animation rotate">
            <div class="hightlight"></div>
          </div>
        </div>
        <div class="spinner__part part-4 animation scale">
          <div class="animation rotate">
            <div class="hightlight"></div>
          </div>
        </div>
        <div class="spinner__part part-3 animation scale">
          <div class="animation rotate">
            <div class="hightlight"></div>
          </div>
        </div>
      </div>
    `;

    const spinner: HTMLDivElement = this.renderer.createElement('div');

    spinner.innerHTML = template;
    this.renderer.addClass(spinner, 'spinner');
    this.renderer.addClass(spinner, this._style);

    return spinner;
  }

  private setPosition(global: boolean = false): void {
    const position: string = window.getComputedStyle(this._element).position;

    if (!global && position === 'static') {
      this.renderer.setStyle(this._element, 'position', 'relative');
    }
  }

  private onShow(): void {
    this.setPosition();
    this.renderer.addClass(this._element, 'show-spinner');
    this._spinner = this.generateSpinnerTemplate();
    this.renderer.appendChild(this._element, this._spinner);
    this._showType = 'local';
  }

  private onHide(): void {
    if (this._showType === 'local') {
      this.renderer.removeClass(this._element, 'show-spinner');
      this.renderer.removeChild(this._element, this._spinner);
      this._showType = '';
    }
  }

  private onShowGlobal(): void {
    if (document.body.classList.contains('show-spinner')) {
      return;
    }

    this.renderer.addClass(document.body, 'show-spinner');
    this.setPosition(true);
    this._spinner = this.generateSpinnerTemplate();
    this.renderer.appendChild(document.body, this._spinner);
    this._showType = 'global';
  }

  private onHideGlobal(): void {
    if (this._showType === 'global') {
      this.renderer.removeClass(document.body, 'show-spinner');
      this.renderer.removeChild(document.body, this._spinner);
      this._showType = '';
    }
  }
}
