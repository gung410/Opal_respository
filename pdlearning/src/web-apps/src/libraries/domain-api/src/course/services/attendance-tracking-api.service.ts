import { AttendanceRatioOfPresentInfo, IAttendanceRatioOfPresentInfo } from '../models/attendance-ratio-of-present-info.model';
import { AttendanceTracking, IAttendanceTracking } from '../models/attendance-tracking.model';
import { BaseBackendService, CommonFacadeService, IFilter } from '@opal20/infrastructure';

import { Constant } from '@opal20/authentication';
import { IAttendanceTrackingStatusRequest } from '../dtos/attendance-tracking-status-request';
import { IChangeAttendanceTrackingReasonForAbsenceRequest } from '../dtos/change-attendance-tracking-reason-request';
import { ISearchAttendaceTrackingRequest } from './../dtos/search-attendance-tracking-request';
import { Injectable } from '@angular/core';
import { SearchAttendaceTrackingResult } from '../dtos/attendance-tracking-result';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class AttendanceTrackingService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getAttendaceTrackingBySessionId(
    sessionId: string,
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner?: boolean
  ): Promise<SearchAttendaceTrackingResult> {
    return this.post<ISearchAttendaceTrackingRequest, SearchAttendaceTrackingResult>(
      '/attendancetracking/session',
      {
        sessionId,
        searchText,
        filter,
        skipCount,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
      },
      showSpinner
    )
      .pipe(
        map(_ => {
          return new SearchAttendaceTrackingResult(_);
        })
      )
      .toPromise();
  }
  public getAttendanceTrackingById(id: string, showSpinner?: boolean): Promise<AttendanceTracking> {
    return this.get<IAttendanceTracking>(`/attendancetracking/${id}`, null, showSpinner)
      .pipe(map(data => new AttendanceTracking(data)))
      .toPromise();
  }
  public changeAttendanceTrackingStatus(request: IAttendanceTrackingStatusRequest): Promise<void> {
    return this.put<IAttendanceTrackingStatusRequest, void>(`/attendancetracking/changeStatus`, request).toPromise();
  }

  public getAttendenceRatioOfPresents(
    registrationIds: string[],
    classRunId: string,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Promise<AttendanceRatioOfPresentInfo[]> {
    const request = {
      registrationIds: registrationIds,
      classRunId: classRunId,
      skipCount: skipCount,
      maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
    };
    return this.get<IAttendanceRatioOfPresentInfo[]>('/attendancetracking/ratioofpresents', request, showSpinner)
      .pipe(
        map(_ => {
          return _.map(p => new AttendanceRatioOfPresentInfo(p));
        })
      )
      .toPromise();
  }

  public getUserAttendanceTrackingByClassRunId(classRunId: string, showSpinner: boolean = false): Promise<AttendanceTracking[]> {
    return this.get<IAttendanceTracking[]>(`/attendancetracking/currentUser/${classRunId}`, null, showSpinner)
      .pipe(map(_ => _.map(p => new AttendanceTracking(p))))
      .toPromise();
  }

  public takeAttendanceTracking(sessionId: string, sessionCode: string, showSpinner: boolean = false): Promise<AttendanceTracking> {
    return this.put<{ sessionId: string; sessionCode: string }, IAttendanceTracking>(
      '/attendancetracking/learnerTakeAttendance',
      { sessionId, sessionCode },
      showSpinner
    )
      .pipe(map(_ => new AttendanceTracking(_)))
      .toPromise();
  }

  public changeReasonForAbsence(request: IChangeAttendanceTrackingReasonForAbsenceRequest, showSpinner: boolean = false): Promise<void> {
    return this.put<IChangeAttendanceTrackingReasonForAbsenceRequest, void>(
      '/attendancetracking/changeReason',
      request,
      showSpinner
    ).toPromise();
  }
}
