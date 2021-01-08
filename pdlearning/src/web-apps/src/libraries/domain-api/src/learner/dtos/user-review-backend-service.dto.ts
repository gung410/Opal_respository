import { IUserReviewModel, UserReviewModel } from '../models/user-review.model';

import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';
import { UserReviewItemType } from '../../content/models/user-review-item-type.enum';

export interface IPageUserReviewRequest extends IPagedResultRequestDto {
  itemId?: string;
  classrunId?: string;
  itemTypeFilter?: UserReviewItemType[];
  orderBy?: string;
  classRunId?: string;
}

export class PagedUserReviewModelResult implements IPagedResultDto<IUserReviewModel> {
  public rating: number = 0;
  public totalCount: number = 0;
  public items: UserReviewModel[] = [];

  constructor(rating: number, totalCount: number, items: IUserReviewModel[]) {
    if (items === undefined) {
      return;
    }
    this.totalCount = totalCount;
    this.items = items !== undefined ? items.map(p => new UserReviewModel(p)) : [];
    this.rating = rating;
  }
}

export interface IUserReviewRequest {
  itemId: string;
  itemType: UserReviewItemType;
}

export interface ICreateUserReviewRequest {
  itemId: string;
  rating: number;
  itemType: UserReviewItemType;
  commentContent?: string | undefined;
  classRunId?: string;
}

export interface IUpdateUserReviewRequest {
  rating: number;
  itemType: UserReviewItemType;
  commentContent?: string | undefined;
}
