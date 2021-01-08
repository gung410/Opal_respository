import { SystemRoleEnum, UserInfoModel } from '../../share/models/user-info.model';

export interface IAnnouncementTemplate {
  id: string;
  title: string;
  message: string;
  createdBy: string;
  createdDate: Date;
  changedDate?: Date;
}

export class AnnouncementTemplate implements IAnnouncementTemplate {
  public id: string = '';
  public title: string = '';
  public message: string = '';
  public createdBy: string = '';
  public createdDate: Date = new Date();
  public changedDate?: Date;

  constructor(data?: IAnnouncementTemplate) {
    if (data != null) {
      this.id = data.id;
      this.title = data.title;
      this.message = data.message;
      this.createdBy = data.createdBy;
      this.createdDate = data.createdDate ? new Date(data.createdDate) : undefined;
      this.changedDate = data.changedDate ? new Date(data.changedDate) : undefined;
    }
  }

  public canBeDeleted(user: UserInfoModel): boolean {
    return this.createdBy === user.id || user.hasRole(SystemRoleEnum.SystemAdministrator);
  }
}
