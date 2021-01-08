import {
  AssessmentAnswerRepository,
  AssessmentRepository,
  Assignment,
  AssignmentRepository,
  ClassRun,
  ClassRunRepository,
  Course,
  CourseRepository,
  ParticipantAssignmentTrack,
  ParticipantAssignmentTrackRepository,
  UserInfoModel
} from '@opal20/domain-api';
import {
  AssignmentDetailMode,
  AssignmentDetailViewModel,
  AssignmentMode,
  BreadcrumbItem,
  BreadcrumbService,
  IOpalReportDynamicParams,
  LMMRoutePaths,
  LMMTabConfiguration,
  NavigationPageService,
  OpalReportDynamicComponent,
  ParticipantAssignmentTrackViewModel,
  RegistrationFilterComponent,
  RegistrationFilterModel,
  RouterPageInput
} from '@opal20/domain-components';
import { BaseFormComponent, ComponentType, DateUtils, IGridFilter, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, HostBinding } from '@angular/core';
import { Observable, Subscription, combineLatest, of } from 'rxjs';
import { OpalDialogService, SPACING_CONTENT } from '@opal20/common-components';

import { AssignAssignmentDialogComponent } from './dialogs/assign-assignment-dialog.component';
import { AssignmentDetailPageInput } from '../models/assignment-detail-page-input.model';
import { IAssignAssignmentDialogEvent } from '../models/assign-assignment-dialog-event.model';
import { LMM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/lmm-route-breadcumb-mapping-fn';
import { NAVIGATORS } from '../lmm.config';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { SetupPeerAssessmentDialogComponent } from './dialogs/setup-peer-assessment-dialog.component';

@Component({
  selector: 'assignment-detail-page',
  templateUrl: './assignment-detail-page.component.html'
})
export class AssignmentDetailPageComponent extends BaseFormComponent {
  public filterPopupContent: ComponentType<RegistrationFilterComponent> = RegistrationFilterComponent;
  public classRun: ClassRun = new ClassRun();
  public course: Course = new Course();
  public assignmentVm: AssignmentDetailViewModel = new AssignmentDetailViewModel();
  public breadCrumbItems: BreadcrumbItem[] = [];
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public paramsAssignemtReportDynamic: IOpalReportDynamicParams | null;
  public AssignmentMode: typeof AssignmentMode = AssignmentMode;
  public searchText: string = '';
  public stickySpacing: number = SPACING_CONTENT;
  public filterData: RegistrationFilterModel = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public get title(): string {
    return this.assignmentVm.title;
  }

  public get detailPageInput(): RouterPageInput<AssignmentDetailPageInput, LMMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<AssignmentDetailPageInput, LMMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadData();
      }
    }
  }

  private _detailPageInput: RouterPageInput<AssignmentDetailPageInput, LMMTabConfiguration, unknown> | undefined = NAVIGATORS[
    LMMRoutePaths.AssignmentDetailPage
  ] as RouterPageInput<AssignmentDetailPageInput, LMMTabConfiguration, unknown>;
  private _loadDataSub: Subscription = new Subscription();
  public get selectedTab(): LMMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : LMMTabConfiguration.ClassRunInfoTab;
  }

  private currentUser = UserInfoModel.getMyUserInfo();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    private navigationPageService: NavigationPageService,
    private courseRepository: CourseRepository,
    private classRunRepository: ClassRunRepository,
    private breadcrumbService: BreadcrumbService,
    private assignmentRepository: AssignmentRepository,
    private participantAssignmentTrackRepository: ParticipantAssignmentTrackRepository,
    private assessmentRepository: AssessmentRepository,
    private assessmentAnswerRepository: AssessmentAnswerRepository
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

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public onApplyFilter(data: RegistrationFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public canAssignAssignment(): boolean {
    return this.classRun.canAssignAssignment() && ParticipantAssignmentTrack.hasAssignAssignmentPermission(this.currentUser);
  }

  public onClickSetupPeerAssessment(): void {
    this.opalDialogService.openDialogRef(SetupPeerAssessmentDialogComponent, {
      courseId: this.detailPageInput.data.courseId,
      assignmentId: this.assignmentVm.id,
      classrunId: this.classRun.id
    });
  }

  public onClickAsign(): void {
    const dialogRef = this.opalDialogService.openDialogRef(AssignAssignmentDialogComponent, {
      courseId: this.detailPageInput.data.courseId,
      assignmentId: this.assignmentVm.id,
      classRun: this.classRun
    });

    this.subscribe(dialogRef.result, (data: IAssignAssignmentDialogEvent) => {
      if (data.registrations != null && data.registrations.length > 0 && data.startDate != null && data.endDate != null) {
        this.subscribe(
          this.participantAssignmentTrackRepository.assignAssignment({
            registrations: data.registrations,
            assignmentId: this.assignmentVm.id,
            startDate: DateUtils.removeTime(data.startDate),
            endDate: DateUtils.setTimeToEndInDay(data.endDate)
          }),
          () => {
            this.opalDialogService.openConfirmDialog({
              confirmTitle: 'Assign',
              confirmMsg: 'You have successfully issued the assignment to the user(s).',
              hideYesBtn: true,
              noBtnText: 'Close'
            });
          }
        );
      }
    });
  }

  public readonly(): boolean {
    return this.detailPageInput.data.mode === AssignmentDetailMode.View;
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = assignmentDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public setParamsAssignemtReportDynamic(): void {
    this.paramsAssignemtReportDynamic = OpalReportDynamicComponent.buildAssignmentTracking(this.assignmentVm.id);
  }

  public onViewAssignmentTrack(participantAssignmentTrack: ParticipantAssignmentTrackViewModel): void {
    if (ParticipantAssignmentTrack.hasViewAnswerDoneAssignmentPermission(this.currentUser)) {
      this.navigationPageService.navigateTo(
        LMMRoutePaths.ParticipantAssignmentTrackPage,
        {
          activeTab: LMMTabConfiguration.LearnerAssignmentAnswerTab,
          data: {
            assignmentId: participantAssignmentTrack.assignmentId,
            participantAssignmentTrackId: participantAssignmentTrack.id,
            userId: participantAssignmentTrack.userId,
            courseId: this.detailPageInput.data.courseId,
            classRunId: this.detailPageInput.data.classRunId
          }
        },
        this.detailPageInput
      );
    }
  }

  public canViewLearnerAssignmentTrack(): boolean {
    return ParticipantAssignmentTrack.hasViewLearnerAssignmentTrackPermission(this.currentUser);
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public canSetupPeerAssessment(): boolean {
    return this.assignmentVm.assessmentConfig != null;
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadData();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<AssignmentDetailPageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();
    const courseObs: Observable<Course | null> =
      this.detailPageInput.data.courseId != null ? this.courseRepository.loadCourse(this.detailPageInput.data.courseId) : of(null);
    const classRunObs: Observable<ClassRun | null> =
      this.detailPageInput.data.classRunId != null
        ? this.classRunRepository.loadClassRunById(this.detailPageInput.data.classRunId)
        : of(null);
    const assignmentObs: Observable<Assignment | null> =
      this.detailPageInput.data.assignmentId != null
        ? this.assignmentRepository.getAssignmentById(this.detailPageInput.data.assignmentId)
        : of(null);
    const assessmentObs = this.assessmentRepository.loadAssessments();
    this._loadDataSub = combineLatest(courseObs, classRunObs, assignmentObs, assessmentObs)
      .pipe(this.untilDestroy())
      .subscribe(([course, classRun, assignment, assessmentResult]) => {
        this.course = course;
        this.classRun = classRun;
        this.assignmentVm = new AssignmentDetailViewModel(assignment, assessmentResult.items);
        this.loadBreadcrumb();
        this.setParamsAssignemtReportDynamic();
      });
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      LMM_ROUTE_BREADCUMB_MAPPING_FN(this.detailPageInput, p => this.navigationPageService.navigateByRouter(p), {
        [LMMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName },
        [LMMRoutePaths.ClassRunDetailPage]: { textFn: () => this.classRun.classTitle },
        [LMMRoutePaths.AssignmentDetailPage]: { textFn: () => this.assignmentVm.title }
      })
    );
  }
}

export const assignmentDetailPageTabIndexMap = {
  0: LMMTabConfiguration.AssignmentInfoTab,
  1: LMMTabConfiguration.AssigneesTab
};
