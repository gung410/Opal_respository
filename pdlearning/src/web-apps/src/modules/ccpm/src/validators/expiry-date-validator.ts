import { CustomFormControl } from '@opal20/infrastructure';
import { ValidatorFn } from '@angular/forms';

export const validateExpiryDateType = 'invalidExpiryDate';
export function expiryDateValidator(): ValidatorFn {
  return (control: CustomFormControl) => {
    if (!control.value) {
      return null;
    }

    if (!validateExpiryDate(control.value)) {
      return {
        [validateExpiryDateType]: true
      };
    }

    return null;
  };
}

export function validateExpiryDate(value: Date): boolean {
  const today = new Date();
  today.setHours(23, 59, 59, 999);
  return value > today;
}
