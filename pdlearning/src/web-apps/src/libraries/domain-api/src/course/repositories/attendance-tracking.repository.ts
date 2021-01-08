import { BaseRepository, IFilter } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { AttendanceRatioOfPresentInfo } from '../models/attendance-ratio-of-present-info.model';
import { AttendanceTracking } from '../models/attendance-tracking.model';
import { AttendanceTrackingService } from '../services/attendance-tracking-api.service';
import { CourseRepositoryContext } from '../course-repository-context';
import { IAttendanceTrackingStatusRequest } from '../dtos/attendance-tracking-status-request';
import { Injectable } from '@angular/core';
import { SearchAttendaceTrackingResult } from '../dtos/attendance-tracking-result';

@Injectable()
export class AttendanceTrackingRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: AttendanceTrackingService) {
    super(context);
  }

  public loadAttendenceRatioOfPresents(
    registrationIds: string[],
    classRunId: string,
    skipCount: number = 0,
    maxResultCount: number = 10
  ): Observable<AttendanceRatioOfPresentInfo[]> {
    return this.processUpsertData(
      this.context.attendanceRatioOfPresentInfoTrackingSubject,
      implicitLoad => from(this.apiSvc.getAttendenceRatioOfPresents(registrationIds, classRunId, skipCount, maxResultCount, !implicitLoad)),
      'loadAttendenceRatioOfPresents',
      [registrationIds, classRunId, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.registrationId]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.registrationId
    );
  }

  public loadAttendanceTrackingById(id: string): Observable<AttendanceTracking> {
    return this.processUpsertData(
      this.context.attendaceTrackingSubject,
      implicitLoad => from(this.apiSvc.getAttendanceTrackingById(id, !implicitLoad)),
      'loadAttendanceTrackingById',
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
      null
    );
  }

  public loadSearchAttendaceTracking(
    sessionId: string,
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number = 10
  ): Observable<SearchAttendaceTrackingResult> {
    return this.processUpsertData(
      this.context.attendaceTrackingSubject,
      implicitLoad =>
        from(this.apiSvc.getAttendaceTrackingBySessionId(sessionId, searchText, filter, skipCount, maxResultCount, !implicitLoad)),
      'loadSearchAttendaceTracking',
      [sessionId, searchText, filter, skipCount, maxResultCount],
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

  public changAttendanceStatus(request: IAttendanceTrackingStatusRequest): Observable<void> {
    return from(
      this.apiSvc.changeAttendanceTrackingStatus(request).then(_ => {
        this.upsertData(
          this.context.attendaceTrackingSubject,
          request.ids.map(id => {
            return <Partial<AttendanceTracking>>{ id: id, status: request.status };
          }),
          item => item.id
        );
        return _;
      })
    );
  }
}
