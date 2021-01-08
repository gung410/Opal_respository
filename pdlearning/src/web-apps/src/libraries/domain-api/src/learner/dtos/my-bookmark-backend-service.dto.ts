import { BookmarkType } from '../models/bookmark-info.model';

export interface IUserBookmarkRequest {
  itemId: string;
  itemType: BookmarkType;
}
