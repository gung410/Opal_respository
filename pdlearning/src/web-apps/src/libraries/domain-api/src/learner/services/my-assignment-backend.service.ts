import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { IChangeMyAssignmentStatus, IMyAssignmentRequest } from '../dtos/my-assignment-request.dto';
import { IMyAssignment, MyAssignment, MyAssignmentStatus } from '../models/my-assignment.model';

import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class MyAssignmentApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/me/assignments';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getMyAssignments(request: IMyAssignmentRequest, showSpinner: boolean = true): Promise<MyAssignment[]> {
    const queryParams: IGetParams = {
      registrationId: request.registrationId,
      skipCount: request.skipCount,
      maxResultCount: request.maxResultCount
    };
    return this.get<IPagedResultDto<IMyAssignment>>('', queryParams, showSpinner)
      .pipe(map(result => result.items.map(_ => new MyAssignment(_))))
      .toPromise();
  }

  public changeStatus(request: IChangeMyAssignmentStatus): Promise<void> {
    // Change assignment status to completed was unexpected to do by frontend.
    if (request.status === MyAssignmentStatus.Completed) {
      throw new Error('Unexpected status change');
    }

    return this.post<IChangeMyAssignmentStatus, void>('/changeStatus', request).toPromise();
  }

  public getMyAssignmentsByAssignmentIds(assignmentIds: string[], showSpinner: boolean = true): Promise<MyAssignment[]> {
    return this.post<string[], MyAssignment[]>('/byAssignmentIds', assignmentIds, showSpinner)
      .pipe(map(result => result.map(_ => new MyAssignment(_))))
      .toPromise();
  }

  public getMyAssignmentsByAssignmentId(assignmentId: string, showSpinner: boolean = true): Promise<MyAssignment> {
    return this.get<MyAssignment>(`/byAssignmentId/${assignmentId}`)
      .pipe(map(result => new MyAssignment(result)))
      .toPromise();
  }
}
