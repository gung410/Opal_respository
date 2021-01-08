export class ColumnItemModel<T> {
  item: T;
  colId: number | string;

  constructor(data?: ColumnItemModel<T>) {
    if (!data) {
      return;
    }

    this.item = data.item;
    this.colId = data.colId;
  }
}
