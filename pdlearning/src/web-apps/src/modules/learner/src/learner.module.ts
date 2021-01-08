import {
  AbsenceDetailDialogComponent,
  CommentDialogComponent,
  DetailContentFragment,
  DomainComponentsModule,
  INavigationMenuItem,
  LearnerRoutePaths,
  LearningPathBasicInfoTabComponent,
  NavigationMenuFragment,
  NavigationMenuService
} from '@opal20/domain-components';
import {
  BaseModuleOutlet,
  BaseRoutingModule,
  Fragment,
  FunctionModule,
  MODULE_INPUT_DATA,
  ModuleDataService,
  ModuleFacadeService,
  NAVIGATION_PARAMETERS_KEY,
  TranslationMessage,
  TranslationModule
} from '@opal20/infrastructure';
import { ButtonModule, ButtonsModule } from '@progress/kendo-angular-buttons';
import { CATALOGUE_TYPE_ENUM, CATALOGUE_TYPE_MAPPING_TEXT_CONST } from './constants/catalogue-type.constant';
import {
  CollaborativeSocialLearningApiModule,
  ContentDomainApiModule,
  IndividualDevelopmentPlanApiService,
  LearnerDomainApiModule,
  LearningCatalogueDomainApiModule,
  TaggingDomainApiModule,
  UserDomainApiModule
} from '@opal20/domain-api';
import { ContextMenuModule, MenuModule } from '@progress/kendo-angular-menu';
import { Injector, NgModule, Type } from '@angular/core';
import { Observable, of } from 'rxjs';

import { BookmarkIconComponent } from './components/bookmark-icon.component';
import { CalendarModule } from 'players/calendar/src/calendar.module';
import { CalendarPageComponent } from './components/calendar-page.component';
import { CatalogSearchFilterComponent } from './components/learner-catalogue-search-filter.component';
import { CatalogueDataService } from './services/catalogue-data.service';
import { ClassRunComponent } from './components/learner-classrun.component';
import { CommonComponentsModule } from '@opal20/common-components';
import { CourseDataService } from './services/course-data.service';
import { CslDataService } from './services/csl-data.service';
import { DigitalContentCardComponent } from './components/digital-content-card.component';
import { DigitalContentDataService } from './services/digital-content.service';
import { DigitalContentLongCardComponent } from './components/digital-content-long-card.component';
import { DigitalContentMoreDetailComponent } from './components/learner-digital-content-more-detail.component';
import { EPortfolioPageComponent } from './components/e-porfolio-page.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LearnerAssignmentCommentComponent } from './components/learner-assignment-comment.component';
import { LearnerAssignmentDetailComponent } from './components/learner-assignment-detail.component';
import { LearnerCataloguePageComponent } from './components/learner-catalogue-page.component';
import { LearnerClassRunItemComponent } from './components/learner-classrun-item.component';
import { LearnerClassRunMessageComponent } from './components/learner-classrun-message.component';
import { LearnerComponent } from './learner.components';
import { LearnerCourseContentComponent } from './components/learner-course-content.component';
import { LearnerCourseDetailComponent } from './components/learner-course-detail.component';
import { LearnerCourseMoreDetailsComponent } from './components/learner-course-more-details.component';
import { LearnerDigitalContentDetailComponent } from './components/learner-digital-content-detail.component';
import { LearnerDigitalContentPlayerComponent } from './components/learner-digital-content-player.component';
import { LearnerFragmentPosition } from './learner-fragment-position';
import { LearnerHomePageComponent } from './components/learner-home-page.component';
import { LearnerLearningPathDataService } from './services/learningpath-data.service';
import { LearnerLearningPathFormComponent } from './components/learner-learning-path-dialog/learner-learning-path-form.component';
import { LearnerMetadataTreeView } from './components/learner-metadata-tree-view.component';
import { LearnerMyAchievementsPageComponent } from './components/learner-my-achievements-page.component';
import { LearnerMyLearningComponent } from './components/learner-my-learning.component';
import { LearnerMyLearningPageComponent } from './components/learner-my-learning-page.component';
import { LearnerNavigationService } from './services/learner-navigation.service';
import { LearnerNewsFeedPageComponent } from './components/learner-newsfeed-page.component';
import { LearnerOutletComponent } from './learner-outlet.component';
import { LearnerPdPlanPageComponent } from './components/learner-pdplan-page.component';
import { LearnerPermissionGuardService } from './user-activities-tracking/learner-permission-guard.service';
import { LearnerPermissionHelper } from './learner-permission-helper';
import { LearnerReviewEditingDialogComponent } from './components/learner-review-list/learner-review-editing-dialog.component';
import { LearnerReviewListComponent } from './components/learner-review-list/learner-review-list.component';
import { LearnerRoutingModule } from './learner-routing.module';
import { LearnerSearchFilterResultComponent } from './components/learner-search-filter-result/learner-search-filter-result.component';
import { LearnerToolbarComponent } from './components/learner-toolbar.component';
import { LearnerWithdrawalReasonDialog } from './components/learner-withdrawal-reason-dialog/learner-withdrawal-reason-dialog.component';
import { LearningActionService } from './services/learning-action.service';
import { LearningAssignmentsComponent } from './components/learning-assignments.component';
import { LearningBookmarksComponent } from './components/learning-bookmarks.component';
import { LearningCalendarListComponent } from './components/learning-calendar-list.component';
import { LearningCardComponent } from './components/learning-card.component';
import { LearningCardListComponent } from './components/learning-card-list.component';
import { LearningCarouselComponent } from './components/learning-carousel.component';
import { LearningCommentComponent } from './components/learning-comment.component';
import { LearningCommunitiesComponent } from './components/learning-communities.component';
import { LearningCommunityCardComponent } from './components/learning-community-card.component';
import { LearningCoursesComponent } from './components/learning-courses.component';
import { LearningDetailPage } from './components/learning-detail-page.component';
import { LearningDigitalBadgesListComponent } from './components/learning-digital-badges-list.component';
import { LearningDigitalBadgesPageComponent } from './components/learning-digital-badges-page.component';
import { LearningDigitalContentComponent } from './components/learning-digital-content.component';
import { LearningECertificatesListComponent } from './components/learning-ecertificates-list.component';
import { LearningECertificatesPageComponent } from './components/learning-ecertificates-page.component';
import { LearningFeedbacksComponent } from './components/learning-feedbacks.component';
import { LearningLongCardComponent } from './components/learning-long-card.component';
import { LearningMicrolearningComponent } from './components/learning-microlearning.component';
import { LearningNewsFeedComponent } from './components/learner-newsfeed/learning-newsfeed.component';
import { LearningNewsFeedCourseComponent } from './components/learner-newsfeed/learning-newsfeed-course.component';
import { LearningNewsFeedListComponent } from './components/learner-newsfeed/learning-newsfeed-list.component';
import { LearningOutstandingListComponent } from './components/learning-outstanding-list.component';
import { LearningOutstandingPageComponent } from './components/learning-outstanding-page.component';
import { LearningPathCardComponent } from './components/learning-path-card.component';
import { LearningPathListComponent } from './components/learning-path-list.component';
import { LearningSharedListComponent } from './components/learning-shared-list.component';
import { LearningSharedPageComponent } from './components/learning-shared-page.component';
import { LecturePlayerComponent } from './components/lecture-player.component';
import { MegaMenuDataService } from './services/mega-menu-data.service';
import { MyAchievementService } from './services/my-achievement.service';
import { MyAssignmentDataService } from './services/my-assignment-data.service';
import { MyLearningPathDataService } from './services/my-learning-path-data.service';
import { MyLearningSearchDataService } from './services/my-learning-search.service';
import { PermalinkIconComponent } from './components/permalink-icon.component';
import { PopupModule } from '@progress/kendo-angular-popup';
import { QuickStatusFiterComponent } from './components/quick-status-fiter.component';
import { RatingStarsComponent } from './components/rating-stars.component';
import { ReportsPageComponent } from './components/reports-page.component';
import { ReversePipe } from './pipes/reverse.pipe';
import { ReviewItemComponent } from './components/learner-review-list/review-item.component';
import { Router } from '@angular/router';
import { SafePipe } from './pipes/safe.pipe';
import { StandaloneFormCardComponent } from './components/standalone-form-card.component';
import { StandaloneFormDataService } from './services/standalone-form-service';
import { TabStripModule } from '@progress/kendo-angular-layout';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { TrackingSourceService } from './user-activities-tracking/tracking-souce.service';
import { TrackingTargetComponent } from './user-activities-tracking/tracking-target.component';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { UserTrackingService } from './user-activities-tracking/user-tracking.service';

@NgModule({
  imports: [
    TranslationModule.registerModules([{ moduleId: 'learner' }]),
    FunctionModule,
    LearnerRoutingModule,
    CommonComponentsModule,
    DomainComponentsModule,
    LearnerDomainApiModule,
    ContentDomainApiModule,
    TaggingDomainApiModule,
    UserDomainApiModule,
    LearningCatalogueDomainApiModule,
    ButtonModule,
    ButtonsModule,
    GridModule,
    TabStripModule,
    PopupModule,
    TreeViewModule,
    TooltipModule,
    InputsModule,
    CollaborativeSocialLearningApiModule,
    CalendarModule,
    ContextMenuModule,
    MenuModule
  ],
  declarations: [
    LearnerOutletComponent,
    LearnerComponent,
    LearnerHomePageComponent,
    LearningCardComponent,
    LearningCarouselComponent,
    LearningCardListComponent,
    LearnerCataloguePageComponent,
    LearnerMyLearningPageComponent,
    LearnerMyLearningComponent,
    LearningLongCardComponent,
    LearnerCourseDetailComponent,
    LearnerDigitalContentDetailComponent,
    ReviewItemComponent,
    LecturePlayerComponent,
    RatingStarsComponent,
    LearnerCourseContentComponent,
    LearnerMetadataTreeView,
    LearnerClassRunItemComponent,
    DigitalContentCardComponent,
    StandaloneFormCardComponent,
    DigitalContentLongCardComponent,
    LearningPathCardComponent,
    LearningPathListComponent,
    LearningBookmarksComponent,
    LearningAssignmentsComponent,
    LearningDigitalContentComponent,
    LearningCoursesComponent,
    LearnerPdPlanPageComponent,
    SafePipe,
    ClassRunComponent,
    LearningMicrolearningComponent,
    LearnerDigitalContentPlayerComponent,
    LearningDetailPage,
    LearnerCourseMoreDetailsComponent,
    DigitalContentMoreDetailComponent,
    CatalogSearchFilterComponent,
    LearningFeedbacksComponent,
    LearningCommentComponent,
    LearnerClassRunMessageComponent,
    LearnerAssignmentDetailComponent,
    EPortfolioPageComponent,
    LearningCommunitiesComponent,
    LearningCommunityCardComponent,
    QuickStatusFiterComponent,
    LearnerLearningPathFormComponent,
    LearnerToolbarComponent,
    ReportsPageComponent,
    TrackingTargetComponent,
    PermalinkIconComponent,
    BookmarkIconComponent,
    LearnerAssignmentCommentComponent,
    ReversePipe,
    LearnerSearchFilterResultComponent,
    LearnerWithdrawalReasonDialog,
    LearningNewsFeedListComponent,
    LearningNewsFeedComponent,
    LearningNewsFeedCourseComponent,
    LearnerReviewListComponent,
    LearnerReviewEditingDialogComponent,
    CalendarPageComponent,
    LearningOutstandingListComponent,
    LearnerNewsFeedPageComponent,
    LearningOutstandingPageComponent,
    LearningSharedListComponent,
    LearningSharedPageComponent,
    LearningCalendarListComponent,
    LearnerMyAchievementsPageComponent,
    LearningECertificatesPageComponent,
    LearningECertificatesListComponent,
    LearningDigitalBadgesPageComponent,
    LearningDigitalBadgesListComponent
  ],
  entryComponents: [
    LearnerOutletComponent,
    AbsenceDetailDialogComponent,
    LearningPathBasicInfoTabComponent,
    CommentDialogComponent,
    LearnerWithdrawalReasonDialog,
    LearnerReviewEditingDialogComponent
  ],
  exports: [LearnerOutletComponent],
  providers: [
    CourseDataService,
    LearnerNavigationService,
    IndividualDevelopmentPlanApiService,
    DigitalContentDataService,
    StandaloneFormDataService,
    LearningActionService,
    CatalogueDataService,
    LearnerLearningPathDataService,
    MyAssignmentDataService,
    CslDataService,
    MyLearningPathDataService,
    MegaMenuDataService,
    TrackingSourceService,
    MyLearningSearchDataService,
    UserTrackingService,
    LearnerPermissionGuardService,
    MyAchievementService
  ],
  bootstrap: [LearnerComponent]
})
export class LearnerModule extends BaseRoutingModule {
  protected megaMenuItems: INavigationMenuItem[];
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected router: Router,
    protected injector: Injector,
    protected navigationMenuService: NavigationMenuService
  ) {
    super(moduleFacadeService, router, injector);
  }

  protected initNavigationService(): void {
    const navigationService: NavigationMenuService = this.injector.get(NavigationMenuService);
    const megaMenuData: MegaMenuDataService = this.injector.get(MegaMenuDataService);
    this.megaMenuItems = [];

    megaMenuData.onServiceSchemeChange.subscribe(data => {
      this.megaMenuItems.push(...data);

      this.megaMenuItems.forEach(item => {
        if (item && item.data) {
          (item.data as INavigationMenuItem[]).forEach(p => {
            p.onClick = () => {
              this.moduleFacadeService.navigationService.navigateTo(
                `${LearnerRoutePaths.Catalogue}/servicescheme/${p.parentId}/subject/${p.id}`,
                {
                  filterCriteria: {
                    serviceScheme: p.parentId,
                    subjectArea: p.id
                  }
                }
              );
            };
          });
        }
      });
    });

    const subMenuData: INavigationMenuItem[] = [
      {
        id: CATALOGUE_TYPE_ENUM.AllCourses,
        name: CATALOGUE_TYPE_MAPPING_TEXT_CONST.get(CATALOGUE_TYPE_ENUM.AllCourses),
        onClick: () => {
          this.moduleFacadeService.navigationService.navigateTo(`${LearnerRoutePaths.Catalogue}/${CATALOGUE_TYPE_ENUM.AllCourses}`, {
            catalogueType: CATALOGUE_TYPE_ENUM.AllCourses,
            displayText: CATALOGUE_TYPE_MAPPING_TEXT_CONST.get(CATALOGUE_TYPE_ENUM.AllCourses)
          });
        }
      },
      {
        id: CATALOGUE_TYPE_ENUM.Courses,
        name: CATALOGUE_TYPE_MAPPING_TEXT_CONST.get(CATALOGUE_TYPE_ENUM.Courses),
        onClick: () => {
          this.moduleFacadeService.navigationService.navigateTo(`${LearnerRoutePaths.Catalogue}/${CATALOGUE_TYPE_ENUM.Courses}`, {
            catalogueType: CATALOGUE_TYPE_ENUM.Courses,
            displayText: CATALOGUE_TYPE_MAPPING_TEXT_CONST.get(CATALOGUE_TYPE_ENUM.Courses)
          });
        }
      },
      {
        id: CATALOGUE_TYPE_ENUM.Microlearning,
        name: CATALOGUE_TYPE_MAPPING_TEXT_CONST.get(CATALOGUE_TYPE_ENUM.Microlearning),
        onClick: () => {
          this.moduleFacadeService.navigationService.navigateTo(`${LearnerRoutePaths.Catalogue}/${CATALOGUE_TYPE_ENUM.Microlearning}`, {
            catalogueType: CATALOGUE_TYPE_ENUM.Microlearning,
            displayText: CATALOGUE_TYPE_MAPPING_TEXT_CONST.get(CATALOGUE_TYPE_ENUM.Microlearning)
          });
        }
      },
      {
        id: CATALOGUE_TYPE_ENUM.DigitalContent,
        name: CATALOGUE_TYPE_MAPPING_TEXT_CONST.get(CATALOGUE_TYPE_ENUM.DigitalContent),
        onClick: () => {
          this.moduleFacadeService.navigationService.navigateTo(`${LearnerRoutePaths.Catalogue}/${CATALOGUE_TYPE_ENUM.DigitalContent}`, {
            catalogueType: CATALOGUE_TYPE_ENUM.DigitalContent,
            displayText: CATALOGUE_TYPE_MAPPING_TEXT_CONST.get(CATALOGUE_TYPE_ENUM.DigitalContent)
          });
        }
      }
    ];

    const navigationItems: INavigationMenuItem[] = [
      {
        id: LearnerRoutePaths.Home,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Home'),
        isActivated: true
      },
      {
        id: LearnerRoutePaths.MyLearning,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'My Learning')
      },
      {
        id: LearnerRoutePaths.Catalogue,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Catalogue'),
        menuType: 'learner-catalog',
        data: this.megaMenuItems,
        subData: subMenuData
      },
      {
        id: LearnerRoutePaths.Calendar,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Calendar')
      },
      {
        id: LearnerRoutePaths.PdPlan,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'PD Plan')
      },
      {
        id: LearnerRoutePaths.MyAchievements,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'My Achievements')
      },
      {
        id: LearnerRoutePaths.EPortfolio,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'E-Portfolio')
      },
      {
        id: LearnerRoutePaths.ReportsPage,
        name: new TranslationMessage(this.moduleFacadeService.translator, 'Reports')
      }
    ];

    navigationService.init(
      (menuId, parameters, skipLocationChange) =>
        this.moduleFacadeService.navigationService.navigateTo(menuId, parameters, skipLocationChange),
      navigationItems.filter(route => LearnerPermissionHelper.hasPermissionToAccessRoute(route.id))
    );
  }

  protected get defaultPath(): Observable<string> {
    const moduleDataService: ModuleDataService = this.moduleFacadeService.moduleDataService;
    const moduleData: { path: string; navigationData: { courseId: string } } =
      moduleDataService && moduleDataService.getData(MODULE_INPUT_DATA);
    const navigationService: NavigationMenuService = this.injector.get(NavigationMenuService);

    if (moduleData) {
      navigationService.activate(moduleData.path);
      this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, moduleData.navigationData);

      return of(moduleData.path);
    }

    return of(null);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return LearnerOutletComponent;
  }

  protected get fragments(): { [position: string]: Type<Fragment> } {
    return {
      [LearnerFragmentPosition.NavigationMenu]: NavigationMenuFragment,
      [LearnerFragmentPosition.DetailContentContainer]: DetailContentFragment
    };
  }

  protected onInit(): void {
    this.shellManager.registerDefaultFragments();
  }

  protected onDestroy(): void {
    this.shellManager.unregisterDefaultFragments();
  }
}
