import { AsyncValidatorFn, ValidatorFn } from '@angular/forms';

import { CustomFormControl } from '@opal20/infrastructure';
import { of } from 'rxjs';

export function ifValidator(condition: (control: CustomFormControl) => boolean, validatorFn: () => ValidatorFn): ValidatorFn {
  return (control: CustomFormControl) => {
    if (!condition(control)) {
      return undefined;
    }
    return validatorFn()(control);
  };
}

export function ifAsyncValidator(
  condition: (control: CustomFormControl) => boolean,
  validatorFn: () => AsyncValidatorFn
): AsyncValidatorFn {
  return (control: CustomFormControl) => {
    const validatorResult = validatorFn()(control);
    if (validatorResult instanceof Promise) {
      if (!condition(control)) {
        return new Promise((resolve, reject) => {
          resolve(undefined);
        });
      }
      return validatorResult;
    } else {
      if (!condition(control)) {
        return of(undefined);
      }
      return validatorResult;
    }
  };
}
