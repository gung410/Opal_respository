export interface IFilter {
  containFilters: IContainFilter[];
  fromToFilters: IFromToFilter[];
}

export interface IContainFilter {
  field: string;
  values?: string[];
  notContain: boolean;
}

export interface IFromToFilter {
  field: string;
  fromValue: string;
  toValue: string;
  equalFrom: boolean;
  equalTo: boolean;
}
