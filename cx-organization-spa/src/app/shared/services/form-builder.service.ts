import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Injectable()
export class FormBuilderService extends FormBuilder {
  updateAllValidators(group: FormGroup): void {
    Object.keys(group.controls).forEach((key: string) => {
      const abstractControl = group.get(key);

      if (abstractControl instanceof FormGroup) {
        this.updateAllValidators(abstractControl);
      } else {
        abstractControl.updateValueAndValidity();
      }
    });
  }
}
