export interface ITeamCalendarRangeDate {
  start: Date;
  end: Date;
}

export class TeamCalendarRangeDate implements ITeamCalendarRangeDate {
  public start: Date;
  public end: Date;
  constructor(data: ITeamCalendarRangeDate) {
    this.start = data.start;
    this.end = data.end;
  }
}
