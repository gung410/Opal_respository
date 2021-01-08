import { AbstractControl, AsyncValidatorFn, ValidatorFn } from '@angular/forms';

import { TranslationMessage } from '../../translation/translation.models';

export type ValidatorType = 'min' | 'max' | 'required' | 'minlength' | 'maxlength' | 'pattern' | string;

export interface IValidatorDefinition {
  validator: ValidatorFn | AsyncValidatorFn;
  message?: TranslationMessage | ((control: AbstractControl) => TranslationMessage);
  validatorType?: ValidatorType;
  isAsync?: boolean;
}

export interface IBusinessValidator {
  validate(): boolean;
}

export interface IBusinessValidatorDefinition {
  [key: string]: IBusinessValidator[];
}
