import { DigitalContent, IDigitalContent } from '../models/digital-content';

export interface IDigitalContentSearchResult {
  totalCount: number;
  items: IDigitalContent[];
}

export class DigitalContentSearchResult implements IDigitalContentSearchResult {
  public totalCount: number;
  public items: DigitalContent[];

  constructor(data: IDigitalContentSearchResult) {
    if (data == null) {
      return;
    }

    this.totalCount = data.totalCount;
    this.items = data.items ? data.items.map(p => new DigitalContent(p)) : [];
  }
}
