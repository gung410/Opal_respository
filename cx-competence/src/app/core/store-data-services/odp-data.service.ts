import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { AppConstant } from 'app/shared/app.constant';

@Injectable({
  providedIn: 'root',
})
export class OdpDataService {
  constructor(private httpHelper: HttpHelpers) {}

  public getDirectionConfig() {
    return this.httpHelper.get(
      `${AppConstant.api.competence}/odp/directions/config`
    );
  }

  public getProgrammeConfig() {
    return this.httpHelper.get(
      `${AppConstant.api.competence}/odp/programmes/config`
    );
  }
}
