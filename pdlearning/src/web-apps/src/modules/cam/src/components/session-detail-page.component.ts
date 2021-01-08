import {
  BaseFormComponent,
  DateUtils,
  IFormBuilderDefinition,
  IntervalScheduler,
  ModuleFacadeService,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import {
  BreadcrumbItem,
  BreadcrumbService,
  CAMRoutePaths,
  CAMTabConfiguration,
  NavigationData,
  NavigationPageService,
  RouterPageInput,
  SessionDetailMode,
  SessionDetailViewModel
} from '@opal20/domain-components';
import {
  ButtonAction,
  ButtonGroupButton,
  DialogAction,
  OpalDialogService,
  SPACING_CONTENT,
  requiredAndNoWhitespaceValidator
} from '@opal20/common-components';
import {
  ClassRun,
  ClassRunRepository,
  ContentRepository,
  Course,
  CoursePlanningCycle,
  CoursePlanningCycleRepository,
  CourseRepository,
  DigitalContent,
  ISaveSessionRequest,
  SearchRegistrationsType,
  Session,
  SessionApiService,
  SessionRepository,
  UserInfoModel
} from '@opal20/domain-api';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import {
  checkExistedSessionDateValidator,
  validateExistedSessionDateType
} from '../validators/session/check-existed-session-date-validator';
import { map, switchMap } from 'rxjs/operators';

import { CAM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/cam-route-breadcumb-mapping-fn';
import { NAVIGATORS } from '../cam.config';
import { SessionDetailPageInput } from '../models/session-detail-page-input.model';
import { SessionValidator } from '../validators/session/session-validator';

@Component({
  selector: 'session-detail-page',
  templateUrl: './session-detail-page.component.html'
})
export class SessionDetailPageComponent extends BaseFormComponent {
  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;
  public classRun: ClassRun = new ClassRun();
  public course: Course = new Course();
  public breadCrumbItems: BreadcrumbItem[] = [];
  public actionBtnGroup: ButtonAction<unknown>[] = [];
  public stickySpacing: number = SPACING_CONTENT;

  public get title(): string {
    return this.session.sessionTitle;
  }

  public get detailPageInput(): RouterPageInput<SessionDetailPageInput, CAMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<SessionDetailPageInput, CAMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadSessionInfo();
      }
    }
  }

  public get session(): SessionDetailViewModel {
    return this._session;
  }
  public set session(v: SessionDetailViewModel) {
    this._session = v;
  }

  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Edit',
      onClickFn: () => this.onEditSession(),
      shownIfFn: () => this.showEditSession()
    },
    {
      displayText: 'Save',
      onClickFn: () => this.onSaveSession(),
      shownIfFn: () => this.showSubmitSession(),
      isDisabledFn: () => !this.dataHasChanged()
    },
    {
      displayText: 'Delete',
      onClickFn: () => this.onDeleteSession(),
      shownIfFn: () => this.showDeleteSession()
    }
  ];

  public navigationData: NavigationData;
  public loadingData: boolean = false;
  public registrationStatusChanged: boolean = false;
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  private _detailPageInput: RouterPageInput<SessionDetailPageInput, CAMTabConfiguration, unknown> = NAVIGATORS[
    CAMRoutePaths.SessionDetailPage
  ] as RouterPageInput<SessionDetailPageInput, CAMTabConfiguration, unknown>;
  private _loadSessionInfoSub: Subscription = new Subscription();
  private _session: SessionDetailViewModel = new SessionDetailViewModel();
  private currentUser = UserInfoModel.getMyUserInfo();
  private coursePlanningCycle: CoursePlanningCycle = new CoursePlanningCycle();
  // Auto save after 30 minutes
  private scheduler: IntervalScheduler = new IntervalScheduler(600000, () => {
    if (this.dataHasChanged()) {
      this.onSaveSession();
    }
  });

  public get selectedTab(): CAMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : CAMTabConfiguration.SessionInfoTab;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private classRunRepository: ClassRunRepository,
    private sessionRepository: SessionRepository,
    private navigationPageService: NavigationPageService,
    private courseRepository: CourseRepository,
    private breadcrumbService: BreadcrumbService,
    private coursePlanningCycleRepository: CoursePlanningCycleRepository,
    private opalDialogService: OpalDialogService,
    private sessionApiService: SessionApiService,
    private contentRepository: ContentRepository
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = sessionDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public loadSessionInfo(): void {
    this._loadSessionInfoSub.unsubscribe();
    const courseObs: Observable<Course | null> =
      this.detailPageInput.data.courseId != null ? this.courseRepository.loadCourse(this.detailPageInput.data.courseId) : of(null);
    const classRunObs: Observable<ClassRun | null> =
      this.detailPageInput.data.classRunId != null
        ? this.classRunRepository.loadClassRunById(this.detailPageInput.data.classRunId)
        : of(null);
    const sessionObs =
      this.detailPageInput.data.classRunId != null && this.detailPageInput.data.id != null
        ? this.sessionRepository.loadSessionById(this.detailPageInput.data.id)
        : of(null);
    this.loadingData = true;
    this._loadSessionInfoSub = combineLatest(courseObs, classRunObs, sessionObs)
      .pipe(
        switchMap(([course, classRun, session]) => {
          const coursePlanningCycleObs = course.coursePlanningCycleId
            ? this.coursePlanningCycleRepository.loadCoursePlanningCycleById(course.coursePlanningCycleId)
            : of(null);
          const sessionPreRecordClipObs =
            session && session.preRecordId ? this.contentRepository.loadDigitalContentById(session.preRecordId) : of(null);
          return combineLatest([coursePlanningCycleObs, sessionPreRecordClipObs]).pipe(
            map(
              ([coursePlanningCycle, sessionPreRecordClip]) =>
                <[Course, ClassRun, Session, CoursePlanningCycle, DigitalContent]>[
                  course,
                  classRun,
                  session,
                  coursePlanningCycle,
                  sessionPreRecordClip
                ]
            )
          );
        }),
        this.untilDestroy()
      )
      .subscribe(
        ([course, classRun, session, coursePlanningCycle, sessionPreRecordClip]) => {
          this.course = course;
          this.session = new SessionDetailViewModel({ sessionData: session, sessionPreRecordClip: sessionPreRecordClip });
          this.classRun = classRun;
          this.coursePlanningCycle = coursePlanningCycle;
          this.loadBreadcrumb();
          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(() => this.dataHasChanged(), () => this.validateAndSaveSession());
  }

  public showSubmitSession(): boolean {
    return (
      (this.detailPageInput.data.mode === SessionDetailMode.Edit &&
        this.session.sessionData.canBeModified(this.classRun, this.course) &&
        this.session.sessionData.hasModifiedPermission(this.course, this.currentUser)) ||
      (this.detailPageInput.data.mode === SessionDetailMode.NewSesion && this.selectedTab === CAMTabConfiguration.SessionInfoTab)
    );
  }

  public showEditSession(): boolean {
    return (
      this.detailPageInput.data.mode === SessionDetailMode.View &&
      this.session.sessionData.canBeModified(this.classRun, this.course) &&
      this.session.sessionData.hasModifiedPermission(this.course, this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.SessionInfoTab
    );
  }

  public showDeleteSession(): boolean {
    return (
      this.detailPageInput.data.mode === SessionDetailMode.View &&
      this.session.sessionData.canBeModified(this.classRun, this.course) &&
      this.session.sessionData.hasModifiedPermission(this.course, this.currentUser) &&
      !this.session.sessionData.isStarted()
    );
  }

  public onEditSession(): void {
    this.detailPageInput.data.mode = SessionDetailMode.Edit;
    this.tabStrip.selectTab(0);
  }

  public onSaveSession(): void {
    this.validateAndSaveSession().subscribe(() => this.navigationPageService.navigateBack());
  }

  public onDeleteSession(): void {
    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'You’re about to delete this session. Do you want to proceed?',
        yesBtnText: 'Yes',
        noBtnText: 'No'
      })
      .subscribe(action => {
        if (action === DialogAction.OK) {
          this.sessionRepository.deleteSession(this.detailPageInput.data.id).then(_ => {
            this.showNotification(this.translate('Session deleted successfully'));
            this.navigationPageService.navigateBack();
          });
        }
      });
  }

  public validateAndSaveSession(): Observable<void> {
    return from(
      new Promise<void>((resolve, reject) => {
        this.validate().then(valid => {
          if (valid) {
            this.saveSession().then(_ => {
              this.showNotification();
              resolve();
            }, reject);
          } else {
            reject();
          }
        });
      })
    );
  }

  public dataHasChanged(): boolean {
    return this.session && this.session.dataHasChanged();
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadSessionInfo();
    this.scheduler.init();
  }

  protected onDestroy(): void {
    this.scheduler.destroy();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'session-detail',
      validateByGroupControlNames: [['sessionDate', 'startTime', 'endTime']],
      controls: {
        sessionTitle: {
          defaultValue: null,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            }
          ]
        },
        venue: {
          defaultValue: null,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            }
          ]
        },
        sessionDate: {
          defaultValue: null,
          validators: [
            ...SessionValidator.getSessionDateValidators(
              () => DateUtils.buildDateTime(this.classRun.startDate, this.classRun.planningStartTime),
              () => DateUtils.buildDateTime(this.classRun.endDate, this.classRun.planningEndTime),
              () => this.session.sessionData,
              this.moduleFacadeService
            ),
            {
              validator: checkExistedSessionDateValidator(
                this.sessionApiService,
                () => this.session.originSessionData,
                this.detailPageInput.data.classRunId
              ),
              isAsync: true,
              validatorType: validateExistedSessionDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'There is already a session having the same date')
            }
          ]
        },
        startTime: {
          defaultValue: null,
          validators: SessionValidator.getSessionStartTimeValidators(
            () => DateUtils.buildDateTime(this.classRun.startDate, this.classRun.planningStartTime),
            () => DateUtils.buildDateTime(this.classRun.endDate, this.classRun.planningEndTime),
            () => this.session.sessionData,
            () => this.session.originSessionData,
            this.moduleFacadeService
          )
        },
        endTime: {
          defaultValue: null,
          validators: SessionValidator.getSessionEndTimeValidators(
            () => DateUtils.buildDateTime(this.classRun.startDate, this.classRun.planningStartTime),
            () => DateUtils.buildDateTime(this.classRun.endDate, this.classRun.planningEndTime),
            () => this.session.sessionData,
            this.moduleFacadeService
          )
        }
      },
      options: {
        autoAsyncIndicator: false
      }
    };
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<SessionDetailPageInput, CAMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private saveSession(): Promise<Session> {
    return new Promise((resolve, reject) => {
      const request: ISaveSessionRequest = {
        data: Utils.clone(this.session.sessionData, cloneData => {
          cloneData.classRunId = this.detailPageInput.data.classRunId;
          cloneData.startDateTime = DateUtils.buildDateTime(cloneData.sessionDate, cloneData.startTime);
          cloneData.endDateTime = DateUtils.buildDateTime(cloneData.sessionDate, cloneData.endTime);
        })
      };
      this.sessionRepository
        .saveSession(request)
        .pipe(this.untilDestroy())
        .subscribe(session => {
          resolve(session);
        }, reject);
    });
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      CAM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p => this.navigationPageService.navigateByRouter(p, () => this.dataHasChanged(), () => this.validateAndSaveSession()),
        {
          [CAMRoutePaths.CoursePlanningCycleDetailPage]: {
            textFn: () => (this.coursePlanningCycle != null ? this.coursePlanningCycle.title : '')
          },
          [CAMRoutePaths.CourseDetailPage]: { textFn: () => this.course.courseName },
          [CAMRoutePaths.ClassRunDetailPage]: { textFn: () => this.classRun.classTitle },
          [CAMRoutePaths.SessionDetailPage]: { textFn: () => this.session.sessionTitle }
        }
      )
    );
  }
}

export const sessionDetailPageTabIndexMap = {
  0: CAMTabConfiguration.SessionInfoTab
};
