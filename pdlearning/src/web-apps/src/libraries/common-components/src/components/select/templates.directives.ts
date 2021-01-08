import { Directive, TemplateRef } from '@angular/core';

@Directive({ selector: '[opalOptionTmp]' })
export class OpalOptionTemplateDirective {
  constructor(public template: TemplateRef<unknown>) {}
}

@Directive({ selector: '[opalLabelTmp]' })
export class OpalLabelTemplateDirective {
  constructor(public template: TemplateRef<unknown>) {}
}
