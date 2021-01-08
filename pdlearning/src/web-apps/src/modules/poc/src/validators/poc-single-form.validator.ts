import { CustomFormGroup, IBusinessValidator } from '@opal20/infrastructure';

import { IPOCSingleForm } from '../models/poc-single-form';

export class POCSingleFormValidator implements IBusinessValidator {
  constructor(public form: CustomFormGroup) {}

  public validate(): boolean {
    const value: IPOCSingleForm = this.form.value;

    return value.controlA > value.controlB;
  }
}
