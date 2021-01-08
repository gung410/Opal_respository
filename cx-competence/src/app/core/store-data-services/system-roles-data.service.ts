import { Injectable } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { HttpHelpers } from 'app-utilities/httpHelpers';

@Injectable()
export class SystemRolesDataService {
  constructor(private httpHelper: HttpHelpers) {}

  getSystemRoles() {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: 'SystemRole',
      includeLocalizedData: true.toString(),
    });
  }
}
