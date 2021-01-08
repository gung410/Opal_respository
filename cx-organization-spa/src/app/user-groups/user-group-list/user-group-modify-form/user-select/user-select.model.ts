export class UserPickerEventModel {
  key: string = '';
  pageIndex: number = 1;
  constructor(data?: Partial<UserPickerEventModel>) {
    if (!data) {
      return;
    }
    this.key = data.key;
    this.pageIndex = data.pageIndex ? data.pageIndex : 1;
  }
}
