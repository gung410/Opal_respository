export interface IPagedInfo {
  skipCount: number;
  maxResultCount: number;
}

export interface IVersionTrackingSearchRequest {
  originalObjectId: string;
  pagedInfo: IPagedInfo;
}
