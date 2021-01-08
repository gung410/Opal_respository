import { Directive, TemplateRef } from '@angular/core';

@Directive({
  selector: '[mobilePreviewerContent]'
})
export class MobilePreviewerContentDirective {
  constructor(public templateRef: TemplateRef<unknown>) {}
}
