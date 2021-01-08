export interface IMyLectureModel {
  id: string;
  myCourseId: string;
  lectureId: string;
  userId: string;
  status: string;
  reviewStatus: string | undefined;
  startDate: Date | undefined;
  endDate: Date | undefined;
  lastLogin: Date | undefined;
  createdDate: Date;
  createdBy: string;
  changedDate: Date | undefined;
  changedBy: string | undefined;
}

export class MyLectureModel implements IMyLectureModel {
  public id: string;
  public myCourseId: string;
  public lectureId: string;
  public userId: string;
  public status: string;
  public reviewStatus: string;
  public startDate: Date;
  public endDate: Date;
  public lastLogin: Date;
  public createdDate: Date;
  public createdBy: string;
  public changedDate: Date;
  public changedBy: string;

  constructor(data?: IMyLectureModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.myCourseId = data.myCourseId;
    this.lectureId = data.lectureId;
    this.userId = data.userId;
    this.status = data.status;
    this.reviewStatus = data.userId;
    this.startDate = data.startDate;
    this.endDate = data.endDate;
    this.lastLogin = data.lastLogin;
    this.createdDate = new Date(data.createdDate);
    this.createdBy = data.createdBy;
    this.changedDate = data.changedDate !== undefined ? new Date(data.changedDate) : undefined;
    this.changedBy = data.changedBy;
  }
}

export enum MyLectureStatus {
  NotStarted = 'NotStarted',
  Completed = 'Completed'
}
