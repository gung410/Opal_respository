import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Directive, ElementRef, HostListener, Input, OnInit, Renderer2, forwardRef } from '@angular/core';

const TRIM_VALUE_ACCESSOR = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => AutoTrimDirective),
  multi: true
};

@Directive({
  selector: 'input[autoTrim], textarea[autoTrim]',
  providers: [TRIM_VALUE_ACCESSOR]
})
export class AutoTrimDirective implements ControlValueAccessor, OnInit {
  @Input('autoTrimOn') public onEvent: 'keyup' | 'focusout';
  constructor(private _renderer: Renderer2, private _elementRef: ElementRef) {}
  public _onChange(_: string | number | undefined): void {
    // Default function
  }

  public _onTouched(): void {
    // Default function
  }

  public registerOnChange(fn: (value: string | number | undefined) => void | string | number | undefined): void {
    this._onChange = fn;
  }
  public registerOnTouched(fn: () => void | string | number | undefined): void {
    this._onTouched = fn;
  }

  public writeValue(value: string | number | undefined): void {
    if (value != null) {
      this._renderer.setProperty(this._elementRef.nativeElement, 'value', value);
    }
  }

  public ngOnInit(): void {
    this.onEvent = this.onEvent || 'focusout';
  }

  @HostListener('keyup', ['$event'])
  @HostListener('focusout', ['$event'])
  public _onKeyUp(event: Event): void {
    if (this.onEvent === event.type) {
      const element = <HTMLInputElement>event.target;
      const val = element.value.trim();
      this.writeValue(val);
      this._onChange(val);
    }
  }
}
