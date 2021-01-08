import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[filter-form-placeholder]',
})
export class FilterCatalogFormPlaceholderDirective {
  constructor(public viewContainerRef: ViewContainerRef) {}
}
