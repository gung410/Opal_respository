import { Directive, ElementRef, Renderer2, HostListener, Input } from '@angular/core';
import { cxThrottle } from '../constants/method-decorator.constant';

@Directive({
  selector: '[cxFloatingToolbar]'
})

export class CxFloatingToolbarDirective {
  headerHeight = 128;
  elementRef: ElementRef;
  renderElement: Renderer2;
  @Input('cxFloatingToolbar') options: any;

  constructor(el: Renderer2, elRef: ElementRef) {
    this.renderElement = el;
    this.elementRef = elRef;
    const cxHeaderElement = document.querySelector(this.getPreviousElementSelector());
    if (cxHeaderElement && cxHeaderElement.clientHeight > 0) {
      this.headerHeight = cxHeaderElement.clientHeight;
    }
  }

  @HostListener('window:scroll') // tslint:disable-next-line:no-magic-numbers
  @cxThrottle(10)
  onScroll(): void {
    const currentScrollHeight = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    const floatingClass = this.getFloatingClass();
    if (currentScrollHeight > this.headerHeight && this.headerHeight > 0) {
      this.renderElement.addClass(this.elementRef.nativeElement, floatingClass);
    } else {
      this.renderElement.removeClass(this.elementRef.nativeElement, floatingClass);
    }
  }

  private getPreviousElementSelector() {
    return this.options && this.options.previousSelector ? this.options.previousSelector : '.app-header';
  }

  private getFloatingClass() {
    return this.options && this.options.floatingClass ? this.options.floatingClass : 'cx-toolbar--floating';
  }

}
