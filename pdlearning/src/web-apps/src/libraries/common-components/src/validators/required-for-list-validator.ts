import { CustomFormControl } from '@opal20/infrastructure';
import { ValidatorFn } from '@angular/forms';

export function requiredForListValidator(condition?: (control: CustomFormControl) => boolean): ValidatorFn {
  return (control: CustomFormControl) => {
    if (condition != null && !condition(control)) {
      return undefined;
    }
    return control.value == null || control.value.length === 0 ? { required: true } : null;
  };
}
