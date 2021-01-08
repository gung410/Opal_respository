export interface IPersonalSpaceModel {
  userId: string;
  totalSpace: number;
  totalUsed: number;
  isStorageUnlimited: boolean;
}

export class PersonalSpaceModel implements IPersonalSpaceModel {
  public userId: string;
  public totalSpace: number;
  public totalUsed: number;
  public isStorageUnlimited: boolean;

  constructor(data?: IPersonalSpaceModel) {
    if (data != null) {
      this.userId = data.userId;
      this.totalSpace = data.totalSpace;
      this.totalUsed = data.totalUsed ? data.totalUsed : 0;
      this.isStorageUnlimited = data.isStorageUnlimited;
    }
  }
}
