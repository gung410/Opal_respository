import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccessRightsMatrixModelRequest } from '../dtos/request-dtos/access-rights-matrix-request';
import { AccessRightsRequest } from '../dtos/request-dtos/access-rights-request';
import { AccessRightsMatrixModel } from '../models/access-rights-matrix.model';
import { AccessRightsModel } from '../models/access-rights.model';
import { SystemRoleModel } from '../models/system-role.model';
import { IUpdateAccessRightsRequest } from '../models/update-accessRights-request.model';

@Injectable()
export class PermissionsApiService {
  private permissionsApiUrl: string = `${AppConstant.api.portal}`;

  constructor(private httpHelper: HttpHelpers, private http: HttpClient) {}

  getAccessRights(
    accessRightsRequest: AccessRightsRequest
  ): Observable<AccessRightsModel[]> {
    return this.httpHelper.get(
      `${this.permissionsApiUrl}/AccessRights`,
      accessRightsRequest
    );
  }

  getAccessRightsMatrix(
    accessRightsMatrixModelRequest: AccessRightsMatrixModelRequest
  ): Observable<AccessRightsMatrixModel> {
    return this.httpHelper.get(
      `${this.permissionsApiUrl}/AccessRights/accessrightmatrix`,
      accessRightsMatrixModelRequest
    );
  }

  updateSystemRoleInfo(
    editSystemRoleDto: SystemRoleModel
  ): Observable<SystemRoleModel> {
    return this.httpHelper.put(
      `${this.permissionsApiUrl}/${editSystemRoleDto.id}`,
      editSystemRoleDto
    );
  }

  updateAccessRightsBySystemRoleId(
    updateAccessRightsRequest: IUpdateAccessRightsRequest
  ): Observable<IUpdateAccessRightsRequest> {
    return this.httpHelper.put(
      `${this.permissionsApiUrl}/AccessRights/systemrole/${updateAccessRightsRequest.systemRoleId}`,
      updateAccessRightsRequest
    );
  }

  // Special Case
  // Because data of AccessRights for CSL don't store at the same place with others
  getAccessRightsMatrixOfCSL(): Observable<any> {
    return this.httpHelper
      .get(`${this.permissionsApiUrl}/sites?includeParameters=true`)
      .pipe(
        map(
          (siteParameter: any) =>
            JSON.parse(
              siteParameter.parameters[0].value
            ) as AccessRightsMatrixModel
        )
      );
  }
}
