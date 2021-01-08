import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';

@Injectable()
export class ManagementDataService {
  constructor(private httpHelper: HttpHelpers) {}
  executeApiAction(apiOrigin: string, action: string) {
    return this.httpHelper.get(`${apiOrigin}/management/${action}`);
  }
}
