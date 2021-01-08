import { CustomFormControl } from '@opal20/infrastructure';
import { ValidatorFn } from '@angular/forms';

export const validateRequiredNumberType = 'validateRequiredNumber';

export function requiredNumberValidator(): ValidatorFn {
  return (control: CustomFormControl) => {
    if (control.value === 0) {
      return {
        [validateRequiredNumberType]: true
      };
    }

    return null;
  };
}
