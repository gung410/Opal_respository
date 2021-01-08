import { BaseRepository, IFilter, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { ClassRun } from '../models/classrun.model';
import { ClassRunApiService } from '../services/classrun-api.service';
import { CourseRepositoryContext } from '../course-repository-context';
import { IChangeClassRunStatusRequest } from '../dtos/change-classrun-status-request';
import { IClassRunCancellationStatusRequest } from '../dtos/change-classrun-cancellation-status-request';
import { IClassRunRescheduleStatusRequest } from './../dtos/change-classrun-reschedule-status-request';
import { ISaveClassRunRequest } from '../dtos/save-classrun-request';
import { IToggleCourseAutomateRequest } from '../dtos/toggle-course-automate-request';
import { IToggleCourseCriteriaRequest } from '../dtos/toggle-course-criteria-request';
import { Injectable } from '@angular/core';
import { RegistrationApiService } from '../services/registration-api.service';
import { SearchClassRunResult } from '../dtos/search-classrun-result';
import { SearchClassRunType } from './../models/search-classrun-type.model';

@Injectable()
export class ClassRunRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: ClassRunApiService, private registrationApiSvc: RegistrationApiService) {
    super(context);
  }

  public loadClassRunById(id: string, loadHasLearnerStarted: boolean = false): Observable<ClassRun> {
    return this.processUpsertData(
      this.context.classRunSubject,
      implicitLoad => from(this.apiSvc.getClassRunById(id, loadHasLearnerStarted, !implicitLoad)),
      'loadClassRunById',
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
      ClassRun.optionalProps
    );
  }

  public loadClassRunsByIds(ids: string[]): Observable<ClassRun[]> {
    return this.processUpsertData(
      this.context.classRunSubject,
      implicitLoad => from(this.apiSvc.getClassRunsByIds(ids, !implicitLoad)),
      'loadClassRunsByIds',
      [ids],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id,
      true,
      null,
      null,
      ClassRun.optionalProps
    );
  }

  public loadClassRunsByCourseId(
    courseId: string,
    searchType: SearchClassRunType = SearchClassRunType.Owner,
    searchText: string = '',
    filter: IFilter = null,
    notStarted: boolean = false,
    notEnded: boolean = false,
    skipCount: number = 0,
    maxResultCount: number = 10,
    loadHasContentInfo?: boolean,
    showSpinner: boolean = true,
    asRequest?: boolean
  ): Observable<SearchClassRunResult> {
    return this.processUpsertData(
      this.context.classRunSubject,
      implicitLoad =>
        from(
          this.apiSvc.getClassRunByCourseId(
            courseId,
            searchType,
            searchText,
            filter,
            notStarted,
            notEnded,
            skipCount,
            maxResultCount,
            loadHasContentInfo,
            !implicitLoad && showSpinner
          )
        ),
      'loadClassRunsByCourseId',
      [courseId, searchType, skipCount, maxResultCount, searchText, notStarted, notEnded, filter, loadHasContentInfo],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true,
      asRequest,
      null,
      ClassRun.optionalProps
    );
  }

  public saveClassRun(request: ISaveClassRunRequest): Observable<ClassRun> {
    return from(
      this.apiSvc.saveClassRun(request).then(classrun => {
        this.upsertData(
          this.context.classRunSubject,
          [new ClassRun(Utils.cloneDeep(request.data))],
          item => item.id,
          true,
          null,
          ClassRun.optionalProps
        );
        return classrun;
      })
    );
  }

  public changeClassRunStatus(request: IChangeClassRunStatusRequest): Observable<void> {
    return from(
      this.apiSvc.changeClassRunStatus(request).then(_ => {
        this.upsertData(
          this.context.classRunSubject,
          request.ids.map(id => {
            return <Partial<ClassRun>>{ id: id, status: request.status };
          }),
          item => item.id
        );
        this.processRefreshData('loadClassRunsByCourseId');
        return _;
      })
    );
  }

  public changeClassRunCancellationStatus(request: IClassRunCancellationStatusRequest): Observable<void> {
    return from(
      this.apiSvc.changeCancellationStatus(request).then(_ => {
        this.upsertData(
          this.context.classRunSubject,
          request.ids.map(id => {
            return <Partial<ClassRun>>{
              id: id,
              cancellationStatus: request.cancellationStatus,
              comment: request.comment
            };
          }),
          item => item.id
        );
        this.processRefreshData('loadClassRunsByCourseId');
        return _;
      })
    );
  }

  public changeClassRunRescheduleStatus(request: IClassRunRescheduleStatusRequest, forCourseId: string): Observable<void> {
    return from(
      this.apiSvc.changeRescheduleStatus(request).then(_ => {
        this.upsertData(
          this.context.classRunSubject,
          request.ids.map(id => {
            return <Partial<ClassRun>>{
              id: id,
              rescheduleStatus: request.rescheduleStatus,
              rescheduleStartDate: request.startDateTime,
              rescheduleStartDateTime: request.startDateTime,
              rescheduleEndDate: request.startDateTime,
              rescheduleEndDateTime: request.startDateTime,
              rescheduleSessions: request.rescheduleSessions,
              comment: request.comment
            };
          }),
          item => item.id
        );
        this.processRefreshData('loadClassRunsByCourseId');
        this.processRefreshData('loadCourse', [forCourseId]);
        return _;
      })
    );
  }

  public getCompletionRate(classRunId: string): Observable<number> {
    return this.processUpsertData(
      this.context.classRunCompletionRateInfoSubject,
      implicitLoad => from(this.registrationApiSvc.getCompletionRate(classRunId, !implicitLoad)),
      'getCompletionRate',
      [classRunId],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[classRunId].completionRate;
        return apiResult;
      },
      apiResult => [{ classRunId: classRunId, completionRate: apiResult }],
      x => x.classRunId,
      true
    );
  }

  public toggleCourseCriteria(request: IToggleCourseCriteriaRequest): Observable<void> {
    return from(
      this.apiSvc.toggleCourseCriteria(request).then(_ => {
        this.upsertData(
          this.context.classRunSubject,
          [
            <Partial<ClassRun>>{
              id: request.classRunId,
              courseCriteriaActivated: request.courseCriteriaActivated
            }
          ],
          item => item.id
        );
        this.processRefreshData('loadClassRunsByCourseId');
        return _;
      })
    );
  }

  public toggleCourseAutomate(request: IToggleCourseAutomateRequest): Observable<void> {
    return from(
      this.apiSvc.toggleCourseAutomate(request).then(_ => {
        this.upsertData(
          this.context.classRunSubject,
          [
            <Partial<ClassRun>>{
              id: request.classRunId,
              courseAutomateActivated: request.courseAutomateActivated
            }
          ],
          item => item.id
        );
        this.processRefreshData('loadClassRunsByCourseId');
        return _;
      })
    );
  }
}
