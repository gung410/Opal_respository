import { CustomFormControl, DateUtils } from '@opal20/infrastructure';

import { RescheduleClassRunDetailViewModel } from '@opal20/domain-components';
import { ValidatorFn } from '@angular/forms';

export const validateClassRunStartDateRescheduleWithSessionType = 'invalidClassRunStartDateRescheduleWithSession';

export function classRunStartDateRescheduleWithSessionValidator(
  classRunRescheduleFn: () => RescheduleClassRunDetailViewModel
): ValidatorFn {
  return (control: CustomFormControl) => {
    if (classRunRescheduleFn == null) {
      return null;
    }

    const classRunReschedule: RescheduleClassRunDetailViewModel = classRunRescheduleFn();

    if (classRunReschedule.sessionsData == null || classRunReschedule.sessionsData.length === 0) {
      return null;
    }

    const startDate = classRunReschedule.startDate;
    for (let i = 0; i < classRunReschedule.sessionsData.length; i++) {
      const session = classRunReschedule.sessionsData[i];
      if (session.sessionDate) {
        if (startDate && DateUtils.removeTime(session.sessionDate) < DateUtils.removeTime(startDate)) {
          return {
            [validateClassRunStartDateRescheduleWithSessionType]: true
          };
        }
      }
    }

    return null;
  };
}
