import { CustomFormControl, DateUtils } from '@opal20/infrastructure';

import { ValidatorFn } from '@angular/forms';

export function startEndValidator<T extends number | Date>(
  errorKey: string,
  startFn: (control: CustomFormControl) => T,
  endFn: (control: CustomFormControl) => T,
  allowEqual: boolean = true,
  checkDatePart: 'default' | 'dateOnly' | 'timeOnly' = 'default',
  condition?: (control: CustomFormControl) => T
): ValidatorFn {
  return (control: CustomFormControl) => {
    if (condition != null && !condition(control)) {
      return undefined;
    }
    const start = startFn(control);
    const end = endFn(control);
    if (typeof start === 'number' && typeof end === 'number') {
      if ((allowEqual && start > end) || (!allowEqual && start >= end)) {
        return {
          [errorKey]: true
        };
      }
    } else if (start instanceof Date && end instanceof Date) {
      if (checkDatePart === 'default') {
        if ((allowEqual && start > end) || (!allowEqual && start >= end)) {
          return {
            [errorKey]: true
          };
        }
      } else if (checkDatePart === 'dateOnly') {
        if ((allowEqual && DateUtils.compareOnlyDate(start, end) > 0) || (!allowEqual && DateUtils.compareOnlyDate(start, end) >= 0)) {
          return {
            [errorKey]: true
          };
        }
      } else if (checkDatePart === 'timeOnly') {
        if ((allowEqual && DateUtils.compareTimeOfDate(start, end) > 0) || (!allowEqual && DateUtils.compareTimeOfDate(start, end) >= 0)) {
          return {
            [errorKey]: true
          };
        }
      }
    }

    return null;
  };
}
