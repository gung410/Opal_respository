export interface IChapterAttachment {
  id: string;
  objectId: string;
  fileLocation: string;
  fileName: string;
  createdDate?: Date;
  changedDate?: Date;
}

export class ChapterAttachment implements IChapterAttachment {
  public id: string;
  public objectId: string;
  public fileLocation: string;
  public fileName: string;
  public createdDate?: Date;
  public changedDate?: Date;
  constructor(data?: IChapterAttachment) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.objectId = data.objectId;
    this.fileLocation = data.fileLocation;
    this.fileName = data.fileName;
    this.createdDate = data.createdDate ? new Date(data.createdDate) : null;
    this.changedDate = data.changedDate ? new Date(data.changedDate) : null;
  }
}
