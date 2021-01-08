import { UserBookMarkedModel } from '../models/user-bookmarked.model';

export interface IGetCountUserBookmarkedResult {
  item: UserBookMarkedModel[];
}

export class GetCountUserBookmarkedResult implements IGetCountUserBookmarkedResult {
  public item: UserBookMarkedModel[] = [];

  constructor(item?: UserBookMarkedModel[]) {
    if (item == null) {
      return;
    }
    this.item = item.map(x => new UserBookMarkedModel(x));
  }
}
