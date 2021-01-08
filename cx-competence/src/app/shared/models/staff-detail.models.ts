export class StaffDetail {
  Id: number;
  FullName: string;
  AvatarUrl: string;
  RoleName: string;
  LastLoginStatusDate: Date;
  Progress: Progress;
  constructor(data?: Partial<StaffDetail>) {
    if (!data) {
      return;
    }
    this.Id = data.Id;
    this.FullName = data.FullName;
    this.AvatarUrl = data.AvatarUrl;
    this.RoleName = data.RoleName;
    this.LastLoginStatusDate = data.LastLoginStatusDate;
    this.Progress = data.Progress;
  }
}

export class Progress {
  Value: number;
  Text: string;
}
