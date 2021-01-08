import { CustomFormControl, DateUtils } from '@opal20/infrastructure';

import { ValidatorFn } from '@angular/forms';

export function equalValidator<T extends Date>(
  errorKey: string,
  firstFn: (control: CustomFormControl) => T,
  secondFn: (control: CustomFormControl) => T,
  checkDatePart: 'default' | 'dateOnly' | 'timeOnly' = 'default'
): ValidatorFn {
  return (control: CustomFormControl) => {
    const first = firstFn(control);
    const second = secondFn(control);

    if (checkDatePart === 'default') {
      if (first === second) {
        return {
          [errorKey]: true
        };
      }
    } else if (checkDatePart === 'dateOnly') {
      if (DateUtils.compareOnlyDate(first, second) === 0) {
        return {
          [errorKey]: true
        };
      }
    } else if (checkDatePart === 'timeOnly') {
      if (DateUtils.compareTimeOfDate(first, second) === 0) {
        return {
          [errorKey]: true
        };
      }
    }
    return null;
  };
}
