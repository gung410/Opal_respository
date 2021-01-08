import { AfterViewInit, Directive, ElementRef, Input, Renderer2 } from '@angular/core';

import { AppToolbarService } from '../app-toolbar.service';

@Directive({ selector: 'toolbar-center' })
export class ToolbarCenterDirective implements AfterViewInit {
  @Input() public customContainerClass: string = '';

  constructor(private elementRef: ElementRef, private renderer: Renderer2, private appToolbarService: AppToolbarService) {}

  public ngAfterViewInit(): void {
    this.renderer.addClass(this.elementRef.nativeElement, 'flex');
    this.appToolbarService.attachCenterView(this.elementRef.nativeElement, this.customContainerClass);
  }
}
