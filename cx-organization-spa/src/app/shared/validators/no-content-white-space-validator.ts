import { AbstractControl } from '@angular/forms';

export function noContentWhiteSpaceValidator(
  control: AbstractControl
): { [key: string]: boolean } | null {
  if (control.value && !control.value.trim().length) {
    return { whiteSpace: true };
  }

  return null;
}
