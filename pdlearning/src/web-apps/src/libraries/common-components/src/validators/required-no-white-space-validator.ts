import { ValidatorFn, Validators } from '@angular/forms';

import { CustomFormControl } from '@opal20/infrastructure';

export function requiredAndNoWhitespaceValidator(): ValidatorFn {
  return (control: CustomFormControl) => {
    const isWhitesSpace = (control.value || '').trim().length === 0;

    if (isWhitesSpace) {
      return { required: true };
    }
    return Validators.required(control);
  };
}
