export interface IAnnouncement {
  id?: string;
  title: string;
  message: string;
  sentDate?: Date;
  scheduleDate?: Date;
  participants: string[];
  status: AnnouncementStatus;
  courseId: string;
  classrunId: string;
  createdBy: string;
  createdDate: Date;
  changedDate?: Date;
}

export class Announcement implements IAnnouncement {
  public id?: string;
  public title: string = '';
  public message: string = '';
  public sentDate?: Date = new Date();
  public scheduleDate?: Date = new Date();
  public participants: string[] = [];
  public status: AnnouncementStatus = AnnouncementStatus.Scheduled;
  public courseId: string = '';
  public classrunId: string = '';
  public createdBy: string = '';
  public createdDate: Date = new Date();
  public changedDate?: Date;

  constructor(data?: IAnnouncement) {
    if (data != null) {
      this.id = data.id;
      this.title = data.title;
      this.message = data.message;
      this.scheduleDate = data.scheduleDate ? new Date(data.scheduleDate) : undefined;
      this.sentDate = data.sentDate ? new Date(data.sentDate) : undefined;
      this.participants = data.participants;
      this.status = data.status;
      this.courseId = data.courseId;
      this.classrunId = data.classrunId;
      this.createdBy = data.createdBy;
      this.createdDate = data.createdDate ? new Date(data.createdDate) : undefined;
      this.changedDate = data.changedDate ? new Date(data.changedDate) : undefined;
    }
  }
}

export enum AnnouncementStatus {
  Scheduled = 'Scheduled',
  Sent = 'Sent',
  Cancelled = 'Cancelled'
}
