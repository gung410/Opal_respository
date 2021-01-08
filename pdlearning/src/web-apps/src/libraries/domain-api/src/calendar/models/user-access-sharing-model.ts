export interface IUserAccessSharingModel {
  userId: string;
  fullName: string;
  email: string;
  shared: boolean;
}

export class UserAccessSharingModel implements IUserAccessSharingModel {
  public userId: string;
  public fullName: string;
  public email: string;
  public shared: boolean;

  constructor(data: IUserAccessSharingModel) {
    this.userId = data.userId;
    this.fullName = data.fullName;
    this.email = data.email;
    this.shared = data.shared;
  }
}
