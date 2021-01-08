import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { AppConstant } from 'app/shared/app.constant';
import { FileInfo, IFileInfo } from '../models/file-info.model';

export interface IFileInfoListViewModel extends IFileInfo {
  createdBy: string;
  createdDateLabel: string;
}

export class FileInfoListViewModel extends FileInfo {
  static createFromModel(
    fileInfo: IFileInfo,
    createdBy: string
  ): FileInfoListViewModel {
    return new FileInfoListViewModel({
      ...fileInfo,
      createdBy,
      createdDateLabel: DateTimeUtil.toDateString(
        fileInfo.createdDate,
        AppConstant.dateTimeFormat
      )
    });
  }

  createdBy: string;
  createdDateLabel: string;

  constructor(data?: IFileInfoListViewModel) {
    super(data);
    this.createdBy = data.createdBy;
    this.createdDateLabel = data.createdDateLabel;
  }
}
