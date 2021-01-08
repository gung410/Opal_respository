export interface IPagedResultDto<T> {
  totalCount: number;
  items: T[];
}

export interface ICslPagedResultDto<T> {
  total: number;
  results: T[];
}
