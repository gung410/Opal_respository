import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';

import { IGetModulePermissionRequest } from '../dtos/get-module-permission-request.dto';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PermissionService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.authConfig.permissionUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getModulePermissions(request: IGetModulePermissionRequest, showSpinner: boolean = false): Promise<IModulePermission[]> {
    return this.get<IModulePermission[]>('/', { ...request }, showSpinner).toPromise();
  }
}
