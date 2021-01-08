import { CustomFormControl } from '@opal20/infrastructure';
import { ValidatorFn } from '@angular/forms';

export function mainSubjectRequireValidator(isRequired: () => boolean, validator: ValidatorFn): ValidatorFn {
  return (control: CustomFormControl) => {
    if (!control.parent) {
      return null;
    }
    let error = null;
    if (isRequired()) {
      error = validator(control);
    }
    return error;
  };
}
