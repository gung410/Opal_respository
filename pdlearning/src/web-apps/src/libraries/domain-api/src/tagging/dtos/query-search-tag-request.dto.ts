export interface IQuerySearchTagRequest {
  searchText: string;
  pagedInfo: IPagedInfo;
}

export interface IPagedInfo {
  skipCount: number;
  maxResultCount: number;
}
