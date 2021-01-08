import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { Observable } from 'rxjs';

import { DatahubEventModel } from '../models/audit-history.model';
import { PagingResponseModel } from '../models/user-management.model';

@Injectable({
  providedIn: 'root'
})
export class UserAuditHistoryService {
  constructor(private httpHelpers: HttpHelpers) {}

  getUserHistoricalData(
    extId: string,
    pageIndex: number = 0,
    pageSize: number = 0
  ): Observable<PagingResponseModel<DatahubEventModel>> {
    return this.httpHelpers.post<PagingResponseModel<DatahubEventModel>>(
      `${AppConstant.api.organization}/auditlog`,
      {
        userExtId: extId,
        pageIndex,
        pageSize
      }
    );
  }
}
