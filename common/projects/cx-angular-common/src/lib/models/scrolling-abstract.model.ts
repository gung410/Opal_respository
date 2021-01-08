import { Renderer2, ElementRef, OnInit, OnDestroy } from '@angular/core';

export abstract class Scrolling implements OnInit, OnDestroy {
  lastScrolledHeight = 0;
  renderElement: Renderer2;
  elementRef: ElementRef;
  constructor(el: Renderer2, protected elRef: ElementRef) {
    this.renderElement = el;
    this.elementRef = elRef;
  }

  abstract executeDuringScrolling(): void;

  ngOnInit(): void {
    // This won't able to support for ipad and lesser
    if (this.getWidthOfWindow() > 1024) {
      window.addEventListener('scroll', this.executeDuringScrolling, true);
    }
  }
  ngOnDestroy(): void {
    window.removeEventListener('scroll', this.executeDuringScrolling);
  }

  private getWidthOfWindow(): number {
    return Math.max(
      document.body.scrollWidth,
      document.documentElement.scrollWidth,
      document.body.offsetWidth,
      document.documentElement.offsetWidth,
      document.documentElement.clientWidth
    );
  }
}
