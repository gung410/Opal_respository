import { FormGroup } from '@angular/forms';
import { FormOptions } from './form-options';
import { IFormTranslationMessageMap } from './models/form-message-map';

// tslint:disable:no-any

export class CustomFormGroup extends FormGroup {
  public name?: string;
  public isRoot?: boolean;
  public originalValue?: any;
  public options?: FormOptions;
  public translationMessageMap?: IFormTranslationMessageMap;
  public validateByGroupControlNames?: string[][];
}
