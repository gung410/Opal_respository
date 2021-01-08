import { IVersionTracking } from '../models/version-tracking';

export interface IVersionTrackingSearchResult {
  totalCount: number;
  items: IVersionTracking[];
}

export class VersionTrackingSearchResult implements IVersionTrackingSearchResult {
  public totalCount: number;
  public items: IVersionTracking[];

  constructor(data: IVersionTrackingSearchResult) {
    if (data == null) {
      return;
    }

    this.totalCount = data.totalCount;
    this.items = data.items;
  }
}
