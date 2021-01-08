import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import {
  IEnrollCourseRequest,
  IReEnrollCourseRequest,
  IUpdateCourseStatus,
  PagedCourseModelResult
} from '../dtos/my-course-backend-service.dto';
import { IMyCourseModel, MyCourseModel } from '../models/my-course.model';
import { IMyCourseResultModel, MyCourseResultModel } from '../models/my-course-result.model';
import { IMyLectureModel, MyLectureModel } from '../models/my-lecture.model';
import { ISearchFilterResultModel, MyCourseSearchFilterResultModel } from '../dtos/my-learning-search-result.dto';

import { BookmarkType } from '../models/bookmark-info.model';
import { IMyCoursesSearchRequest } from '../dtos/my-course-search-request.dto';
import { IMyCoursesSummaryRequest } from '../dtos/my-course-summary-request.dto';
import { IMyCoursesSummaryResult } from '../dtos/my-course-summary-result.dto';
import { IMyLearningSearchRequest } from '../dtos/my-learning-search-request.dto';
import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class MyCourseApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/me/courses';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getMyCoursesBySearch(request: IMyLearningSearchRequest): Promise<MyCourseSearchFilterResultModel> {
    const queryParams: IGetParams = {
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount,
      searchText: request.searchText,
      courseType: request.type,
      includeStatistic: request.includeStatistic,
      statisticsFilter: request.statisticsFilter,
      statusFilter: request.statusFilter
    };
    return this.post<IGetParams, ISearchFilterResultModel<IMyCourseResultModel>>('/search', queryParams)
      .pipe(map(_ => new MyCourseSearchFilterResultModel(_)))
      .toPromise();
  }

  public getMyCoursesSummary(request: IMyCoursesSummaryRequest): Promise<IMyCoursesSummaryResult[]> {
    const requestBody: object = {
      statusFilter: request.statusFilter
    };
    return this.post<object, IMyCoursesSummaryResult[]>('/summary', requestBody).toPromise();
  }

  public getMyCourses(request: IMyCoursesSearchRequest, showSpinner: boolean = true): Promise<PagedCourseModelResult> {
    const queryParams: IGetParams = {
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount,
      searchText: request.searchText,
      statusFilter: request.statusFilter,
      orderBy: request.orderBy,
      courseType: request.courseType
    };
    return this.get<IPagedResultDto<IMyCourseResultModel>>('', queryParams, showSpinner)
      .pipe(map(result => new PagedCourseModelResult(result)))
      .toPromise();
  }

  public getRecentInProgressCourses(request: IMyCoursesSearchRequest): Promise<PagedCourseModelResult> {
    const queryParams: IGetParams = {
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };
    return this.get<IPagedResultDto<IMyCourseResultModel>>(`/recentInProgress`, queryParams)
      .pipe(map(result => new PagedCourseModelResult(result)))
      .toPromise();
  }

  public getCompletedCourses(request: IMyCoursesSearchRequest): Promise<PagedCourseModelResult> {
    const queryParams: IGetParams = {
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };
    return this.get<IPagedResultDto<IMyCourseResultModel>>(`/recentCompleted`, queryParams)
      .pipe(map(result => new PagedCourseModelResult(result)))
      .toPromise();
  }

  public getCourseBookmarks(
    maxResultCount: number = 10,
    skipCount: number = 0,
    showSpinner: boolean = true,
    filterItemType: BookmarkType[] = undefined
  ): Promise<PagedCourseModelResult> {
    const queryParams: IGetParams = {
      maxResultCount: maxResultCount,
      skipCount: skipCount,
      bookmarkTypeFilter: filterItemType
    };
    return this.get<IPagedResultDto<IMyCourseResultModel>>(`/bookmarks`, queryParams, showSpinner)
      .pipe(map(result => new PagedCourseModelResult(result)))
      .toPromise();
  }

  public getLecturesInMyCourse(myCourseId: string): Promise<MyLectureModel[]> {
    return this.get<IMyLectureModel[]>(`/${myCourseId}/lectures`)
      .pipe(map(result => result.map(p => new MyLectureModel(p))))
      .toPromise();
  }

  public getByCourseIds(courseIds: string[], showSpinner: boolean = true): Promise<MyCourseResultModel[]> {
    const requestBody = {
      courseIds: courseIds
    };
    return this.post<{ courseIds: string[] }, IMyCourseResultModel[]>('/getByCourseIds', requestBody, showSpinner)
      .pipe(map(result => result.map(p => new MyCourseResultModel(p))))
      .toPromise();
  }

  public getCourseDetails(courseId: string): Promise<MyCourseResultModel> {
    return this.get<IMyCourseResultModel>(`/details/byCourseId/${courseId}`)
      .pipe(map(result => new MyCourseResultModel(result)))
      .toPromise();
  }

  public enrollCourse(courseId: string, lectureIds: string[]): Promise<MyCourseModel> {
    const requestBody: IEnrollCourseRequest = {
      courseId: courseId,
      lectureIds: lectureIds
    };
    return this.post<IEnrollCourseRequest, IMyCourseModel>('/enroll', requestBody)
      .pipe(map(result => new MyCourseModel(result)))
      .toPromise();
  }

  public withdrawFromCourse(myCourseId: string): Promise<void> {
    return this.delete<void>(`/${myCourseId}`).toPromise();
  }

  public deleteMyCourse(myCourseId: string): Promise<void> {
    return this.delete<void>(`/${myCourseId}`).toPromise();
  }

  public completeLecture(myLectureId: string): Promise<void> {
    return this.put<undefined, void>(`/lectures/complete/${myLectureId}`, undefined).toPromise();
  }

  public updateStatusCourse(request: IUpdateCourseStatus): Promise<void> {
    return this.put<IUpdateCourseStatus, void>(`/updateStatus`, request).toPromise();
  }

  public reEnrollCourse(request: IReEnrollCourseRequest): Promise<MyCourseModel> {
    return this.post<IReEnrollCourseRequest, IMyCourseModel>('/reEnroll', request)
      .pipe(map(result => new MyCourseModel(result)))
      .toPromise();
  }
}
