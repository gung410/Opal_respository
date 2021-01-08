import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { UserTypeEnum } from 'app/shared/constants/user-type.enum';
import { Observable } from 'rxjs';

import { SystemRole } from '../models/system-role';

@Injectable()
export class CareerPathsDataService {
  constructor(private httpHelper: HttpHelpers) {}

  getCareerPaths(): Observable<SystemRole[]> {
    return this.httpHelper.get<SystemRole[]>(
      `${AppConstant.api.organization}/usertypes`,
      {
        archetypeEnums: UserTypeEnum.CareerPath,
        includeLocalizedData: true.toString()
      }
    );
  }
}
