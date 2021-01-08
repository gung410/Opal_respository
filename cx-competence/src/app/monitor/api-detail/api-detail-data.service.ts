import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { Observable } from 'rxjs';

@Injectable()
export class ApiDetailDataService {
  constructor(private httpHelper: HttpHelpers) {}

  getMonitorStatus(apiOrigin: string): Observable<any> {
    return this.httpHelper.get(`${apiOrigin}/monitor/status`);
  }
}
