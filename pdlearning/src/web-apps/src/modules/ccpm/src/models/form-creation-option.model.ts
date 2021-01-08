import { FormType } from '@opal20/domain-api';
import { TranslationMessage } from '@opal20/infrastructure';

export interface IFormCreationOption {
  text: string | TranslationMessage;
  value: FormType;
  click: (clickedOption: IFormCreationOption) => void;
}
