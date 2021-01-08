import { Directive, TemplateRef } from '@angular/core';

@Directive({
  selector: '[editableEditMode]'
})
export class EditModeDirective {
  constructor(public tpl: TemplateRef<unknown>) {}
}
