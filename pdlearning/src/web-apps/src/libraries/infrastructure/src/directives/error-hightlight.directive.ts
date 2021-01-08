import { AfterViewInit, Directive, ElementRef, EventEmitter, HostListener, Input, Optional } from '@angular/core';
import { Observable, Subject, Subscription, fromEvent } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { CustomFormControl } from '../form/form-control';
import { FormControlName } from '@angular/forms';
import { KeyCode } from '../utils/key-codes.enum';
import { Utils } from '../utils/utils';

@Directive({
  selector: '[errorHightLight]'
})
export class ErrorHightLightDirective implements AfterViewInit {
  @Input()
  public formControl: CustomFormControl | undefined;

  public clearErrors: () => void = Utils.debounce(() => {
    this.removeHightLightErrors();
  }, 300);

  public showErrors: () => void = Utils.debounce(() => {
    this.addHightLightErrors();
  }, 300);

  private destroy$: Subject<void> = new Subject();
  private hightLightErrorSubscriber: Subscription = new Subscription();

  constructor(
    public eleRef: ElementRef,
    @Optional()
    private formControlName: FormControlName
  ) {}

  private get control(): CustomFormControl {
    return ((this.formControlName && this.formControlName.control) || this.formControl) as CustomFormControl;
  }

  @HostListener('ngModelChange', ['$event'])
  public onModelChange(event: unknown): void {
    if (this.formControlName && this.formControlName.touched) {
      this.showErrors();
    }
  }

  public ngOnInit(): void {
    fromEvent<KeyboardEvent>(this.eleRef.nativeElement, 'keydown')
      .pipe(
        takeUntil(this.destroy$),
        filter(event => event.keyCode !== KeyCode.Enter)
      )
      .subscribe(() => {
        this.clearErrors();
      });
    fromEvent<KeyboardEvent>(this.eleRef.nativeElement, 'focusout')
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.addHightLightErrors();
      });
  }

  public ngAfterViewInit(): void {
    if (this.control) {
      setTimeout(() => {
        this.subscribeAddHightLightOnError();
      });
    }
  }

  public ngOnDestroy(): void {
    this.removeHightLightErrors();
    this.destroy$.next();
  }

  public removeHightLightErrors(): void {
    this.removeErrorClass();
  }

  public addHightLightErrors(): void {
    this.subscribeAddHightLightOnErrorIfNeeded();
    if (!this.isElementVisible()) {
      this.removeHightLightErrors();
    } else if (this.control) {
      this.addErrorClass();
    }
  }

  public get element(): HTMLElement {
    return this.eleRef.nativeElement;
  }

  private isElementVisible(): boolean {
    return this.element.getClientRects().length !== 0;
  }

  private subscribeAddHightLightOnError(): void {
    if (this.control.showTipOnError && this.control.showTipOnError instanceof Observable) {
      this.hightLightErrorSubscriber = this.control.showTipOnError.pipe(takeUntil(this.destroy$)).subscribe(message => {
        if (message) {
          this.addErrorClass();
        } else {
          this.removeErrorClass();
        }
      });
    }
  }

  private subscribeAddHightLightOnErrorIfNeeded(): void {
    if (
      this.control &&
      this.control.showTipOnError &&
      this.control.showTipOnError instanceof EventEmitter &&
      this.control.showTipOnError.observers.length === 0
    ) {
      this.subscribeAddHightLightOnError();
    }
  }

  private addErrorClass(): void {
    this.eleRef.nativeElement.classList.add('border-error');
  }

  private removeErrorClass(): void {
    this.eleRef.nativeElement.classList.remove('border-error');
  }
}
