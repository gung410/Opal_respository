import { AfterViewInit, Directive, ElementRef, Input } from '@angular/core';

@Directive({
  selector: '[cxFocus]'
})
export class FocusDirective implements AfterViewInit {
  // tslint:disable-next-line:no-input-rename
  @Input() set focusIf(condition: boolean) {
    if (condition === true) {
      this.focus();

      return;
    }

    this.unfocus();
  }

  private isAutoFocus: boolean = true;

  constructor(public element: ElementRef<HTMLElement>) {}

  ngAfterViewInit(): void {
    if (this.isAutoFocus) {
      setTimeout(() => this.element.nativeElement.focus(), 0);
    }
  }

  private focus(): void {
    setTimeout(() => this.element.nativeElement.focus(), 0);
    // this.element.nativeElement.focus();
  }

  private unfocus(): void {
    setTimeout(() => this.element.nativeElement.blur(), 0);
    // this.element.nativeElement.blur();
  }
}
