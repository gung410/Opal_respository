import { ILearnerViolationCourseCriteria, LearnerViolationCourseCriteria } from './learner-violation-course-criteria.model';
import { SystemRoleEnum, UserInfoModel } from '../../share/models/user-info.model';

import { CAM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/cam-permission-key';
import { ClassRun } from './classrun.model';
import { Course } from './course.model';

export interface IRegistration {
  id: string;

  userId: string;
  courseId: string;
  classRunId?: string;
  registrationType: RegistrationType;
  createdDate?: Date;
  registrationDate?: Date;
  status: RegistrationStatus;
  lastStatusChangedDate?: Date;
  withdrawalStatus: WithdrawalStatus;
  withdrawalRequestDate: Date;
  classRunChangeStatus: ClassRunChangeStatus;
  classRunChangeRequestedDate?: Date;
  classRunChangeId: string;
  approvingOfficer: string;
  approvingDate?: Date;
  administratedBy: string;
  administrationDate: Date;
  createBy: string;
  externalId: string;
  learningContentProgress?: number;
  learningStatus?: RegistrationLearningStatus;
  courseCriteriaViolation?: ILearnerViolationCourseCriteria;
  courseCriteriaOverrided?: boolean;
  isExpired: boolean;
  postCourseEvaluationFormCompleted: boolean;
  learningCompletedDate?: Date;
}
export class Registration implements IRegistration {
  public static optionalProps: (keyof IRegistration)[] = ['courseCriteriaViolation'];
  public id: string;
  public userId: string;
  public createdDate?: Date;
  public registrationDate?: Date;
  public registrationType: RegistrationType;
  public courseId: string;
  public classRunId?: string;
  public status: RegistrationStatus;
  public lastStatusChangedDate?: Date;
  public withdrawalStatus: WithdrawalStatus;
  public withdrawalRequestDate: Date;
  public classRunChangeStatus: ClassRunChangeStatus;
  public classRunChangeRequestedDate?: Date;
  public classRunChangeId: string;
  public approvingOfficer: string;
  public approvingDate?: Date;
  public administratedBy: string;
  public administrationDate: Date;
  public createBy: string;
  public externalId: string;
  public learningContentProgress?: number;
  public learningStatus: RegistrationLearningStatus = RegistrationLearningStatus.NotStarted;
  public courseCriteriaViolation?: LearnerViolationCourseCriteria;
  public courseCriteriaOverrided?: boolean;
  public isExpired: boolean = false;
  public postCourseEvaluationFormCompleted: boolean = false;
  public learningCompletedDate?: Date;

  public static hasViewManagedRegistrationsPermission(user: UserInfoModel, course: Course, classRun: ClassRun): boolean {
    return (
      course.hasContentCreatorPermission(user) ||
      course.hasFacilitatorsPermission(user) ||
      course.hasAdministrationPermission(user) ||
      classRun.hasFacilitatorPermission(user, course) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.ViewRegistrations)
    );
  }

  public static hasManageRegistrationsPermission(user: UserInfoModel, course: Course, classRun: ClassRun): boolean {
    return (
      course.hasFacilitatorsPermission(user) ||
      course.hasAdministrationPermission(user) ||
      classRun.hasFacilitatorPermission(user, course) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations)
    );
  }

  public static canManageRegistrations(course: Course): boolean {
    return !course.isArchived();
  }

  public static canManageAnnouncement(user: UserInfoModel, course: Course, classRun: ClassRun): boolean {
    return (
      course.hasContentCreatorPermission(user) || course.hasFacilitatorsPermission(user) || classRun.hasFacilitatorPermission(user, course)
    );
  }

  public static canApproveRegistration(user: UserInfoModel): boolean {
    return user.hasAdministratorRoles() || user.hasRole(SystemRoleEnum.CourseAdministrator);
  }

  public static canActionOnParticipantList(classRun: ClassRun, course: Course): boolean {
    return !classRun.started() && !course.isArchived();
  }

  public static canDoLearningActionOnParticipantList(course: Course): boolean {
    return !course.isArchived();
  }

  public static hasDoLearningActionOnParticipantListPermission(user: UserInfoModel, course: Course): boolean {
    return course.hasAdministrationPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  public static canActionOnWaitlist(classRun: ClassRun, course: Course): boolean {
    return !classRun.started() && !course.isArchived();
  }

  public static hasActionOnWaitlistPermission(user: UserInfoModel, course: Course): boolean {
    return course.hasAdministrationPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  public static hasActionOnRegistrationListPermission(user: UserInfoModel, course: Course): boolean {
    return course.hasAdministrationPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  public static hasActionOnParticipantListPermission(user: UserInfoModel, course: Course): boolean {
    return course.hasAdministrationPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  public static hasActionOnWithdrawalRequestListPermission(user: UserInfoModel, course: Course): boolean {
    return course.hasAdministrationPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  public static hasActionOnChangeClassRequestListPermission(user: UserInfoModel, course: Course): boolean {
    return course.hasAdministrationPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  public static hasSendOfferPermission(user: UserInfoModel, course: Course): boolean {
    return course.hasAdministrationPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  public static canViewRegistrations(classRun: ClassRun): boolean {
    return classRun.canViewRegistrations();
  }

  public static hasViewRegistrationsPermissions(course: Course, user: UserInfoModel): boolean {
    return (
      course.hasContentCreatorPermission(user) ||
      course.hasAdministrationPermission(user) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.ViewRegistrations)
    );
  }

  public static hasWithdrawByCAPermission(course: Course, currentUser: UserInfoModel): boolean {
    return course.hasAdministrationPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  public static hasChangeClassByCAPermission(course: Course, currentUser: UserInfoModel): boolean {
    return course.hasAdministrationPermission(currentUser) && currentUser.hasPermissionPrefix(CAM_PERMISSIONS.ManageRegistrations);
  }

  constructor(data?: IRegistration) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.createdDate = data.createdDate ? new Date(data.createdDate) : undefined;
    this.registrationDate = data.registrationDate ? new Date(data.registrationDate) : undefined;
    this.registrationType = data.registrationType;
    this.courseId = data.courseId;
    this.classRunId = data.classRunId;
    this.status = data.status;
    this.lastStatusChangedDate = data.lastStatusChangedDate ? new Date(data.lastStatusChangedDate) : undefined;
    this.withdrawalStatus = data.withdrawalStatus;
    this.withdrawalRequestDate = data.withdrawalRequestDate ? new Date(data.withdrawalRequestDate) : undefined;
    this.classRunChangeStatus = data.classRunChangeStatus;
    this.classRunChangeRequestedDate = data.classRunChangeRequestedDate ? new Date(data.classRunChangeRequestedDate) : undefined;
    this.classRunChangeId = data.classRunChangeId;
    this.approvingOfficer = data.approvingOfficer;
    this.approvingDate = data.approvingDate ? new Date(data.approvingDate) : undefined;
    this.administratedBy = data.administratedBy;
    this.administrationDate = data.administrationDate ? new Date(data.administrationDate) : undefined;
    this.createBy = data.createBy;
    this.externalId = data.externalId;
    this.learningContentProgress = data.learningContentProgress;
    this.learningStatus = data.learningStatus != null ? data.learningStatus : RegistrationLearningStatus.NotStarted;
    this.courseCriteriaViolation =
      data.courseCriteriaViolation != null ? new LearnerViolationCourseCriteria(data.courseCriteriaViolation) : null;
    this.courseCriteriaOverrided = data.courseCriteriaOverrided;
    this.isExpired = data.isExpired;
    this.postCourseEvaluationFormCompleted = data.postCourseEvaluationFormCompleted;
    this.learningCompletedDate = data.learningCompletedDate;
  }

  public canChangeClassByCA(course: Course): boolean {
    return (
      [
        RegistrationStatus.PendingConfirmation,
        RegistrationStatus.WaitlistPendingApprovalByLearner,
        RegistrationStatus.WaitlistConfirmed,
        RegistrationStatus.OfferPendingApprovalByLearner,
        RegistrationStatus.ConfirmedByCA,
        RegistrationStatus.Approved
      ].includes(this.status) &&
      this.learningStatus === RegistrationLearningStatus.NotStarted &&
      !course.isArchived() &&
      !this.isExpired
    );
  }

  public canWithdrawByCA(course: Course): boolean {
    return (
      (this.status === RegistrationStatus.Approved ||
        this.status === RegistrationStatus.ConfirmedByCA ||
        this.status === RegistrationStatus.OfferConfirmed ||
        this.status === RegistrationStatus.WaitlistPendingApprovalByLearner ||
        this.status === RegistrationStatus.WaitlistConfirmed ||
        this.status === RegistrationStatus.OfferPendingApprovalByLearner) &&
      this.learningStatus === RegistrationLearningStatus.NotStarted &&
      !course.isArchived() &&
      !this.isExpired
    );
  }

  public canActionOnRegistrationList(classRun: ClassRun, course: Course): boolean {
    return (
      !course.isArchived() &&
      this.status === RegistrationStatus.Approved &&
      (!classRun.started() || course.isELearning()) &&
      !classRun.ended() &&
      !this.isExpired
    );
  }

  public canActionOnWithdrawalRequestList(classRun: ClassRun, course: Course): boolean {
    return !course.isArchived() && this.withdrawalStatus === WithdrawalStatus.Approved && !classRun.started() && !this.isExpired;
  }

  public canActionOnChangeClassRequestList(classRun: ClassRun, course: Course): boolean {
    return !course.isArchived() && this.classRunChangeStatus === ClassRunChangeStatus.Approved && !classRun.started() && !this.isExpired;
  }

  public canSendOffer(user: UserInfoModel, classRun: ClassRun, course: Course): boolean {
    return (
      Registration.canActionOnWaitlist(classRun, course) &&
      this.status === RegistrationStatus.WaitlistConfirmed &&
      (!classRun.started() || course.isELearning()) &&
      !classRun.ended() &&
      !this.isExpired
    );
  }

  public needToSetExpired(course: Course, classRun: ClassRun): boolean {
    return this.canBeSetExpired() && classRun.needSetExpiredRegistrations(course);
  }

  public isExpiredOrNeedToSetExpired(course: Course, classRun: ClassRun): boolean {
    return (this.canBeSetExpired() && classRun.needSetExpiredRegistrations(course)) || this.isExpired;
  }

  public canBeSetExpired(): boolean {
    return this.isExisted() && !this.isParticipant() && !this.isRejected();
  }

  public isParticipant(): boolean {
    return this.isExisted() && (this.status === RegistrationStatus.ConfirmedByCA || this.status === RegistrationStatus.OfferConfirmed);
  }

  public isRejected(): boolean {
    return (
      this.status === RegistrationStatus.RejectedByCA ||
      this.status === RegistrationStatus.Rejected ||
      this.status === RegistrationStatus.OfferRejected ||
      this.status === RegistrationStatus.WaitlistRejected ||
      this.withdrawalStatus === WithdrawalStatus.Withdrawn
    );
  }

  public isExisted(): boolean {
    return (
      this.withdrawalStatus !== WithdrawalStatus.Withdrawn &&
      this.classRunChangeStatus !== ClassRunChangeStatus.ConfirmedByCA &&
      this.isExpired === false
    );
  }
}
export enum RegistrationStatus {
  PendingConfirmation = 'PendingConfirmation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  ConfirmedByCA = 'ConfirmedByCA',
  RejectedByCA = 'RejectedByCA',
  WaitlistPendingApprovalByLearner = 'WaitlistPendingApprovalByLearner',
  WaitlistConfirmed = 'WaitlistConfirmed',
  WaitlistRejected = 'WaitlistRejected',
  OfferPendingApprovalByLearner = 'OfferPendingApprovalByLearner',
  OfferRejected = 'OfferRejected',
  OfferConfirmed = 'OfferConfirmed',
  AddedByCAConflict = 'AddedByCAConflict',
  AddedByCAClassfull = 'AddedByCAClassfull',

  // This is virtual status, used to display Unsuccessful when registrations need to be set expired
  Expired = 'Expired'
}
export enum RegistrationType {
  None = 'None',
  Manual = 'Manual',
  Application = 'Application',
  Nominated = 'Nominated',
  AddedByCA = 'AddedByCA'
}
export enum WithdrawalStatus {
  PendingConfirmation = 'PendingConfirmation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Withdrawn = 'Withdrawn',
  RejectedByCA = 'RejectedByCA'
}
export enum ClassRunChangeStatus {
  PendingConfirmation = 'PendingConfirmation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  ConfirmedByCA = 'ConfirmedByCA',
  RejectedByCA = 'RejectedByCA'
}

export enum RegistrationLearningStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Passed = 'Passed',
  Failed = 'Failed',
  Completed = 'Completed'
}
