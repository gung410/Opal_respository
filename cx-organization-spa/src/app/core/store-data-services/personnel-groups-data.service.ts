import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { UserTypeEnum } from 'app/shared/constants/user-type.enum';
import { Observable } from 'rxjs';

@Injectable()
export class PersonnelGroupsDataService {
  constructor(private httpHelper: HttpHelpers) {}

  getPersonnelGroups(): Observable<any> {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: UserTypeEnum.PersonnelGroup,
      includeLocalizedData: true.toString()
    });
  }
}
