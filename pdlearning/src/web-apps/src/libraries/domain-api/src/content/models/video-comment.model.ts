export interface IVideoComment {
  id: string;
  userId: string;
  objectId: string;
  sourceType: VideoCommentSourceType;
  originalObjectId: string;
  content: string;
  videoId: string;
  videoTime: number;
  createdDate: Date;
  changedDate?: Date;
}

export class VideoComment implements IVideoComment {
  public id: string;
  public userId: string;
  public objectId: string;
  public sourceType: VideoCommentSourceType;
  public originalObjectId: string;
  public content: string;
  public videoId: string;
  public videoTime: number;
  public createdDate: Date;
  public changedDate?: Date;
  constructor(data?: IVideoComment) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.objectId = data.objectId;
    this.sourceType = data.sourceType;
    this.originalObjectId = data.originalObjectId;
    this.content = data.content;
    this.videoId = data.videoId;
    this.videoTime = data.videoTime;
    this.createdDate = new Date(data.createdDate);
    this.changedDate = data.changedDate ? new Date(data.changedDate) : null;
  }
}

export enum VideoCommentSourceType {
  CCPM = 'CCPM',
  LMM = 'LMM',
  CSL = 'CSL'
}
