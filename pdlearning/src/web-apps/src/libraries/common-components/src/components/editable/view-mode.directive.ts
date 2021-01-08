import { Directive, TemplateRef } from '@angular/core';

@Directive({
  selector: '[editableViewMode]'
})
export class ViewModeDirective {
  constructor(public tpl: TemplateRef<unknown>) {}
}
