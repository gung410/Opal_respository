import { SystemRoleEnum, UserInfoModel } from '../../share/models/user-info.model';

export enum BlockoutDateStatus {
  Draft = 'Draft',
  Active = 'Active'
}
export interface IBlockoutDateModel {
  id?: string;
  title: string;
  description: string;
  createdBy: string;
  startDay: number;
  startMonth: number;
  endDay: number;
  endMonth: number;
  validYear: number;
  status?: BlockoutDateStatus;
  planningCycleId?: string;
  serviceSchemes: string[];
  isConfirmed: boolean;
}

export class BlockoutDateModel implements IBlockoutDateModel {
  public id?: string;
  public title: string = '';
  public description: string = '';
  public createdBy: string = '';
  public startDay: number = 0;
  public startMonth: number = 0;
  public endDay: number = 0;
  public endMonth: number = 0;
  public validYear: number = 0;
  public status?: BlockoutDateStatus = BlockoutDateStatus.Draft;
  public planningCycleId?: string = '';
  public serviceSchemes: string[] = [];
  public isConfirmed: boolean = false;

  public static haveCudPermission(user: UserInfoModel): boolean {
    return user.hasRole(SystemRoleEnum.SystemAdministrator) || user.hasRole(SystemRoleEnum.CoursePlanningCoordinator);
  }

  constructor(data?: IBlockoutDateModel) {
    if (data) {
      this.id = data.id;
      this.title = data.title;
      this.description = data.description;
      this.createdBy = data.createdBy;
      this.startDay = data.startDay;
      this.startMonth = data.startMonth;
      this.endDay = data.endDay;
      this.endMonth = data.endMonth;
      this.validYear = data.validYear;
      this.status = data.status;
      this.planningCycleId = data.planningCycleId;
      this.serviceSchemes = data.serviceSchemes;
      this.isConfirmed = data.isConfirmed;
    }
  }
}
