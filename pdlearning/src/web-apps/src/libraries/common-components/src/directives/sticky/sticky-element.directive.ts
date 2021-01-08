import { Directive, ElementRef, Renderer2 } from '@angular/core';

import { BaseStickyElementDirective } from './base-sticky-element.directive';
import { interval } from 'rxjs';
import { setElementSticky } from './sticky.function';

@Directive({
  selector: '[stickyElement]',
  exportAs: 'stickyElement'
})
export class StickyElementDirective extends BaseStickyElementDirective {
  constructor(renderer: Renderer2, elementRef: ElementRef) {
    super(renderer, elementRef);
  }

  public stickyProcess(): void {
    this.subscription = interval(500).subscribe(() => {
      const stickyTop = setElementSticky(this.renderer, this.element, this.dependHtmlElement, this.stickySpacing);
      this.intervalStickyPosition(stickyTop);
    });
  }
}
