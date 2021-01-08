import { AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { CustomFormControl, Utils } from '@opal20/infrastructure';
import { Observable, Subject, from, of } from 'rxjs';
import { Session, SessionApiService } from '@opal20/domain-api';
import { delay, map, switchMap, takeUntil } from 'rxjs/operators';

export const validateExistedSessionDateType = 'invalidateExistedSessionDate';

export function checkExistedSessionDateValidator(
  sessionApiService: SessionApiService,
  sessionFn: () => Session,
  classRunId: string
): AsyncValidatorFn {
  const cancelPreviousSub = new Subject();
  return (control: CustomFormControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
    cancelPreviousSub.next();
    const session = sessionFn();
    if (Utils.isNullOrEmpty(control.value)) {
      return of(null);
    }
    return of(null).pipe(
      delay(500),
      switchMap(_ => {
        return from(
          sessionApiService.checkExistedSessionField({ sessionDate: control.value, sessionId: session.id, classRunId: classRunId }, true)
        ).pipe(
          takeUntil(cancelPreviousSub),
          map(existed => {
            return existed === true ? { [validateExistedSessionDateType]: true } : null;
          })
        );
      }),
      takeUntil(cancelPreviousSub)
    );
  };
}
