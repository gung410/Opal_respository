import { CustomFormControl, DateUtils } from '@opal20/infrastructure';

import { ValidatorFn } from '@angular/forms';

export const validateFutureDateType = 'invalidFutureDate';

export function futureDateValidator(removeTime: boolean = true): ValidatorFn {
  return (control: CustomFormControl) => {
    if (control.value && DateUtils.compareDate(control.value, new Date(), !removeTime) < 0) {
      return {
        [validateFutureDateType]: true
      };
    }

    return null;
  };
}
