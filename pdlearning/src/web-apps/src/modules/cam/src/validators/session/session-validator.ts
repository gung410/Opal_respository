import { DateUtils, IValidatorDefinition, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { futureDateValidator, ifValidator, startEndValidator, validateFutureDateType } from '@opal20/common-components';

import { Session } from '@opal20/domain-api';
import { Validators } from '@angular/forms';

export class SessionValidator {
  public static getSessionDateValidators(
    classRunStartDateTimeFn: () => Date,
    classRunEndDateTimeFn: () => Date,
    sessionFn: () => Session,
    moduleFacadeService: ModuleFacadeService
  ): IValidatorDefinition[] {
    return [
      {
        validator: Validators.required,
        validatorType: 'required'
      },
      {
        validator: startEndValidator('sessionStartTimeWithClassStartTime', p => classRunStartDateTimeFn(), p => p.value, true, 'dateOnly'),
        validatorType: 'sessionStartTimeWithClassStartTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'Session date must be in the time frame of the classrun')
      },
      {
        validator: startEndValidator('sessionStartTimeWithClassEndTime', p => p.value, p => classRunEndDateTimeFn(), true, 'dateOnly'),
        validatorType: 'sessionStartTimeWithClassEndTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'Session date must be in the time frame of the classrun')
      },
      {
        validator: ifValidator(p => Utils.isDifferent(p.value, sessionFn().sessionDate), () => futureDateValidator()),
        validatorType: validateFutureDateType,
        message: new TranslationMessage(moduleFacadeService.translator, 'Session Date cannot be in the past')
      }
    ];
  }

  public static getSessionStartTimeValidators(
    classRunStartDateTimeFn: () => Date,
    classRunEndDateTimeFn: () => Date,
    sessionFn: () => Session,
    originalSessionFn: () => Session,
    moduleFacadeService: ModuleFacadeService
  ): IValidatorDefinition[] {
    return [
      {
        validator: Validators.required,
        validatorType: 'required'
      },
      {
        validator: ifValidator(
          p => sessionFn().sessionDate != null,
          () =>
            startEndValidator(
              'sessionStartTimeWithPlanningStartTime',
              p => classRunStartDateTimeFn(),
              p => DateUtils.buildDateTime(sessionFn().sessionDate, p.value),
              true,
              'default'
            )
        ),
        validatorType: 'sessionStartTimeWithPlanningStartTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'Session time must be in the time frame of the classrun')
      },
      {
        validator: ifValidator(
          p => sessionFn().sessionDate != null,
          () =>
            startEndValidator(
              'sessionStartTimeWithPlanningEndTime',
              p => DateUtils.buildDateTime(sessionFn().sessionDate, p.value),
              p => classRunEndDateTimeFn(),
              true,
              'default'
            )
        ),
        validatorType: 'sessionStartTimeWithPlanningEndTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'Session time must be in the time frame of the classrun')
      },
      {
        validator: ifValidator(
          p => sessionFn().sessionDate != null && Utils.isDifferent(originalSessionFn().sessionDate, sessionFn().sessionDate),
          () =>
            startEndValidator(
              'sessionStartTimeWithCurrentTime',
              p => new Date(),
              p => DateUtils.buildDateTime(sessionFn().sessionDate, p.value),
              true,
              'default'
            )
        ),
        validatorType: 'sessionStartTimeWithCurrentTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'Session start time must be in the future')
      },
      {
        validator: startEndValidator('sessionStartTime', p => p.value, p => sessionFn().endTime, true, 'timeOnly'),
        validatorType: 'sessionStartTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'Start Time cannot be greater than End Time')
      }
    ];
  }

  public static getSessionEndTimeValidators(
    classRunStartDateTimeFn: () => Date,
    classRunEndDateTimeFn: () => Date,
    sessionFn: () => Session,
    moduleFacadeService: ModuleFacadeService
  ): IValidatorDefinition[] {
    return [
      {
        validator: Validators.required,
        validatorType: 'required'
      },
      {
        validator: ifValidator(
          p => sessionFn().sessionDate != null,
          () =>
            startEndValidator(
              'sessionEndTimeWithPlanningStartTime',
              p => classRunStartDateTimeFn(),
              p => DateUtils.buildDateTime(sessionFn().sessionDate, p.value),
              true,
              'default'
            )
        ),
        validatorType: 'sessionEndTimeWithPlanningStartTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'Session time must be in the time frame of the classrun')
      },
      {
        validator: ifValidator(
          p => sessionFn().sessionDate != null,
          () =>
            startEndValidator(
              'sessionEndTimeWithPlanningEndTime',
              p => DateUtils.buildDateTime(sessionFn().sessionDate, p.value),
              p => classRunEndDateTimeFn(),
              true,
              'default'
            )
        ),
        validatorType: 'sessionEndTimeWithPlanningEndTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'Session time must be in the time frame of the classrun')
      },
      {
        validator: startEndValidator('sessionEndTime', p => sessionFn().startTime, p => p.value, true, 'timeOnly'),
        validatorType: 'sessionEndTime',
        message: new TranslationMessage(moduleFacadeService.translator, 'End Time cannot be less than Start Time')
      }
    ];
  }
}
