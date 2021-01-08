export interface IBaseTeamEventViewModel {
  summary: boolean;
  expanded: boolean;
}

export interface ITeamMemberEventModel {
  id: string;
  title: string;
  start: Date;
  end: Date;
  parentId?: string;
  subTitle?: string;
}

export interface ITeamMemberEventViewModel extends ITeamMemberEventModel, IBaseTeamEventViewModel {}

export class TeamMemberEventViewModel implements ITeamMemberEventViewModel {
  public id: string;
  public title: string;
  public parentId?: string;
  public start: Date;
  public end: Date;
  public summary: boolean;
  public expanded: boolean;

  constructor(mainData: ITeamMemberEventModel, baseData?: IBaseTeamEventViewModel) {
    this.id = mainData.id;
    this.title = mainData.title;
    this.start = mainData.start;
    this.end = mainData.end;
    this.parentId = mainData.parentId;
    this.summary = baseData != null ? baseData.summary : true;
    this.expanded = baseData != null ? baseData.expanded : false;
  }
}
