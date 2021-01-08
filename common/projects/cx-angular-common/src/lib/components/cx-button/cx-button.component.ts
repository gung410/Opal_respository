import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { MediaObserver } from '@angular/flex-layout';

import { BaseComponent } from '../../abstracts/base.component';

@Component({
  selector: 'cx-button',
  templateUrl: './cx-button.component.html',
  styleUrls: ['./cx-button.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxButtonComponent extends BaseComponent implements AfterViewInit {
  public static Types = {
    primary: 'primary',
    secondary: 'secondary',
    success: 'success',
    danger: 'danger',
    warning: 'warning',
    info: 'info',
    outlinePrimary: 'outline-primary',
    outlineSecondary: 'outline-secondary',
    outlineSuccess: 'outline-success',
    outlineDanger: 'outline-danger',
    outlineWarning: 'outline-warning',
    outlineInfo: 'outline-info'
  };

  @ViewChild('mainBtn')
  public mainBtn: ElementRef;

  @Input()
  public type: string | undefined;
  @Input()
  public size: string | undefined;
  @Input()
  public tabIndex: number | undefined;
  @Input() iconClass: string;

  private enabledVal = true;
  public get enabled() {
    return this.enabledVal;
  }
  @Input() public set enabled(value) {
    this.enabledVal = value;
    this._setEnabledElementStyle();
  }

  @Output()
  public closed: EventEmitter<any> = new EventEmitter<any>();

  @Output()
  public clicked: EventEmitter<any> = new EventEmitter<any>();

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    elementRef: ElementRef,
    media: MediaObserver
  ) {
    super(changeDetectorRef, elementRef, media);
  }

  ngAfterViewInit(): void {
    super.ngAfterViewInit();
    this._setEnabledElementStyle();
  }

  public onCloseBtnClicked(e: any) {
    this.closed.emit(e);
  }

  public onBtnClicked(e: any) {
    this.clicked.emit(e);
  }

  public focus() {
    const mainBtnElement = this.mainBtn.nativeElement as HTMLElement;
    mainBtnElement.focus();
  }

  private _setEnabledElementStyle() {
    this.element.style.pointerEvents = this.enabled ? 'auto' : 'none';
  }
}
