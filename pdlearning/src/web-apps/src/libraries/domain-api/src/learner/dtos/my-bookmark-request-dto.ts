import { BookmarkType } from '../models/bookmark-info.model';
import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';

export interface IGetMyBookmarkRequest extends IPagedResultRequestDto {
  itemType: BookmarkType;
  itemIds?: string[];
}
