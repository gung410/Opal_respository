import { ValidatorFn, Validators } from '@angular/forms';

import { CustomFormControl } from '@opal20/infrastructure';

export function requiredIfValidator(condition: (control: CustomFormControl) => boolean): ValidatorFn {
  return (control: CustomFormControl) => {
    if (!condition(control)) {
      return undefined;
    }
    return Validators.required(control);
  };
}
