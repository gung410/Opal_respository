export interface IBookmarkInfoModel {
  id: string;
  userId: string;
  itemType: BookmarkType;
  itemId: string;
  itemName?: string | undefined;
  comment?: string | undefined;
  createdDate: Date;
  createdBy: string;
  changedDate?: Date | undefined;
  changedBy?: string | undefined;
}

export class BookmarkInfoModel implements IBookmarkInfoModel {
  public id: string;
  public userId: string;
  public itemType: BookmarkType;
  public itemId: string;
  public itemName: string;
  public comment: string;
  public createdDate: Date;
  public createdBy: string;
  public changedDate?: Date;
  public changedBy?: string;

  constructor(data?: IBookmarkInfoModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.itemType = data.itemType;
    this.itemId = data.itemId;
    this.itemName = data.itemName;
    this.comment = data.comment;
    this.createdDate = new Date(data.createdDate);
    this.createdBy = data.createdBy;
    this.changedDate = data.changedDate !== undefined ? new Date(data.changedDate) : undefined;
    this.changedBy = data.changedBy;
  }
}

export enum BookmarkType {
  Course = 'Course',
  Lecture = 'Lecture',
  Section = 'Section',
  Question = 'Question',
  DigitalContent = 'DigitalContent',
  LearningPath = 'LearningPath',
  Microlearning = 'Microlearning',
  Community = 'Community',
  LearningPathLMM = 'LearningPathLMM'
}
