export interface IUpcomingSession {
  classRunId: string;
  startDateTime: Date;
}

export class UpcomingSession {
  public classRunId: string;
  public startDateTime: Date;
  constructor(data?: IUpcomingSession) {
    if (data == null) {
      return;
    }
    this.classRunId = data.classRunId;
    this.startDateTime = data.startDateTime ? new Date(data.startDateTime) : undefined;
  }
}
