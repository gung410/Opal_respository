export interface ISharedTeamModel {
  accessShareId: string;
  ownerFullName: string;
}

export class SharedTeamModel implements ISharedTeamModel {
  public accessShareId: string;
  public ownerFullName: string;

  constructor(data: ISharedTeamModel) {
    this.accessShareId = data.accessShareId;
    this.ownerFullName = data.ownerFullName;
  }
}
