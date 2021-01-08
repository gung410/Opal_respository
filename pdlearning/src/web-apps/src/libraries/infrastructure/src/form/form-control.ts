import { AbstractControlOptions, AsyncValidatorFn, FormControl, ValidatorFn } from '@angular/forms';

import { EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';
import { TranslationMessage } from '../translation/translation.models';

/**
 * Custom form control
 * {@see FormControl} for more properties and methods.
 */
export class CustomFormControl extends FormControl {
  /**
   * Show tooltip message when occured errors
   */
  public _showTooltipOnError: EventEmitter<string | undefined> = new EventEmitter();

  /**
   * Focus control when occured errors
   */
  public _focusOnError: EventEmitter<void> = new EventEmitter();

  constructor(
    formState?: unknown,
    validatorOrOpts?: ValidatorFn | ValidatorFn[] | AbstractControlOptions | null,
    asyncValidator?: AsyncValidatorFn | AsyncValidatorFn[] | null
  ) {
    super(formState, validatorOrOpts, asyncValidator);
  }

  public get showTipOnError(): EventEmitter<string> {
    return this._showTooltipOnError;
  }

  public get focusOnError(): Observable<void> {
    return this._focusOnError;
  }

  public showTooltip(message?: TranslationMessage): void {
    this._showTooltipOnError.emit(message && message.toString());
  }

  public hideTooltip(): void {
    this._showTooltipOnError.emit(undefined);
  }

  public focus(): void {
    this._focusOnError.emit();
  }
}
