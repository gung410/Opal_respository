export class SortModel {
  currentSortType: string;
  currentFieldSort: string;
  constructor(data?: Partial<SortModel>) {
    if (!data) {
      return;
    }

    this.currentSortType = data.currentSortType ? data.currentSortType : '';
    this.currentFieldSort = data.currentFieldSort ? data.currentFieldSort : '';
  }
}
