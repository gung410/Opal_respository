import { Directive, Input, OnDestroy, OnInit } from '@angular/core';

import { AppToolbarService } from '../app-toolbar.service';
import { HeaderService } from '../../header/header.service';

@Directive({ selector: 'app-toolbar' })
export class AppToolbarDirective implements OnInit, OnDestroy {
  @Input()
  public mergeHeader: boolean = false;

  @Input()
  public unmergeHeaderOnDestroy: boolean = true;

  @Input()
  public detailToolbar: boolean;

  @Input()
  public mergeBottom: boolean;

  private _customClass: string;
  public get customClass(): string {
    return this._customClass;
  }
  @Input()
  public set customClass(v: string) {
    const prevValue = this._customClass;
    this._customClass = v;
    if (this.initiated) {
      this.updateCustomClass(v, prevValue);
    }
  }

  private readonly mergedToolbarClassName: string = 'merged-toolbar';
  private readonly detailToolbarClassName: string = 'detail-toolbar';
  private readonly noBottomClassName: string = 'no-bottom';
  private initiated: boolean = false;

  constructor(private appToolbarService: AppToolbarService, private headerService: HeaderService) {}

  public ngOnInit(): void {
    if (this.mergeHeader) {
      this.headerService.addClasses([this.mergedToolbarClassName]);
    }

    const appToolbarClasses: string[] = [];

    if (this.detailToolbar) {
      appToolbarClasses.push(this.detailToolbarClassName);

      if (!this.mergeBottom) {
        appToolbarClasses.push(this.noBottomClassName);
      }
    }

    if (this.customClass) {
      appToolbarClasses.push(this.customClass);
    }

    this.appToolbarService.addClasses(appToolbarClasses);
    this.initiated = true;
  }

  public ngOnDestroy(): void {
    this.appToolbarService.detachViews();

    if (this.mergeHeader && this.unmergeHeaderOnDestroy) {
      this.headerService.removeClasses([this.mergedToolbarClassName]);
    }

    this.appToolbarService.removeClasses([this.detailToolbarClassName, this.noBottomClassName]);
  }

  private updateCustomClass(value: string, prevValue: string): void {
    if (prevValue) {
      this.appToolbarService.removeClasses([prevValue]);
    }
    if (value) {
      this.appToolbarService.addClasses([value]);
    }
  }
}
