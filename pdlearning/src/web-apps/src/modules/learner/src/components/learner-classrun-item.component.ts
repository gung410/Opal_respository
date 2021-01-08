import * as moment from 'moment';

import { ATTENDANCE_STATUS_COLOR_MAP, AttendanceStatusEnum } from '../models/session-status.model';
import { AbsenceDetailDialogComponent, LEARNER_PERMISSIONS } from '@opal20/domain-components';
import {
  AttendanceStatus,
  AttendanceTracking,
  AttendanceTrackingService,
  BookingSource,
  ClassRunRescheduleStatus,
  ClassRunStatus,
  LearningStatus,
  MyRegistrationStatus,
  Session,
  WebinarApiService
} from '@opal20/domain-api';
import { BaseComponent, DateUtils, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { isEmpty, orderBy } from 'lodash-es';

import { ApplyClassRunStateType } from './learner-classrun.component';
import { ClassRunDataService } from '../services/class-run-data.service';
import { CourseClassRun } from '../models/class-run-item.model';
import { CourseModel } from '../models/course.model';
import { LEARNER_CLASSRUN_STATUS_COLOR_MAP } from '../constants/learner-classrun-status-mapping.const';
import { LearnerWithdrawalReasonDialog } from './learner-withdrawal-reason-dialog/learner-withdrawal-reason-dialog.component';
import { OpalDialogService } from '@opal20/common-components';

const COMPLETE_RATIO_CONST: number = 0.75;
@Component({
  selector: 'learner-classrun-item',
  templateUrl: './learner-classrun-item.component.html'
})
export class LearnerClassRunItemComponent extends BaseComponent {
  @Input() public item: CourseClassRun;
  @Input() public course: CourseModel;
  @Input() public applyClassRunState: ApplyClassRunStateType;
  @Input() public showActionButton: boolean = true;
  @Input() public showApplyButton: boolean = true;
  @Input() public showWithdrawButton: boolean = false;
  @Input() public showChangeClassButton: boolean = false;
  @Input() public readyToLearn: boolean = false;
  @Output() public apply: EventEmitter<CourseClassRun> = new EventEmitter<CourseClassRun>();
  @Output() public withdraw: EventEmitter<CourseClassRun> = new EventEmitter<CourseClassRun>();
  @Output() public changeClassRun: EventEmitter<CourseClassRun> = new EventEmitter<CourseClassRun>();
  public learnerClassRunStatusColorMap = LEARNER_CLASSRUN_STATUS_COLOR_MAP;
  public sessionMapping = ATTENDANCE_STATUS_COLOR_MAP;
  public readonly applyText: string = 'Apply';

  public rescheduleStatus = ClassRunRescheduleStatus;

  public ClassRunStatus: typeof ClassRunStatus = ClassRunStatus;

  public isExpanding = false;
  public isExpandingClassRun: boolean;
  public expandingSessions: Dictionary<boolean> = {};

  public attendanceTrackingDict: Dictionary<AttendanceTracking> = {};
  public attendanceTrackings: AttendanceTracking[] = [];
  public hasAttendanceTracking: boolean = false;
  public completedAttendanceTracking: boolean = false;

  public upcomingSession: Session;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private classRunDataService: ClassRunDataService,
    private attendanceTrackingService: AttendanceTrackingService,
    private opalDialogService: OpalDialogService,
    private webinarApiService: WebinarApiService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.sortSessions();
    this.listenCheckCompleted();
    this.getAttendanceTracking();
  }

  public get showLinkCommunity(): boolean {
    const myRegistrationStatus = this.item.myRegistrationStatus;

    return (
      myRegistrationStatus === MyRegistrationStatus.OfferConfirmed ||
      myRegistrationStatus === MyRegistrationStatus.ConfirmedByCA ||
      this.item.learningStatus === LearningStatus.Completed ||
      this.item.learningStatus === LearningStatus.Failed
    );
  }

  public applyClicked(): void {
    this.apply.emit(this.item);
  }

  public onWithdrawClicked(): void {
    this.withdraw.emit(this.item);
  }

  public onChangeClassClicked(): void {
    this.changeClassRun.emit(this.item);
  }

  public getDateTime(startDate: Date, startTime: Date): Date {
    const datetime = startDate;
    datetime.setHours(startTime.getHours());
    datetime.setMinutes(startTime.getMinutes());
    return datetime;
  }

  public onCheckinClicked(session: Session): void {
    this.classRunDataService.openCheckInDialog.next(session);
  }

  public onCantParticipate(session: Session): void {
    this.classRunDataService.openCantParticipateDialog.next(this.attendanceTrackingDict[session.id]);
  }

  public onToggleSession(sessionId: string): void {
    this.expandingSessions[sessionId] = !this.expandingSessions[sessionId];
  }

  public showCheckin(session: Session): boolean {
    return (
      this.upcomingSession &&
      (this.attendanceTrackingDict[session.id] == null ||
        (this.attendanceTrackingDict[session.id].status == null && this.attendanceTrackingDict[session.id].isCodeScanned === false)) &&
      moment().isSame(session.startDateTime, 'day')
    );
  }

  public isAbsent(session: Session): boolean {
    return this.attendanceTrackingDict[session.id] && this.attendanceTrackingDict[session.id].status === AttendanceStatus.Absent;
  }

  public hasAbsenceMessage(session: Session): boolean {
    return !isEmpty(this.attendanceTrackingDict[session.id].reasonForAbsence);
  }

  public currentStatusMapping(attendanceTracking: AttendanceTracking): AttendanceStatusEnum {
    return attendanceTracking.status === AttendanceStatus.Absent ? AttendanceStatusEnum.Incompleted : AttendanceStatusEnum.Completed;
  }

  public viewAbsenceMessage(session: Session): void {
    const attendanceTrackingItem = this.attendanceTrackingDict[session.id];
    this.opalDialogService.openDialogRef(AbsenceDetailDialogComponent, {
      title: `${this.translate('Reason for Absence')}`,
      reasonForAbsence: attendanceTrackingItem.reasonForAbsence,
      attachment: attendanceTrackingItem.attachment
    });
  }

  public get isWithdrawAgain(): boolean {
    return this.item.myWithdrawalStatus != null;
  }

  public get isChangeClassAgain(): boolean {
    return this.item.myClassRunChangeStatus != null;
  }

  public get applyClassRunStateText(): string {
    switch (this.applyClassRunState) {
      case 'Nominated':
        return 'Nominated';
      case 'ChangedTo':
        return 'Changed class run';
      case 'Applied':
        return 'Applied';
      case 'Completed':
        return 'Completed';
      case 'Incomplete':
        return 'Incomplete';
      case 'Failed':
        return 'Failed';
      case 'Apply':
        return 'Apply';
      default:
        return '';
    }
  }

  /**
   * when user has withdrawn or has been rejected
   */
  public get showApplyAgain(): boolean {
    return (
      this.classRunDataService.rejectedClassRunDict[this.item.id] != null ||
      this.classRunDataService.withdrawnClassRunDict[this.item.id] != null
    );
  }

  public get showWithdrawalReason(): boolean {
    return (
      this.item.myWithdrawalStatus != null ||
      (this.item.myRegistrationStatus == null && this.classRunDataService.withdrawnClassRunDict[this.item.id] != null)
    );
  }

  public get checkinPermissionKey(): string {
    return LEARNER_PERMISSIONS.Action_Checkin_DoAssignment_DownloadContent_DoPostCourse;
  }

  public onWithdrawalReasonClicked(): void {
    const registrationId = this.item.registrationId || this.classRunDataService.withdrawnClassRunDict[this.item.id].registrationId;
    this.opalDialogService.openDialogRef(LearnerWithdrawalReasonDialog, { registrationId });
  }

  public getVenue(session: Session): string {
    if (session.isLearningOnline()) {
      return 'Online Session';
    }

    return session.venue;
  }

  public onJoinWebinarClicked(sessionId: string): void {
    this.webinarApiService.getJoinURL(sessionId, BookingSource.Course).then(result => {
      if (result.isSuccess) {
        window.open(result.joinUrl);
      }
    });
  }

  public canJoinWebinar(session: Session): boolean {
    return (
      session.isLearningOnline() &&
      (new Date() >= DateUtils.addMinutes(session.startDateTime, -30) && new Date() <= DateUtils.addMinutes(session.endDateTime, 30))
    );
  }

  public canShowExpiredWebinar(sessionEndDateTime: Date): boolean {
    return new Date() > DateUtils.addMinutes(sessionEndDateTime, 30);
  }

  public canShowWebinar(session: Session): boolean {
    return this.item.isParticipant && session.isLearningOnline();
  }

  public openCslWindow(): void {
    let url = '';
    if (this.item.isNotUnpublished) {
      url = '/csl/s/' + this.item.classRunCode;
    } else {
      url = '/csl/s/class-' + this.item.id;
    }
    window.open(url, '_blank');
  }

  private sortSessions(): void {
    if (!this.item.sessions || !this.item.sessions.length) {
      return;
    }
    this.item.sessions = orderBy(this.item.sessions, session => session.startDateTime);
  }

  private getAttendanceTracking(): void {
    this.hasAttendanceTracking =
      (this.item.myRegistrationStatus === MyRegistrationStatus.ConfirmedByCA ||
        this.item.myRegistrationStatus === MyRegistrationStatus.OfferConfirmed) &&
      this.item.sessions &&
      this.item.sessions.length > 0;
    // If class run has any sessions => check attendance checking. Otherwise, mark completed all attendance trackingg
    if (this.hasAttendanceTracking) {
      this.attendanceTrackingService.getUserAttendanceTrackingByClassRunId(this.item.id).then(response => {
        if (response && response.length) {
          this.attendanceTrackingDict = Utils.toDictionary(response, p => p.sessionId);
          this.attendanceTrackings = response;
          this.markCompletedAllAttendances();
        }
        // attendanceTrackingDict is null when LMM still don't init the session code
        // so need to check both cases
        this.upcomingSession = this.item.sessions.find(
          s =>
            (this.attendanceTrackingDict[s.id] == null ||
              (this.attendanceTrackingDict[s.id].status == null && this.attendanceTrackingDict[s.id].isCodeScanned === false)) &&
            moment().isSameOrBefore(s.startDateTime, 'day')
        );
        if (this.upcomingSession) {
          this.isExpanding = true;
          this.expandingSessions[this.upcomingSession.id] = true;
          this.classRunDataService.upcomingSession.next(this.upcomingSession);
        }
      });
    } else {
      this.markCompletedAllAttendances();
    }
  }

  private listenCheckCompleted(): void {
    this.classRunDataService.updateAttendanceTracking.pipe(this.untilDestroy()).subscribe(session => {
      this.updateSessionStatus(session);
      this.markCompletedAllAttendances();
    });
  }

  private markCompletedAllAttendances(): void {
    if (this.completedAttendanceTracking || !this.course.myClassRun || this.course.myClassRun.registrationId !== this.item.registrationId) {
      return;
    }

    if (!this.item.sessions.length) {
      this.completedAttendanceTracking = true;
      this.classRunDataService.completedAttendanceTrackingSubject.next(this.completedAttendanceTracking);
      return;
    }
  }

  private updateSessionStatus(attendanceTracking: AttendanceTracking): void {
    this.attendanceTrackingDict[attendanceTracking.sessionId] = attendanceTracking;
    const index = this.attendanceTrackings.findIndex(p => p.id === attendanceTracking.id);
    if (index > -1) {
      this.attendanceTrackings[index] = attendanceTracking;
    } else {
      this.attendanceTrackings.push(attendanceTracking);
    }
  }
}
