export interface IPagedCxCompetenceModel<T> {
  totalItems: number;
  pageIndex: number;
  pageSize: number;
  hasMoreData: boolean;
  items: T[];
}
