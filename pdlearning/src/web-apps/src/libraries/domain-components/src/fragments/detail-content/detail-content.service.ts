import { EventEmitter, Injectable } from '@angular/core';

import { DetailContentFragment } from './detail-content.fragment';

@Injectable()
export class DetailContentService {
  public onShow: EventEmitter<void> = new EventEmitter();
  public onHide: EventEmitter<void> = new EventEmitter();
  private fragment!: DetailContentFragment;

  public init(fragment: DetailContentFragment): void {
    this.fragment = fragment;
  }

  public attachLeftView(element: HTMLElement, customContainerClass?: string): void {
    this.fragment.attachLeftView(element, customContainerClass);
  }

  public attachRightView(element: HTMLElement, customContainerClass?: string): void {
    this.fragment.attachRightView(element, customContainerClass);
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

  public currentFragment(): DetailContentFragment {
    return this.fragment;
  }
}
