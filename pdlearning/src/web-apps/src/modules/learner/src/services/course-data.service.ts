import * as lodash from 'lodash-es';

import {
  BookmarkType,
  ClassRun,
  ClassRunRepository,
  Course,
  CourseRepository,
  ICatalogSearchResult,
  IMyCoursesSearchRequest,
  IMyCoursesSummaryRequest,
  IMyCoursesSummaryResult,
  IMyDigitalContentSearchRequest,
  IMyLearningSearchRequest,
  IRecommendationByOrganisationItemResult,
  IdpRepository,
  LearningContentRepository,
  LectureIdMapNameModel,
  MetadataTagModel,
  MyClassRunModel,
  MyCourseApiService,
  MyCourseRepository,
  MyCourseResultModel,
  MyRegistrationStatus,
  SessionRepository,
  TaggingApiService,
  UpcomingSession
} from '@opal20/domain-api';
import { CourseModel, CourseViewPagedResult, CourseViewSearchFilterResultModel } from '../models/course.model';
import { ILearningItemModel, LearningItemModel, LearningItemResult } from '../models/learning-item.model';
import { Observable, combineLatest, from, of } from 'rxjs';
import { map, shareReplay, switchMap } from 'rxjs/operators';

import { DigitalContentDataService } from './digital-content.service';
import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { Injectable } from '@angular/core';
import { PagedCourseDetailsModelResult } from '../dtos/course-backend-service.dto';

function calculatePageNum(skipCount: number, maxResultCount: number): number {
  return Math.max(1, Math.ceil((skipCount + 1) / maxResultCount));
}

export type LoadFullCourseOption = {
  loadCourseDetail?: boolean;
  loadClassRunDetail?: boolean;
  loadCurrentLectureName?: boolean;
  loadUpcomingSession?: boolean;
};

@Injectable()
export class CourseDataService {
  private metaTags$: Observable<MetadataTagModel[]>;
  constructor(
    private myCourseApiService: MyCourseApiService,
    private myCourseRepository: MyCourseRepository,
    private courseRepository: CourseRepository,
    private learningContentRepository: LearningContentRepository,
    private digitalContentDataService: DigitalContentDataService,
    private taggingBackendService: TaggingApiService,
    private idpRepository: IdpRepository,
    private classRunRepository: ClassRunRepository,
    private sessionRepository: SessionRepository
  ) {}

  public getMyCoursesSummary(request: IMyCoursesSummaryRequest): Promise<IMyCoursesSummaryResult[]> {
    return this.myCourseApiService.getMyCoursesSummary(request).then((courseResult: IMyCoursesSummaryResult[]) => {
      return courseResult;
    });
  }

  public getMyCoursesBySearch(request: IMyLearningSearchRequest): Observable<CourseViewSearchFilterResultModel> {
    return from(this.myCourseApiService.getMyCoursesBySearch(request)).pipe(
      switchMap(response => {
        return this.loadFullCoursesModel(response.items, {
          loadCourseDetail: true,
          loadClassRunDetail: true,
          loadCurrentLectureName: true,
          loadUpcomingSession: true
        }).pipe(
          map(fullCoursesModel => {
            return new CourseViewSearchFilterResultModel({
              totalCount: response.totalCount,
              statistics: response.statistics,
              items: fullCoursesModel
            });
          })
        );
      })
    );
  }

  public getMyCourses(request: IMyCoursesSearchRequest): Observable<CourseViewPagedResult> {
    return this.myCourseRepository.loadMyCourses(request).pipe(
      switchMap(response => {
        return this.loadFullCoursesModel(response.items, {
          loadCourseDetail: true,
          loadClassRunDetail: true,
          loadCurrentLectureName: true,
          loadUpcomingSession: true
        }).pipe(
          map(fullCoursesModel => {
            return new CourseViewPagedResult({
              totalCount: response.totalCount,
              items: fullCoursesModel
            });
          })
        );
      })
    );
  }

  public getMyLearnings(
    myCourseRequest: IMyCoursesSearchRequest,
    microLearningRequest: IMyCoursesSearchRequest,
    contentRequest: IMyDigitalContentSearchRequest
  ): Observable<LearningItemResult> {
    return combineLatest(
      this.getMyCourses(myCourseRequest),
      this.getMyCourses(microLearningRequest),
      this.digitalContentDataService.getMyDigitalContents(contentRequest)
    ).pipe(
      map(([myCourses, microLearning, contents]) => {
        const coursesItems: ILearningItemModel[] = myCourses.items.map(myCourse => new LearningItemModel(myCourse));
        const microLearningItems = microLearning.items.map(myCourse => new LearningItemModel(myCourse));
        const digitalContentItems = contents.items.map(myDigital => DigitalContentItemModel.createDigitalContentItemModel(myDigital));

        const learnings = coursesItems.concat(microLearningItems, digitalContentItems);
        const learningsCount = myCourses.totalCount + contents.totalCount + microLearning.totalCount;
        return new LearningItemResult(lodash.orderBy(learnings, 'createdDate', 'desc'), learningsCount);
      })
    );
  }

  public getMyBookmarks(showSpinner: boolean = true, maxResultCount: number = 10, skipCount: number = 0): Observable<LearningItemResult> {
    const filterCourseBookmarkTypes: BookmarkType[] = [BookmarkType.Course, BookmarkType.Microlearning];
    return combineLatest(
      this.getBookmarkCourses(showSpinner, maxResultCount, skipCount, filterCourseBookmarkTypes),
      this.digitalContentDataService.getMyBookmarkedDigitalContents(maxResultCount, skipCount)
    ).pipe(
      map(([courseResult, contentResult]) => {
        const contentItems = contentResult.items.map(p => DigitalContentItemModel.createDigitalContentItemModel(p));
        const courseItems: ILearningItemModel[] = courseResult.items.map(c => new LearningItemModel(c));
        const learnings = courseItems.concat(contentItems);
        const learningsCount = courseResult.totalCount + contentResult.totalCount;
        return new LearningItemResult(lodash.orderBy(learnings, 'createdDate', 'desc'), learningsCount);
      })
    );
  }

  public sortMergeItems(originalSourceIds: string[], mergedItems: ILearningItemModel[]): ILearningItemModel[] {
    return originalSourceIds.map(v => mergedItems.find(i => i.id === v)).filter(v => v !== undefined);
  }

  public getBookmarkCourses(
    showSpinner: boolean = true,
    maxResultCount: number = 10,
    skipCount: number = 0,
    filterItemType: BookmarkType[] = undefined
  ): Observable<CourseViewPagedResult> {
    return from(this.myCourseApiService.getCourseBookmarks(maxResultCount, skipCount, showSpinner, filterItemType)).pipe(
      switchMap(response => {
        return this.loadFullCoursesModel(response.items, {
          loadCourseDetail: true,
          loadCurrentLectureName: true,
          loadUpcomingSession: true
        }).pipe(
          map(fullCoursesModel => {
            return new CourseViewPagedResult({
              totalCount: response.totalCount,
              items: fullCoursesModel.filter(p => p.courseDetail != null)
            });
          })
        );
      })
    );
  }

  public getOrganisationSuggestedCourses(
    maxResultCount: number = 10,
    skipCount: number = 0,
    showSpinner: boolean = true
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    const pageIndex = calculatePageNum(skipCount, maxResultCount);
    return this.getCourseModelFromRecommendedResult(this.idpRepository.loadListRecommendationsByOU(pageIndex, maxResultCount));
  }

  public getCourseDetails(courseId: string): Observable<CourseModel> {
    const myCourse$ = from(this.myCourseApiService.getCourseDetails(courseId));
    const courseDetail$ = this.courseRepository.loadCourse(courseId);
    return combineLatest([myCourse$, courseDetail$]).pipe(
      map(([courseResult, courseDetailResult]) => {
        return new CourseModel(courseResult, courseDetailResult);
      })
    );
  }

  public getCourseLearningItem(courseIds: string[]): Observable<ILearningItemModel[]> {
    if (courseIds.length > 0) {
      return this.myCourseRepository.loadMyCoursesByCourseIds(courseIds).pipe(
        switchMap(myCourses =>
          this.loadFullCoursesModel(myCourses, { loadCourseDetail: true, loadUpcomingSession: true }).pipe(
            map(fullCourses => {
              const result = fullCourses.map(p => new LearningItemModel(p));
              result.sort((a, b) => courseIds.indexOf(a.id) - courseIds.indexOf(b.id));
              return result;
            })
          )
        )
      );
    }
    return of([]);
  }

  public getCourseMetadata(forceLoad: boolean = false): Observable<MetadataTagModel[]> {
    if (this.metaTags$ === undefined || forceLoad) {
      this.metaTags$ = this.taggingBackendService.getAllMetaDataTags().pipe(shareReplay({ bufferSize: 1, refCount: true }));
    }

    return this.metaTags$;
  }

  public getCourseModelResultFromCatalogResult(
    result: Observable<ICatalogSearchResult>
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return result.pipe(
      switchMap(csr => {
        const contentIds = csr.resources.filter(r => r.resourcetype === 'content').map(r => r.id);
        const courseIds = csr.resources.filter(r => r.resourcetype === 'course' || r.resourcetype === 'microlearning').map(r => r.id);
        const courseLearningItem$ = courseIds.length > 0 ? this.getCourseLearningItem(courseIds) : of([]);
        const contentLearningItem$ = contentIds.length > 0 ? this.digitalContentDataService.getContentLearningItems(contentIds) : of([]);

        return combineLatest(courseLearningItem$, contentLearningItem$).pipe(
          map(([e1, e2]) => ({
            total: csr.total,
            items: this.sortMergeItems(csr.resources.map(r => r.id), [...e1, ...e2])
          }))
        );
      })
    );
  }

  private getCourseModelFromRecommendedResult(
    result: Observable<IRecommendationByOrganisationItemResult>
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return result.pipe(
      switchMap(recommendation => {
        const courseIds = recommendation
          ? recommendation.items.filter(item => item && item.additionalProperties).map(item => item.additionalProperties.courseId)
          : [];
        const courseLearningItem = this.getCourseLearningItem(courseIds);
        return combineLatest(courseLearningItem).pipe(
          map(([learningItems]) => ({
            total: recommendation ? recommendation.totalItems : 0,
            items: [...learningItems]
          }))
        );
      })
    );
  }

  private getMyCourseFromCourseDetails(courseDetailsResult: PagedCourseDetailsModelResult): Observable<CourseViewPagedResult> {
    const courseIds: string[] = courseDetailsResult.items.map(p => p.id);
    return this.myCourseRepository.loadMyCoursesByCourseIds(courseIds).pipe(
      map(listMyCourse => {
        const fullDetailCourses = courseDetailsResult.items.map(item => {
          const myCourse = listMyCourse.find(x => x.courseId === item.id);
          return new CourseModel(myCourse, item);
        });

        const result = new CourseViewPagedResult({
          items: fullDetailCourses,
          totalCount: courseDetailsResult.totalCount
        });
        return result;
      })
    );
  }

  private loadFullCoursesModel(myCourses: MyCourseResultModel[], options: LoadFullCourseOption): Observable<CourseModel[]> {
    const courseIds: string[] = options.loadCourseDetail ? myCourses.map(p => p.courseId) : [];
    const classRunIds: string[] = options.loadClassRunDetail
      ? myCourses
          .reduce((myClassRuns: MyClassRunModel[], p) => (myClassRuns = myClassRuns.concat(p.myClassRuns)), [])
          .filter(p => p != null)
          .reduce((ids, p) => ids.concat(p.classRunId), [])
          .filter(p => p != null)
      : [];
    const lectureIds = options.loadCurrentLectureName
      ? myCourses.map(p => p.myCourseInfo && p.myCourseInfo.currentLecture).filter(p => p != null)
      : [];
    const upcomingSessionClassRunIds = options.loadUpcomingSession
      ? myCourses
          .map(p => p.myClassRun)
          .filter(p => p != null && (p.status === MyRegistrationStatus.OfferConfirmed || p.status === MyRegistrationStatus.ConfirmedByCA))
          .map(p => p.classRunId)
      : [];

    const courseDetail$: Observable<Course[]> = courseIds.length ? this.courseRepository.loadCourses(courseIds) : of([]);
    const classRunDetail$: Observable<ClassRun[]> = classRunIds.length ? this.classRunRepository.loadClassRunsByIds(classRunIds) : of([]);
    const lectures$: Observable<LectureIdMapNameModel[]> = lectureIds.length
      ? this.learningContentRepository.loadAllLectureNamesByListIds(lectureIds)
      : of([]);
    const upcomingSessions$: Observable<UpcomingSession[]> = upcomingSessionClassRunIds.length
      ? this.sessionRepository.loadUpcomingSessionsByClassRunIds(upcomingSessionClassRunIds)
      : of([]);

    return combineLatest(courseDetail$, classRunDetail$, lectures$, upcomingSessions$).pipe(
      map(([courseDetailsResult, classRunResult, lectureResult, upcomingSessions]) => {
        return myCourses.map(myCourse => {
          const courseDetail =
            courseDetailsResult && courseDetailsResult.length > 0 ? courseDetailsResult.find(_ => _.id === myCourse.courseId) : undefined;
          const itemClassRunIds = myCourse.myClassRuns ? myCourse.myClassRuns.map(_ => _.classRunId) : [];
          const classRunsDetail = classRunResult.filter(classRun => itemClassRunIds.includes(classRun.id));
          const lecture = lectureResult.find(_ => myCourse.myCourseInfo && myCourse.myCourseInfo.currentLecture === _.lectureId);
          const upcomingSession = upcomingSessions.find(_ => myCourse.myClassRun && myCourse.myClassRun.classRunId === _.classRunId);

          const courseModel = new CourseModel(
            myCourse,
            courseDetail,
            classRunsDetail,
            lecture && lecture.name,
            upcomingSession && upcomingSession.startDateTime
          );

          return courseModel;
        });
      })
    );
  }
}
