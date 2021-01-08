export class ShowHideColumnModel {
  selected: any;
  column: any;
  constructor(data?: Partial<ShowHideColumnModel>) {
    if (!data) {
      return;
    }
    this.selected = data.selected ? data.selected : undefined;
    this.column = data.column ? data.column : undefined;
  }
}
