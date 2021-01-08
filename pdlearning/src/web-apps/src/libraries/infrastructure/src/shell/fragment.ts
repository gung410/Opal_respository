import { AfterViewInit, ChangeDetectorRef, ElementRef, OnInit, Renderer2 } from '@angular/core';

import { Align } from '@progress/kendo-angular-popup';
import { Subscribable } from '../subscribable';

export abstract class Fragment extends Subscribable implements OnInit, AfterViewInit {
  public defaultContextMenuAnchorAlign: Align = { horizontal: 'center', vertical: 'bottom' };
  public defaultContextMenuPopupAlign: Align = { horizontal: 'center', vertical: 'top' };

  protected abstract position: string;

  constructor(protected renderer: Renderer2, protected changeDetectorRef: ChangeDetectorRef, protected elementRef: ElementRef) {
    super();
  }

  public ngOnInit(): void {
    this.onInit();
  }

  public ngAfterViewInit(): void {
    if (this.elementRef) {
      this.renderer.addClass(this.elementRef.nativeElement, this.position);
    }

    this.onAfterViewInit();
  }

  public detectChanges(): void {
    this.changeDetectorRef.detectChanges();
  }

  public hide(): void {
    this.stopChangeDetection();
    this.renderer.setStyle(this.elementRef.nativeElement, 'display', 'none');
  }

  public show(): void {
    this.resumeChangeDetection();
    this.renderer.setStyle(this.elementRef.nativeElement, 'display', '');
  }

  public addClasses(names: string[]): void {
    names.forEach(name => this.renderer.addClass(this.elementRef.nativeElement, name));
  }

  public removeClasses(names: string[]): void {
    names.forEach(name => this.renderer.removeClass(this.elementRef.nativeElement, name));
  }

  protected onInit(): void {
    // Virtual method
  }

  protected onAfterViewInit(): void {
    // Virtual method
  }

  protected stopChangeDetection(): void {
    this.changeDetectorRef.detach();
  }

  protected resumeChangeDetection(): void {
    this.changeDetectorRef.reattach();
  }

  protected registerView(view: HTMLElement, element: HTMLElement): void {
    this.removeChildNodes(view);
    this.renderer.appendChild(view, element);
  }

  protected removeChildNodes(view: HTMLElement): void {
    for (const el of Array.from(view.children)) {
      this.renderer.removeChild(view, el);
    }
  }
}
