import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BreadcrumbItem,
  BreadcrumbService,
  LMMRoutePaths,
  LMMTabConfiguration,
  NavigationData,
  NavigationPageService,
  RouterPageInput,
  SessionDetailViewModel
} from '@opal20/domain-components';
import { ButtonAction, SPACING_CONTENT } from '@opal20/common-components';
import {
  ClassRun,
  ClassRunRepository,
  ContentRepository,
  Course,
  CourseRepository,
  DigitalContent,
  ISaveSessionRequest,
  SearchRegistrationsType,
  Session,
  SessionRepository
} from '@opal20/domain-api';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { Observable, Subscription, combineLatest, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import { map, switchMap, take } from 'rxjs/operators';

import { LMM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/lmm-route-breadcumb-mapping-fn';
import { NAVIGATORS } from '../lmm.config';
import { SessionDetailPageInput } from '../models/session-detail-page-input.model';

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

  public get detailPageInput(): RouterPageInput<SessionDetailPageInput, LMMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<SessionDetailPageInput, LMMTabConfiguration, unknown> | undefined) {
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

  public navigationData: NavigationData;
  public loadingData: boolean = false;
  public registrationStatusChanged: boolean = false;
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  private _detailPageInput: RouterPageInput<SessionDetailPageInput, LMMTabConfiguration, unknown> = NAVIGATORS[
    LMMRoutePaths.SessionDetailPage
  ] as RouterPageInput<SessionDetailPageInput, LMMTabConfiguration, unknown>;
  private _loadSessionInfoSub: Subscription = new Subscription();
  private _session: SessionDetailViewModel = new SessionDetailViewModel();

  public get selectedTab(): LMMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : LMMTabConfiguration.SessionInfoTab;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private classRunRepository: ClassRunRepository,
    private sessionRepository: SessionRepository,
    private navigationPageService: NavigationPageService,
    private courseRepository: CourseRepository,
    private breadcrumbService: BreadcrumbService,
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
        : of(new Session());
    this.loadingData = true;
    this._loadSessionInfoSub = combineLatest(courseObs, classRunObs, sessionObs)
      .pipe(
        switchMap(([course, classRun, session]) => {
          const sessionPreRecordClipObs =
            session.preRecordId != null ? this.contentRepository.loadDigitalContentById(session.preRecordId) : of(null);
          return combineLatest([sessionPreRecordClipObs]).pipe(
            map(([sessionPreRecordClip]) => <[Course, ClassRun, Session, DigitalContent]>[course, classRun, session, sessionPreRecordClip])
          );
        }),
        this.untilDestroy()
      )
      .subscribe(
        ([course, classRun, session, sessionPreRecordClip]) => {
          this.course = course;
          this.session = new SessionDetailViewModel({ sessionData: session, sessionPreRecordClip: sessionPreRecordClip });
          this.classRun = classRun;
          this.loadBreadcrumb();
          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack();
  }

  public updatePreRecordClip(preRecordClip: DigitalContent | null): void {
    if (preRecordClip != null) {
      // Load Content ById because preRecordClip from search missing fileLocation. Only get by id will return full.
      this.contentRepository
        .loadDigitalContentById(preRecordClip.id)
        .pipe(
          take(1),
          this.untilDestroy()
        )
        .subscribe(fullContent => {
          this.session.preRecordClip = fullContent;
          this.savePreRecordToSession();
        });
    } else {
      this.session.preRecordClip = null;
      this.savePreRecordToSession();
    }
  }

  public updateUsePreRecordClip(usePreRecord: boolean): void {
    this.session.usePreRecordClip = usePreRecord;
    this.savePreRecordToSession();
  }

  public saveSession(): Promise<Session> {
    const request: ISaveSessionRequest = {
      data: Utils.clone(this.session.sessionData)
    };
    return this.sessionRepository
      .saveSession(request)
      .pipe(
        map(_ => {
          this.showNotification('Saved Successfully');
          return _;
        })
      )
      .toPromise();
  }

  public savePreRecordToSession(): Promise<Session> {
    const request: ISaveSessionRequest = {
      data: Utils.clone(this.session.sessionData),
      updatePreRecordClipOnly: true
    };
    return this.sessionRepository
      .saveSession(request)
      .pipe(
        map(_ => {
          this.session.usePreRecordClip
            ? this.showNotification('Pre-recorded clip saved successfully')
            : this.showNotification('Pre-recorded clip removed successfully');
          return _;
        })
      )
      .toPromise();
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadSessionInfo();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'session-detail',
      controls: {
        sessionTitle: {
          defaultValue: null,
          validators: null
        },
        venue: {
          defaultValue: null,
          validators: null
        },
        sessionDate: {
          defaultValue: null,
          validators: null
        },
        startTime: {
          defaultValue: null,
          validators: null
        },
        endTime: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<SessionDetailPageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
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
        [LMMRoutePaths.SessionDetailPage]: { textFn: () => this.session.sessionTitle }
      })
    );
  }
}

export const sessionDetailPageTabIndexMap = {
  0: LMMTabConfiguration.SessionInfoTab
};
