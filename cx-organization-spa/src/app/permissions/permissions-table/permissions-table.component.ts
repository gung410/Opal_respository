import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService
} from '@conexus/cx-angular-common';
import { Dictionary } from '@conexus/cx-angular-common/typings';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ColDef } from 'ag-grid-community';
import { ShowHideColumnModel } from 'app-models/ag-grid-column.model';
import {
  AgGridConfigModel,
  ColumDefModel
} from 'app-models/ag-grid-config.model';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { Utils } from 'app-utilities/utils';
import { CreateSystemRoleRequest } from 'app/core/dtos/create-system-role-request.dto';
import { SystemRoleInfo } from 'app/core/dtos/system-role-info.dto';
import { UpdateSystemRoleInfoRequest } from 'app/core/dtos/update-system-role-info-request.dto';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { CommonHelpers } from 'app/shared/common.helpers';
import { BasePresentationComponent } from 'app/shared/components/component.abstract';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { SystemRoleDialogComponent } from '../dialogs/system-role-dialog/system-role-dialog.component';
import { SystemRolesShowHideComponent } from '../dialogs/system-roles-show-hide-column/system-roles-show-hide-column.component';
import { ActionSignal } from '../dtos/action-signal.dto';
import { EditingSignal } from '../dtos/editing-signal.dto';
import { AccessRightsMatrixModelRequest } from '../dtos/request-dtos/access-rights-matrix-request';
import { ObjectType } from '../enum/object-type.enum';
import { SystemRoleAction } from '../enum/permission-action.enum';
import { AccessRightMatrixHelper } from '../helpers/access-right-matrix-helper';
import { AccessRightsModel } from '../models/access-rights.model';
import { ColumnItemModel } from '../models/column-item.model';
import { GrantedAccessRightsModel } from '../models/granted-access-rights.model';
import { SystemRoleHeaderExtraAttributesModel } from '../models/system-role-header-extra-attributes.model';
import { SystemRoleModel } from '../models/system-role.model';
import { IUpdateAccessRightsRequest } from '../models/update-accessRights-request.model';
import { CheckboxRendererComponent } from '../renderers/checkbox-renderer/checkbox-renderer.component';
import { DropdownListModulesHeaderComponent } from '../renderers/module-header/dropdown-list-modules-header.component';
import { SystemRoleHeaderComponent } from '../renderers/system-role-header/system-role-header.component';
import { PermissionsApiService } from '../services/permissions-api.service';
import { PermissionsColumnService } from '../services/permissions-column.service';
import { PermissionsTableService } from '../services/permissions-table.service';
import { AccessRightsMatrixModel } from './../models/access-rights-matrix.model';

@Component({
  selector: 'permissions-table',
  templateUrl: './permissions-table.component.html',
  styleUrls: ['./permissions-table.component.scss']
})
export class PermissionsTableComponent
  extends BasePresentationComponent
  implements OnInit, OnDestroy {
  agGridConfig: AgGridConfigModel;
  accessRightsMatrixData: AccessRightsMatrixModel;
  accessRightsDic: Dictionary<AccessRightsModel>;
  permissionDic: Dictionary<SystemRoleModel>;
  grantedAccessRightDic: Dictionary<GrantedAccessRightsModel>;
  isEditing: boolean = false;
  originSystemRoleListColumnDef: ColumDefModel[];
  showHideColumns: ShowHideColumnModel;
  systemRoleListColumnDef: ColumDefModel[];
  defaultLetterSize: number = 7.5;
  leftAndRightMarginSize: number = 30;

  get currentGrantedAccessRights(): GrantedAccessRightsModel[] {
    return this._currentGrantedAccessRights;
  }
  set currentGrantedAccessRights(
    newGrantedAccessRights: GrantedAccessRightsModel[]
  ) {
    if (newGrantedAccessRights == null) {
      return;
    }

    this._currentGrantedAccessRights = Utils.cloneDeep(newGrantedAccessRights);
  }

  private readonly DEFAULT_MODULE_ID: number = 5; // System Admin Module ID
  private readonly FORCE_TIME: number = 30000;
  private _currentModuleId: number = this.DEFAULT_MODULE_ID;
  private _selectedSystemRoleId: number | string | null = null;
  private _selectedSystemRoleName: string | null = null;
  private columnDataChangeSubscription: Subscription;
  private _currentGrantedAccessRights: GrantedAccessRightsModel[] = [];
  private _permissionTableSubscription: Subscription = new Subscription();
  private _lastRowRenderSubscription: Subscription;

  constructor(
    private translateAdapterService: TranslateAdapterService,
    changeDetectorRef: ChangeDetectorRef,
    private ngbModal: NgbModal,
    private toastrService: ToastrService,
    private dialog: MatDialog,
    private permissionsApiSvc: PermissionsApiService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private systemRolesDataService: SystemRolesDataService,
    private permissionsTableSvc: PermissionsTableService,
    private permissionsColumnSvc: PermissionsColumnService<
      ColumnItemModel<GrantedAccessRightsModel>
    >
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    this.getPermissionMatrixByModuleId(true);
    this.addScrollEventListener();
  }

  initGridConfig(): void {
    this.agGridConfig = new AgGridConfigModel({
      columnDefs: this.setColumnDef(),
      frameworkComponents: {
        checkboxRenderer: CheckboxRendererComponent
      },
      rowData: [],
      selectedRows: [],
      context: { componentParent: this },
      defaultColDef: {
        flex: 1
      }
    });
    this.buildRowClassRules();
  }
  initGridData(): void {
    this.agGridConfig.rowData = this.permissionsTableSvc.generateRowData(
      this.accessRightsMatrixData
    );
    this.changeDetectorRef.detectChanges();
  }

  onGridReady(params: any): void {
    if (!params) {
      return;
    }
    this.agGridConfig.gridApi = params.api;
    this.agGridConfig.gridColumnApi = params.columnApi;
    this.agGridConfig.gridApi.setDomLayout('autoHeight');
    this.agGridConfig.gridApi.sizeColumnsToFit();
    if (
      this.agGridConfig.gridColumnApi &&
      this.agGridConfig.gridColumnApi.columnController
    ) {
      this.initGridColumnShowHideForAgGridConfig();
    }
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  onUpdateSystemRole(systemRoleId: number): void {
    const updateAccessRightsRequest = this.generateUpdateAccessRightRequest(
      systemRoleId
    );

    // Call service to send request;
  }

  onSystemRoleDialogClicked(): void {
    const dialogRef = this.dialog.open(SystemRoleDialogComponent, {
      width: '930px',
      minHeight: '600px'
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (!(result && result.createSystemRoleRequestDto)) {
        return;
      }

      // tslint:disable-next-line: no-unsafe-any
      const createSystemRoleRequest: CreateSystemRoleRequest = Utils.cloneDeep(
        result.createSystemRoleRequestDto
      );

      this._permissionTableSubscription.add(
        this.systemRolesDataService
          .createSystemRole(createSystemRoleRequest)
          .subscribe(
            (response) => {
              this.toastrService.success(
                `${createSystemRoleRequest.localizedData[0].fields[0].localizedText} was successfully created`
              );
              this.getPermissionMatrixByModuleId(true, this._currentModuleId);
            },
            (err) => {
              this.toastrService.error(
                'Something bad happened. Please try again later'
              );
            }
          )
      );
    });
  }

  openDialogToEnableColumns(): void {
    const modalRef = this.ngbModal.open(SystemRolesShowHideComponent, {
      size: 'sm',
      backdrop: 'static',
      centered: true
    });
    const instanceComponent = modalRef.componentInstance as SystemRolesShowHideComponent;
    const clonedOriginSystemRoleListColumnDef = Utils.cloneDeep(
      this.originSystemRoleListColumnDef
    );
    clonedOriginSystemRoleListColumnDef.splice(0, 2);
    const showHideColumns = clonedOriginSystemRoleListColumnDef;

    showHideColumns.forEach((showHideColumn) => {
      const hiddenColumn = this.systemRoleListColumnDef.find(
        (col) => col.colId === showHideColumn.colId
      );
      if (hiddenColumn) {
        showHideColumn.hide = hiddenColumn.hide;
      }
    });
    instanceComponent.systemRoleListColumnDef = showHideColumns;

    this._permissionTableSubscription.add(
      instanceComponent.changeShowHideColumn.subscribe((observe) => {
        this.changeSelectedColumn(observe.$event, observe.columnNeedToHide);
      })
    );
    this._permissionTableSubscription.add(
      instanceComponent.cancel.subscribe(() => {
        modalRef.close();
      })
    );
  }

  changeSelectedColumn(selected: any, column: any): void {
    if (column && column.colId) {
      this.showHideColumns = new ShowHideColumnModel({
        column,
        selected
      });
      const columnNeedToHide = this.systemRoleListColumnDef.find(
        (col) => col.colId === column.colId
      );
      if (columnNeedToHide) {
        columnNeedToHide.hide = !selected;
        this.updateColumns(this.showHideColumns);
      }
    }
  }

  ngOnDestroy(): void {
    this.refreshPermissionsColumnService();
    this._permissionTableSubscription.unsubscribe();
    if (this._lastRowRenderSubscription) {
      this._lastRowRenderSubscription.unsubscribe();
    }
  }

  onColumnShowHideDef(systemRoleListColumnDef: ColumDefModel[]): void {
    this.originSystemRoleListColumnDef = systemRoleListColumnDef;
    const clonedOriginSystemRoleListColumnDef = Utils.cloneDeep(
      this.originSystemRoleListColumnDef
    );
    clonedOriginSystemRoleListColumnDef.splice(0, 2);
    this.systemRoleListColumnDef = clonedOriginSystemRoleListColumnDef;
  }

  updateColumns(showHideColumn: ShowHideColumnModel): void {
    if (!showHideColumn) {
      return;
    }

    this.agGridConfig.gridColumnApi.setColumnVisible(
      this.showHideColumns.column.colId,
      this.showHideColumns.selected
    );
    this.agGridConfig.gridApi.sizeColumnsToFit();
    this.changeDetectorRef.detectChanges();
  }

  sizeColumnToFit(): void {
    this.agGridConfig.gridApi.sizeColumnsToFit();
  }

  private initGridColumnShowHideForAgGridConfig(): void {
    if (
      this.agGridConfig.columnShowHide &&
      this.agGridConfig.columnShowHide.length <= 0
    ) {
      this.agGridConfig.columnShowHide = this.agGridConfig.gridColumnApi.columnController.columnDefs;
      this.onColumnShowHideDef(this.agGridConfig.columnShowHide);
    }
  }

  private addScrollEventListener(): void {
    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
    window.addEventListener('scroll', CommonHelpers.freezeAgGridScroll(), true);
  }

  private refreshPermissionsColumnService(): void {
    this.permissionsColumnSvc.refreshColumn();
    if (this.columnDataChangeSubscription) {
      this.columnDataChangeSubscription.unsubscribe();
    }
  }

  private generateUpdateAccessRightRequest(
    systemRoleId: number
  ): IUpdateAccessRightsRequest {
    const accessRights = this.accessRightsMatrixData.grantedAccessRights.filter(
      (grantedAccessRight) => grantedAccessRight.systemRoleId === systemRoleId
    );

    // tslint:disable-next-line:no-angle-bracket-type-assertion
    const updateAccessRightsRequest = <IUpdateAccessRightsRequest>{
      systemRoleId,
      accessRights: []
    };

    accessRights.forEach((accessRight) => {
      const updatedAccessRight = Utils.clone(
        this.accessRightsDic[accessRight.accessRightId],
        (clonedAccessRight) => {
          clonedAccessRight.grantedType = this.grantedAccessRightDic[
            `${accessRight.accessRightId}-${systemRoleId}`
          ].grantedType;
        }
      );

      updateAccessRightsRequest.accessRights.push(updatedAccessRight);
    });

    return updateAccessRightsRequest;
  }

  private getPermissionMatrixByModuleId(
    isFirstInit: boolean,
    moduleId: number = this.DEFAULT_MODULE_ID
  ): void {
    this.cxGlobalLoaderService.showLoader();
    this._permissionTableSubscription.add(
      this.permissionsApiSvc
        .getAccessRightsMatrix(
          new AccessRightsMatrixModelRequest({
            objectTypes: [ObjectType.NormalMenuItem, ObjectType.PageAction],
            parentAccessRightIds: [moduleId],
            includeChildren: true,
            includeAccessRights: true,
            includeSystemRoles: true,
            includeLocalizedData: true
          })
        )
        .subscribe((matrix) => {
          this.accessRightsMatrixData = matrix;

          this.permissionDic = Utils.toDictionary(
            this.accessRightsMatrixData.systemRoles,
            (role) => role.id.toString()
          );

          this.accessRightsDic = Utils.toDictionary(
            this.accessRightsMatrixData.accessRights,
            (right) => right.id
          );

          this.grantedAccessRightDic = Utils.toDictionary(
            this.accessRightsMatrixData.grantedAccessRights,
            (grantedAccessRight) =>
              `${grantedAccessRight.accessRightId}-${grantedAccessRight.systemRoleId}`
          );

          if (isFirstInit) {
            this.permissionsTableSvc.resetTableService();
            this.initGridConfig();
            this.initGridData();
            this.hideLoader();

            return;
          }
          this.refreshGridData();
        })
    );
  }

  private hideLoader(): void {
    if (this._lastRowRenderSubscription) {
      this._lastRowRenderSubscription.unsubscribe();
    }
    let isForceHideLoader: boolean = true;
    this._lastRowRenderSubscription = this.permissionsTableSvc.lastRowRender$.subscribe(
      (isLastRow) => {
        if (isLastRow) {
          isForceHideLoader = false;
          this.cxGlobalLoaderService.hideLoader();
        }
      }
    );

    setTimeout(() => {
      if (isForceHideLoader) {
        this.cxGlobalLoaderService.hideLoader();
      }
    }, this.FORCE_TIME);
  }

  // private printAccessRights(accessRights: AccessRightsModel[]): void {
  //   accessRights.forEach((r) => {
  //     console.log(
  //       '---------------------------------------------------------------------'
  //     );
  //     console.log('Id: ' + r.id);
  //     console.log('Name: ' + r.localizedData[0].fields[0].localizedText);
  //     console.log('Type: ' + r.objectType);
  //     console.log('No: ' + r.no);
  //     console.log('Parent id: ' + r.parentId);
  //   });
  // }

  private refreshGridData(): void {
    this.permissionsColumnSvc.refreshColumn();
    if (this._selectedSystemRoleId) {
      this.permissionsTableSvc.sendEditingSignal(
        new EditingSignal({
          isEditing: false,
          systemRoleId: this._selectedSystemRoleId.toString()
        })
      );
    }
    this.agGridConfig.rowData = this.permissionsTableSvc.generateRowData(
      this.accessRightsMatrixData
    );

    this.agGridConfig.gridApi.sizeColumnsToFit();

    this.hideLoader();
    // this.agGridConfig.gridApi.refreshCells({ force: true });
  }

  private buildRowClassRules(): void {}

  private setColumnDef(): ColDef[] {
    if (!this.accessRightsMatrixData) {
      return [];
    }

    const matrixCols: ColDef[] = [];
    const permissionCol: ColDef[] = this.buildPermissionCol();
    matrixCols.push(...permissionCol);

    this.accessRightsMatrixData.systemRoles.forEach((systemRole) => {
      const systemRoleCol: ColDef = {};

      systemRoleCol.headerName =
        systemRole.localizedData[0].fields[0].localizedText;

      systemRoleCol.field = systemRole.id.toString();

      systemRoleCol.colId = systemRole.id.toString();

      systemRoleCol.cellRenderer = 'checkboxRenderer';

      systemRoleCol.resizable = false;

      systemRoleCol.minWidth =
        this.defaultLetterSize * systemRoleCol.headerName.length +
        this.leftAndRightMarginSize;

      systemRoleCol.cellClassRules = {
        // tslint:disable-next-line:object-literal-shorthand
        // 'editing-column-color': (params): boolean => {
        //   return this.isEditing === true;
        // }
      };

      systemRoleCol.headerComponentFramework = SystemRoleHeaderComponent;

      systemRoleCol.headerComponentParams = {
        onClick: this.onPermissionActionsClicked.bind(this),
        extraAttributes: new SystemRoleHeaderExtraAttributesModel({
          isDefaultSystemRole: systemRole.isDefaultSystemRole
        })
      };

      matrixCols.push(systemRoleCol);
    });

    return matrixCols;
  }

  private buildPermissionCol(): ColDef[] {
    return [
      {
        field: 'accessRightId',
        hide: true
      },
      {
        headerName: 'Select Module',
        field: 'accessRight',
        headerComponentFramework: DropdownListModulesHeaderComponent,
        headerComponentParams: {
          onClick: this.onModuleSelectionClicked.bind(this)
        },
        minWidth: 400,
        cellClassRules: {
          // 'first-level': (params): boolean => {
          //   return params.data.level === ?;
          // }
        },
        pinned: true,
        autoHeight: true,
        resizable: true
      }
    ];
  }

  private onModuleSelectionClicked(moduleId: number): void {
    if (!moduleId) {
      return;
    }
    // Call Api to reload the permission table based on new moduleId
    this._currentModuleId = moduleId;
    this.getPermissionMatrixByModuleId(false, moduleId);
  }

  private onPermissionActionsClicked(
    systemRoleAction: SystemRoleAction,
    systemRoleId: number | string,
    systemRoleName?: string
  ): void {
    this._selectedSystemRoleId = systemRoleId;
    if (systemRoleName) {
      this._selectedSystemRoleName = systemRoleName;
    }
    switch (systemRoleAction) {
      case SystemRoleAction.Edit:
        this.processEditSystemRole();
        break;
      case SystemRoleAction.Save:
        this.processSaveSystemRole();
        break;
      case SystemRoleAction.Clone:
        this.processCloneSystemRole();
        break;
      case SystemRoleAction.Cancel:
        this.processCancelSystemRole();
        break;
      case SystemRoleAction.EditRole:
        this.processEditRole();
        break;
      default:
        break;
    }
  }

  private processEditSystemRole(): void {
    this.columnDataChangeSubscription = this.permissionsColumnSvc.columnDataChange$.subscribe(
      (cols: GrantedAccessRightsModel[]) => {
        this.currentGrantedAccessRights = cols;
        // console.log(cols);
      }
    );
  }

  private processEditRole(): void {
    const systemRoleInfo = new SystemRoleInfo({
      id: Number(this._selectedSystemRoleId),
      isEditMode: true,
      name: ''
    });
    const dialogRef = this.dialog.open(SystemRoleDialogComponent, {
      width: '930px',
      minHeight: '600px',
      data: systemRoleInfo
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result && result.isDeleted) {
        this.systemRolesDataService
          .deleteSystemRole(this._selectedSystemRoleId)
          .subscribe((_) => {
            this.toastrService.success(
              `${result.roleName} was successfully deleted.`
            );

            this.refreshPermissionsColumnService();
            this.destroyAgGridConfig();
            this.getPermissionMatrixByModuleId(true, this._currentModuleId);
          });

        return;
      }

      if (!(result && result.updateSystemRoleInfoRequestDto)) {
        return;
      }

      // tslint:disable-next-line: no-unsafe-any
      const updateSystemRoleInfoRequest: UpdateSystemRoleInfoRequest = Utils.cloneDeep(
        result.updateSystemRoleInfoRequestDto
      );

      this._permissionTableSubscription.add(
        this.systemRolesDataService
          .updateSystemRole(updateSystemRoleInfoRequest)
          .subscribe((response) => {
            this.toastrService.success(
              `${updateSystemRoleInfoRequest.localizedData[0].fields[0].localizedText} was successfully updated.`
            );
            this.destroyAgGridConfig();
            this.getPermissionMatrixByModuleId(true, this._currentModuleId);
          })
      );
    });
  }

  private destroyAgGridConfig(): void {
    delete this.agGridConfig;
  }

  private processSaveSystemRole(): void {
    this.showConfirmationDialog(
      () => {
        const updatedMatrix = AccessRightMatrixHelper.updateAccessRightsMatrix(
          this.accessRightsMatrixData,
          this.currentGrantedAccessRights
        );

        let updateAccessRightPayload = AccessRightMatrixHelper.generateAccessRightsBySystemRoleId(
          updatedMatrix,
          Number(this.currentGrantedAccessRights[0].systemRoleId)
        );

        updateAccessRightPayload = AccessRightMatrixHelper.addModuleInfo(
          this._currentModuleId,
          updateAccessRightPayload
        );

        this._permissionTableSubscription.add(
          this.permissionsApiSvc
            .updateAccessRightsBySystemRoleId(updateAccessRightPayload)
            .subscribe((updateAccessRightsRes) => {
              if (!updateAccessRightsRes) {
                return;
              }

              this.toastrService.success(
                `Permissions of ${this._selectedSystemRoleName} was successfully updated.`
              );

              this.sendActionsSignal([
                {
                  actionName: 'Save',
                  colId: this.currentGrantedAccessRights[0].systemRoleId.toString(),
                  isPerformAction: true
                },
                {
                  actionName: 'Cancel',
                  colId: this.currentGrantedAccessRights[0].systemRoleId.toString(),
                  isPerformAction: false
                }
              ]);
            })
        );

        // Unsubscribe the previous data from selected column
        this.columnDataChangeSubscription.unsubscribe();
      },
      'Save',
      'Are you sure you want to apply the new permissions to this role?'
    );
  }

  private processCloneSystemRole(): void {
    this.showConfirmationDialog(
      () => {
        this.permissionsTableSvc
          .duplicateSystemRole(Number(this._selectedSystemRoleId))
          .subscribe(
            (createdSystemRole) => {
              if (!createdSystemRole) {
                return;
              }

              this.toastrService.success(
                `${this._selectedSystemRoleName} was successfully duplicated`
              );

              this.refreshPermissionsColumnService();
              this.destroyAgGridConfig();
              this.getPermissionMatrixByModuleId(true, this._currentModuleId);
            },
            (err) => {
              if (err.error.title) {
                this.toastrService.error(
                  `There is already a system role associated with this role title. Please input a different name`
                );
              }
            }
          );

        this.sendActionsSignal([
          {
            actionName: 'Cancel',
            colId: this._selectedSystemRoleId.toString(),
            isPerformAction: false
          }
        ]);
      },
      'Duplicate',
      'Are you sure you want to duplicate this role?'
    );

    // Unsubscribe the previous data from selected column
    if (this.columnDataChangeSubscription) {
      this.columnDataChangeSubscription.unsubscribe();
    }
  }

  private processCancelSystemRole(): void {
    if (!this.isColumnDataChanges) {
      this.sendActionsSignal([
        {
          actionName: 'Cancel',
          colId: this.currentGrantedAccessRights[0].systemRoleId.toString(),
          isPerformAction: false
        }
      ]);
      this.columnDataChangeSubscription.unsubscribe();

      return;
    }

    this.showConfirmationDialog(
      () => {
        this.sendActionsSignal([
          {
            actionName: 'Cancel',
            colId: this.currentGrantedAccessRights[0].systemRoleId.toString(),
            isPerformAction: true
          }
        ]);

        this.columnDataChangeSubscription.unsubscribe();
      },
      'Cancel',
      'You have unsaved changes, are you sure you want to cancel?',
      'No'
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

  private get isColumnDataChanges(): boolean {
    // return this._isGrantedAccessRightsDiffer;
    return this.permissionsColumnSvc.isDifferentWithOriginColumnData(
      this._currentGrantedAccessRights
    );
  }

  private sendActionsSignal(signals: ActionSignal[]): void {
    signals.forEach((signal) => {
      this.permissionsTableSvc.sendActionSignal({
        actionName: signal.actionName,
        colId: signal.colId,
        isPerformAction: signal.isPerformAction
      });
    });
  }
}
