export interface IMyLearningPackageModel {
  id: string;
  userId: string;
  myLectureId?: string | undefined;
  myDigitalContentId?: string | undefined;
  type: MyLearningPackageType;
  state: string;
  createdDate: Date;
  createdBy: string;
  changedDate?: Date | undefined;
  changedBy: string;
}

export class MyLearningPackageModel implements IMyLearningPackageModel {
  public id: string;
  public userId: string;
  public myLectureId?: string;
  public myDigitalContentId?: string;
  public type: MyLearningPackageType;
  public state: string;
  public createdDate: Date;
  public createdBy: string;
  public changedDate?: Date;
  public changedBy: string;

  constructor(data?: IMyLearningPackageModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.myLectureId = data.myLectureId;
    this.myDigitalContentId = data.myDigitalContentId;
    this.type = data.type;
    this.state = data.state;
    this.createdDate = new Date(data.createdDate);
    this.createdBy = data.createdBy;
    this.changedDate = data.changedDate !== undefined ? new Date(data.changedDate) : undefined;
    this.changedBy = data.changedBy;
  }
}

export enum MyLearningPackageType {
  SCORM = 'SCORM'
}
