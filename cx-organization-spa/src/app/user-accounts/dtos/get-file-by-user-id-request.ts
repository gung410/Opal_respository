import { FileTarget } from '../models/file-info.model';

export interface IGetFileByUserIdRequest {
  searchText?: string;
  userGuid?: string;
  pageIndex?: number;
  pageSize?: number;
  fileTarget: FileTarget;
  orderBy?: string;
  orderType?: string;
}

export class GetFileByUserIdRequest implements IGetFileByUserIdRequest {
  searchText?: string;
  userGuid?: string;
  pageIndex?: number;
  pageSize?: number;
  fileTarget: FileTarget;
  orderBy?: string;
  orderType?: string;

  constructor(data?: Partial<IGetFileByUserIdRequest>) {
    if (!data) {
      return;
    }

    this.searchText = data.searchText;
    this.userGuid = data.userGuid;
    this.pageIndex = data.pageIndex;
    this.pageSize = data.pageSize;
    this.fileTarget = data.fileTarget;
    this.orderBy = data.orderBy;
    this.orderType = data.orderType;
  }
}
