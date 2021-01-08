import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  HostListener,
  OnInit,
} from '@angular/core';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService,
  CxSurveyJsModeEnum,
  debounce,
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { BaseUserPermission } from 'app-models/common/permission/permission-setting';
import {
  IStrategicThrustPermission,
  StrategicThrustPermission,
} from 'app-models/common/permission/strategic-thrust-permission';
import { Department } from 'app-models/department-model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import {
  ODPConfigFilterParams,
  ODPFilterParams,
} from 'app/organisational-development/models/odp.models';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { commonCxFloatAttribute } from 'app/shared/constants/common.const';
import { CommonHelpers } from 'app/shared/helpers/common.helpers';
import { ToastrService } from 'ngx-toastr';
import { OdpService } from '../odp.service';
import { CodeRendererComponent } from './code-render.component';
import { CreateNewStrategicThrustComponent } from './create-new-strategic-thrust/create-new-strategic-thrust.component';
import {
  columnDefsConfig,
  ParamMetadata,
  ParseISOToYear,
  StrategicThrust,
  StrategicThrustToDTO,
} from './strategic-thrust-util';

@Component({
  selector: 'app-strategic-thrusts',
  templateUrl: './strategic-thrusts.component.html',
  styleUrls: ['./strategic-thrusts.component.scss'],
})
export class StrategicThrustsComponent
  extends BaseSmartComponent
  implements OnInit, IStrategicThrustPermission {
  defaultColDef: {} = {
    editable: false,
    resizable: true,
  };
  currentUser: User;
  currentDepartmentId: number;
  strategicThrustPermission: StrategicThrustPermission;
  frameworkComponents: any;
  columnDefs: any[];
  filterParams: ODPFilterParams = new ODPFilterParams();
  opjDepartmentId: number;
  opjDepartmentName: string;
  rowData: any[];
  objectivesMetadata: any[] = [];
  surveyConfig: any;
  gridApi: any;
  isShowDeleteRecord: boolean = false;
  noRowsTemplate: string = '<div class="grid-nodata">No data</div>';
  gridToolbarAttribute: object = commonCxFloatAttribute;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private odpService: OdpService,
    public ngbModal: NgbModal,
    private toastrService: ToastrService,
    private translateService: TranslateService,
    private authService: AuthService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private cxGlobalLoaderService: CxGlobalLoaderService
  ) {
    super(changeDetectorRef);
    this.columnDefs = columnDefsConfig;
    this.frameworkComponents = {
      codeCellRenderer: CodeRendererComponent,
    };
  }

  initStrategicThrustPermission(loginUser: User): void {
    this.strategicThrustPermission = new StrategicThrustPermission(loginUser);
  }

  ngOnInit(): void {
    this.currentUser = this.authService.userData().getValue();
    this.initStrategicThrustPermission(this.currentUser);
    this.updateColumnDefinition(this.strategicThrustPermission);

    this.odpService
      .getStrategicThrustsConfig(new ODPConfigFilterParams())
      .subscribe((odpConfig) => {
        if (!odpConfig || !odpConfig.configuration) {
          this.toastrService.error(
            this.translateService.instant(
              'Odp.NewStrategicThrust.MissingConfiguration'
            )
          );
        } else {
          this.surveyConfig = odpConfig.configuration;
        }
      });
    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
  }

  onGridReady(params?: any): void {
    this.gridApi = params.api;
    this.gridApi.sizeColumnsToFit();
  }

  // tslint:disable-next-line: no-unsafe-any
  @HostListener('window:resize') // tslint:disable-next-line:no-magic-numbers
  @debounce(100)
  onResize(): void {
    if (!this.gridApi) {
      return;
    }
    this.gridApi.sizeColumnsToFit();
  }

  changedDepartment(opjDepartment: Department): void {
    this.getStrategicThrusts(opjDepartment);
  }

  onSelectionChanged(event: any): void {
    const rowsSelected: [] = this.gridApi.getSelectedRows();
    this.isShowDeleteRecord = rowsSelected.length > 0 ? true : false;
  }

  onCellFocused(params: any): void {
    if (params.column.colId === 'name') {
      for (const iterator of this.objectivesMetadata) {
        if (iterator.resultIdentity.id === params.data.id) {
          this.createOrEditStrategicThrust(false, iterator);
          break;
        }
      }
    }
  }

  openConfirmDialogDelete(): void {
    const selectedRows = this.gridApi.getSelectedRows().length;
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'lg',
      centered: true,
    });
    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.Cancel'
    );
    modalComponent.confirmButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.ConfirmOK'
    );
    modalComponent.header = this.translateService.instant(
      'Odp.ConfirmationDialog.Header'
    );
    modalComponent.content = this.translateService.instant(
      'Odp.ConfirmationDialog.Deactive',
      { recordCount: selectedRows }
    );
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      modalRef.close();
      this.doDelete();
    });
  }

  doDelete(): void {
    const selectedRows = this.gridApi.getSelectedRows();
    if (selectedRows && selectedRows.length > 0) {
      this.cxGlobalLoaderService.showLoader();
      const resultIdentities: { [k: string]: any } = {};
      resultIdentities.identities = selectedRows;
      resultIdentities.deactivateAllVersion = true;
      this.odpService.deleteStrategicThrusts(resultIdentities).subscribe(
        (responses: any) => {
          this.cxGlobalLoaderService.hideLoader();

          // The problem is if we choose both items are can't delete and can delete
          // status request is 207, which mean it still execute this block
          const recordHasAlreadyUsed = responses.find(
            (response) => response.status === 400
          );
          if (recordHasAlreadyUsed) {
            this.toastrService.warning(
              this.translateService.instant(
                'Odp.NewStrategicThrust.DeleteSuccessButNotAll'
              )
            );
          } else {
            this.isShowDeleteRecord = false;
            this.toastrService.success(
              this.translateService.instant('Common.Message.DeleteSuccessfully')
            );
          }
        },
        (error: HttpErrorResponse) => {
          this.cxGlobalLoaderService.hideLoader();
          this.toastrService.warning(
            this.translateService.instant(
              'Odp.NewStrategicThrust.DeleteFailBecauseAlreadyUsedInLP'
            )
          );
        },
        () => {
          this.getStrategicThrustsByFilterParams(this.filterParams);
        }
      );
    }
  }

  createOrEditStrategicThrust(
    isCreate: boolean = true,
    dataJson: any = null
  ): void {
    if (!this.ngbModal.hasOpenModals()) {
      const modalRef = this.ngbModal.open(CreateNewStrategicThrustComponent, {
        centered: true,
        size: 'lg',
      });
      const newSTComponent = modalRef.componentInstance as CreateNewStrategicThrustComponent;
      newSTComponent.formConfigJson = this.surveyConfig;
      newSTComponent.formConfigJson.mode = this.strategicThrustPermission
        .allowEdit
        ? CxSurveyJsModeEnum.Edit
        : CxSurveyJsModeEnum.Display;
      newSTComponent.btnSubmitName = 'Save';
      if (isCreate) {
        newSTComponent.dialogHeaderText = this.translateService.instant(
          'Common.Action.NewStrategicThrust'
        );
      } else {
        newSTComponent.dialogHeaderText = this.strategicThrustPermission
          .allowEdit
          ? this.translateService.instant('Common.Action.EditStrategicThrust')
          : this.translateService.instant('Common.Action.ViewStrategicThrust');
        dataJson.answer.startYear = ParseISOToYear(dataJson.answer.startYear);
        dataJson.answer.endYear = ParseISOToYear(dataJson.answer.endYear);
        newSTComponent.dataAnswer = dataJson.answer;
      }

      this.subscription.add(
        newSTComponent.createOrEditST.subscribe((dataForm) => {
          const newParamMetadata = new ParamMetadata({
            answerJsonForm: dataForm,
            currentDepartmentId: this.opjDepartmentId,
            cxSurveyjsExtendedService: this.cxSurveyjsExtendedService,
            isCreate: null,
            objectIdentity: null,
          });
          if (!isCreate) {
            newParamMetadata.isCreate = false;
            newParamMetadata.objectIdentity = dataJson;
          }
          this.onSubmitStrategicThrust(
            new StrategicThrustToDTO(newParamMetadata)
          );
          modalRef.close();
        })
      );

      this.subscription.add(
        newSTComponent.cancel.subscribe(() => {
          modalRef.close();
        })
      );
    }
  }

  private onSubmitStrategicThrust(dataForm: StrategicThrustToDTO): void {
    this.cxGlobalLoaderService.showLoader();
    this.odpService.updateOrCreateStrategicThrust(dataForm).subscribe(
      (result: StrategicThrustToDTO) => {
        this.cxGlobalLoaderService.hideLoader();
        if (result && result.resultIdentity && result.resultIdentity.extId) {
          if (dataForm.isCreating()) {
            this.toastrService.success(
              this.translateService.instant('Common.Message.CreateSuccessfully')
            );
          } else {
            this.toastrService.success(
              this.translateService.instant('Common.Message.UpdateSuccessfully')
            );
          }
          this.getStrategicThrustsByFilterParams(this.filterParams);
        } else {
          this.toastrService.error(
            this.translateService.instant('Common.Message.APINotCompatible')
          );
        }
      },
      (error: HttpErrorResponse) => {
        this.cxGlobalLoaderService.hideLoader();
        if (error.status === 409) {
          this.toastrService.warning(
            this.translateService.instant(
              'Odp.NewStrategicThrust.CreateFailedBecauseOfExistingOne'
            )
          );
        } else {
          this.toastrService.error(`${error.error.error}`);
        }
      }
    );
  }

  private updateColumnDefinition(userPermission: BaseUserPermission): void {
    this.columnDefs.forEach((columnDef) => {
      if (columnDef.field === 'name') {
        columnDef.headerCheckboxSelection = userPermission.allowDelete;
        columnDef.headerCheckboxSelectionFilteredOnly =
          userPermission.allowDelete;
        columnDef.checkboxSelection = userPermission.allowDelete;
      }
    });
  }

  private getStrategicThrusts(opjDepartment: Department): void {
    this.objectivesMetadata = [];
    this.filterParams.departmentIds = [opjDepartment.identity.id];
    this.opjDepartmentId = opjDepartment.identity.id;
    this.opjDepartmentName = opjDepartment.departmentName;
    this.getStrategicThrustsByFilterParams(this.filterParams);
  }

  private getStrategicThrustsByFilterParams(
    filterParams: ODPFilterParams
  ): void {
    this.cxGlobalLoaderService.showLoader();
    this.odpService.getStrategicThrusts(filterParams).subscribe((response) => {
      this.cxGlobalLoaderService.hideLoader();
      if (response && response.length > 0) {
        this.objectivesMetadata = [];
        const dataResponse = response.map((element) => {
          const data = new StrategicThrust(element).formatIndex();
          this.objectivesMetadata.push(element);

          return data;
        });
        this.rowData = dataResponse;
      } else {
        this.rowData = [];
        this.noRowsTemplate = this.translateService.instant(
          'Odp.NewStrategicThrust.NoRowsTemplate'
        );
      }
    });
  }
}
