import { uniqueId } from 'lodash';

export class FilterModel {
  public appliedFilter: AppliedFilterModel[];
  constructor(data?: Partial<FilterModel>) {
    if (!data) {
      this.appliedFilter = [];
      return;
    }
    this.appliedFilter = data.appliedFilter;
  }
}
export class AppliedFilterModel {
  public filterOptions: any;
  public data: ObjectData;
  public id = uniqueId();
  constructor(data?: Partial<AppliedFilterModel>) {
    if (!data) {
      return;
    }
    this.filterOptions = data.filterOptions;
    this.data = data.data;
  }
}

export class ObjectData {
  public id = uniqueId();
  public value: any;
  public text: string;
  constructor(data?: Partial<ObjectData>) {
    if (!data) {
      return;
    }
    this.value = data.value;
    this.text = data.text;
  }
}
