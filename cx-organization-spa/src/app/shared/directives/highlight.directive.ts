import {
  Directive,
  DoCheck,
  ElementRef,
  HostListener,
  Input
} from '@angular/core';
import { findIndexCommon } from '../constants/common.const';

@Directive({
  selector: '[Highlight]'
})
export class HighlightDirective implements DoCheck {
  @Input() highlightColor: string;
  @Input() itemId: number;
  @Input() itemIds: number[];
  @Input() highlightMode: 'selection' | 'hover' = 'hover';

  private readonly DEFAULT_COLOR: string = 'yellow';

  constructor(private el: ElementRef<HTMLElement>) {}

  ngDoCheck(): void {
    if (this.highlightMode === 'hover') {
      return;
    }
    if (!this.itemIds || !this.itemId) {
      return;
    }

    const itemIndex = this.itemIds.findIndex((id) => id === this.itemId);

    if (itemIndex === findIndexCommon.notFound) {
      this.highlight(null);
    } else {
      this.highlight(this.highlightColor || this.DEFAULT_COLOR);
    }
  }

  // tslint:disable-next-line: no-unsafe-any
  @HostListener('mouseenter') onMouseEnter(): void {
    if (this.highlightMode === 'selection') {
      return;
    }
    this.highlight(this.highlightColor || 'yellow');
  }

  // tslint:disable-next-line: no-unsafe-any
  @HostListener('mouseleave') onMouseLeave(): void {
    if (this.highlightMode === 'selection') {
      return;
    }
    this.highlight(null);
  }

  // tslint:disable-next-line: no-unsafe-any
  @HostListener('click') onMouseClick(): void {
    if (this.highlightMode === 'hover') {
      return;
    }

    if (this.itemIds || this.itemId) {
      return;
    }

    if (this.el.nativeElement.style.backgroundColor) {
      this.highlight(null);
    } else {
      this.highlight(this.highlightColor || 'yellow');
    }
  }

  private highlight(color: string): void {
    this.el.nativeElement.style.backgroundColor = color;
  }
}
