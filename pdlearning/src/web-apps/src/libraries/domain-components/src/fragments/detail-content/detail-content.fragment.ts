import { ChangeDetectorRef, Component, ElementRef, Renderer2, ViewChild, ViewContainerRef } from '@angular/core';

import { DetailContentService } from './detail-content.service';
import { Fragment } from '@opal20/infrastructure';
import { FragmentPosition } from '../fragment-position';

@Component({
  selector: 'detail-content-fragment',
  templateUrl: './detail-content.fragment.html'
})
export class DetailContentFragment extends Fragment {
  public static originalLeftContainerClass: string = 'detail-content__left-container';
  public static originalRightContainerClass: string = 'detail-content__right-container';

  public leftContainerClass: string = DetailContentFragment.originalLeftContainerClass;
  public rightContainerClass: string = DetailContentFragment.originalRightContainerClass;

  protected position: string = FragmentPosition.DetailContentContainer;

  @ViewChild('leftContent', { read: ViewContainerRef, static: true })
  private leftContent!: ViewContainerRef;

  @ViewChild('rightContent', { read: ViewContainerRef, static: true })
  private rightContent!: ViewContainerRef;

  constructor(
    protected renderer: Renderer2,
    protected changeDetectorRef: ChangeDetectorRef,
    protected elementRef: ElementRef,
    private detailContentService: DetailContentService
  ) {
    super(renderer, changeDetectorRef, elementRef);

    this.detailContentService.init(this);
  }

  public attachLeftView(element: HTMLElement, customContainerClass?: string): void {
    this.registerView(this.leftContent.element.nativeElement, element);
    setTimeout(() => {
      this.leftContainerClass =
        `${DetailContentFragment.originalLeftContainerClass}` + (customContainerClass !== undefined ? ` ${customContainerClass}` : '');
    });
  }

  public attachRightView(element: HTMLElement, customContainerClass?: string): void {
    this.registerView(this.rightContent.element.nativeElement, element);
    setTimeout(() => {
      this.rightContainerClass =
        `${DetailContentFragment.originalRightContainerClass}` + (customContainerClass !== undefined ? ` ${customContainerClass}` : '');
    });
  }

  public detachViews(): void {
    this.removeChildNodes(this.leftContent.element.nativeElement);
    this.removeChildNodes(this.rightContent.element.nativeElement);
  }
}
