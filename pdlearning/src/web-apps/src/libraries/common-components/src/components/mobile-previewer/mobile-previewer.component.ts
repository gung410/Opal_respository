import { AfterViewInit, ChangeDetectorRef, Component, ContentChild, TemplateRef } from '@angular/core';

import { MobilePreviewerContentDirective } from './mobile-previewer-content.directive';

@Component({
  selector: 'mobile-previewer',
  template: `
    <div class="container">
      <div class="content">
        <ng-container [ngTemplateOutlet]="contentTemplate"></ng-container>
      </div>
      <div class="navigation-bar"></div>
    </div>
  `
})
export class MobilePreviewerComponent implements AfterViewInit {
  public contentTemplate: TemplateRef<unknown>;

  @ContentChild(MobilePreviewerContentDirective, { static: false }) public directive: MobilePreviewerContentDirective;

  constructor(private cdr: ChangeDetectorRef) {}

  public ngAfterViewInit(): void {
    this.contentTemplate = this.directive.templateRef;
    this.cdr.detectChanges();
  }
}
