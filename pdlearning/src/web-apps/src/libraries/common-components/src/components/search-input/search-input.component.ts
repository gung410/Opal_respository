import { Component, ElementRef, EventEmitter, Input, Output, ViewChild, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

import { Utils } from '@opal20/infrastructure';

@Component({
  selector: 'search-input',
  templateUrl: './search-input.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SearchInputComponent),
      multi: true
    }
  ]
})
export class SearchInputComponent implements ControlValueAccessor {
  public searchTerm: string;
  public propagateChange: (searchTerm: string) => {};
  public propagateTouch: () => {};
  public get isShowClearText(): boolean {
    return !Utils.isNullOrEmpty(this.searchTerm);
  }

  @Input() public placeholder: string = '';
  @Input() public disabled: boolean = false;

  @Output() public search: EventEmitter<string> = new EventEmitter<string>();

  @ViewChild('searchInput', { static: false })
  public searchInput: ElementRef;

  public writeValue(obj: string): void {
    this.searchTerm = obj;
  }

  public registerOnChange(fn: (searchTerm: string) => {}): void {
    this.propagateChange = fn;
  }

  public registerOnTouched(fn: () => {}): void {
    this.propagateTouch = fn;
  }

  public setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  public onNgModelChanged(searchTerm: string): void {
    this.searchTerm = searchTerm;
    if (!this.propagateChange) {
      return;
    }
    this.propagateChange(this.searchTerm);
    if (this.searchTerm.length === 0) {
      this.search.emit(this.searchTerm);
    }
  }

  public onSearch(event: Event): void {
    this.search.emit(this.searchTerm);
    event.preventDefault();
  }

  public onClearSearch(): void {
    this.searchInput.nativeElement.value = '';
    this.onNgModelChanged('');
  }
}
