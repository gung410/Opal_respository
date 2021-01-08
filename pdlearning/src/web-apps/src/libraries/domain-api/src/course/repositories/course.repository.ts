import { BaseRepository, IFilter, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { Course } from '../models/course.model';
import { CourseApiService } from '../services/course-api.service';
import { CourseRepositoryContext } from '../course-repository-context';
import { CourseStatus } from '../../share/models/course-status.enum';
import { IArchiveCourseRequest } from '../dtos/archive-course-request';
import { IChangeCourseStatusRequest } from '../dtos/change-course-status-request';
import { ICloneCourseRequest } from '../dtos/clone-course-request';
import { ISaveCourseRequest } from '../dtos/save-course-request';
import { ISearchCourseUserRequest } from '../dtos/search-course-users-request';
import { ITransferOwnerRequest } from '../dtos/transfer-owner-request';
import { Injectable } from '@angular/core';
import { SearchCourseResult } from '../dtos/search-course-result';
import { SearchCourseType } from '../models/search-course-type.model';
import { SearchCourseUserResult } from './../dtos/search-course-user-result';

@Injectable()
export class CourseRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: CourseApiService) {
    super(context);
  }

  public loadSearchCourses(
    searchText: string = '',
    filter: IFilter = null,
    searchType: SearchCourseType = SearchCourseType.Owner,
    searchStatus: CourseStatus[] = null,
    skipCount: number = 0,
    maxResultCount: number = 10,
    checkCourseContent: boolean = false,
    coursePlanningCycleId?: string,
    showSpinner: boolean = true
  ): Observable<SearchCourseResult> {
    return this.processUpsertData(
      this.context.coursesSubject,
      implicitLoad =>
        from(
          this.apiSvc.searchCourses(
            searchText,
            filter,
            searchType,
            searchStatus,
            skipCount,
            maxResultCount,
            checkCourseContent,
            coursePlanningCycleId,
            !implicitLoad && showSpinner
          )
        ),
      'searchCourse',
      [searchText, filter, searchType, searchStatus, skipCount, maxResultCount, checkCourseContent, coursePlanningCycleId],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true,
      null,
      null,
      Course.optionalProps
    );
  }

  public loadSearchCourseUsers(request: ISearchCourseUserRequest, showSpinner: boolean = true): Observable<SearchCourseUserResult> {
    return this.processUpsertData(
      this.context.courseUsersSubject,
      implicitLoad => from(this.apiSvc.searchCourseUser(request, !implicitLoad && showSpinner)),
      'searchCourseUser',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true
    );
  }

  public loadCourse(id: string): Observable<Course> {
    return this.processUpsertData(
      this.context.coursesSubject,
      implicitLoad => from(this.apiSvc.getCourse(id, !implicitLoad)),
      'loadCourse',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id];
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id,
      true,
      null,
      null,
      Course.optionalProps
    );
  }

  public loadCourses(ids: string[], showSpinner: boolean = true): Observable<Course[]> {
    return this.processUpsertData(
      this.context.coursesSubject,
      implicitLoad => from(this.apiSvc.getCoursesByIds(ids, !implicitLoad && showSpinner)),
      'loadCourses',
      [ids],
      'implicitReload',
      (repoData, apiResult) => {
        return apiResult.map(_ => repoData[_.id]).filter(p => p != null);
      },
      apiResult => apiResult,
      x => x.id,
      true,
      null,
      null,
      Course.optionalProps
    );
  }

  public cloneCourse(request: ICloneCourseRequest): Observable<Course> {
    return this.processUpsertData(
      this.context.coursesSubject,
      implicitLoad => from(this.apiSvc.cloneCourse(request, !implicitLoad)),
      'cloneCourse',
      [request],
      'explicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id];
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id,
      true,
      null,
      null,
      Course.optionalProps
    );
  }
  public saveCourse(request: ISaveCourseRequest): Observable<Course> {
    return from(
      this.apiSvc.saveCourse(request).then(course => {
        this.upsertData(
          this.context.coursesSubject,
          [new Course(Utils.cloneDeep(course))],
          item => item.id,
          true,
          null,
          Course.optionalProps
        );
        return course;
      })
    );
  }

  public changeCourseStatus(request: IChangeCourseStatusRequest): Observable<void> {
    return from(
      this.apiSvc.changeCourseStatus(request).then(_ => {
        this.upsertData(
          this.context.coursesSubject,
          request.ids.map(id => {
            return <Partial<Course>>{ id: id, status: request.status };
          }),
          item => item.id
        );
        return _;
      })
    );
  }

  public deleteCourse(courseId: string): Promise<void> {
    return this.apiSvc.deleteCourse(courseId).then(_ => {
      this.processRefreshData('searchCourse');
    });
  }

  public transferOwnerCourse(request: ITransferOwnerRequest): Promise<Course> {
    return this.apiSvc.transferOwnerCourse(request).then(_ => {
      this.processRefreshData('searchCourse');
      return _;
    });
  }

  public archiveCourse(request: IArchiveCourseRequest): Promise<void> {
    return this.apiSvc.archiveCourse(request).then(_ => {
      this.processRefreshData('searchCourse');
      return _;
    });
  }
}
