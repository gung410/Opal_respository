import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { Observable } from 'rxjs';
import { ArcheTypeEnum } from 'app-enums/user-type.enum';
import { AppConstant } from '../app.constant';

@Injectable({
  providedIn: 'root',
})
export class UserTypeService {
  constructor(private http: HttpHelpers) {}

  public getPersonnelGroups(): Observable<any> {
    return this.http.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: ArcheTypeEnum.PersonnelGroup,
      includeLocalizedData: true.toString(),
    });
  }

  public getCareerPaths(): Observable<any> {
    return this.http.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: ArcheTypeEnum.CareerPath,
      includeLocalizedData: true.toString(),
    });
  }

  public getDevelopmentalRole(): Observable<any> {
    return this.http.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: ArcheTypeEnum.DevelopmentalRole,
      includeLocalizedData: true.toString(),
    });
  }
}
