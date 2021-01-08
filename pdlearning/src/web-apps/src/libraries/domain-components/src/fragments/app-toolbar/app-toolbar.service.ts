import { EventEmitter, Injectable } from '@angular/core';

import { AppToolbarFragment } from './app-toolbar.fragment';

@Injectable()
export class AppToolbarService {
  public onShow: EventEmitter<void> = new EventEmitter();
  public onHide: EventEmitter<void> = new EventEmitter();
  private fragment!: AppToolbarFragment;

  public init(fragment: AppToolbarFragment): void {
    this.fragment = fragment;
  }

  public attachLeftView(element: HTMLElement, customContainerClass?: string): void {
    this.fragment.attachLeftView(element, customContainerClass);
  }

  public attachCenterView(element: HTMLElement, customContainerClass?: string): void {
    this.fragment.attachCenterView(element, customContainerClass);
  }

  public attachRightView(element: HTMLElement, customContainerClass?: string): void {
    this.fragment.attachRightView(element, customContainerClass);
  }

  public dettachCenterView(): void {
    this.fragment.dettachCenterView();
  }

  public detachViews(): void {
    this.fragment.detachViews();
  }

  public show(): void {
    this.fragment.show();
    this.onShow.next();
  }

  public hide(): void {
    this.fragment.hide();
    this.onHide.next();
  }

  public addClasses(names: string[]): void {
    this.fragment.addClasses(names);
  }

  public removeClasses(names: string[]): void {
    this.fragment.removeClasses(names);
  }
}
