import {
  AfterViewInit,
  ChangeDetectorRef,
  ElementRef,
  OnDestroy,
  OnInit,
  Renderer2
} from "@angular/core";
import { Subscription, Subject, MonoTypeOperatorFunction } from "rxjs";
import { takeUntil } from "rxjs/operators";

export abstract class BaseDirective
  implements OnInit, AfterViewInit, OnDestroy {
  public viewInitiated = false;
  public initiated = false;
  public destroyed = false;
  public onDestroy$: Subject<any> = new Subject();

  constructor(
    protected elementRef: ElementRef,
    protected renderer: Renderer2,
    protected changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.initiated = true;
  }

  ngAfterViewInit(): void {
    this.viewInitiated = true;
  }

  ngOnDestroy(): void {
    this.unsubscribeSubscriptions();
    this.onDestroy$.next();
    this.onDestroy$.complete();
    this.initiated = false;
    this.destroyed = true;
  }

  public untilDestroy<T>(): MonoTypeOperatorFunction<T> {
    return takeUntil(this.onDestroy$);
  }
  public get element(): HTMLElement {
    return this.elementRef.nativeElement;
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
}
