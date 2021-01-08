import { BaseFormComponent, IFilter, IFormBuilderDefinition, ModuleFacadeService, NotificationType, Utils } from '@opal20/infrastructure';
import {
  BreadcrumbItem,
  BreadcrumbService,
  CONTENT_STATUS_COLOR_MAP,
  CommentDialogComponent,
  CommentTabInput,
  ContextMenuAction,
  CourseDetailMode,
  CourseDetailViewModel,
  IDialogActionEvent,
  LMMRoutePaths,
  LMMTabConfiguration,
  ListClassRunGridComponentService,
  NavigationData,
  NavigationPageService,
  RouterPageInput,
  SelectCourseDialogComponent,
  SelectCourseModel,
  WebAppLinkBuilder
} from '@opal20/domain-components';
import {
  BrokenLinkModuleIdentifier,
  COMMENT_ACTION_MAPPING,
  ClassRunRepository,
  CommentApiService,
  CommentServiceType,
  ContentStatus,
  Course,
  CourseContentItemModel,
  CoursePlanningCycleRepository,
  CourseRepository,
  CourseStatus,
  CourseType,
  ECertificateRepository,
  EntityCommentType,
  FormRepository,
  FormStatus,
  FormType,
  LearningContentRepository,
  OrganizationRepository,
  SearchClassRunType,
  TaggingRepository,
  UserInfoModel,
  UserRepository
} from '@opal20/domain-api';
import { ButtonGroupButton, DialogAction, OpalDialogService, SPACING_CONTENT } from '@opal20/common-components';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { map, switchMap, take } from 'rxjs/operators';

import { Constant } from '@opal20/authentication';
import { ContentDetailPageComponent } from './content-detail-page.component';
import { CourseDetailPageInput } from '../models/course-detail-page-input.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { LMM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/lmm-route-breadcumb-mapping-fn';
import { NAVIGATORS } from '../lmm.config';
import { SelectEvent } from '@progress/kendo-angular-layout';

@Component({
  selector: 'course-detail-page',
  templateUrl: './course-detail-page.component.html'
})
export class CourseDetailPageComponent extends BaseFormComponent {
  @ViewChild(ContentDetailPageComponent, { static: false }) public contentDetailPage: ContentDetailPageComponent;
  public brokenLinkModule: BrokenLinkModuleIdentifier = BrokenLinkModuleIdentifier.Course;

  public breadCrumbItems: BreadcrumbItem[] = [];
  public stickySpacing: number = SPACING_CONTENT;
  public get title(): string {
    return this.course.courseName;
  }

  public get subTitle(): string {
    return this.course.courseCode;
  }

  public get detailPageInput(): RouterPageInput<CourseDetailPageInput, LMMTabConfiguration, LMMTabConfiguration> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<CourseDetailPageInput, LMMTabConfiguration, LMMTabConfiguration> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadCourseInfo();
      }
    }
  }

  public get commentTabInput(): CommentTabInput {
    return {
      originalObjectId: this.detailPageInput.data.id,
      commentServiceType: CommentServiceType.Course,
      entityCommentType: EntityCommentType.CourseContent,
      mappingAction: COMMENT_ACTION_MAPPING,
      hasReply: true
    };
  }

  public get course(): CourseDetailViewModel | null {
    return this._course;
  }
  public set course(v: CourseDetailViewModel) {
    this._course = v;
  }

  public courseStatusColorMap = CONTENT_STATUS_COLOR_MAP;
  public navigationData: NavigationData;
  public loadingCourseVmData: boolean = false;
  public loadingCourseContents: boolean = false;

  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  public ContextMenuAction: typeof ContextMenuAction = ContextMenuAction;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public CourseStatus: typeof CourseStatus = CourseStatus;
  public ContentStatus: typeof ContentStatus = ContentStatus;
  public courseContents: CourseContentItemModel[] = [];
  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Duplicate',
      onClickFn: () => this.duplicateCourseContent(),
      shownIfFn: () => this.canDuplicateCourseContent()
    },
    {
      displayText: 'Submit for Approval',
      onClickFn: () => this.onActionApproval(ContextMenuAction.SubmitForApproval),
      shownIfFn: () => this.canSubmitContentCourse()
    },
    {
      displayText: 'Approve',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Approve),
      shownIfFn: () => this.canApproveOrRejectContentCourse()
    },
    {
      displayText: 'Reject',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Reject),
      shownIfFn: () => this.canApproveOrRejectContentCourse()
    },
    {
      displayText: 'Publish',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Publish),
      shownIfFn: () => this.canPublishContentCourse()
    },
    {
      displayText: 'Unpublish',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Unpublish),
      shownIfFn: () => this.canUnpublishContentCourse()
    },
    {
      id: ContextMenuAction.NavigateToCSLCommunity,
      icon: 'hyperlink-open',
      displayText: this.translateCommon('Open Community'),
      shownInMoreFn: () => true,
      onClickFn: () => {
        window.open(WebAppLinkBuilder.buildCSLCommunityDetailForCourseUrl(this.course.courseData), '_blank');
      },
      shownIfFn: () => this.course.courseData.communityCreated()
    }
  ];
  private _detailPageInput: RouterPageInput<CourseDetailPageInput, LMMTabConfiguration, LMMTabConfiguration> = NAVIGATORS[
    LMMRoutePaths.CourseDetailPage
  ] as RouterPageInput<CourseDetailPageInput, LMMTabConfiguration, LMMTabConfiguration>;

  private _loadCourseInfoSub: Subscription = new Subscription();
  private _loadCourseContentsSub: Subscription = new Subscription();
  private _course: CourseDetailViewModel | null = new CourseDetailViewModel(new Course(), {}, [], [], {}, []);
  private currentUser = UserInfoModel.getMyUserInfo();
  public get selectedTab(): LMMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : LMMTabConfiguration.CourseInfoTab;
  }
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public listClassRunGridComponentService: ListClassRunGridComponentService,
    private courseRepository: CourseRepository,
    private classRunRepository: ClassRunRepository,
    private taggingRepository: TaggingRepository,
    private userRepository: UserRepository,
    private organizationRepository: OrganizationRepository,
    private navigationPageService: NavigationPageService,
    private opalDialogService: OpalDialogService,
    private formRepository: FormRepository,
    private coursePlanningCycleRepository: CoursePlanningCycleRepository,
    private breadcrumbService: BreadcrumbService,
    private ecertificateRepository: ECertificateRepository,
    private learningContentRepository: LearningContentRepository,
    private commentApiService: CommentApiService
  ) {
    super(moduleFacadeService);
    this.commentApiService.initApiService(CommentServiceType.Course);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public loadCourseInfo(): void {
    this.loadCourse();
    this.loadCourseContent();
  }

  public loadCourseContent(): void {
    this._loadCourseContentsSub.unsubscribe();
    if (this.detailPageInput.data.id) {
      this.loadingCourseContents = true;
      this._loadCourseContentsSub = this.learningContentRepository
        .getTableOfContents(this.detailPageInput.data.id)
        .pipe(this.untilDestroy())
        .subscribe(toc => {
          this.courseContents = toc;
          this.loadingCourseContents = false;
        });
    }
  }

  public loadCourse(): void {
    this._loadCourseInfoSub.unsubscribe();
    const courseObs: Observable<Course | null> =
      this.detailPageInput.data.id != null ? this.courseRepository.loadCourse(this.detailPageInput.data.id) : of(null);
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    const formObs = this.formRepository.searchForm(
      [FormStatus.Published],
      FormType.Survey,
      CourseDetailViewModel.formsSurveyTypes,
      0,
      Constant.MAX_ITEMS_PER_REQUEST,
      null,
      true
    );

    this.loadingCourseVmData = true;
    this._loadCourseInfoSub = combineLatest(courseObs, taggingObs, formObs)
      .pipe(
        switchMap(([course, metadatas, formResult]) => {
          return CourseDetailViewModel.create(
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
            ids => this.courseRepository.loadCourses(ids),
            ids => this.organizationRepository.loadOrganizationalUnitsByIds(ids, true),
            coursePlanningCycleId => this.coursePlanningCycleRepository.loadCoursePlanningCycleById(coursePlanningCycleId),
            ecertificateTemplateId => this.ecertificateRepository.getECertificateTemplateById(ecertificateTemplateId),
            course,
            metadatas,
            formResult.items,
            course.coursePlanningCycleId
          ).pipe(map(courseVm => <[CourseDetailViewModel, Course]>[courseVm, course]));
        })
      )
      .pipe(this.untilDestroy())
      .subscribe(
        ([courseVm, course]) => {
          this.course = courseVm;
          this.loadBreadcrumb();
          if (course == null) {
            this.course.courseType = this.detailPageInput.data.mode === CourseDetailMode.Recurring ? CourseType.Recurring : CourseType.New;
          }
          this.loadingCourseVmData = false;
        },
        () => {
          this.loadingCourseVmData = false;
        }
      );
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(
      () => (this.contentDetailPage ? this.contentDetailPage.hasDataChanged() : false),
      () => this.contentDetailPage.saveContentData()
    );
  }

  public checkClassRunDataHasPendingApprovalStatusFnCreator(): () => Observable<boolean> {
    const filterClassRun: IFilter = {
      containFilters: [
        {
          field: 'ContentStatus',
          values: [ContentStatus.PendingApproval],
          notContain: false
        }
      ],
      fromToFilters: []
    };
    return () => {
      return this.listClassRunGridComponentService
        .loadClassRunsByCourseId(
          this.detailPageInput.data.id,
          SearchClassRunType.LearningManagement,
          '',
          filterClassRun,
          false,
          false,
          0,
          0
        )
        .pipe(
          map(data => {
            if (data != null) {
              return this.detailPageInput.data.mode === CourseDetailMode.ForApprover && data.total > 0;
            }
            return false;
          })
        );
    };
  }

  public getCourseAndClassrunIds(showSpinner: boolean): Promise<string[]> {
    const classRunsObs: Observable<string[]> =
      this.detailPageInput.data.id != null
        ? this.classRunRepository
            .loadClassRunsByCourseId(
              this.detailPageInput.data.id,
              SearchClassRunType.LearningManagement,
              '',
              null,
              false,
              false,
              0,
              Constant.MAX_ITEMS_PER_REQUEST,
              null,
              null,
              showSpinner
            )
            .pipe(
              this.untilDestroy(),
              take(1),
              map(_ => {
                return [this.detailPageInput.data.id].concat(_.items.map(p => p.id));
              })
            )
        : of([]);

    return classRunsObs.toPromise();
  }

  public canViewClassRun(): boolean {
    return (
      (this.detailPageInput.data.mode === CourseDetailMode.ForApprover ||
        this.detailPageInput.data.mode === CourseDetailMode.View ||
        this.detailPageInput.data.mode === CourseDetailMode.EditContent) &&
      this.course.courseData.canViewClassRun()
    );
  }

  public canViewContent(): boolean {
    return this.course.courseData.canViewContent();
  }

  public canViewStatistic(): boolean {
    return (
      this.course.courseData.afterPublishing() &&
      (this.course.courseData.postCourseEvaluationFormId != null || this.course.courseData.preCourseEvaluationFormId != null)
    );
  }

  public canViewComment(): boolean {
    return (
      this.detailPageInput.data.mode === CourseDetailMode.ForApprover ||
      this.detailPageInput.data.mode === CourseDetailMode.View ||
      this.detailPageInput.data.mode === CourseDetailMode.EditContent
    );
  }

  public checkCommentsHasDataFnCreator(): () => Observable<boolean> {
    return () => {
      return from(
        this.commentApiService.getCommentNotSeen({
          objectIds: [this.detailPageInput.data.id],
          entityCommentType: EntityCommentType.CourseContent
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

  public canViewFeedback(): boolean {
    return this.course.isMicrolearning;
  }

  public canSubmitContentCourse(): boolean {
    return (
      this.courseContents.length > 0 &&
      this.course.courseData.hasContentCreatorPermission(this.currentUser) &&
      (this.course.courseData.contentStatus === ContentStatus.Draft || this.course.courseData.contentStatus === ContentStatus.Rejected) &&
      this.selectedTab === LMMTabConfiguration.CourseContentTab
    );
  }

  public canApproveOrRejectContentCourse(): boolean {
    return (
      this.detailPageInput.data.mode === CourseDetailMode.ForApprover &&
      this.selectedTab === LMMTabConfiguration.CourseContentTab &&
      this.course.courseData.hasApproveRejectCourseContentPermission(this.currentUser) &&
      this.course.courseData.canApproveRejectCourseContent()
    );
  }

  public canPublishContentCourse(): boolean {
    return (
      this.detailPageInput.data.mode !== CourseDetailMode.ForApprover &&
      this.course.courseData.hasPublishCourseContentPermission(this.currentUser) &&
      this.course.courseData.canPublishContent(this.currentUser) &&
      this.selectedTab === LMMTabConfiguration.CourseContentTab
    );
  }

  public canUnpublishContentCourse(): boolean {
    return (
      this.detailPageInput.data.mode !== CourseDetailMode.ForApprover &&
      this.course.courseData.canUnpublishContent(this.currentUser) &&
      this.selectedTab === LMMTabConfiguration.CourseContentTab
    );
  }

  public canDuplicateCourseContent(): boolean {
    return this.course.courseData.canUserEditContent(this.currentUser) && this.selectedTab === LMMTabConfiguration.CourseContentTab;
  }

  public duplicateCourseContent(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
      SelectCourseDialogComponent,
      SelectCourseDialogComponent.selectToCloneCourseContentsConfig(this.detailPageInput.data.id)
    );
    this.subscribe(dialogRef.result, (data: SelectCourseModel) => {
      const fromCourseId = data.id;
      if (fromCourseId) {
        this.learningContentRepository
          .cloneContentForCourse({
            fromCourseId: fromCourseId,
            toCourseId: this.detailPageInput.data.id
          })
          .then(_ => {
            this.showNotification();

            if (this.courseContents.length === _.length) {
              this.showNotification(this.translate('No course content is duplicated.'), NotificationType.Warning);
            }
          });
      }
    });
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = courseDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public onActionApproval(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.SubmitForApproval:
        this.changeCourseContentStatus(this.course.courseData, ContentStatus.PendingApproval);
        break;
      case ContextMenuAction.Approve:
        this.showApprovalDialog(
          {
            title: `${this.translate('Approve Content Course')}: ${this.course.courseName}`,
            requiredCommentField: false
          },
          ContentStatus.Approved
        );
        break;
      case ContextMenuAction.Reject:
        this.showApprovalDialog(
          {
            title: `${this.translate('Reject Content Course')}: ${this.course.courseName}`
          },
          ContentStatus.Rejected
        );
        break;
      case ContextMenuAction.Publish:
        this.changeCourseContentStatus(this.course.courseData, ContentStatus.Published);
        break;
      case ContextMenuAction.Unpublish:
        this.changeCourseContentStatus(this.course.courseData, ContentStatus.Unpublished);
        break;
      default:
        break;
    }
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadCourseInfo();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      // Overview Info
      formName: 'overview-info',
      controls: {
        thumbnailUrl: {
          defaultValue: null,
          validators: null
        },
        courseName: {
          defaultValue: null,
          validators: null
        },
        pdActivityType: {
          defaultValue: null,
          validators: null
        },
        durationHours: {
          defaultValue: null,
          validators: null
        },
        durationMinutes: {
          defaultValue: null,
          validators: null
        },
        categoryIds: {
          defaultValue: null,
          validators: null
        },
        learningMode: {
          defaultValue: null,
          validators: null
        },
        courseCode: {
          defaultValue: null,
          validators: null
        },
        externalCode: {
          defaultValue: null,
          validators: null
        },
        courseOutlineStructure: {
          defaultValue: null,
          validators: null
        },
        courseObjective: {
          defaultValue: null,
          validators: null
        },
        description: {
          defaultValue: null,
          validators: null
        },
        // Provider Info,
        trainingAgency: {
          defaultValue: null,
          validators: null
        },
        otherTrainingAgencyReason: {
          defaultValue: null,
          validators: null
        },
        nieAcademicGroups: {
          defaultValue: null,
          validators: null
        },
        ownerDivisionIds: {
          defaultValue: null,
          validators: null
        },
        ownerBranchIds: {
          defaultValue: null,
          validators: null
        },
        partnerOrganisationIds: {
          defaultValue: null,
          validators: null
        },
        moeOfficerId: {
          defaultValue: null,
          validators: null
        },
        moeOfficerPhoneNumber: {
          defaultValue: null,
          validators: null
        },
        moeOfficerEmail: {
          defaultValue: null,
          validators: null
        },
        notionalCost: {
          defaultValue: null,
          validators: null
        },
        courseFee: {
          defaultValue: null,
          validators: null
        },

        // Metadata
        serviceSchemeIds: {
          defaultValue: null,
          validators: null
        },
        learningFrameworkIds: {
          defaultValue: null,
          validators: null
        },
        learningDimensionIds: {
          defaultValue: null,
          validators: null
        },
        learningAreaIds: {
          defaultValue: null,
          validators: null
        },
        learningSubAreaIds: {
          defaultValue: null,
          validators: null
        },
        subjectAreaIds: {
          defaultValue: null,
          validators: null
        },
        pdAreaThemeId: {
          defaultValue: null,
          validators: null
        },
        courseLevel: {
          defaultValue: null,
          validators: null
        },
        metadataKeys: {
          defaultValue: null,
          validators: null
        },
        teacherOutcomeIds: {
          defaultValue: null,
          validators: null
        },
        // Copyright
        allowPersonalDownload: {
          defaultValue: null,
          validators: null
        },
        allowNonCommerInMoeReuseWithoutModification: {
          defaultValue: null,
          validators: null
        },
        allowNonCommerReuseWithoutModification: {
          defaultValue: null,
          validators: null
        },
        allowNonCommerInMOEReuseWithModification: {
          defaultValue: null,
          validators: null
        },
        allowNonCommerReuseWithModification: {
          defaultValue: null,
          validators: null
        },
        copyrightOwner: {
          defaultValue: null,
          validators: null
        },
        acknowledgementAndCredit: {
          defaultValue: null,
          validators: null
        },
        remarks: {
          defaultValue: null,
          validators: null
        },
        // Target Audience
        trackIds: {
          defaultValue: null,
          validators: null
        },
        developmentalRoleIds: {
          defaultValue: null,
          validators: null
        },
        teachingLevels: {
          defaultValue: null,
          validators: null
        },
        teachingCourseStudyIds: {
          defaultValue: null,
          validators: null
        },
        placeOfWork: {
          defaultValue: null,
          validators: null
        },
        applicableDivisionIds: {
          defaultValue: null,
          validators: null
        },
        applicableBranchIds: {
          defaultValue: null,
          validators: null
        },
        applicableZoneIds: {
          defaultValue: null,
          validators: null
        },
        applicableClusterIds: {
          defaultValue: null,
          validators: null
        },
        applicableSchoolIds: {
          defaultValue: null,
          validators: null
        },
        registrationMethod: {
          defaultValue: null,
          validators: null
        },
        maximumPlacesPerSchool: {
          defaultValue: null,
          validators: null
        },
        prerequisiteCourseIds: {
          defaultValue: null,
          validators: null
        },
        numOfSchoolLeader: {
          defaultValue: null,
          validators: null
        },
        numOfSeniorOrLeadTeacher: {
          defaultValue: null,
          validators: null
        },
        numOfMiddleManagement: {
          defaultValue: null,
          validators: null
        },
        numOfBeginningTeacher: {
          defaultValue: null,
          validators: null
        },
        numOfExperiencedTeacher: {
          defaultValue: null,
          validators: null
        },
        teachingSubjectIds: {
          defaultValue: null,
          validators: null
        },
        jobFamily: {
          defaultValue: null,
          validators: null
        },
        cocurricularActivityIds: {
          defaultValue: null,
          validators: null
        },
        easSubstantiveGradeBandingIds: {
          defaultValue: null,
          validators: null
        },
        // Course Planning
        natureOfCourse: {
          defaultValue: null,
          validators: null
        },
        numOfPlannedClass: {
          defaultValue: null,
          validators: null
        },
        numOfSessionPerClass: {
          defaultValue: null,
          validators: null
        },
        numOfHoursPerSession: {
          defaultValue: null,
          validators: null
        },
        numOfMinutesPerSession: {
          defaultValue: null,
          validators: null
        },
        planningPublishDate: {
          defaultValue: null,
          validators: null
        },
        startDate: {
          defaultValue: null,
          validators: null
        },
        expiredDate: {
          defaultValue: null,
          validators: null
        },
        planningArchiveDate: {
          defaultValue: null,
          validators: null
        },
        pdActivityPeriods: {
          defaultValue: null,
          validators: null
        },
        minParticipantPerClass: {
          defaultValue: null,
          validators: null
        },
        maxParticipantPerClass: {
          defaultValue: null,
          validators: null
        },
        maxReLearningTimes: {
          defaultValue: null,
          validators: null
        },
        // Evaluation And ECertificate
        preCourseEvaluationFormId: {
          defaultValue: null,
          validators: null
        },
        postCourseEvaluationFormId: {
          defaultValue: null,
          validators: null
        },
        eCertificateTemplateId: {
          defaultValue: null,
          validators: null
        },
        eCertificatePrerequisite: {
          defaultValue: null,
          validators: null
        },
        courseNameInECertificate: {
          defaultValue: null,
          validators: null
        },
        // Administration
        firstAdministratorId: {
          defaultValue: null,
          validators: null
        },
        secondAdministratorId: {
          defaultValue: null,
          validators: null
        },
        primaryApprovingOfficerId: {
          defaultValue: null,
          validators: null
        },
        alternativeApprovingOfficerId: {
          defaultValue: null,
          validators: null
        },
        courseFacilitatorId: {
          defaultValue: null,
          validators: null
        },
        courseCoFacilitatorId: {
          defaultValue: null,
          validators: null
        },
        collaborativeContentCreatorIds: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }

  private showApprovalDialog(input: unknown, contentStatus: ContentStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    this.subscribe(dialogRef.result, (data: IDialogActionEvent) => {
      if (data.action === DialogAction.OK) {
        this.changeCourseContentStatus(this.course.courseData, contentStatus, data.comment);
      }
    });
  }

  private changeCourseContentStatus(course: Course, contentStatus: ContentStatus, comment: string = ''): void {
    this.subscribe(
      this.learningContentRepository.changeCourseContentStatus({ ids: [course.id], contentStatus: contentStatus, comment: comment }),
      () => {
        this.showNotification();
        this.navigationPageService.navigateBack();
      }
    );
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<CourseDetailPageInput, LMMTabConfiguration, LMMTabConfiguration> = this.getNavigateData();
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
            () => this.contentDetailPage.saveContentData()
          ),
        {
          [LMMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName }
        }
      )
    );
  }
}

export const courseDetailPageTabIndexMap = {
  0: LMMTabConfiguration.CourseInfoTab,
  1: LMMTabConfiguration.CourseContentTab,
  2: LMMTabConfiguration.ClassRunsTab,
  3: LMMTabConfiguration.FeedbackTab,
  4: LMMTabConfiguration.CourseStatisticTab,
  5: LMMTabConfiguration.CourseCommentTab,
  6: LMMTabConfiguration.ReportBrokenLinkTab
};
