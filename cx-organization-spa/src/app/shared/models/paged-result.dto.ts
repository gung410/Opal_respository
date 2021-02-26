export class PagedResultDto<T> {
  // hasMoreData?: boolean;
  items?: T[];
  totalCount?: number;
  constructor(data?: Partial<PagedResultDto<T>>) {
    if (!data) {
      return;
    }

    this.items = data.items;
    this.totalCount = data.totalCount;
    // this.hasMoreData = data.hasMoreData;
  }
}
