import { CustomFormControl } from '@opal20/infrastructure';
import { ValidatorFn } from '@angular/forms';

export const validateNoContentWhitespaceType = 'invalidNoContentWhitespace';

export function noContentWhitespaceValidator(): ValidatorFn {
  return (control: CustomFormControl) => {
    const hasWhitesSpace = control.value && control.value.length !== control.value.replace(/\s/g, '').length;

    if (hasWhitesSpace) {
      return { [validateNoContentWhitespaceType]: true };
    }

    return null;
  };
}
