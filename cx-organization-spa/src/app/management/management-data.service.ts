import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { Observable } from 'rxjs';

@Injectable()
export class ManagementDataService {
  constructor(private httpHelper: HttpHelpers) {}
  executeApiAction(apiOrigin: string, action: string): Observable<any> {
    return this.httpHelper.get(`${apiOrigin}/management/${action}`);
  }
}
