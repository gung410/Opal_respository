import { HeaderFragment } from './header.fragment';
import { Injectable } from '@angular/core';

@Injectable()
export class HeaderService {
  public moduleName: string = '';
  private _returnHomeCallback: (() => void) | undefined;
  private fragment!: HeaderFragment;

  public get returnHomeCallback(): (() => void) | undefined {
    return this._returnHomeCallback;
  }

  public set returnHomeCallback(value: (() => void) | undefined) {
    this._returnHomeCallback = value;
    if (this.fragment) {
      this.fragment.detectChanges();
    }
  }

  public init(fragment: HeaderFragment): void {
    this.fragment = fragment;
  }

  public attachTitleView(element: HTMLElement): void {
    this.fragment.attachTitleView(element);
  }

  public detachTitleView(): void {
    this.fragment.detachTitleView();
  }

  public addClasses(names: string[]): void {
    this.fragment.addClasses(names);
  }

  public removeClasses(names: string[]): void {
    this.fragment.removeClasses(names);
  }

  public show(): void {
    this.fragment.show();
  }

  public hide(): void {
    this.fragment.hide();
  }
}
