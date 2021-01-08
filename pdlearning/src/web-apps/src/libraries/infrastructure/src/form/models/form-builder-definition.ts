import { FormOptions } from '../form-options';
import { IFormControlDefinition } from './form-control-definition';

export interface IFormBuilderDefinition {
  controls: IFormControlDefinition;
  formName?: string;
  options?: FormOptions;
  validateByGroupControlNames?: string[][];
}
