import { Injectable } from '@angular/core';
import { CreateSystemRoleRequest } from 'app/core/dtos/create-system-role-request.dto';
import { CreateSystemRoleResponse } from 'app/core/dtos/create-system-role-response.dto';
import { GetSystemRoleByIdRequest } from 'app/core/dtos/get-system-role-by-id-request.dto';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { Utils } from '../../shared/utilities/utils';
import { ActionSignal } from '../dtos/action-signal.dto';
import { EditingSignal } from '../dtos/editing-signal.dto';
import { HeaderComparison } from '../dtos/header-comparison.dto';
import { AccessRightsRequest } from '../dtos/request-dtos/access-rights-request';
import { GrantedType } from '../enum/granted-type.enum';
import { ObjectType } from '../enum/object-type.enum';
import { AccessRightsMatrixModel } from '../models/access-rights-matrix.model';
import { AccessRightRoles } from '../models/permissionRoles.model';
import { PermissionsApiService } from './permissions-api.service';

@Injectable()
export class PermissionsTableService {
  editingSignal$: Observable<EditingSignal>; // this is editing for the whole column signal
  actionSignal$: Observable<ActionSignal>;
  headerName$: BehaviorSubject<HeaderComparison> = new BehaviorSubject<HeaderComparison>(
    null
  );

  private editingSubject: Subject<EditingSignal> = new Subject<EditingSignal>();
  private actionSubject: Subject<ActionSignal> = new Subject<ActionSignal>();

  constructor(
    private permissionsApiSvc: PermissionsApiService,
    private systemRolesDataService: SystemRolesDataService
  ) {
    this.editingSignal$ = this.editingSubject.asObservable();
    this.actionSignal$ = this.actionSubject.asObservable();
  }

  sendEditingSignal(editingSignal: EditingSignal): void {
    this.editingSubject.next(editingSignal);
  }

  sendActionSignal(actionSignal: ActionSignal): void {
    this.actionSubject.next(actionSignal);
  }

  isHeaderNameDifference(): boolean {
    const headerCompare = new HeaderComparison(this.headerName$.getValue());

    return headerCompare.isDifferent();
  }

  generateRowData(accessRightsMatrixModel: AccessRightsMatrixModel): any[] {
    const systemRolesFollowedAccessRights = this.generateAccessRightsFollowedSystemRoles(
      accessRightsMatrixModel
    );

    const flattenPermissionRolesArrayResult = this.flattenPermissionRolesArray(
      systemRolesFollowedAccessRights
    );

    return flattenPermissionRolesArrayResult;
  }

  duplicateSystemRole(
    systemRoleId: number
  ): Observable<CreateSystemRoleResponse> {
    return this.systemRolesDataService
      .getSystemRoleById(
        new GetSystemRoleByIdRequest({
          id: systemRoleId,
          includeLocalizedData: true
        })
      )
      .pipe(
        switchMap((systemRoleInfo) => {
          if (!systemRoleInfo) {
            return of(null);
          }

          systemRoleInfo.localizedData[0].fields[0].localizedText = `Copy of ${systemRoleInfo.localizedData[0].fields[0].localizedText}`;

          return this.systemRolesDataService.createSystemRole(
            new CreateSystemRoleRequest({
              systemRoleTemplates: [systemRoleId],
              localizedData: systemRoleInfo.localizedData
            }),
            true
          );
        })
      );
  }

  private flattenPermissionRolesArray(
    permissionRolesArray: AccessRightRoles[]
  ): any[] {
    const flattenedPermissionRolesArray: any = [];

    permissionRolesArray.forEach((permissionRoles) => {
      const flattenedPermissionRoles: any = {};
      // tslint:disable-next-line:no-string-literal
      flattenedPermissionRoles['accessRight'] = permissionRoles.accessRight;

      flattenedPermissionRoles.accessRightId = permissionRoles.accessRightId;

      permissionRoles.roles.forEach((role) => {
        flattenedPermissionRoles[role.label] = role.value;
      });

      flattenedPermissionRolesArray.push(flattenedPermissionRoles);
    });

    return flattenedPermissionRolesArray;
  }

  private generateAccessRightsFollowedSystemRoles(
    accessRightsMatrixModel: AccessRightsMatrixModel
  ): AccessRightRoles[] {
    const accessRightsDic = Utils.toDictionary(
      accessRightsMatrixModel.accessRights,
      (right) => right.id
    );

    const grantedAccessRightDic = Utils.toDictionary(
      accessRightsMatrixModel.grantedAccessRights,
      (grantedAccessRight) =>
        `${grantedAccessRight.accessRightId}-${grantedAccessRight.systemRoleId}`
    );

    const permissionRolesArray: AccessRightRoles[] = [];
    accessRightsMatrixModel.accessRights.forEach((accessRight) => {
      const permissionRoles: AccessRightRoles = new AccessRightRoles();
      permissionRoles.accessRight =
        accessRightsDic[
          accessRight.id
        ].localizedData[0].fields[0].localizedText;

      permissionRoles.accessRightId = accessRight.id;

      accessRightsMatrixModel.systemRoles.forEach((sysRole) => {
        permissionRoles.roles.push({
          label: sysRole.id.toString(),
          value:
            grantedAccessRightDic[`${accessRight.id}-${sysRole.id}`]
              .grantedType === GrantedType.Allow
        });
      });

      permissionRolesArray.push(permissionRoles);
    });

    return permissionRolesArray;
  }
}
