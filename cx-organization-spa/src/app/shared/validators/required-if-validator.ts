import { AbstractControl, ValidatorFn, Validators } from '@angular/forms';

export function requiredIfValidator(
  predicate: (control: AbstractControl) => boolean
): ValidatorFn {
  return (control: AbstractControl) => {
    if (!predicate(control)) {
      return undefined;
    }

    return Validators.required(control);
  };
}
