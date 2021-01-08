import { CustomFormControl, DateUtils } from '@opal20/infrastructure';

import { RescheduleClassRunDetailViewModel } from '@opal20/domain-components';
import { ValidatorFn } from '@angular/forms';

export const validateClassRunEndDateRescheduleWithSessionType = 'invalidClassRunEndDateRescheduleWithSession';

export function classRunEndDateRescheduleWithSessionValidator(classRunRescheduleFn: () => RescheduleClassRunDetailViewModel): ValidatorFn {
  return (control: CustomFormControl) => {
    if (classRunRescheduleFn == null) {
      return null;
    }

    const classRunReschedule: RescheduleClassRunDetailViewModel = classRunRescheduleFn();

    if (classRunReschedule.sessionsData == null || classRunReschedule.sessionsData.length === 0) {
      return null;
    }

    const endDate = classRunReschedule.endDate;
    for (let i = 0; i < classRunReschedule.sessionsData.length; i++) {
      const session = classRunReschedule.sessionsData[i];
      if (session.sessionDate) {
        if (endDate && DateUtils.removeTime(session.sessionDate) > DateUtils.removeTime(endDate)) {
          return {
            [validateClassRunEndDateRescheduleWithSessionType]: true
          };
        }
      }
    }

    return null;
  };
}
