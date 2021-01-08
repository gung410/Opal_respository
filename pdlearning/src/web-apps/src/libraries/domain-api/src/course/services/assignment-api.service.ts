import { Assignment, IAssignment } from '../models/assignment.model';
import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { INoOfAssignmentDoneInfo, NoOfAssignmentDoneInfo } from '../models/no-of-assignment-done-info.model';

import { AssignmentType } from '../models/assignment-type.model';
import { Constant } from '@opal20/authentication';
import { ISaveAssignmentRequest } from '../dtos/save-assignment-request';
import { ISearchAssignmentByIdsRequest } from '../dtos/search-assignment-by-ids-request';
import { ISetupPeerAssessmentRequest } from '../dtos/setup-peer-assessment-request';
import { Injectable } from '@angular/core';
import { SearchAssignmentResult } from './../dtos/search-assignment-result';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class AssignmentApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getAssignmentById(id: string, forLearnerAnswer: boolean = false, showSpinner?: boolean): Promise<Assignment> {
    const request = {
      id: id,
      forLearnerAnswer: forLearnerAnswer
    };
    return this.get<IAssignment>(`/assignment/byId`, request, showSpinner)
      .pipe(map(data => new Assignment(data)))
      .toPromise();
  }

  public saveAssignment(request: ISaveAssignmentRequest): Promise<Assignment> {
    return this.post<ISaveAssignmentRequest, IAssignment>(`/assignment/save`, request)
      .pipe(map(data => new Assignment(data)))
      .toPromise();
  }

  public deleteAssignment(id: string): Promise<void> {
    return this.delete<void>(`/assignment/${id}`).toPromise();
  }

  public getAssignments(
    courseId: string,
    classRunId: string,
    filterType: AssignmentType = AssignmentType.Quiz,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    includeQuizForm: boolean = false,
    showSpinner?: boolean
  ): Promise<SearchAssignmentResult> {
    const request = {
      courseId: courseId,
      classRunId: classRunId,
      filterType: filterType,
      skipCount: skipCount,
      maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount,
      includeQuizForm: includeQuizForm
    };
    return this.get<SearchAssignmentResult>('/assignment/getAssignments', request, showSpinner)
      .pipe(
        map(_ => {
          return new SearchAssignmentResult(_);
        })
      )
      .toPromise();
  }

  public getAssignmentsByIds(ids: string[]): Promise<Assignment[]> {
    const request: ISearchAssignmentByIdsRequest = {
      ids: ids,
      includeQuizForm: true
    };
    return this.post<ISearchAssignmentByIdsRequest, IAssignment[]>(`/assignment/getAssignmentByIds`, request)
      .pipe(map(data => data.map(_ => new Assignment(_))))
      .toPromise();
  }

  public getNoOfAssignmentDones(registrationIds: string[], classRunId: string, showSpinner?: boolean): Promise<NoOfAssignmentDoneInfo[]> {
    const request = {
      registrationIds: registrationIds,
      classRunId: classRunId
    };
    return this.get<INoOfAssignmentDoneInfo[]>('/assignment/getNoOfAssignmentDones', request, showSpinner)
      .pipe(
        map(_ => {
          return _.map(p => new NoOfAssignmentDoneInfo(p));
        })
      )
      .toPromise();
  }

  public setupPeerAssessment(request: ISetupPeerAssessmentRequest): Promise<void> {
    return this.post<ISetupPeerAssessmentRequest, void>(`/assignment/setupPeerAssessment`, request).toPromise();
  }
}
