import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LocalizedDataItem } from 'app-models/localized-data-item.model';
import { AssignableSystemRole } from 'app/core/dtos/assignable-system-role.dto';
import { CreateSystemRoleRequest } from 'app/core/dtos/create-system-role-request.dto';
import { GetSystemRoleByIdRequest } from 'app/core/dtos/get-system-role-by-id-request.dto';
import { GetSystemRolesWithPermissionsRequest } from 'app/core/dtos/get-system-roles-with-permissions-request.dto';
import { SystemRoleInfo } from 'app/core/dtos/system-role-info.dto';
import { UpdateSystemRoleInfoRequest } from 'app/core/dtos/update-system-role-info-request.dto';
import {
  Granted,
  SystemRolePermissionSubject
} from 'app/core/models/system-role-permission-subject.model';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { PermissionsTableComponent } from 'app/permissions/permissions-table/permissions-table.component';
import { PermissionsApiService } from 'app/permissions/services/permissions-api.service';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { Subscription } from 'rxjs';
import { NumericDictionary, Utils } from '../../../shared/utilities/utils';

@Component({
  selector: 'system-role-dialog',
  templateUrl: './system-role-dialog.component.html',
  styleUrls: ['./system-role-dialog.component.scss']
})
export class SystemRoleDialogComponent implements OnInit, OnDestroy {
  isEditMode: boolean = false;
  systemRoleName: string = '';
  systemRoleOriginName: string;
  assignableRoleIds: number[] = [87, 88, 89, 90, 112, 145, 142];
  selectedSystemRoleInfo: SystemRoleInfo = new SystemRoleInfo();
  readonly specialCharsRegExp: RegExp = /[^A-Za-z0-9\s]/;
  assignableSystemRoles: AssignableSystemRole[] = [];
  dialogTitle: string = 'Create new role';
  isEmptyString: boolean = false;
  isSpecialChars: boolean = false;
  isDuplicatedSystemRoleName: boolean = false;

  private _existedSystemRoleNames: string[] = [];
  private _dialogSubscription: Subscription = new Subscription();

  constructor(
    public dialogRef: MatDialogRef<PermissionsTableComponent>,
    public permissionsApiService: PermissionsApiService,
    public systemRolesDataService: SystemRolesDataService,
    private ngbModal: NgbModal,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    @Inject(MAT_DIALOG_DATA) public systemRoleInfo: SystemRoleInfo
  ) {
    this.getExistedSystemRolesName();
    if (!systemRoleInfo) {
      return;
    }

    this.selectedSystemRoleInfo = systemRoleInfo;
  }
  ngOnDestroy(): void {
    this._dialogSubscription.unsubscribe();
  }

  ngOnInit(): void {
    if (this.selectedSystemRoleInfo.isEditMode) {
      this.getSystemRolesWithPermissions(true);

      return;
    }
    this.getSystemRolesWithPermissions();
  }

  onSelectedValueChanged(assignableSystemRole: AssignableSystemRole): void {
    const indexAssignableSystemRole = this.assignableSystemRoles.findIndex(
      (existedAssignableSystemRoles) =>
        existedAssignableSystemRoles.id === assignableSystemRole.id
    );

    if (indexAssignableSystemRole === findIndexCommon.notFound) {
      return;
    }

    this.assignableSystemRoles[indexAssignableSystemRole].granted =
      assignableSystemRole.granted;
  }

  get numberOfSelectedRoles(): number {
    return this.assignableSystemRoles.filter(
      (role) => role.granted !== Granted.Deny
    ).length;
  }

  get opalTextAreaStyle(): unknown {
    return {
      border: '1x solid #d8dce6',
      boxSizing: 'border-box',
      borderRadius: '5px',
      width: '100%',
      padding: '10px'
    };
  }

  onDeleteRoleClicked(): void {
    this.showConfirmationDialog(
      () => {
        this.dialogRef.close({
          isDeleted: true,
          roleName: this.systemRoleName
        });
      },
      'Delete',
      'Are you sure you want to delete this role?'
    );
  }

  onConfirmClicked(): void {
    if (!this.isValidSystemRoleTile) {
      return;
    }

    if (this.selectedSystemRoleInfo.isEditMode) {
      const updateSystemRoleInfoRequest = new UpdateSystemRoleInfoRequest();
      updateSystemRoleInfoRequest.systemRolePermissionSubjects = [];
      updateSystemRoleInfoRequest.localizedData = [];

      this.assignableSystemRoles.forEach((assignableSystemRole) => {
        updateSystemRoleInfoRequest.systemRolePermissionSubjects.push({
          id: assignableSystemRole.id,
          granted: assignableSystemRole.granted
        });
      });

      updateSystemRoleInfoRequest.id = this.selectedSystemRoleInfo.id;
      updateSystemRoleInfoRequest.localizedData.push(
        this.buildSystemRoleName()
      );

      this.dialogRef.close({
        updateSystemRoleInfoRequestDto: updateSystemRoleInfoRequest
      });

      return;
    }
    const createSystemRoleRequest = new CreateSystemRoleRequest();
    createSystemRoleRequest.systemRolePermissionSubjects = [];
    createSystemRoleRequest.localizedData = [];

    this.assignableSystemRoles.forEach((assignableSystemRole) => {
      createSystemRoleRequest.systemRolePermissionSubjects.push({
        id: assignableSystemRole.id,
        granted: assignableSystemRole.granted
      });
    });

    createSystemRoleRequest.localizedData.push(this.buildSystemRoleName());

    this.dialogRef.close({
      createSystemRoleRequestDto: createSystemRoleRequest
    });
  }

  onClose(): void {
    this.dialogRef.close();
  }

  private getSystemRolesWithPermissions(
    isGetPermissionById: boolean = false
  ): void {
    this.cxGlobalLoaderService.showLoader();
    this._dialogSubscription.add(
      this.systemRolesDataService
        .getSystemRolesWithPermissions(
          new GetSystemRolesWithPermissionsRequest({
            systemRoleIds: this.assignableRoleIds,
            includeLocalizedData: true,
            includeSystemRolePermissionSubjects: true
          })
        )
        .subscribe(
          (systemRoleSubjects) => {
            systemRoleSubjects.forEach((systemRoleSubject) => {
              this.assignableSystemRoles.push(
                new AssignableSystemRole({
                  name:
                    systemRoleSubject.localizedData[0].fields[0].localizedText,
                  id: systemRoleSubject.id
                })
              );
            });

            if (isGetPermissionById) {
              this.getSystemRolesById(this.selectedSystemRoleInfo.id);

              return;
            }
          },
          (err) => {},
          () => {
            if (!isGetPermissionById) {
              this.cxGlobalLoaderService.hideLoader();
            }
          }
        )
    );
  }

  private getSystemRolesById(systemRoleId: number): void {
    this._dialogSubscription.add(
      this.systemRolesDataService
        .getSystemRoleById(
          new GetSystemRoleByIdRequest({
            id: systemRoleId,
            includeLocalizedData: true,
            includeSystemRolePermissionSubjects: true
          })
        )
        .subscribe(
          (systemRoleSubjects) => {
            this.systemRoleName =
              systemRoleSubjects.localizedData[0].fields[0].localizedText;
            this.systemRoleOriginName = this.systemRoleName;

            this.dialogTitle = `Edit ${this.systemRoleName}`;

            const existedSystemRolePermissionSubjectsDic: NumericDictionary<SystemRolePermissionSubject> = Utils.toDictionary(
              systemRoleSubjects.systemRolePermissionSubjects,
              (role) => role.id
            );
            this.assignableSystemRoles.forEach((assignableSystemRole) => {
              assignableSystemRole.granted =
                existedSystemRolePermissionSubjectsDic[
                  assignableSystemRole.id
                ].granted;
            });
          },
          (error) => {},
          () => {
            this.cxGlobalLoaderService.hideLoader();
          }
        )
    );
  }

  private buildSystemRoleName(): LocalizedDataItem {
    return {
      languageCode: 'en-US',
      id: 2,
      fields: [
        {
          localizedText: this.systemRoleName,
          name: 'Name'
        },
        {
          localizedText: this.systemRoleName,
          name: 'Description'
        }
      ]
    };
  }

  private getExistedSystemRolesName(): void {
    this._dialogSubscription.add(
      this.systemRolesDataService
        .getSystemRolesWithPermissions(
          new GetSystemRolesWithPermissionsRequest({
            includeLocalizedData: true
          })
        )
        .subscribe((systemRoles) => {
          systemRoles.forEach((role) => {
            this._existedSystemRoleNames.push(
              role.localizedData[0].fields[0].localizedText
            );
          });
        })
    );
  }

  private showConfirmationDialog(
    onConfirm: () => void,
    headerName: string,
    content: string,
    cancelButtonText: string = 'Cancel'
  ): void {
    const cxConfirmationDialogModalRef = this.ngbModal.open(
      CxConfirmationDialogComponent,
      {
        size: 'sm',
        centered: true
      }
    );

    const cxConfirmationDialogModal = cxConfirmationDialogModalRef.componentInstance as CxConfirmationDialogComponent;
    cxConfirmationDialogModal.showConfirmButton = true;
    cxConfirmationDialogModal.showCloseButton = true;
    cxConfirmationDialogModal.confirmButtonText = 'Yes';
    cxConfirmationDialogModal.cancelButtonText = cancelButtonText;
    cxConfirmationDialogModal.header = headerName;
    cxConfirmationDialogModal.content = content;

    cxConfirmationDialogModal.confirm.subscribe(() => {
      onConfirm();
      cxConfirmationDialogModalRef.close();
    });
    cxConfirmationDialogModal.cancel.subscribe(() => {
      cxConfirmationDialogModalRef.close();
    });
  }

  private get isValidSystemRoleTile(): boolean {
    this.isEmptyString = this.checkEmptyString();

    this.isSpecialChars = this.checkSpecialChars();

    this.isDuplicatedSystemRoleName = this.checkExistedSystemRoleName();

    if (
      this.isEmptyString ||
      this.isSpecialChars ||
      this.isDuplicatedSystemRoleName
    ) {
      return false;
    }

    return true;
  }

  private checkEmptyString(): boolean {
    return !this.systemRoleName.trim().length;
  }

  private checkSpecialChars(): boolean {
    return this.specialCharsRegExp.test(this.systemRoleName);
  }

  private checkExistedSystemRoleName(): boolean {
    const newRoleName = this.systemRoleName.trim();

    if (
      this.selectedSystemRoleInfo.isEditMode &&
      newRoleName === this.systemRoleOriginName
    ) {
      return false;
    }

    return this._existedSystemRoleNames.some(
      (existedRoleName) => existedRoleName === newRoleName
    );
  }
}
