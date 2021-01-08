import { TeamCalendarViewType } from '../enums/team-calendar-view-type-enum';

export interface ITeamCalendarContextModel {
  selectedYear: number;
  selectedViewType: TeamCalendarViewType;
  selectedUserIds: string[];
}

export class TeamCalendarContextModel implements ITeamCalendarContextModel {
  public selectedYear: number;
  public selectedViewType: TeamCalendarViewType;
  public selectedUserIds: string[];

  constructor(data: ITeamCalendarContextModel) {
    this.selectedYear = data.selectedYear;
    this.selectedViewType = data.selectedViewType;
    this.selectedUserIds = data.selectedUserIds;
  }
}
