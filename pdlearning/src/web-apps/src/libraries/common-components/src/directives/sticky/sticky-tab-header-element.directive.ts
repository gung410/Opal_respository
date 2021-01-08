import { Directive, ElementRef, Renderer2 } from '@angular/core';

import { BaseStickyElementDirective } from './base-sticky-element.directive';
import { interval } from 'rxjs';
import { setElementSticky } from './sticky.function';

@Directive({
  selector: '[stickyTabHeaderElement]',
  exportAs: 'stickyTabHeaderElement'
})
export class StickyTabHeaderElementDirective extends BaseStickyElementDirective {
  public get headerElement(): HTMLElement {
    return <HTMLElement>this.element.children[0];
  }

  constructor(renderer: Renderer2, elementRef: ElementRef) {
    super(renderer, elementRef);
  }

  public stickyProcess(): void {
    this.subscription = interval(500).subscribe(() => {
      const stickyTop = setElementSticky(this.renderer, this.headerElement, this.dependHtmlElement, this.stickySpacing);
      this.intervalStickyPosition(stickyTop);
    });
  }
}
