import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'rating-stars',
  templateUrl: './rating-stars.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RatingStarsComponent),
      multi: true
    }
  ]
})
export class RatingStarsComponent extends BaseComponent implements ControlValueAccessor {
  @Input()
  public size: 'tiny' | 'sm' | 'lg' = 'sm';
  @Input()
  public clickable: boolean = false;
  public onChange?: (_: unknown) => void;
  public onTouched?: (_: unknown) => void;

  public get value(): number {
    return this._value;
  }
  public set value(v: number) {
    if (v !== this._value) {
      this._value = v;
    }
  }

  public mouserOverStar1: boolean = false;
  public mouserOverStar2: boolean = false;
  public mouserOverStar3: boolean = false;

  private _value: number;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public writeValue(value?: number): void {
    this._value = value;
    if (this.onChange != null) {
      this.onChange(value);
    }
  }
  public registerOnChange(fn: (_: unknown) => void): void {
    this.onChange = fn;
  }
  public registerOnTouched(fn: (_: unknown) => void): void {
    this.onTouched = fn;
  }

  public onStarClick(event: number): void {
    if (this.clickable) {
      this.writeValue(event);
    }
  }
}
