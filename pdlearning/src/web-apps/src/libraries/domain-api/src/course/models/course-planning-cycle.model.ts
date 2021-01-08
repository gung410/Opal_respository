import { SystemRoleEnum, UserInfoModel } from '../../share/models/user-info.model';

import { DateUtils } from '@opal20/infrastructure';
export interface ICoursePlanningCycle {
  id: string;
  yearCycle: number;
  title: string;
  startDate: Date;
  endDate: Date;
  description: string;
  createdBy: string;
  changedBy: string;
  numberOfCourses?: number;
  isConfirmedBlockoutDate: boolean;
}

export class CoursePlanningCycle implements ICoursePlanningCycle {
  public static optionalProps: (keyof ICoursePlanningCycle)[] = ['numberOfCourses'];
  public id: string;
  public yearCycle: number;
  public title: string;
  public startDate: Date;
  public endDate: Date;
  public description: string;
  public createdBy: string;
  public changedBy: string;
  public numberOfCourses?: number;
  public isConfirmedBlockoutDate: boolean;

  public static canVerifyCourse(user: UserInfoModel): boolean {
    return user && (user.hasRole(SystemRoleEnum.SystemAdministrator) || user.hasRole(SystemRoleEnum.CoursePlanningCoordinator));
  }

  public get status(): CoursePlanningCycleStatus {
    if (this.startDate == null || DateUtils.removeTime(new Date()) < DateUtils.removeTime(this.startDate)) {
      return CoursePlanningCycleStatus.NotStarted;
    }
    if (
      DateUtils.removeTime(new Date()) >= DateUtils.removeTime(this.startDate) &&
      DateUtils.removeTime(new Date()) <= DateUtils.removeTime(this.endDate)
    ) {
      return CoursePlanningCycleStatus.InProgress;
    }
    return CoursePlanningCycleStatus.Completed;
  }

  constructor(data?: ICoursePlanningCycle) {
    if (data) {
      this.id = data.id;
      this.yearCycle = data.yearCycle;
      this.title = data.title;
      this.startDate = data.startDate ? new Date(data.startDate) : undefined;
      this.endDate = data.endDate ? new Date(data.endDate) : undefined;
      this.description = data.description;
      this.createdBy = data.createdBy;
      this.changedBy = data.changedBy;
      this.numberOfCourses = data.numberOfCourses;
      this.isConfirmedBlockoutDate = data.isConfirmedBlockoutDate;
    }
  }
}

export enum CoursePlanningCycleStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Completed = 'Completed'
}
