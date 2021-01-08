import { FileType } from './file-type.enum';

export enum PersonalFileQueryMode {
  All = 'All'
}

export const PERSONAL_FILE_QUERY_MODE_LABEL = new Map<PersonalFileQueryMode, string>([[PersonalFileQueryMode.All, 'Files']]);

export interface IPersonalFileModel {
  id: string;
  fileName: string;
  fileType: FileType;
  fileSize: number;
  fileLocation: string;
  fileExtension: string;
  createdDate?: Date;
}
export class PersonalFileModel implements IPersonalFileModel {
  public id: string;
  public fileName: string;
  public fileType: FileType;
  public fileSize: number;
  public fileLocation: string;
  public fileExtension: string;
  public createdDate?: Date;

  constructor(data?: IPersonalFileModel) {
    if (data != null) {
      this.id = data.id;
      this.fileName = data.fileName;
      this.fileType = data.fileType;
      this.fileSize = data.fileSize;
      this.fileLocation = data.fileLocation;
      this.fileExtension = data.fileExtension;
      this.createdDate = data.createdDate;
    }
  }
}
