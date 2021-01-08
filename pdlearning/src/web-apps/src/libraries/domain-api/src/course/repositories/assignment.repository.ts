import { BaseRepository, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { Assignment } from '../models/assignment.model';
import { AssignmentApiService } from '../services/assignment-api.service';
import { AssignmentType } from '../models/assignment-type.model';
import { CourseRepositoryContext } from './../course-repository-context';
import { ISaveAssignmentRequest } from '../dtos/save-assignment-request';
import { ISetupPeerAssessmentRequest } from '../dtos/setup-peer-assessment-request';
import { Injectable } from '@angular/core';
import { NoOfAssignmentDoneInfo } from '../models/no-of-assignment-done-info.model';
import { SearchAssignmentResult } from './../dtos/search-assignment-result';

@Injectable()
export class AssignmentRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: AssignmentApiService) {
    super(context);
  }

  public loadNoOfAssignmentDone(registrationIds: string[], classRunId: string): Observable<NoOfAssignmentDoneInfo[]> {
    return this.processUpsertData(
      this.context.noOfAssignmentDoneInfoTrackingSubject,
      implicitLoad => from(this.apiSvc.getNoOfAssignmentDones(registrationIds, classRunId, !implicitLoad)),
      'loadNoOfAssignmentDone',
      [registrationIds, classRunId],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.registrationId]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.registrationId
    );
  }

  public saveAssignment(request: ISaveAssignmentRequest): Observable<Assignment> {
    return from(
      this.apiSvc.saveAssignment(request).then(assignment => {
        this.upsertData(this.context.assignmentSubject, [Utils.cloneDeep(assignment)], item => item.id, true);
        this.refreshForEditingContent(request.data.courseId, request.data.classRunId);
        return assignment;
      })
    );
  }

  public getAssignmentById(id: string, forLearnerAnswer: boolean = false): Observable<Assignment> {
    return this.processUpsertData(
      this.context.assignmentSubject,
      implicitLoad => from(this.apiSvc.getAssignmentById(id, forLearnerAnswer, !implicitLoad)),
      'getAssignmentById',
      [id, forLearnerAnswer],
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
      Assignment.optionalProps
    );
  }

  public getAssignments(
    courseId: string,
    classRunId: string,
    filterType: AssignmentType = AssignmentType.Quiz,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    includeQuizForm: boolean = false
  ): Observable<SearchAssignmentResult> {
    return this.processUpsertData(
      this.context.assignmentSubject,
      implicitLoad =>
        from(this.apiSvc.getAssignments(courseId, classRunId, filterType, skipCount, maxResultCount, includeQuizForm, !implicitLoad)),
      'getAssignments',
      [courseId, classRunId, filterType, skipCount, maxResultCount, includeQuizForm],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(p => repoData[p.id]).filter(p => p != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true,
      null,
      null,
      Assignment.optionalProps
    );
  }

  public deleteAssignment(id: string, courseId: string, classRunId?: string): Promise<void> {
    return this.apiSvc.deleteAssignment(id).then(_ => {
      this.processRefreshData('getAssignments', [courseId, classRunId]);
      this.refreshForEditingContent(courseId, classRunId);
    });
  }

  public setupPeerAssessment(request: ISetupPeerAssessmentRequest): Observable<void> {
    return from(
      this.apiSvc.setupPeerAssessment(request).then(result => {
        this.processRefreshData('loadNoOfAssessmentDone');
        return result;
      })
    );
  }

  private refreshForEditingContent(courseId: string, classRunId?: string): void {
    this.processRefreshData('getTableOfContents', [courseId, classRunId]);
    this.processRefreshData('loadCourse', [courseId]);
    if (classRunId != null) {
      this.processRefreshData('loadClassRunById', [classRunId]);
    }
  }
}
