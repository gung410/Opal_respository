import { BaseBackendService, CommonFacadeService, IFilter } from '@opal20/infrastructure';
import { GetLearnerCourseViolationQueryResult, IGetLearnerCourseViolationQueryResult } from '../dtos/get-learner-course-violation-result';
import { IClassRunChangeRequest, IMassClassRunChangeRequest } from '../dtos/classrun-change-request.dto';
import { IRegistration, Registration } from '../models/registrations.model';
import { IRegistrationECertificateModel, RegistrationECertificateModel } from '../models/registration-ecertificate.model';
import { Observable, of } from 'rxjs';

import { Constant } from '@opal20/authentication';
import { HttpResponse } from '@angular/common/http';
import { IAddParticipantsRequest } from '../dtos/add-participants-request';
import { IAddParticipantsResult } from '../dtos/add-participants-result';
import { IChangeLearnerStatusRequest } from '../dtos/change-learner-status-request';
import { IChangeRegistrationChangeClassRunStatusRequest } from '../dtos/change-registration-change-classrun-status-request';
import { IChangeRegistrationCourseCriteriaOverridedStatusRequest } from '../dtos/change-registration-course-criteria-overrided-status-request';
import { IChangeRegistrationStatusByCourseClassRunRequest } from '../dtos/change-registration-status-by-course-class-run-request';
import { IChangeRegistrationStatusRequest } from '../dtos/change-registration-status-request';
import { IChangeRegistrationWithdrawalStatusRequest } from '../dtos/change-registration-withdrawal-status-request';
import { ICreateRegistrationRequest } from '../dtos/create-registration-request';
import { IExportParticipantRequest } from '../dtos/export-participants-request';
import { IExportParticipantTemplateRequest } from '../dtos/export-participant-template-request';
import { IImportParticipantRequest } from '../dtos/import-participant-request';
import { ISearchRegistrationRequest } from './../dtos/search-registration-request';
import { IWithdrawalRequest } from '../dtos/withdrawal-request';
import { Injectable } from '@angular/core';
import { SearchRegistrationResult } from '../dtos/search-registration-result';
import { SearchRegistrationsType } from '../models/search-registrations-type.model';
import { map } from 'rxjs/operators';

@Injectable()
export class RegistrationApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public searchRegistration(
    courseId: string,
    classRunId: string,
    excludeAssignedAssignmentId?: string,
    searchType: SearchRegistrationsType = SearchRegistrationsType.ClassRunRegistration,
    searchText: string = '',
    applySearchTextForCourse: boolean = false,
    userFilter: IFilter = null,
    filter: IFilter = null,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Promise<SearchRegistrationResult> {
    const request = {
      courseId: courseId,
      classRunId: classRunId,
      excludeAssignedAssignmentId: excludeAssignedAssignmentId,
      searchType: searchType,
      searchText: searchText,
      applySearchTextForCourse: applySearchTextForCourse,
      userFilter: userFilter,
      filter: filter,
      skipCount: skipCount,
      maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
    };
    return this.post<ISearchRegistrationRequest, SearchRegistrationResult>('/registration/search', request, showSpinner)
      .pipe(
        map(_ => {
          return new SearchRegistrationResult(_);
        })
      )
      .toPromise();
  }

  public getRegistrationById(id: string, showSpinner?: boolean): Promise<Registration> {
    return this.get<Registration>(`/registration/${id}`, null, showSpinner)
      .pipe(map(data => new Registration(data)))
      .toPromise();
  }

  public getRegistrationByIds(ids: string[], showSpinner: boolean = true): Promise<Registration[]> {
    if (ids.length === 0) {
      return of([]).toPromise();
    }
    return this.post<string[], IRegistration[]>(`/registration/getByIds`, ids, showSpinner)
      .pipe(map(result => result.map(_ => new Registration(_))))
      .toPromise();
  }

  public changeRegistrationStatus(request: IChangeRegistrationStatusRequest): Promise<void> {
    return this.put<IChangeRegistrationStatusRequest, void>(`/registration/changeStatus`, request).toPromise();
  }

  public overrideRegistrationCourseCriteria(request: IChangeRegistrationCourseCriteriaOverridedStatusRequest): Promise<void> {
    return this.put<IChangeRegistrationCourseCriteriaOverridedStatusRequest, void>(
      `/registration/overrideRegistrationCourseCriteria`,
      request
    ).toPromise();
  }

  public changeRegistrationWithdrawalStatus(request: IChangeRegistrationWithdrawalStatusRequest): Promise<void> {
    return this.put<IChangeRegistrationWithdrawalStatusRequest, void>(`/registration/changeWithdrawStatus`, request).toPromise();
  }

  public changeRegistrationClassRunChangeStatus(
    request: IChangeRegistrationChangeClassRunStatusRequest,
    showSpinner?: boolean
  ): Promise<void> {
    return this.put<IChangeRegistrationChangeClassRunStatusRequest, void>(
      `/registration/changeClassRunChangeStatus`,
      request,
      showSpinner
    ).toPromise();
  }

  public changeRegistrationStatusByCourseClassRun(request: IChangeRegistrationStatusByCourseClassRunRequest): Promise<void> {
    return this.put<IChangeRegistrationStatusByCourseClassRunRequest, void>(
      `/registration/changeStatusByCourseClassRun`,
      request
    ).toPromise();
  }

  public createRegistration(request: ICreateRegistrationRequest): Promise<Registration[]> {
    return this.post<ICreateRegistrationRequest, IRegistration[]>('/registration/createRegistration', request)
      .pipe(map(result => result.map(_ => new Registration(_))))
      .toPromise();
  }

  // Should use changeRegistrationWithdrawalStatus function
  public withdrawal(request: IWithdrawalRequest): Promise<unknown> {
    return this.put<IWithdrawalRequest, unknown>(`/registration/changeWithdrawStatus`, request).toPromise();
  }

  public completeOrIncompleteRegistration(request: IChangeLearnerStatusRequest): Promise<void> {
    return this.post<IChangeLearnerStatusRequest, void>(`/registration/completeOrIncompleteRegistration`, request).toPromise();
  }

  public getCompletionRate(classRunId: string, showSpinner?: boolean): Promise<number> {
    return this.get<number>(`/registration/getCompletionRate/${classRunId}`, null, showSpinner).toPromise();
  }

  public createClassRunChange(request: IClassRunChangeRequest): Promise<void> {
    return this.post<IClassRunChangeRequest, void>(`/registration/changeClassRun`, request).toPromise();
  }

  public massChangeClassRun(request: IMassClassRunChangeRequest): Promise<void> {
    return this.post<IMassClassRunChangeRequest, void>(`/registration/massChangeClassRun`, request).toPromise();
  }

  public addParticipants(request: IAddParticipantsRequest): Promise<IAddParticipantsResult> {
    return this.post<IAddParticipantsRequest, IAddParticipantsResult>(`/registration/addParticipants`, request).toPromise();
  }

  public importParticipant(request: IImportParticipantRequest): Promise<IAddParticipantsResult> {
    const formData = new FormData();
    formData.append('file', request.file);
    return this.post<FormData, IAddParticipantsResult>(`/registration/${request.courseId}/importParticipant`, formData).toPromise();
  }

  public getLearnerCourseViolation(courseId: string, classRunId: string): Promise<GetLearnerCourseViolationQueryResult> {
    const request = { courseId: courseId, classrunId: classRunId };
    return this.get<IGetLearnerCourseViolationQueryResult>(`/registration/getLearnerCourseViolation`, request)
      .pipe(
        map(_ => {
          return new GetLearnerCourseViolationQueryResult(_);
        })
      )
      .toPromise();
  }

  public completePostEvaluation(registrationId: string): Promise<void> {
    return this.put<object, void>(`/registration/${registrationId}/completePostEvaluation`, {}).toPromise();
  }

  public exportParticipant(request: IExportParticipantRequest): Observable<HttpResponse<Blob>> {
    return this.postDownloadFile<IExportParticipantRequest>(`/registration/exportParticipants`, request);
  }

  public downloadExportParticipantTemplate(request: IExportParticipantTemplateRequest): Promise<HttpResponse<Blob>> {
    return this.postDownloadFile<IExportParticipantTemplateRequest>(`/registration/exportParticipantTemplate`, request).toPromise();
  }

  public getRegistrationCertificateById(registrationId: string, showSpinner?: boolean): Promise<RegistrationECertificateModel> {
    return this.get<IRegistrationECertificateModel>(`/registration/${registrationId}/getCertificate`, null, showSpinner).toPromise();
  }
}
