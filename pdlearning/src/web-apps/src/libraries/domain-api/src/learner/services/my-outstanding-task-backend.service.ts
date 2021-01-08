import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { IOutstandingTaskResult, OutstandingTaskResult } from '../dtos/my-outstanding-result.dto';

import { IGetMyOutstandingTaskRequest } from '../dtos/my-outstanding-task-service.dto';
import { map } from 'rxjs/operators';

export class MyOutstandingTaskApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/me/outstandingTasks';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getOutstandingTasks(request: IGetMyOutstandingTaskRequest, showSpinner: boolean = true): Promise<OutstandingTaskResult> {
    const queryParams: IGetParams = {
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };

    return this.get<IOutstandingTaskResult>(``, queryParams, showSpinner)
      .pipe(map(_ => new OutstandingTaskResult(_)))
      .toPromise();
  }
}
