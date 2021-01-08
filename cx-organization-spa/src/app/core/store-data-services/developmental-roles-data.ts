import { Injectable } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { UserTypeEnum } from 'app/shared/constants/user-type.enum';
import { Observable } from 'rxjs';
import { HttpHelpers } from 'app-utilities/http-helpers';

@Injectable()
export class DevelopmentalRolesDataService {
  constructor(private httpHelper: HttpHelpers) {}

  getDevelopmentalRoles(): Observable<any> {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: UserTypeEnum.DevelopmentalRole,
      includeLocalizedData: true.toString()
    });
  }
}
