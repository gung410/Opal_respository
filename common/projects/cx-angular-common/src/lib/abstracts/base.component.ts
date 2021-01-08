import {
  AfterViewChecked,
  AfterViewInit,
  ChangeDetectorRef,
  ElementRef,
  EventEmitter,
  OnDestroy,
  OnInit
} from '@angular/core';
import { MediaObserver } from '@angular/flex-layout';
import {
  BehaviorSubject,
  Subscription,
  Subject,
  MonoTypeOperatorFunction
} from 'rxjs';
import { CxObjectUtil } from '../utils/object.util';
import { CxProcessUtil } from '../utils/process-util';
import { takeUntil } from 'rxjs/operators';

export abstract class BaseComponent
  implements AfterViewInit, OnDestroy, OnInit, AfterViewChecked {
  public static defaultDetectChangesDelay = 100;
  public get element(): HTMLElement {
    return this.elementRef.nativeElement;
  }

  protected initiated = false;
  protected viewInitiated = false;
  protected viewChecked = false;
  protected destroyed = false;
  protected get canDetectChanges() {
    return this.initiated && !this.destroyed;
  }
  protected get defaultDetectChangesDelay() {
    return BaseComponent.defaultDetectChangesDelay;
  }
  // tslint:disable-next-line:variable-name
  protected _detectChangesDelaySubs: Subscription = new Subscription();

  public onDestroy$ = new Subject();
  public errors$: BehaviorSubject<string[] | undefined> = new BehaviorSubject<string[] | undefined>(undefined);
  // tslint:disable-next-line:variable-name
  private _errors: string[] | undefined;
  public get errors(): string[] | undefined {
    return this._errors;
  }
  public set errors(value) {
    if (!CxObjectUtil.isDifferent(this._errors, value)) {
      return;
    }
    this._errors = value;
    this.errors$.next(value);
    this.detectChanges();
  }
  public connectingApi$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  // tslint:disable-next-line:variable-name
  private _connectingApi = false;
  public get connectingApi() {
    return this._connectingApi;
  }
  public set connectingApi(value) {
    if (this._connectingApi === value) {
      return;
    }
    this._connectingApi = value;
    this.connectingApi$.next(value);
    this.detectChanges();
  }

  public get errorsAsHtml() {
    return this.getErrorsAsHtml(this.errors);
  }

  public get indentErrorsAsHtml(): string | undefined {
    return this.errors !== undefined
      ? this.errors.map(x => `&nbsp;&nbsp;&nbsp;&nbsp;- ${x}`).join('<br>')
      : undefined;
  }

  public unsubscribeSubscriptions() {
    const keys = Object.keys(this);
    const self: any = this;
    keys.forEach(key => {
      const currentKeyValue = self[key];
      if (currentKeyValue instanceof Subscription) {
        currentKeyValue.unsubscribe();
      }
    });
  }

  public untilDestroy<T>(): MonoTypeOperatorFunction<T> {
    return takeUntil(this.onDestroy$);
  }

  protected detectChanges(
    immediateOrDelay?,
    onDone?: () => any,
    checkParentForHostbinding = false
  ) {
    this._detectChangesDelaySubs.unsubscribe();
    if (!this.canDetectChanges) {
      return;
    }

    this._detectChangesDelaySubs = CxProcessUtil.delay(
      () => {
        if (this.canDetectChanges) {
          this.changeDetectorRef.detectChanges();
          if (checkParentForHostbinding) {
              this.changeDetectorRef.markForCheck();
          }
          if (onDone !== undefined) {
              onDone();
          }
        }
      },
      immediateOrDelay !== undefined
        ? immediateOrDelay
        : BaseComponent.defaultDetectChangesDelay
    );
  }

  protected getErrorsAsHtml(
    ...errorss: (string[] | undefined)[]
  ): string | undefined {
    let result = '';
    errorss.forEach(item => {
      const errors = item;
      if (errors !== undefined && errors.length) {
        result += errors.join('<br>');
      }
    })
    return result === '' ? undefined : result;
  }

  protected emitEvent<T>(event: EventEmitter<T>, value: T) {
    if (this.initiated) {
      event.emit(value);
    }
  }

  protected cancelOnDestroyDelay = (
    fn: () => any,
    immediateOrTime?: number | boolean
  ): Subscription => {
    return CxProcessUtil.delay(fn, immediateOrTime, this.onDestroy$);
  }

  constructor(
    public changeDetectorRef: ChangeDetectorRef,
    public elementRef: ElementRef,
    public media: MediaObserver
  ) {}

  ngOnInit(): void {
    this.initiated = true;
  }

  ngAfterViewInit(): void {
    this.viewInitiated = true;
  }

  ngAfterViewChecked(): void {
    this.viewChecked = true;
  }

  ngOnDestroy(): void {
    this.unsubscribeSubscriptions();
    this.onDestroy$.next();
    this.onDestroy$.complete();
    this.initiated = false;
    this.destroyed = true;
  }
}
