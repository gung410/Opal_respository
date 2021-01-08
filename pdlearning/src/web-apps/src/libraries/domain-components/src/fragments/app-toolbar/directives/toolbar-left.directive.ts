import { AfterViewInit, Directive, ElementRef, Input } from '@angular/core';

import { AppToolbarService } from '../app-toolbar.service';

@Directive({ selector: 'toolbar-left' })
export class ToolbarLeftDirective implements AfterViewInit {
  @Input() public customContainerClass: string = '';

  constructor(private elementRef: ElementRef, private appToolbarService: AppToolbarService) {}

  public ngAfterViewInit(): void {
    this.appToolbarService.attachLeftView(this.elementRef.nativeElement, this.customContainerClass);
  }
}
