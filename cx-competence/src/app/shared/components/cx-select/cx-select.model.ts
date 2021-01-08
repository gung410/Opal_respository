import { ObjectUtilities } from 'app-utilities/object-utils';

export class CxSelectItemModel<T> {
  id: string;
  parentTagId: string;
  avatar?: string;
  primaryField?: string;
  secondaryField?: string;
  thirdField?: string;
  dataObject?: T;
  disabled?: boolean;
  constructor(item: Partial<CxSelectItemModel<T>>) {
    if (!item) {
      return;
    }

    ObjectUtilities.copyPartialObject(item, this);
  }
}

export class CxSelectConfigModel {
  addItem: boolean = false;
  multiple: boolean = true;
  clearable: boolean = false;
  searchText: string = 'Please enter 2 or more characters';
  searchable: boolean = true;
  placeholder: string = 'Search...';
  hideSelected: boolean = true;
  closeOnSelect: boolean = true;
  minSearchTerm: number = 2;
  virtualScroll: boolean = true;
  showInitValue: boolean = false;
  disableItemText: string;
  autoLoadMore: boolean = false;
  loadMorePageSize: number = 10;
  pageNumberStartAt: number = 1;

  constructor(config?: Partial<CxSelectConfigModel>) {
    if (!config) {
      return;
    }

    ObjectUtilities.copyPartialObject(config, this);
  }
}
