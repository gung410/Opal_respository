export class PeoplePickerEventModel {
  key: string;
  pageIndex: number;
  constructor(data?: Partial<PeoplePickerEventModel>) {
    if (!data) {
      this.key = '';
      this.pageIndex = 1;

      return;
    }
    this.key = data.key;
    this.pageIndex = data.pageIndex ? data.pageIndex : 1;
  }
}
