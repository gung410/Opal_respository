import { AbstractControl, ValidatorFn } from '@angular/forms';

export function ifValidator(
  predicate: (control: AbstractControl) => boolean,
  validatorFn: () => ValidatorFn
): ValidatorFn {
  return (
    control: AbstractControl
  ): { [key: string]: boolean } | null | undefined => {
    if (!predicate(control)) {
      return undefined;
    }

    return validatorFn()(control);
  };
}
