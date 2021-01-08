import { IUserModel, UserModel } from '../models/user.model';

import { IPagedResultDto } from '../../share/dtos/paged-result.dto';

export class SearchUsersForLearningPathResultDto implements IPagedResultDto<IUserModel> {
  public totalCount: number = 0;
  public items: UserModel[] = [];
  constructor(data?: IPagedResultDto<IUserModel>) {
    if (!data) {
      return;
    }
    this.totalCount = data.totalCount;
    this.items = data.items ? data.items.map(item => new UserModel(item)) : [];
  }
}
