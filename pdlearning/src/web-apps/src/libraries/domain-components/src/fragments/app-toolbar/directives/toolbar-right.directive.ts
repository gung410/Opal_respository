import { AfterViewInit, Directive, ElementRef, Input } from '@angular/core';

import { AppToolbarService } from '../app-toolbar.service';

@Directive({
  selector: 'toolbar-right'
})
export class ToolbarRightDirective implements AfterViewInit {
  @Input() public customContainerClass: string = '';

  constructor(private elementRef: ElementRef, private appToolbarService: AppToolbarService) {}

  public ngAfterViewInit(): void {
    this.appToolbarService.attachRightView(this.elementRef.nativeElement, this.customContainerClass);
  }
}
