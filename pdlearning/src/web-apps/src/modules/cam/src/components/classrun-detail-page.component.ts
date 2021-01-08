import {
  BaseFormComponent,
  ComponentType,
  DateUtils,
  IFilter,
  IFormBuilderDefinition,
  IGridFilter,
  IntervalScheduler,
  ModuleFacadeService,
  NotificationType,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import {
  BreadcrumbItem,
  BreadcrumbService,
  CAMRoutePaths,
  CAMTabConfiguration,
  CLASSRUN_STATUS_COLOR_MAP,
  ClassRunDetailMode,
  ClassRunDetailViewModel,
  CommentDialogComponent,
  CommentTabInput,
  ContextMenuAction,
  ContextMenuEmit,
  IDialogActionEvent,
  ListRegistrationGridComponentService,
  ListRegistrationGridDisplayColumns,
  NavigationPageService,
  RegistrationFilterComponent,
  RegistrationFilterModel,
  RegistrationViewModel,
  RouterPageInput,
  WebAppLinkBuilder
} from '@opal20/domain-components';
import {
  ButtonAction,
  ButtonGroupButton,
  DialogAction,
  OpalDialogService,
  SPACING_CONTENT,
  futureDateValidator,
  ifValidator,
  requiredAndNoWhitespaceValidator,
  requiredForListValidator,
  requiredNumberValidator,
  startEndValidator,
  validateFutureDateType,
  validateRequiredNumberType
} from '@opal20/common-components';
import {
  COMMENT_ACTION_MAPPING,
  ClassRun,
  ClassRunApiService,
  ClassRunCancellationStatus,
  ClassRunChangeStatus,
  ClassRunRepository,
  ClassRunRescheduleStatus,
  ClassRunStatus,
  CommentApiService,
  CommentServiceType,
  Course,
  CoursePlanningCycle,
  CoursePlanningCycleRepository,
  CourseRepository,
  EntityCommentType,
  IChangeRegistrationChangeClassRunStatusRequest,
  IChangeRegistrationStatusRequest,
  IChangeRegistrationWithdrawalStatusRequest,
  IClassRunCancellationStatusRequest,
  IClassRunRescheduleStatusRequest,
  ISaveClassRunRequest,
  Registration,
  RegistrationRepository,
  RegistrationStatus,
  SearchClassRunType,
  SearchRegistrationsType,
  UserInfoModel,
  UserRepository,
  WithdrawalStatus
} from '@opal20/domain-api';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import { map, switchMap } from 'rxjs/operators';

import { CAM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/cam-route-breadcumb-mapping-fn';
import { CancellationRequestDialogComponent } from './dialogs/cancellation-request-dialog.component';
import { ChangeRegistrationClassDialogComponent } from './dialogs/change-registration-class-dialog.component';
import { ClassRunCancellationInput } from './../models/classrun-cancellation-request-input.model';
import { ClassRunDetailPageInput } from '../models/classrun-detail-page-input.model';
import { ClassRunRescheduleInput } from '../models/classrun-reschedule-request-input.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { NAVIGATORS } from './../cam.config';
import { RescheduleRequestDialogComponent } from './dialogs/reschedule-request-dialog.component';
import { SearchLearnerProfileType } from '../models/search-learner-profile-type.model';
import { SendPlacementLetterDialogComponent } from './dialogs/send-placement-letter-dialog.component';
import { Validators } from '@angular/forms';

@Component({
  selector: 'classrun-detail-page',
  templateUrl: './classrun-detail-page.component.html'
})
export class ClassRunDetailPageComponent extends BaseFormComponent {
  public filterPopupContent: ComponentType<RegistrationFilterComponent> = RegistrationFilterComponent;
  public get commentTabInput(): CommentTabInput {
    return {
      originalObjectId: this.detailPageInput.data.id,
      commentServiceType: CommentServiceType.Course,
      entityCommentType: EntityCommentType.ClassRun,
      mappingAction: COMMENT_ACTION_MAPPING,
      hasReply: true,
      mode: this.course.isArchived ? 'view' : 'edit'
    };
  }

  public get detailPageInput(): RouterPageInput<ClassRunDetailPageInput, CAMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<ClassRunDetailPageInput, CAMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadClassRunInfo();
      }
    }
  }

  public get classRunVm(): ClassRunDetailViewModel {
    return this._classRunVm;
  }
  public set classRunVm(v: ClassRunDetailViewModel) {
    this._classRunVm = v;
  }

  public get title(): string {
    return this.classRunVm.classTitle;
  }

  public get subTitle(): string {
    return this.classRunVm.classRunCode;
  }

  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;

  public course: Course = new Course();
  public registration: Registration = new Registration();
  public searchText: string = '';
  public filterData: RegistrationFilterModel = null;
  public breadCrumbItems: BreadcrumbItem[] = [];
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public stickySpacing: number = SPACING_CONTENT;

  public get hasActionBtnGroup(): boolean {
    return this.actionBtnGroup.findIndex(x => x.actionFn != null && (x.hiddenFn == null || !x.hiddenFn())) > -1;
  }

  public actionBtnGroup: ButtonAction<RegistrationViewModel>[] = [
    {
      id: 'confirm',
      text: this.translateCommon('Confirm'),
      conditionFn: this.confirmOrRejectConditionFn,
      actionFn: dataItems => {
        switch (this.selectedTab) {
          case CAMTabConfiguration.RegistrationsTab:
            return this.handleMassAction(RegistrationMassAction.ConfirmRegistration, dataItems);
          case CAMTabConfiguration.WithdrawalRequestsTab:
            return this.handleMassAction(RegistrationMassAction.ConfirmWithdrawalRequest, dataItems);
          case CAMTabConfiguration.ChangeClassRunRequestsTab:
            return this.handleMassAction(RegistrationMassAction.ConfirmChangeClassRequest, dataItems);
        }
      },
      hiddenFn: () =>
        this.selectedTab !== CAMTabConfiguration.RegistrationsTab &&
        this.selectedTab !== CAMTabConfiguration.WithdrawalRequestsTab &&
        this.selectedTab !== CAMTabConfiguration.ChangeClassRunRequestsTab
    },
    {
      id: 'reject',
      text: this.translateCommon('Reject'),
      conditionFn: this.confirmOrRejectConditionFn,
      actionFn: dataItems => {
        switch (this.selectedTab) {
          case CAMTabConfiguration.RegistrationsTab:
            return this.handleMassAction(RegistrationMassAction.RejectRegistration, dataItems);
          case CAMTabConfiguration.WithdrawalRequestsTab:
            return this.handleMassAction(RegistrationMassAction.RejectWithdrawalRequest, dataItems);
          case CAMTabConfiguration.ChangeClassRunRequestsTab:
            return this.handleMassAction(RegistrationMassAction.RejectChangeClassRequest, dataItems);
        }
      },
      hiddenFn: () =>
        this.selectedTab !== CAMTabConfiguration.RegistrationsTab &&
        this.selectedTab !== CAMTabConfiguration.WithdrawalRequestsTab &&
        this.selectedTab !== CAMTabConfiguration.ChangeClassRunRequestsTab
    },
    {
      id: 'moveToWaitlist',
      text: this.translateCommon('Move to waitlist'),
      conditionFn: dataItem =>
        dataItem.canActionOnRegistrationList(this.classRunVm.data, this.course) &&
        Registration.hasActionOnRegistrationListPermission(this.currentUser, this.course),
      actionFn: dataItems => this.handleMassAction(RegistrationMassAction.MoveToWaitlistRegistration, dataItems),
      hiddenFn: () => this.selectedTab !== CAMTabConfiguration.RegistrationsTab
    },
    {
      id: 'sendOffer',
      text: this.translateCommon('Send Offer'),
      conditionFn: dataItem =>
        dataItem.canSendOffer(this.currentUser, this.classRunVm.data, this.course) &&
        Registration.hasSendOfferPermission(this.currentUser, this.course),
      actionFn: dataItems => this.handleMassAction(RegistrationMassAction.SendOfferWaitlistRegistration, dataItems),
      hiddenFn: () => this.selectedTab !== CAMTabConfiguration.WaitlistTab
    },
    {
      id: 'withdraw',
      text: this.translateCommon('Withdraw'),
      conditionFn: this.withdrawOrChangeClassConditionFn(RegistrationMassAction.WithdrawRegistrationManually),
      actionFn: dataItems => this.handleMassAction(RegistrationMassAction.WithdrawRegistrationManually, dataItems),
      hiddenFn: () =>
        this.selectedTab !== CAMTabConfiguration.RegistrationsTab &&
        this.selectedTab !== CAMTabConfiguration.ParticipantsTab &&
        this.selectedTab !== CAMTabConfiguration.WaitlistTab
    },
    {
      id: 'changeRegistrationsClassRun',
      text: this.translateCommon('Change Class'),
      conditionFn: this.withdrawOrChangeClassConditionFn(RegistrationMassAction.ChangeRegistrationsClassrun),
      actionFn: dataItems => this.handleMassAction(RegistrationMassAction.ChangeRegistrationsClassrun, dataItems),
      hiddenFn: () =>
        this.selectedTab !== CAMTabConfiguration.RegistrationsTab &&
        this.selectedTab !== CAMTabConfiguration.ParticipantsTab &&
        this.selectedTab !== CAMTabConfiguration.WaitlistTab
    }
  ];
  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Edit',
      onClickFn: () => this.onEditClassRun(),
      shownIfFn: () => this.showEditClassRun()
    },
    {
      displayText: 'Save',
      onClickFn: () => this.onSaveClassRun(),
      shownIfFn: () => this.showSubmitClassRun(),
      isDisabledFn: () => !this.dataHasChanged()
    },
    {
      displayText: 'Publish',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Publish),
      shownIfFn: () => this.showPublishClassRun()
    },
    {
      displayText: 'Unpublish',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Unpublish),
      shownIfFn: () => this.showUnpublishClassRun()
    },
    {
      displayText: 'Approve',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Approve),
      shownIfFn: () => this.showApprovalRequest()
    },
    {
      displayText: 'Reject',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Reject),
      shownIfFn: () => this.showApprovalRequest()
    },
    {
      id: ContextMenuAction.CancellationRequest,
      icon: '',
      displayText: 'Cancel Class',
      shownInMoreFn: () => true,
      onClickFn: () => this.onActionApproval(ContextMenuAction.CancellationRequest),
      shownIfFn: () => this.showCancellationRequest()
    },
    {
      id: ContextMenuAction.RescheduleRequest,
      icon: '',
      displayText: 'Reschedule',
      shownInMoreFn: () => true,
      onClickFn: () => this.onActionApproval(ContextMenuAction.RescheduleRequest),
      shownIfFn: () => this.showRescheduleRequest()
    },
    {
      id: ContextMenuAction.ActivateCourseCriteria,
      icon: '',
      displayText: 'Activate Course Criteria',
      shownInMoreFn: () => true,
      onClickFn: () => this.onActionApproval(ContextMenuAction.ActivateCourseCriteria),
      shownIfFn: () => this.showToggleCourseCriteriaActivated()
    },
    {
      id: ContextMenuAction.DeActivateCourseCriteria,
      icon: '',
      displayText: 'Deactivate Course Criteria',
      shownInMoreFn: () => true,
      onClickFn: () => this.onActionApproval(ContextMenuAction.DeActivateCourseCriteria),
      shownIfFn: () => this.showToggleCourseCriteriaDeactivated()
    },
    {
      id: ContextMenuAction.ActivateCourseAutomate,
      icon: '',
      displayText: 'Enable Automate Selection',
      shownInMoreFn: () => true,
      onClickFn: () => this.onActionApproval(ContextMenuAction.ActivateCourseAutomate),
      shownIfFn: () => this.showToggleCourseAutomateActivated()
    },
    {
      id: ContextMenuAction.DeActivateCourseAutomate,
      icon: '',
      displayText: 'Disable Automate Selection',
      shownInMoreFn: () => true,
      onClickFn: () => this.onActionApproval(ContextMenuAction.DeActivateCourseAutomate),
      shownIfFn: () => this.showToggleCourseAutomateDeActivated()
    },
    {
      id: ContextMenuAction.NavigateToCSLCommunity,
      icon: 'hyperlink-open',
      displayText: this.translateCommon('Open Community'),
      shownInMoreFn: () => true,
      onClickFn: () => {
        window.open(WebAppLinkBuilder.buildCSLCommunityDetailForClassUrl(this.classRunVm.data), '_blank');
      },
      shownIfFn: () => this.classRunVm.data.communityCreated()
    }
  ];

  public displayColumnsRegistrations: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.selected,
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.serviceScheme,
    ListRegistrationGridDisplayColumns.teachingLevel,
    ListRegistrationGridDisplayColumns.teachingSubjectJobFamily,
    ListRegistrationGridDisplayColumns.accountType,
    ListRegistrationGridDisplayColumns.designation,
    ListRegistrationGridDisplayColumns.signedUp,
    ListRegistrationGridDisplayColumns.registrationType,
    ListRegistrationGridDisplayColumns.accountType,
    ListRegistrationGridDisplayColumns.status
  ];

  public displayColumnsWaitlist: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.selected,
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.serviceScheme,
    ListRegistrationGridDisplayColumns.designation,
    ListRegistrationGridDisplayColumns.teachingLevel,
    ListRegistrationGridDisplayColumns.teachingSubjectJobFamily,
    ListRegistrationGridDisplayColumns.accountType,
    ListRegistrationGridDisplayColumns.signedUp,
    ListRegistrationGridDisplayColumns.registrationType,
    ListRegistrationGridDisplayColumns.designation,
    ListRegistrationGridDisplayColumns.status,
    ListRegistrationGridDisplayColumns.courseCriteria
  ];

  public displayColumnsParticipants: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.selected,
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.accountType,
    ListRegistrationGridDisplayColumns.designation,
    ListRegistrationGridDisplayColumns.serviceScheme,
    ListRegistrationGridDisplayColumns.teachingLevel,
    ListRegistrationGridDisplayColumns.teachingSubjectJobFamily,
    ListRegistrationGridDisplayColumns.registrationType,
    ListRegistrationGridDisplayColumns.status
  ];

  public displayColumnsWithdrawal: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.selected,
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.designation,
    ListRegistrationGridDisplayColumns.accountType,
    ListRegistrationGridDisplayColumns.serviceScheme,
    ListRegistrationGridDisplayColumns.teachingLevel,
    ListRegistrationGridDisplayColumns.teachingSubjectJobFamily,
    ListRegistrationGridDisplayColumns.requestDate,
    ListRegistrationGridDisplayColumns.reason,
    ListRegistrationGridDisplayColumns.status
  ];

  public displayColumnsChangeClassRun: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.selected,
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.designation,
    ListRegistrationGridDisplayColumns.accountType,
    ListRegistrationGridDisplayColumns.serviceScheme,
    ListRegistrationGridDisplayColumns.teachingLevel,
    ListRegistrationGridDisplayColumns.teachingSubjectJobFamily,
    ListRegistrationGridDisplayColumns.requestDate,
    ListRegistrationGridDisplayColumns.changeTo,
    ListRegistrationGridDisplayColumns.status
  ];

  public ContextMenuAction: typeof ContextMenuAction = ContextMenuAction;

  public ClassRunStatus: typeof ClassRunStatus = ClassRunStatus;

  public classRunStatusColorMap = CLASSRUN_STATUS_COLOR_MAP;
  public loadingData: boolean = false;
  public registrationStatusChanged: boolean = false;
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public SearchLearnerProfileType: typeof SearchLearnerProfileType = SearchLearnerProfileType;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  public registrationSelectedItems: RegistrationViewModel[] = [];
  public participantSelectedItems: RegistrationViewModel[] = [];
  public waitlistSelectedItems: RegistrationViewModel[] = [];
  public withdrawalRequestSelectedItems: RegistrationViewModel[] = [];
  public changeClassRequestSelectedItems: RegistrationViewModel[] = [];
  public hasCoursePlanningAsParentPage: boolean;

  private _detailPageInput: RouterPageInput<ClassRunDetailPageInput, CAMTabConfiguration, unknown> = NAVIGATORS[
    CAMRoutePaths.ClassRunDetailPage
  ] as RouterPageInput<ClassRunDetailPageInput, CAMTabConfiguration, unknown>;
  private _loadClassRunInfoSub: Subscription = new Subscription();
  private _classRunVm: ClassRunDetailViewModel = new ClassRunDetailViewModel();
  private currentUser = UserInfoModel.getMyUserInfo();
  public get selectedTab(): CAMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : CAMTabConfiguration.ClassRunInfoTab;
  }
  private coursePlanningCycle: CoursePlanningCycle = new CoursePlanningCycle();
  // Auto save after 30 minutes
  private scheduler: IntervalScheduler = new IntervalScheduler(600000, () => {
    if (this.dataHasChanged()) {
      this.onSaveClassRun();
    }
  });

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private courseRepository: CourseRepository,
    private classRunRepository: ClassRunRepository,
    private userRepository: UserRepository,
    private navigationPageService: NavigationPageService,
    private registrationRepository: RegistrationRepository,
    private opalDialogService: OpalDialogService,
    private classRunApiService: ClassRunApiService,
    public listRegistrationGridComponentService: ListRegistrationGridComponentService,
    private breadcrumbService: BreadcrumbService,
    private coursePlanningCycleRepository: CoursePlanningCycleRepository,
    private commentApiService: CommentApiService
  ) {
    super(moduleFacadeService);
    this.commentApiService.initApiService(CommentServiceType.Course);
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public withdrawOrChangeClassConditionFn(
    option: RegistrationMassAction.WithdrawRegistrationManually | RegistrationMassAction.ChangeRegistrationsClassrun
  ): (registration: RegistrationViewModel) => boolean {
    return registration => {
      const generalCondition =
        (registration.canActionOnRegistrationList(this.classRunVm.data, this.course) &&
          Registration.hasActionOnRegistrationListPermission(this.currentUser, this.course)) ||
        (Registration.canActionOnParticipantList(this.classRunVm.data, this.course) &&
          Registration.hasActionOnParticipantListPermission(this.currentUser, this.course)) ||
        (Registration.canActionOnWaitlist(this.classRunVm.data, this.course) &&
          Registration.hasActionOnWaitlistPermission(this.currentUser, this.course));
      if (option === RegistrationMassAction.WithdrawRegistrationManually) {
        return generalCondition && !this.classRunVm.data.started() && this.canWithdrawRegistrationByCA(registration);
      }
      return generalCondition && !this.classRunVm.data.started() && this.canChangeRegistrationClassByCA(registration);
    };
  }

  public get confirmOrRejectConditionFn(): (dataItem: Registration) => boolean {
    return dataItem =>
      (this.selectedTab === CAMTabConfiguration.RegistrationsTab &&
        dataItem.canActionOnRegistrationList(this.classRunVm.data, this.course) &&
        Registration.hasActionOnRegistrationListPermission(this.currentUser, this.course)) ||
      (this.selectedTab === CAMTabConfiguration.WithdrawalRequestsTab &&
        dataItem.canActionOnWithdrawalRequestList(this.classRunVm.data, this.course) &&
        Registration.hasActionOnWithdrawalRequestListPermission(this.currentUser, this.course)) ||
      (this.selectedTab === CAMTabConfiguration.ChangeClassRunRequestsTab &&
        dataItem.canActionOnChangeClassRequestList(this.classRunVm.data, this.course) &&
        Registration.hasActionOnChangeClassRequestListPermission(this.currentUser, this.course));
  }

  public get isRescheduleMode(): boolean {
    return (
      this.detailPageInput.data.searchType === SearchClassRunType.ReschedulePending &&
      this.classRunVm.rescheduleStatus === ClassRunRescheduleStatus.PendingApproval
    );
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = classRunDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public loadClassRunInfo(): void {
    this._loadClassRunInfoSub.unsubscribe();
    const courseObs: Observable<Course | null> =
      this.detailPageInput.data.courseId != null ? this.courseRepository.loadCourse(this.detailPageInput.data.courseId) : of(null);
    const classRunObs =
      this.detailPageInput.data.courseId != null && this.detailPageInput.data.id != null
        ? this.classRunRepository.loadClassRunById(this.detailPageInput.data.id)
        : of(null);
    this.loadingData = true;
    this._loadClassRunInfoSub = combineLatest(courseObs, classRunObs)
      .pipe(
        switchMap(([course, classRun]) => {
          const coursePlanningCycleObs = course.coursePlanningCycleId
            ? this.coursePlanningCycleRepository.loadCoursePlanningCycleById(course.coursePlanningCycleId)
            : of(null);
          return coursePlanningCycleObs.pipe(
            switchMap(coursePlanningCycle => {
              return ClassRunDetailViewModel.create(
                ids =>
                  this.userRepository.loadUserInfoList(
                    {
                      userIds: ids,
                      pageIndex: 0,
                      pageSize: 0
                    },
                    null,
                    ['All']
                  ),
                course,
                classRun
              ).pipe(map(classRunVm => <[Course, ClassRunDetailViewModel, CoursePlanningCycle]>[course, classRunVm, coursePlanningCycle]));
            })
          );
        }),
        this.untilDestroy()
      )
      .subscribe(
        ([course, classRunVm, coursePlanningCycle]) => {
          this.classRunVm = classRunVm;
          this.course = course;
          this.coursePlanningCycle = coursePlanningCycle;
          this.loadBreadcrumb();
          this.initFormData();
          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(() => this.dataHasChanged(), () => this.validateAndSaveClassRun());
  }

  public showSubmitClassRun(): boolean {
    return (
      (this.detailPageInput.data.mode === ClassRunDetailMode.Edit || this.detailPageInput.data.mode === ClassRunDetailMode.NewClassRun) &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public showEditClassRun(): boolean {
    return (
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.classRunVm.data.isEditable(this.course) &&
      ClassRun.hasEditClassRunPermission(this.course, this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public onEditClassRun(): void {
    this.detailPageInput.data.mode = ClassRunDetailMode.Edit;
    this.tabStrip.selectTab(0);
  }

  public showPublishClassRun(): boolean {
    return (
      this.classRunVm.data.canPublish(this.course) &&
      ClassRun.hasPublishUnPublishPermission(this.course, this.currentUser) &&
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public showUnpublishClassRun(): boolean {
    return (
      this.classRunVm.data.canUnpublish(this.course) &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab &&
      ClassRun.hasPublishUnPublishPermission(this.course, this.currentUser)
    );
  }

  public canWithdrawRegistrationByCA(registrationVm: RegistrationViewModel): boolean {
    return registrationVm.canWithdrawByCA(this.course) && this.course.hasAdministrationPermission(this.currentUser);
  }

  public canChangeRegistrationClassByCA(registrationVm: RegistrationViewModel): boolean {
    return registrationVm.canChangeClassByCA(this.course) && this.course.hasAdministrationPermission(this.currentUser);
  }

  public canViewSessions(): boolean {
    return this.detailPageInput.data.mode === ClassRunDetailMode.View && this.course.hasViewSessionsPermission(this.currentUser);
  }

  public canViewRegistration(): boolean {
    return this.canViewRegistrations();
  }

  public canViewParticipant(): boolean {
    return this.canViewRegistrations();
  }

  public canViewWaitlist(): boolean {
    return this.canViewRegistrations();
  }

  public canViewWithdrawalRequest(): boolean {
    return this.canViewRegistrations();
  }

  public canViewChangeClassRunRequest(): boolean {
    return this.canViewRegistrations();
  }

  public canViewComment(): boolean {
    return this.detailPageInput.data.mode === ClassRunDetailMode.View;
  }

  public checkCommentsHasDataFnCreator(): () => Observable<boolean> {
    return () => {
      return from(
        this.commentApiService.getCommentNotSeen({
          objectIds: [this.detailPageInput.data.id],
          entityCommentType: EntityCommentType.ClassRun
        })
      ).pipe(
        map(data => {
          if (data == null || data.length === 0) {
            return false;
          }

          const commentDic = Utils.toDictionarySelect(data, x => x.objectId, x => x.commentNotSeenIds);

          if (commentDic[this.detailPageInput.data.id] == null) {
            return null;
          }

          return commentDic[this.detailPageInput.data.id].length > 0;
        })
      );
    };
  }

  public showCancellationRequestDialog(input: unknown, status: ClassRunCancellationStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CancellationRequestDialogComponent, input);

    this.subscribe(dialogRef.result, (data: ClassRunCancellationInput) => {
      if (data.comment) {
        const cancellationRequest: IClassRunCancellationStatusRequest = {
          ids: [this.detailPageInput.data.id],
          comment: data.comment,
          cancellationStatus: status
        };
        this.changeCancellationStatus(cancellationRequest);
      }
    });
  }

  public showRescheduleRequestDialog(input: unknown, status: ClassRunRescheduleStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(RescheduleRequestDialogComponent, input);

    this.subscribe(dialogRef.result, (data: ClassRunRescheduleInput) => {
      if (data.comment && data.startDateTime && data.endDateTime) {
        const rescheduleRequest: IClassRunRescheduleStatusRequest = {
          ids: [this.detailPageInput.data.id],
          comment: data.comment,
          startDateTime: data.startDateTime,
          endDateTime: data.endDateTime,
          rescheduleSessions: data.rescheduleSessions,
          rescheduleStatus: status
        };
        this.changeRescheduleStatus(rescheduleRequest);
      }
    });
  }

  public showCancellationRequest(): boolean {
    return (
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.classRunVm.data.canCancelClass(this.course) &&
      ClassRun.hasCancelClassPermission(this.course, this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public showRescheduleRequest(): boolean {
    return (
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.classRunVm.data.canRescheduleClass(this.course) &&
      ClassRun.hasRescheduleClassPermission(this.course, this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public showApprovalRequest(): boolean {
    return (
      (this.classRunVm.data.canCancellationRequestBeApproved(this.course) &&
        ClassRun.hasApprovalCancellationClassRunRequestPermission(this.course, this.currentUser) &&
        this.detailPageInput.data.searchType === SearchClassRunType.CancellationPending) ||
      (this.classRunVm.data.canRescheduleRequestBeApproved(this.course) &&
        ClassRun.hasApprovalRescheduleClassRunRequestPermission(this.course, this.currentUser) &&
        this.detailPageInput.data.searchType === SearchClassRunType.ReschedulePending)
    );
  }

  public showToggleCourseCriteriaActivated(): boolean {
    return (
      this.course.canActivateCourseCriteria(this.currentUser) &&
      !this.classRunVm.data.courseCriteriaActivated &&
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public showToggleCourseCriteriaDeactivated(): boolean {
    return (
      this.course.canActivateCourseCriteria(this.currentUser) &&
      this.classRunVm.data.courseCriteriaActivated &&
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public showToggleCourseAutomateActivated(): boolean {
    return (
      !this.course.canByPassCA() &&
      this.course.canActivateCourseAutomate(this.currentUser) &&
      !this.classRunVm.data.courseAutomateActivated &&
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public showToggleCourseAutomateDeActivated(): boolean {
    return (
      !this.course.canByPassCA() &&
      this.course.canActivateCourseAutomate(this.currentUser) &&
      this.classRunVm.data.courseAutomateActivated &&
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.selectedTab === CAMTabConfiguration.ClassRunInfoTab
    );
  }

  public onSaveClassRun(): void {
    this.validateAndSaveClassRun().subscribe(() => this.navigationPageService.navigateBack());
  }

  public validateAndSaveClassRun(): Observable<boolean> {
    return from(
      new Promise<boolean>((resolve, reject) => {
        this.validate().then(valid => {
          if (valid) {
            this.saveClassRun().then(_ => {
              this.showNotification();
              resolve(true);
            }, reject);
          } else {
            reject(false);
          }
        });
      })
    );
  }

  public dataHasChanged(): boolean {
    return this.classRunVm && this.classRunVm.dataHasChanged();
  }

  public onActionApproval(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.Publish:
        this.showWarningPublishDialog();
        break;
      case ContextMenuAction.Unpublish:
        this.changeClassRunStatus(this.classRunVm.data, ClassRunStatus.Unpublished);
        break;
      case ContextMenuAction.CancellationRequest:
        this.showCancellationRequestDialog(
          {
            title: `${this.translate('Cancel Request')}: ${this.classRunVm.classTitle}`,
            classRunId: this.detailPageInput.data.id
          },
          ClassRunCancellationStatus.PendingApproval
        );
        break;
      case ContextMenuAction.RescheduleRequest:
        this.showRescheduleRequestDialog(
          {
            title: `${this.translate('Reschedule Request')}: ${this.classRunVm.classTitle}`,
            classRunId: this.detailPageInput.data.id,
            courseServiceSchemeIds: this.course.serviceSchemeIds
          },
          ClassRunRescheduleStatus.PendingApproval
        );
        break;
      case ContextMenuAction.ActivateCourseCriteria:
        this.toggleCourseCriteria(this.detailPageInput.data.id, true);
        break;
      case ContextMenuAction.DeActivateCourseCriteria:
        this.toggleCourseCriteria(this.detailPageInput.data.id, false);
        break;
      case ContextMenuAction.ActivateCourseAutomate:
        this.toggleCourseAutomate(this.detailPageInput.data.id, true);
        break;
      case ContextMenuAction.DeActivateCourseAutomate:
        this.toggleCourseAutomate(this.detailPageInput.data.id, false);
        break;
      case ContextMenuAction.Approve:
        switch (this.detailPageInput.data.searchType) {
          case SearchClassRunType.CancellationPending:
            this.showApprovalCancellationDialog(
              {
                title: `${this.translate('Approve Cancellation')}: ${this.classRunVm.classTitle}`,
                requiredCommentField: false
              },
              ClassRunCancellationStatus.Approved
            );
            break;
          case SearchClassRunType.ReschedulePending:
            this.showApprovalRescheduleDialog(
              {
                title: `${this.translate('Approve Reschedule')}: ${this.classRunVm.classTitle}`,
                requiredCommentField: false
              },
              ClassRunRescheduleStatus.Approved
            );
            break;
        }
        break;

      case ContextMenuAction.Reject:
        switch (this.detailPageInput.data.searchType) {
          case SearchClassRunType.CancellationPending:
            this.showApprovalCancellationDialog(
              {
                title: `${this.translate('Reject Cancellation')}: ${this.classRunVm.classTitle}`
              },
              ClassRunCancellationStatus.Rejected
            );
            break;
          case SearchClassRunType.ReschedulePending:
            this.showApprovalRescheduleDialog(
              {
                title: `${this.translate('Reject Reschedule')}: ${this.classRunVm.classTitle}`
              },
              ClassRunRescheduleStatus.Rejected
            );
            break;
        }
        break;
      default:
        break;
    }
  }

  public selectedContextMenuRegistrationList(contextMenuEmit: ContextMenuEmit<RegistrationViewModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Confirm:
        this.handleConfirmRegistration([contextMenuEmit.dataItem]);
        break;
      case ContextMenuAction.Reject:
        this.handleRejectRegistration([contextMenuEmit.dataItem]);
        break;
      case ContextMenuAction.MoveToWaitlist:
        this.handleMoveToWaitlistRegistration([contextMenuEmit.dataItem]);
        break;
      case ContextMenuAction.WithdrawManuallyByCA:
        this.handleConfirmWithdrawalRequest([contextMenuEmit.dataItem], '', false, true);
        break;
      case ContextMenuAction.ChangeRegistrationClass:
        this.handleChangeRegistrationClass([contextMenuEmit.dataItem]);
        break;
      default:
        break;
    }
  }

  public handleMassAction(massAction: RegistrationMassAction, dataItems: RegistrationViewModel[]): Promise<boolean> {
    let massActionPromise: Promise<boolean>;
    switch (massAction) {
      case RegistrationMassAction.ConfirmRegistration:
        massActionPromise = this.handleConfirmRegistration(dataItems, false);
        break;
      case RegistrationMassAction.ConfirmWithdrawalRequest:
        massActionPromise = this.handleConfirmWithdrawalRequest(dataItems, 'Confirm Withdrawal', true, false);
        break;
      case RegistrationMassAction.ConfirmChangeClassRequest:
        massActionPromise = this.handleConfirmChangeClassRequest(dataItems, false);
        break;
      case RegistrationMassAction.RejectRegistration:
        massActionPromise = this.handleRejectRegistration(dataItems, false);
        break;
      case RegistrationMassAction.RejectWithdrawalRequest:
        massActionPromise = this.handleRejectWithdrawalRequest(dataItems, false);
        break;
      case RegistrationMassAction.RejectChangeClassRequest:
        massActionPromise = this.handleRejectChangeClassRequest(dataItems, false);
        break;
      case RegistrationMassAction.MoveToWaitlistRegistration:
        massActionPromise = this.handleMoveToWaitlistRegistration(dataItems, false);
        break;
      case RegistrationMassAction.SendOfferWaitlistRegistration:
        massActionPromise = this.handleSendOfferWaitlistRegistration(this.detailPageInput.data.id, dataItems, false);
        break;
      case RegistrationMassAction.WithdrawRegistrationManually:
        massActionPromise = this.handleConfirmWithdrawalRequest(dataItems, '', false, false);
        break;
      case RegistrationMassAction.ChangeRegistrationsClassrun:
        massActionPromise = this.handleChangeRegistrationClass(dataItems, false);
        break;
    }
    return massActionPromise.then(_ => {
      this.resetSelectedItems();
      return _;
    });
  }

  public handleConfirmRegistration(registrations: RegistrationViewModel[], showNotification: boolean = true): Promise<boolean> {
    return this.confirmRegistration(registrations, showNotification);
  }

  public handleRejectRegistration(registrations: RegistrationViewModel[], showNotification: boolean = true): Promise<boolean> {
    return this.showApprovalDialog(
      {
        title: `${this.translate('Reject Registration')}:`
      },
      registrations,
      RegistrationStatus.RejectedByCA,
      showNotification
    );
  }

  public handleMoveToWaitlistRegistration(registrations: RegistrationViewModel[], showNotification: boolean = true): Promise<boolean> {
    return this.showApprovalDialog(
      {
        title: `${this.translate('Move to Waitlist')}:`
      },
      registrations,
      RegistrationStatus.WaitlistPendingApprovalByLearner,
      showNotification
    );
  }

  public showWarningPublishDialog(): void {
    if (this.classRunVm.data.planningStartTime == null || this.classRunVm.data.planningEndTime == null) {
      this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'You need to provide both Class Start Time and Class End Time before you are able to publish the classrun.',
        yesBtnText: 'OK',
        hideNoBtn: true
      });
    } else {
      this.changeClassRunStatus(this.classRunVm.data, ClassRunStatus.Published);
    }
  }
  public showApprovalDialog(
    input: unknown,
    registrations: Registration[],
    registrationStatus: RegistrationStatus,
    showNotification: boolean = true
  ): Promise<boolean> {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    return dialogRef.result
      .pipe(
        switchMap((data: IDialogActionEvent) => {
          if (data.action === DialogAction.OK) {
            return this.changeRegistrationStatus(registrations, registrationStatus, data.comment, showNotification);
          }
          return of(false);
        })
      )
      .toPromise();
  }

  public confirmRegistration(registrations: Registration[], showNotification: boolean = true): Promise<boolean> {
    return from(this.classRunApiService.checkClassIsFull(this.detailPageInput.data.id))
      .pipe(
        switchMap(isClassFull => {
          if (!isClassFull) {
            this.showApprovalDialog(
              {
                title: `${this.translate('Confirm Registration')}:`,
                requiredCommentField: false
              },
              registrations,
              RegistrationStatus.ConfirmedByCA,
              showNotification
            );
          } else {
            return this.showApprovalDialog(
              {
                title: `${this.translate('The class is currently full. Do you want to move this learner to waitlist?')}`
              },
              registrations,
              RegistrationStatus.WaitlistPendingApprovalByLearner,
              showNotification
            );
          }
        })
      )
      .toPromise();
  }

  public showApprovalWithdrawDialog(
    input: unknown,
    registrations: Registration[],
    withdrawalStatus: WithdrawalStatus,
    showCommentField: boolean = true,
    showNotification: boolean = true
  ): Promise<boolean> {
    if (showCommentField) {
      const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
      return dialogRef.result
        .pipe(
          switchMap((data: IDialogActionEvent) => {
            if (data.action === DialogAction.OK) {
              return this.changeRegistrationWithdrawStatus(registrations, withdrawalStatus, data.comment, showNotification);
            }
            return of(false);
          })
        )
        .toPromise();
    }
    return this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Withdraw',
        confirmMsg: 'Do you want to withdraw this learner from the current class?',
        yesBtnText: 'Proceed'
      })
      .pipe(
        switchMap(action => {
          if (action === DialogAction.OK) {
            return this.changeRegistrationWithdrawStatus(registrations, withdrawalStatus, 'Withdrawn By CA', showNotification, true);
          }
          return of(false);
        })
      )
      .toPromise();
  }

  public showApprovalChangeClassDialog(
    input: unknown,
    registrations: Registration[],
    classRunChangeStatus: ClassRunChangeStatus,
    showNotification: boolean = true
  ): Promise<boolean> {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    return dialogRef.result
      .pipe(
        switchMap((data: IDialogActionEvent) => {
          if (data.action === DialogAction.OK) {
            return this.changeRegistrationChangeClassRunStatus(registrations, classRunChangeStatus, data.comment, showNotification);
          }
          return of(false);
        })
      )
      .toPromise();
  }

  public selectedContextMenuParticipantList(contextMenuEmit: ContextMenuEmit<RegistrationViewModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.WithdrawManuallyByCA:
        this.handleConfirmWithdrawalRequest([contextMenuEmit.dataItem], '', false, true);
        break;
      case ContextMenuAction.ChangeRegistrationClass:
        this.handleChangeRegistrationClass([contextMenuEmit.dataItem]);
        break;
      default:
        break;
    }
  }

  public selectedContextMenuWaitlistList(contextMenuEmit: ContextMenuEmit<RegistrationViewModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.SendOffer:
        this.handleSendOfferWaitlistRegistration(contextMenuEmit.dataItem.classRunId, [contextMenuEmit.dataItem]);
        break;
      case ContextMenuAction.WithdrawManuallyByCA:
        this.handleConfirmWithdrawalRequest([contextMenuEmit.dataItem], '', false, true);
        break;
      case ContextMenuAction.ChangeRegistrationClass:
        this.handleChangeRegistrationClass([contextMenuEmit.dataItem]);
        break;
      default:
        break;
    }
  }

  public handleSendOfferWaitlistRegistration(
    classRunId: string,
    registrations: Registration[],
    showNotification: boolean = true
  ): Promise<boolean> {
    return from(this.classRunApiService.checkClassIsFull(classRunId))
      .pipe(
        switchMap(_ => {
          if (!_) {
            return this.changeRegistrationStatus(registrations, RegistrationStatus.OfferPendingApprovalByLearner, '', showNotification);
          }
          return this.opalDialogService
            .openConfirmDialog({
              confirmTitle: 'Warning',
              confirmMsg: 'The class is currently full.',
              hideNoBtn: true,
              yesBtnText: 'Close'
            })
            .pipe(map(__ => false));
        })
      )
      .toPromise();
  }

  public selectedContextMenuWithdrawList(contextMenuEmit: ContextMenuEmit<RegistrationViewModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Confirm:
        this.handleConfirmWithdrawalRequest([contextMenuEmit.dataItem], 'Confirm Withdrawal', true, true);
        break;
      case ContextMenuAction.Reject:
        this.handleRejectWithdrawalRequest([contextMenuEmit.dataItem]);
        break;
      default:
        break;
    }
  }

  public handleConfirmWithdrawalRequest(
    registrations: RegistrationViewModel[],
    title: string,
    showCommentField: boolean = true,
    showNotification: boolean = true
  ): Promise<boolean> {
    return this.showApprovalWithdrawDialog(
      {
        title: title.length > 0 ? `${this.translate(title)}` : '',
        requiredCommentField: false
      },
      registrations,
      WithdrawalStatus.Withdrawn,
      showCommentField,
      showNotification
    );
  }

  public handleRejectWithdrawalRequest(registrations: RegistrationViewModel[], showNotification: boolean = true): Promise<boolean> {
    return this.showApprovalWithdrawDialog(
      {
        title: `${this.translate('Reject Withdrawal')}:`
      },
      registrations,
      WithdrawalStatus.RejectedByCA,
      true,
      showNotification
    );
  }

  public isDisableSendPlacementBtn(): boolean {
    return this.participantSelectedItems.length <= 0 || this.course.isArchived();
  }

  public hasParticipantActionPermission(): boolean {
    return this.course.hasAdministrationPermission(this.currentUser) && this.selectedTab === CAMTabConfiguration.ParticipantsTab;
  }

  public selectedContextMenuChangeClassRunList(contextMenuEmit: ContextMenuEmit<RegistrationViewModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Confirm:
        this.handleConfirmChangeClassRequest([contextMenuEmit.dataItem]);
        break;
      case ContextMenuAction.Reject:
        this.handleRejectChangeClassRequest([contextMenuEmit.dataItem]);
        break;
      default:
        break;
    }
  }

  public handleConfirmChangeClassRequest(registrations: RegistrationViewModel[], showNotification: boolean = true): Promise<boolean> {
    return this.showApprovalChangeClassDialog(
      {
        title: `${this.translate('Confirm Change Class')}:`,
        requiredCommentField: false
      },
      registrations,
      ClassRunChangeStatus.ConfirmedByCA,
      showNotification
    );
  }

  public handleRejectChangeClassRequest(registrations: RegistrationViewModel[], showNotification: boolean = true): Promise<boolean> {
    return this.showApprovalChangeClassDialog(
      {
        title: `${this.translate('Reject Change Class')}:`
      },
      registrations,
      ClassRunChangeStatus.RejectedByCA,
      showNotification
    );
  }

  public handleChangeRegistrationClass(registrations: RegistrationViewModel[], showNotification: boolean = true): Promise<boolean> {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
      ChangeRegistrationClassDialogComponent,
      {
        courseId: this.course.id,
        currentClassId: this.classRunVm.id,
        registrationIds: registrations.map(p => p.id)
      },
      ChangeRegistrationClassDialogComponent.defaultDialogSettings
    );
    return dialogRef.result
      .pipe(
        map((data: DialogAction) => {
          return data === DialogAction.OK;
        })
      )
      .toPromise();
  }

  public sendPlacementLetter(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(SendPlacementLetterDialogComponent, {
      registrationVms: this.participantSelectedItems,
      classRunId: this.detailPageInput.data.id
    });

    this.subscribe(dialogRef.result, (data: DialogAction) => {
      if (data === DialogAction.OK) {
        this.participantSelectedItems = [];
      }
    });
  }

  public checkSearchRegistrationHasDataFnCreator(searchType: SearchRegistrationsType): () => Observable<boolean> {
    const containFilters = [];
    const fromToFilters = [];

    if (searchType === SearchRegistrationsType.ClassRunRegistration) {
      containFilters.push({
        field: 'Status',
        values: [RegistrationStatus.Approved],
        notContain: false
      });
    } else if (searchType === SearchRegistrationsType.ChangeClassRun) {
      containFilters.push({
        field: 'ClassRunChangeStatus',
        values: [ClassRunChangeStatus.Approved],
        notContain: false
      });
    } else if (searchType === SearchRegistrationsType.Withdrawal) {
      containFilters.push({
        field: 'WithdrawalStatus',
        values: [WithdrawalStatus.Approved],
        notContain: false
      });
    }

    const filterRegistration: IFilter = {
      containFilters: containFilters,
      fromToFilters: fromToFilters
    };

    return () => {
      const registrationObs: Observable<OpalGridDataResult<RegistrationViewModel>> = this.classRunVm.data.started()
        ? of({
            data: [],
            total: 0
          })
        : this.listRegistrationGridComponentService.loadRegistration(
            this.detailPageInput.data.courseId,
            this.detailPageInput.data.id,
            searchType,
            '',
            false,
            null,
            filterRegistration,
            0,
            0,
            false,
            null,
            null,
            null,
            null
          );
      return registrationObs.pipe(
        map(data => {
          return data.total > 0;
        })
      );
    };
  }

  public onViewRegistration(dataItem: RegistrationViewModel, activeTab: CAMTabConfiguration, searchType: SearchLearnerProfileType): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.LearnerProfilePage,
      {
        activeTab: CAMTabConfiguration.PersonalInfoTab,
        data: {
          registrationId: dataItem.id,
          userId: dataItem.userId,
          courseId: dataItem.courseId,
          classRunId: dataItem.classRunId,
          searchType: searchType
        }
      },
      this.detailPageInput
    );
  }

  public get getSelectedItemsForMassAction(): RegistrationViewModel[] {
    switch (this.selectedTab) {
      case CAMTabConfiguration.RegistrationsTab:
        return this.registrationSelectedItems;
      case CAMTabConfiguration.WaitlistTab:
        return this.waitlistSelectedItems;
      case CAMTabConfiguration.WithdrawalRequestsTab:
        return this.withdrawalRequestSelectedItems;
      case CAMTabConfiguration.ChangeClassRunRequestsTab:
        return this.changeClassRequestSelectedItems;
      case CAMTabConfiguration.ParticipantsTab:
        return this.participantSelectedItems;
      default:
        return [];
    }
  }

  public get canFilterRegistration(): boolean {
    return [
      CAMTabConfiguration.RegistrationsTab,
      CAMTabConfiguration.ParticipantsTab,
      CAMTabConfiguration.WaitlistTab,
      CAMTabConfiguration.WithdrawalRequestsTab,
      CAMTabConfiguration.ChangeClassRunRequestsTab
    ].includes(this.selectedTab);
  }

  public resetSelectedItems(): void {
    this.registrationSelectedItems = [];
    this.waitlistSelectedItems = [];
    this.withdrawalRequestSelectedItems = [];
    this.changeClassRequestSelectedItems = [];
    this.participantSelectedItems = [];
  }

  public onApplyFilter(data: RegistrationFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.hasCoursePlanningAsParentPage = this.navigationPageService.findParentOfCurrentRouter(CAMRoutePaths.CoursePlanningPage) != null;
    this.loadClassRunInfo();
    this.scheduler.init();
  }

  protected onDestroy(): void {
    this.scheduler.destroy();
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return this.currentUser.permissionDic;
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'classrun-detail',
      validateByGroupControlNames: [
        ['minClassSize', 'maxClassSize'],
        ['startDate', 'endDate', 'applicationStartDate', 'applicationEndDate'],
        ['planningStartTime', 'planningEndTime']
      ],
      controls: {
        classTitle: {
          defaultValue: this.classRunVm.classTitle,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            },
            {
              validator: Validators.maxLength(2000)
            }
          ]
        },
        classRunCode: {
          defaultValue: this.classRunVm.classRunCode,
          validators: null
        },
        startDate: {
          defaultValue: this.classRunVm.startDate,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Date is mandatory field')
            },
            {
              validator: ifValidator(p => Utils.isDifferent(this.classRunVm.originalData.startDate, p.value), () => futureDateValidator()),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Date cannot be in the past')
            },
            {
              validator: startEndValidator('startDateWithCourseStartDate', p => this.course.startDate, p => p.value, true, 'dateOnly'),
              validatorType: 'startDateWithCourseStartDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date must be in the time frame of the course')
            },
            {
              validator: startEndValidator('startDateWithCourseEndDate', p => p.value, p => this.course.expiredDate, true, 'dateOnly'),
              validatorType: 'startDateWithCourseEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date must be in the time frame of the course')
            },
            {
              validator: startEndValidator('classRunStartDate', p => p.value, p => this.classRunVm.endDate, true, 'dateOnly'),
              validatorType: 'classRunStartDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Date cannot be greater than Class End Date')
            }
          ]
        },
        endDate: {
          defaultValue: this.classRunVm.endDate,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class End Date is mandatory field')
            },
            {
              validator: ifValidator(p => Utils.isDifferent(this.classRunVm.originalData.endDate, p.value), () => futureDateValidator()),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class End Date cannot be in the past')
            },
            {
              validator: startEndValidator('endDateWithCourseStartDate', p => this.course.startDate, p => p.value, true, 'dateOnly'),
              validatorType: 'endDateWithCourseStartDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End date must be in the time frame of the course')
            },
            {
              validator: startEndValidator('endDateWithCourseEndDate', p => p.value, p => this.course.expiredDate, true, 'dateOnly'),
              validatorType: 'endDateWithCourseEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End date must be in the time frame of the course')
            },
            {
              validator: startEndValidator('classRunEndDate', p => this.classRunVm.startDate, p => p.value, true, 'dateOnly'),
              validatorType: 'classRunEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class End Date cannot be less than Class Start Date')
            }
          ]
        },
        planningStartTime: {
          defaultValue: this.classRunVm.planningStartTime,
          validators: [
            {
              validator: startEndValidator('classRunStartTime', p => p.value, p => this.classRunVm.planningEndTime, true, 'timeOnly'),
              validatorType: 'classRunStartTime',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Time cannot be greater than Class End Time')
            },
            {
              validator: ifValidator(
                p => DateUtils.compareOnlyDate(this.classRunVm.startDate, new Date()) === 0,
                () =>
                  startEndValidator(
                    'classRunPlanningStartTimeWithCurrentTime',
                    p => new Date(),
                    p => DateUtils.buildDateTime(this.classRunVm.startDate, p.value),
                    true,
                    'default'
                  )
              ),
              validatorType: 'classRunPlanningStartTimeWithCurrentTime',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Time must be in the future')
            }
          ]
        },
        planningEndTime: {
          defaultValue: this.classRunVm.planningEndTime,
          validators: [
            {
              validator: startEndValidator('classRunEndTime', p => this.classRunVm.planningStartTime, p => p.value, true, 'timeOnly'),
              validatorType: 'classRunEndTime',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Class End Time cannot be less than Class Start Time')
            }
          ]
        },
        facilitatorIds: {
          defaultValue: this.classRunVm.data.facilitatorIds,
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required'
            }
          ]
        },
        coFacilitatorIds: {
          defaultValue: this.classRunVm.data.coFacilitatorIds,
          validators: null
        },
        minClassSize: {
          defaultValue: this.classRunVm.minClassSize,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },
            {
              validator: startEndValidator('classRunMinimumWithMaximum', p => p.value, p => this.classRunVm.maxClassSize),
              validatorType: 'classRunMinimumWithMaximum',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Minimum class size cannot be greater than maximum class size'
              )
            },
            {
              validator: requiredNumberValidator(),
              validatorType: validateRequiredNumberType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The first number must not be entered 0')
            }
          ]
        },
        maxClassSize: {
          defaultValue: this.classRunVm.maxClassSize,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },
            {
              validator: startEndValidator('classRunMaximumWithMinimum', p => this.classRunVm.minClassSize, p => p.value),
              validatorType: 'classRunMaximumWithMinimum',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Maximum class size cannot be less than minimum class size'
              )
            }
          ]
        },
        applicationStartDate: {
          defaultValue: this.classRunVm.applicationStartDate,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Application Start Date is mandatory field')
            },
            {
              validator: ifValidator(
                p => Utils.isDifferent(p.value, this.classRunVm.originalData.applicationStartDate),
                () => futureDateValidator()
              ),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Application Start Date cannot be in the past')
            },
            {
              validator: startEndValidator(
                'classRunApplicationStartDateWithStartDate',
                p => p.value,
                p => this.classRunVm.startDate,
                true,
                'dateOnly'
              ),
              validatorType: 'classRunApplicationStartDateWithStartDate',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Application Start Date cannot be after ClassRun Start Date'
              )
            },
            {
              validator: startEndValidator(
                'classRunApplicationStartDateWithApplicationEndDate',
                p => p.value,
                p => this.classRunVm.applicationEndDate,
                true,
                'dateOnly'
              ),
              validatorType: 'classRunApplicationStartDateWithApplicationEndDate',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Class Application Start Date cannot be greater than Class Application End Date'
              )
            }
          ]
        },
        applicationEndDate: {
          defaultValue: this.classRunVm.applicationEndDate,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Application End Date is mandatory field')
            },
            {
              validator: futureDateValidator(),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Application End Date cannot be in the past')
            },
            {
              validator: startEndValidator(
                'classRunApplicationEndDateWithClassRunStartDate',
                p => p.value,
                p => this.classRunVm.startDate,
                true,
                'dateOnly',
                p => !this.course.isELearning()
              ),
              validatorType: 'classRunApplicationEndDateWithClassRunStartDate',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Application End Date cannot be after ClassRun Start Date'
              )
            },
            {
              validator: startEndValidator(
                'classRunApplicationEndDateWithClassRunEndDate',
                p => p.value,
                p => this.classRunVm.endDate,
                true,
                'dateOnly',
                p => this.course.isELearning()
              ),
              validatorType: 'classRunApplicationEndDateWithClassRunEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Application End Date cannot be after ClassRun End Date')
            },
            {
              validator: startEndValidator(
                'classRunApplicationEndDateWithApplicationStartDate',
                p => this.classRunVm.applicationStartDate,
                p => p.value,
                true,
                'dateOnly'
              ),
              validatorType: 'classRunApplicationEndDateWithApplicationStartDate',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Class Application End Date cannot be less than Class Application Start Date'
              )
            }
          ]
        }
      }
    };
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<ClassRunDetailPageInput, CAMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private changeClassRunStatus(classRun: ClassRun, status: ClassRunStatus): void {
    this.subscribe(this.classRunRepository.changeClassRunStatus({ ids: [classRun.id], status: status }), () => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  private toggleCourseCriteria(id: string, courseCriteriaActivated: boolean): void {
    this.subscribe(
      this.classRunRepository.toggleCourseCriteria({ classRunId: id, courseCriteriaActivated: courseCriteriaActivated }),
      () => {
        this.showNotification(
          `Course criteria ${courseCriteriaActivated ? 'activated' : 'deactivated'} successfully.`,
          NotificationType.Success
        );
        this.navigationPageService.navigateBack();
      }
    );
  }

  private toggleCourseAutomate(id: string, courseAutomateActivated: boolean): void {
    this.subscribe(
      this.classRunRepository.toggleCourseAutomate({ classRunId: id, courseAutomateActivated: courseAutomateActivated }),
      () => {
        this.showNotification(
          `Course Automate Selection ${courseAutomateActivated ? 'enabled' : 'disabled'} successfully.`,
          NotificationType.Success
        );
        this.navigationPageService.navigateBack();
      }
    );
  }

  private changeCancellationStatus(request: IClassRunCancellationStatusRequest): void {
    this.subscribe(this.classRunRepository.changeClassRunCancellationStatus(request), () => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  private changeRescheduleStatus(request: IClassRunRescheduleStatusRequest): void {
    this.subscribe(this.classRunRepository.changeClassRunRescheduleStatus(request, this.classRunVm.courseId), () => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  private changeRegistrationStatus(
    registrations: Registration[],
    status: RegistrationStatus,
    comment: string = '',
    showNotification: boolean = true
  ): Observable<boolean> {
    const registrationChangeStatus: IChangeRegistrationStatusRequest = {
      ids: registrations.map(p => p.id),
      status: status,
      comment: comment
    };
    return from(
      this.registrationRepository.changeRegistrationStatus(registrationChangeStatus).then(() => {
        if (showNotification) {
          this.showNotification();
        }
        registrations.forEach(p => (p.status = status));
        this.resetFilter();
        return true;
      })
    ).pipe(this.untilDestroy());
  }

  private changeRegistrationWithdrawStatus(
    registrations: Registration[],
    withdrawalStatus: WithdrawalStatus,
    comment: string = '',
    showNotification: boolean = true,
    isManual: boolean = false
  ): Observable<boolean> {
    const request: IChangeRegistrationWithdrawalStatusRequest = {
      ids: registrations.map(p => p.id),
      withdrawalStatus: withdrawalStatus,
      comment: comment,
      isManual: isManual
    };
    return from(
      this.registrationRepository.changeRegistrationWithdrawalStatus(request).then(() => {
        if (showNotification) {
          this.showNotification();
        }
        registrations.forEach(p => (p.withdrawalStatus = withdrawalStatus));
        this.resetFilter();
        return true;
      })
    ).pipe(this.untilDestroy());
  }

  private changeRegistrationChangeClassRunStatus(
    registrations: Registration[],
    classRunChangeStatus: ClassRunChangeStatus,
    comment: string = '',
    showNotification: boolean = true
  ): Observable<boolean> {
    const request: IChangeRegistrationChangeClassRunStatusRequest = {
      ids: registrations.map(p => p.id),
      classRunChangeStatus: classRunChangeStatus,
      comment: comment
    };
    return from(
      this.registrationRepository.changeRegistrationClassRunChangeStatus(request).then(() => {
        if (showNotification) {
          this.showNotification();
        }
        registrations.forEach(p => (p.classRunChangeStatus = classRunChangeStatus));
        this.resetFilter();
        return true;
      })
    ).pipe(this.untilDestroy());
  }

  private showApprovalCancellationDialog(input: unknown, classRunCancellationStatus: ClassRunCancellationStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    this.subscribe(dialogRef.result, (data: IDialogActionEvent) => {
      if (data.action === DialogAction.OK) {
        const approveCancellationRequest: IClassRunCancellationStatusRequest = {
          ids: [this.detailPageInput.data.id],
          cancellationStatus: classRunCancellationStatus,
          comment: data.comment
        };
        this.changeCancellationStatus(approveCancellationRequest);
      }
    });
  }

  private showApprovalRescheduleDialog(input: unknown, classRunRescheduleStatus: ClassRunRescheduleStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    this.subscribe(dialogRef.result, (data: IDialogActionEvent) => {
      if (data.action === DialogAction.OK) {
        const approveRescheduleRequest: IClassRunRescheduleStatusRequest = {
          ids: [this.detailPageInput.data.id],
          rescheduleStatus: classRunRescheduleStatus,
          comment: data.comment
        };
        this.changeRescheduleStatus(approveRescheduleRequest);
      }
    });
  }

  private canViewRegistrations(): boolean {
    return (
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      Registration.canViewRegistrations(this.classRunVm.data) &&
      Registration.hasViewRegistrationsPermissions(this.course, this.currentUser) &&
      !this.hasCoursePlanningAsParentPage
    );
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      CAM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p => this.navigationPageService.navigateByRouter(p, () => this.dataHasChanged(), () => this.validateAndSaveClassRun()),
        {
          [CAMRoutePaths.CoursePlanningCycleDetailPage]: {
            textFn: () => (this.coursePlanningCycle != null ? this.coursePlanningCycle.title : '')
          },
          [CAMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName },
          [CAMRoutePaths.ClassRunDetailPage]: { textFn: () => this.classRunVm.classTitle }
        }
      )
    );
  }

  private saveClassRun(): Promise<ClassRun> {
    return new Promise((resolve, reject) => {
      const request: ISaveClassRunRequest = {
        data: Utils.clone(this.classRunVm.data, cloneData => {
          cloneData.courseId = this.detailPageInput.data.courseId;
          cloneData.startDateTime = cloneData.buildStartDateTime();
          cloneData.endDateTime = cloneData.buildEndDateTime();
          cloneData.applicationStartDate = cloneData.buildApplicationStartTime();
          cloneData.applicationEndDate = cloneData.buildApplicationEndTime();
        })
      };
      this.classRunRepository
        .saveClassRun(request)
        .pipe(this.untilDestroy())
        .subscribe(classRun => {
          resolve(classRun);
        }, reject);
    });
  }
}

export enum RegistrationMassAction {
  ConfirmRegistration = 'confirmRegistration',
  ConfirmWithdrawalRequest = 'confirmWithdrawalRequest',
  ConfirmChangeClassRequest = 'confirmChangeClassRequest',
  RejectRegistration = 'rejectRegistration',
  RejectWithdrawalRequest = 'rejectWithdrawalRequest',
  RejectChangeClassRequest = 'rejectChangeClassRequest',
  MoveToWaitlistRegistration = 'moveToWaitlist',
  SendOfferWaitlistRegistration = 'sendOfferWaitlist',
  ChangeRegistrationsClassrun = 'changeRegistrationsClassrun',
  WithdrawRegistrationManually = 'withdrawRegistrationManually'
}

export const classRunDetailPageTabIndexMap = {
  0: CAMTabConfiguration.ClassRunInfoTab,
  1: CAMTabConfiguration.SessionsTab,
  2: CAMTabConfiguration.RegistrationsTab,
  3: CAMTabConfiguration.ParticipantsTab,
  4: CAMTabConfiguration.WaitlistTab,
  5: CAMTabConfiguration.WithdrawalRequestsTab,
  6: CAMTabConfiguration.ChangeClassRunRequestsTab,
  7: CAMTabConfiguration.ClassRunCommentTab
};
