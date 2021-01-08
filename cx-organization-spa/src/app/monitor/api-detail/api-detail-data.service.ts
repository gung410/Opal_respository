import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpHelpers } from 'app-utilities/http-helpers';

@Injectable()
export class ApiDetailDataService {
  constructor(private httpHelper: HttpHelpers) {}

  getMonitorStatus(apiOrigin: string): Observable<any> {
    return this.httpHelper.get(`${apiOrigin}/monitor/status`);
  }
}
