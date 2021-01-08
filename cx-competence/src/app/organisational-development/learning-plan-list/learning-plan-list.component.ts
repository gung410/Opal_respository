import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  HostListener,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { Router } from '@angular/router';
import {
  CxFormModal,
  CxGlobalLoaderService,
  CxItemTableHeaderModel,
  CxSurveyjsFormModalOptions,
  CxSurveyjsVariable,
  CxTableContainersComponent,
  CxTreeButtonCondition,
  debounce,
  DepartmentHierarchiesModel,
} from '@conexus/cx-angular-common';
import { CxObjectRoute } from '@conexus/cx-angular-common/lib/components/cx-tree/models/cx-object-route.model';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { environment } from 'app-environments/environment';
import { User } from 'app-models/auth.model';
import { Identity } from 'app-models/common.model';
import {
  ILearningPlanPermission,
  LearningPlanPermission,
} from 'app-models/common/permission/learning-plan-permission';
import { Department } from 'app-models/department-model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { BreadcrumbSettingService } from 'app-services/bread-crumb-setting.service';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { DepartmentHierarchyFilterService } from 'app-services/department-hierarchy-filter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { OrganizationalUnitHelpers } from 'app-utilities/organizational-unit.helpers';
import { SystemRoleHelpers } from 'app-utilities/system-role.helpers';
import { ILearningPlanGridRowModel } from 'app/approval-page/models/learning-plan-grid-row.model';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import {
  ChangePDPlanStatusDto,
  ODPConfigFilterParams,
  ODPDto,
  ODPFilterParams,
} from 'app/organisational-development/models/odp.models';
import { OdpService } from 'app/organisational-development/odp.service';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { commonCxFloatAttribute } from 'app/shared/constants/common.const';
import { LearningPlanListHeaders } from 'app/shared/constants/learning-plan-list-header.constant';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { CommonHelpers } from 'app/shared/helpers/common.helpers';
import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import { initUniversalToolbar } from 'app/staff/staff.container/staff-list/models/staff-action-mapping';
import { clone, uniqBy } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import {
  OdpActivity,
  OdpStatusCode,
} from '../learning-plan-detail/odp.constant';
import { DepartmentQueryModel } from '../models/filter-param.model';
import { IdpDto } from '../models/idp.model';
import { StaffListService } from './../../staff/staff.container/staff-list.service';
import { LPLNameRendererComponent } from './name-renderer.component';

@Component({
  selector: 'learning-plan-list',
  templateUrl: './learning-plan-list.component.html',
  styleUrls: ['./learning-plan-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LearningPlanListComponent
  extends BaseSmartComponent
  implements OnInit, ILearningPlanPermission {
  @ViewChild(CxTableContainersComponent)
  planTableContainer: CxTableContainersComponent<any, any>;
  currentUser: User;
  planList: ODPDto[] = [];
  filterParams: ODPFilterParams = new ODPFilterParams();
  learningPlanTableHeaders: CxItemTableHeaderModel[] = LearningPlanListHeaders;
  enableSubmit: boolean;
  showSubmitBtn: boolean = false;
  hasSubmitRight: boolean = false;
  columnDefs: any;
  rowSelection: any;
  rowData: ILearningPlanGridRowModel[];
  defaultColDef: any;
  context: any;
  frameworkComponents: any;
  isLoadingDepartments: boolean;
  breadCrumbNavigation: any[] = [];
  selectedDepartmentIds: {} = {};
  buttonCondition: object = new CxTreeButtonCondition();
  departmentFilterParams: FilterParamModel = new FilterParamModel();
  showStartNewLearningPlan: boolean = false;
  departmentRouteMap: object = {};
  departmentModel: DepartmentHierarchiesModel = new DepartmentHierarchiesModel();
  reloadDataAfterClearSearch: boolean = false;
  showDepartmentHierarchyBrowser: boolean = false;
  opjDepartmentId: number;
  opjDepartmentName: string;
  noRowsTemplate: string = '<div class="grid-nodata">No data</div>';
  gridToolbarAttribute: object = commonCxFloatAttribute;

  // Permission
  learningPlanPermission: LearningPlanPermission;

  private columns: any[] = [];
  private gridApi: any;
  private gridColumnApi: any;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private odpService: OdpService,
    private authService: AuthService,
    private router: Router,
    private toastrService: ToastrService,
    private breadcrumbSettingService: BreadcrumbSettingService,
    private staffListService: StaffListService,
    private departmentHierarchyFilterService: DepartmentHierarchyFilterService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private formModal: CxFormModal,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private translateService: TranslateService,
    private translateAdapterService: TranslateAdapterService,
    private departmentStoreService: DepartmentStoreService
  ) {
    super(changeDetectorRef);
  }

  initLearningPlanPermissionn(loginUser: User): void {
    this.learningPlanPermission = new LearningPlanPermission(loginUser);
  }

  ngOnInit(): void {
    this.currentUser = this.authService.userData().getValue();
    this.initLearningPlanPermissionn(this.currentUser);
    this.applyAccessChecking(this.currentUser);
    const currentDepartmentId = this.currentUser.departmentId;
    this.initDepartmentBrowser(currentDepartmentId);
    this.loadDataByDepartment(currentDepartmentId);

    this.setColumnDefs();
    this.defaultColDef = {
      headerCheckboxSelection: this.isFirstColumn,
      checkboxSelection: this.isFirstColumn,
      resizable: true,
    };
    this.rowSelection = 'multiple';
    this.context = { componentParent: this };
    this.frameworkComponents = {
      nameRenderer: LPLNameRendererComponent,
      detailRenderer: LPLNameRendererComponent,
    };
    window.scroll(0, 0);
    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
  }

  isFirstColumn(params: any): boolean {
    const displayedColumns = params.columnApi.getAllDisplayedColumns();
    const thisIsFirstColumn = displayedColumns[0] === params.column;

    return thisIsFirstColumn;
  }

  getLearningPlanList(opjDepartmentId: number): void {
    this.cxGlobalLoaderService.showLoader();
    this.filterParams.departmentIds = [opjDepartmentId];
    this.odpService.getLearningPlanList(this.filterParams).subscribe(
      (plans: any) => {
        if (plans) {
          this.planList = plans;
        } else {
          this.planList = [];
        }
        this.setRowData(this.planList);
        this.cxGlobalLoaderService.hideLoader();
        this.changeDetectorRef.detectChanges();
      },
      (_) => {
        this.planList = [];
        this.setRowData(this.planList);
        this.cxGlobalLoaderService.hideLoader();
        this.changeDetectorRef.detectChanges();
      }
    );
  }

  onClickStartNewPlan(): void {
    if (!this.formModal.hasOpenModals()) {
      this.cxGlobalLoaderService.showLoader();
      const surveyjsVariables = [
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.formDisplayMode,
          value: 'create',
        }),
      ];
      const options = {
        fixedButtonsFooter: true,
        showModalHeader: true,
        modalHeaderText: this.translateService.instant(
          'Odp.StartNewPlan.PopupHeader'
        ),
        cancelName: this.translateService.instant('Common.Action.Cancel'),
        submitName: this.translateService.instant('Common.Action.Save'),
        variables: surveyjsVariables,
      } as CxSurveyjsFormModalOptions;
      this.subscription.add(
        this.odpService
          .getLearningPlanConfig(new ODPConfigFilterParams())
          .subscribe((odpConfig: any) => {
            this.cxGlobalLoaderService.hideLoader();
            if (!odpConfig || !odpConfig.configuration) {
              this.toastrService.error(
                this.translateService.instant(
                  'Odp.StartNewPlan.MissingConfiguration'
                )
              );

              return;
            }
            this.cxSurveyjsExtendedService.setCurrentObject_DepartmentVariables(
              this.departmentModel.currentDepartmentId
            );
            const modalRef = this.formModal.openSurveyJsForm(
              odpConfig.configuration,
              null,
              [],
              options,
              { size: 'lg', centered: true }
            );
            const submitObserver = this.subscription.add(
              this.formModal.submit.subscribe((submitData) => {
                this.onSubmitNewLearningPlan(submitData);
                modalRef.close();
              })
            );
            modalRef.result
              .then((res) => {
                submitObserver.unsubscribe();
              })
              .catch((err) => {
                submitObserver.unsubscribe();
              });
          })
      );
    }
  }

  onSubmitNewLearningPlan(submitData: any): void {
    const pdplanDto = new PDPlanDto();
    pdplanDto.objectiveInfo = {
      identity: new Identity({
        archetype: this.cxSurveyjsExtendedService.getVariable(
          SurveyVariableEnum.currentDepartment_archetype
        ),
        customerId: environment.CustomerId,
        ownerId: environment.OwnerId,
        id: this.opjDepartmentId,
      }),
    };
    pdplanDto.answer = submitData;
    pdplanDto.timestamp = submitData.CyclePeriod + '-01-01';
    pdplanDto.errorIfExistingResult = true;
    this.convertDatetimeToISOFormat(pdplanDto);
    this.odpService.savePlan(pdplanDto, OdpActivity.Plan, true).subscribe(
      (pdplanResult: PDPlanDto) => {
        if (
          pdplanResult &&
          pdplanResult.resultIdentity &&
          pdplanResult.resultIdentity.extId
        ) {
          this.toastrService.success(
            this.translateService.instant('Odp.StartNewPlan.SubmitSuccess')
          );
          const planRoute = `/odp/plan-detail/${pdplanResult.resultIdentity.extId}`;
          this.router.navigate([planRoute]);
        } else {
          this.toastrService.error(
            this.translateService.instant('Common.Message.APINotCompatible')
          );
        }
      },
      (error: HttpErrorResponse) => {
        if (error.status === 409) {
          // Conflict
          this.toastrService.warning(
            this.translateService.instant(
              'Odp.StartNewPlan.CreateFailedBecauseOfExistingOne'
            )
          );
        } else {
          this.toastrService.error(`${error.error.error}`);
        }
      }
    );
  }

  onClickSubmit(): any {
    if (!this.gridApi) {
      return;
    }
    const selectedRows = this.gridApi.getSelectedRows();
    if (selectedRows && selectedRows.length > 0) {
      const selectedPlans = selectedRows.map((row) => {
        return row.plan;
      }) as ODPDto[];
      this.submitLearningPlans(selectedPlans);
    }
  }

  onSelectPlan(changingItem: any): void {
    this.updateMassActionButtonsShowHideStatus();
  }
  onSelectedDepartmentLabel(selectedDepartment: any): void {
    const newRoute = {};
    this.departmentModel.currentDepartmentId = selectedDepartment.identity.id;
    this.resetDepartmentDropdownData(selectedDepartment, newRoute);
    this.reloadDepartmentAndLearningPlans(
      this.departmentModel.currentDepartmentId
    );
    this.departmentHierarchyFilterService.setCurrentGlobalDepartmentId(
      this.departmentModel.currentDepartmentId
    );
  }

  onSelectDepartmentNode(objectRoute: CxObjectRoute<any>): void {
    this.departmentModel.currentDepartmentId = objectRoute.object.identity.id;
    this.resetDepartmentDropdownData(objectRoute.object, objectRoute.route);
    this.reloadDepartmentAndLearningPlans(
      this.departmentModel.currentDepartmentId
    );
    this.departmentHierarchyFilterService.setCurrentGlobalDepartmentId(
      this.departmentModel.currentDepartmentId
    );
  }
  expandChildDepartment(departmentSelected: Department): void {
    if (departmentSelected && departmentSelected.identity) {
      this.getHierarchicalDepartments(
        departmentSelected.identity.id,
        this.initFilterDepartment(),
        false
      );
    }
  }
  clickPlan(plan: any): void {
    const currentUrl = this.router.url;
    if (plan && plan.answer && plan.answer.Title) {
      this.breadcrumbSettingService.changeBreadcrumb({
        route: currentUrl,
        param: plan.answer.Title,
      });
    }
  }

  onGridReady(params: any): void {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.gridApi.hideOverlay();
  }

  @HostListener('window:resize') // tslint:disable-next-line:no-magic-numbers
  @debounce(100)
  onResize(): void {
    if (!this.gridApi) {
      return;
    }
    this.gridApi.sizeColumnsToFit();
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  onSelectionChanged(e: any): void {
    const selectedRows = this.gridApi.getSelectedRows();
    this.showSubmitBtn = selectedRows && selectedRows.length > 0 ? true : false;
    this.updateMassActionButtonsShowHideStatus();
  }

  onSearch(searchkey: any): void {
    if (searchkey !== '') {
      this.departmentModel.isShowSearchResult = true;
      this.subscription.add(
        this.staffListService
          .getHierarchicalDepartmentsByQuery(
            this.currentUser.departmentId,
            this.initFilterDepartment(searchkey)
          )
          .subscribe((response: any) => {
            this.departmentModel.departments = uniqBy(
              this.departmentModel.departments.concat(response),
              'identity.id'
            );
            this.departmentModel.searchDepartmentResult = response.filter(
              (department) =>
                department.departmentName
                  .trim()
                  .toLowerCase()
                  .indexOf(searchkey.trim().toLowerCase()) >= 0
            );

            this.departmentModel.departmentPathMap = [];
            this.departmentModel.searchDepartmentResult.forEach((element) => {
              this.departmentModel.departmentPathMap[
                element.identity.id
              ] = this.getPathOfDepartment(response, element);
              const routeDepartment = {};
              this.getRouteOfDepartment(response, element, routeDepartment);
              this.departmentRouteMap[element.identity.id] = routeDepartment;
            });
            this.departmentModel.isNoSearchResult =
              this.departmentModel.searchDepartmentResult.length === 0;
            this.changeDetectorRef.detectChanges();
          })
      );
    } else {
      this.departmentModel.isNoSearchResult = false;
      this.departmentModel.searchDepartmentResult = [];
      this.departmentModel.isShowSearchResult = false;
      this.reloadDataAfterClearSearch = true;
      this.changeDetectorRef.detectChanges();
    }
  }
  onClickSearchResult(department: any): void {
    const objectRoute = {
      route: this.departmentRouteMap[department.identity.id],
      object: department,
    };
    this.departmentModel.isShowSearchResult = false;
    this.departmentModel.isDetectExpandTree = true;
    this.departmentModel.searchDepartmentResult = [];
    this.changeDetectorRef.detectChanges();
    this.onSelectDepartmentNode(objectRoute);
  }

  private convertDatetimeToISOFormat(pdplan: PDPlanDto): void {
    if (pdplan.answer.startDate) {
      pdplan.answer.startDate = DateTimeUtil.surveyToDateLocalTime(
        pdplan.answer.startDate
      ).toISOString();
    }
    if (pdplan.answer.endDate) {
      pdplan.answer.endDate = DateTimeUtil.surveyToDateLocalTime(
        pdplan.answer.endDate
      ).toISOString();
    }
  }

  private setColumnDefs(): void {
    this.columnDefs = [
      {
        headerName: 'title',
        field: 'name',
        cellRenderer: 'nameRenderer',
        colId: 'name',
        minWidth: 500,
      },
      {
        headerName: 'period',
        field: 'period',
        colId: 'period',
        minWidth: 150,
      },
      {
        headerName: 'learning directions',
        field: 'learningDirections',
        colId: 'learningDirections',
        minWidth: 150,
      },
      {
        headerName: 'status',
        field: 'status',
        colId: 'status',
        minWidth: 150,
      },
      {
        headerName: 'completion rate',
        field: 'completionRate',
        colId: 'completionRate',
        minWidth: 150,
      },
    ];

    if (this.learningPlanPermission.allowViewOverall) {
      this.columnDefs.push({
        headerName: '',
        field: 'detail',
        cellRenderer: 'detailRenderer',
        colId: 'detail',
        minWidth: 220,
      });
    }
  }

  private setRowData(plans: IdpDto[]): void {
    if (!plans || plans.length === 0) {
      this.rowData = new Array();

      return;
    }

    if (this.gridColumnApi && this.gridColumnApi.columnController) {
      if (this.columns && this.columns.length <= 0) {
        this.columns = this.gridColumnApi.columnController.columnDefs;
        this.columns.pop();
      }
    }

    this.rowData = new Array();
    plans.forEach((plan) => {
      this.rowData.push({
        name: {
          display: plan.answer ? plan.answer.Title : '',
          route: `/odp/plan-detail/${plan.resultIdentity.extId}`,
        },
        period: plan.surveyInfo ? plan.surveyInfo.name : 'N/A',
        learningDirections: plan.childrenCount,
        status: plan.assessmentStatusInfo.assessmentStatusName
          ? plan.assessmentStatusInfo.assessmentStatusName
          : plan.assessmentStatusInfo.assessmentStatusCode,
        completionRate: plan.additionalProperties.completionRate
          ? plan.additionalProperties.completionRate + '%'
          : '0%',
        detail: {
          display: this.translateService.instant(
            'Common.Action.OverallView'
          ) as string,
          route: `/odp/overall-org-plan/${plan.resultIdentity.extId}`,
        },
        plan,
      });
    });
  }

  private updateMassActionButtonsShowHideStatus(): void {
    if (!this.gridApi) {
      return;
    }
    const selectedRows = this.gridApi.getSelectedRows();
    if (selectedRows && selectedRows.length > 0) {
      const selectedPlans = selectedRows.map((row) => {
        return row.plan;
      }) as ODPDto[];
      // All selected items must have status "Started" to get the button "Submit" enabled.
      this.enableSubmit = selectedPlans.every(
        (x) =>
          x.assessmentStatusInfo &&
          x.assessmentStatusInfo.assessmentStatusCode === OdpStatusCode.Started
      );
    }
  }

  private submitLearningPlans(selectedPlans: ODPDto[]): void {
    this.cxGlobalLoaderService.showLoader();
    this.enableSubmit = false; // Prevent clicking twice.
    selectedPlans.forEach((plan) => {
      if (plan.pdPlanActivity === OdpActivity.Plan) {
        const newStatusTypeCode = OdpStatusCode.PendingForApproval;
        const changePDPlanStatusDto = new ChangePDPlanStatusDto({
          targetStatusType: {
            assessmentStatusCode: newStatusTypeCode,
          },
          autoMapTargetStatusType: false,
        });
        this.odpService
          .changeStatusPlan(
            plan.resultIdentity.id,
            changePDPlanStatusDto,
            plan.pdPlanActivity
          )
          .subscribe(
            (changePDPlanStatusResult) => {
              this.showChangeStatusMessage(
                plan,
                changePDPlanStatusResult,
                newStatusTypeCode
              );
              this.updateMassActionButtonsShowHideStatus();
              this.cxGlobalLoaderService.hideLoader();
            },
            (_) => {
              this.toastrService.error(
                `Unexpected error happening when trying to submit Organisational PD Journey "${plan.answer.Title}".`
              );
              this.cxGlobalLoaderService.hideLoader();
            }
          );
      }
    });
  }
  private showChangeStatusMessage(
    pdPlanDto: ODPDto,
    changePDPlanStatusResult: ChangePDPlanStatusDto,
    expectedStatusTypeCode: string
  ): void {
    if (
      changePDPlanStatusResult &&
      changePDPlanStatusResult.targetStatusType &&
      changePDPlanStatusResult.targetStatusType.assessmentStatusCode ===
        expectedStatusTypeCode
    ) {
      this.toastrService.success(
        `Organisational PD Journey "${pdPlanDto.answer.Title}" has been submitted successfully.`
      );
      pdPlanDto.assessmentStatusInfo =
        changePDPlanStatusResult.targetStatusType;
      const index = this.planList.findIndex(
        (plan) => plan.resultIdentity.id === pdPlanDto.resultIdentity.id
      );
      if (index > -1) {
        this.updateLearningPlanList(index, pdPlanDto);
      }
    } else {
      this.toastrService.error(
        `Failed to submit Organisational PD Journey "${pdPlanDto.answer.Title}".`
      );
    }
  }

  private updateLearningPlanList(index: number, updatedPlan: ODPDto): void {
    const newPLanList = clone(this.planList);
    newPLanList[index] = updatedPlan;
    this.planList = newPLanList;
    this.setRowData(this.planList);
    this.changeDetectorRef.detectChanges();
  }

  private resetDepartmentDropdownData(newDepartment: any, route: any): void {
    this.selectedDepartmentIds = {};
    this.selectedDepartmentIds[newDepartment.identity.id] =
      newDepartment.identity.id;
  }

  private buildCurrentDepartmentCrumb(
    currentDepartmentCrumb: any[],
    hierarchicalUserDepartments: any[],
    userDepartmentId: number,
    currentDepartmentId: number
  ): any[] {
    if (!currentDepartmentCrumb) {
      currentDepartmentCrumb = [];
    }
    for (const department of hierarchicalUserDepartments) {
      if (department.identity.id !== currentDepartmentId) {
        continue;
      }
      currentDepartmentCrumb.push({
        name: department.departmentName,
        identity: department.identity,
      });
      if (department.identity.id === userDepartmentId) {
        continue;
      }
      currentDepartmentId = department.parentDepartmentId;

      return this.buildCurrentDepartmentCrumb(
        currentDepartmentCrumb,
        hierarchicalUserDepartments,
        userDepartmentId,
        currentDepartmentId
      );
    }

    return currentDepartmentCrumb.reverse();
  }

  private reloadDepartmentAndLearningPlans(currentDepartmentId: number): void {
    this.loadDataByDepartment(currentDepartmentId);
    this.getHierarchicalDepartments(
      currentDepartmentId,
      this.initFilterDepartment()
    );
  }

  private getHierarchicalDepartments(
    currentDepartmentId: number,
    departmentQuery: DepartmentQueryModel,
    isBuildTableAndCurmb: boolean = true
  ): void {
    this.subscription.add(
      this.staffListService
        .getHierarchicalDepartmentsByQuery(currentDepartmentId, departmentQuery)
        .subscribe((response: any) => {
          if (!response) {
            return;
          }

          if (this.reloadDataAfterClearSearch) {
            this.departmentModel.departments = [];
            this.changeDetectorRef.detectChanges();
          }
          this.departmentModel.departments = uniqBy(
            this.departmentModel.departments.concat(response),
            'identity.id'
          );
          if (isBuildTableAndCurmb) {
            this.breadCrumbNavigation = [];
            this.breadCrumbNavigation = Object.assign(
              [],
              this.buildCurrentDepartmentCrumb(
                this.breadCrumbNavigation,
                this.departmentModel.departments,
                this.currentUser.departmentId,
                currentDepartmentId
              )
            );
          }
          this.departmentModel.isDetectExpandTree = false;
          this.reloadDataAfterClearSearch = false;
          this.changeDetectorRef.detectChanges();
        })
    );
  }

  private getPathOfDepartment(
    departmentArray: Array<any>,
    department: any
  ): any {
    const parentDepartment = departmentArray.find(
      (element) => element.identity.id === department.parentDepartmentId
    );
    if (!parentDepartment) {
      return department.departmentName;
    } else {
      return this.getPathOfDepartment(departmentArray, parentDepartment).concat(
        `/${department.departmentName}`
      );
    }
  }

  private getRouteOfDepartment(
    departmentArray: Array<any>,
    department: any,
    newObject: any
  ): void {
    const parentDepartment = departmentArray.find(
      (element) => element.identity.id === department.parentDepartmentId
    );
    if (!parentDepartment) {
      newObject[department.identity.id] = department.identity.id;
    } else {
      newObject[parentDepartment.identity.id] = parentDepartment.identity.id;
      this.getRouteOfDepartment(departmentArray, parentDepartment, newObject);
    }
  }

  private loadDataByDepartment(currentDepartmentId: number): void {
    forkJoin([
      this.departmentStoreService.getDepartmentTypesByDepartmentId(
        currentDepartmentId
      ),
      this.departmentStoreService.getDepartmentById(currentDepartmentId),
    ]).subscribe((results) => {
      const currentDepartmentOrganizationalUnitTypes = results[0];
      const currentDepartment = results[1];
      const organizationalUnitTypesHavingOPJ =
        environment.pdplanConfig.organizationalUnitTypesHavingOPJ;
      if (
        OrganizationalUnitHelpers.hasAnyOrganizationalUnitType(
          currentDepartmentOrganizationalUnitTypes,
          organizationalUnitTypesHavingOPJ
        )
      ) {
        this.departmentModel.currentDepartmentId = currentDepartmentId;
        this.loadData(currentDepartment);
      } else {
        this.loadDataFromAncestor(
          currentDepartment,
          organizationalUnitTypesHavingOPJ
        );
      }
    });
  }

  private initDepartmentBrowser(currentDepartmentId: number): void {
    this.getHierarchicalDepartments(
      currentDepartmentId,
      this.initFilterDepartment()
    );
    this.departmentModel.text.header = this.translateService.instant(
      'Common.Label.OrganisationUnit'
    );
    this.departmentModel = initUniversalToolbar(this.translateAdapterService);
  }

  private loadDataFromAncestor(
    currentDepartment: Department,
    organizationalUnitTypesHavingOPJ: string[]
  ): void {
    this.departmentStoreService
      .getAncestorDepartmentsOfDepartment(currentDepartment.identity.id, true)
      .subscribe((ancestorDepartments) => {
        const sortedAncestorDepartments = ancestorDepartments.sort(
          (a, b) => b.path.length - a.path.length
        );
        for (const ancestorDepartment of sortedAncestorDepartments) {
          if (
            OrganizationalUnitHelpers.hasAnyOrganizationalUnitType(
              ancestorDepartment.organizationalUnitTypes,
              organizationalUnitTypesHavingOPJ
            )
          ) {
            this.loadData(ancestorDepartment);

            break;
          }
        }
      });
  }

  private loadData(opjDepartment: Department): void {
    this.opjDepartmentId = opjDepartment.identity.id;
    this.opjDepartmentName = opjDepartment.departmentName;
    this.getLearningPlanList(opjDepartment.identity.id);
  }

  private initFilterDepartment(searchText?: string): DepartmentQueryModel {
    const paramQuery = new DepartmentQueryModel({
      includeParent: false,
      includeChildren: true,
      maxChildrenLevel: 1,
      countChildren: true,
    });
    if (searchText !== undefined) {
      paramQuery.searchText = searchText;
    }

    return paramQuery;
  }

  private applyAccessChecking(currentUser: User): void {
    this.showStartNewLearningPlan = this.learningPlanPermission.allowCreate;
    this.hasSubmitRight = this.learningPlanPermission.allowSubmit;
    this.showDepartmentHierarchyBrowser = SystemRoleHelpers.hasAnyRole(
      currentUser.systemRoles,
      environment.pdplanConfig.permissionConfig.departmentHierarchyBrowser
        .includeChildren.roleExtIds
    );
  }
}
