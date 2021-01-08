import { AfterViewInit, Directive, ElementRef, Input } from '@angular/core';

import { DetailContentService } from './../detail-content.service';

@Directive({ selector: 'detail-content-left' })
export class DetailContentLeftDirective implements AfterViewInit {
  @Input() public customContainerClass: string = '';

  constructor(private elementRef: ElementRef, private detailContentService: DetailContentService) {}

  public ngAfterViewInit(): void {
    this.detailContentService.attachLeftView(this.elementRef.nativeElement, this.customContainerClass);
  }
}
