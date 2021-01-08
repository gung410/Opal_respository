import { CustomFormControl, DateUtils } from '@opal20/infrastructure';

import { ValidatorFn } from '@angular/forms';

export const validateMustBePastDateType = 'invalidMustBePastDate';

export function mustBeInThePastValidator(removeTime: boolean = true): ValidatorFn {
  return (control: CustomFormControl) => {
    if (control.value && DateUtils.compareDate(control.value, new Date(), !removeTime) > 0) {
      return {
        [validateMustBePastDateType]: true
      };
    }

    return null;
  };
}
