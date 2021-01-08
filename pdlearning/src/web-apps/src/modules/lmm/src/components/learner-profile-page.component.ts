import {
  ATTENDANCE_TRACKING_STATUS_COLOR_MAP,
  BreadcrumbItem,
  BreadcrumbService,
  ContextMenuAction,
  LMMRoutePaths,
  LMMTabConfiguration,
  LearnerProfileViewModel,
  LearnerProfileVmService,
  NavigationData,
  NavigationPageService,
  REGISTRATION_LEARNING_STATUS_COLOR_MAP,
  RouterPageInput
} from '@opal20/domain-components';
import {
  AttendanceStatus,
  AttendanceTracking,
  AttendanceTrackingRepository,
  ClassRun,
  ClassRunRepository,
  Course,
  CourseRepository,
  IAttendanceTrackingStatusRequest,
  Registration,
  RegistrationLearningStatus,
  RegistrationRepository,
  SearchRegistrationsType,
  UserInfoModel
} from '@opal20/domain-api';
import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { ButtonAction, ButtonGroupButton, SPACING_CONTENT } from '@opal20/common-components';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';

import { LMM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/lmm-route-breadcumb-mapping-fn';
import { LearnerProfilePageInput } from '../models/learner-profile-page-input.model';
import { NAVIGATORS } from '../lmm.config';
import { SearchLearnerProfileType } from '../models/search-learner-profile-type.model';

@Component({
  selector: 'learner-profile-page',
  templateUrl: './learner-profile-page.component.html'
})
export class LearnerProfilePageComponent extends BasePageComponent {
  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;
  public classRun: ClassRun = new ClassRun();
  public course: Course = new Course();
  public registration: Registration = new Registration();
  public attendanceTracking: AttendanceTracking = new AttendanceTracking();
  public breadCrumbItems: BreadcrumbItem[] = [];
  public stickySpacing: number = SPACING_CONTENT;
  public actionBtnGroup: ButtonAction<unknown>[] = [];

  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Complete',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.Completed),
      shownIfFn: () => this.showCompleteButton()
    },
    {
      displayText: 'Incomplete',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.Incomplete),
      shownIfFn: () => this.showIncompleteButton()
    },
    {
      displayText: 'Present',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.Present),
      shownIfFn: () => this.showPresentButton()
    },
    {
      displayText: 'Absent',
      onClickFn: () => this.onActionButtonGroups(ContextMenuAction.Absent),
      shownIfFn: () => this.showAbsentButton()
    }
  ];
  public get title(): string {
    return this.learnerVm.user.fullName;
  }

  public get detailPageInput(): RouterPageInput<LearnerProfilePageInput, LMMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<LearnerProfilePageInput, LMMTabConfiguration, unknown> | undefined) {
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
    if (this.detailPageInput.data.searchType === SearchLearnerProfileType.Participant) {
      return REGISTRATION_LEARNING_STATUS_COLOR_MAP;
    } else if (this.detailPageInput.data.searchType === SearchLearnerProfileType.AttendanceTracking) {
      return ATTENDANCE_TRACKING_STATUS_COLOR_MAP;
    }
    return REGISTRATION_LEARNING_STATUS_COLOR_MAP;
  }

  public navigationData: NavigationData;
  public loadingData: boolean = false;
  public registrationStatusChanged: boolean = false;
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public ContextMenuAction: typeof ContextMenuAction = ContextMenuAction;
  private _detailPageInput: RouterPageInput<LearnerProfilePageInput, LMMTabConfiguration, unknown> = NAVIGATORS[
    LMMRoutePaths.LearnerProfilePage
  ] as RouterPageInput<LearnerProfilePageInput, LMMTabConfiguration, unknown>;
  private _loadUserInfoSub: Subscription = new Subscription();
  private _learnerVm: LearnerProfileViewModel = new LearnerProfileViewModel();
  private currentUser = UserInfoModel.getMyUserInfo();
  public get selectedTab(): LMMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : LMMTabConfiguration.PersonalInfoTab;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private classRunRepository: ClassRunRepository,
    private registrationRepository: RegistrationRepository,
    private attendanceTrackingRepository: AttendanceTrackingRepository,
    private learnerProfileVmService: LearnerProfileVmService,
    private navigationPageService: NavigationPageService,
    private courseRepository: CourseRepository,
    private breadcrumbService: BreadcrumbService
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack();
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = learnerProfilePageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public loadUserInfo(): void {
    this._loadUserInfoSub.unsubscribe();
    const registrationObs: Observable<Registration | null> =
      this.detailPageInput.data.registrationId != null
        ? this.registrationRepository.getRegistrationById(this.detailPageInput.data.registrationId)
        : of(null);
    const attendanceTrackingObs: Observable<AttendanceTracking | null> =
      this.detailPageInput.data.attendanceTrackingId != null
        ? this.attendanceTrackingRepository.loadAttendanceTrackingById(this.detailPageInput.data.attendanceTrackingId)
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
    this._loadUserInfoSub = combineLatest(registrationObs, attendanceTrackingObs, courseObs, classRunObs, userObs)
      .pipe(this.untilDestroy())
      .subscribe(
        ([registration, attendanceTracking, course, classRun, user]) => {
          this.course = course;
          this.classRun = classRun;
          this.registration = registration;
          this.attendanceTracking = attendanceTracking;

          if (user) {
            this.learnerVm = user;
          }
          this.loadBreadcrumb();
          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public onActionButtonGroups(action: ContextMenuAction): void {
    switch (this.detailPageInput.data.searchType) {
      case SearchLearnerProfileType.Participant:
        this.onActionParticipant(action);
        break;
      case SearchLearnerProfileType.AttendanceTracking:
        this.onActionAttendanceTracking(action);
        break;
      default:
        break;
    }
  }

  public showCompleteButton(): boolean {
    return (
      this.detailPageInput.data.searchType === SearchLearnerProfileType.Participant &&
      this.registration &&
      this.registration.learningStatus === RegistrationLearningStatus.Failed &&
      Registration.canManageRegistrations(this.course) &&
      Registration.hasManageRegistrationsPermission(this.currentUser, this.course, this.classRun) &&
      this.selectedTab === LMMTabConfiguration.PersonalInfoTab
    );
  }

  public showIncompleteButton(): boolean {
    return (
      this.detailPageInput.data.searchType === SearchLearnerProfileType.Participant &&
      this.registration &&
      this.registration.learningStatus === RegistrationLearningStatus.Completed &&
      Registration.canManageRegistrations(this.course) &&
      Registration.hasManageRegistrationsPermission(this.currentUser, this.course, this.classRun) &&
      this.selectedTab === LMMTabConfiguration.PersonalInfoTab
    );
  }

  public showPresentButton(): boolean {
    return (
      this.detailPageInput.data.searchType === SearchLearnerProfileType.AttendanceTracking &&
      this.attendanceTracking &&
      this.attendanceTracking.status !== AttendanceStatus.Present &&
      this.selectedTab === LMMTabConfiguration.PersonalInfoTab
    );
  }

  public showAbsentButton(): boolean {
    return (
      this.detailPageInput.data.searchType === SearchLearnerProfileType.AttendanceTracking &&
      this.attendanceTracking &&
      this.attendanceTracking.status !== AttendanceStatus.Absent &&
      this.selectedTab === LMMTabConfiguration.PersonalInfoTab
    );
  }

  public onActionParticipant(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.Completed:
        this.completeOrIncompleteRegistration([this.registration.id], true);
        break;
      case ContextMenuAction.Incomplete:
        this.completeOrIncompleteRegistration([this.registration.id], false);
        break;
    }
  }

  public onActionAttendanceTracking(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.Present:
        this.changeAttendanceStatus(AttendanceStatus.Present);
        break;
      case ContextMenuAction.Absent:
        this.changeAttendanceStatus(AttendanceStatus.Absent);
        break;
    }
  }

  public completeOrIncompleteRegistration(registrationIds: string[], isCompleted: boolean): void {
    this.registrationRepository
      .completeOrIncompleteRegistration({
        courseId: this.detailPageInput.data.courseId,
        classRunId: this.detailPageInput.data.classRunId,
        registrationIds: registrationIds,
        isCompleted: isCompleted
      })
      .then(_ => {
        this.showNotification();
        this.navigationPageService.navigateBack();
      });
  }

  public changeAttendanceStatus(status: AttendanceStatus): void {
    const request: IAttendanceTrackingStatusRequest = {
      sessionId: this.attendanceTracking.sessionId,
      ids: [this.attendanceTracking.id],
      status: status
    };
    this.attendanceTrackingRepository.changAttendanceStatus(request).subscribe(() => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  public displayStatus(): RegistrationLearningStatus {
    if (this.detailPageInput.data.searchType === SearchLearnerProfileType.Participant) {
      return this.registration.learningStatus;
    }
    return null;
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadUserInfo();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<LearnerProfilePageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      LMM_ROUTE_BREADCUMB_MAPPING_FN(this.detailPageInput, p => this.navigationPageService.navigateByRouter(p), {
        [LMMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName },
        [LMMRoutePaths.ClassRunDetailPage]: { textFn: () => this.classRun.classTitle },
        [LMMRoutePaths.LearnerProfilePage]: { textFn: () => this.learnerVm.user.fullName }
      })
    );
  }
}

export const learnerProfilePageTabIndexMap = {
  0: LMMTabConfiguration.PersonalInfoTab
};
