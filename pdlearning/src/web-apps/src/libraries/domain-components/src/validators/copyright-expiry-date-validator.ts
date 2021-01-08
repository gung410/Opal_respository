import { CustomFormControl } from '@opal20/infrastructure';
import { ValidatorFn } from '@angular/forms';

export const copyrightExpiryDateValidatorType = 'invalidCopyrightExpiryDate';
export function copyrightExpiryDateValidator(): ValidatorFn {
  return (control: CustomFormControl) => {
    if (!control.value) {
      return null;
    }
    if (!copyrightValidateExpiryDate(control.value)) {
      return {
        [copyrightExpiryDateValidatorType]: true
      };
    }

    return null;
  };
}

export function copyrightValidateExpiryDate(value: Date, startDate?: Date): boolean {
  const today = new Date();
  today.setHours(23, 59, 59, 999);
  return value > today && (startDate == null || value > startDate);
}
