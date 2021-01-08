import { Injectable } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import {
  AssignLnAssessmentResultModel,
  AssignContentsDTO,
} from 'app/organisational-development/models/idp.model';
import { Observable } from 'rxjs';

@Injectable()
export class LnAssessmentsDataService {
  constructor(private http: HttpHelpers) {}
  assignLNAssessmentsToEmployees(
    assignedEmployeeIdentities: {
      archetype: string;
      id: number;
      extId: string;
    }[],
    dueDate: Date
  ): Observable<AssignLnAssessmentResultModel[]> {
    const assignContentsDto: AssignContentsDTO = {
      identities: assignedEmployeeIdentities,
      dueDate,
      timestamp: new Date(),
      updateIfExists: true,
      ignoreUpdatingAssessmentStatus: true,
    };

    return this.http.post<AssignLnAssessmentResultModel[]>(
      `${AppConstant.api.competence}/idp/needs/assign_contents`,
      assignContentsDto,
      undefined,
      { avoidIntercepterCatchError: true }
    );
  }
}
