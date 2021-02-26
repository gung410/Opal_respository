import { Injectable } from '@angular/core';
import { CreateSystemRoleRequest } from 'app/core/dtos/create-system-role-request.dto';
import { CreateSystemRoleResponse } from 'app/core/dtos/create-system-role-response.dto';
import { GetSystemRoleByIdRequest } from 'app/core/dtos/get-system-role-by-id-request.dto';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';
import { Utils } from '../../shared/utilities/utils';
import { ActionSignal } from '../dtos/action-signal.dto';
import { EditingSignal } from '../dtos/editing-signal.dto';
import { HeaderComparison } from '../dtos/header-comparison.dto';
import { AccessRightsRequest } from '../dtos/request-dtos/access-rights-request';
import { GrantedType } from '../enum/granted-type.enum';
import { ModuleType } from '../enum/module-type.enum';
import { ObjectType } from '../enum/object-type.enum';
import { AccessRightsLevel } from '../models/access-rights-level.model';
import { AccessRightsMatrixModel } from '../models/access-rights-matrix.model';
import { AccessRightsModel } from '../models/access-rights.model';
import { AccessRightRoles } from '../models/permissionRoles.model';
import { AccessRightsViewModel } from '../viewmodels/acessrights.viewmodel';
import { AccessRightsMatrixModelRequest } from './../dtos/request-dtos/access-rights-matrix-request';
import { PermissionsApiService } from './permissions-api.service';

@Injectable()
export class PermissionsTableService {
  editingSignal$: Observable<EditingSignal>; // this is editing for the whole column signal
  actionSignal$: Observable<ActionSignal>;
  moduleSelectionSignal$: BehaviorSubject<number> = new BehaviorSubject<number>(
    0
  );
  headerName$: BehaviorSubject<HeaderComparison> = new BehaviorSubject<HeaderComparison>(
    null
  );
  lastRowRender$: Observable<boolean>;
  isChangeTable: boolean = false;
  moduleItems: AccessRightsViewModel[];
  communitySiteModuleId: number;

  get currentModuleId(): number {
    return this._currentModuleId;
  }
  set currentModuleId(newModuleId: number) {
    if (newModuleId == null) {
      return;
    }
    this._currentModuleId = newModuleId;
    this.moduleSelectionSignal$.next(newModuleId);
  }

  get lastRowId(): number {
    return this._lastRowId;
  }
  set lastRowId(lastRowId: number) {
    if (Utils.isDifferent(this._lastRowId, lastRowId)) {
      this.isChangeTable = true;
    }
    this._lastRowId = lastRowId;
  }
  private readonly _defaultLasRowId: number = -99;
  private _lastRowId: number = this._defaultLasRowId;
  private _currentModuleId: number;

  private editingSubject: Subject<EditingSignal> = new Subject<EditingSignal>();
  private actionSubject: Subject<ActionSignal> = new Subject<ActionSignal>();
  private lastRowRenderSubject: Subject<boolean> = new Subject<boolean>();

  constructor(
    private permissionsApiSvc: PermissionsApiService,
    private systemRolesDataService: SystemRolesDataService
  ) {
    this.editingSignal$ = this.editingSubject.asObservable();
    this.actionSignal$ = this.actionSubject.asObservable();
    this.lastRowRender$ = this.lastRowRenderSubject.asObservable();
  }

  sendEditingSignal(editingSignal: EditingSignal): void {
    this.editingSubject.next(editingSignal);
  }

  sendActionSignal(actionSignal: ActionSignal): void {
    this.actionSubject.next(actionSignal);
  }

  sendLastRowRenderSignal(rowId: number): void {
    if (rowId === this.lastRowId && this.isChangeTable) {
      this.isChangeTable = false;
      this.lastRowRenderSubject.next(true);
    }
  }

  resetTableService(): void {
    this.lastRowId = this._defaultLasRowId;
  }

  isHeaderNameDifference(): boolean {
    const headerCompare = new HeaderComparison(this.headerName$.getValue());

    return headerCompare.isDifferent();
  }

  getDefaultModuleId(): Promise<number> {
    return this.permissionsApiSvc
      .getAccessRights(
        new AccessRightsRequest({
          objectTypes: [ObjectType.Module],
          includeLocalizedData: true
        })
      )
      .pipe(
        map((modules) => {
          const filteredModules = modules
            .filter(
              (module) => module.module !== ModuleType.BatchJobsMonitoring
            )
            .filter(
              (module) => module.module !== ModuleType.ReportingAndAnalytics
            );

          return filteredModules.map(
            (module) => new AccessRightsViewModel(module)
          );
        }),
        tap((modules) => {
          this.moduleItems = modules;
        }),
        map((modules) => {
          this.getCommunitySiteId(modules);
          const organizationSpaModule = modules.find(
            (module) => module.module === ModuleType.OrganizationSpa
          );
          if (organizationSpaModule) {
            this.currentModuleId = organizationSpaModule.id;

            return organizationSpaModule.id;
          }

          this.currentModuleId = modules[0].id;

          return modules[0].id;
        })
      )
      .toPromise();
  }

  generateRowData(accessRightsMatrixModel: AccessRightsMatrixModel): any[] {
    const systemRolesFollowedAccessRights = this.generateAccessRightsFollowedSystemRoles(
      accessRightsMatrixModel
    );

    const flattenPermissionRolesArrayResult = this.flattenPermissionRolesArray(
      systemRolesFollowedAccessRights
    );

    const lastAccessRight =
      flattenPermissionRolesArrayResult[
        flattenPermissionRolesArrayResult.length - 1
      ];
    if (lastAccessRight && lastAccessRight.accessRightId) {
      this.lastRowId = lastAccessRight.accessRightId;
    }

    return flattenPermissionRolesArrayResult;
  }

  getAccessRightsMatrixByModuleId(
    accessRightsMatrixModelRequest: AccessRightsMatrixModelRequest
  ): Observable<AccessRightsMatrixModel> {
    if (
      accessRightsMatrixModelRequest.parentAccessRightIds[0] ===
      this.communitySiteModuleId
    ) {
      return this.permissionsApiSvc.getAccessRightsMatrixOfCSL();
    }

    return this.permissionsApiSvc.getAccessRightsMatrix(
      accessRightsMatrixModelRequest
    );
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

  private getCommunitySiteId(modules: AccessRightsViewModel[]): void {
    const communitySiteModule = modules.find(
      (module) => module.module === ModuleType.CommunitySite
    );
    if (communitySiteModule) {
      this.communitySiteModuleId = communitySiteModule.id;
    }
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

      flattenedPermissionRoles['isHideAccessRight'] =
        permissionRoles.isHideAccessRight;

      flattenedPermissionRoles['level'] = permissionRoles.level;

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

      permissionRoles.isHideAccessRight = accessRight.hideConfiguration;

      // if (accessRight instanceof AccessRightsModel) {
      permissionRoles.level = (accessRight as AccessRightsLevel).level;
      // }

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
