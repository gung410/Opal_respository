import {
  AssignmentDetailMode,
  AssignmentViewModel,
  AttendanceTrackingViewModel,
  BreadcrumbItem,
  BreadcrumbService,
  CONTENT_STATUS_COLOR_MAP,
  ClassRunDetailMode,
  ClassRunDetailViewModel,
  CommentDialogComponent,
  CommentTabInput,
  ContextMenuAction,
  ContextMenuEmit,
  IDialogActionEvent,
  IOpalReportDynamicParams,
  LMMRoutePaths,
  LMMTabConfiguration,
  ListRegistrationGridDisplayColumns,
  NavigationData,
  NavigationPageService,
  OpalReportDynamicComponent,
  RegistrationFilterComponent,
  RegistrationFilterModel,
  RegistrationViewModel,
  RouterPageInput,
  WebAppLinkBuilder
} from '@opal20/domain-components';
import { BaseFormComponent, ComponentType, IFormBuilderDefinition, IGridFilter, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BrokenLinkModuleIdentifier,
  COMMENT_ACTION_MAPPING,
  ClassRun,
  ClassRunRepository,
  ClassRunStatus,
  CommentApiService,
  CommentServiceType,
  ContentStatus,
  Course,
  CourseContentItemModel,
  CourseRepository,
  EntityCommentType,
  LearningContentRepository,
  Registration,
  RegistrationLearningStatus,
  RegistrationRepository,
  SearchRegistrationsType,
  UserInfoModel,
  UserRepository
} from '@opal20/domain-api';
import { ButtonAction, ButtonGroupButton, DialogAction, OpalDialogService, SPACING_CONTENT } from '@opal20/common-components';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import { map, switchMap } from 'rxjs/operators';

import { ClassRunDetailPageInput } from '../models/classrun-detail-page-input.model';
import { ContentDetailPageComponent } from './content-detail-page.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { LMM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/lmm-route-breadcumb-mapping-fn';
import { NAVIGATORS } from '../lmm.config';
import { SearchLearnerProfileType } from '../models/search-learner-profile-type.model';

@Component({
  selector: 'classrun-detail-page',
  templateUrl: './classrun-detail-page.component.html'
})
export class ClassRunDetailPageComponent extends BaseFormComponent {
  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;
  @ViewChild(ContentDetailPageComponent, { static: false }) public contentDetailPage: ContentDetailPageComponent;

  public filterPopupContent: ComponentType<RegistrationFilterComponent> = RegistrationFilterComponent;
  public brokenLinkModule: BrokenLinkModuleIdentifier = BrokenLinkModuleIdentifier.Course;
  public registrationSearchText: string = '';
  public registrationFilterData: RegistrationFilterModel = null;
  public assignmentSearchText: string = '';
  public assignmentFilterData: unknown = null;
  public registrationFilter: IGridFilter = {
    search: '',
    filter: null
  };
  public assignmentFilter: IGridFilter = {
    search: '',
    filter: null
  };
  public course: Course = new Course();
  public paramsReportDynamic: IOpalReportDynamicParams | null;
  public breadCrumbItems: BreadcrumbItem[] = [];
  public participantSelectedItems: RegistrationViewModel[] = [];

  public get hasActionBtnGroup(): boolean {
    return this.actionBtnGroup.findIndex(x => x.actionFn != null && (x.hiddenFn == null || !x.hiddenFn())) > -1;
  }

  public actionBtnGroup: ButtonAction<RegistrationViewModel>[] = [
    {
      id: 'complete',
      text: this.translateCommon('Complete'),
      conditionFn: dataItem =>
        dataItem.learningStatus === RegistrationLearningStatus.Failed &&
        Registration.canManageRegistrations(this.course) &&
        Registration.hasManageRegistrationsPermission(this.currentUser, this.course, this._classRunVM.data),
      actionFn: dataItems => this.handleMassAction(RegistrationMassAction.Complete, dataItems),
      hiddenFn: () => this.selectedTab !== LMMTabConfiguration.ParticipantsTab
    },
    {
      id: 'incomplete',
      text: this.translateCommon('Incomplete'),
      conditionFn: dataItem =>
        dataItem.learningStatus === RegistrationLearningStatus.Completed &&
        Registration.canManageRegistrations(this.course) &&
        Registration.hasManageRegistrationsPermission(this.currentUser, this.course, this._classRunVM.data),
      actionFn: dataItems => this.handleMassAction(RegistrationMassAction.Incomplete, dataItems),
      hiddenFn: () => this.selectedTab !== LMMTabConfiguration.ParticipantsTab
    }
  ];

  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Submit for Approval',
      onClickFn: () => this.onActionApproval(ContextMenuAction.SubmitForApproval),
      shownIfFn: () => this.canSubmitContentClassRun()
    },
    {
      displayText: 'Approve',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Approve),
      shownIfFn: () => this.canApprovalContentClassRun()
    },
    {
      displayText: 'Reject',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Reject),
      shownIfFn: () => this.canApprovalContentClassRun()
    },
    {
      displayText: 'Publish',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Publish),
      shownIfFn: () => this.canPublishContentClassRun()
    },
    {
      displayText: 'Unpublish',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Unpublish),
      shownIfFn: () => this.canUnpublishContentClassRun()
    },
    {
      id: ContextMenuAction.NavigateToCSLCommunity,
      icon: 'hyperlink-open',
      displayText: this.translateCommon('Open Community'),
      shownInMoreFn: () => true,
      onClickFn: () => {
        window.open(WebAppLinkBuilder.buildCSLCommunityDetailForClassUrl(this.classRunVM.data), '_blank');
      },
      shownIfFn: () => this.classRunVM.data.communityCreated()
    }
  ];

  public ContextMenuAction: typeof ContextMenuAction = ContextMenuAction;
  public stickySpacing: number = SPACING_CONTENT;

  public get title(): string {
    return this.classRunVM.classTitle;
  }

  public get subTitle(): string {
    return this.classRunVM.classRunCode;
  }

  public get detailPageInput(): RouterPageInput<ClassRunDetailPageInput, LMMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public ClassRunStatus: typeof ClassRunStatus = ClassRunStatus;

  public set detailPageInput(v: RouterPageInput<ClassRunDetailPageInput, LMMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadClassRunInfo();
      }
    }
  }

  public get classRunVM(): ClassRunDetailViewModel {
    return this._classRunVM;
  }
  public set classRunVM(v: ClassRunDetailViewModel) {
    this._classRunVM = v;
  }

  public get commentTabInput(): CommentTabInput {
    return {
      originalObjectId: this.classRunVM.data.id,
      commentServiceType: CommentServiceType.Course,
      entityCommentType: EntityCommentType.ClassRunContent,
      mappingAction: COMMENT_ACTION_MAPPING,
      hasReply: true
    };
  }

  public get canFilterParticipant(): boolean {
    return this.selectedTab === LMMTabConfiguration.ParticipantsTab;
  }

  public classRunContents: CourseContentItemModel[] = [];
  public classRunStatusColorMap = CONTENT_STATUS_COLOR_MAP;
  public navigationData: NavigationData;
  public loadingData: boolean = false;
  public loadingClassRunContents: boolean = false;
  public completionRate: number;
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public SearchLearnerProfileType: typeof SearchLearnerProfileType = SearchLearnerProfileType;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public participantsGridDisplayColumns: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.selected,
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.learningContentProgress,
    ListRegistrationGridDisplayColumns.noOfAssignmentDone,
    ListRegistrationGridDisplayColumns.attendanceRatioOfPresent,
    ListRegistrationGridDisplayColumns.serviceScheme,
    ListRegistrationGridDisplayColumns.developmentalRole,
    ListRegistrationGridDisplayColumns.status,
    ListRegistrationGridDisplayColumns.actions,
    ListRegistrationGridDisplayColumns.postCourseEvaluationFormCompleted
  ];

  private _detailPageInput: RouterPageInput<ClassRunDetailPageInput, LMMTabConfiguration, unknown> = NAVIGATORS[
    LMMRoutePaths.ClassRunDetailPage
  ] as RouterPageInput<ClassRunDetailPageInput, LMMTabConfiguration, unknown>;
  private _loadClassRunInfoSub: Subscription = new Subscription();
  private _loadClassRunContentsSub: Subscription = new Subscription();
  private _loadCompletedRateSub: Subscription = new Subscription();
  private _classRunVM: ClassRunDetailViewModel = new ClassRunDetailViewModel();
  public get selectedTab(): LMMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : LMMTabConfiguration.ClassRunInfoTab;
  }
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private courseRepository: CourseRepository,
    private classRunRepository: ClassRunRepository,
    private userRepository: UserRepository,
    private navigationPageService: NavigationPageService,
    private opalDialogService: OpalDialogService,
    private registrationRepository: RegistrationRepository,
    private breadcrumbService: BreadcrumbService,
    private learningContentRepository: LearningContentRepository,
    private commentApiService: CommentApiService
  ) {
    super(moduleFacadeService);
    this.commentApiService.initApiService(CommentServiceType.Course);
  }

  public get getSelectedItemsForMassAction(): RegistrationViewModel[] {
    if (this.selectedTab === LMMTabConfiguration.ParticipantsTab) {
      return this.participantSelectedItems;
    }
    return [];
  }

  public handleMassAction(massAction: RegistrationMassAction, dataItems: RegistrationViewModel[]): Promise<boolean> {
    let massActionPromise: Promise<boolean>;
    switch (massAction) {
      case RegistrationMassAction.Complete:
        massActionPromise = this.completeOrIncompleteRegistration(dataItems, true, false);
        break;
      case RegistrationMassAction.Incomplete:
        massActionPromise = this.completeOrIncompleteRegistration(dataItems, false, false);
        break;
    }
    return massActionPromise.then(_ => {
      this.resetSelectedItems();
      return _;
    });
  }

  public resetSelectedItems(): void {
    this.participantSelectedItems = [];
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = classRunDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public onSubmitSearch(): void {
    this.registrationFilter = {
      ...this.registrationFilter,
      search: this.registrationSearchText
    };
  }

  public loadClassRunInfo(): void {
    this.loadClassRun();
    this.loadClassRunContent();
    this.loadCompletionRate();
  }

  public loadClassRunContent(): void {
    this._loadClassRunContentsSub.unsubscribe();
    if (this.detailPageInput.data.id) {
      this.loadingClassRunContents = true;
      this._loadClassRunContentsSub = this.learningContentRepository
        .getTableOfContents(this.detailPageInput.data.courseId, this.detailPageInput.data.id)
        .pipe(this.untilDestroy())
        .subscribe(toc => {
          this.classRunContents = toc;
          this.loadingClassRunContents = false;
        });
    }
  }

  public loadCompletionRate(): void {
    this._loadCompletedRateSub.unsubscribe();
    this._loadCompletedRateSub = this.classRunRepository
      .getCompletionRate(this.detailPageInput.data.id)
      .pipe(this.untilDestroy())
      .subscribe((data: number) => {
        this.completionRate = data;
      });
  }

  public loadClassRun(): void {
    this._loadClassRunInfoSub.unsubscribe();
    const courseObs: Observable<Course | null> =
      this.detailPageInput.data.courseId != null ? this.courseRepository.loadCourse(this.detailPageInput.data.courseId) : of(null);
    const classRunObs =
      this.detailPageInput.data.courseId != null && this.detailPageInput.data.id != null
        ? this.classRunRepository.loadClassRunById(this.detailPageInput.data.id, true)
        : of(null);
    this.loadingData = true;
    this._loadClassRunInfoSub = combineLatest(courseObs, classRunObs)
      .pipe(
        switchMap(([course, classRun]) => {
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
          ).pipe(map(classRunVm => <[Course, ClassRunDetailViewModel]>[course, classRunVm]));
        }),
        this.untilDestroy()
      )
      .subscribe(
        ([course, classRunVm]) => {
          this.course = course;
          this.classRunVM = classRunVm;
          this.loadBreadcrumb();
          this.setParamsReportDynamic();
          this.initFormData();
          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(
      () => (this.contentDetailPage ? this.contentDetailPage.hasDataChanged() : false),
      () => this.contentDetailPage.saveContentData()
    );
  }

  public canViewParticipants(): boolean {
    return (
      this.classRunVM.data.isNotCancelled() &&
      (Registration.hasViewManagedRegistrationsPermission(this.currentUser, this.course, this.classRunVM.data) ||
        this.classRunVM.data.hasViewParticipantPermission(this.currentUser))
    );
  }

  public canViewCompletionRate(): boolean {
    return this.classRunVM.data.hasViewCompletionRatePermission(this.currentUser);
  }

  public canViewFeedback(): boolean {
    return this.classRunVM.data.isNotCancelled() && this.classRunVM.data.started();
  }

  public canViewAttendanceTracking(): boolean {
    return (
      this.classRunVM.data.isNotCancelled() &&
      this.classRunVM.data.started() &&
      Registration.hasViewManagedRegistrationsPermission(this.currentUser, this.course, this.classRunVM.data)
    );
  }

  public canViewAnnouncement(): boolean {
    return (
      this.classRunVM.data.isNotCancelled() &&
      this.classRunVM.data.started() &&
      Registration.canManageAnnouncement(this.currentUser, this.course, this.classRunVM.data)
    );
  }

  public canApprovalContentClassRun(): boolean {
    return (
      this.detailPageInput.data.mode === ClassRunDetailMode.ForApprover &&
      this.classRunVM.data.contentStatus === ContentStatus.PendingApproval &&
      this.selectedTab === LMMTabConfiguration.ClassRunContentTab
    );
  }

  public canPublishContentClassRun(): boolean {
    return (
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.classRunVM.data.canPublishContent(this.course) &&
      this.selectedTab === LMMTabConfiguration.ClassRunContentTab
    );
  }

  public canUnpublishContentClassRun(): boolean {
    return (
      this.detailPageInput.data.mode === ClassRunDetailMode.View &&
      this.classRunVM.data.canUnpublishContent(this.course) &&
      this.selectedTab === LMMTabConfiguration.ClassRunContentTab
    );
  }

  public canViewComment(): boolean {
    return this.detailPageInput.data.mode === ClassRunDetailMode.ForApprover || this.detailPageInput.data.mode === ClassRunDetailMode.View;
  }

  public checkCommentsHasDataFnCreator(): () => Observable<boolean> {
    return () => {
      return from(
        this.commentApiService.getCommentNotSeen({
          objectIds: [this.detailPageInput.data.id],
          entityCommentType: EntityCommentType.ClassRunContent
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

  public onActionApproval(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.SubmitForApproval:
        this.changeClassRunContentStatus(this.classRunVM.data, ContentStatus.PendingApproval);
        break;
      case ContextMenuAction.Approve:
        this.showApprovalDialog(
          {
            title: `${this.translate('Approve Content Class')}: ${this.course.courseName}`,
            requiredCommentField: false
          },
          ContentStatus.Approved
        );
        break;
      case ContextMenuAction.Reject:
        this.showApprovalDialog(
          {
            title: `${this.translate('Reject Content Class')}: ${this.course.courseName}`
          },
          ContentStatus.Rejected
        );
        break;
      case ContextMenuAction.Publish:
        this.changeClassRunContentStatus(this.classRunVM.data, ContentStatus.Published);
        break;
      case ContextMenuAction.Unpublish:
        this.changeClassRunContentStatus(this.classRunVM.data, ContentStatus.Unpublished);
        break;
      default:
        break;
    }
  }

  public canSubmitContentClassRun(): boolean {
    return (
      this.classRunContents.length > 0 &&
      this.course.hasContentCreatorPermission(this.currentUser) &&
      (this.classRunVM.data.contentStatus === ContentStatus.Draft || this.classRunVM.data.contentStatus === ContentStatus.Rejected) &&
      this.selectedTab === LMMTabConfiguration.ClassRunContentTab
    );
  }

  public canViewSessions(): boolean {
    return (
      (this.detailPageInput.data.mode === ClassRunDetailMode.View &&
        this.currentUser &&
        (this.course.hasContentCreatorPermission(this.currentUser) ||
          this.course.hasAdministrationPermission(this.currentUser) ||
          this.course.hasApprovalPermission(this.currentUser))) ||
      this.course.hasFacilitatorsPermission(this.currentUser)
    );
  }

  public onViewAssignment(assignment: AssignmentViewModel): void {
    this.navigationPageService.navigateTo(
      LMMRoutePaths.AssignmentDetailPage,
      {
        activeTab: LMMTabConfiguration.AssignmentInfoTab,
        data: {
          mode: AssignmentDetailMode.View,
          assignmentId: assignment.id,
          courseId: this.detailPageInput.data.courseId,
          classRunId: this.detailPageInput.data.id
        }
      },
      this.detailPageInput
    );
  }

  public displayStatus(): ContentStatus {
    return this.classRunVM.data.contentStatus;
  }

  public displayStatusPrefix(): string {
    if (ClassRun.isContentResubmit(this.classRunVM.data, this.course)) {
      return '(Resubmit)';
    }
    return '';
  }

  public onGridContextMenuSelected(contextMenuEmit: ContextMenuEmit<RegistrationViewModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Completed:
        this.completeOrIncompleteRegistration([contextMenuEmit.dataItem], true);
        break;
      case ContextMenuAction.Incomplete:
        this.completeOrIncompleteRegistration([contextMenuEmit.dataItem], false);
        break;
    }
  }

  public completeOrIncompleteRegistration(
    registrations: RegistrationViewModel[],
    isCompleted: boolean,
    showNotification: boolean = true
  ): Promise<boolean> {
    return this.registrationRepository
      .completeOrIncompleteRegistration({
        courseId: this.detailPageInput.data.courseId,
        classRunId: this.detailPageInput.data.id,
        registrationIds: registrations.map(p => p.id),
        isCompleted: isCompleted
      })
      .then(_ => {
        if (showNotification) {
          this.showNotification();
        }
        return true;
      });
  }

  public onViewRegistration(
    dataItem: RegistrationViewModel,
    activeTab: LMMTabConfiguration,
    searchType: SearchLearnerProfileType.Participant
  ): void {
    this.navigationPageService.navigateTo(
      LMMRoutePaths.LearnerProfilePage,
      {
        activeTab: LMMTabConfiguration.PersonalInfoTab,
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

  public viewAttendanceTracking(dataItem: AttendanceTrackingViewModel): void {
    this.navigationPageService.navigateTo(
      LMMRoutePaths.LearnerProfilePage,
      {
        activeTab: LMMTabConfiguration.PersonalInfoTab,
        data: {
          attendanceTrackingId: dataItem.id,
          userId: dataItem.userId,
          courseId: this.detailPageInput.data.courseId,
          classRunId: this.detailPageInput.data.id,
          searchType: SearchLearnerProfileType.AttendanceTracking
        }
      },
      this.detailPageInput
    );
  }

  public setParamsReportDynamic(): void {
    this.paramsReportDynamic = OpalReportDynamicComponent.buildCourseCompletion(this.classRunVM.courseId, this.detailPageInput.data.id);
  }

  public resetFilterAssignment(): void {
    this.assignmentFilter = {
      ...this.assignmentFilter,
      search: this.assignmentSearchText
    };
  }

  public resetFilterParticipant(): void {
    this.registrationFilter = {
      ...this.registrationFilter,
      search: this.registrationSearchText
    };
  }

  public onApplyFilterParticipant(data: RegistrationFilterModel): void {
    this.registrationFilterData = data;

    this.registrationFilter = {
      ...this.registrationFilter,
      filter: this.registrationFilterData ? this.registrationFilterData.convert() : null
    };
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadClassRunInfo();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'classrun-detail',
      controls: {
        classTitle: {
          defaultValue: this.classRunVM.classTitle,
          validators: null
        },
        classRunCode: {
          defaultValue: this.classRunVM.classRunCode,
          validators: null
        },
        startDate: {
          defaultValue: this.classRunVM.startDate,
          validators: null
        },
        endDate: {
          defaultValue: this.classRunVM.endDate,
          validators: null
        },
        planningStartTime: {
          defaultValue: this.classRunVM.planningStartTime,
          validators: null
        },
        planningEndTime: {
          defaultValue: this.classRunVM.planningEndTime,
          validators: null
        },
        facilitatorIds: {
          defaultValue: this.classRunVM.data.facilitatorIds,
          validators: null
        },
        coFacilitatorIds: {
          defaultValue: this.classRunVM.data.coFacilitatorIds,
          validators: null
        },
        minClassSize: {
          defaultValue: this.classRunVM.minClassSize,
          validators: null
        },
        maxClassSize: {
          defaultValue: this.classRunVM.maxClassSize,
          validators: null
        },
        applicationStartDate: {
          defaultValue: this.classRunVM.applicationStartDate,
          validators: null
        },
        applicationEndDate: {
          defaultValue: this.classRunVM.applicationEndDate,
          validators: null
        }
      }
    };
  }

  private showApprovalDialog(input: Partial<CommentDialogComponent>, contentStatus: ContentStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    this.subscribe(dialogRef.result, (data: IDialogActionEvent) => {
      if (data.action === DialogAction.OK) {
        this.changeClassRunContentStatus(this.classRunVM.data, contentStatus, data.comment);
      }
    });
  }

  private changeClassRunContentStatus(classRun: ClassRun, contentStatus: ContentStatus, comment: string = ''): void {
    this.subscribe(
      this.learningContentRepository.changeClassRunContentStatus({ ids: [classRun.id], contentStatus: contentStatus, comment: comment }),
      () => {
        this.showNotification();
        this.navigationPageService.navigateBack();
      }
    );
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<ClassRunDetailPageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      LMM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p =>
          this.navigationPageService.navigateByRouter(
            p,
            () => (this.contentDetailPage ? this.contentDetailPage.hasDataChanged() : false),
            () => from(this.contentDetailPage.saveContentData())
          ),
        {
          [LMMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName },
          [LMMRoutePaths.ClassRunDetailPage]: { textFn: () => this.classRunVM.classTitle }
        }
      )
    );
  }
}

export enum RegistrationMassAction {
  Complete = 'complete',
  Incomplete = 'incomplete'
}

export const classRunDetailPageTabIndexMap = {
  0: LMMTabConfiguration.ClassRunInfoTab,
  1: LMMTabConfiguration.ClassRunContentTab,
  2: LMMTabConfiguration.SessionsTab,
  3: LMMTabConfiguration.ParticipantsTab,
  4: LMMTabConfiguration.AssignmentsTab,
  5: LMMTabConfiguration.FeedbackTab,
  6: LMMTabConfiguration.AttendanceTrackingTab,
  7: LMMTabConfiguration.ClassRunCommentTab,
  8: LMMTabConfiguration.ReportBrokenLinkTab,
  9: LMMTabConfiguration.AnnouncementTab
};
