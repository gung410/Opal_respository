import { AfterViewInit, ElementRef, Input, OnDestroy, Renderer2 } from '@angular/core';

import { Subscription } from 'rxjs';

export abstract class BaseStickyElementDirective implements AfterViewInit, OnDestroy {
  @Input() public stickyDependElement?: HTMLElement | ElementRef;
  /**
   * Space between depend element and this header element/ this element
   */
  @Input() public stickySpacing: number = 0;

  @Input() public stickyEnable: boolean = true;

  public get dependHtmlElement(): HTMLElement | null {
    return this.stickyDependElement != null && this.stickyDependElement instanceof ElementRef
      ? this.stickyDependElement.nativeElement
      : this.stickyDependElement;
  }

  public get element(): HTMLElement {
    return this.elementRef.nativeElement;
  }

  protected subscription: Subscription = new Subscription();

  private numberNotInterval: number = 10;
  private stickTop: number = 0;
  private countDependNotChange: number = 0;
  constructor(public renderer: Renderer2, public elementRef: ElementRef) {}

  public ngAfterViewInit(): void {
    if (this.stickyEnable) {
      this.stickyProcess();
    }
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  // interval detect sticky position. When element dont change 10 times, will not detect sticky.
  protected intervalStickyPosition(stickyTop: number): void {
    if (this.stickTop === stickyTop) {
      this.countDependNotChange++;
    } else {
      this.countDependNotChange = 0;
    }

    if (this.countDependNotChange === this.numberNotInterval - 1) {
      this.subscription.unsubscribe();
    }

    this.stickTop = stickyTop;
  }

  protected abstract stickyProcess(): void;
}
