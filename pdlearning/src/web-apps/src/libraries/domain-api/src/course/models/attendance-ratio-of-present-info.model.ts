export interface IAttendanceRatioOfPresentInfo {
  registrationId: string;
  totalSessions: number;
  presentSessions: number;
}

export class AttendanceRatioOfPresentInfo implements IAttendanceRatioOfPresentInfo {
  public registrationId: string;
  public totalSessions: number;
  public presentSessions: number;

  constructor(data?: IAttendanceRatioOfPresentInfo) {
    if (data) {
      this.registrationId = data.registrationId;
      this.totalSessions = data.totalSessions;
      this.presentSessions = data.presentSessions;
    }
  }

  public displayAsText(): string {
    return `${this.presentSessions}/${this.totalSessions}`;
  }
}
