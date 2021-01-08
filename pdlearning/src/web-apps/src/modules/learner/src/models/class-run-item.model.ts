import {
  ClassRunChangeStatus,
  ClassRunStatus,
  LearningStatus,
  MyClassRunModel,
  MyRegistrationStatus,
  RegistrationType,
  Session,
  WithdrawalStatus
} from '@opal20/domain-api';
import { ClassRunViewModel, IClassRunViewModel } from '@opal20/domain-components';
import { canProcessClassRun, isClassRunApplicable, isClassRunEnded, isClassRunStarted } from '../learner-utils';

import { APPLIED_REGISTRATION_STATUSES } from '../constants/learning-card-status.constant';
import { Utils } from '@opal20/infrastructure';
export interface ICourseClassRun extends IClassRunViewModel {
  myClassRun: MyClassRunModel;
  sessions: Session[];
  totalParticipant?: number;
}
export class CourseClassRun extends ClassRunViewModel {
  /**
   * reference to the MyClassRuns in CourseModel
   */
  public myClassRun: MyClassRunModel;
  public sessions: Session[];
  public totalParticipant?: number;

  public static createCourseClassRun(
    classRunViewModel: ClassRunViewModel,
    sessions: Session[],
    myClassRun?: MyClassRunModel,
    totalParticipant?: number
  ): CourseClassRun {
    return new CourseClassRun({
      ...Utils.toPureObject<ClassRunViewModel, IClassRunViewModel>(classRunViewModel),
      sessions,
      myClassRun,
      totalParticipant
    });
  }
  constructor(data?: ICourseClassRun) {
    super(data);
    if (!data) {
      return;
    }
    this.myClassRun = data.myClassRun;
    this.sessions = data.sessions ? data.sessions : [];
    this.totalParticipant = data.totalParticipant;
  }

  public resetRegistration(): void {
    this.myClassRun = undefined;
    this.totalParticipant = undefined;
  }

  //#region myclassrun proxy
  public get myRegistrationStatus(): MyRegistrationStatus {
    return this.myClassRun && this.myClassRun.status;
  }
  public get myWithdrawalStatus(): WithdrawalStatus {
    return this.myClassRun && this.myClassRun.withdrawalStatus;
  }
  public get myClassRunChangeStatus(): ClassRunChangeStatus {
    return this.myClassRun && this.myClassRun.classRunChangeStatus;
  }
  public get classRunChangeId(): string {
    return this.myClassRun && this.myClassRun.classRunChangeId;
  }
  public get registrationId(): string {
    return this.myClassRun && this.myClassRun.registrationId;
  }
  public get registrationType(): RegistrationType {
    return this.myClassRun && this.myClassRun.registrationType;
  }
  public get learningStatus(): LearningStatus {
    return this.myClassRun && this.myClassRun.learningStatus;
  }
  public get postCourseEvaluationFormCompleted(): boolean {
    return this.myClassRun && this.myClassRun.postCourseEvaluationFormCompleted;
  }

  public set myRegistrationStatus(v: MyRegistrationStatus) {
    this.myClassRun.status = v;
  }
  public set myWithdrawalStatus(v: WithdrawalStatus) {
    this.myClassRun.withdrawalStatus = v;
  }
  public set myClassRunChangeStatus(v: ClassRunChangeStatus) {
    this.myClassRun.classRunChangeStatus = v;
  }
  public set classRunChangeId(v: string) {
    this.myClassRun.classRunChangeId = v;
  }
  public set registrationId(v: string) {
    this.myClassRun.registrationId = v;
  }
  public set registrationType(v: RegistrationType) {
    this.myClassRun.registrationType = v;
  }
  public set learningStatus(v: LearningStatus) {
    this.myClassRun.learningStatus = v;
  }
  public set postCourseEvaluationFormCompleted(v: boolean) {
    this.myClassRun.postCourseEvaluationFormCompleted = v;
  }
  //#endregion

  public get isApplicable(): boolean {
    return isClassRunApplicable(this.applicationStartDate, this.applicationEndDate) && this.status === ClassRunStatus.Published;
  }

  public get isApplied(): boolean {
    return (
      APPLIED_REGISTRATION_STATUSES.includes(this.myRegistrationStatus) &&
      this.myClassRunChangeStatus !== ClassRunChangeStatus.ConfirmedByCA &&
      this.learningStatus !== LearningStatus.Failed
    );
  }

  public get isStarted(): boolean {
    return isClassRunStarted(this.startDate);
  }

  public get isEnded(): boolean {
    return isClassRunEnded(this.endDate);
  }

  public get isValidDateToChangeClassAndWithdraw(): boolean {
    const beforeWorkingDay = 1;
    return canProcessClassRun(this.startDate, beforeWorkingDay);
  }

  public get isNominated(): boolean {
    return this.registrationType === RegistrationType.Nominated || this.registrationType === RegistrationType.AddedByCA;
  }

  public get isChangingClass(): boolean {
    return (
      this.myClassRunChangeStatus &&
      this.myClassRunChangeStatus !== ClassRunChangeStatus.Rejected &&
      this.myClassRunChangeStatus !== ClassRunChangeStatus.RejectedByCA
    );
  }

  public get isClassRunFinished(): boolean {
    return this.myClassRun && this.myClassRun.isClassRunFinished;
  }

  public get isParticipant(): boolean {
    return (
      this.myRegistrationStatus &&
      (this.myRegistrationStatus === MyRegistrationStatus.ConfirmedByCA ||
        this.myRegistrationStatus === MyRegistrationStatus.OfferConfirmed)
    );
  }

  public get hasOffer(): boolean {
    return (
      this.myRegistrationStatus === MyRegistrationStatus.WaitlistPendingApprovalByLearner ||
      this.myRegistrationStatus === MyRegistrationStatus.OfferPendingApprovalByLearner
    );
  }

  /**
   * Class is not published or ended
   */
  public get isClassRunUnavailable(): boolean {
    return this.status !== ClassRunStatus.Published || this.isEnded;
  }
}
