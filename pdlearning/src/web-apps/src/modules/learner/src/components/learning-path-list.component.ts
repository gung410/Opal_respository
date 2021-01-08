import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import {
  BookmarkType,
  IGetMyLearningPathRequest,
  IGetUserSharingRequest,
  LearnerLearningPath,
  LearningPathApiService,
  MyLearningPathApiService,
  SharingType,
  UserSharingAPIService
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { ILearningItemModel, LearningType } from '../models/learning-item.model';
import { LEARNER_LEARNING_PATH_SECTION, LearningPathSectionEnum } from '../constants/learner-learning-path.enum';
import {
  LEARNER_PERMISSIONS,
  LearnerRoutePaths,
  LearningPathDetailMode,
  MyLearningTab,
  WebAppLinkBuilder
} from '@opal20/domain-components';
import { MyLearningSearchFilterResult, SearchFilterModel } from '../models/my-learning-search-filter-model';
import { Observable, from } from 'rxjs';

import { CatalogueDataService } from '../services/catalogue-data.service';
import { CourseDataService } from '../services/course-data.service';
import { LearnerLearningPathModel } from '../models/learning-path.model';
import { LearningActionService } from '../services/learning-action.service';
import { LearningCardListComponent } from './learning-card-list.component';
import { MyLearningSearchDataService } from '../services/my-learning-search.service';
import { SEARCH_FILTER_TYPE_INITIALIZE } from '../constants/my-learning-tab.enum';
import { map } from 'rxjs/operators';

interface IFormData {
  learningPathItem?: LearnerLearningPath;
  mode: LearningPathDetailMode;
}
@Component({
  selector: 'learning-path-list',
  templateUrl: './learning-path-list.component.html'
})
export class LearningPathListComponent extends BaseComponent {
  @ViewChild('learningPathsList', { static: false })
  public learningPathListComponent: LearningCardListComponent;

  @ViewChild('learningPathItemsList', { static: false })
  public learningPathItemsListComponent: LearningCardListComponent;

  @Input()
  public searchingText: string;

  @Input()
  public isSearchingWithText: boolean;

  @Output()
  public learningCardClick: EventEmitter<LearnerLearningPathModel> = new EventEmitter<LearnerLearningPathModel>();

  @Output()
  public learningPathFormAction: EventEmitter<IFormData> = new EventEmitter<IFormData>();

  @Output()
  public showLearningPathItem: EventEmitter<boolean> = new EventEmitter();

  public currentSection: LearningPathSectionEnum;

  public myLearningPathsItems: LearnerLearningPathModel[];
  public myLearningPathsItemsTotalCount: number = 0;

  public sharedLearningPathForMeItems: LearnerLearningPathModel[];
  public sharedLearningPathForMeItemsTotalCount: number = 0;

  public recommendLearningPathFromLMMItems: LearnerLearningPathModel[];
  public recommendLearningPathFromLMMItemsTotalCount: number = 0;

  public showLearningPathList: boolean = false;
  public learningPathListTitle: string;
  public numberOfItemsOnPage = 12;
  public learningPathSectionEnum = LearningPathSectionEnum;
  public learningPathOwnerSection: boolean = true;
  public showCopyPermalink: boolean = true;

  public filterByType: SearchFilterModel = SEARCH_FILTER_TYPE_INITIALIZE.get(MyLearningTab.LearningPaths);

  public getPagedLearningPathCallback: (
    maxResultCount: number,
    skipCount: number
  ) => Observable<{ total: number; items: LearnerLearningPathModel[] }>;

  public set currentLearningPath(value: LearnerLearningPathModel) {
    this._currentLearningPath = value;
    this.showLearningPathItem.emit(this._currentLearningPath != null);
  }

  public get currentLearningPath(): LearnerLearningPathModel {
    return this._currentLearningPath;
  }

  private _currentLearningPath: LearnerLearningPathModel;
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private courseDataService: CourseDataService,
    private userSharingService: UserSharingAPIService,
    private catalogDataService: CatalogueDataService,
    private myLearningPathApiService: MyLearningPathApiService,
    private lmmLearningPathApiService: LearningPathApiService,
    private myLearningSearchDataService: MyLearningSearchDataService,
    private learningActionService: LearningActionService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    const routingParameters: {
      courseId: string;
      activeTab: MyLearningTab;
      pathId: string;
      type: string;
    } = this.getNavigateData() || {
      courseId: undefined,
      activeTab: undefined,
      pathId: undefined,
      type: undefined
    };

    if (routingParameters.pathId) {
      if (routingParameters.type === 'fromlmm') {
        this.getPagedLearningPathCallback = this.getPagedRecommendLearningPathsCallback;
        this.learningPathListTitle = LEARNER_LEARNING_PATH_SECTION.get(LearningPathSectionEnum.RecommendForYou);
        this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${MyLearningTab.LearningPaths}/${routingParameters.pathId}/fromlmm`);
        this.lmmLearningPathApiService.getLearningPath(routingParameters.pathId).then(learningPath => {
          this.currentLearningPath = LearnerLearningPathModel.convertLearningPathFromLMM(learningPath);
          this.learningPathOwnerSection = false;
        });
      } else {
        this.getPagedLearningPathCallback = this.getPagedLearningPathsCallback;
        this.learningPathListTitle = LEARNER_LEARNING_PATH_SECTION.get(LearningPathSectionEnum.MyOwn);
        this.myLearningPathApiService.getLearningPathById(routingParameters.pathId).then(learningPath => {
          this.currentLearningPath = LearnerLearningPathModel.createByMyLearningPath(learningPath);
          this.learningPathOwnerSection = this.currentLearningPath.isOwner;
        });
      }
    } else {
      this.onInitData();
    }
  }

  public onDestroy(): void {
    this.showLearningPathItem.emit(false);
  }

  public onInitData(): void {
    this.getLearningPaths();
    this.getSharedLearningPathsToMe();
    this.getRecommendLearningPathsFromLMM();
    this.getPagedLearningPathCallback = this.getPagedLearningPathsCallback;
  }

  public getLearningPaths(): void {
    const request: IGetMyLearningPathRequest = {
      searchText: null,
      maxResultCount: this.numberOfItemsOnPage,
      skipCount: 0
    };

    this.myLearningPathApiService.getLearningPaths(request).then(result => {
      this.myLearningPathsItems = result.items.map(p => LearnerLearningPathModel.createByMyLearningPath(p));
      this.myLearningPathsItemsTotalCount = result.totalCount;
    });
  }

  public getSharedLearningPathsToMe(): void {
    const request: IGetUserSharingRequest = {
      searchText: null,
      maxResultCount: this.numberOfItemsOnPage,
      skipCount: 0
    };
    this.userSharingService.getSharedLearningPathToMe(request).then(result => {
      this.sharedLearningPathForMeItems = result.items.map(p => LearnerLearningPathModel.createByMyLearningPath(p));
      this.sharedLearningPathForMeItemsTotalCount = result.totalCount;
    });
  }

  public getRecommendLearningPathsFromLMM(): void {
    this.catalogDataService
      .getRecommendLearningPathsFromLMM(null, null, this.numberOfItemsOnPage, 0, true)
      .pipe(this.untilDestroy())
      .subscribe(response => {
        if (response) {
          this.recommendLearningPathFromLMMItems = response.items;
        }
        this.recommendLearningPathFromLMMItemsTotalCount = response.total;
      });
  }

  public getPagedLearningPathItemsCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    const pagedItemIds = this.getPagedItemsInLearningPath(this.currentLearningPath, maxResultCount, skipCount);
    return this.courseDataService.getCourseLearningItem(pagedItemIds).pipe(
      map(result => {
        return {
          total: this.currentLearningPath.courses.length,
          items: result
        };
      })
    );
  }

  public getPagedLearningPathsCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: LearnerLearningPathModel[];
  }> {
    const request: IGetMyLearningPathRequest = {
      searchText: null,
      maxResultCount: maxResultCount,
      skipCount: skipCount
    };
    return from(this.myLearningPathApiService.getLearningPaths(request)).pipe(
      map(result => {
        const items = result.items.map(p => LearnerLearningPathModel.createByMyLearningPath(p));
        this.myLearningPathsItemsTotalCount = result.totalCount;
        return {
          total: result.totalCount,
          items: items
        };
      })
    );
  }

  public getPagedSharedLearningPathsCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: LearnerLearningPathModel[];
  }> {
    const request: IGetUserSharingRequest = {
      searchText: null,
      itemType: SharingType.LearningPath,
      maxResultCount: maxResultCount,
      skipCount: skipCount
    };
    return from(this.userSharingService.getSharedLearningPathToMe(request)).pipe(
      map(result => {
        const items = result.items.map(p => LearnerLearningPathModel.createByMyLearningPath(p));
        this.sharedLearningPathForMeItemsTotalCount = result.totalCount;
        return {
          total: result.totalCount,
          items: items
        };
      })
    );
  }

  public getPagedRecommendLearningPathsCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: LearnerLearningPathModel[];
  }> {
    return this.catalogDataService.getRecommendLearningPathsFromLMM(null, null, maxResultCount, skipCount, true).pipe(
      map(response => {
        return {
          total: response.total,
          items: response.items
        };
      })
    );
  }

  public onShowMyLearningClick(currentLearningPathSection: LearningPathSectionEnum): void {
    this.showLearningPathList = true;
    this.currentSection = currentLearningPathSection;
    this.learningPathListTitle = LEARNER_LEARNING_PATH_SECTION.get(this.currentSection);
    switch (this.currentSection) {
      case LearningPathSectionEnum.MyOwn:
        this.learningPathOwnerSection = true;
        this.showCopyPermalink = true;
        this.getPagedLearningPathCallback = this.getPagedLearningPathsCallback;
        break;
      case LearningPathSectionEnum.SharedToMe:
        this.learningPathOwnerSection = false;
        this.showCopyPermalink = false;
        this.getPagedLearningPathCallback = this.getPagedSharedLearningPathsCallback;
        break;
      case LearningPathSectionEnum.RecommendForYou:
        this.learningPathOwnerSection = false;
        this.showCopyPermalink = true;
        this.getPagedLearningPathCallback = this.getPagedRecommendLearningPathsCallback;
        break;
      default:
        break;
    }
  }

  public triggerDataChange(fromPage1: boolean = false): void {
    this.getLearningPaths();
    if (this.learningPathListComponent) {
      this.learningPathListComponent.triggerDataChange(fromPage1);
    }
    if (this.learningPathItemsListComponent) {
      this.learningPathItemsListComponent.triggerDataChange(fromPage1);
    }
    this.currentLearningPath = undefined;
  }

  public onLearningPathDetailItemClick(event: LearnerLearningPathModel): void {
    this.learningCardClick.emit(event);
  }

  public onLearningPathClick(event: LearnerLearningPathModel, section: LearningPathSectionEnum = this.currentSection): void {
    this.learningPathOwnerSection = event.isOwner;
    this.currentLearningPath = event;
    this.currentSection = section;
    this.showLearningPathList = true;
    this.learningPathListTitle = LEARNER_LEARNING_PATH_SECTION.get(this.currentSection);
    switch (this.currentSection) {
      case LearningPathSectionEnum.MyOwn:
        this.learningPathOwnerSection = true;
        this.showCopyPermalink = true;
        this.getPagedLearningPathCallback = this.getPagedLearningPathsCallback;
        break;
      case LearningPathSectionEnum.SharedToMe:
        this.learningPathOwnerSection = false;
        this.showCopyPermalink = false;
        this.getPagedLearningPathCallback = this.getPagedSharedLearningPathsCallback;
        break;
      case LearningPathSectionEnum.RecommendForYou:
        this.learningPathOwnerSection = false;
        this.showCopyPermalink = true;
        this.getPagedLearningPathCallback = this.getPagedRecommendLearningPathsCallback;
        break;
      default:
        break;
    }
    if (this.currentSection === LearningPathSectionEnum.RecommendForYou) {
      this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${MyLearningTab.LearningPaths}/${event.id}/fromlmm`);
    } else {
      this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${MyLearningTab.LearningPaths}/${event.id}`);
    }
  }

  public onLearningPathBackClick(event: LearnerLearningPathModel): void {
    this.currentLearningPath = undefined;
    this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${MyLearningTab.LearningPaths}`);
    this.onInitData();
  }

  public onLearningPathsBackClick(event: LearnerLearningPathModel): void {
    // Force get data because bookmark not sync when hit back button
    this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${MyLearningTab.LearningPaths}`);
    this.onInitData();
    this.showLearningPathList = false;
  }

  public onLearningPathItemsBackClick(event?: LearnerLearningPathModel): void {
    this.currentLearningPath = undefined;
    this.showLearningPathList = true;
    this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${MyLearningTab.LearningPaths}`);
    // this.onInitData();
  }

  public onCreate(): void {
    this.learningPathFormAction.emit({ mode: LearningPathDetailMode.NewLearningPath });
  }

  public onEdit(): void {
    this.myLearningPathApiService.getLearningPathById(this.currentLearningPath.id).then(learningPath => {
      this.learningPathFormAction.emit({ learningPathItem: learningPath, mode: LearningPathDetailMode.Edit });
    });
  }

  public iconPermalinkClick(e: Event): void {
    if (!this.currentLearningPath.isPublic && this.currentLearningPath.isOwner) {
      this.myLearningPathApiService.publicLearningPath(this.currentLearningPath.id).then(() => {
        this.currentLearningPath.isPublic = true;
      });
    }
  }

  public getPagedSearchFilterCallBack(searchModel: SearchFilterModel): Observable<MyLearningSearchFilterResult<LearnerLearningPathModel>> {
    return this.myLearningSearchDataService.searchLearningPaths(searchModel);
  }

  public iconBookmarkClick(): void {
    const bookmarkType = this.currentLearningPath.fromLMM === true ? BookmarkType.LearningPathLMM : BookmarkType.LearningPath;
    this.currentLearningPath.isBookmark
      ? this.learningActionService.unBookmark(this.currentLearningPath.id, bookmarkType)
      : this.learningActionService.bookmark(this.currentLearningPath.id, bookmarkType);
    this.currentLearningPath.isBookmark = !this.currentLearningPath.isBookmark;
  }

  public get learningPathCUDPermissionKey(): string {
    return LEARNER_PERMISSIONS.MyLearning_LearningPath_CUD;
  }

  private getPagedItemsInLearningPath(learningPath: LearnerLearningPathModel, maxResultCount: number, skipCount: number): string[] {
    if (learningPath && learningPath.courses && learningPath.courses.length === 0) {
      return;
    }
    const itemIdsPaged = learningPath.courses.slice(skipCount, skipCount + maxResultCount);
    return itemIdsPaged.map(x => x.courseId);
  }

  public get detailUrl(): string {
    return WebAppLinkBuilder.buildLearningPathDetailUrl(
      this.currentLearningPath.id,
      this.currentLearningPath.type === LearningType.LearningPathLMM
    );
  }
}
