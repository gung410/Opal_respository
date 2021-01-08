import { ClassRun, ClassRunStatus } from './classrun.model';

import { CAM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/cam-permission-key';
import { Course } from './course.model';
import { DateUtils } from '@opal20/infrastructure';
import { LMM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/lmm-permission-key';
import { UserInfoModel } from '../../share/models/user-info.model';

export interface ISession {
  id: string;
  classRunId: string;
  sessionTitle: string;
  sessionCode?: string;
  venue: string;
  learningMethod: boolean; // Online if true, offline if false.
  startDateTime?: Date;
  endDateTime?: Date;
  rescheduleStartDateTime?: Date;
  rescheduleEndDateTime?: Date;
  createdBy: string;
  changedBy: string;
  createdDate: Date;
  usePreRecordClip: boolean;
  preRecordId?: string;
  preRecordPath?: string;
}
export class Session implements ISession {
  public static optionalProps: (keyof ISession)[] = ['sessionCode'];

  public id: string;
  public classRunId: string;
  public sessionCode?: string;
  public sessionTitle: string = '';
  public venue: string = '';
  public learningMethod: boolean = false;
  public sessionDate: Date = new Date();
  public startTime?: Date = new Date();
  public endTime?: Date = new Date();
  public startDateTime?: Date = new Date();
  public endDateTime?: Date = new Date();
  public rescheduleStartDateTime?: Date;
  public rescheduleEndDateTime?: Date;
  public createdBy: string;
  public changedBy: string;
  public createdDate: Date;
  public usePreRecordClip: boolean = false;
  public preRecordId?: string;
  public preRecordPath?: string;

  private readonly minDaysBeforeStartToUpdateMeeting: number = 2;

  public static hasViewSessionCodePermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.GetSessionCode);
  }

  constructor(data?: ISession) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.classRunId = data.classRunId;
    this.sessionTitle = data.sessionTitle;
    this.sessionCode = data.sessionCode;
    this.venue = data.venue;
    this.learningMethod = data.learningMethod;
    this.startDateTime = data.startDateTime ? new Date(data.startDateTime) : new Date();
    this.endDateTime = data.endDateTime ? new Date(data.endDateTime) : new Date();
    this.sessionDate = data.startDateTime ? new Date(data.startDateTime) : new Date();
    this.startTime = data.startDateTime ? new Date(data.startDateTime) : new Date();
    this.endTime = data.endDateTime ? new Date(data.endDateTime) : new Date();
    this.rescheduleStartDateTime = data.rescheduleStartDateTime ? new Date(data.rescheduleStartDateTime) : new Date();
    this.rescheduleEndDateTime = data.rescheduleEndDateTime ? new Date(data.rescheduleEndDateTime) : new Date();
    this.createdBy = data.createdBy;
    this.changedBy = data.changedBy;
    this.createdDate = new Date(data.createdDate);
    this.usePreRecordClip = data.usePreRecordClip;
    this.preRecordId = data.preRecordId;
    this.preRecordPath = data.preRecordPath;
  }

  public isStarted(): boolean {
    return this.startDateTime && new Date() >= this.startDateTime;
  }

  public canBeModified(classRun: ClassRun, course: Course): boolean {
    return !classRun.started() && classRun.status === ClassRunStatus.Unpublished && !course.isArchived();
  }

  public hasModifiedPermission(course: Course, currentUser: UserInfoModel): boolean {
    return course.hasAdministrationPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.CreateEditSession);
  }

  public isLearningOnline(): boolean {
    return this.learningMethod;
  }

  public canJoinWebinar(maxMinutesCanJoinWebinarEarly: number): boolean {
    return (
      this.isLearningOnline() &&
      (new Date() >= DateUtils.addMinutes(this.startDateTime, -maxMinutesCanJoinWebinarEarly) &&
        new Date() <= DateUtils.addMinutes(this.endDateTime, maxMinutesCanJoinWebinarEarly))
    );
  }

  /**
   * Can change learning method before sessionDate according to minDaysBeforeStartToUpdateMeeting.
   */
  public canChangeLearningMethod(): boolean {
    const changeLearningMethodDeadline = DateUtils.addDays(this.sessionDate, -this.minDaysBeforeStartToUpdateMeeting);
    return new Date() < changeLearningMethodDeadline;
  }
}
