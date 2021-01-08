import {
  AssessmentAnswer,
  Assignment,
  AssignmentRepository,
  COMMENT_ACTION_MAPPING,
  ClassRun,
  ClassRunRepository,
  CommentApiService,
  CommentNotification,
  CommentServiceType,
  Course,
  CourseRepository,
  EntityCommentType,
  IMarkScoreForQuizQuestionAnswer,
  IMarkScoreForQuizQuestionAnswerRequest,
  ParticipantAssignmentTrack,
  ParticipantAssignmentTrackRepository,
  ParticipantAssignmentTrackStatus,
  PublicUserInfo,
  UserInfoModel,
  UserRepository
} from '@opal20/domain-api';
import {
  AssignmentDetailViewModel,
  BreadcrumbItem,
  BreadcrumbService,
  CommentTabInput,
  IAssessmentPlayerInput,
  LMMRoutePaths,
  LMMTabConfiguration,
  NavigationData,
  NavigationPageService,
  PARTICIPANT_ASSIGNMENT_TRACK_STATUS_COLOR_MAP,
  ParticipantAssignmentTrackDetailViewModel,
  RouterPageInput
} from '@opal20/domain-components';
import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';

import { ButtonAction } from '@opal20/common-components';
import { LMM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/lmm-route-breadcumb-mapping-fn';
import { NAVIGATORS } from '../lmm.config';
import { ParticipantAssignmentTrackDetailPageInput } from '../models/participant-assignment-track-input.model';
import { map } from 'rxjs/operators';

@Component({
  selector: 'participant-assignment-track-page',
  templateUrl: './participant-assignment-track-page.component.html'
})
export class ParticipantAssignmentTrackDetailPageComponent extends BaseFormComponent {
  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;
  public title: string = '';
  public classRun: ClassRun = new ClassRun();
  public course: Course = new Course();
  public assignmentVm: AssignmentDetailViewModel = new AssignmentDetailViewModel();
  public participantAssignmentTrackVm: ParticipantAssignmentTrackDetailViewModel = new ParticipantAssignmentTrackDetailViewModel();
  public user: PublicUserInfo = new PublicUserInfo();
  public breadCrumbItems: BreadcrumbItem[] = [];
  public actionBtnGroup: ButtonAction<unknown>[] = [];
  public get detailPageInput(): RouterPageInput<ParticipantAssignmentTrackDetailPageInput, LMMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<ParticipantAssignmentTrackDetailPageInput, LMMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadData();
      }
    }
  }

  public get commentTabInput(): CommentTabInput {
    return {
      originalObjectId: this.detailPageInput.data.participantAssignmentTrackId,
      commentServiceType: CommentServiceType.Course,
      entityCommentType: EntityCommentType.ParticipantAssignmentTrackQuizAnswer,
      mappingAction: COMMENT_ACTION_MAPPING,
      commentNotification: CommentNotification.AssignmentFeedbackToLearner,
      hasReply: true
    };
  }

  public get status(): ParticipantAssignmentTrackStatus {
    return this.participantAssignmentTrackVm.status;
  }

  public get statusColorMap(): unknown {
    return PARTICIPANT_ASSIGNMENT_TRACK_STATUS_COLOR_MAP;
  }

  public get mainAssessmentInput(): IAssessmentPlayerInput {
    return {
      assessmentId: this.participantAssignmentTrackVm.assignmentVm.assessmentId
        ? this.participantAssignmentTrackVm.assignmentVm.assessmentId
        : '',
      participantAssignmentTrackId: this.detailPageInput.data ? this.detailPageInput.data.participantAssignmentTrackId : '',
      userId: AssessmentAnswer.assessmentForFacilitator
    };
  }

  public get peerAssessmentInput(): IAssessmentPlayerInput {
    return {
      assessmentId: this.participantAssignmentTrackVm.assignmentVm.assessmentId
        ? this.participantAssignmentTrackVm.assignmentVm.assessmentId
        : '',
      participantAssignmentTrackId: this.detailPageInput.data ? this.detailPageInput.data.participantAssignmentTrackId : '',
      userId: this.detailPageInput.data ? this.detailPageInput.data.userId : ''
    };
  }

  public navigationData: NavigationData;
  public loadingData: boolean = false;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public AssessmentAnswer: typeof AssessmentAnswer = AssessmentAnswer;
  private _detailPageInput: RouterPageInput<ParticipantAssignmentTrackDetailPageInput, LMMTabConfiguration, unknown> = NAVIGATORS[
    LMMRoutePaths.ParticipantAssignmentTrackPage
  ] as RouterPageInput<ParticipantAssignmentTrackDetailPageInput, LMMTabConfiguration, unknown>;
  private _loadDataSub: Subscription = new Subscription();

  public get selectedTab(): LMMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : LMMTabConfiguration.LearnerAssignmentAnswerTab;
  }

  private currentUser = UserInfoModel.getMyUserInfo();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private classRunRepository: ClassRunRepository,
    private navigationPageService: NavigationPageService,
    private courseRepository: CourseRepository,
    private assignmentRepository: AssignmentRepository,
    private userRepository: UserRepository,
    private participantAssignmentTrackRepository: ParticipantAssignmentTrackRepository,
    private breadcrumbService: BreadcrumbService,
    private commentApiService: CommentApiService
  ) {
    super(moduleFacadeService);
    this.commentApiService.initApiService(CommentServiceType.Course);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public dataHasChanged(): boolean {
    return this.participantAssignmentTrackVm && this.participantAssignmentTrackVm.dataHasChanged();
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack();
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = participantAssignmentTrackDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public checkCommentsHasDataFnCreator(): () => Observable<boolean> {
    return () => {
      return from(
        this.commentApiService.getCommentNotSeen({
          objectIds: [this.detailPageInput.data.participantAssignmentTrackId],
          entityCommentType: EntityCommentType.ParticipantAssignmentTrackQuizAnswer
        })
      ).pipe(
        map(data => {
          if (data == null || data.length === 0) {
            return false;
          }

          const commentDic = Utils.toDictionarySelect(data, x => x.objectId, x => x.commentNotSeenIds);

          if (commentDic[this.detailPageInput.data.participantAssignmentTrackId] == null) {
            return null;
          }

          return commentDic[this.detailPageInput.data.participantAssignmentTrackId].length > 0;
        })
      );
    };
  }

  public onSaveManualScore(): void {
    this.validateAndSaveScore().subscribe();
  }

  public validateAndSaveScore(): Observable<ParticipantAssignmentTrack> {
    return from(
      new Promise<ParticipantAssignmentTrack>((resolve, reject) => {
        this.saveScore().then(_ => {
          resolve(_);
        }, reject);
      })
    );
  }

  public saveScore(): Promise<ParticipantAssignmentTrack> {
    const markScoreForQuizQuestionAnswers: IMarkScoreForQuizQuestionAnswer[] = [];
    Object.keys(this.participantAssignmentTrackVm.assignmentAnswerTrackDic).forEach(key => {
      const assignmentAnswerTrack = this.participantAssignmentTrackVm.assignmentAnswerTrackDic[key];
      const givedScore = assignmentAnswerTrack.manualScore != null ? assignmentAnswerTrack.manualScore : assignmentAnswerTrack.score;

      if (assignmentAnswerTrack.giveScore !== givedScore) {
        markScoreForQuizQuestionAnswers.push({
          score: assignmentAnswerTrack.giveScore,
          quizAssignmentFormQuestionId: assignmentAnswerTrack.questionId
        });
      }
    });
    const request: IMarkScoreForQuizQuestionAnswerRequest = {
      markScoreForQuizQuestionAnswers: markScoreForQuizQuestionAnswers,
      participantAssignmentTrackId: this.detailPageInput.data.participantAssignmentTrackId
    };

    return this.participantAssignmentTrackRepository
      .markScoreManuallyForQuizAssignmentQuestionAnswer(request)
      .toPromise()
      .then(_ => {
        this.showNotification();
        return _;
      });
  }

  public canViewAssessment(): boolean {
    return this.assignmentVm.assessmentId != null && this.participantAssignmentTrackVm.submittedDate != null;
  }

  public canViewPeerAssessment(): boolean {
    return this.assignmentVm.assessmentId != null && this.participantAssignmentTrackVm.submittedDate != null;
  }

  public showCommentFeedbackAssignment(): boolean {
    return this.participantAssignmentTrackVm.participantAssignmentTrack.hasViewCommentFeedbackAssignmentPermission(this.currentUser);
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadData();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<ParticipantAssignmentTrackDetailPageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
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
    const userObs: Observable<PublicUserInfo[] | null> =
      this.detailPageInput.data.userId != null
        ? this.userRepository.loadPublicUserInfoList({ userIds: [this.detailPageInput.data.userId] })
        : of(null);
    const participantAssignmentTrackObs: Observable<ParticipantAssignmentTrack | null> =
      this.detailPageInput.data.participantAssignmentTrackId != null
        ? this.participantAssignmentTrackRepository.getParticipantAssignmentTrackById(
            this.detailPageInput.data.participantAssignmentTrackId
          )
        : of(null);

    this._loadDataSub = combineLatest(courseObs, classRunObs, assignmentObs, userObs, participantAssignmentTrackObs)
      .pipe(this.untilDestroy())
      .subscribe(([course, classRun, assignment, users, participantAssignmentTrack]) => {
        this.course = course;
        this.classRun = classRun;
        this.assignmentVm = new AssignmentDetailViewModel(assignment);
        this.participantAssignmentTrackVm = new ParticipantAssignmentTrackDetailViewModel(participantAssignmentTrack, this.assignmentVm);
        this.user = users != null && users.length > 0 ? users[0] : new PublicUserInfo();
        this.loadBreadcrumb();
      });
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      LMM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p => this.navigationPageService.navigateByRouter(p, () => this.dataHasChanged(), () => this.validateAndSaveScore()),
        {
          [LMMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName },
          [LMMRoutePaths.ClassRunDetailPage]: { textFn: () => this.classRun.classTitle },
          [LMMRoutePaths.AssignmentDetailPage]: { textFn: () => this.assignmentVm.title },
          [LMMRoutePaths.ParticipantAssignmentTrackPage]: { textFn: () => this.user.fullName }
        }
      )
    );
  }
}

export const participantAssignmentTrackDetailPageTabIndexMap = {
  0: LMMTabConfiguration.LearnerAssignmentAnswerTab,
  1: LMMTabConfiguration.LearnerAssignmentAnswerCommentTab,
  2: LMMTabConfiguration.AssessmentTab,
  3: LMMTabConfiguration.PeerAssessmentTab
};
