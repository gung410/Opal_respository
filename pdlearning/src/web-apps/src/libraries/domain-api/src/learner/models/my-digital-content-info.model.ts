import { DigitalContentType } from '../../content/models/digital-content-type.enum';

export interface IMyDigitalContentInfo {
  id: string;
  digitalContentId: string;
  digitalContentType: DigitalContentType;
  userId: string;
  version?: string | undefined;
  status: MyDigitalContentStatus;
  reviewStatus?: string | undefined;
  progressMeasure?: number | undefined;
  lastLogin?: Date | undefined;
  disenrollUtc?: Date | undefined;
  readDate?: Date | undefined;
  reminderSentDate?: Date | undefined;
  startDate?: Date | undefined;
  endDate?: Date | undefined;
  completedDate?: Date | undefined;
  createdDate: Date;
  createdBy: string;
  changedDate?: Date | undefined;
  changedBy?: string | undefined;
}

export class MyDigitalContentInfo implements IMyDigitalContentInfo {
  public id: string;
  public digitalContentId: string;
  public digitalContentType: DigitalContentType;
  public userId: string;
  public version: string;
  public status: MyDigitalContentStatus;
  public reviewStatus: string;
  public progressMeasure: number;
  public lastLogin: Date;
  public disenrollUtc: Date;
  public readDate: Date;
  public reminderSentDate: Date;
  public startDate: Date;
  public endDate: Date;
  public completedDate: Date;
  public createdDate: Date;
  public createdBy: string;
  public changedDate: Date;
  public changedBy: string;

  constructor(data?: IMyDigitalContentInfo) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.digitalContentId = data.digitalContentId;
    this.digitalContentType = data.digitalContentType;
    this.userId = data.userId;
    this.version = data.version;
    this.status = data.status;
    this.reviewStatus = data.reviewStatus;
    this.progressMeasure = data.progressMeasure;
    this.lastLogin = data.lastLogin;
    this.disenrollUtc = data.disenrollUtc;
    this.readDate = data.readDate;
    this.reminderSentDate = data.reminderSentDate;
    this.startDate = data.startDate;
    this.endDate = data.endDate;
    this.completedDate = data.completedDate !== undefined ? new Date(data.completedDate) : undefined;
    this.createdDate = new Date(data.createdDate);
    this.createdBy = data.createdBy;
    this.changedDate = data.changedDate !== undefined ? new Date(data.changedDate) : undefined;
    this.changedBy = data.changedBy;
  }
}

export enum MyDigitalContentStatus {
  InProgress = 'InProgress',
  Completed = 'Completed',
  NotStarted = 'NotStarted'
}
