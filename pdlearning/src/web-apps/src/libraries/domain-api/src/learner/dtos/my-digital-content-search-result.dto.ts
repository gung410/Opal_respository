import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { MyDigitalContent } from '../models/my-digital-content.model';

export class MyDigitalContentSearchResult implements IPagedResultDto<MyDigitalContent> {
  public items: MyDigitalContent[] = [];
  public totalCount: number = 0;

  constructor(data?: IPagedResultDto<MyDigitalContent>) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new MyDigitalContent(item));
    this.totalCount = data.totalCount;
  }
}
