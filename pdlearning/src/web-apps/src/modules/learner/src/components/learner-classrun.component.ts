import * as moment from 'moment';

import {
  ApprovalGroupType,
  ClassRunChangeStatus,
  LearningStatus,
  MyRegistrationStatus,
  RegistrationApiService,
  RegistrationType,
  UserInfoModel,
  WithdrawalStatus
} from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input, TemplateRef, ViewChild } from '@angular/core';
import { DialogAction, OpalDialogService } from '@opal20/common-components';
import { LEARNER_PERMISSIONS, ListClassRunGridComponentService } from '@opal20/domain-components';

import { APPLIED_REGISTRATION_STATUSES } from '../constants/learning-card-status.constant';
import { ClassRunDataService } from '../services/class-run-data.service';
import { CourseClassRun } from '../models/class-run-item.model';
import { CourseModel } from '../models/course.model';
import { Observable } from 'rxjs';

export type ApplyClassRunStateType = 'Nominated' | 'ChangedTo' | 'Applied' | 'Completed' | 'Failed' | 'Apply' | 'Incomplete';
@Component({
  selector: 'learner-classrun',
  templateUrl: './learner-classrun.component.html'
})
export class ClassRunComponent extends BaseComponent {
  @ViewChild('confirmationWithdrawalInput', { static: false })
  public confirmationWithdrawalTemplate: TemplateRef<unknown>;
  @ViewChild('confirmationClassrunInput', { static: false })
  public confirmationClassrunTemplate: TemplateRef<unknown>;

  @Input() public course: CourseModel;
  @Input() public readyToLearn: boolean = false;
  @Input() public addCourseToPlanCallback: () => Promise<void>;
  public approvingOfficerId: string;
  public alternativeApprovingOfficerId: string;
  public confirmationReason: string = '';
  public classFull: boolean = false;
  constructor(
    public listClassRunGridComponentService: ListClassRunGridComponentService,
    protected moduleFacadeService: ModuleFacadeService,
    private classRunDataService: ClassRunDataService,
    private registrationApiService: RegistrationApiService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.getApprovingOfficers();
  }

  public onClassRunClicked(classRun: CourseClassRun): void {
    this.registrationApiService.getLearnerCourseViolation(this.course.courseId, classRun.id).then(p => {
      const isViolationPreRequisiteCourse = p.violationDetail && p.violationDetail.getAllViolationPreRequisiteCourseIds().length > 0;
      if (isViolationPreRequisiteCourse) {
        this.showViolationPrerequisiteCourseMessage();
        return;
      }

      this.showConfirmApplyClassRun(classRun);
    });
  }

  public async onWithdrawClicked(classRun: CourseClassRun): Promise<void> {
    if (!this.validateWithdraw(classRun)) {
      this.opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Warning',
          confirmMsg: 'The system do not allow withdraw class run before start date',
          hideNoBtn: true,
          yesBtnText: 'OK'
        })
        .subscribe();
      return;
    }

    this.showWithdrawDialog(classRun);
  }

  public async onChangeClassRunClicked(classRun: CourseClassRun): Promise<void> {
    if (!this.validateChangeClassRun(classRun)) {
      this.opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Warning',
          confirmMsg: 'The system do not allow change class run before start date',
          hideNoBtn: true,
          yesBtnText: 'OK'
        })
        .subscribe();
      return;
    }

    this.showChangeClassRunDialog(classRun);
  }

  /** @returns null when user didn't apply any class */
  public getApplyClassRunState(currentClassRunItem: CourseClassRun): ApplyClassRunStateType {
    if (currentClassRunItem.isClassRunFinished) {
      if (currentClassRunItem.learningStatus === LearningStatus.Completed) {
        return 'Completed';
      }

      if (currentClassRunItem.learningStatus === LearningStatus.Failed) {
        return 'Incomplete';
      }
    }

    if (this.currentClassRun == null) {
      return null;
    }

    if (this.currentClassRun.isChangingClass && this.currentClassRun.classRunChangeId === currentClassRunItem.id) {
      return 'ChangedTo';
    }

    if (this.currentClassRun.id === currentClassRunItem.id) {
      return currentClassRunItem.isNominated ? 'Nominated' : 'Applied';
    }

    if (this.classRunDataService.isPrivateCourse) {
      return null;
    }

    // when learner didn't apply the current class
    return 'Apply';
  }

  public showApply(currentClassRunItem: CourseClassRun): boolean {
    return (
      (this.currentClassRun == null || this.currentClassRun.isClassRunFinished) &&
      !this.classRunDataService.isPrivateCourse &&
      !this.classRunDataService.hasReachedMaxRelearningTimes() &&
      !currentClassRunItem.isClassRunFinished &&
      !this.classRunDataService.isUnPublishedCourse &&
      !this.classRunDataService.isExpiredCourse &&
      !this.classRunDataService.isArchivedCourse
    );
  }

  public showChangeClassRun(currentClassRunItem: CourseClassRun): boolean {
    return (
      // cannot change to a class that has already applied.
      currentClassRunItem.myRegistrationStatus == null &&
      this.canChangeOrWithdraw &&
      this.currentClassRun.registrationType === RegistrationType.Manual &&
      this.currentClassRun.id !== currentClassRunItem.id &&
      !this.currentClassRun.isChangingClass
    );
  }

  public showWithdraw(currentClassRunItem: CourseClassRun): boolean {
    return this.canChangeOrWithdraw && this.currentClassRun.id === currentClassRunItem.id;
  }

  public isLessThan10DaysFromStartDate(): boolean {
    return moment(new Date())
      .add(10, 'days')
      .isAfter(this.currentClassRun.startDate, 'day');
  }

  public get currentClassRun(): CourseClassRun {
    return this.classRunDataService.currentClassRun;
  }

  public get courseClassRunItems(): CourseClassRun[] {
    return this.classRunDataService.courseClassRunItems;
  }

  public get hasClassRunPermission(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_ClassRun);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }

  private get canChangeOrWithdraw(): boolean {
    return (
      this.currentClassRun != null &&
      this.currentClassRun.isStarted === false &&
      this.currentClassRun.myRegistrationStatus !== MyRegistrationStatus.PendingConfirmation &&
      this.currentClassRun.learningStatus === LearningStatus.NotStarted &&
      !this.isWithdrawalInProgress(this.currentClassRun.myWithdrawalStatus) &&
      !this.isChangingClassInProgress(this.currentClassRun.myClassRunChangeStatus)
    );
  }

  private showChangeClassRunDialog(classRun: CourseClassRun): void {
    this.classFull = classRun.maxClassSize === classRun.totalParticipant ? true : false;
    this.confirmationReason = '';
    this.opalDialogService
      .openConfirmDialog(
        {
          confirmTitle: 'Change class run',
          confirmMsg: '',
          yesBtnText: 'Submit',
          noBtnText: 'Close',
          bodyTemplate: this.confirmationClassrunTemplate,
          disableYesBtnFn: () => Utils.isEmpty(this.confirmationReason)
        },
        { maxWidth: '494px' }
      )
      .subscribe(confirm => {
        if (confirm === DialogAction.OK) {
          this.changeClass(classRun);
        }
      });
  }

  private showWithdrawDialog(classRun: CourseClassRun): void {
    this.confirmationReason = '';
    this.opalDialogService
      .openConfirmDialog(
        {
          confirmTitle: 'Withdraw',
          confirmMsg: '',
          yesBtnText: 'Submit',
          noBtnText: 'Close',
          bodyTemplate: this.confirmationWithdrawalTemplate,
          disableYesBtnFn: () => Utils.isEmpty(this.confirmationReason)
        },
        { maxWidth: '494px' }
      )
      .subscribe(action => {
        if (action === DialogAction.OK) {
          this.withdraw(classRun);
        }
      });
  }

  private changeClass(classRun: CourseClassRun): void {
    this.classRunDataService.changeClassRun(classRun, this.confirmationReason);
  }

  private withdraw(classRun: CourseClassRun): void {
    this.classRunDataService.withdrawClassRun(this.confirmationReason);
  }

  private validateWithdraw(classRun: CourseClassRun): boolean {
    return classRun.isValidDateToChangeClassAndWithdraw;
  }

  private validateChangeClassRun(classRun: CourseClassRun): boolean {
    return this.currentClassRun.isValidDateToChangeClassAndWithdraw && classRun.isValidDateToChangeClassAndWithdraw;
  }

  private onApplyingClassRun(classRun: CourseClassRun): void {
    if (!APPLIED_REGISTRATION_STATUSES.includes(classRun.myRegistrationStatus)) {
      this.addToPlanAndApplyClassRun(classRun);
      return;
    }
  }

  private addToPlanAndApplyClassRun(classRun: CourseClassRun): void {
    if (!this.addCourseToPlanCallback) {
      return;
    }

    // Don't add to plan if the current user doesn't have approvingOfficer and alternativeApprovingOfficer.
    if (!this.approvingOfficerId && !this.alternativeApprovingOfficerId) {
      // Still call registerClassRun method to show an error message if any.
      this.registerClassRun(classRun);
      return;
    }

    this.addCourseToPlanCallback().then(() => this.registerClassRun(classRun));
  }

  private registerClassRun(classRun: CourseClassRun): void {
    this.classRunDataService.applyClassRun(classRun, this.approvingOfficerId, this.alternativeApprovingOfficerId).then(() => {
      this.onRegisteredSuccess().subscribe();
    });
  }

  private getApprovingOfficers(): void {
    const officers = UserInfoModel.getMyUserInfo().groups;
    const approvingOfficer = officers.find(o => o.type === ApprovalGroupType.PrimaryApprovalGroup);
    const alternativeApprovingOfficer = officers.find(o => o.type === ApprovalGroupType.AlternativeApprovalGroup);
    this.approvingOfficerId = approvingOfficer ? approvingOfficer.userIdentity.extId : null;
    this.alternativeApprovingOfficerId = alternativeApprovingOfficer ? alternativeApprovingOfficer.userIdentity.extId : null;
  }

  private onRegisteredSuccess(): Observable<DialogAction> {
    return this.opalDialogService.openConfirmDialog({
      confirmTitle: 'Successful',
      confirmMsg: 'Your application has been submitted successfully to your approving officer for approval.',
      hideNoBtn: true,
      yesBtnText: 'Ok'
    });
  }

  private showViolationPrerequisiteCourseMessage(): Observable<DialogAction> {
    return this.opalDialogService.openConfirmDialog({
      confirmTitle: 'Warning',
      confirmMsg: MISS_PREREQUISITE_COURSE_MESSAGE,
      hideNoBtn: true,
      yesBtnText: 'OK'
    });
  }

  private showConfirmApplyClassRun(classRun: CourseClassRun): void {
    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Confirmation',
        confirmMsg: 'Your application will be sent to your approving officer.',
        yesBtnText: 'Proceed',
        noBtnText: 'Cancel'
      })
      .subscribe(action => {
        if (action === DialogAction.OK) {
          this.onApplyingClassRun(classRun);
        }
      });
  }

  private isWithdrawalInProgress(withdrawalStatus: WithdrawalStatus): boolean {
    return withdrawalStatus === WithdrawalStatus.PendingConfirmation || withdrawalStatus === WithdrawalStatus.Approved;
  }

  private isChangingClassInProgress(changeClassStatus: ClassRunChangeStatus): boolean {
    return changeClassStatus === ClassRunChangeStatus.PendingConfirmation || changeClassStatus === ClassRunChangeStatus.Approved;
  }
}

const MISS_PREREQUISITE_COURSE_MESSAGE = 'Unable to apply for this course because you have not finished the prerequisite course(s).';
