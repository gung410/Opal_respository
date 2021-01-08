import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
export class ContextMenuEmit<T> {
  public event: ContextMenuSelectEvent;
  public dataItem: T;
  public rowIndex: number;

  constructor(event: ContextMenuSelectEvent, dataItem: T, rowIndex: number) {
    this.event = event;
    this.dataItem = dataItem;
    this.rowIndex = rowIndex;
  }
}
