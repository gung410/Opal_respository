export enum FileTarget {
  None = 'None',
  MassUserCreation = 'MassUserCreation'
}

export interface IFileInfo {
  fileInfoId: number;
  fileName: string;
  filePath: string;
  originalFileName: string;
  type: string;
  numberOfRecord: number;
  fileTarget: FileTarget;
  createdDate: Date;
  userGuid: string;
}

export class FileInfo implements IFileInfo {
  fileInfoId: number;
  fileName: string;
  filePath: string;
  originalFileName: string;
  type: string;
  numberOfRecord: number;
  fileTarget: FileTarget = FileTarget.None;
  createdDate: Date;
  userGuid: string;

  constructor(data?: Partial<IFileInfo>) {
    if (!data) {
      return;
    }

    this.fileInfoId = data.fileInfoId;
    this.fileName = data.fileName;
    this.filePath = data.filePath;
    this.originalFileName = data.originalFileName;
    this.type = data.type;
    this.numberOfRecord = data.numberOfRecord;
    this.fileTarget = data.fileTarget;
    this.createdDate = data.createdDate;
    this.userGuid = data.userGuid;
  }
}
