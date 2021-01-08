import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[filter-form-placeholder]',
})
export class FilterFormPlaceholderDirective {
  constructor(public viewContainerRef: ViewContainerRef) {}
}
