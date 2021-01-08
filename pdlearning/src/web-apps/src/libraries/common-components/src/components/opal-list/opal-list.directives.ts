import { Directive, TemplateRef } from '@angular/core';

@Directive({ selector: '[opalListItemTmp]' })
export class OpalListItemTemplateDirective {
  constructor(public template: TemplateRef<unknown>) {}
}
