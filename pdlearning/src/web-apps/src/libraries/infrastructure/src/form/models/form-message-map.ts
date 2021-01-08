import { AbstractControl } from '@angular/forms';
import { TranslationMessage } from '../../translation/translation.models';

export interface IFormTranslationMessageMap {
  [controlName: string]: { [message: string]: TranslationMessage | ((control: AbstractControl) => TranslationMessage) };
}
