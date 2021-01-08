import { CustomFormControl } from '@opal20/infrastructure';
import { ValidatorFn } from '@angular/forms';

export const copyrightStartDateValidatorType = 'invalidCopyrightStartDate';
export function copyrightStartDateValidator(): ValidatorFn {
  return (control: CustomFormControl) => {
    if (!control.value) {
      return null;
    }
    if (!copyrightValidateStartDate(control.value)) {
      return {
        [copyrightStartDateValidatorType]: true
      };
    }

    return null;
  };
}

export function copyrightValidateStartDate(value: Date): boolean {
  return true;
}
