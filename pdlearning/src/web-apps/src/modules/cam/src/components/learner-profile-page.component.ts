import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BreadcrumbItem,
  BreadcrumbService,
  CAMRoutePaths,
  CAMTabConfiguration,
  COURSE_REGISTRATION_STATUS_PREFIX_MAP,
  CommentDialogComponent,
  ContextMenuAction,
  IDialogActionEvent,
  LearnerProfileViewModel,
  LearnerProfileVmService,
  NavigationData,
  NavigationPageService,
  REGISTRATION_CHANGE_CLASSRUN_STATUS_COLOR_MAP,
  REGISTRATION_STATUS_ADDING_PARTICIPANTS_COLOR_MAP,
  REGISTRATION_STATUS_COLOR_MAP,
  RouterPageInput,
  WITHDRAWAL_STATUS_COLOR_MAP
} from '@opal20/domain-components';
import { ButtonAction, ButtonGroupButton, DialogAction, OpalDialogService, SPACING_CONTENT } from '@opal20/common-components';
import {
  ClassRun,
  ClassRunApiService,
  ClassRunChangeStatus,
  ClassRunRepository,
  Course,
  CoursePlanningCycle,
  CoursePlanningCycleRepository,
  CourseRepository,
  IChangeRegistrationChangeClassRunStatusRequest,
  IChangeRegistrationStatusRequest,
  IChangeRegistrationWithdrawalStatusRequest,
  Registration,
  RegistrationLearningStatus,
  RegistrationRepository,
  RegistrationStatus,
  SearchRegistrationsType,
  UserInfoModel,
  WithdrawalStatus
} from '@opal20/domain-api';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import { map, switchMap } from 'rxjs/operators';

import { CAM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/cam-route-breadcumb-mapping-fn';
import { ChangeRegistrationClassDialogComponent } from './dialogs/change-registration-class-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { LearnerProfilePageInput } from '../models/learner-profile-page-input.model';
import { NAVIGATORS } from '../cam.config';
import { SearchLearnerProfileType } from '../models/search-learner-profile-type.model';

@Component({
  selector: 'learner-profile-page',
  templateUrl: './learner-profile-page.component.html'
})
export class LearnerProfilePageComponent extends BasePageComponent {
  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;
  public registration: Registration = new Registration();
  public classRun: ClassRun = new ClassRun();
  public course: Course = new Course();
  public breadCrumbItems: BreadcrumbItem[] = [];
  public actionBtnGroup: ButtonAction<unknown>[] = [];

  public get title(): string {
    return this.learnerVm.user.fullName;
  }

  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Confirm',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.Confirm),
      shownIfFn: () => this.showConfirmRejectButton()
    },
    {
      displayText: 'Reject',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.Reject),
      shownIfFn: () => this.showConfirmRejectButton()
    },
    {
      displayText: 'Move to Waitlist',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.MoveToWaitlist),
      shownIfFn: () => this.showMoveToWaitlistButton()
    },
    {
      displayText: 'Send Offer',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.SendOffer),
      shownIfFn: () => this.showSendOfferButton()
    },
    {
      displayText: 'Withdraw',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.WithdrawManuallyByCA),
      shownIfFn: () => this.showWithdrawOrChangeClassButton('withdraw')
    },
    {
      displayText: 'Change Class',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.ChangeRegistrationClass),
      shownIfFn: () => this.showWithdrawOrChangeClassButton('changeClass')
    }
  ];

  public get detailPageInput(): RouterPageInput<LearnerProfilePageInput, CAMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<LearnerProfilePageInput, CAMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadUserInfo();
      }
    }
  }

  public get learnerVm(): LearnerProfileViewModel {
    return this._learnerVm;
  }
  public set learnerVm(v: LearnerProfileViewModel) {
    this._learnerVm = v;
  }

  public get statusColorMap(): unknown {
    if (this.registration.isExpiredOrNeedToSetExpired(this.course, this.classRun)) {
      return REGISTRATION_STATUS_COLOR_MAP;
    } else if (this.detailPageInput.data.searchType === SearchLearnerProfileType.Withdrawal) {
      return WITHDRAWAL_STATUS_COLOR_MAP;
    } else if (this.detailPageInput.data.searchType === SearchLearnerProfileType.AddingParticipant) {
      return REGISTRATION_STATUS_ADDING_PARTICIPANTS_COLOR_MAP;
    } else if (this.detailPageInput.data.searchType === SearchLearnerProfileType.ChangeClassRun) {
      return REGISTRATION_CHANGE_CLASSRUN_STATUS_COLOR_MAP;
    }
    return REGISTRATION_STATUS_COLOR_MAP;
  }

  public navigationData: NavigationData;
  public loadingData: boolean = false;
  public registrationStatusChanged: boolean = false;
  public stickySpacing: number = SPACING_CONTENT;
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  public ContextMenuAction: typeof ContextMenuAction = ContextMenuAction;
  private _detailPageInput: RouterPageInput<LearnerProfilePageInput, CAMTabConfiguration, unknown> = NAVIGATORS[
    CAMRoutePaths.LearnerProfilePage
  ] as RouterPageInput<LearnerProfilePageInput, CAMTabConfiguration, unknown>;
  private _loadUserInfoSub: Subscription = new Subscription();
  private _learnerVm: LearnerProfileViewModel = new LearnerProfileViewModel();
  private currentUser = UserInfoModel.getMyUserInfo();
  private coursePlanningCycle: CoursePlanningCycle = new CoursePlanningCycle();
  public get selectedTab(): CAMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : CAMTabConfiguration.PersonalInfoTab;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private registrationRepository: RegistrationRepository,
    private classRunRepository: ClassRunRepository,
    private learnerProfileVmService: LearnerProfileVmService,
    private navigationPageService: NavigationPageService,
    private courseRepository: CourseRepository,
    private breadcrumbService: BreadcrumbService,
    private opalDialogService: OpalDialogService,
    private classRunApiService: ClassRunApiService,
    private coursePlanningCycleRepository: CoursePlanningCycleRepository
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = learnerProfilePageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack();
  }

  public loadUserInfo(): void {
    this._loadUserInfoSub.unsubscribe();
    const registrationObs: Observable<Registration | null> =
      this.detailPageInput.data.registrationId != null
        ? this.registrationRepository.getRegistrationById(this.detailPageInput.data.registrationId)
        : of(null);
    const courseObs: Observable<Course | null> =
      this.detailPageInput.data.courseId != null ? this.courseRepository.loadCourse(this.detailPageInput.data.courseId) : of(null);
    const classRunObs: Observable<ClassRun | null> =
      this.detailPageInput.data.classRunId != null
        ? this.classRunRepository.loadClassRunById(this.detailPageInput.data.classRunId)
        : of(null);
    const userObs =
      this.detailPageInput.data.userId != null
        ? this.learnerProfileVmService.loadLearnerProfile(this.detailPageInput.data.userId)
        : of(null);
    this.loadingData = true;
    this._loadUserInfoSub = combineLatest(registrationObs, courseObs, classRunObs, userObs)
      .pipe(
        switchMap(([registration, course, classRun, user]) => {
          const coursePlanningCycleObs = course.coursePlanningCycleId
            ? this.coursePlanningCycleRepository.loadCoursePlanningCycleById(course.coursePlanningCycleId)
            : of(null);
          return coursePlanningCycleObs.pipe(
            map(
              coursePlanningCycle =>
                <[Registration, Course, ClassRun, LearnerProfileViewModel, CoursePlanningCycle]>[
                  registration,
                  course,
                  classRun,
                  user,
                  coursePlanningCycle
                ]
            )
          );
        }),
        this.untilDestroy()
      )
      .subscribe(
        ([registration, course, classRun, user, coursePlanningCycle]) => {
          this.registration = registration;
          this.course = course;
          this.classRun = classRun;
          if (user) {
            this.learnerVm = user;
          }
          this.coursePlanningCycle = coursePlanningCycle;
          this.loadBreadcrumb();
          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public showConfirmRejectButton(): boolean {
    return (
      ((this.detailPageInput.data.searchType === SearchLearnerProfileType.ClassRunRegistration &&
        this.registration.canActionOnRegistrationList(this.classRun, this.course) &&
        Registration.hasActionOnRegistrationListPermission(this.currentUser, this.course)) ||
        (this.detailPageInput.data.searchType === SearchLearnerProfileType.Withdrawal &&
          this.registration.canActionOnWithdrawalRequestList(this.classRun, this.course) &&
          Registration.hasActionOnRegistrationListPermission(this.currentUser, this.course)) ||
        (this.detailPageInput.data.searchType === SearchLearnerProfileType.ChangeClassRun &&
          this.registration.canActionOnChangeClassRequestList(this.classRun, this.course) &&
          Registration.hasActionOnRegistrationListPermission(this.currentUser, this.course))) &&
      this.selectedTab === CAMTabConfiguration.PersonalInfoTab
    );
  }

  public showMoveToWaitlistButton(): boolean {
    return (
      this.detailPageInput.data.searchType === SearchLearnerProfileType.ClassRunRegistration &&
      this.selectedTab === CAMTabConfiguration.PersonalInfoTab &&
      this.registration.canActionOnRegistrationList(this.classRun, this.course) &&
      Registration.hasActionOnRegistrationListPermission(this.currentUser, this.course)
    );
  }

  public showSendOfferButton(): boolean {
    return (
      this.detailPageInput.data.searchType === SearchLearnerProfileType.Waitlist &&
      this.registration.canSendOffer(this.currentUser, this.classRun, this.course) &&
      Registration.hasSendOfferPermission(this.currentUser, this.course) &&
      this.selectedTab === CAMTabConfiguration.PersonalInfoTab
    );
  }

  public showWithdrawOrChangeClassButton(option: 'withdraw' | 'changeClass'): boolean {
    const generalCondition =
      ((this.detailPageInput.data.searchType === SearchLearnerProfileType.ClassRunRegistration &&
        this.registration.canActionOnRegistrationList(this.classRun, this.course) &&
        Registration.hasActionOnRegistrationListPermission(this.currentUser, this.course)) ||
        (this.detailPageInput.data.searchType === SearchLearnerProfileType.Participant &&
          Registration.canActionOnParticipantList(this.classRun, this.course) &&
          Registration.hasActionOnParticipantListPermission(this.currentUser, this.course)) ||
        (this.detailPageInput.data.searchType === SearchLearnerProfileType.Waitlist &&
          Registration.canActionOnWaitlist(this.classRun, this.course) &&
          Registration.hasActionOnWaitlistPermission(this.currentUser, this.course))) &&
      this.selectedTab === CAMTabConfiguration.PersonalInfoTab;
    if (option === 'withdraw') {
      return generalCondition && this.canWithdrawRegistrationByCA();
    }
    return generalCondition && this.canChangeRegistrationClassByCA();
  }

  public canWithdrawRegistrationByCA(): boolean {
    return (
      this.registration.canWithdrawByCA(this.course) &&
      !this.classRun.started() &&
      this.course.hasAdministrationPermission(this.currentUser) &&
      !this.course.isArchived()
    );
  }

  public canChangeRegistrationClassByCA(): boolean {
    return (
      this.registration.canChangeClassByCA(this.course) &&
      !this.classRun.started() &&
      this.course.hasAdministrationPermission(this.currentUser) &&
      !this.course.isArchived()
    );
  }

  public displayStatus(): RegistrationStatus | WithdrawalStatus | ClassRunChangeStatus | RegistrationLearningStatus {
    if (this.registration.isExpiredOrNeedToSetExpired(this.course, this.classRun)) {
      return RegistrationStatus.Expired;
    } else if (this.detailPageInput.data.searchType === SearchLearnerProfileType.Withdrawal) {
      return this.registration.withdrawalStatus;
    } else if (this.detailPageInput.data.searchType === SearchLearnerProfileType.ChangeClassRun) {
      return this.registration.classRunChangeStatus;
    }
    return this.registration.status;
  }

  public displayPrefixStatus(): string {
    if (
      this.detailPageInput.data.searchType === SearchLearnerProfileType.Withdrawal ||
      this.detailPageInput.data.searchType === SearchLearnerProfileType.AddingParticipant ||
      this.detailPageInput.data.searchType === SearchLearnerProfileType.ChangeClassRun
    ) {
      return '';
    } else if (
      this.registration.isExpiredOrNeedToSetExpired(this.course, this.classRun) ||
      (this.registration.status !== RegistrationStatus.WaitlistPendingApprovalByLearner &&
        this.registration.status !== RegistrationStatus.WaitlistConfirmed &&
        this.registration.status !== RegistrationStatus.OfferPendingApprovalByLearner)
    ) {
      return COURSE_REGISTRATION_STATUS_PREFIX_MAP[this.registration.registrationType];
    }
  }

  public onActionButtonGroups(action: ContextMenuAction): void {
    switch (this.detailPageInput.data.searchType) {
      case SearchLearnerProfileType.ClassRunRegistration:
        this.onRegistrationAction(action);
        break;
      case SearchLearnerProfileType.Withdrawal:
        this.onWithdrawAction(action);
        break;
      case SearchLearnerProfileType.Waitlist:
        this.onWaitlistAction(action);
        break;
      case SearchLearnerProfileType.ChangeClassRun:
        this.onActionChangeClassRun(action);
        break;
      case SearchLearnerProfileType.Participant:
        this.onParticipantAction(action);
        break;
      default:
        break;
    }
  }

  public onRegistrationAction(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.Confirm:
        this.showApprovalDialog(
          {
            title: `${this.translate('Confirm Registration')}:`,
            requiredCommentField: false
          },
          this.registration,
          RegistrationStatus.ConfirmedByCA
        );
        break;
      case ContextMenuAction.Reject:
        this.showApprovalDialog(
          {
            title: `${this.translate('Reject Registration')}:`
          },
          this.registration,
          RegistrationStatus.RejectedByCA
        );
        break;
      case ContextMenuAction.MoveToWaitlist:
        this.showApprovalDialog(
          {
            title: `${this.translate('Move to Waitlist')}:`
          },
          this.registration,
          RegistrationStatus.WaitlistPendingApprovalByLearner
        );
        break;
      case ContextMenuAction.ChangeRegistrationClass:
        this.showChangeRegistrationClassDialog(this.registration);
        break;
      case ContextMenuAction.WithdrawManuallyByCA:
        this.showApprovalWithdrawDialog(null, this.registration, WithdrawalStatus.Withdrawn, false);
      default:
        break;
    }
  }

  public onWithdrawAction(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.Confirm:
        this.showApprovalWithdrawDialog(
          {
            title: `${this.translate('Confirm Withdrawal')}:`,
            requiredCommentField: false
          },
          this.registration,
          WithdrawalStatus.Withdrawn,
          true
        );
        break;
      case ContextMenuAction.Reject:
        this.showApprovalWithdrawDialog(
          {
            title: `${this.translate('Reject Withdrawal')}:`
          },
          this.registration,
          WithdrawalStatus.RejectedByCA
        );
        break;
      default:
        break;
    }
  }

  public onActionChangeClassRun(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.Confirm:
        this.showApprovalChangeClassDialog(
          {
            title: `${this.translate('Confirm Change Class')}:`,
            requiredCommentField: false
          },
          this.registration,
          ClassRunChangeStatus.ConfirmedByCA
        );
        break;
      case ContextMenuAction.Reject:
        this.showApprovalChangeClassDialog(
          {
            title: `${this.translate('Reject Change Class')}:`
          },
          this.registration,
          ClassRunChangeStatus.RejectedByCA
        );
        break;
      default:
        break;
    }
  }

  public onWaitlistAction(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.SendOffer:
        this.classRunApiService.checkClassIsFull(this.registration.classRunId).then(_ => {
          if (!_) {
            this.changeRegistrationStatus(this.registration, RegistrationStatus.OfferPendingApprovalByLearner);
          } else {
            this.opalDialogService.openConfirmDialog({
              confirmTitle: 'Warning',
              confirmMsg: 'The class is currently full.',
              hideNoBtn: true,
              yesBtnText: 'Close'
            });
          }
        });
        break;
      case ContextMenuAction.ChangeRegistrationClass:
        this.showChangeRegistrationClassDialog(this.registration);
        break;
      case ContextMenuAction.WithdrawManuallyByCA:
        this.showApprovalWithdrawDialog(null, this.registration, WithdrawalStatus.Withdrawn, false);
      default:
        break;
    }
  }

  public onParticipantAction(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.ChangeRegistrationClass:
        this.showChangeRegistrationClassDialog(this.registration);
        break;
      case ContextMenuAction.WithdrawManuallyByCA:
        this.showApprovalWithdrawDialog(null, this.registration, WithdrawalStatus.Withdrawn, false);
      default:
        break;
    }
  }

  public showApprovalDialog(input: unknown, registration: Registration, registrationStatus: RegistrationStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    this.subscribe(dialogRef.result, (data: IDialogActionEvent) => {
      if (data.action === DialogAction.OK) {
        this.changeRegistrationStatus(registration, registrationStatus, data.comment);
      }
    });
  }

  public showApprovalWithdrawDialog(
    input: unknown,
    registration: Registration,
    withdrawalStatus: WithdrawalStatus,
    showCommentField: boolean = true
  ): void {
    if (showCommentField) {
      const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
      this.subscribe(dialogRef.result, (data: IDialogActionEvent) => {
        if (data.action === DialogAction.OK) {
          this.changeRegistrationWithdrawStatus(registration, withdrawalStatus, data.comment);
        }
      });
    } else {
      const confirmDialog = this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Withdraw',
        confirmMsg: 'Do you want to withdraw this learner from the current class?',
        yesBtnText: 'Proceed'
      });
      this.subscribe(confirmDialog, action => {
        if (action === DialogAction.OK) {
          return this.changeRegistrationWithdrawStatus(registration, withdrawalStatus, '');
        }
        return of(false);
      });
    }
  }

  public showChangeRegistrationClassDialog(registration: Registration): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
      ChangeRegistrationClassDialogComponent,
      {
        courseId: this.course.id,
        currentClassId: this.classRun.id,
        registrationIds: [registration.id]
      },
      ChangeRegistrationClassDialogComponent.defaultDialogSettings
    );
    dialogRef.result.pipe(this.untilDestroy()).subscribe(data => {
      if (data === DialogAction.OK) {
        this.showNotification(this.translate('Your request has been process successfully.'));
        this.navigationPageService.navigateBack();
      }
    });
  }

  public showApprovalChangeClassDialog(input: unknown, registration: Registration, classRunChangeStatus: ClassRunChangeStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    this.subscribe(dialogRef.result, (data: IDialogActionEvent) => {
      if (data.action === DialogAction.OK) {
        this.changeRegistrationChangeClassRunStatus(registration, classRunChangeStatus, data.comment);
      }
    });
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadUserInfo();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<LearnerProfilePageInput, CAMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private changeRegistrationStatus(registration: Registration, status: RegistrationStatus, comment: string = ''): void {
    const registrationChangeStatus: IChangeRegistrationStatusRequest = {
      ids: [registration.id],
      status: status,
      comment: comment
    };
    this.registrationRepository.changeRegistrationStatus(registrationChangeStatus).then(() => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  private changeRegistrationWithdrawStatus(registration: Registration, withdrawalStatus: WithdrawalStatus, comment: string = ''): void {
    const request: IChangeRegistrationWithdrawalStatusRequest = {
      ids: [registration.id],
      withdrawalStatus: withdrawalStatus,
      comment: comment,
      isManual: false
    };
    this.registrationRepository.changeRegistrationWithdrawalStatus(request).then(() => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  private changeRegistrationChangeClassRunStatus(
    registration: Registration,
    classRunChangeStatus: ClassRunChangeStatus,
    comment: string = ''
  ): void {
    const request: IChangeRegistrationChangeClassRunStatusRequest = {
      ids: [registration.id],
      classRunChangeStatus: classRunChangeStatus,
      comment: comment
    };
    this.registrationRepository.changeRegistrationClassRunChangeStatus(request).then(() => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      CAM_ROUTE_BREADCUMB_MAPPING_FN(this.detailPageInput, p => this.navigationPageService.navigateByRouter(p), {
        [CAMRoutePaths.CoursePlanningCycleDetailPage]: {
          textFn: () => (this.coursePlanningCycle != null ? this.coursePlanningCycle.title : '')
        },
        [CAMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName },
        [CAMRoutePaths.ClassRunDetailPage]: { textFn: () => this.classRun.classTitle },
        [CAMRoutePaths.LearnerProfilePage]: { textFn: () => this.learnerVm.user.fullName }
      })
    );
  }
}

export const learnerProfilePageTabIndexMap = {
  0: CAMTabConfiguration.PersonalInfoTab
};
