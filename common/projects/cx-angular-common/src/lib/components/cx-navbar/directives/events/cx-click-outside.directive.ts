import { Directive, ElementRef, EventEmitter, Output, HostListener } from '@angular/core';

@Directive({
  selector: '[cxClickOutside]'
})
export class CxClickOutsideDirective {

  constructor(private _elementRef : ElementRef) {
  }

  @Output()
    public clickOutside = new EventEmitter();

  @HostListener('document:click', ['$event.target'])
    public onClick(targetElement) {
      const clickedInside = this._elementRef.nativeElement.contains(targetElement);
      if (!clickedInside) {
        this.clickOutside.emit(null);
      }
    }
}