import { BaseBackendService, CommonFacadeService, IFilter } from '@opal20/infrastructure';
import { Course, ICourse } from '../models/course.model';

import { CheckCourseEndDateValidWithClassEndDateRequest } from '../dtos/check-course-end-date-valid-with-class-end-date-request';
import { CheckExistedCourseFieldRequest } from './../dtos/check-existed-course-field-request';
import { Constant } from '@opal20/authentication';
import { CourseStatus } from '../../share/models/course-status.enum';
import { IArchiveCourseRequest } from '../dtos/archive-course-request';
import { IChangeCourseStatusRequest } from '../dtos/change-course-status-request';
import { ICloneCourseRequest } from './../dtos/clone-course-request';
import { ISaveCourseRequest } from './../dtos/save-course-request';
import { ISearchCourseRequest } from './../dtos/search-course-request';
import { ISearchCourseUserRequest } from '../dtos/search-course-users-request';
import { ITransferOwnerRequest } from '../dtos/transfer-owner-request';
import { Injectable } from '@angular/core';
import { SearchCourseResult } from '../dtos/search-course-result';
import { SearchCourseType } from '../models/search-course-type.model';
import { SearchCourseUserResult } from './../dtos/search-course-user-result';
import { map } from 'rxjs/internal/operators/map';
import { of } from 'rxjs';

@Injectable()
export class CourseApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getCourse(id: string, showSpinner?: boolean): Promise<Course> {
    return this.get<ICourse>(`/courses/${id}`, null, showSpinner)
      .pipe(map(data => new Course(data)))
      .toPromise();
  }

  public getCoursesByIds(courseIds: string[], showSpinner: boolean = true): Promise<Course[]> {
    if (courseIds.length === 0) {
      return of([]).toPromise();
    }
    return this.post<string[], ICourse[]>(`/courses/getByIds`, courseIds, showSpinner)
      .pipe(map(result => result.map(_ => new Course(_))))
      .toPromise();
  }

  public saveCourse(request: ISaveCourseRequest): Promise<Course> {
    return this.post<ISaveCourseRequest, ICourse>(`/courses/save`, request)
      .pipe(map(data => new Course(data)))
      .toPromise();
  }

  public deleteCourse(id: string): Promise<void> {
    return this.delete<void>(`/courses/${id}`).toPromise();
  }

  public cloneCourse(request: ICloneCourseRequest, showSpinner: boolean = true): Promise<Course> {
    return this.post<ICloneCourseRequest, ICourse>(`/courses/clone`, request, showSpinner)
      .pipe(map(data => new Course(data)))
      .toPromise();
  }

  public changeCourseStatus(request: IChangeCourseStatusRequest): Promise<void> {
    return this.put<IChangeCourseStatusRequest, void>(`/courses/changeStatus`, request).toPromise();
  }

  public transferOwnerCourse(request: ITransferOwnerRequest, showSpinner: boolean = true): Promise<Course> {
    return this.put<ITransferOwnerRequest, ICourse>(`/courses/transfer`, request, showSpinner)
      .pipe(map(_ => new Course(_)))
      .toPromise();
  }

  public searchCourses(
    searchText: string = '',
    filter: IFilter = null,
    searchType: SearchCourseType = SearchCourseType.Owner,
    searchStatus: CourseStatus[] = null,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    checkCourseContent: boolean | null = false,
    coursePlanningCycleId?: string,
    showSpinner?: boolean
  ): Promise<SearchCourseResult> {
    return this.post<ISearchCourseRequest, SearchCourseResult>(
      '/courses/search',
      {
        searchText,
        filter,
        searchType,
        searchStatus,
        skipCount,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount,
        checkCourseContent,
        coursePlanningCycleId
      },
      showSpinner
    )
      .pipe(
        map(_ => {
          return new SearchCourseResult(_);
        })
      )
      .toPromise();
  }

  public searchCourseUser(request: ISearchCourseUserRequest, showSpinner?: boolean): Promise<SearchCourseUserResult> {
    return this.post<object, SearchCourseUserResult>(`/courses/searchUsers`, request, showSpinner)
      .pipe(map(result => new SearchCourseUserResult(result)))
      .toPromise();
  }

  public checkExistedCourseField(request: CheckExistedCourseFieldRequest, showSpinner?: boolean): Promise<boolean> {
    return this.post<CheckExistedCourseFieldRequest, boolean>(`/courses/checkExistedCourseField`, request, showSpinner).toPromise();
  }

  public archiveCourse(request: IArchiveCourseRequest): Promise<void> {
    return this.put<IArchiveCourseRequest, void>(`/courses/archive`, request).toPromise();
  }

  public checkCourseEndDateValidWithClassEndDateValidator(
    request: CheckCourseEndDateValidWithClassEndDateRequest,
    showSpinner?: boolean
  ): Promise<boolean> {
    return this.post<CheckCourseEndDateValidWithClassEndDateRequest, boolean>(
      `/courses/checkCourseEndDateValidWithClassEndDate`,
      request,
      showSpinner
    ).toPromise();
  }
}
