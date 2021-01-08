import { IBaseUserInfo, IUserInfoModel } from '../../share/models/user-info.model';

export interface IUserInfoListResult {
  items: IUserInfoModel[];
}

export interface IBaseUserInfoResult {
  hasMoreData: boolean;
  totalItems: number;
  items: IBaseUserInfo[];
}

export class BaseUserInfoWithCheckMoreData {
  public hasMoreData: boolean;
  public items: IBaseUserInfo[];
  constructor(data?: IBaseUserInfoResult) {
    this.hasMoreData = data.hasMoreData;
    this.items = data.items;
  }
}
