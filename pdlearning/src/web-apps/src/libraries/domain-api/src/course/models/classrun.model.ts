import { ContentStatus, Course } from './course.model';
import { DateUtils, Utils } from '@opal20/infrastructure';

import { CAM_PERMISSIONS } from '../../share/permission-keys/cam-permission-key';
import { LMM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/lmm-permission-key';
import { UserInfoModel } from '../../share/models/user-info.model';

export interface IClassRun {
  id?: string;

  courseId: string;
  classTitle: string;
  classRunCode: string;
  startDateTime?: Date;
  endDateTime?: Date;
  planningStartTime?: Date;
  planningEndTime?: Date;
  facilitatorIds: string[];
  coFacilitatorIds: string[];
  minClassSize: number;
  maxClassSize: number;
  applicationStartDate?: Date;
  applicationEndDate?: Date;
  status: ClassRunStatus;
  classRunVenueId?: string;
  cancellationStatus?: ClassRunCancellationStatus;
  rescheduleStatus?: ClassRunRescheduleStatus;
  rescheduleStartDateTime?: Date;
  rescheduleEndDateTime?: Date;
  rescheduleStartDate?: Date;
  rescheduleEndDate?: Date;
  createdBy: string;
  createdDate: Date;
  changedBy: string;
  submittedContentDate?: Date;
  approvalContentDate?: Date;
  contentStatus: ContentStatus;
  publishedContentDate?: Date;
  hasContent?: boolean;
  courseCriteriaActivated: boolean;
  courseAutomateActivated: boolean;
  hasLearnerStarted?: boolean;
}
export class ClassRun implements IClassRun {
  public static optionalProps: (keyof IClassRun)[] = ['hasContent', 'hasLearnerStarted'];

  public id?: string;
  public courseId: string;
  public classTitle: string = '';
  public classRunCode: string;
  public planningStartTime?: Date = null;
  public planningEndTime?: Date = null;
  public facilitatorIds: string[] = [];
  public coFacilitatorIds: string[] = [];
  public minClassSize: number;
  public maxClassSize: number;
  public applicationStartDate?: Date = new Date();
  public applicationEndDate?: Date = new Date();
  public status: ClassRunStatus = ClassRunStatus.Unpublished;
  public classRunVenueId?: string;
  public cancellationStatus?: ClassRunCancellationStatus;
  public rescheduleStatus?: ClassRunRescheduleStatus;
  public rescheduleStartDateTime?: Date;
  public rescheduleEndDateTime?: Date;
  public rescheduleStartDate?: Date;
  public rescheduleEndDate?: Date;
  public createdBy: string;
  public createdDate: Date = new Date();
  public changedBy: string;
  public contentStatus: ContentStatus = ContentStatus.Draft;
  public submittedContentDate?: Date;
  public approvalContentDate?: Date;
  public publishedContentDate?: Date;
  public hasContent?: boolean;
  public courseCriteriaActivated: boolean = false;
  public courseAutomateActivated: boolean = false;
  public hasLearnerStarted?: boolean;
  public _startDateTime?: Date = new Date();
  public get startDateTime(): Date | null {
    return this._startDateTime;
  }
  public set startDateTime(v: Date) {
    this._startDateTime = v;
    this._startDate = v ? DateUtils.removeTime(v) : null;
  }
  public _endDateTime?: Date = new Date();
  public get endDateTime(): Date | null {
    return this._endDateTime;
  }
  public set endDateTime(v: Date) {
    this._endDateTime = v;
    this._endDate = v ? DateUtils.removeTime(v) : null;
  }
  public _startDate: Date | null;
  public get startDate(): Date | null {
    return this._startDate;
  }
  public set startDate(startDate: Date) {
    this._startDate = startDate;
    DateUtils.setDateOnly(startDate, this.startDateTime);
  }
  public _endDate: Date | null;
  public get endDate(): Date | null {
    return this._endDate;
  }
  public set endDate(endDate: Date) {
    this._endDate = endDate;
    DateUtils.setDateOnly(endDate, this.endDateTime);
  }

  public static displayContentStatus(classRun: ClassRun, course: Course, classHasContentChecker?: (p: ClassRun) => boolean): ContentStatus {
    return (classHasContentChecker && !classHasContentChecker(classRun)) || classRun.hasContent === false
      ? course.contentStatus
      : classRun.contentStatus;
  }

  public static isContentResubmit(classRun: ClassRun, course: Course, classHasContentChecker?: (p: ClassRun) => boolean): boolean {
    return (classHasContentChecker && !classHasContentChecker(classRun)) || classRun.hasContent === false
      ? course.isContentResubmit()
      : classRun.isContentResubmit();
  }

  public static hasApprovalCancellationClassRunRequestPermission(course: Course, currentUser: UserInfoModel): boolean {
    return (
      course.hasApprovalPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.CancellationClassRunRequestApproval)
    );
  }

  public static hasApprovalRescheduleClassRunRequestPermission(course: Course, currentUser: UserInfoModel): boolean {
    return course.hasApprovalPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.RescheduledClassRunRequestApproval);
  }

  public static hasPublishUnPublishPermission(course: Course, currentUser: UserInfoModel): boolean {
    return course.hasAdministrationPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.PublishUnpublishClassRun);
  }

  public static hasCancelClassPermission(course: Course, currentUser: UserInfoModel): boolean {
    return course.hasAdministrationPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.CancelClassRun);
  }

  public static hasRescheduleClassPermission(course: Course, currentUser: UserInfoModel): boolean {
    return course.hasAdministrationPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.RescheduleClassRun);
  }

  public static hasCreateSessionPermission(currentUser: UserInfoModel, course: Course): boolean {
    return course.hasAdministrationPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.CreateEditSession);
  }

  public static hasViewSessionPermission(currentUser: UserInfoModel, course: Course): boolean {
    return course && course.hasViewSessionPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.ViewSessionDetail);
  }

  public static hasEditClassRunPermission(course: Course, currentUser: UserInfoModel): boolean {
    return course.hasAdministrationPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.CreateEditClassRun);
  }

  constructor(data?: IClassRun) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.courseId = data.courseId;
    this.classTitle = data.classTitle;
    this.classRunCode = data.classRunCode;
    this.startDateTime = data.startDateTime ? new Date(data.startDateTime) : null;
    this.endDateTime = data.endDateTime ? new Date(data.endDateTime) : null;
    this.planningStartTime = data.planningStartTime ? new Date(data.planningStartTime) : null;
    this.planningEndTime = data.planningEndTime ? new Date(data.planningEndTime) : null;
    this.facilitatorIds = Utils.defaultIfNull(data.facilitatorIds, []);
    this.coFacilitatorIds = Utils.defaultIfNull(data.coFacilitatorIds, []);
    this.minClassSize = data.minClassSize;
    this.maxClassSize = data.maxClassSize;
    this.applicationStartDate = data.applicationStartDate ? new Date(data.applicationStartDate) : null;
    this.applicationEndDate = data.applicationEndDate ? new Date(data.applicationEndDate) : null;
    this.status = data.status ? data.status : ClassRunStatus.Unpublished;
    this.classRunVenueId = data.classRunVenueId;
    this.cancellationStatus = data.cancellationStatus;
    this.rescheduleStatus = data.rescheduleStatus;
    this.rescheduleStartDateTime = data.rescheduleStartDateTime ? new Date(data.rescheduleStartDateTime) : new Date();
    this.rescheduleEndDateTime = data.rescheduleEndDateTime ? new Date(data.rescheduleEndDateTime) : new Date();
    this.rescheduleStartDate = data.rescheduleStartDateTime ? new Date(data.rescheduleStartDateTime) : new Date();
    this.rescheduleEndDate = data.rescheduleEndDateTime ? new Date(data.rescheduleEndDateTime) : new Date();
    this.createdBy = data.createdBy;
    this.createdDate = new Date(data.createdDate);
    this.changedBy = data.changedBy;
    this.contentStatus = data.contentStatus ? data.contentStatus : ContentStatus.Draft;
    this.submittedContentDate = data.submittedContentDate ? new Date(data.submittedContentDate) : undefined;
    this.approvalContentDate = data.approvalContentDate ? new Date(data.approvalContentDate) : undefined;
    this.publishedContentDate = data.publishedContentDate ? new Date(data.publishedContentDate) : undefined;
    this.hasContent = data.hasContent;
    this.courseCriteriaActivated = data.courseCriteriaActivated;
    this.courseAutomateActivated = data.courseAutomateActivated;
    this.hasLearnerStarted = data.hasLearnerStarted;
  }

  public isPublishedOnce(): boolean {
    return this.classRunCode != null && this.classRunCode !== '';
  }

  public isContentResubmit(): boolean {
    return this.approvalContentDate && (this.contentStatus === ContentStatus.Draft || this.contentStatus === ContentStatus.PendingApproval);
  }

  public isNotUnpublished(): boolean {
    return this.status === ClassRunStatus.Published || this.status === ClassRunStatus.Cancelled;
  }

  public isNotCancelled(): boolean {
    return this.status === ClassRunStatus.Published || this.status === ClassRunStatus.Unpublished;
  }

  public canViewRegistrations(): boolean {
    return this.isNotUnpublished() && this.isNotCancelled();
  }

  public dontHaveCancellationRequest(): boolean {
    return this.cancellationStatus !== ClassRunCancellationStatus.PendingApproval;
  }

  public dontHaveRescheduleRequest(): boolean {
    return this.rescheduleStatus !== ClassRunRescheduleStatus.PendingApproval;
  }

  public isCancelPendingApproval(): boolean {
    return this.cancellationStatus === ClassRunCancellationStatus.PendingApproval;
  }

  public isReschedulePendingApproval(): boolean {
    return this.rescheduleStatus === ClassRunRescheduleStatus.PendingApproval;
  }

  public canSendCancellationRequest(course: Course): boolean {
    return (
      (this.cancellationStatus !== ClassRunCancellationStatus.PendingApproval || this.cancellationStatus == null) && !course.isArchived()
    );
  }

  public canSendRescheduleRequest(course: Course): boolean {
    return this.rescheduleStatus !== ClassRunRescheduleStatus.PendingApproval || (this.rescheduleStatus == null && !course.isArchived());
  }

  public publishedAndStarted(): boolean {
    return this.status === ClassRunStatus.Published && this.started();
  }

  public publishedAndEnded(): boolean {
    return this.status === ClassRunStatus.Published && this.ended();
  }

  public communityCreated(): boolean {
    return this.status === ClassRunStatus.Published;
  }

  public publishedAndNotStarted(): boolean {
    return this.status === ClassRunStatus.Published && !this.started();
  }

  public publishedAndNotEnded(): boolean {
    return this.status === ClassRunStatus.Published && !this.ended();
  }

  public started(): boolean {
    return this.startDate != null && DateUtils.removeTime(new Date()) >= DateUtils.removeTime(this.startDate);
  }

  public needSetExpiredRegistrations(course: Course): boolean {
    return !course.isELearning() && this.started();
  }

  public ended(): boolean {
    return this.endDate != null && DateUtils.removeTime(new Date()) >= DateUtils.removeTime(this.endDate);
  }

  public applicationStarted(): boolean {
    return this.applicationStartDate != null && DateUtils.removeTime(new Date()) >= DateUtils.removeTime(this.applicationStartDate);
  }

  public rescheduled(): boolean {
    return this.rescheduleStatus === ClassRunRescheduleStatus.Approved && this.rescheduleStartDateTime != null;
  }

  public hasFacilitatorPermission(user: UserInfoModel, course: Course): boolean {
    return this.facilitatorIds.includes(user.id) || this.coFacilitatorIds.includes(user.id) || course.hasFullRight;
  }

  public canPublish(course: Course): boolean {
    return course && course.isPublishedOnce() && this.status === ClassRunStatus.Unpublished && !course.isArchived();
  }

  public canUnpublish(course: Course): boolean {
    return course && this.status === ClassRunStatus.Published && !course.isArchived();
  }

  public canCancelClass(course: Course): boolean {
    return (
      course &&
      this.started() &&
      this.canSendCancellationRequest(course) &&
      this.dontHaveRescheduleRequest() &&
      this.isNotCancelled() &&
      !course.isArchived()
    );
  }

  public canRescheduleClass(course: Course): boolean {
    return (
      course &&
      this.applicationStarted() &&
      this.canSendRescheduleRequest(course) &&
      this.dontHaveCancellationRequest() &&
      this.isNotCancelled() &&
      !course.isArchived()
    );
  }

  public canCreateSession(course: Course): boolean {
    return course && this.isNotCancelled() && !this.started() && !course.isArchived();
  }

  public canViewSession(): boolean {
    return this.isNotCancelled();
  }

  public canPublishContent(course: Course): boolean {
    return (this.contentStatus === ContentStatus.Approved || this.contentStatus === ContentStatus.Unpublished) && !course.isArchived();
  }

  public canUnpublishContent(course: Course): boolean {
    return this.contentStatus === ContentStatus.Published && !course.isArchived();
  }

  public hasViewParticipantPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ViewParticipantTab);
  }

  public hasViewCompletionRatePermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ViewCompletionRate);
  }

  public canAssignAssignment(): boolean {
    return this.endDate >= new Date();
  }

  public buildEndDateTime(): Date {
    let result = DateUtils.buildDateTime(this.endDate, this.planningEndTime);
    if (this.planningEndTime == null) {
      result = DateUtils.setTimeToEndInDay(result);
    }
    return result;
  }

  public buildApplicationStartTime(): Date {
    return DateUtils.removeTime(this.applicationStartDate);
  }

  public buildApplicationEndTime(): Date {
    return DateUtils.setTimeToEndInDay(this.applicationEndDate);
  }

  public buildStartDateTime(): Date {
    return DateUtils.buildDateTime(this.startDate, this.planningStartTime);
  }

  public getAllUserIds(): string[] {
    return [].concat(this.facilitatorIds ? this.facilitatorIds : []).concat(this.coFacilitatorIds ? this.coFacilitatorIds : []);
  }

  public isContentEditable(course: Course): boolean {
    return this.contentStatus !== ContentStatus.Published && !course.isArchived() && !this.hasLearnerStarted;
  }

  public isEditable(course: Course): boolean {
    return course && !this.started() && this.status === ClassRunStatus.Unpublished && !course.isArchived();
  }

  public canUserEditContent(course: Course, user: UserInfoModel): boolean {
    const hasPermission = course.hasContentCreatorPermission(user) || course.hasFacilitatorsPermission(user);
    return hasPermission && this.isContentEditable(course);
  }

  public canCancellationRequestBeApproved(course: Course): boolean {
    return !course.isArchived() && this.cancellationStatus === ClassRunCancellationStatus.PendingApproval;
  }

  public canRescheduleRequestBeApproved(course: Course): boolean {
    return !course.isArchived() && this.rescheduleStatus === ClassRunRescheduleStatus.PendingApproval;
  }
}
export enum ClassRunStatus {
  Published = 'Published',
  Unpublished = 'Unpublished',
  Cancelled = 'Cancelled'
}

export enum ClassRunCancellationStatus {
  PendingApproval = 'PendingApproval',
  Approved = 'Approved',
  Rejected = 'Rejected'
}

export enum ClassRunRescheduleStatus {
  PendingApproval = 'PendingApproval',
  Approved = 'Approved',
  Rejected = 'Rejected'
}

export interface IClassRunCompletionRateInfo {
  classRunId: string;
  completionRate: number;
}
