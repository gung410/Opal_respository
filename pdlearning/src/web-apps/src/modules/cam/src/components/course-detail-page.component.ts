import {
  BaseFormComponent,
  CustomFormControl,
  DateUtils,
  IFormBuilderDefinition,
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
  COURSE_IN_COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP,
  COURSE_STATUS_COLOR_MAP,
  CommentDialogComponent,
  CommentTabInput,
  ContextMenuAction,
  CourseCriteriaDetailMode,
  CourseCriteriaDetailViewModel,
  CourseDetailComponent,
  CourseDetailMode,
  CourseDetailViewModel,
  IDialogActionEvent,
  IDownloadTemplateOption,
  IOpalReportDynamicParams,
  ISelectUserDialogResult,
  NavigationPageService,
  OpalReportDynamicComponent,
  RouterPageInput,
  SelectFilesDialogComponent,
  SelectUserDialogComponent,
  WebAppLinkBuilder
} from '@opal20/domain-components';
import {
  ButtonGroupButton,
  ContextMenuItem,
  DialogAction,
  OpalDialogService,
  SPACING_CONTENT,
  futureDateValidator,
  ifAsyncValidator,
  ifValidator,
  noContentWhitespaceValidator,
  requiredAndNoWhitespaceValidator,
  requiredForListValidator,
  requiredIfValidator,
  requiredNumberValidator,
  startEndValidator,
  validateFutureDateType,
  validateNoContentWhitespaceType,
  validateRequiredNumberType
} from '@opal20/common-components';
import {
  COMMENT_ACTION_MAPPING,
  CommentApiService,
  CommentServiceType,
  Course,
  CourseApiService,
  CourseContentItemModel,
  CoursePlanningCycle,
  CoursePlanningCycleRepository,
  CourseRepository,
  CourseStatus,
  CourseType,
  ECertificateRepository,
  EntityCommentType,
  ExportParticipantTemplateRequestFileFormat,
  FormRepository,
  FormStatus,
  FormType,
  ISaveCourseRequest,
  ITransferOwnerRequest,
  LearningContentRepository,
  MetadataId,
  OrganizationRepository,
  RegistrationApiService,
  RegistrationRepository,
  TaggingRepository,
  TrainingAgencyType,
  UserInfoModel,
  UserRepository
} from '@opal20/domain-api';
import { Component, ElementRef, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import {
  checkCourseEndDateValidWithClassEndDateValidator,
  validateCourseEndDateType
} from '../validators/check-course-end-date-valid-with-classrun-end-date';
import { checkExistedExternalCodeValidator, validateExistedExternalCodeType } from '../validators/check-existed-external-code-validator';
import { map, switchMap } from 'rxjs/operators';

import { CAM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/cam-route-breadcumb-mapping-fn';
import { Constant } from '@opal20/authentication';
import { ContextMenuEvent } from '@progress/kendo-angular-menu';
import { CourseCriteriaDetailPageComponent } from './course-criteria-detail-page.component';
import { CourseDetailPageInput } from './../models/course-detail-page-input.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { ExportParticipantDialogComponent } from './dialogs/export-participants-dialog.component';
import { NAVIGATORS } from '../cam.config';
import { Validators } from '@angular/forms';

@Component({
  selector: 'course-detail-page',
  templateUrl: './course-detail-page.component.html'
})
export class CourseDetailPageComponent extends BaseFormComponent {
  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;
  @ViewChild(CourseCriteriaDetailPageComponent, { static: false })
  public courseCriteriaDetailPage: CourseCriteriaDetailPageComponent;
  @ViewChild('anchor', { static: false }) public anchorEl: ElementRef;

  public reportName: string = 'Application Status';
  public paramsReportDynamic: IOpalReportDynamicParams | null;
  public breadCrumbItems: BreadcrumbItem[] = [];
  public stickySpacing: number = SPACING_CONTENT;
  public get title(): string {
    return this.course.courseName;
  }

  public get subTitle(): string {
    return this.course.courseCode;
  }

  public get course(): CourseDetailViewModel {
    return this._course;
  }
  public set course(v: CourseDetailViewModel) {
    this._course = v;
  }

  public get courseCriteria(): CourseCriteriaDetailViewModel {
    return this._courseCriteria;
  }
  public set courseCriteria(v: CourseCriteriaDetailViewModel) {
    this._courseCriteria = v;
  }

  public get detailPageInput(): RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined) {
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
      entityCommentType: EntityCommentType.Course,
      mappingAction: COMMENT_ACTION_MAPPING,
      hasReply: true,
      mode: this.course.isArchived ? 'view' : 'edit'
    };
  }

  public get courseStatusColorMap(): typeof COURSE_IN_COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP | typeof COURSE_STATUS_COLOR_MAP {
    return this.course && this.course.courseData && this.course.courseData.coursePlanningCycleId
      ? COURSE_IN_COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP
      : COURSE_STATUS_COLOR_MAP;
  }

  public get getContextMenuByCourse(): ContextMenuItem[] {
    return this.contextMenuItemsForCourse.filter(
      item =>
        (this.course.courseData.canDeleteCourse(this.currentUser) &&
          item.id === ContextMenuAction.Delete &&
          this.selectedTab === CAMTabConfiguration.CourseInfoTab) ||
        (this.course.courseData.canTransferOwnershipCourse(this.currentUser) &&
          item.id === ContextMenuAction.TransferOwnership &&
          this.selectedTab === CAMTabConfiguration.CourseInfoTab)
    );
  }

  public contextMenuItemsForCourse: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    },
    {
      id: ContextMenuAction.TransferOwnership,
      text: this.translateCommon('Transfer Ownership'),
      icon: 'user'
    }
  ];
  public classRunIds: string[] = [];
  public loadingData: boolean = false;
  public dataLoadedOnce: boolean = false;
  public courseContents: CourseContentItemModel[] = [];
  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  public ContextMenuAction: typeof ContextMenuAction = ContextMenuAction;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  public CourseStatus: typeof CourseStatus = CourseStatus;
  public CommentServiceType: typeof CommentServiceType = CommentServiceType;
  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Import Participants',
      onClickFn: () => this.onImportParticipant(),
      shownIfFn: () => this.showImportParticipant()
    },
    {
      displayText: 'Edit',
      onClickFn: () => this.onEditCourse(),
      shownIfFn: () => this.showEditCourse()
    },
    {
      displayText: 'Edit',
      onClickFn: () => this.onEditCourseCriteria(),
      shownIfFn: () => this.showEditCourseCriteria()
    },
    {
      displayText: 'Save',
      onClickFn: () => this.onSaveCourse(),
      shownIfFn: () => this.showSaveCourse(),
      isDisabledFn: () => !this.dataHasChanged()
    },
    {
      displayText: 'Save',
      onClickFn: () => this.onSaveCourseCriteria(),
      shownIfFn: () => this.showSaveCourseCriteria()
    },
    {
      displayText: 'Submit for Approval',
      onClickFn: () => this.onSubmitCourse(),
      shownIfFn: () => this.showSubmitCourse()
    },
    {
      displayText: 'Approve',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Approve),
      shownIfFn: () => this.showApprovalCourse()
    },
    {
      displayText: 'Confirm',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Verified),
      shownIfFn: () => this.showVerifiedCourse()
    },
    {
      displayText: 'Reject',
      onClickFn: () => this.onRejectCourse(),
      shownIfFn: () => this.showRejectCourse()
    },
    {
      displayText: 'Publish',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Publish),
      shownIfFn: () => this.showPublishCourse()
    },
    {
      displayText: 'Unpublish',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Unpublish),
      shownIfFn: () => this.showUnpublishCourse()
    },
    {
      displayText: 'Complete Planning',
      onClickFn: () => this.onActionApproval(ContextMenuAction.Completed),
      shownIfFn: () => this.showPlanningCompletedCourse()
    },
    {
      displayText: 'Reuse Existing Data',
      onClickFn: () => this.reuseCourseExistingDataForCourseCriteria(),
      shownIfFn: () => this.showReuseCourseExistingDataForCourseCriteria()
    },
    // {
    //   displayText: 'Export',
    //   onClickFn:() => this.onExportParticipants(),
    //   shownIfFn:() => this.showExportParticipant()
    // },
    {
      id: ContextMenuAction.Delete,
      icon: 'delete',
      displayText: 'Delete',
      shownInMoreFn: () => true,
      onClickFn: () => {
        this.modalService.showConfirmMessage(
          new TranslationMessage(this.moduleFacadeService.globalTranslator, "You're about to delete this course. Do you want to proceed?"),
          () => {
            this.courseRepository.deleteCourse(this.detailPageInput.data.id).then(() => {
              this.showNotification(`${this.course.courseData.courseName} is successfully deleted`, NotificationType.Success);
              this.navigationPageService.navigateBack();
            });
          }
        );
      },
      shownIfFn: () => this.course.courseData.canDeleteCourse(this.currentUser) && this.selectedTab === CAMTabConfiguration.CourseInfoTab
    },
    {
      id: ContextMenuAction.TransferOwnership,
      icon: 'user',
      displayText: this.translateCommon('Transfer Ownership'),
      shownInMoreFn: () => true,
      onClickFn: () => {
        const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
          SelectUserDialogComponent,
          SelectUserDialogComponent.selectTransferOwnerCoursesConfig()
        );
        this.subscribe(dialogRef.result, data => {
          if ((<ISelectUserDialogResult>data).id) {
            this.courseRepository
              .transferOwnerCourse(<ITransferOwnerRequest>{
                courseId: this.detailPageInput.data.id,
                newOwnerId: (<ISelectUserDialogResult>data).id
              })
              .then(() => {
                this.showNotification(this.translate('Ownership transferred successfully'), NotificationType.Success);
                this.navigationPageService.navigateBack();
              });
          }
        });
      },
      shownIfFn: () =>
        this.course.courseData.canTransferOwnershipCourse(this.currentUser) && this.selectedTab === CAMTabConfiguration.CourseInfoTab
    },
    {
      id: ContextMenuAction.Archive,
      icon: 'aggregate-fields',
      displayText: this.translateCommon('Archive'),
      shownInMoreFn: () => true,
      onClickFn: () => {
        this.opalDialogService
          .openConfirmDialog({
            confirmTitle: 'Warning',
            confirmMsg: 'Do you want to archive this course ? This action cannot be reverted.'
          })
          .subscribe(action => {
            if (action === DialogAction.OK) {
              this.archiveCourse(this.course.courseData);
            }
          });
      },
      shownIfFn: () => this.course.courseData.canArchiveCourse(this.currentUser) && this.selectedTab === CAMTabConfiguration.CourseInfoTab
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
  public hasCoursePlanningAsParentPage: boolean;

  private _detailPageInput: RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> = NAVIGATORS[
    CAMRoutePaths.CourseDetailPage
  ] as RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration>;
  private _loadCourseInfoSub: Subscription = new Subscription();
  private _course: CourseDetailViewModel = new CourseDetailViewModel(null, {}, [], [], {}, []);
  private _courseCriteria: CourseCriteriaDetailViewModel;
  private currentUser = UserInfoModel.getMyUserInfo();
  // Auto save after 30 minutes
  private scheduler: IntervalScheduler = new IntervalScheduler(600000, () => {
    if (this.dataHasChanged() && (this.course.status == null || this.course.status === CourseStatus.Draft)) {
      this.validateAndSaveCourse(CourseStatus.Draft).subscribe();
    }
  });

  private coursePlanningCycle: CoursePlanningCycle = new CoursePlanningCycle();
  public get selectedTab(): CAMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : CAMTabConfiguration.CourseInfoTab;
  }
  private modeOnInit?: CourseDetailMode;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private courseRepository: CourseRepository,
    private taggingRepository: TaggingRepository,
    private userRepository: UserRepository,
    private organizationRepository: OrganizationRepository,
    private registrationRepository: RegistrationRepository,
    private opalDialogService: OpalDialogService,
    private navigationPageService: NavigationPageService,
    private formRepository: FormRepository,
    private breadcrumbService: BreadcrumbService,
    private courseApiService: CourseApiService,
    private coursePlanningCycleRepository: CoursePlanningCycleRepository,
    private ecertificateRepository: ECertificateRepository,
    private learningContentRepository: LearningContentRepository,
    private commentApiService: CommentApiService,
    private registrationApiService: RegistrationApiService
  ) {
    super(moduleFacadeService);
    this.commentApiService.initApiService(CommentServiceType.Course);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public loadCourseInfo(): void {
    this._loadCourseInfoSub.unsubscribe();
    const courseObs: Observable<Course | null> =
      this.detailPageInput.data.id != null
        ? this.courseRepository.loadCourse(this.detailPageInput.data.id)
        : of(Course.createForPlanningCycle(this.detailPageInput.data.coursePlanningCycleId));
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    const formObs = this.formRepository.searchForm(
      [FormStatus.Published, FormStatus.ReadyToUse],
      FormType.Survey,
      CourseDetailViewModel.formsSurveyTypes,
      0,
      Constant.MAX_ITEMS_PER_REQUEST,
      null,
      true
    );

    const courseContentObs: Observable<CourseContentItemModel[]> =
      this.detailPageInput.data.id != null ? this.learningContentRepository.getTableOfContents(this.detailPageInput.data.id) : of([]);
    this.loadingData = true;
    this._loadCourseInfoSub = combineLatest(courseObs, taggingObs, formObs, courseContentObs)
      .pipe(
        switchMap(([course, metadatas, formResult, toc]) => {
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
            this.detailPageInput.data.coursePlanningCycleId
          ).pipe(map(courseVm => <[CourseDetailViewModel, Course, CourseContentItemModel[]]>[courseVm, course, toc]));
        }),
        this.untilDestroy()
      )
      .subscribe(
        ([courseVm, course, toc]) => {
          if (this.loadingData || !(this.detailPageInput.data.mode === CourseDetailMode.Edit && this.course.dataHasChanged())) {
            this.courseContents = toc;
            this.course = courseVm;
            if (course == null) {
              this.course.courseType =
                this.detailPageInput.data.mode === CourseDetailMode.Recurring ? CourseType.Recurring : CourseType.New;
            }
            this.coursePlanningCycle = courseVm.coursePlanningCycle;
            this.initFormData();
            this.loadBreadcrumb();
            this.setParamsReportDynamic();
            this.setMenuTitleReport();
            this.loadingData = false;
            this.dataLoadedOnce = true;
          }
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public setParamsReportDynamic(): void {
    if (!this.course) {
      return;
    }

    this.paramsReportDynamic = OpalReportDynamicComponent.buildListOfApplicationStatusReportParams(
      this.detailPageInput.data.id,
      this.course.isMicrolearning
    );
  }

  public reuseCourseExistingDataForCourseCriteria(): void {
    this.courseCriteriaDetailPage.reuseCourseExistingDataForCourseCriteria();
    this.showNotification(this.translate('Reuse course existing data for course criteria successfully.'));
  }

  public onExportParticipants(): Promise<boolean> {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(ExportParticipantDialogComponent, {
      courseId: this.detailPageInput.data.id,
      classRunIds: this.classRunIds
    });
    return dialogRef.result
      .pipe(
        map((data: DialogAction) => {
          return data === DialogAction.OK;
        })
      )
      .toPromise();
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(
      () => this.dataHasChanged(),
      () => this.validateAndSaveCourse(this.modeOnInit === CourseDetailMode.ForApprover ? null : CourseStatus.Draft)
    );
  }

  public onEditCourse(): void {
    this.detailPageInput.data.mode = CourseDetailMode.Edit;
    this.tabStrip.selectTab(0);
  }

  public onEditCourseCriteria(): void {
    this.detailPageInput.data.courseCriteriaMode = CourseCriteriaDetailMode.Edit;
    this.tabStrip.selectTab(7);
  }

  public onSaveCourse(): void {
    const status = this.course.isCompletingCourseForPlanning ? CourseStatus.PlanningCycleVerified : CourseStatus.Draft;
    this.subscribe(this.validateAndSaveCourse(status), _ => {
      this.navigationPageService.navigateBack();
    });
  }

  public onSaveCourseCriteria(): void {
    this.subscribe(this.courseCriteriaDetailPage.validateAndSaveCourseCriteria(), _ => {
      this.detailPageInput.data.courseCriteriaMode = CourseCriteriaDetailMode.View;
    });
  }

  public onSubmitCourse(): void {
    this.validateAndSaveCourse(CourseStatus.PendingApproval).subscribe(() => this.navigationPageService.navigateBack());
  }

  public onImportParticipant(): void {
    const downloadImportParticipantsOption: IDownloadTemplateOption<ExportParticipantTemplateRequestFileFormat> = {
      templateFormats: [
        {
          fileFormatName: '.csv',
          fileFormat: ExportParticipantTemplateRequestFileFormat.CSV
        },
        {
          fileFormatName: '.xlsx',
          fileFormat: ExportParticipantTemplateRequestFileFormat.Excel
        }
      ],
      downloadTemplateFn: fileFormat => this.registrationApiService.downloadExportParticipantTemplate({ fileFormat: fileFormat })
    };
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(SelectFilesDialogComponent, {
      downloadTemplateOption: downloadImportParticipantsOption
    });
    this.subscribe(dialogRef.result, (data: File[]) => {
      if (data.length > 0) {
        this.registrationRepository.importParticipant({ file: data[0], courseId: this.detailPageInput.data.id }).then(importResult => {
          if (importResult) {
            this.showNotification(
              `${importResult.numberOfAddedParticipants} user(s) added successfully;  ${importResult.totalNumberOfUsers -
                importResult.numberOfAddedParticipants}  user(s) unable to be added`
            );
          }
        });
      }
    });
  }

  public validateAndSaveCourse(status: CourseStatus = null): Observable<void> {
    return from(
      new Promise<void>((resolve, reject) => {
        this.validate(status === CourseStatus.Draft ? ['courseName'] : null).then(valid => {
          if (valid) {
            this.saveCourse(status).then(course => {
              this.showNotification();
              resolve();
            }, reject);
          } else {
            reject({});
          }
        });
      })
    );
  }

  public dataHasChanged(): boolean {
    return this.course && this.course.dataHasChanged() && this.isInSavingCourseMode();
  }

  public isInSavingCourseMode(): boolean {
    return (
      this.detailPageInput.data.mode === CourseDetailMode.Edit ||
      this.detailPageInput.data.mode === CourseDetailMode.NewCourse ||
      this.detailPageInput.data.mode === CourseDetailMode.Recurring
    );
  }

  public showSaveCourse(): boolean {
    return (
      this.selectedTab === CAMTabConfiguration.CourseInfoTab &&
      ((this.detailPageInput.data.mode === CourseDetailMode.Edit &&
        this.course.status !== CourseStatus.PendingApproval &&
        this.course.originCourseData.hasCreateEditCoursePermission(this.currentUser) &&
        !this.course.isArchived) ||
        this.detailPageInput.data.mode === CourseDetailMode.NewCourse ||
        this.detailPageInput.data.mode === CourseDetailMode.Recurring)
    );
  }

  public showSaveCourseCriteria(): boolean {
    return (
      this.detailPageInput.data.courseCriteriaMode === CourseCriteriaDetailMode.Edit &&
      this.selectedTab === CAMTabConfiguration.CourseCriteriaTab &&
      !this.course.isArchived
    );
  }

  public dataCourseCriteriaHasChanged(): boolean {
    return this.courseCriteria && this.courseCriteria.dataCourseCriteriaHasChanged();
  }

  public showSubmitCourse(): boolean {
    const notShowSubmitBtnStatuses = [
      CourseStatus.PendingApproval,
      CourseStatus.PlanningCycleVerified,
      CourseStatus.PlanningCycleCompleted,
      CourseStatus.Archived
    ];
    return (
      this.selectedTab === CAMTabConfiguration.CourseInfoTab &&
      ((this.detailPageInput.data.mode === CourseDetailMode.Edit && !notShowSubmitBtnStatuses.includes(this.course.status)) ||
        this.detailPageInput.data.mode === CourseDetailMode.NewCourse ||
        this.detailPageInput.data.mode === CourseDetailMode.Recurring)
    );
  }

  public showEditCourse(): boolean {
    return (
      this.selectedTab === CAMTabConfiguration.CourseInfoTab &&
      (this.course.isCompletingCourseForPlanning || this.course.isEditable) &&
      ((this.detailPageInput.data.mode === CourseDetailMode.View &&
        this.course.status !== CourseStatus.PendingApproval &&
        this.course.originCourseData.hasCreateEditCoursePermission(this.currentUser)) ||
        (this.detailPageInput.data.mode === CourseDetailMode.ForApprover &&
          this.course.originCourseData.hasApprovalPermission(this.currentUser)))
    );
  }

  public showEditCourseCriteria(): boolean {
    return (
      this.detailPageInput.data.courseCriteriaMode === CourseCriteriaDetailMode.View &&
      this.selectedTab === CAMTabConfiguration.CourseCriteriaTab &&
      !this.course.isArchived
    );
  }
  public showRejectCourse(): boolean {
    return (this.showApprovalCourse() || this.showVerifiedCourse()) && this.selectedTab === CAMTabConfiguration.CourseInfoTab;
  }

  public showApprovalCourse(): boolean {
    return (
      (this.detailPageInput.data.mode === CourseDetailMode.Edit || this.detailPageInput.data.mode === CourseDetailMode.ForApprover) &&
      this.course.courseData.canBeApproved() &&
      Course.haveApproveCoursePermission(this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.CourseInfoTab
    );
  }

  public showPublishCourse(): boolean {
    return (
      this.detailPageInput.data.mode === CourseDetailMode.View &&
      this.course.courseData.canPublishCourse(this.courseContents) &&
      this.course.courseData.hasPublishCoursePermission(this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.CourseInfoTab
    );
  }

  public showUnpublishCourse(): boolean {
    return (
      this.detailPageInput.data.mode === CourseDetailMode.View &&
      this.course.originCourseData.canUnpublishCourse(this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.CourseInfoTab
    );
  }

  public showVerifiedCourse(): boolean {
    return (
      this.detailPageInput.data.mode === CourseDetailMode.ForVerifier &&
      this.course.originCourseData.canVerifyCourse(this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.CourseInfoTab
    );
  }

  public showPlanningCompletedCourse(): boolean {
    return (
      this.detailPageInput.data.mode === CourseDetailMode.Edit &&
      this.course.status === CourseStatus.PlanningCycleVerified &&
      this.selectedTab === CAMTabConfiguration.CourseInfoTab
    );
  }

  public showImportParticipant(): boolean {
    return this.detailPageInput.data.mode === CourseDetailMode.View && this.course.originCourseData.canImportParticipant(this.currentUser);
  }

  public showReuseCourseExistingDataForCourseCriteria(): boolean {
    return (
      this.detailPageInput.data.courseCriteriaMode === CourseCriteriaDetailMode.Edit &&
      this.selectedTab === CAMTabConfiguration.CourseCriteriaTab &&
      !this.course.isArchived
    );
  }
  public showExportParticipant(): boolean {
    return this.course && this.course.originCourseData.canExportParticipant(this.currentUser);
  }

  public onRejectCourse(): void {
    if (this.showVerifiedCourse()) {
      this.onActionApproval(ContextMenuAction.VerificationRejected);
    } else if (this.showApprovalCourse()) {
      this.onActionApproval(ContextMenuAction.Reject);
    }
  }

  public canViewClassRuns(): boolean {
    return (
      (this.detailPageInput.data.mode === CourseDetailMode.ForApprover ||
        this.detailPageInput.data.mode === CourseDetailMode.ForVerifier ||
        this.detailPageInput.data.mode === CourseDetailMode.View) &&
      this.course.courseData.canViewClassRun() &&
      this.course.originCourseData.hasViewClassRunsPermission(this.currentUser)
    );
  }

  public canViewComment(): boolean {
    return (
      this.detailPageInput.data.mode === CourseDetailMode.ForApprover ||
      this.detailPageInput.data.mode === CourseDetailMode.ForVerifier ||
      this.detailPageInput.data.mode === CourseDetailMode.View
    );
  }

  public checkCommentsHasDataFnCreator(): () => Observable<boolean> {
    return () => {
      return from(
        this.commentApiService.getCommentNotSeen({
          objectIds: [this.detailPageInput.data.id],
          entityCommentType: EntityCommentType.Course
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

  public canViewStatisticalReport(): boolean {
    return this.course.courseData.canViewStatisticalReport(this.currentUser) && !this.hasCoursePlanningAsParentPage;
  }

  public canViewAddingParticipant(): boolean {
    return this.course.courseData.canAddParticipants(this.currentUser) && !this.hasCoursePlanningAsParentPage;
  }

  public canViewSendCoursePublicity(): boolean {
    return this.course.status === CourseStatus.Published && !this.hasCoursePlanningAsParentPage;
  }

  public canViewSendCourseNominationAnnouncement(): boolean {
    return this.course.status === CourseStatus.Published && !this.hasCoursePlanningAsParentPage;
  }

  public canViewCourseCriteria(): boolean {
    return !Utils.isNullOrEmpty(this.detailPageInput.data.id);
  }

  public canViewOrderRefreshment(): boolean {
    return this.course.status === CourseStatus.Published && !this.course.isMicrolearning;
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = courseDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public onActionApproval(action: ContextMenuAction): void {
    switch (action) {
      case ContextMenuAction.Approve:
        this.showApprovalDialog(
          {
            title: `${this.translate('Approve Course')}: ${this.course.courseName}`,
            requiredCommentField: false
          },
          CourseStatus.Approved
        );
        break;
      case ContextMenuAction.Reject:
        this.showApprovalDialog(
          {
            title: `${this.translate('Reject Course')}: ${this.course.courseName}`
          },
          CourseStatus.Rejected
        );
        break;
      case ContextMenuAction.Publish:
        this.changeCourseStatus(this.course.courseData, CourseStatus.Published);
        break;
      case ContextMenuAction.Unpublish:
        this.changeCourseStatus(this.course.courseData, CourseStatus.Unpublished);
        break;
      case ContextMenuAction.Verified:
        this.showDialog(
          {
            title: `${this.translate('Verify Course')}: ${this.course.courseName}`,
            requiredCommentField: false
          },
          CourseStatus.PlanningCycleVerified
        );
        break;
      case ContextMenuAction.VerificationRejected:
        this.showDialog(
          {
            title: `${this.translate('Reject Course')}: ${this.course.courseName}`
          },
          CourseStatus.VerificationRejected
        );
        break;
      case ContextMenuAction.Completed:
        this.validateAndSaveCourse(CourseStatus.PlanningCycleCompleted).subscribe(_ => {
          this.detailPageInput.data.mode = CourseDetailMode.View;
        });
        break;
      default:
        break;
    }
  }

  public onItemSelect(contextMenuEmit: ContextMenuEvent): void {
    switch (contextMenuEmit.item.id) {
      case 'delete':
        this.modalService.showConfirmMessage(
          new TranslationMessage(this.moduleFacadeService.globalTranslator, "You're about to delete this course. Do you want to proceed?"),
          () => {
            this.courseRepository.deleteCourse(this.detailPageInput.data.id).then(() => {
              this.showNotification(`${this.course.courseData.courseName} is successfully deleted`, NotificationType.Success);
              this.navigationPageService.navigateBack();
            });
          }
        );
        break;
      case 'transferOwnership':
        const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
          SelectUserDialogComponent,
          SelectUserDialogComponent.selectTransferOwnerCoursesConfig()
        );
        this.subscribe(dialogRef.result, data => {
          if ((<ISelectUserDialogResult>data).id) {
            this.courseRepository
              .transferOwnerCourse(<ITransferOwnerRequest>{
                courseId: this.detailPageInput.data.id,
                newOwnerId: (<ISelectUserDialogResult>data).id
              })
              .then(() => {
                this.showNotification(this.translate('Ownership transferred successfully'), NotificationType.Success);
                this.navigationPageService.navigateBack();
              });
          }
        });
        break;
      default:
        break;
    }
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.modeOnInit = this.detailPageInput.data.mode;
    this.hasCoursePlanningAsParentPage = this.navigationPageService.findParentOfCurrentRouter(CAMRoutePaths.CoursePlanningPage) != null;
    this.loadCourseInfo();
    this.scheduler.init();
  }

  protected onDestroy(): void {
    this.scheduler.destroy();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      // Overview Info
      formName: 'overview-info',
      validateByGroupControlNames: [
        ['durationHours', 'durationMinutes'],
        ['notionalCost', 'courseFee'],
        ['minParticipantPerClass', 'maxParticipantPerClass'],
        ['planningPublishDate', 'startDate', 'expiredDate', 'planningArchiveDate'],
        ['numOfHoursPerSession', 'numOfMinutesPerSession']
      ],
      controls: {
        thumbnailUrl: {
          defaultValue: this.course.thumbnailUrl,
          validators: null
        },
        courseName: {
          defaultValue: this.course.courseName,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('courseName', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredAndNoWhitespaceValidator()
              ),
              validatorType: 'required'
            },
            {
              validator: Validators.maxLength(2000)
            }
          ]
        },
        pdActivityType: {
          defaultValue: this.course.pdActivityType,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('pdActivityType', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => Validators.required
              ),
              validatorType: 'required'
            }
          ]
        },
        durationHours: {
          defaultValue: this.course.durationHours,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('durationHours', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(() => !this.course.isMicrolearning && !this.course.isPlanningVerificationRequired)
              ),
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isPlanningVerificationRequired &&
                  this.course.durationMinutes === 0 &&
                  !this.course.isViewOnlyForField('durationHours', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredNumberValidator()
              ),
              validatorType: validateRequiredNumberType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Please input hours/minutes duration')
            }
          ]
        },
        durationMinutes: {
          defaultValue: this.course.durationMinutes,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('durationMinutes', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(() => !this.course.isPlanningVerificationRequired)
              ),
              validatorType: 'required'
            }
          ]
        },
        categoryIds: {
          defaultValue: this.course.categoryIds,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('categoryIds', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator()
              ),
              validatorType: 'required'
            }
          ]
        },
        learningMode: {
          defaultValue: this.course.learningMode,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('learningMode', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => Validators.required
              ),
              validatorType: 'required'
            }
          ]
        },
        courseCode: {
          defaultValue: this.course.courseCode,
          validators: null
        },
        externalCode: {
          defaultValue: this.course.externalCode,
          validators: [
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField('externalCode', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredAndNoWhitespaceValidator()
              ),
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField('externalCode', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => noContentWhitespaceValidator()
              ),
              validatorType: validateNoContentWhitespaceType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'This field can not contain whitespace')
            },
            {
              validator: ifValidator(p => !this.course.isMicrolearning, () => Validators.maxLength(6))
            },
            {
              validator: ifAsyncValidator(
                p =>
                  !this.course.isMicrolearning &&
                  this.isInSavingCourseMode() &&
                  !this.course.isViewOnlyForField('externalCode', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => checkExistedExternalCodeValidator(this.courseApiService, () => this.course.originCourseData)
              ),
              isAsync: true,
              validatorType: validateExistedExternalCodeType,
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'This code already exists and has been used in another course'
              )
            }
          ]
        },
        courseOutlineStructure: {
          defaultValue: this.course.courseOutlineStructure,
          validators: null
        },
        courseObjective: {
          defaultValue: this.course.courseObjective,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('courseObjective', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredAndNoWhitespaceValidator()
              ),
              validatorType: 'required'
            }
          ]
        },
        description: {
          defaultValue: this.course.description,
          validators: [
            {
              validator: ifValidator(
                p =>
                  !this.course.isPlanningVerificationRequired &&
                  !this.course.isViewOnlyForField('description', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredAndNoWhitespaceValidator()
              ),
              validatorType: 'required'
            }
          ]
        },
        // Provider Info,
        trainingAgency: {
          defaultValue: this.course.trainingAgency,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('trainingAgency', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator()
              ),
              validatorType: 'required'
            }
          ]
        },
        nieAcademicGroups: {
          defaultValue: this.course.nieAcademicGroups,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('nieAcademicGroups', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator(() => this.course.trainingAgencyContains(TrainingAgencyType.NIE))
              ),
              validatorType: 'required'
            }
          ]
        },
        otherTrainingAgencyReason: {
          defaultValue: this.course.otherTrainingAgencyReason,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'otherTrainingAgencyReason',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () =>
                  requiredForListValidator(
                    () => !this.course.isMicrolearning && !this.course.trainingAgencyContains(TrainingAgencyType.MOE)
                  )
              ),
              validatorType: 'required'
            }
          ]
        },
        ownerDivisionIds: {
          defaultValue: this.course.ownerDivisionIds,
          validators: null
        },
        ownerBranchIds: {
          defaultValue: this.course.ownerBranchIds,
          validators: null
        },
        partnerOrganisationIds: {
          defaultValue: this.course.partnerOrganisationIds,
          validators: null
        },
        moeOfficerId: {
          defaultValue: this.course.moeOfficerId,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('moeOfficerId', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(p => !this.course.isMicrolearning)
              ),
              validatorType: 'required'
            }
          ]
        },
        moeOfficerPhoneNumber: {
          defaultValue: this.course.moeOfficerPhoneNumber,
          validators: [
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField(
                    'moeOfficerPhoneNumber',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => Validators.pattern(new RegExp('^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-s./0-9]*$', 'i'))
              ),
              validatorType: 'pattern',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The phone number must be digit number')
            }
            // Currently not supported Validate length of Phonenumber, but maybe use in the future
            // {
            //   validator: ifValidator(p => !this.course.isMicrolearning, () => Validators.maxLength(8)),
            //   validatorType: 'maxLength',
            //   message: new TranslationMessage(
            //     this.moduleFacadeService.translator,
            //     'The phone number must be a 8 digit number or less number'
            //   )
            // },
          ]
        },
        moeOfficerEmail: {
          defaultValue: this.course.moeOfficerEmail,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('moeOfficerEmail', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(p => !this.course.isMicrolearning)
              ),
              validatorType: 'required'
            }
          ]
        },
        notionalCost: {
          defaultValue: this.course.notionalCost,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('notionalCost', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(p => !this.course.isMicrolearning && !this.course.isPlanningVerificationRequired)
              ),
              validatorType: 'required'
            }
          ]
        },
        courseFee: {
          defaultValue: this.course.courseFee,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('courseFee', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(p => !this.course.isMicrolearning && !this.course.isPlanningVerificationRequired)
              ),
              validatorType: 'required'
            }
          ]
        },

        // Metadata
        serviceSchemeIds: {
          defaultValue: this.course.serviceSchemeIds,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('serviceSchemeIds', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator()
              ),
              validatorType: 'required'
            }
          ]
        },
        learningFrameworkIds: {
          defaultValue: this.course.learningFrameworkIds,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('learningFrameworkIds', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator()
              ),
              validatorType: 'required'
            }
          ]
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
          defaultValue: this.course.subjectAreaIds,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('subjectAreaIds', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator()
              ),
              validatorType: 'required'
            }
          ]
        },
        pdAreaThemeId: {
          defaultValue: this.course.pdAreaThemeId,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('pdAreaThemeId', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => Validators.required
              ),
              validatorType: 'required'
            }
          ]
        },
        courseLevel: {
          defaultValue: this.course.courseLevel,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('courseLevel', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => Validators.required
              ),
              validatorType: 'required'
            }
          ]
        },
        metadataKeys: {
          defaultValue: this.course.metadataKeys,
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
          defaultValue: this.course.copyrightOwner,
          validators: [
            {
              validator: ifValidator(
                p =>
                  !this.course.isPlanningVerificationRequired &&
                  !this.course.isViewOnlyForField('copyrightOwner', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredAndNoWhitespaceValidator()
              ),
              validatorType: 'required'
            }
          ]
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
          defaultValue: this.course.trackIds,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('trackIds', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator(() => this.course.tracksTree && this.course.tracksTree.length > 0)
              ),
              validatorType: 'required'
            }
          ]
        },
        developmentalRoleIds: {
          defaultValue: this.course.developmentalRoleIds,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('developmentalRoleIds', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator(() => this.course.developmentalRolesTree && this.course.developmentalRolesTree.length > 0)
              ),
              validatorType: 'required'
            }
          ]
        },
        teachingLevels: {
          defaultValue: this.course.teachingLevels,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('teachingLevels', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () =>
                  requiredForListValidator(
                    () =>
                      this.course.teachingLevelsTree &&
                      this.course.teachingLevelsTree.length > 0 &&
                      (!this.course.hasOnlyOneServiceSchemesChecked() ||
                        !this.course.serviceSchemesContains(MetadataId.ExecutiveAndAdministrativeStaff))
                  )
              ),
              validatorType: 'required'
            }
          ]
        },
        teachingCourseStudyIds: {
          defaultValue: this.course.teachingCourseStudyIds,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'teachingCourseStudyIds',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () =>
                  requiredForListValidator(
                    () =>
                      this.course.teachingCourseStudysTree &&
                      this.course.teachingCourseStudysTree.length > 0 &&
                      (!this.course.hasOnlyOneServiceSchemesChecked() ||
                        !this.course.serviceSchemesContains(MetadataId.ExecutiveAndAdministrativeStaff))
                  )
              ),
              validatorType: 'required'
            }
          ]
        },
        placeOfWork: {
          defaultValue: this.course.placeOfWork,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('placeOfWork', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => Validators.required
              ),
              validatorType: 'required'
            }
          ]
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
          defaultValue: this.course.maximumPlacesPerSchool,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'maximumPlacesPerSchool',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(p => this.course.displayMaximumOfPlacesPerSchool && !this.course.isPlanningVerificationRequired)
              ),
              validatorType: 'required'
            }
          ]
        },
        prerequisiteCourseIds: {
          defaultValue: null,
          validators: null
        },
        numOfSchoolLeader: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('numOfSchoolLeader', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(() => !this.course.isMicrolearning && this.hasCoursePlanningAsParentPage === true)
              ),
              validatorType: 'required'
            }
          ]
        },
        numOfSeniorOrLeadTeacher: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'numOfSeniorOrLeadTeacher',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(() => !this.course.isMicrolearning && this.hasCoursePlanningAsParentPage === true)
              ),
              validatorType: 'required'
            }
          ]
        },
        numOfMiddleManagement: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'numOfMiddleManagement',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(() => !this.course.isMicrolearning && this.hasCoursePlanningAsParentPage === true)
              ),
              validatorType: 'required'
            }
          ]
        },
        numOfBeginningTeacher: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'numOfBeginningTeacher',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(() => !this.course.isMicrolearning && this.hasCoursePlanningAsParentPage === true)
              ),
              validatorType: 'required'
            }
          ]
        },
        numOfExperiencedTeacher: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'numOfExperiencedTeacher',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(() => !this.course.isMicrolearning && this.hasCoursePlanningAsParentPage === true)
              ),
              validatorType: 'required'
            }
          ]
        },
        teachingSubjectIds: {
          defaultValue: this.course.teachingSubjectIds,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('teachingSubjectIds', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () =>
                  requiredForListValidator(
                    () =>
                      this.course.teachingSubjectSelectItems &&
                      this.course.teachingSubjectSelectItems.length > 0 &&
                      (!this.course.hasOnlyOneServiceSchemesChecked() ||
                        !this.course.serviceSchemesContains(MetadataId.ExecutiveAndAdministrativeStaff))
                  )
              ),
              validatorType: 'required'
            }
          ]
        },
        jobFamily: {
          defaultValue: this.course.jobFamily,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('jobFamily', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () =>
                  requiredForListValidator(
                    () =>
                      this.course.jobFamilysTree &&
                      this.course.jobFamilysTree.length > 0 &&
                      this.course.serviceSchemesContains(MetadataId.ExecutiveAndAdministrativeStaff)
                  )
              ),
              validatorType: 'required'
            }
          ]
        },
        cocurricularActivityIds: {
          defaultValue: this.course.cocurricularActivityIds,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'cocurricularActivityIds',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () =>
                  requiredForListValidator(
                    () =>
                      this.course.cocurricularActivitySelectItems &&
                      this.course.cocurricularActivitySelectItems.length > 0 &&
                      (!this.course.hasOnlyOneServiceSchemesChecked() ||
                        !this.course.serviceSchemesContains(MetadataId.ExecutiveAndAdministrativeStaff))
                  )
              ),
              validatorType: 'required'
            }
          ]
        },
        easSubstantiveGradeBandingIds: {
          defaultValue: this.course.easSubstantiveGradeBandingIds,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'easSubstantiveGradeBandingIds',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () =>
                  requiredForListValidator(
                    () =>
                      this.course.easSubstantiveGradeBandingTree &&
                      this.course.easSubstantiveGradeBandingTree.length > 0 &&
                      this.course.serviceSchemesContains(MetadataId.ExecutiveAndAdministrativeStaff)
                  )
              ),
              validatorType: 'required'
            }
          ]
        },
        // Course Planning
        natureOfCourse: {
          defaultValue: this.course.natureOfCourse,
          validators: [
            {
              validator: ifValidator(
                () => !this.course.isViewOnlyForField('natureOfCourse', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(() => !this.course.isPlanningVerificationRequired)
              ),
              validatorType: 'required'
            }
          ]
        },
        numOfPlannedClass: {
          defaultValue: this.course.numOfPlannedClass,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('numOfPlannedClass', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(() => !this.course.isMicrolearning)
              ),
              validatorType: 'required'
            },
            {
              validator: ifValidator(p => !this.course.isMicrolearning, () => requiredNumberValidator()),
              validatorType: validateRequiredNumberType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The first number must not be entered 0')
            }
          ]
        },
        numOfSessionPerClass: {
          defaultValue: this.course.numOfSessionPerClass,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('numOfSessionPerClass', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(() => !this.course.isMicrolearning)
              ),
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField('numOfSessionPerClass', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredNumberValidator()
              ),
              validatorType: validateRequiredNumberType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The first number must not be entered 0')
            }
          ]
        },
        numOfHoursPerSession: {
          defaultValue: this.course.numOfHoursPerSession,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('numOfHoursPerSession', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredIfValidator(() => !this.course.isMicrolearning)
              ),
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  this.course.numOfMinutesPerSession === 0 &&
                  !this.course.isViewOnlyForField('numOfHoursPerSession', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredNumberValidator()
              ),
              validatorType: validateRequiredNumberType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Please input number of hours/minutes per session')
            }
          ]
        },
        numOfMinutesPerSession: {
          defaultValue: this.course.numOfMinutesPerSession,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'numOfMinutesPerSession',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(() => !this.course.isMicrolearning)
              ),
              validatorType: 'required'
            }
          ]
        },
        planningPublishDate: {
          defaultValue: this.course.planningPublishDate,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p => Utils.isDifferent(this.course.originCourseData.planningPublishDate, p.value),
                () => futureDateValidator()
              ),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Publish Date cannot be in the past')
            },
            {
              validator: startEndValidator('coursePublishDateWithStartDate', p => p.value, p => this.course.startDate, true, 'dateOnly'),
              validatorType: 'coursePublishDateWithStartDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Publish Date cannot be greater than Start Date')
            },
            {
              validator: startEndValidator('coursePublishDateWithEndDate', p => p.value, p => this.course.expiredDate, false, 'dateOnly'),
              validatorType: 'coursePublishDateWithEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Publish Date cannot be greater than End Date')
            },
            {
              validator: startEndValidator(
                'publishDateBeforeArchiveDate',
                p => p.value,
                p => this.course.planningArchiveDate,
                false,
                'dateOnly'
              ),
              validatorType: 'publishDateBeforeArchiveDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Publish Date must be before Archive Date')
            }
          ]
        },
        startDate: {
          defaultValue: this.course.startDate,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },
            {
              validator: ifValidator(p => Utils.isDifferent(this.course.originCourseData.startDate, p.value), () => futureDateValidator()),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date cannot be in the past')
            },
            {
              validator: ifValidator(
                p => !this.course.isMicrolearning,
                () =>
                  startEndValidator('courseStartDateWithPublishDate', p => this.course.planningPublishDate, p => p.value, true, 'dateOnly')
              ),
              validatorType: 'courseStartDateWithPublishDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date cannot be less than Publish Date')
            },
            {
              validator: startEndValidator('courseStartDateWithEndDate', p => p.value, p => this.course.expiredDate, false, 'dateOnly'),
              validatorType: 'courseStartDateWithEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date cannot be greater than End Date')
            },
            {
              validator: startEndValidator(
                'courseStartDateWithArchiveDate',
                p => p.value,
                p => this.course.planningArchiveDate,
                false,
                'dateOnly'
              ),
              validatorType: 'courseStartDateWithArchiveDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date cannot be greater than Archive Date')
            }
          ]
        },
        expiredDate: {
          defaultValue: this.course.expiredDate,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p => Utils.isDifferent(this.course.originCourseData.expiredDate, p.value),
                () => futureDateValidator()
              ),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date cannot be in the past')
            },
            {
              validator: ifValidator(
                p => !this.course.isMicrolearning,
                () =>
                  startEndValidator('courseEndDateWithPublishDate', p => this.course.planningPublishDate, p => p.value, false, 'dateOnly')
              ),
              validatorType: 'courseEndDateWithPublishDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date cannot be less than Publish Date')
            },
            {
              validator: startEndValidator('courseEndDate', p => this.course.startDate, p => p.value, false, 'dateOnly'),
              validatorType: 'courseEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date cannot be less than Start Date')
            },
            {
              validator: startEndValidator(
                'courseEndDateWithArchiveDate',
                p => p.value,
                p => this.course.planningArchiveDate,
                true,
                'dateOnly'
              ),
              validatorType: 'courseEndDateWithArchiveDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date cannot be greater than Archive Date')
            },
            {
              validator: ifAsyncValidator(
                p => !this.course.isMicrolearning,
                () => checkCourseEndDateValidWithClassEndDateValidator(this.courseApiService, () => this.course.courseData)
              ),
              isAsync: true,
              validatorType: validateCourseEndDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Course End Date cannot be less than Class End Date')
            }
          ]
        },
        planningArchiveDate: {
          defaultValue: this.course.planningArchiveDate,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p => Utils.isDifferent(this.course.originCourseData.planningArchiveDate, p.value),
                () => futureDateValidator()
              ),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Archive Date cannot be in the past')
            },
            {
              validator: startEndValidator('courseArchiveDateWithStartDate', p => this.course.startDate, p => p.value, false, 'dateOnly'),
              validatorType: 'courseArchiveDateWithStartDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Archive Date cannot be less than Start Date')
            },
            {
              validator: startEndValidator('courseArchiveDateWithEndDate', p => this.course.expiredDate, p => p.value, true, 'dateOnly'),
              validatorType: 'courseArchiveDateWithEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Archive Date cannot be less than End Date')
            },
            {
              validator: startEndValidator(
                'publishDateBeforeArchiveDate',
                p => this.course.planningPublishDate,
                p => p.value,
                false,
                'dateOnly'
              ),
              validatorType: 'publishDateBeforeArchiveDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Archive Date must be after Publish Date')
            },
            {
              validator: ifValidator(
                p => this.coursePlanningCycle && this.coursePlanningCycle.id != null,
                () => (control: CustomFormControl) => {
                  if (control.value && +(<Date>control.value.getFullYear()) < this.coursePlanningCycle.yearCycle) {
                    return {
                      archiveDateBeforePlanningCyclePeriod: true
                    };
                  }
                  return null;
                }
              ),
              validatorType: 'archiveDateBeforePlanningCyclePeriod',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Archive year must not be before than planning cycle period'
              )
            }
          ]
        },
        pdActivityPeriods: {
          defaultValue: this.course.pdActivityPeriods,
          validators: [
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField('pdActivityPeriods', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredForListValidator()
              ),
              validatorType: 'required'
            }
          ]
        },
        minParticipantPerClass: {
          defaultValue: this.course.minParticipantPerClass,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'minParticipantPerClass',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(() => !this.course.isMicrolearning)
              ),
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField(
                    'minParticipantPerClass',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => startEndValidator('courseMinimumWithMaximum', p => p.value, p => this.course.maxParticipantPerClass)
              ),
              validatorType: 'courseMinimumWithMaximum',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Minimum class size cannot be greater than maximum class size'
              )
            },
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField(
                    'minParticipantPerClass',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredNumberValidator()
              ),
              validatorType: validateRequiredNumberType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The first number must not be entered 0')
            }
          ]
        },
        maxParticipantPerClass: {
          defaultValue: this.course.maxParticipantPerClass,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'maxParticipantPerClass',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(() => !this.course.isMicrolearning)
              ),
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField(
                    'maxParticipantPerClass',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => startEndValidator('courseMaximumWithMinimum', p => this.course.minParticipantPerClass, p => p.value)
              ),
              validatorType: 'courseMaximumWithMinimum',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Maximum class size cannot be less than minimum class size'
              )
            },
            {
              validator: ifValidator(
                p =>
                  !this.course.isMicrolearning &&
                  !this.course.isViewOnlyForField(
                    'maxParticipantPerClass',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredNumberValidator()
              ),
              validatorType: validateRequiredNumberType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The first number must not be entered 0')
            }
          ]
        },
        maxReLearningTimes: {
          defaultValue: this.course.maxReLearningTimes,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('maxReLearningTimes', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => Validators.required
              ),
              validatorType: 'required'
            },
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField('maxReLearningTimes', CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)),
                () => requiredNumberValidator()
              ),
              validatorType: validateRequiredNumberType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The first number must not be entered 0')
            }
          ]
        },
        // Evaluation And ECertificate
        // preCourseEvaluationFormId: {
        //   defaultValue: null,
        //   validators: [
        //     {
        //       validator: requiredIfValidator(() => !this.course.isMicrolearning),
        //       validatorType: 'required'
        //     }
        //   ]
        // },
        postCourseEvaluationFormId: {
          defaultValue: this.course.postCourseEvaluationFormId,
          validators: [
            {
              validator: ifValidator(
                () =>
                  !this.course.isViewOnlyForField(
                    'postCourseEvaluationFormId',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => requiredIfValidator(() => !this.course.isMicrolearning && !this.course.isPlanningVerificationRequired)
              ),
              validatorType: 'required'
            }
          ]
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
          validators: [
            {
              validator: ifValidator(
                p =>
                  !!p.value &&
                  !this.course.isViewOnlyForField(
                    'courseNameInECertificate',
                    CourseDetailComponent.asViewMode(this.detailPageInput.data.mode)
                  ),
                () => Validators.pattern(new RegExp('^(.){1,100}$', 'gm'))
              ),
              validatorType: 'pattern',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'The course name in ecertificate must be smaller than 101 characters'
              )
            }
          ]
        },
        // Administration
        firstAdministratorId: {
          defaultValue: this.course.firstAdministratorId,
          validators: [
            {
              validator: requiredIfValidator(p => !this.course.isMicrolearning),
              validatorType: 'required'
            }
          ]
        },
        secondAdministratorId: {
          defaultValue: null,
          validators: null
        },
        primaryApprovingOfficerId: {
          defaultValue: this.course.primaryApprovingOfficerId,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        alternativeApprovingOfficerId: {
          defaultValue: null,
          validators: null
        },
        courseFacilitatorId: {
          defaultValue: this.course.courseCoFacilitatorId,
          validators: [
            {
              validator: requiredIfValidator(p => !this.course.isMicrolearning),
              validatorType: 'required'
            }
          ]
        },
        courseCoFacilitatorId: {
          defaultValue: null,
          validators: null
        },
        collaborativeContentCreatorIds: {
          defaultValue: null,
          validators: null
        }
      },
      options: {
        autoAsyncIndicator: false
      }
    };
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return this.currentUser.permissionDic;
  }

  private saveCourse(status: CourseStatus): Promise<Course> {
    return new Promise((resolve, reject) => {
      const request: ISaveCourseRequest = {
        data: Utils.clone(this.course.courseData, cloneData => {
          cloneData.status = status != null ? status : cloneData.status;
          cloneData.courseType = this.detailPageInput.data.mode === CourseDetailMode.Recurring ? CourseType.Recurring : CourseType.New;
          cloneData.planningPublishDate = cloneData.planningPublishDate ? DateUtils.removeTime(cloneData.planningPublishDate) : null;
          cloneData.planningArchiveDate = cloneData.planningArchiveDate ? DateUtils.removeTime(cloneData.planningArchiveDate) : null;
          cloneData.startDate = cloneData.startDate ? DateUtils.removeTime(cloneData.startDate) : null;
          cloneData.expiredDate = cloneData.expiredDate ? DateUtils.removeTime(cloneData.expiredDate) : null;
        })
      };
      this.courseRepository
        .saveCourse(request)
        .pipe(
          map(course => {
            this.course.updateCourseData(course);
            return course;
          }),
          switchMap(course => {
            return this.taggingRepository
              .saveCourseMetadata(course.id, { tagIds: course.getAllMetadataTagIds(), searchTags: course.metadataKeys })
              .pipe(map(_ => course));
          }),
          this.untilDestroy()
        )
        .subscribe(course => {
          resolve(course);
        }, reject);
    });
  }

  private showApprovalDialog(input: unknown, status: CourseStatus.Approved | CourseStatus.Rejected): void {
    // Skip validation for approving course without editing
    const validateObs = this.detailPageInput.data.mode === CourseDetailMode.Edit ? this.validate() : Promise.resolve(true);
    validateObs.then(valid => {
      if (valid) {
        this.showDialog(input, status);
      }
    });
  }

  private showDialog(input: unknown, status: CourseStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    this.subscribe(dialogRef.result, (data: IDialogActionEvent) => {
      if (data.action === DialogAction.OK) {
        if (this.detailPageInput.data.mode === CourseDetailMode.Edit) {
          this.saveCourse(status).then(_ => {
            this.showNotification();
            this.navigationPageService.navigateBack();
          });
        } else if (
          this.detailPageInput.data.mode === CourseDetailMode.ForApprover ||
          this.detailPageInput.data.mode === CourseDetailMode.ForVerifier
        ) {
          this.changeCourseStatus(this.course.courseData, status, data.comment);
        }
      }
    });
  }

  private archiveCourse(course: Course): Promise<void> {
    return this.courseRepository.archiveCourse({ ids: [course.id] }).then(_ => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  private changeCourseStatus(course: Course, status: CourseStatus, comment: string = ''): void {
    this.subscribe(this.courseRepository.changeCourseStatus({ ids: [course.id], status: status, comment: comment }), () => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      CAM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p => this.navigationPageService.navigateByRouter(p, () => this.dataHasChanged(), () => this.validateAndSaveCourse()),
        {
          [CAMRoutePaths.CoursePlanningCycleDetailPage]: {
            textFn: () => (this.coursePlanningCycle != null ? this.coursePlanningCycle.title : '')
          },
          [CAMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName }
        }
      )
    );
  }

  private setMenuTitleReport(): void {
    if (this.course.isMicrolearning) {
      this.reportName = 'MLU Summary';
    }
  }
}

export const courseDetailPageTabIndexMap = {
  0: CAMTabConfiguration.CourseInfoTab,
  1: CAMTabConfiguration.ClassRunsTab,
  2: CAMTabConfiguration.CourseCommentTab,
  3: CAMTabConfiguration.CourseStatisticTab,
  4: CAMTabConfiguration.AddingParticipantTab,
  5: CAMTabConfiguration.SendCoursePublicityTab,
  6: CAMTabConfiguration.SendCourseNominationTab,
  7: CAMTabConfiguration.CourseCriteriaTab,
  8: CAMTabConfiguration.SendOrderRefreshmentTab
};
