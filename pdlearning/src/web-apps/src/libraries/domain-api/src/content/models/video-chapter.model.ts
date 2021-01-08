import { ChapterAttachment, IChapterAttachment } from './chapter-attachment.model';

export interface IVideoChapter {
  id: string;
  /**
   * Digital content Id
   */
  objectId: string;
  originalObjectId: string;
  title: string;
  description?: string;
  timeStart: number;
  timeEnd: number;
  attachments: IChapterAttachment[];
  createdDate?: Date;
  changedDate?: Date;
}

export class VideoChapter implements IVideoChapter {
  public id: string;
  public objectId: string;
  public originalObjectId: string;
  public title: string;
  public description?: string;
  public timeStart: number;
  public timeEnd: number;
  public attachments: ChapterAttachment[];
  public createdDate?: Date;
  public changedDate?: Date;
  constructor(data?: IVideoChapter) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.objectId = data.objectId;
    this.originalObjectId = data.originalObjectId;
    this.title = data.title;
    this.description = data.description;
    this.timeStart = data.timeStart;
    this.timeEnd = data.timeEnd;
    this.attachments = data.attachments ? data.attachments.map(p => new ChapterAttachment(p)) : [];
    this.createdDate = new Date(data.createdDate);
    this.changedDate = data.changedDate ? new Date(data.changedDate) : null;
  }
}

export enum VideoChapterSourceType {
  CSL = 'CSL',
  DigitalContent = 'DigitalContent' // For CCPM - LMM
}
