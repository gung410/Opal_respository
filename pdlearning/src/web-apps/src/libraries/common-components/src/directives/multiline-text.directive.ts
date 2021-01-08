import { AfterViewInit, Directive, ElementRef } from '@angular/core';
/**
 * When using textarea it will convert the break line to a character
 * This directive helps to convert that a character to a break line in HTML.
 */
@Directive({
  selector: '[multilineText]'
})
export class MultilineTextDirective implements AfterViewInit {
  constructor(private ref: ElementRef) {}
  public ngAfterViewInit(): void {
    this.ref.nativeElement.innerHTML = this.ref.nativeElement.innerHTML.replace(/\n/g, '<br>');
  }
}
