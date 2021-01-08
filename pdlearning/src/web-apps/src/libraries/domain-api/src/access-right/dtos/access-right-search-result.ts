import { IAccessRight } from '../models/access-right';

export interface IAccessRightSearchResult {
  totalCount: number;
  items: IAccessRight[];
}

export class AccessRightSearchResult implements IAccessRightSearchResult {
  public totalCount: number;
  public items: IAccessRight[];

  constructor(data: IAccessRightSearchResult) {
    if (!data) {
      return;
    }

    this.totalCount = data.totalCount;
    this.items = data.items;
  }
}
