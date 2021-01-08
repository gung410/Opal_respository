import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[popupContent]'
})
export class PopupContentDirective {
  constructor(public viewContainerRef: ViewContainerRef) {}
}
