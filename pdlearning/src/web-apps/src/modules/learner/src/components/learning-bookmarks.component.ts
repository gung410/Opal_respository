import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { BookmarkType, IGetMyBookmarkRequest, LearningPathModel, MyBookmarkRepository } from '@opal20/domain-api';
import { CommunityItemModel, ICommunityItemModel } from '../models/community-item.model';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { ILearningItemModel, LearningItemModel, LearningType } from '../models/learning-item.model';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { CourseDataService } from '../services/course-data.service';
import { CslDataService } from '../services/csl-data.service';
import { DigitalContentDataService } from '../services/digital-content.service';
import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { LearnerLearningPathDataService } from '../services/learningpath-data.service';
import { LearnerLearningPathModel } from '../models/learning-path.model';
import { LearningCardListComponent } from './learning-card-list.component';

@Component({
  selector: 'learning-bookmarks',
  templateUrl: './learning-bookmarks.component.html'
})
export class LearningBookmarksComponent extends BaseComponent {
  @Output()
  public learningCardClick: EventEmitter<ILearningItemModel> = new EventEmitter<ILearningItemModel>();

  public isShowBookmarkDetail: boolean = false;
  public isShowBookmarkDetailItems: boolean = false;
  public bookmarkDetailTitle: string;
  public totalCoursesBookmark: number = 0;
  public totalCoursesMicrolearningBookmark: number = 0;
  public totalDigitalContentsBookmark: number = 0;
  public totalLearningPathsBookmark: number = 0;
  public totalCommunitiesBookmark: number = 0;

  public currentBookmarkType: LearningType = LearningType.Course;
  public learningType: typeof LearningType = LearningType;
  public currentLearningPath;

  public getPagedBookmarkItemsCallback: (
    maxResultCount: number,
    skipCount: number
  ) => Observable<{ total: number; items: ILearningItemModel[] }>;

  @ViewChild('bookmarkList', { static: false })
  private bookmarkListComponent: LearningCardListComponent;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private courseDataService: CourseDataService,
    private digitalContentDataService: DigitalContentDataService,
    private cslService: CslDataService,
    private learnerLearningPathDataService: LearnerLearningPathDataService,
    private myBookmarkRepository: MyBookmarkRepository
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.getPagedBookmarkItemsCallback = this.getPagedBookmarkedCoursesCallback;
    this.initBookmarAmount();
  }

  public triggerDataChange(fromPage1: boolean = false): void {
    this.bookmarkListComponent.triggerDataChange(fromPage1);
  }

  public onViewCoursesBookMark(): void {
    this.getPagedBookmarkItemsCallback = this.getPagedBookmarkedCoursesCallback;
    this.currentBookmarkType = LearningType.Course;
    this.isShowBookmarkDetail = true;
    this.isShowBookmarkDetailItems = false;
    this.bookmarkDetailTitle = 'Courses';
  }

  public onViewCoursesMicrolearningBookMark(): void {
    this.getPagedBookmarkItemsCallback = this.getPagedBookmarkedCoursesMicrolearningCallback;
    this.currentBookmarkType = LearningType.Microlearning;
    this.isShowBookmarkDetail = true;
    this.isShowBookmarkDetailItems = false;
    this.bookmarkDetailTitle = 'Microlearning';
  }

  public onViewDigitalContentsBookMark(): void {
    this.getPagedBookmarkItemsCallback = this.getPagedBookmarkedDigitalContentCallback;
    this.currentBookmarkType = LearningType.DigitalContent;
    this.isShowBookmarkDetail = true;
    this.isShowBookmarkDetailItems = false;
    this.bookmarkDetailTitle = 'Digital Content';
  }

  public onViewLearningPathsBookMark(): void {
    this.getPagedBookmarkItemsCallback = this.getPagedBookmarkedLearningPathCallback;
    this.currentBookmarkType = LearningType.LearningPath;
    this.isShowBookmarkDetail = true;
    this.isShowBookmarkDetailItems = false;
    this.bookmarkDetailTitle = 'Learning Paths';
  }

  public onViewCommunitiesBookMark(): void {
    this.getPagedBookmarkItemsCallback = this.getPagedBookmarkedCommunityCallback;
    this.currentBookmarkType = LearningType.Community;
    this.isShowBookmarkDetail = true;
    this.isShowBookmarkDetailItems = false;
    this.bookmarkDetailTitle = 'Community';
  }

  public getPagedBookmarkedCoursesCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.courseDataService.getBookmarkCourses(true, maxResultCount, skipCount, [BookmarkType.Course]).pipe(
      map(result => {
        const items = result.items.map(p => new LearningItemModel(p));
        return {
          total: result.totalCount,
          items: items
        };
      })
    );
  }

  public getPagedBookmarkedCoursesMicrolearningCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.courseDataService.getBookmarkCourses(true, maxResultCount, skipCount, [BookmarkType.Microlearning]).pipe(
      map(result => {
        const items = result.items.map(p => new LearningItemModel(p));
        return {
          total: result.totalCount,
          items: items
        };
      })
    );
  }

  public getPagedBookmarkedDigitalContentCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.digitalContentDataService.getMyBookmarkedDigitalContents(maxResultCount, skipCount).pipe(
      map(result => {
        const items = result.items.map(p => DigitalContentItemModel.createDigitalContentItemModel(p));
        return {
          total: result.totalCount,
          items: items
        };
      })
    );
  }

  public getPagedBookmarkedLearningPathCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    const request: IGetMyBookmarkRequest = {
      itemType: BookmarkType.LearningPath,
      maxResultCount: maxResultCount,
      skipCount: skipCount
    };

    const response = this.myBookmarkRepository.loadBookmarkLearningPath(request).pipe(
      switchMap(result => {
        const learningPathLMMIds = result.items
          .filter(p => p.bookmarkInfo !== null && p.bookmarkInfo.itemType === BookmarkType.LearningPathLMM)
          .map(p => p.bookmarkInfo.itemId);
        if (learningPathLMMIds && learningPathLMMIds.length === 0) {
          const items = result.items.map(p => LearnerLearningPathModel.createByMyLearningPath(p));
          return of({
            total: result.totalCount,
            items: items
          });
        }
        return this.learnerLearningPathDataService.getLearningPathFromLMM(learningPathLMMIds).pipe(
          map((resp: LearningPathModel[]) => {
            const items = result.items.map(lp => {
              if (lp.bookmarkInfo.itemType === BookmarkType.LearningPathLMM) {
                const learningPathLMM = resp.filter(i => i.id === lp.bookmarkInfo.itemId)[0];
                if (learningPathLMM) {
                  return LearnerLearningPathModel.convertLearningPathFromLMM(learningPathLMM, lp.bookmarkInfo);
                }
              } else {
                return LearnerLearningPathModel.createByMyLearningPath(lp);
              }
            });
            return {
              total: result.totalCount,
              items: items
            };
          })
        );
      })
    );
    return response;
  }

  public getPagedBookmarkedCommunityCallback(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ICommunityItemModel[];
  }> {
    const request: IGetMyBookmarkRequest = {
      itemType: BookmarkType.Community,
      maxResultCount: maxResultCount,
      skipCount: skipCount
    };

    const response = this.cslService.getMyCommunityBookmarks(request).pipe(
      map(result => {
        if (result === null) {
          return {
            total: 0,
            items: []
          };
        }
        const items = result.items.map(p => CommunityItemModel.createCommunityItemModel(p));
        return {
          total: result.totalCount,
          items: items
        };
      })
    );
    return response;
  }

  public onBookmarkDetailBackClick(): void {
    this.initBookmarAmount();
    this.isShowBookmarkDetail = false;
  }

  public onBackBookmarkDetail(event: ILearningItemModel): void {
    this.isShowBookmarkDetail = false;
    this.initBookmarAmount();
  }

  public initBookmarAmount(): void {
    this.getPagedBookmarkedCoursesCallback(1, 0)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.totalCoursesBookmark = result.total;
      });

    this.getPagedBookmarkedCoursesMicrolearningCallback(1, 0)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.totalCoursesMicrolearningBookmark = result.total;
      });

    this.getPagedBookmarkedDigitalContentCallback(1, 0)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.totalDigitalContentsBookmark = result.total;
      });

    this.getPagedBookmarkedLearningPathCallback(1, 0)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.totalLearningPathsBookmark = result.total;
      });

    this.getPagedBookmarkedCommunityCallback(1, 0)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.totalCommunitiesBookmark = result.total;
      });
  }

  public onLearningCardClick(event: LearningItemModel): void {
    if (!event) {
      return;
    }

    if (event.type === LearningType.LearningPath) {
      this.isShowBookmarkDetail = false;
      this.isShowBookmarkDetailItems = true;
      this.currentLearningPath = event;
      return;
    }

    this.learningCardClick.emit(event);
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

  public onLearningPathItemsBackClick(event: LearnerLearningPathModel): void {
    this.currentLearningPath = undefined;
    this.isShowBookmarkDetail = true;
    this.isShowBookmarkDetailItems = false;
  }

  public onLearningPathDetailItemClick(event: LearnerLearningPathModel): void {
    this.learningCardClick.emit(event);
  }

  private getPagedItemsInLearningPath(learningPath: LearnerLearningPathModel, maxResultCount: number, skipCount: number): string[] {
    if (learningPath && learningPath.courses && learningPath.courses.length === 0) {
      return;
    }
    skipCount = skipCount === 0 ? skipCount + 1 : skipCount;
    const itemIdsPaged = learningPath.courses.slice((skipCount - 1) * maxResultCount, skipCount * maxResultCount);
    return itemIdsPaged.map(x => x.courseId);
  }
}
