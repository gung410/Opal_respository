import { AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { Course, CourseApiService } from '@opal20/domain-api';
import { CustomFormControl, Utils } from '@opal20/infrastructure';
import { Observable, Subject, from, of } from 'rxjs';
import { map, switchMap, takeUntil } from 'rxjs/operators';

export const validateCourseEndDateType = 'invalidCourseEndDate';

export function checkCourseEndDateValidWithClassEndDateValidator(
  courseApiService: CourseApiService,
  courseFn: () => Course
): AsyncValidatorFn {
  const cancelPreviousSub = new Subject();
  return (control: CustomFormControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
    cancelPreviousSub.next();
    const course = courseFn();
    if (Utils.isNullOrEmpty(control.value)) {
      return of(null);
    }
    return of(null).pipe(
      switchMap(_ => {
        return from(
          courseApiService.checkCourseEndDateValidWithClassEndDateValidator({ courseId: course.id, endDate: control.value }, true)
        ).pipe(
          takeUntil(cancelPreviousSub),
          map(existed => {
            return existed === false ? { [validateCourseEndDateType]: true } : null;
          })
        );
      }),
      takeUntil(cancelPreviousSub)
    );
  };
}
