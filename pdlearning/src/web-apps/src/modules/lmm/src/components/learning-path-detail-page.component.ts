import { BaseFormComponent, IFormBuilderDefinition, IntervalScheduler, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BookmarkType,
  CourseRepository,
  GetCountUserBookmarkedResult,
  ISaveLearningPathRequest,
  LearningPathModel,
  LearningPathRepository,
  LearningPathStatus,
  MyBookmarkApiService,
  TaggingRepository,
  UserInfoModel
} from '@opal20/domain-api';
import {
  BreadcrumbItem,
  BreadcrumbService,
  LEARNING_PATH_STATUS_COLOR_MAP,
  LMMRoutePaths,
  LMMTabConfiguration,
  LearningPathDetailMode,
  LearningPathDetailViewModel,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import { ButtonGroupButton, OpalDialogService, SPACING_CONTENT, requiredAndNoWhitespaceValidator } from '@opal20/common-components';
import { Component, HostBinding } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { CourseDetailDialogComponent } from './dialogs/course-detail-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { LMM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/lmm-route-breadcumb-mapping-fn';
import { LearningPathDetailPageInput } from '../models/learning-path-detail-page-input.model';
import { NAVIGATORS } from '../lmm.config';
import { SelectEvent } from '@progress/kendo-angular-layout';

@Component({
  selector: 'learning-path-detail-page',
  templateUrl: './learning-path-detail-page.component.html'
})
export class LearningPathDetailPageComponent extends BaseFormComponent {
  public get detailPageInput(): RouterPageInput<LearningPathDetailPageInput, LMMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<LearningPathDetailPageInput, LMMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadLearningPathInfo();
      }
    }
  }

  public get title(): string {
    return this.learningPathDetailVM.learningPath.title;
  }

  public get learningPathDetailVM(): LearningPathDetailViewModel {
    return this._learningPathDetailVM;
  }
  public set learningPathDetailVM(v: LearningPathDetailViewModel) {
    this._learningPathDetailVM = v;
  }

  public loadingLearningPathCourses: boolean = false;
  public learningPathStatusColorMap = LEARNING_PATH_STATUS_COLOR_MAP;
  public stickySpacing: number = SPACING_CONTENT;
  public breadCrumbItems: BreadcrumbItem[] = [];
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public LearningPathStatus: typeof LearningPathStatus = LearningPathStatus;
  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Edit',
      onClickFn: () => this.onEditLearningPath(),
      shownIfFn: () => this.canEditLearningPath()
    },
    {
      displayText: 'Save',
      onClickFn: () => this.onSaveLearningPath(),
      shownIfFn: () => this.canSaveLearningPath()
    },
    {
      displayText: 'Publish',
      onClickFn: () => this.onPublishLearningPath(),
      shownIfFn: () => this.canPublishLearningPath()
    },
    {
      displayText: 'Unpublish',
      onClickFn: () => this.onUnpublishLearningPath(),
      shownIfFn: () => this.canUnpublishLearningPath()
    }
  ];
  private currentUser = UserInfoModel.getMyUserInfo();
  private _learningPathDetailVM: LearningPathDetailViewModel = new LearningPathDetailViewModel();
  private _detailPageInput: RouterPageInput<LearningPathDetailPageInput, LMMTabConfiguration, unknown> = NAVIGATORS[
    LMMRoutePaths.LearningPathDetailPage
  ] as RouterPageInput<LearningPathDetailPageInput, LMMTabConfiguration, unknown>;
  private _loadLearningPathInfoSub: Subscription = new Subscription();
  // Auto save after 30 minutes
  private scheduler: IntervalScheduler = new IntervalScheduler(600000, () => {
    if (this.dataHasChanged()) {
      this.onSaveLearningPath();
    }
  });
  public get selectedTab(): LMMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : LMMTabConfiguration.LearningPathInfoTab;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public navigationPageService: NavigationPageService,
    private learningPathRepository: LearningPathRepository,
    private courseRepository: CourseRepository,
    private opalDialogService: OpalDialogService,
    private breadcrumbService: BreadcrumbService,
    private taggingRepository: TaggingRepository,
    private myBookmarkApiService: MyBookmarkApiService
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public dataHasChanged(): boolean {
    return (
      this.learningPathDetailVM &&
      this.learningPathDetailVM.dataHasChanged() &&
      (this.detailPageInput.data.mode === LearningPathDetailMode.Edit ||
        this.detailPageInput.data.mode === LearningPathDetailMode.NewLearningPath)
    );
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = learningPathDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public loadLearningPathInfo(): void {
    this._loadLearningPathInfoSub.unsubscribe();
    const learningPathObs: Observable<LearningPathModel | null> =
      this.detailPageInput.data.id != null
        ? this.learningPathRepository.loadLearningPath(this.detailPageInput.data.id)
        : of(new LearningPathModel());
    const bookmarkCountObs: Observable<GetCountUserBookmarkedResult | null> =
      this.detailPageInput.data.id != null
        ? from(this.myBookmarkApiService.getCountUserBookmarked(BookmarkType.LearningPathLMM, [this.detailPageInput.data.id]))
        : of(new GetCountUserBookmarkedResult());
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    this.loadingLearningPathCourses = true;
    this._loadLearningPathInfoSub = combineLatest(learningPathObs, taggingObs, bookmarkCountObs)
      .pipe(
        switchMap(([learningPath, metadatas, myUserBookmarkedResult]) =>
          this.courseRepository.loadCourses(learningPath.listCourses.map(a => a.courseId)).pipe(
            map(data => {
              return new LearningPathDetailViewModel(
                learningPath,
                data,
                myUserBookmarkedResult.item.length > 0 ? myUserBookmarkedResult.item[0].countTotal : 0,
                metadatas
              );
            })
          )
        )
      )
      .pipe(this.untilDestroy())
      .subscribe(
        result => {
          this.learningPathDetailVM = result;
          this.loadBreadcrumb();
          this.loadingLearningPathCourses = false;
        },
        error => {
          this.loadingLearningPathCourses = false;
        }
      );
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(() => this.dataHasChanged(), () => this.validateAndSaveLearningPath());
  }

  public canSaveLearningPath(): boolean {
    return (
      (this.detailPageInput.data.mode === LearningPathDetailMode.Edit ||
        this.detailPageInput.data.mode === LearningPathDetailMode.NewLearningPath) &&
      this.selectedTab === LMMTabConfiguration.LearningPathInfoTab
    );
  }

  public canEditLearningPath(): boolean {
    return (
      !this.learningPathDetailVM.learningPath.isPublished() &&
      this.learningPathDetailVM.learningPath.hasEditPublishUnpublishPermission(this.currentUser) &&
      this.detailPageInput.data.mode === LearningPathDetailMode.View &&
      this.selectedTab === LMMTabConfiguration.LearningPathInfoTab
    );
  }

  public canPublishLearningPath(): boolean {
    return (
      !this.learningPathDetailVM.learningPath.isPublished() &&
      this.learningPathDetailVM.learningPath.hasEditPublishUnpublishPermission(this.currentUser) &&
      this.detailPageInput.data.mode === LearningPathDetailMode.View &&
      this.selectedTab === LMMTabConfiguration.LearningPathInfoTab
    );
  }

  public canUnpublishLearningPath(): boolean {
    return (
      this.learningPathDetailVM.learningPath.isPublished() &&
      this.learningPathDetailVM.learningPath.hasEditPublishUnpublishPermission(this.currentUser) &&
      this.detailPageInput.data.mode === LearningPathDetailMode.View &&
      this.selectedTab === LMMTabConfiguration.LearningPathInfoTab
    );
  }

  public onEditLearningPath(): void {
    this.detailPageInput.data.mode = LearningPathDetailMode.Edit;
  }

  public onSaveLearningPath(): void {
    this.validateAndSaveLearningPath().subscribe(() => this.navigationPageService.navigateBack());
  }

  public onPublishLearningPath(): void {
    this.saveLearningPath(LearningPathStatus.Published).then(() => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  public onUnpublishLearningPath(): void {
    this.saveLearningPath(LearningPathStatus.Unpublished).then(() => {
      this.showNotification();
      this.navigationPageService.navigateBack();
    });
  }

  public onClickPopulate(): void {
    this.learningPathDetailVM.populateMetadata();
    this.showNotification();
  }

  public openCoursePreviewDialogFn(courseId: string): DialogRef {
    return this.opalDialogService.openDialogRef(
      CourseDetailDialogComponent,
      {
        courseId: courseId
      },
      {
        padding: '0px',
        maxWidth: '100vw',
        maxHeight: '100vh',
        width: '100vw',
        height: '100vh'
      }
    );
  }

  public showPopulateMetadataLPButton(): boolean {
    return (
      this.learningPathDetailVM.learningPath.hasPopulateMetadataLearningPathPermission(this.currentUser) &&
      (this.detailPageInput.data.mode === LearningPathDetailMode.Edit ||
        this.detailPageInput.data.mode === LearningPathDetailMode.NewLearningPath)
    );
  }

  public validateAndSaveLearningPath(): Observable<void> {
    return from(
      new Promise<void>((resolve, reject) => {
        this.validate().then(valid => {
          if (valid) {
            if (this.learningPathDetailVM.listCourses == null || this.learningPathDetailVM.listCourses.length === 0) {
              this.opalDialogService.openConfirmDialog({
                confirmTitle: 'Error',
                confirmMsg: 'You cannot save this learning path. Please select PD Opportunities',
                hideNoBtn: true,
                yesBtnText: 'Ok'
              });
              reject();
            } else {
              this.saveLearningPath(this.learningPathDetailVM.learningPath.status).then(() => {
                this.showNotification();
                resolve();
              }, reject);
            }
          } else {
            reject();
          }
        });
      })
    );
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      // Overview Info
      formName: 'basic-info',
      controls: {
        title: {
          defaultValue: 'Draft',
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            }
          ]
        },
        thumbnailUrl: {
          defaultValue: null,
          validators: null
        },
        // Metadata
        serviceSchemeIds: {
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
        courseLevelIds: {
          defaultValue: null,
          validators: null
        },
        pdAreaThemeIds: {
          defaultValue: null,
          validators: null
        },
        metadataKeys: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadLearningPathInfo();
    this.scheduler.init();
  }

  protected onDestroy(): void {
    this.scheduler.destroy();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<LearningPathDetailPageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private saveLearningPath(status: LearningPathStatus): Promise<LearningPathModel> {
    return new Promise((resolve, reject) => {
      const request: ISaveLearningPathRequest = {
        data: Utils.clone(this.learningPathDetailVM.learningPath, cloneData => {
          cloneData.status = status;
        })
      };
      this.learningPathRepository
        .saveLearningPath(request)
        .pipe(
          map(learningPath => {
            this.learningPathDetailVM.id = learningPath.id;
            return learningPath;
          }),
          switchMap(learningPath => {
            return this.taggingRepository
              .saveLearningPathMetadata(learningPath.id, {
                tagIds: learningPath.getAllMetadataTagIds(),
                searchTags: this.learningPathDetailVM.metadataKeys
              })
              .pipe(map(_ => learningPath));
          }),
          this.untilDestroy()
        )
        .subscribe(course => {
          resolve(course);
        }, reject);
    });
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      LMM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p => this.navigationPageService.navigateByRouter(p, () => this.dataHasChanged(), () => this.validateAndSaveLearningPath()),
        {
          [LMMRoutePaths.LearningPathDetailPage]: { textFn: () => this.learningPathDetailVM.title }
        }
      )
    );
  }
}

export const learningPathDetailPageTabIndexMap = {
  0: LMMTabConfiguration.LearningPathInfoTab
};
