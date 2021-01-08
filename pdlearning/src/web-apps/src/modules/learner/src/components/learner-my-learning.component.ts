import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Course, CourseRepository, LearnerLearningPath, LearningPathModel, UserInfoModel } from '@opal20/domain-api';
import {
  LEARNER_PERMISSIONS,
  LearnerRoutePaths,
  LearningPathDetailMode,
  LearningPathDetailViewModel,
  MyLearningTab
} from '@opal20/domain-components';
import { MY_LEARNING_TAB_NAMES, SEARCH_PLACEHOLDER_TAB } from '../constants/my-learning-tab.enum';

import { ILearningItemModel } from '../models/learning-item.model';
import { LearningBookmarksComponent } from './learning-bookmarks.component';
import { LearningCoursesComponent } from './learning-courses.component';
import { LearningDigitalContentComponent } from './learning-digital-content.component';
import { LearningMicrolearningComponent } from './learning-microlearning.component';
import { LearningPathListComponent } from './learning-path-list.component';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

interface IFormData {
  learningPathItem: LearnerLearningPath;
  mode: LearningPathDetailMode;
}
@Component({
  selector: 'learner-my-learning',
  templateUrl: './learner-my-learning.component.html'
})
export class LearnerMyLearningComponent extends BasePageComponent {
  @Input()
  public set defaultTab(v: MyLearningTab) {
    if (v != null && v !== this.activeTab) {
      this.activeTab = v;
    }
  }

  @Output()
  public learningCardClick: EventEmitter<ILearningItemModel> = new EventEmitter<ILearningItemModel>();

  public myLearningTab: typeof MyLearningTab = MyLearningTab;
  public myLearningTabs: MyLearningTab[] = [];
  public myLearningTabNames: Map<MyLearningTab, string> = MY_LEARNING_TAB_NAMES;
  public myLearningPlaceholder: Map<MyLearningTab, string> = SEARCH_PLACEHOLDER_TAB;
  public myLearningPath: LearningPathDetailViewModel;
  public mode: LearningPathDetailMode;
  public isShowLearningPathActionForm: boolean = false;
  public currentSearchText: string;
  public searchText: string;
  public isSearchingWithText: boolean = false;

  public isShowSearchBar: boolean = true;

  public set activeTab(tab: MyLearningTab) {
    if (tab === this._activeTab) {
      return;
    }
    if (!this.myLearningTabs.length) {
      this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}`);
      return;
    }

    this._activeTab = this.hasPermissionToAccessTab(tab) ? tab : this.myLearningTabs[0];

    this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${this.activeTab}`);
  }

  public get activeTab(): MyLearningTab {
    return this._activeTab;
  }

  @ViewChild('myCourses', { static: false })
  private myCoursesListComponent: LearningCoursesComponent;
  @ViewChild('learningBookmarks', { static: false })
  private learningBookmarksComponent: LearningBookmarksComponent;
  @ViewChild('myMicrolearnings', { static: false })
  private myMicrolearningsComponent: LearningMicrolearningComponent;
  @ViewChild('myDigitalContents', { static: false })
  private myDigitalContentComponent: LearningDigitalContentComponent;
  @ViewChild('learningPathList', { static: false })
  private learningPathListComponent: LearningPathListComponent;

  private _activeTab: MyLearningTab;
  constructor(protected moduleFacadeService: ModuleFacadeService, private courseRepository: CourseRepository) {
    super(moduleFacadeService);
    this.loadMyLearningTabs();
  }

  public reloadData(): void {
    if (this.activeComponent) {
      this.activeComponent.triggerDataChange();
    }
    this.updateDeeplink(`learner/${LearnerRoutePaths.MyLearning}/${this.activeTab}`);
  }

  public onLearningCardClick(event: ILearningItemModel): void {
    this.learningCardClick.emit(event);
  }

  public onTabClicked(tab: MyLearningTab): void {
    this.activeTab = tab;
    this.onClearSearch();
  }

  public onLearningPathFormAction(data: IFormData): void {
    if (data.learningPathItem == null) {
      this.myLearningPath = new LearningPathDetailViewModel();
      this.showLearningPathPage(data.mode);
      return;
    }
    this.getLearningPathModel(data.learningPathItem)
      .pipe(this.untilDestroy())
      .subscribe(() => {
        this.showLearningPathPage(data.mode);
      });
  }

  public onBackClick(): void {
    this.isShowLearningPathActionForm = false;
  }

  public onHandleActionForm(): void {
    this.isShowLearningPathActionForm = false;
    this.reloadData();
  }

  public onSubmitSearch(): void {
    if (!this.searchText) {
      this.onClearSearch();
      return;
    }
    this.currentSearchText = this.searchText;
    this.isSearchingWithText = true;
  }

  public onClearSearch(): void {
    this.currentSearchText = undefined;
    this.searchText = undefined;
    this.isSearchingWithText = false;
  }

  public onLearningPathDetailShown(isShow: boolean): void {
    this.isShowSearchBar = !isShow;
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }

  private get activeComponent(): MyLearningTabComponent {
    switch (this.activeTab) {
      case MyLearningTab.Courses:
        return this.myCoursesListComponent;
      case MyLearningTab.Microlearning:
        return this.myMicrolearningsComponent;
      case MyLearningTab.DigitalContent:
        return this.myDigitalContentComponent;
      case MyLearningTab.Bookmarks:
        return this.learningBookmarksComponent;
      case MyLearningTab.LearningPaths:
        return this.learningPathListComponent;
    }
  }

  private showLearningPathPage(mode: LearningPathDetailMode): void {
    this.mode = mode;
    this.isShowLearningPathActionForm = true;
  }

  private loadMyLearningTabs(): void {
    this.myLearningTabs = Object.values(MyLearningTab).filter(tab => this.hasPermissionToAccessTab(tab));
  }

  private hasPermissionToAccessTab(tab: MyLearningTab): boolean {
    return this.hasPermission(TAB_PERMISSION_MAP.get(tab));
  }

  private getLearningPathModel(data: LearnerLearningPath): Observable<Course[]> {
    const model = new LearningPathModel({
      id: data.id,
      title: data.title,
      thumbnailUrl: data.thumbnailUrl,
      createdBy: data.createdBy,
      createdDate: new Date(data.createdDate),
      listCourses: data.courses.map(c => {
        return {
          id: c.id,
          courseId: c.courseId,
          order: c.order
        };
      })
    });
    const courseIds = model.listCourses.map(c => c.courseId);
    return this.courseRepository.loadCourses(courseIds).pipe(
      tap(courses => {
        this.myLearningPath = new LearningPathDetailViewModel(model, courses);
      })
    );
  }
}

const TAB_PERMISSION_MAP: Map<MyLearningTab, string> = new Map([
  [MyLearningTab.Courses, LEARNER_PERMISSIONS.MyLearning_Course],
  [MyLearningTab.Microlearning, LEARNER_PERMISSIONS.MyLearning_Microlearning],
  [MyLearningTab.DigitalContent, LEARNER_PERMISSIONS.MyLearning_DigitalContent],
  [MyLearningTab.LearningPaths, LEARNER_PERMISSIONS.MyLearning_LearningPath],
  [MyLearningTab.Community, LEARNER_PERMISSIONS.MyLearning_Community],
  [MyLearningTab.Bookmarks, LEARNER_PERMISSIONS.MyLearning_Bookmark]
]);

type MyLearningTabComponent =
  | LearningCoursesComponent
  | LearningBookmarksComponent
  | LearningMicrolearningComponent
  | LearningDigitalContentComponent
  | LearningPathListComponent;
