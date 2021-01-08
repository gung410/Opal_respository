import { Directive, ElementRef, Renderer2 } from '@angular/core';

import { BaseStickyElementDirective } from './base-sticky-element.directive';
import { RootElementScrollableService } from '../../services/root-element-scrollable.service';
import { setGridSticky } from './sticky.function';

@Directive({
  selector: '[stickyGridHeaderElement]',
  exportAs: 'stickyGridHeaderElement'
})
export class StickyGridHeaderElementDirective extends BaseStickyElementDirective {
  public get headerElement(): HTMLElement {
    return <HTMLElement>this.element.children[0].children[0];
  }

  private _beingSticky: boolean = false;
  private _lastScrollTopPosition: number = 0;

  constructor(renderer: Renderer2, elementRef: ElementRef, private rootElementScrollableService: RootElementScrollableService) {
    super(renderer, elementRef);
  }

  public stickyProcess(): void {
    this.subscription = this.rootElementScrollableService.onScroll$.subscribe(scrollContainer => {
      const headerElement: HTMLElement = this.headerElement;

      const isScrollingUp = scrollContainer.scrollTop < this._lastScrollTopPosition;
      if (isScrollingUp && this._beingSticky) {
        const dependElementBottomPosition = this.stickyDependElement != null ? this.dependHtmlElement.getBoundingClientRect().bottom : null;
        const elementTopPosition = headerElement.getBoundingClientRect().top;
        const switchToNormalPosition =
          dependElementBottomPosition != null ? dependElementBottomPosition : scrollContainer.getBoundingClientRect().top;

        // If scroll up, and the header fixed element position relative to depend element or scroll container not correct anymore, switch back to normal.
        if (switchToNormalPosition > elementTopPosition) {
          this.renderer.setStyle(headerElement, 'position', 'static');
          this.renderer.setStyle(headerElement, 'width', 'auto');
          this.renderer.setStyle(headerElement, 'z-index', 0);
          this._beingSticky = false;
        }
      }

      const isScrollingDown = scrollContainer.scrollTop > this._lastScrollTopPosition;
      if (isScrollingDown && !this._beingSticky) {
        const dependElementBottomPosition = this.stickyDependElement != null ? this.dependHtmlElement.getBoundingClientRect().bottom : null;
        const rootElementTopPosition = scrollContainer.getBoundingClientRect().top;
        const switchToStickyPosition = dependElementBottomPosition != null ? dependElementBottomPosition : rootElementTopPosition;

        const elementTopPosition = this.element.getBoundingClientRect().top;

        if (switchToStickyPosition > elementTopPosition) {
          setGridSticky(this.renderer, headerElement, this.dependHtmlElement, this.stickySpacing);
          this._beingSticky = true;
        }
      }

      this._lastScrollTopPosition = scrollContainer.scrollTop;
    });
  }
}
