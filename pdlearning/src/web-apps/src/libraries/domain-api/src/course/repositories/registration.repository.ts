import { BaseRepository, IFilter } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { CourseRepositoryContext } from '../course-repository-context';
import { IAddParticipantsRequest } from '../dtos/add-participants-request';
import { IAddParticipantsResult } from '../dtos/add-participants-result';
import { IChangeLearnerStatusRequest } from '../dtos/change-learner-status-request';
import { IChangeRegistrationChangeClassRunStatusRequest } from '../dtos/change-registration-change-classrun-status-request';
import { IChangeRegistrationCourseCriteriaOverridedStatusRequest } from '../dtos/change-registration-course-criteria-overrided-status-request';
import { IChangeRegistrationStatusRequest } from '../dtos/change-registration-status-request';
import { IChangeRegistrationWithdrawalStatusRequest } from '../dtos/change-registration-withdrawal-status-request';
import { IImportParticipantRequest } from '../dtos/import-participant-request';
import { IMassClassRunChangeRequest } from '../dtos/classrun-change-request.dto';
import { Injectable } from '@angular/core';
import { Registration } from '../models/registrations.model';
import { RegistrationApiService } from '../services/registration-api.service';
import { SearchRegistrationResult } from '../dtos/search-registration-result';
import { SearchRegistrationsType } from '../models/search-registrations-type.model';

@Injectable()
export class RegistrationRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: RegistrationApiService) {
    super(context);
  }
  public loadSearchRegistration(
    courseId: string,
    classRunId: string,
    assignmentId?: string,
    searchType: SearchRegistrationsType = SearchRegistrationsType.ClassRunRegistration,
    searchText: string = '',
    applySearchTextForCourse: boolean = false,
    userFilter: IFilter = null,
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number = 10
  ): Observable<SearchRegistrationResult> {
    return this.processUpsertData(
      this.context.registrationSubject,
      implicitLoad =>
        from(
          this.apiSvc.searchRegistration(
            courseId,
            classRunId,
            assignmentId,
            searchType,
            searchText,
            applySearchTextForCourse,
            userFilter,
            filter,
            skipCount,
            maxResultCount,
            !implicitLoad
          )
        ),
      'loadSearchRegistration',
      [courseId, classRunId, assignmentId, searchType, searchText, applySearchTextForCourse, userFilter, filter, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      null,
      null,
      null,
      Registration.optionalProps
    );
  }

  public getRegistrationById(id: string): Observable<Registration> {
    return this.processUpsertData(
      this.context.registrationSubject,
      implicitLoad => from(this.apiSvc.getRegistrationById(id, !implicitLoad)),
      'loadRegistrationById',
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
      Registration.optionalProps
    );
  }

  public getRegistrationByIds(ids: string[]): Observable<Registration[]> {
    return this.processUpsertData(
      this.context.registrationSubject,
      implicitLoad => from(this.apiSvc.getRegistrationByIds(ids, !implicitLoad)),
      'loadRegistrationByIds',
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
      Registration.optionalProps
    );
  }

  public changeRegistrationStatus(request: IChangeRegistrationStatusRequest): Promise<void> {
    return this.apiSvc.changeRegistrationStatus(request).then(_ => {
      this.upsertData(
        this.context.registrationSubject,
        request.ids.map(id => {
          return {
            id: id,
            status: request.status
          };
        }),
        x => x.id
      );
      this.processRefreshData('loadSearchRegistration');
      return _;
    });
  }

  public overrideRegistrationCourseCriteria(request: IChangeRegistrationCourseCriteriaOverridedStatusRequest): Promise<void> {
    return this.apiSvc.overrideRegistrationCourseCriteria(request).then(_ => {
      this.upsertData(
        this.context.registrationSubject,
        request.registrationIds.map(id => {
          return {
            id: id,
            classrunId: request.classrunId
          };
        }),
        x => x.id
      );
      this.processRefreshData('loadSearchRegistration');
      return _;
    });
  }

  public changeRegistrationWithdrawalStatus(request: IChangeRegistrationWithdrawalStatusRequest): Promise<void> {
    return this.apiSvc.changeRegistrationWithdrawalStatus(request).then(_ => {
      this.upsertData(
        this.context.registrationSubject,
        request.ids.map(id => {
          return {
            id: id,
            withdrawalStatus: request.withdrawalStatus,
            comment: request.comment
          };
        }),
        x => x.id
      );
      this.processRefreshData('loadSearchRegistration');
      return _;
    });
  }

  public changeRegistrationClassRunChangeStatus(request: IChangeRegistrationChangeClassRunStatusRequest): Promise<void> {
    return this.apiSvc.changeRegistrationClassRunChangeStatus(request).then(_ => {
      this.upsertData(
        this.context.registrationSubject,
        request.ids.map(id => {
          return {
            id: id,
            classRunChangeStatus: request.classRunChangeStatus,
            comment: request.comment
          };
        }),
        x => x.id
      );
      this.processRefreshData('loadSearchRegistration');
      return _;
    });
  }

  public completeOrIncompleteRegistration(request: IChangeLearnerStatusRequest): Promise<void> {
    return this.apiSvc.completeOrIncompleteRegistration(request).then(_ => {
      this.processRefreshData('getCompletionRate', [request.classRunId]);
      this.processRefreshData('loadSearchRegistration', [request.courseId, request.classRunId]);
      return _;
    });
  }

  public addParticipants(request: IAddParticipantsRequest): Promise<IAddParticipantsResult> {
    return this.apiSvc.addParticipants(request).then(_ => {
      this.processRefreshData('loadSearchRegistration', [request.courseId, request.classRunId]);
      return _;
    });
  }

  public importParticipant(request: IImportParticipantRequest): Promise<IAddParticipantsResult> {
    return this.apiSvc.importParticipant(request).then(_ => {
      this.processRefreshData('loadSearchRegistration');
      return _;
    });
  }

  public massChangeClassRun(request: IMassClassRunChangeRequest): Promise<void> {
    return this.apiSvc.massChangeClassRun(request).then(_ => {
      this.processRefreshData('loadSearchRegistration');
      return _;
    });
  }
}
