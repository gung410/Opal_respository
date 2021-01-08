import { AfterViewInit, Directive, ElementRef, EventEmitter, HostListener, Input, NgZone, Optional, Renderer2 } from '@angular/core';
import { Observable, Subject, Subscription, fromEvent } from 'rxjs';
import { TooltipDirective, TooltipSettings } from '@progress/kendo-angular-tooltip';
import { filter, takeUntil } from 'rxjs/operators';

import { CustomFormControl } from '../form/form-control';
import { FormBuilderService } from '../form/form-builder.service';
import { FormControlName } from '@angular/forms';
import { KeyCode } from '../utils/key-codes.enum';
import { PopupService } from '@progress/kendo-angular-popup';
import { Utils } from '../utils/utils';

@Directive({
  selector: '[kendoErrorTooltip]',
  providers: [TooltipSettings]
})
export class ErrorTooltipDirective extends TooltipDirective implements AfterViewInit {
  @Input()
  public formControl: CustomFormControl | undefined;
  public clearErrorsDebounce: () => void = Utils.debounce(() => {
    this.hideTooltipErrors();
  }, 300);
  public showErrorsDebounce: () => void = Utils.debounce(() => {
    this.showTooltipErrors();
  }, 300);

  private destroy$: Subject<void> = new Subject();
  private showOnErrorSubscriber: Subscription = new Subscription();

  constructor(
    protected ref: ElementRef,
    protected gZone: NgZone,
    protected renderer2: Renderer2,
    protected popup: PopupService,
    protected settings: TooltipSettings,
    protected legacySettings: TooltipSettings,
    protected formBuilderService: FormBuilderService,
    @Optional()
    private formControlName: FormControlName
  ) {
    super(ref, gZone, renderer2, popup, settings, legacySettings);

    this.position = 'right';
    this.showOn = 'none';
    this.tooltipClass = 'error-tooltip';

    // // This code override the private function of kendo directive to support not hide error when
    // // scrolling control
    // // tslint:disable-next-line:no-any
    // const _this: any = this;
    // _this.showContent = (anchorRef: ElementRef<HTMLElement>) => {
    //   if (!anchorRef.nativeElement.getAttribute('data-title') && !this.template) {
    //     return;
    //   }
    //   this.ngZone.run(() => {
    //     _this.openPopup(anchorRef);
    //     _this.popupPositionChangeSubscription.unsubscribe();
    //     _this.bindContent(this.popupRef.content, anchorRef);
    //   });
    // };
  }

  private get control(): CustomFormControl {
    return ((this.formControlName && this.formControlName.control) || this.formControl) as CustomFormControl;
  }

  @HostListener('ngModelChange', ['$event'])
  public onModelChange(event: unknown): void {
    if (this.formControlName && this.formControlName.touched) {
      this.showErrorsDebounce();
    }
  }

  public ngOnInit(): void {
    super.ngOnInit();
    fromEvent<KeyboardEvent>(this.ref.nativeElement, 'keydown')
      .pipe(
        takeUntil(this.destroy$),
        filter(event => event.keyCode !== KeyCode.Enter)
      )
      .subscribe(() => this.clearErrorsDebounce());
    fromEvent<KeyboardEvent>(this.ref.nativeElement, 'focusout')
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.showTooltipErrors();
      });
  }

  public ngAfterViewInit(): void {
    if (this.control) {
      // Prevent ExpressionChangedAfterItHasBeenCheckedError
      setTimeout(() => {
        this.subscribeShowTipOnError();
      });
    }
  }

  public ngOnDestroy(): void {
    this.hideTooltipErrors();
    super.ngOnDestroy();
    this.destroy$.next();
  }

  public hideTooltipErrors(): void {
    this.hide();
  }

  public showTooltipErrors(): void {
    // We do this because sometime, dont know why there's no subscriber in showTipOnError observable. Need to investigate more.
    this.subscribeShowTipOnErrorIfNeeded();
    if (!this.isElementVisible()) {
      this.hideTooltipErrors();
    } else if (this.control) {
      this.formBuilderService.showError(this.control, true);
    }
  }

  public get element(): HTMLElement {
    return this.ref.nativeElement;
  }

  private isElementVisible(): boolean {
    return this.element.getClientRects().length !== 0;
  }

  private subscribeShowTipOnError(): void {
    if (this.control.showTipOnError && this.control.showTipOnError instanceof Observable) {
      this.showOnErrorSubscriber = this.control.showTipOnError.pipe(takeUntil(this.destroy$)).subscribe(message => {
        if (message) {
          this.ref.nativeElement.setAttribute('data-title', message);
          this.hide();
          if (this.isElementVisible()) {
            this.show(this.ref);
          }
        } else {
          this.ref.nativeElement.removeAttribute('data-title');
          this.hide();
        }
      });
    }
  }

  private subscribeShowTipOnErrorIfNeeded(): void {
    if (
      this.control &&
      this.control.showTipOnError &&
      this.control.showTipOnError instanceof EventEmitter &&
      this.control.showTipOnError.observers.length === 0
    ) {
      this.subscribeShowTipOnError();
    }
  }
}
