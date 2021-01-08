import { AssignmentRepository, AssignmentType, SearchAssignmentResult } from '@opal20/domain-api';

import { AssignmentViewModel } from '../models/assignment-view.model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class ListAssignmentGridComponentService {
  constructor(private assignmentRepository: AssignmentRepository) {}

  public loadAssignments(
    courseId: string,
    classRunId: string,
    filterType: AssignmentType = AssignmentType.Quiz,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<AssignmentViewModel>> {
    return this.progressAssignments(
      this.assignmentRepository.getAssignments(courseId, classRunId, filterType, skipCount, maxResultCount),
      checkAll,
      selectedsFn
    );
  }

  private progressAssignments(
    assignmentObs: Observable<SearchAssignmentResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<AssignmentViewModel>> {
    return assignmentObs.pipe(
      map(searchAssignmentResult => {
        return <OpalGridDataResult<AssignmentViewModel>>{
          data: searchAssignmentResult.items.map(assignment => {
            return AssignmentViewModel.createFromModel(assignment, checkAll, selectedsFn != null ? selectedsFn() : {});
          }),
          total: searchAssignmentResult.totalCount
        };
      })
    );
  }
}
