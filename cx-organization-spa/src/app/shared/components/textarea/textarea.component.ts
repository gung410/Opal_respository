import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'opal-textarea',
  templateUrl: './textarea.component.html',
  styleUrls: ['./textarea.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OpalTextareaComponent),
      multi: true
    }
  ]
})
export class OpalTextareaComponent implements ControlValueAccessor {
  get maxNumberOfCharacters(): number {
    return this._maxNumberOfCharacters;
  }
  @Input() set maxNumberOfCharacters(value: number) {
    if (value == null) {
      return;
    }
    this._maxNumberOfCharacters = value;
  }

  get preventEnter(): boolean {
    return this._preventEnter;
  }
  @Input() set preventEnter(value: boolean) {
    if (value == null) {
      return;
    }
    this._preventEnter = value;
  }

  get height(): number {
    return this._height;
  }
  @Input() set height(value: number) {
    if (value == null) {
      return;
    }
    this._height = value;
  }

  get width(): number {
    return this._width;
  }
  @Input() set width(value: number) {
    if (value == null) {
      return;
    }
    this._width = value;
  }

  get placeholder(): string {
    return this._placeholder;
  }
  @Input() set placeholder(value: string) {
    if (value == null) {
      return;
    }
    this._placeholder = value;
  }

  get className(): string {
    return this._className;
  }
  @Input() set className(value: string) {
    if (value == null) {
      return;
    }
    this._className = value;
  }

  get styles(): any {
    return this._styles;
  }
  @Input() set styles(value: any) {
    if (value == null) {
      return;
    }
    this._styles = value;
  }

  get counterDisplay(): 'fraction' | 'charsLeft' {
    return this._counterDisplay;
  }
  @Input() set counterDisplay(value: 'fraction' | 'charsLeft') {
    if (value == null) {
      return;
    }
    this._counterDisplay = value;
  }

  get counterPosition(): 'center' | 'left' | 'right' {
    return this._counterPosition;
  }
  @Input() set counterPosition(value: 'center' | 'left' | 'right') {
    if (value == null) {
      return;
    }

    const posStyle = {
      display: 'flex',
      justifyContent: ''
    };

    switch (value) {
      case 'center': {
        posStyle.justifyContent = 'center';
        break;
      }
      case 'left': {
        posStyle.justifyContent = 'flex-start';
        break;
      }
      case 'right': {
        posStyle.justifyContent = 'flex-end';
        break;
      }
      default: {
        posStyle.justifyContent = 'flex-end';
        break;
      }
    }
    this.positionStyle = posStyle;
    this._counterPosition = value;
  }

  onChange: (data: any) => void;
  onTouched: () => void;
  isDisabled: boolean;

  textValue: string = '';
  currentNumberOfChars: number = 0;
  positionStyle: any = {};

  private _maxNumberOfCharacters: number = 100;
  private _preventEnter: boolean = false;
  private _height: number = 10;
  private _width: number = 80;
  private _placeholder: string = '';
  private _className: string = '';
  private _styles: any = '';
  private _counterDisplay: 'fraction' | 'charsLeft' = 'fraction';
  private _counterPosition: 'center' | 'left' | 'right' = 'left';

  writeValue(obj: string): void {
    if (obj) {
      this.textValue = obj;
      this.currentNumberOfChars = obj.length;
    }
  }
  registerOnChange(fn: any): void {
    // throw new Error("Method not implemented.");
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    // throw new Error("Method not implemented.");
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    throw new Error('Method not implemented.');
  }

  onModelChange(textValue: string): void {
    this.currentNumberOfChars = textValue.length;
    this.writeValue(textValue);
    this.onChange(textValue);
  }

  onEnter(event: any): void {
    if (this.preventEnter === true) {
      event.preventDefault();
    }
  }
}
