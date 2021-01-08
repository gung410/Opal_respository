import { AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { Course, CourseApiService } from '@opal20/domain-api';
import { CustomFormControl, Utils } from '@opal20/infrastructure';
import { Observable, Subject, from, of } from 'rxjs';
import { delay, map, switchMap, takeUntil } from 'rxjs/operators';

export const validateExistedExternalCodeType = 'invalidExistedExternalCode';

export function checkExistedExternalCodeValidator(courseApiService: CourseApiService, courseFn: () => Course): AsyncValidatorFn {
  const cancelPreviousSub = new Subject();
  return (control: CustomFormControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
    cancelPreviousSub.next();
    const course = courseFn();
    if (Utils.isNullOrEmpty(control.value)) {
      return of(null);
    }
    return of(null).pipe(
      delay(500),
      switchMap(_ => {
        return from(courseApiService.checkExistedCourseField({ externalCode: control.value, courseId: course.id }, true)).pipe(
          takeUntil(cancelPreviousSub),
          map(existed => {
            return existed === true ? { [validateExistedExternalCodeType]: true } : null;
          })
        );
      }),
      takeUntil(cancelPreviousSub)
    );
  };
}
