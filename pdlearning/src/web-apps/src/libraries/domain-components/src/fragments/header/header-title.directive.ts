import { AfterViewInit, Directive, ElementRef, OnDestroy } from '@angular/core';

import { HeaderService } from './header.service';

@Directive({ selector: 'header-title' })
export class HeaderTitleDirective implements AfterViewInit, OnDestroy {
  constructor(private elementRef: ElementRef, private headerService: HeaderService) {}

  public ngAfterViewInit(): void {
    this.headerService.attachTitleView(this.elementRef.nativeElement);
  }

  public ngOnDestroy(): void {
    this.headerService.detachTitleView();
  }
}
