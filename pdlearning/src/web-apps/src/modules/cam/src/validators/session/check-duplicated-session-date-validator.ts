import { CustomFormControl, DateUtils } from '@opal20/infrastructure';

import { ValidatorFn } from '@angular/forms';
import { validateExistedSessionDateType } from './check-existed-session-date-validator';

export function checkDuplicatedSessionDateValidator(classRunRescheduleFn: () => Date[]): ValidatorFn {
  return (control: CustomFormControl) => {
    if (classRunRescheduleFn == null) {
      return null;
    }

    const sessionDates = classRunRescheduleFn();
    if (sessionDates.findIndex(p => DateUtils.removeTime(p).getTime() === DateUtils.removeTime(control.value).getTime()) > -1) {
      return { [validateExistedSessionDateType]: true };
    }
    return null;
  };
}
