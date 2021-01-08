export interface SessionDTO {
  id: string;
  classRunId: string;
  sessionTitle: string;
  sessionCode?: string;
  venue: string;
  learningMethod: boolean; // Online if true, offline if false.
  startDateTime?: Date;
  endDateTime?: Date;
  rescheduleStartDateTime?: Date;
  rescheduleEndDateTime?: Date;
  createdBy: string;
  changedBy: string;
  createdDate: Date;
}

export class SessionModel {
  public id: string;
  public classRunId: string;
  public sessionCode?: string;
  public sessionTitle: string = '';
  public venue: string = '';
  public learningMethod: boolean = false;
  public sessionDate: Date = new Date();
  public startTime?: Date = new Date();
  public endTime?: Date = new Date();
  public startDateTime?: Date = new Date();
  public endDateTime?: Date = new Date();
  public rescheduleStartDateTime?: Date;
  public rescheduleEndDateTime?: Date;
  public createdBy: string;
  public changedBy: string;
  public createdDate: Date;
  constructor(sessionDTO?: SessionDTO) {
    if (!sessionDTO) {
      return;
    }
    this.id = sessionDTO.id;
    this.classRunId = sessionDTO.classRunId;
    this.sessionTitle = sessionDTO.sessionTitle;
    this.sessionCode = sessionDTO.sessionCode;
    this.venue = sessionDTO.venue;
    this.learningMethod = sessionDTO.learningMethod;
    this.startDateTime = sessionDTO.startDateTime
      ? new Date(sessionDTO.startDateTime)
      : new Date();
    this.endDateTime = sessionDTO.endDateTime
      ? new Date(sessionDTO.endDateTime)
      : new Date();
    this.sessionDate = sessionDTO.startDateTime
      ? new Date(sessionDTO.startDateTime)
      : new Date();
    this.startTime = sessionDTO.startDateTime
      ? new Date(sessionDTO.startDateTime)
      : new Date();
    this.endTime = sessionDTO.endDateTime
      ? new Date(sessionDTO.endDateTime)
      : new Date();
    this.rescheduleStartDateTime = sessionDTO.rescheduleStartDateTime
      ? new Date(sessionDTO.rescheduleStartDateTime)
      : new Date();
    this.rescheduleEndDateTime = sessionDTO.rescheduleEndDateTime
      ? new Date(sessionDTO.rescheduleEndDateTime)
      : new Date();
    this.createdBy = sessionDTO.createdBy;
    this.changedBy = sessionDTO.changedBy;
    this.createdDate = new Date(sessionDTO.createdDate);
  }
}

export enum CourseDetailTabEnum {
  About = 'About',
  Content = 'Content',
  ClassRun = 'Class Run',
  Review = 'Review',
  Comment = 'Comment',
}
