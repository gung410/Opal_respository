import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, ValidationErrors } from '@angular/forms';

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

  getFormValidationErrors(group: FormGroup): void {
    Object.keys(group.controls).forEach((key) => {
      const controlErrors: ValidationErrors = group.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach((keyError) => {
          console.log(`keyControl: ${key}`);
          console.log(`keyError: ${keyError}`);
          console.log(`keyValue: ${controlErrors[keyError]}`);
          console.log('-----------------------');
        });
      }
    });
  }
}
