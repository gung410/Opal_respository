import { IComment } from '../models/comment';

export interface ISearchCommentResult {
  totalCount: number;
  items: IComment[];
}

export class SearchCommentResult implements ISearchCommentResult {
  public totalCount: number;
  public items: IComment[];

  constructor(data: ISearchCommentResult) {
    if (data == null) {
      return;
    }

    this.totalCount = data.totalCount;
    this.items = data.items;
  }
}
