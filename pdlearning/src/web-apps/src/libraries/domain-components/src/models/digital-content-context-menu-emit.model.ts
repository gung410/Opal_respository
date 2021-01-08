import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { DigitalContent } from '@opal20/domain-api';
export class DigitalContentContextMenuEmit {
  public event: ContextMenuSelectEvent;
  public dataItem: DigitalContent;
  public rowIndex: number;

  constructor(event: ContextMenuSelectEvent, dataItem: DigitalContent, rowIndex: number) {
    this.event = event;
    this.dataItem = dataItem;
    this.rowIndex = rowIndex;
  }
}
