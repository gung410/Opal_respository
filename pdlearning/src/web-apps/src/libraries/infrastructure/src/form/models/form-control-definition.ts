import { IValidatorDefinition } from './validator-definition';

export interface IFormControlDefinition {
  [key: string]: IFormControl;
}

export interface IFormControl {
  defaultValue?: unknown;
  validators?: IValidatorDefinition[];
  manualTrackChange?: boolean;
}
