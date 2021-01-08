import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { RolesRequest } from 'app/broadcast-messages/requests-dto/roles-request';
import { AppConstant } from 'app/shared/app.constant';
import { UserTypeEnum } from 'app/shared/constants/user-type.enum';
import { Observable } from 'rxjs';
import { CreateSystemRoleRequest } from '../dtos/create-system-role-request.dto';
import { CreateSystemRoleResponse } from '../dtos/create-system-role-response.dto';
import { IGetSystemRoleByIdRequest } from '../dtos/get-system-role-by-id-request.dto';
import { GetSystemRolesWithPermissionsRequest } from '../dtos/get-system-roles-with-permissions-request.dto';
import { UpdateSystemRoleInfoRequest } from '../dtos/update-system-role-info-request.dto';
import { SystemRoleSubjects } from '../models/system-role-subjects.model';

@Injectable()
export class SystemRolesDataService {
  readonly SYSTEM_ROLE_CODE: number = 43;
  private systemRoleUrl: string = `${AppConstant.api.portal}/SystemRoles`;
  constructor(private httpHelper: HttpHelpers) {}

  // OUTDATED ENDPOINTS
  getSystemRoles(): Observable<any> {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: UserTypeEnum.SystemRole,
      includeLocalizedData: true.toString()
    });
  }

  getRoles(request: RolesRequest): Observable<any> {
    return this.httpHelper.get(
      `${AppConstant.api.organization}/usertypes`,
      request
    );
  }

  getRolesInfo(request: RolesRequest): Observable<any> {
    return this.httpHelper.get(
      `${AppConstant.api.organization}/roles`,
      request
    );
  }

  // LATEST ENDPOINTS
  getSystemRolesWithPermissions(
    getSystemRolesWithPermissions: GetSystemRolesWithPermissionsRequest
  ): Observable<SystemRoleSubjects[]> {
    return this.httpHelper.get(
      `${this.systemRoleUrl}/`,
      getSystemRolesWithPermissions
    );
  }

  getSystemRoleById(
    getSystemRoleByIdRequest: IGetSystemRoleByIdRequest
  ): Observable<SystemRoleSubjects> {
    return this.httpHelper.get(
      `${this.systemRoleUrl}/${getSystemRoleByIdRequest.id}`,
      getSystemRoleByIdRequest
    );
  }

  updateSystemRole(
    updateSystemRoleInfoRequest: UpdateSystemRoleInfoRequest
  ): Observable<unknown> {
    return this.httpHelper.put(
      `${this.systemRoleUrl}/${updateSystemRoleInfoRequest.id}`,
      updateSystemRoleInfoRequest
    );
  }

  createSystemRole(
    createSystemRoleRequest: CreateSystemRoleRequest,
    isAvoidIntercepterCatchError: boolean = false
  ): Observable<CreateSystemRoleResponse> {
    return this.httpHelper.post(
      `${this.systemRoleUrl}/`,
      createSystemRoleRequest,
      null,
      isAvoidIntercepterCatchError
        ? {
            avoidIntercepterCatchError: true
          }
        : null
    );
  }

  deleteSystemRole(systemRoleId: number | string): Observable<any> {
    return this.httpHelper.delete(`${this.systemRoleUrl}/${systemRoleId}`);
  }
}
