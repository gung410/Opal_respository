import { ChangeDetectorRef, Component, ElementRef, Renderer2, ViewChild, ViewContainerRef } from '@angular/core';

import { AppToolbarService } from './app-toolbar.service';
import { Fragment } from '@opal20/infrastructure';
import { FragmentPosition } from '../fragment-position';

@Component({
  selector: 'app-toolbar-fragment',
  templateUrl: './app-toolbar.fragment.html'
})
export class AppToolbarFragment extends Fragment {
  public static originalLeftContainerClass: string = 'toolbar__left-container';
  public static originalCenterContainerClass: string = 'toolbar__center-container';
  public static originalRightContainerClass: string = 'toolbar__right-container';

  public leftContainerClass: string = AppToolbarFragment.originalLeftContainerClass;
  public centerContainerClass: string = AppToolbarFragment.originalCenterContainerClass;
  public rightContainerClass: string = AppToolbarFragment.originalRightContainerClass;

  protected position: string = FragmentPosition.AppToolbar;

  @ViewChild('leftContent', { read: ViewContainerRef, static: true })
  private leftContent!: ViewContainerRef;

  @ViewChild('centerContent', { read: ViewContainerRef, static: true })
  private centerContent!: ViewContainerRef;

  @ViewChild('rightContent', { read: ViewContainerRef, static: true })
  private rightContent!: ViewContainerRef;

  constructor(
    protected renderer: Renderer2,
    protected changeDetectorRef: ChangeDetectorRef,
    protected elementRef: ElementRef,
    private appToolbarService: AppToolbarService
  ) {
    super(renderer, changeDetectorRef, elementRef);

    this.appToolbarService.init(this);
  }

  public attachLeftView(element: HTMLElement, customContainerClass?: string): void {
    this.registerView(this.leftContent.element.nativeElement, element);
    setTimeout(() => {
      this.leftContainerClass =
        `${AppToolbarFragment.originalLeftContainerClass}` + (customContainerClass !== undefined ? ` ${customContainerClass}` : '');
    });
  }

  public attachCenterView(element: HTMLElement, customContainerClass?: string): void {
    this.registerView(this.centerContent.element.nativeElement, element);
    setTimeout(() => {
      this.centerContainerClass =
        `${AppToolbarFragment.originalCenterContainerClass}` + (customContainerClass !== undefined ? ` ${customContainerClass}` : '');
    });
  }

  public dettachCenterView(): void {
    this.removeChildNodes(this.centerContent.element.nativeElement);
  }

  public attachRightView(element: HTMLElement, customContainerClass?: string): void {
    this.registerView(this.rightContent.element.nativeElement, element);
    setTimeout(() => {
      this.rightContainerClass =
        `${AppToolbarFragment.originalRightContainerClass}` + (customContainerClass !== undefined ? ` ${customContainerClass}` : '');
    });
  }

  public detachViews(): void {
    this.removeChildNodes(this.leftContent.element.nativeElement);
    this.removeChildNodes(this.centerContent.element.nativeElement);
    this.removeChildNodes(this.rightContent.element.nativeElement);
  }
}
