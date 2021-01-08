import {
  AttendanceTracking,
  ClassRunApiService,
  ClassRunChangeStatus,
  IChangeRegistrationStatusRequest,
  IClassRunChangeRequest,
  ICreateRegistrationRequest,
  ITotalParticipantClassRunRequest,
  IWithdrawalRequest,
  LearningStatus,
  MyClassRunModel,
  MyCourseDisplayStatus,
  MyCourseModel,
  MyCourseStatus,
  MyRegistrationStatus,
  RegistrationApiService,
  RegistrationMethod,
  RegistrationStatus,
  RegistrationType,
  SearchClassRunType,
  Session,
  SessionApiService,
  UserInfoModel,
  WithdrawalStatus
} from '@opal20/domain-api';
import { BehaviorSubject, Observable, Subject, combineLatest } from 'rxjs';
import {
  CLASSRUNCHANGE_STATUS_TO_DISPLAY_STATUS_MAP,
  WITHDRAWAL_STATUS_TO_DISPLAY_STATUS_MAP
} from '../constants/learning-card-status.constant';
import { ClassRunViewModel, ListClassRunGridComponentService } from '@opal20/domain-components';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, switchMap } from 'rxjs/operators';

import { CourseClassRun } from '../models/class-run-item.model';
import { CourseModel } from '../models/course.model';
import { Injectable } from '@angular/core';
import { toDisplayStatusFromRegistrationStatus } from '../learner-utils';
@Injectable()
export class ClassRunDataService {
  public course: CourseModel;
  /**
   * Current applied class run.
   * Not Withdrawn.
   * Not Rejected.
   * Not Cancelled.
   * Not Ended
   */
  public currentClassRun$: BehaviorSubject<CourseClassRun> = new BehaviorSubject<CourseClassRun>(null);
  /**
   * Triggered when learner reject a Offer or a Waitlist
   */
  public displayStatus$: BehaviorSubject<MyCourseDisplayStatus> = new BehaviorSubject<MyCourseDisplayStatus>(null);

  public upcomingSession: Subject<Session> = new Subject<Session>();
  public updateAttendanceTracking: Subject<AttendanceTracking> = new Subject<AttendanceTracking>();
  public completedAttendanceTrackingSubject: Subject<boolean> = new Subject<boolean>();

  public openCantParticipateDialog: Subject<AttendanceTracking> = new Subject<AttendanceTracking>();
  public openCheckInDialog: Subject<Session> = new Subject<Session>();

  public courseClassRunItems: CourseClassRun[];
  public currentClassRun: CourseClassRun;

  constructor(
    private listClassRunGridComponentService: ListClassRunGridComponentService,
    private sessionApiService: SessionApiService,
    private classRunApiService: ClassRunApiService,
    private registrationApiService: RegistrationApiService
  ) {}

  public getClassRunsWithSessionData(): Observable<CourseClassRun[]> {
    const selecteds = {};
    const checkAll: boolean = false;
    const state: PageChangeEvent = {
      skip: 0,
      take: 25
    };
    return this.listClassRunGridComponentService
      .loadClassRunsByCourseId(
        this.course.courseId,
        SearchClassRunType.Learner,
        '',
        null,
        false,
        false,
        state.skip,
        state.take,
        checkAll,
        () => selecteds
      )
      .pipe(
        switchMap((gridDataResult: GridDataResult) => {
          if (!gridDataResult || !gridDataResult.data || !gridDataResult.data.length) {
            return;
          }
          const classRunData: ClassRunViewModel[] = gridDataResult.data;
          const myClassRuns = this.course.myClassRuns;
          const myClassRunIds = myClassRuns.map(p => p.classRunId);
          const classRunIds = classRunData.map(classRun => classRun.id);
          const request: ITotalParticipantClassRunRequest = { classRunIds: classRunIds };
          const totalParticipantInClassRun$ = this.classRunApiService.getTotalParticipantInClassRun(request);
          const sessionsByIds$ = this.sessionApiService.getSessionsByIds(classRunIds);

          return combineLatest(totalParticipantInClassRun$, sessionsByIds$).pipe(
            map(([totalParticipantInClassRunResponse, allSessionsResponse]) => {
              const courseClassRunItems = classRunData
                .map(item => {
                  const sessions = allSessionsResponse ? allSessionsResponse.filter(s => s.classRunId === item.id) : [];
                  const myClassRunItem = this.course.myClassRuns.find(myClsRun => item.id === myClsRun.classRunId);
                  const totalParticipant =
                    totalParticipantInClassRunResponse && totalParticipantInClassRunResponse.find(p => p.classRunId === item.id);
                  const totalParticipantInClassRun = totalParticipant ? totalParticipant.participantTotal : 0;
                  return CourseClassRun.createCourseClassRun(item, sessions, myClassRunItem, totalParticipantInClassRun);
                })
                .filter(item => myClassRunIds.includes(item.id) || item.isApplicable);

              return courseClassRunItems;
            })
          );
        })
      );
  }

  public get isPrivateCourse(): boolean {
    return this.course && this.course.isPrivateCourse;
  }

  public get isUnPublishedCourse(): boolean {
    return this.course && this.course.isUnPublished;
  }

  public get isArchivedCourse(): boolean {
    return this.course && this.course.isArchived;
  }

  public get isExpiredCourse(): boolean {
    return this.course && this.course.isExpired;
  }

  public get courseId(): string {
    return this.course && this.course.courseId;
  }

  public get isByPassAOFlow(): boolean {
    const registrationMethod = this.course && this.course.courseDetail.registrationMethod;
    return registrationMethod === RegistrationMethod.Public;
  }

  public get rejectedClassRunDict(): Dictionary<MyClassRunModel> {
    return this.course.rejectedClassRunDict;
  }

  public get withdrawnClassRunDict(): Dictionary<MyClassRunModel> {
    return this.course.withdrawnClassRunDict;
  }

  public hasReachedMaxRelearningTimes(): boolean {
    return this.course.hasReachedMaxRelearningTimes();
  }

  public updateDisplayStatus(newStatus: MyCourseDisplayStatus): void {
    this.course.myCourseInfo.displayStatus = newStatus;
  }

  /**
   * Update new status.
   * Update Current Class Run.
   * Update display status.
   */
  public acceptOffer(courseClassrun: CourseClassRun): void {
    const targetStatus = this.getOfferStatus(courseClassrun, true);
    this.changeRegistrationStatus(courseClassrun, targetStatus);
  }

  /**
   * Update new status.
   * Remove in MyClassRuns and CourseClassRuns.
   * Update Current Class Run.
   * Update display status.
   */
  public rejectOffer(courseClassrun: CourseClassRun): void {
    const targetStatus = this.getOfferStatus(courseClassrun, false);
    this.changeRegistrationStatus(courseClassrun, targetStatus).then(() => {
      const myClassIndex = this.course.myClassRuns.indexOf(courseClassrun.myClassRun);
      this.course.myClassRuns.splice(myClassIndex, 1);

      courseClassrun.resetRegistration();

      // case nominate and manual in same class
      if (this.course.myClassRun && this.course.myClassRun.classRunId === courseClassrun.id) {
        courseClassrun.myClassRun = this.course.myClassRun;
      }

      this.updateCurrentClassRun();
      if (this.currentClassRun && !this.currentClassRun.isClassRunFinished) {
        // case have 2 registrations are pending (nominated, manual)
        const newDisplayStatus = toDisplayStatusFromRegistrationStatus(
          this.currentClassRun.myRegistrationStatus,
          this.currentClassRun.registrationType
        );
        this.updateDisplayStatus(newDisplayStatus);
      }
    });
  }

  /**
   * Update ClassRunChangeId by destination ClassRunId.
   * Update MyClassRunChangeStatus.
   * Update display status.
   */
  public changeClassRun(courseClassrun: CourseClassRun, confirmationReason: string): void {
    const changeClassRunRequest: IClassRunChangeRequest = {
      registrationId: this.currentClassRun.registrationId,
      classRunChangeId: courseClassrun.id,
      comment: confirmationReason
    };
    this.registrationApiService.createClassRunChange(changeClassRunRequest).then(() => {
      this.currentClassRun.classRunChangeId = courseClassrun.id;
      this.currentClassRun.myClassRunChangeStatus = this.isByPassAOFlow
        ? ClassRunChangeStatus.Approved
        : ClassRunChangeStatus.PendingConfirmation;
      this.updateDisplayStatus(CLASSRUNCHANGE_STATUS_TO_DISPLAY_STATUS_MAP.get(this.currentClassRun.myClassRunChangeStatus));
    });
  }

  /**
   * Update MyWithdrawalStatus.
   * Update display status.
   */
  public withdrawClassRun(confirmationReason: string): void {
    const withdrawalRequest: IWithdrawalRequest = {
      ids: [this.currentClassRun.registrationId],
      withdrawalStatus: WithdrawalStatus.PendingConfirmation,
      comment: confirmationReason
    };
    this.registrationApiService.withdrawal(withdrawalRequest).then(() => {
      this.currentClassRun.myWithdrawalStatus = this.isByPassAOFlow ? WithdrawalStatus.Approved : WithdrawalStatus.PendingConfirmation;
      this.updateDisplayStatus(WITHDRAWAL_STATUS_TO_DISPLAY_STATUS_MAP.get(this.currentClassRun.myWithdrawalStatus));
    });
  }

  /**
   * New MyCourse\n/n
   * New MyClassrun.
   * Remove expiredClassRun.
   * Update MyClassRun in CourseClassRun.
   * Update Current Class Run.
   * Update display status.
   */
  public applyClassRun(courseClassrun: CourseClassRun, approvingOfficerId: string, alternativeApprovingOfficerId: string): Promise<void> {
    const classRunData: ICreateRegistrationRequest = {
      registrationType: RegistrationType.Manual,
      approvingOfficer: approvingOfficerId,
      alternativeApprovingOfficer: alternativeApprovingOfficerId,
      registrations: [
        {
          courseId: this.course.courseId,
          classRunId: courseClassrun.id
        }
      ]
    };
    return this.registrationApiService.createRegistration(classRunData).then(registrations => {
      const newMyCourse = new MyCourseModel({
        id: null,
        courseId: this.courseId,
        userId: UserInfoModel.getMyUserInfo().id,
        status: MyCourseStatus.NotStarted,
        createdDate: null,
        createdBy: null
      });
      this.course.myCourseInfo = new MyCourseModel(newMyCourse);

      const appliedRegistration = registrations[0];

      const newClassRun = new MyClassRunModel({
        userId: UserInfoModel.getMyUserInfo().extId,
        courseId: appliedRegistration.courseId,
        classRunId: appliedRegistration.classRunId,
        status: MyRegistrationStatus[appliedRegistration.status],
        registrationId: appliedRegistration.id,
        registrationType: RegistrationType.Manual,
        learningStatus: LearningStatus.NotStarted,
        postCourseEvaluationFormCompleted: false
      });

      this.course.expiredMyClassRun = undefined;
      this.course.myClassRuns.unshift(newClassRun);

      courseClassrun.myClassRun = newClassRun;

      this.updateCurrentClassRun();
      this.updateDisplayStatus(MyCourseDisplayStatus[courseClassrun.myRegistrationStatus]);
    });
  }

  /**
   * Update the CurentClassRun based on the myClassRun on Course model.
   * Trigger when rejecting, applying a class and loading first time.
   */
  public updateCurrentClassRun(): void {
    const newCurrentClassRun =
      this.course.myClassRun && this.courseClassRunItems
        ? this.courseClassRunItems.find(p => p.myClassRun === this.course.myClassRun)
        : undefined;
    this.currentClassRun = newCurrentClassRun;
    this.currentClassRun$.next(this.currentClassRun);
  }

  public updateCourseModel(course: CourseModel): void {
    this.course = course;
    const myClassRuns = this.course.myClassRuns;

    if (this.courseClassRunItems && this.courseClassRunItems.length) {
      this.courseClassRunItems.forEach(p => {
        const myClassRun = myClassRuns.find(classRun => classRun.classRunId === p.id);
        p.myClassRun = myClassRun;
      });
    }
    this.updateCurrentClassRun();
  }

  /**
   * Update new status.
   * Update Current Class Run.
   * Update display status.
   */
  private changeRegistrationStatus(courseClassrun: CourseClassRun, targetStatus: MyRegistrationStatus): Promise<void> {
    const registrationChangeStatus: IChangeRegistrationStatusRequest = {
      ids: [courseClassrun.registrationId],
      // Reason to user RegistrationStatus[targetStatus] because we reused service in courseAPI
      status: RegistrationStatus[targetStatus],
      comment: ''
    };
    return this.registrationApiService.changeRegistrationStatus(registrationChangeStatus).then(() => {
      courseClassrun.myRegistrationStatus = targetStatus;

      this.updateDisplayStatus(MyCourseDisplayStatus[courseClassrun.myRegistrationStatus]);
    });
  }

  private getOfferStatus(courseClassrun: CourseClassRun, isAccept: boolean): MyRegistrationStatus {
    switch (courseClassrun.myRegistrationStatus) {
      case MyRegistrationStatus.WaitlistPendingApprovalByLearner:
        return isAccept ? MyRegistrationStatus.WaitlistConfirmed : MyRegistrationStatus.WaitlistRejected;
      case MyRegistrationStatus.OfferPendingApprovalByLearner:
        return isAccept ? MyRegistrationStatus.OfferConfirmed : MyRegistrationStatus.OfferRejected;
    }
  }
}
