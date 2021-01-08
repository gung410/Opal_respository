import { Utils } from '../../shared/utilities/utils';

export class HeaderComparison {
  newName: string;
  oldName: string;
  constructor(data?: HeaderComparison) {
    if (!data) {
      return;
    }

    this.newName = data.newName;
    this.oldName = data.oldName;
  }

  isDifferent(): boolean {
    return Utils.isDifferent(this.newName, this.oldName);
  }
}
