import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, TemplateRef, ViewChild } from '@angular/core';
import {
  CourseFeedModel,
  FormAnswerModel,
  IGetMyOutstandingTaskRequest,
  IMyCoursesSearchRequest,
  IMyCoursesSummaryRequest,
  IMyDigitalContentSearchRequest,
  IOutstandingTask,
  ITrackingSharedDetailByModel,
  LearningCourseType,
  MetadataTagModel,
  MyCourseStatus,
  MyCoursesSummaryResult,
  MyDigitalContentStatus,
  MyLearningStatisticType,
  MyOutstandingTaskApiService,
  Newsfeed,
  OutstandingTask,
  OutstandingTaskType,
  TrackingSharedDetailByModel,
  UserPreferenceModel,
  UserPreferenceRepository
} from '@opal20/domain-api';
import { DialogAction, OpalDialogService } from '@opal20/common-components';
import { ILearningItemModel, LearningItemModel, LearningType } from '../models/learning-item.model';
import { LEARNER_PERMISSIONS, LearnerRoutePaths, MyLearningTab, QuizPlayerIntegrationsService } from '@opal20/domain-components';

import { CATALOGUE_TYPE_ENUM } from '../constants/catalogue-type.constant';
import { CatalogueDataService } from '../services/catalogue-data.service';
import { CourseDataService } from '../services/course-data.service';
import { LearnerNavigationService } from '../services/learner-navigation.service';
import { LearningActionService } from '../services/learning-action.service';
import { LearningCardListComponent } from './learning-card-list.component';
import { Observable } from 'rxjs';
import { SECTION_WIDGET } from '../constants/section-widget';
import { User } from '@opal20/authentication';
import { UserPreferenceSettingKeys } from '../models/user-preference.model';
import { UserTrackingService } from '../user-activities-tracking/user-tracking.service';
import { map } from 'rxjs/operators';

type ShowingSettingType = {
  showSettingsOutstanding: boolean;
  showingSettingsNewsFeed: boolean;
  showingSettingsMyLearning: boolean;
  showingSettingsRecommendedForYou: boolean;
  showingSettingsRecommendedByYourOrganisation: boolean;
  showingSettingsBookmarks: boolean;
  showingSettingsShared: boolean;
  showingSettingsCalendar: boolean;
};

type WigetSettingType = { name: string; userPreferenceModel: UserPreferenceModel };

const itemsPerPage: number = 12;
@Component({
  selector: 'learner-home-page',
  templateUrl: './learner-home-page.component.html'
})
export class LearnerHomePageComponent extends BasePageComponent {
  public recentInProgressLearningItems: ILearningItemModel[] = [];
  public suggestedLearningItems: ILearningItemModel[] = [];
  public organisationSuggestedLearningItems: ILearningItemModel[] = [];
  public bookmarkLearningItems: ILearningItemModel[] = [];
  public user: User;
  public showingCourseRecommendation: boolean = false;
  public showingOrganisationCourseRecommendation: boolean = false;
  public currentCourseId: string | undefined;
  public currentLearningItem: ILearningItemModel | undefined;
  public numberOfCompletedCourses: number;
  public numberOfOngoingCourses: number;
  public organisationSuggestedLearningItemsTotalCount: number;
  public suggestedLearningItemsTotalCount: number;
  public bookmarkLearningItemsTotalCount: number;
  public recentInProgressLearningItemsTotalCount: number;
  public numberOfItemsOnRecommendation: number = itemsPerPage;
  public numberOfItemsOnOrganisationRecommendation: number = itemsPerPage;
  public navigateData: { type: string };
  public showNewsFeed: boolean = false;
  public showOutstandingTask: boolean = false;
  public showStandaloneFormPlayer: boolean = false;
  public outstandingTasks: IOutstandingTask[] = [];
  public outstandingTasksTotalCount: number = 0;
  public canStartTask: boolean = false;
  public canContinueTask: boolean = false;
  public assignmentId: string;
  public showShared: boolean = false;
  public totalShared: number = 0;
  public sharedItems: ITrackingSharedDetailByModel[] = [];
  public sharedMetadata: Observable<Dictionary<MetadataTagModel>>;
  public numberOfHeightTimes: number = 1;

  @ViewChild('settingsInput', { static: false })
  public settingsTemplate: TemplateRef<unknown>;
  public listWidgets: WigetSettingType[];

  public showingSettings: ShowingSettingType = {
    showSettingsOutstanding: false,
    showingSettingsNewsFeed: false,
    showingSettingsMyLearning: false,
    showingSettingsRecommendedForYou: false,
    showingSettingsRecommendedByYourOrganisation: false,
    showingSettingsBookmarks: false,
    showingSettingsShared: false,
    showingSettingsCalendar: false
  };

  @ViewChild('courseRecommendationList', { static: false })
  private courseRecommendationListComponent: LearningCardListComponent;
  @ViewChild('organisationRecommendationList', { static: false })
  private organisationRecommendationList: LearningCardListComponent;
  private inProgressOrderBy: string = 'LastLogin DESC';

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private courseDataService: CourseDataService,
    private learnerNavigationService: LearnerNavigationService,
    private learningActionService: LearningActionService,
    private catalogueDataService: CatalogueDataService,
    private opalDialogService: OpalDialogService,
    private userPreferenceRepository: UserPreferenceRepository,
    private quizPlayerService: QuizPlayerIntegrationsService,
    private myOutstandingTaskApiService: MyOutstandingTaskApiService,
    private userTrackingService: UserTrackingService
  ) {
    super(moduleFacadeService);
    this.user = AppGlobal.user;
  }

  public onInit(): void {
    this.userPreferenceRepository
      .loadUserPreferences()
      .pipe(this.untilDestroy())
      .subscribe(results => {
        const newsFeed = results.find(u => u.key === UserPreferenceSettingKeys.HomeNewsFeed);
        const outstanding = results.find(u => u.key === UserPreferenceSettingKeys.HomeOutstanding);
        const shared = results.find(u => u.key === UserPreferenceSettingKeys.HomeShared);
        const myLearning = results.find(u => u.key === UserPreferenceSettingKeys.HomeMyLearning);
        const recommendForU = results.find(u => u.key === UserPreferenceSettingKeys.HomeRecommendForU);
        const recommendForOrg = results.find(u => u.key === UserPreferenceSettingKeys.HomeRecommendForOrg);
        const bookmark = results.find(u => u.key === UserPreferenceSettingKeys.HomeBookmark);
        const calendar = results.find(u => u.key === UserPreferenceSettingKeys.HomeCalendar);

        this.showingSettings = {
          showSettingsOutstanding: <boolean>outstanding.value,
          showingSettingsMyLearning: <boolean>myLearning.value,
          showingSettingsRecommendedForYou: <boolean>recommendForU.value,
          showingSettingsRecommendedByYourOrganisation: <boolean>recommendForOrg.value,
          showingSettingsBookmarks: <boolean>bookmark.value,
          showingSettingsNewsFeed: <boolean>newsFeed.value,
          showingSettingsShared: <boolean>shared.value,
          showingSettingsCalendar: calendar ? <boolean>calendar.value : true
        };

        if (!this.showingSettings.showingSettingsCalendar) {
          this.numberOfHeightTimes += 1;
        }

        if (!this.showingSettings.showSettingsOutstanding) {
          this.numberOfHeightTimes += 1;
        }

        if (!this.showingSettings.showingSettingsShared) {
          this.numberOfHeightTimes += 1;
        }

        if (!this.showingSettings.showingSettingsNewsFeed) {
          // reset heighttimes if newsfeed hidden
          this.numberOfHeightTimes = 1;
        }

        this.listWidgets = [
          {
            name: SECTION_WIDGET.NewsFeed,
            userPreferenceModel: newsFeed
          },
          {
            name: SECTION_WIDGET.Calendar,
            userPreferenceModel: calendar
          },
          {
            name: SECTION_WIDGET.Outstanding,
            userPreferenceModel: outstanding
          },
          {
            name: SECTION_WIDGET.Shared,
            userPreferenceModel: shared
          },
          {
            name: SECTION_WIDGET.MyLearning,
            userPreferenceModel: myLearning
          },
          {
            name: SECTION_WIDGET.RecommendedForYou,
            userPreferenceModel: recommendForU
          },
          {
            name: SECTION_WIDGET.RecommendedByYourOrganisation,
            userPreferenceModel: recommendForOrg
          },
          {
            name: SECTION_WIDGET.Bookmarks,
            userPreferenceModel: bookmark
          }
        ];

        this.loadData();
      });
  }

  public getPagedSuggestedCoursesCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.catalogueDataService.getSuggestedCourses(maxResultCount, skipCount).pipe(
      map(result => {
        const items = result.items;
        return {
          total: result.total,
          items: items
        };
      })
    );
  }

  public getOrganisationSuggestedCoursesCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.courseDataService.getOrganisationSuggestedCourses(maxResultCount, skipCount).pipe(
      map(result => {
        return {
          total: result.total,
          items: result.items
        };
      })
    );
  }

  public loadData(): void {
    this.navigateData = this.getNavigateData();
    this.loadRecentInProgressCourses();
    this.loadSuggestedCourses();
    this.loadOrganisationSuggestedLearningItems();
    this.loadBookmarkCourses();
    this.loadMyCoursesSummaryByStatus([MyCourseStatus.Completed, MyCourseStatus.InProgress]);
    this.listenBookmarkChanged();
    this.loadOutstandingTasks();
    this.loadSharedItems();

    // when the data inside navigation has value and matched with the below condition => force load by
    if (this.navigateData && this.navigateData.type) {
      switch (this.navigateData.type) {
        case CATALOGUE_TYPE_ENUM.RecommendedForYou:
          this.showingCourseRecommendation = true;
          break;
        case CATALOGUE_TYPE_ENUM.RecommendedByYourOrganisation:
          this.showingOrganisationCourseRecommendation = true;
          break;
        case LearnerRoutePaths.Newsfeed:
          this.showNewsFeed = true;
        default:
          break;
      }
      return;
    }
    this.updateDeeplink(`learner/${LearnerRoutePaths.Home}`);
  }

  public listenBookmarkChanged(): void {
    this.learningActionService.bookmarkChanged.pipe(this.untilDestroy()).subscribe(_ => {
      this.loadBookmarkCourses();
    });
  }

  public onNewsFeedClick(event: Newsfeed): void {
    this.navigateToNewsFeedCourseDetail(event);
  }

  public onOutstandingTaskClick(event: OutstandingTask): void {
    this.showOutstandingTask = false;
    this.navigateToTaskDetail(event);
  }

  public onSharedClick(event: TrackingSharedDetailByModel): void {
    this.showShared = false;
    this.navigateToSharedDetail(event);
  }

  public onLoadMoreNewsFeedClick(): void {
    this.showNewsFeed = true;
  }

  public onShowMoreOutstandingTaskClicked(): void {
    this.showOutstandingTask = true;
  }

  public onNewsFeedBack(): void {
    if (this.showNewsFeed) {
      this.showNewsFeed = false;
    }
  }

  public onOutstandingTaskBack(): void {
    if (this.showOutstandingTask) {
      this.showOutstandingTask = false;
      this.loadOutstandingTasks();
    }
  }

  public onStandaloneFormBackClick(): void {
    this.showStandaloneFormPlayer = false;
    this.loadOutstandingTasks();
  }

  public onLearningCardClick(event: ILearningItemModel): void {
    this.currentLearningItem = event;
    if (event instanceof LearningItemModel) {
      this.onMicroLearningClick(event);
      return;
    }
    this.currentCourseId = event.id;
    this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${event.type.toLocaleLowerCase()}/${this.currentCourseId}`);
  }

  public onMicroLearningClick(event: LearningItemModel): void {
    this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${event.type.toLocaleLowerCase()}/${event.id}`);

    if (event.isExpiredCourse) {
      return;
    }

    this.currentCourseId = event.id;
  }

  public onShowCourseRecommendationClick(): void {
    this.updateDeeplink(`learner/${LearnerRoutePaths.Home}/${CATALOGUE_TYPE_ENUM.RecommendedForYou}`);
    this.showingCourseRecommendation = true;
  }

  public onShowOrganisationCourseRecommendationsClick(): void {
    this.updateDeeplink(`learner/${LearnerRoutePaths.Home}/${CATALOGUE_TYPE_ENUM.RecommendedByYourOrganisation}`);
    this.showingOrganisationCourseRecommendation = true;
  }

  public onShowMyLearningClick(): void {
    this.learnerNavigationService.navigateTo(LearnerRoutePaths.MyLearning, { activeTab: MyLearningTab.Courses });
  }

  public onShowBookmarkClick(): void {
    this.learnerNavigationService.navigateTo(LearnerRoutePaths.MyLearning, { activeTab: MyLearningTab.Bookmarks });
  }

  public onCourseRecommendationBackButtonClick(event: boolean): void {
    if (event) {
      this.loadBookmarkCourses(false);
    }
    this.updateDeeplink(`learner/${LearnerRoutePaths.Home}`);
    this.showingCourseRecommendation = false;
  }

  public onOrganisationCourseRecommendationBackButtonClick(event: boolean): void {
    this.updateDeeplink(`learner/${LearnerRoutePaths.Home}`);
    this.showingOrganisationCourseRecommendation = false;
  }

  public onCourseDetailBackClick(): void {
    this.currentCourseId = undefined;
    this.currentLearningItem = undefined;
    this.loadData();
    if (this.showingCourseRecommendation && this.courseRecommendationListComponent !== undefined) {
      this.updateDeeplink(`learner/${LearnerRoutePaths.Home}/${CATALOGUE_TYPE_ENUM.RecommendedForYou}`);
      this.courseRecommendationListComponent.triggerDataChange();
    } else if (this.showingOrganisationCourseRecommendation && this.organisationRecommendationList !== undefined) {
      this.updateDeeplink(`learner/${LearnerRoutePaths.Home}/${CATALOGUE_TYPE_ENUM.RecommendedByYourOrganisation}`);
      this.organisationRecommendationList.triggerDataChange();
    } else {
      this.updateDeeplink(`learner/${LearnerRoutePaths.Home}`);
    }
  }

  public showSettingsDialog(): void {
    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Customize homepage widgets',
        confirmMsg: 'Please select widget(s) to display on your homepage. At least one widget must be selected.',
        yesBtnText: 'Submit',
        noBtnText: 'Cancel',
        bodyTemplate: this.settingsTemplate,
        disableYesBtnFn: () => !this.hasAnySelectedWidget()
      })
      .subscribe(action => {
        if (action === DialogAction.OK) {
          this.userPreferenceRepository.updateUserPreferences(this.listWidgets.map(w => w.userPreferenceModel));
          this.numberOfHeightTimes = 1;
        }
      });
  }

  public hasAnySelectedWidget(): boolean {
    return this.listWidgets.some(v => v.userPreferenceModel.value);
  }

  public changedSelectedWidget(widget: WigetSettingType): void {
    widget.userPreferenceModel.value = !widget.userPreferenceModel.value;
  }

  public onAssignmentDetailBackClick(): void {
    this.assignmentId = undefined;
    this.loadOutstandingTasks();
  }

  public get currentUserName(): string {
    return this.user && this.user.fullName ? this.user.fullName : AppGlobal.user.firstName;
  }

  public get sectionWidget(): typeof SECTION_WIDGET {
    return SECTION_WIDGET;
  }

  public onShowMoreSharedClicked(): void {
    this.showShared = true;
  }

  public onSharedBack(): void {
    if (this.showShared) {
      this.showShared = false;
    }
  }

  public get customizeWidgetPermissionKey(): string {
    return LEARNER_PERMISSIONS.Home_Setting;
  }

  private loadRecentInProgressCourses(): void {
    const myCourseRequest: IMyCoursesSearchRequest = {
      skipCount: 0,
      maxResultCount: 20,
      searchText: '',
      orderBy: this.inProgressOrderBy,
      courseType: LearningCourseType.FaceToFace,
      statusFilter: MyLearningStatisticType.InProgress
    };

    const microLearningRequest: IMyCoursesSearchRequest = {
      skipCount: 0,
      maxResultCount: 10,
      searchText: '',
      orderBy: this.inProgressOrderBy,
      courseType: LearningCourseType.Microlearning,
      statusFilter: MyLearningStatisticType.InProgress
    };

    const contentRequest: IMyDigitalContentSearchRequest = {
      maxResultCount: 10,
      skipCount: 0,
      orderBy: 'CreatedDate desc',
      statusFilter: MyDigitalContentStatus.InProgress
    };
    this.courseDataService
      .getMyLearnings(myCourseRequest, microLearningRequest, contentRequest)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.recentInProgressLearningItems = result.items;
        this.recentInProgressLearningItemsTotalCount = result.total;
      });
  }

  private loadSuggestedCourses(): void {
    this.catalogueDataService
      .getSuggestedCourses()
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.suggestedLearningItems = result.items;
        this.suggestedLearningItemsTotalCount = result.total;
      });
  }

  private loadOrganisationSuggestedLearningItems(showSpinner: boolean = true): void {
    this.courseDataService
      .getOrganisationSuggestedCourses()
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.organisationSuggestedLearningItems = result.items;
        this.organisationSuggestedLearningItemsTotalCount = result.total;
      });
  }

  private loadBookmarkCourses(showSpinner: boolean = true): void {
    this.courseDataService
      .getMyBookmarks(showSpinner)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.bookmarkLearningItems = result.items;
        this.bookmarkLearningItemsTotalCount = result.total;
      });
  }

  private loadMyCoursesSummaryByStatus(status: MyCourseStatus[]): void {
    const request: IMyCoursesSummaryRequest = {
      statusFilter: status
    };
    this.courseDataService.getMyCoursesSummary(request).then((result: MyCoursesSummaryResult[]) => {
      if (result) {
        const completedSummary = result.find(x => x.statusFilter === MyCourseStatus.Completed);
        this.numberOfCompletedCourses = completedSummary ? completedSummary.total : 0;

        const inProgressSummary = result.find(x => x.statusFilter === MyCourseStatus.InProgress);
        this.numberOfOngoingCourses = inProgressSummary ? inProgressSummary.total : 0;
      }
    });
  }

  private navigateToNewsFeedCourseDetail(event: Newsfeed): void {
    if (event instanceof CourseFeedModel) {
      const courseFeed = event as CourseFeedModel;
      const learningItem: ILearningItemModel = {
        id: courseFeed.courseId,
        title: `${LearningType.Course}`,
        isBookmark: false,
        type: LearningType.Course
      };
      this.currentLearningItem = learningItem;

      this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${LearningType.Course.toLocaleLowerCase()}/${courseFeed.courseId}`);
    }
  }

  private loadOutstandingTasks(): void {
    this.canContinueTask = false;
    this.canStartTask = false;
    this.myOutstandingTaskApiService
      .getOutstandingTasks(<IGetMyOutstandingTaskRequest>{
        maxResultCount: this.numberOfHeightTimes === 3 ? 14 : 7, // number of data display, depend on height times
        skipCount: 0
      })
      .then(result => {
        this.outstandingTasksTotalCount = result.totalCount;
        this.outstandingTasks = result.items;
      });
  }

  private navigateToTaskDetail(event: OutstandingTask): void {
    if (event instanceof OutstandingTask) {
      switch (event.type) {
        case OutstandingTaskType.Course:
        case OutstandingTaskType.Microlearning:
          this.initLearningItem(event.courseId, '${LearningType.Course}', LearningType.Course);
          this.detectToStartOrContinueTask(event);

          this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${LearningType.Course.toLocaleLowerCase()}/${event.courseId}`);
          break;
        case OutstandingTaskType.DigitalContent:
          this.initLearningItem(event.digitalContentId, '${LearningType.DigitalContent}', LearningType.DigitalContent);
          this.detectToStartOrContinueTask(event);

          this.updateDeeplink(
            `learner/${LearnerRoutePaths.Detail}/${LearningType.DigitalContent.toLocaleLowerCase()}/${event.digitalContentId}`
          );
          break;
        case OutstandingTaskType.Assignment:
          this.openAssignmentPlayer(event);
          break;
        case OutstandingTaskType.StandaloneForm:
          this.openStandaloneFormPlayer(event);
          break;
      }
    }
  }

  private loadSharedItems(): void {
    const skipCount = 0;
    const maxResultCount = this.numberOfHeightTimes === 3 ? 14 : 7; // number of data display, depend on height times
    this.userTrackingService.getSharedToIncludedTag(skipCount, maxResultCount).then(result => {
      this.sharedItems = result.items;
      this.totalShared = result.totalCount;

      this.sharedMetadata = this.courseDataService.getCourseMetadata().pipe(map(tags => Utils.toDictionary(tags, p => p.id)));
    });
  }

  private navigateToSharedDetail(event: TrackingSharedDetailByModel): void {
    if (event instanceof TrackingSharedDetailByModel) {
      const learningType = event.itemType === LearningType.DigitalContent ? LearningType.DigitalContent : LearningType.Course;
      this.initLearningItem(event.itemId, event.title, learningType);

      this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${learningType.toLocaleLowerCase()}/${event.itemId}`);
    }
  }

  private openAssignmentPlayer(outstandingTask: OutstandingTask): void {
    this.assignmentId = outstandingTask.assignmentId;
    this.detectToStartOrContinueTask(outstandingTask);
  }

  private openStandaloneFormPlayer(outstandingTask: OutstandingTask): void {
    this.showStandaloneFormPlayer = true;
    this.quizPlayerService.setup({
      onQuizFinished: (formAnswerModel: FormAnswerModel) => {
        // After a user completes the poll form, the player will show the poll results page.
        if (outstandingTask.formType !== 'Poll') {
          this.showStandaloneFormPlayer = false;
        }
      }
    });

    this.quizPlayerService.setFormOriginalObjectId(outstandingTask.formId);
  }

  private detectToStartOrContinueTask(outstandingTask: OutstandingTask): void {
    if (outstandingTask.isNotStarted) {
      this.canStartTask = true;
    } else {
      this.canContinueTask = true;
    }
  }

  private initLearningItem(id: string, title: string, type: LearningType): void {
    const learningItem = {
      id: id,
      title: title,
      isBookmark: false,
      type: type
    };

    this.currentLearningItem = learningItem;
  }
}
