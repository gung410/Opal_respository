export class CheckboxesFilterModel {
  public checkedKeys: Array<string>;
  public filterSet: Array<{ text: string; value: string }>;

  constructor(checkKeys: Array<string>, filterSet: Array<{ text: string; value: string }>) {
    this.checkedKeys = checkKeys;
    this.filterSet = filterSet;
  }
}
