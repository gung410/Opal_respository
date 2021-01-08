import { IValidatorDefinition } from '@opal20/infrastructure';

export class DetailTitleSettings {
  public subTitlePosition: SubTitlePosition | undefined = SubTitlePosition.Bottom;
  public titleValidators: IValidatorDefinition[] = [];
  public editModeEnabled: boolean = false;
  public editIconButtonEnabled: boolean = true;

  constructor() {
    this.subTitlePosition = SubTitlePosition.Bottom;
    this.titleValidators = [];
    this.editModeEnabled = false;
    this.editIconButtonEnabled = true;
  }
}

export enum SubTitlePosition {
  Top = 'top',
  Bottom = 'bottom'
}
