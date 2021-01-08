import * as _ from 'lodash';

export class FilterModel {
  appliedFilter: AppliedFilterModel[];
  constructor(data?: Partial<FilterModel>) {
    if (!data) {
      this.appliedFilter = [];

      return;
    }
    this.appliedFilter = data.appliedFilter;
  }
}
// tslint:disable:max-classes-per-file
export class AppliedFilterModel {
  filterOptions: any;
  data: ObjectData;
  id: string = _.uniqueId();
  constructor(data?: Partial<AppliedFilterModel>) {
    if (!data) {
      return;
    }
    this.filterOptions = data.filterOptions;
    this.data = data.data;
  }
}

export class ObjectData {
  id: string = _.uniqueId();
  value: any;
  text: string;
  constructor(data?: Partial<ObjectData>) {
    if (!data) {
      return;
    }
    this.value = data.value;
    this.text = data.text;
  }
}
