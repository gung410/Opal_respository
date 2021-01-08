import { AfterViewInit, Directive, ElementRef, Input } from '@angular/core';

import { DetailContentService } from '../detail-content.service';

@Directive({
  selector: 'detail-content-right'
})
export class DetailContentRightDirective implements AfterViewInit {
  @Input() public customContainerClass: string = '';

  constructor(private elementRef: ElementRef, private detailContentService: DetailContentService) {}

  public ngAfterViewInit(): void {
    this.detailContentService.attachRightView(this.elementRef.nativeElement, this.customContainerClass);
  }
}
