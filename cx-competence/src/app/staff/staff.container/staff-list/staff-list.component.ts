import { GridApi } from '@ag-grid-community/core';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  HostListener,
  Input,
  OnInit,
  QueryList,
  ViewChildren,
  ViewEncapsulation,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  ActionsModel,
  ActionToolbarModel,
  CxColumnSortType,
  CxCustomizedCheckboxComponent,
  CxGlobalLoaderService,
  CxTreeButtonCondition,
  debounce,
  DepartmentHierarchiesModel,
} from '@conexus/cx-angular-common';
import { CxBreadCrumbItem } from '@conexus/cx-angular-common/lib/components/cx-breadcrumb-simple/model/breadcrumb.model';
import { CxObjectRoute } from '@conexus/cx-angular-common/lib/components/cx-tree/models/cx-object-route.model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { User } from 'app-models/auth.model';
import { Department } from 'app-models/department-model';
import { DepartmentHierarchyFilterService } from 'app-services/department-hierarchy-filter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { UserService } from 'app-services/user.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { Utilities } from 'app-utilities/utilities';
import {
  IdpStatusCodeEnum,
  IdpStatusEnum,
} from 'app/individual-development/idp.constant';
import { ExportLearningNeedsAnalysisDialogComponent } from 'app/learning-needs-analysis/export-learning-needs-analysis-dialog/export-learning-needs-analysis-dialog.component';
import { AppConstant } from 'app/shared/app.constant';
import { LinkViewButtonRendererComponent } from 'app/shared/components/ag-grid-renderer/link-view-button/link-view-button-renderer.component';
import { LnaCompletionRateRendererComponent } from 'app/shared/components/ag-grid-renderer/lna-completion-rate-renderer/lna-completion-rate-renderer.component';
import { LNAStatusRendererComponent } from 'app/shared/components/ag-grid-renderer/lna-status-renderer/lna-status-renderer.component';
import { PdPlanStatusRendererComponent } from 'app/shared/components/ag-grid-renderer/pd-plan-status/pdplan-status-renderer.component';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import {
  commonCxFloatAttribute,
  findIndexCommon,
} from 'app/shared/constants/common.const';
import { GroupFilterConst } from 'app/shared/constants/filter-group.constant';
import { PDPlanProcessClass } from 'app/shared/constants/pdplan-process-constant';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import {
  StatusTypeEnum,
  UserStatusTypeEnum,
} from 'app/shared/constants/user-status-type.enum';
import { CommonHelpers } from 'app/shared/helpers/common.helpers';
import { environment } from 'environments/environment';
import { cloneDeep, intersection, isEmpty, uniqBy } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { StaffListService } from './../staff-list.service';
import { SLApprovingOfficerRendererComponent } from './ag-grid-renderer/approving-officer-renderer.component';
import { LNAHeaderComponent } from './ag-grid-renderer/LNA-header.component';
import { SLNameRendererComponent } from './ag-grid-renderer/name-renderer.component';
import { SLServiceSchemeRendererComponent } from './ag-grid-renderer/service-scheme-renderer.component';
import { SLUserGroupsRendererComponent } from './ag-grid-renderer/user-groups-renderer.component';
import { AssignLnAssessmentsDialogComponent } from './assign-ln-assessments-dialog/assign-ln-assessments-dialog.component';
import {
  DepartmentQueryModel,
  FilterParamModel,
} from './models/filter-param.model';
import { FilterModel } from './models/filter.model';
import {
  LearningNeedAnalysisRemindingList,
  LearningNeedAnalysisRemindingRequest,
} from './models/reminder-request.model';
import {
  initStaffActions,
  initUniversalToolbar,
} from './models/staff-action-mapping';
import { PagedStaffsList, Staff } from './models/staff.model';
import { UserEntityStatusEnum } from './models/user-accounts.model';
import { ReminderDialogComponent } from './reminder-dialog/reminder-dialog.component';
import { LearningNeedAnalysisReminderService } from './reminder-dialog/reminder.service';
import { StaffExportComponent } from './staff-export/staff-export.component';
import { StaffFilterComponent } from './staff-filter/staff-filter.component';
import { StaffShowHideComponent } from './staff-show-hide-column/staff-show-hide-column.component';
@Component({
  selector: 'staff-list',
  templateUrl: './staff-list.component.html',
  styleUrls: ['./staff-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StaffListComponent extends BaseSmartComponent implements OnInit {
  @ViewChildren(CxCustomizedCheckboxComponent)
  filterListComponents: QueryList<CxCustomizedCheckboxComponent>;
  @Input()
  currentUser: User;
  // department navigation tree view
  buttonCondition: CxTreeButtonCondition<any> = new CxTreeButtonCondition();
  reloadDataAfterClearSearch: boolean = false;
  currentDepartmentCrumb: any[];
  defaultPageSize: number = AppConstant.ItemPerPage;
  filterData: FilterModel = new FilterModel();
  filterDataApplied: FilterModel = new FilterModel();
  showPaging: boolean = false;
  filterParams: FilterParamModel = new FilterParamModel();
  defaultUserStatusFilter: any[] = [
    StatusTypeEnum.Active.code,
    StatusTypeEnum.Inactive.code,
    StatusTypeEnum.New.code,
  ];
  pagedStaffsList: PagedStaffsList;
  columnDefCopyToCheckShowHides: any = [];
  currentStaffsPage: number = 1;
  currentStaffsInPage: Staff[] = [];
  currentStaffTableSortType: CxColumnSortType = CxColumnSortType.UNSORTED;
  staffsPerPage: number = environment.ItemPerPage;
  staffSortField: string = '';
  scrollTriggerValue: number = 160;
  loading: boolean = false;
  hasFilter: boolean = false;
  noEmployeeText: string = 'No Records';
  pDPlanProcessClass: any = PDPlanProcessClass;
  columnDefs: any;
  rowSelection: any;
  rowData: any;
  defaultColDef: any;
  context: any;
  frameworkComponents: any;
  columns: any[] = [];
  adjustFilterApplied: FilterModel;
  filterOptions: any;
  isDisplayOrganisationSearch: boolean = false;
  currentUserRoles: any[] = [];
  searchDepartmentResult: any[] = [];
  departmentRouteMap: object = {};
  isSearchedAcrossSubOrg: boolean;
  departmentModel: DepartmentHierarchiesModel = new DepartmentHierarchiesModel();
  breadCrumbNavigation: any[] = [];
  userActions: ActionToolbarModel;
  isVerticalToShowMenuAction: boolean;
  noRowsTemplate: string = '<div class="grid-nodata">No data</div>';
  gridToolbarAttribute: object = commonCxFloatAttribute;

  private searchParam: string = '';
  private gridApi: GridApi;
  private gridColumnApi: any;
  private get employeesSelected(): any {
    return this.gridApi.getSelectedRows();
  }
  private fromDateIndex: number = 0;
  private toDateIndex: number = 1;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private userService: UserService,
    private reminderService: LearningNeedAnalysisReminderService,
    private translateService: TranslateService,
    private route: ActivatedRoute,
    private router: Router,
    private ngbModal: NgbModal,
    private toastrService: ToastrService,
    private staffListService: StaffListService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private departmentHierarchyFilterService: DepartmentHierarchyFilterService,
    private translateAdapterService: TranslateAdapterService
  ) {
    super(changeDetectorRef);
    this.setColumnDefs();
    this.defaultColDef = {
      headerCheckboxSelection: this.isFirstColumn,
      checkboxSelection: this.isFirstColumn,
      resizable: true,
    };
    this.rowSelection = 'multiple';
    this.context = { componentParent: this };
    this.frameworkComponents = {
      nameRenderer: SLNameRendererComponent,
      approvingOfficerRenderer: SLApprovingOfficerRendererComponent,
      userGroupsRenderer: SLUserGroupsRendererComponent,
      serviceSchemeRenderer: SLServiceSchemeRendererComponent,
      LNAHeaderComponent,
      LNAStatusRenderer: LNAStatusRendererComponent,
      PDPStatusRenderer: PdPlanStatusRendererComponent,
      viewRenderer: LinkViewButtonRendererComponent,
      LnaCompletionRateRenderer: LnaCompletionRateRendererComponent,
    };
  }

  ngOnInit(): void {
    this.filterParams.pageSize = this.defaultPageSize;
    this.currentUserRoles =
      this.currentUser.systemRoles &&
      this.currentUser.systemRoles.map((role) => role.identity.extId)
        ? this.currentUser.systemRoles.map((role) => role.identity.extId)
        : [];
    this.getHierarchicalDepartments(
      this.currentUser.topAccessibleDepartment.identity.id,
      this.initFilterDepartment()
    );
    this.noEmployeeText = this.translateService.instant(
      'Common.Label.NoRecords'
    );
    this.departmentModel.text.header = this.translateService.instant(
      'Common.Label.OrganisationUnit'
    );
    this.departmentModel = initUniversalToolbar(this.translateAdapterService);
    this.departmentModel.currentDepartmentId = this.currentUser.defaultHierarchyDepartment.departmentId;
    this.userActions = initStaffActions(this.translateAdapterService);
    const totalItems =
      this.userActions.listNonEssentialActions.length +
      this.userActions.listSpecifyActions.length;
    const maximumItems = 7;
    this.isVerticalToShowMenuAction = totalItems > maximumItems ? false : true;
    this.subscription.add(
      this.staffListService.searchValueSubject.subscribe((searchValue) => {
        this.searchParam = searchValue;
        this.getStaffListByQueryParam();
      })
    );
    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
    window.addEventListener('scroll', CommonHelpers.freezeAgGridScroll(), true);
  }

  changeSelectedColumn(selected: any, column: any): void {
    if (column && column.colId) {
      const columnNeedToHide = this.columnDefCopyToCheckShowHides.find(
        (col) => col.colId === column.colId
      );
      if (columnNeedToHide) {
        columnNeedToHide.hide = !selected;
      }
      this.gridColumnApi.setColumnVisible(column.colId, selected);
      this.gridApi.sizeColumnsToFit();
    }
  }

  onGridReady(params: any): void {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.gridApi.sizeColumnsToFit();
    const subscription = this.staffListService.searchingBehaviorSubject.subscribe(
      (searchValue) => {
        if (searchValue) {
          this.searchParam = searchValue;
        }
      }
    );
    subscription.unsubscribe();
    this.getStaffListByQueryParam();
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

  onSortChanged(event: any): void {
    const sortModels = this.gridApi.getSortModel();
    if (sortModels) {
      const sortModel = sortModels[0];
      this.currentStaffTableSortType = sortModel
        ? sortModel.sort === 'asc'
          ? CxColumnSortType.ASCENDING
          : CxColumnSortType.DESCENDING
        : CxColumnSortType.UNSORTED;
      this.staffSortField = sortModel ? sortModel.colId : '';
      this.getListEmployee(this.filterParams);
    }
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  onClickNominate(event: any): void {}

  onClickReminder(): void {
    if (!this.gridApi) {
      return;
    }
    const selectedRows = this.gridApi.getSelectedRows();
    const selectedEmployees = selectedRows
      ? selectedRows.map((row) => row.staff)
      : [];
    if (selectedEmployees.length === 0) {
      this.toastrService.warning(
        this.translateService.instant(
          'Staff.ReminderDialog.Alert.Warning.SelectOneRow'
        )
      );

      return;
    }

    const validUserStatuses = [
      UserStatusTypeEnum.New,
      UserStatusTypeEnum.Active,
    ];
    const invalidUsers = selectedEmployees.filter(
      (emp) =>
        !emp.entityStatus ||
        !validUserStatuses.includes(emp.entityStatus.statusId)
    );
    if (invalidUsers.length > 0) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.AssignLearningNeedsAnalysis.ValidationMessage.InvalidAccountStatus',
          { invalidUsers: this.getEmployeeNamesAsString(invalidUsers) }
        )
      );

      return;
    }

    const notSubmittedStatuses: string[] = [
      IdpStatusCodeEnum.NotStarted,
      IdpStatusCodeEnum.Started,
    ];
    const isAllowSetReminder = selectedEmployees.find(
      (employee) =>
        !notSubmittedStatuses.includes(
          employee.assessmentInfos.LearningNeed.statusInfo.assessmentStatusCode
        )
    );
    if (isAllowSetReminder) {
      this.toastrService.warning(
        this.translateService.instant(
          'Staff.ReminderDialog.Alert.Warning.UnableToSendReminder'
        )
      );

      return;
    }

    const reminderModalRef = this.ngbModal.open(ReminderDialogComponent, {
      centered: true,
      size: 'lg',
    });
    const reminderDialogComponent = reminderModalRef.componentInstance as ReminderDialogComponent;
    reminderDialogComponent.selectedEmployees = selectedEmployees.filter(
      (employee) =>
        notSubmittedStatuses.includes(
          employee.assessmentInfos.LearningNeed.statusInfo.assessmentStatusCode
        )
    );

    this.subscription.add(
      reminderDialogComponent.done.subscribe(
        (reminderUserListModel: LearningNeedAnalysisRemindingList) => {
          const reminderRequestDtos = reminderUserListModel.learningNeedCompletionRemindings.map(
            (reminderUser: LearningNeedAnalysisRemindingRequest) => {
              return {
                ...reminderUser,
                year: this.getLNAYearPeriod(),
              };
            }
          );

          this.reminderService
            .remindToCompleteLearningNeedAnalysis({
              learningNeedCompletionRemindings: reminderRequestDtos,
              dateToSend: reminderUserListModel.dateToSend,
            })
            .subscribe(
              (success) => {
                this.toastrService.success(
                  this.translateService.instant(
                    'Staff.ReminderDialog.Alert.Success'
                  )
                );
              },
              (error) => {
                this.toastrService.warning(
                  this.translateService.instant(
                    'Staff.ReminderDialog.Alert.Warning.SendReminderFail'
                  )
                );
              }
            );

          this.gridApi.deselectAll();
          reminderModalRef.close();
        }
      )
    );
    this.subscription.add(
      reminderDialogComponent.cancel.subscribe(() => {
        this.gridApi.deselectAll();
        reminderModalRef.close();
      })
    );
  }

  onClickExport(isExportAll: boolean = false): void {
    if (
      !isExportAll &&
      this.employeesSelected &&
      this.employeesSelected.length === 0
    ) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.Export.Not_Selected_Users'
        )
      );

      return;
    }
    const modalRef = this.ngbModal.open(StaffExportComponent, {
      size: 'lg',
      centered: true,
    });
    const modalRefComponent = modalRef.componentInstance as StaffExportComponent;
    this.addMoreFilterCondition(this.filterParams, true);
    modalRefComponent.appliedFilterData = { ...this.filterParams };
    if (!isExportAll) {
      modalRefComponent.selectedItems = this.employeesSelected;
    }
    this.subscription.add(
      modalRefComponent.completeExport.subscribe(() => {
        modalRef.close();
      })
    );
    this.subscription.add(
      modalRefComponent.cancel.subscribe(() => {
        modalRef.close();
      })
    );
  }

  onClickExportLearningNeedsAnalysis(): void {
    const modalRef = this.ngbModal.open(
      ExportLearningNeedsAnalysisDialogComponent,
      { size: 'lg' }
    );
    const modalRefComponent = modalRef.componentInstance as ExportLearningNeedsAnalysisDialogComponent;
    this.addMoreFilterCondition(this.filterParams, true);
    modalRefComponent.appliedEmployeeFilter = { ...this.filterParams };
    modalRefComponent.totalUsers = this.pagedStaffsList.totalItems;
    this.subscription.add(
      modalRefComponent.completeExport.subscribe(() => {
        modalRef.close();
      })
    );
    this.subscription.add(
      modalRefComponent.cancel.subscribe(() => {
        modalRef.close();
      })
    );
  }

  onSortTypeChange($event: {
    fieldSort: string;
    sortType: CxColumnSortType;
  }): void {
    this.currentStaffTableSortType = $event.sortType;
    this.staffSortField =
      $event.sortType === CxColumnSortType.UNSORTED ? '' : $event.fieldSort;
    this.getListEmployee(this.filterParams);
  }

  onRemoveStaffs(staffs: any[]): void {}

  getAvatarFromEmail(emailAddress: string): string {
    return this.userService.getGravatarImageUrl(emailAddress);
  }

  onCurrentStaffPageChange(pageIndex: number): void {
    this.currentStaffsPage = pageIndex;
    this.filterParams.pageIndex = pageIndex;
    this.getListEmployee(this.filterParams);
    window.scroll(0, 0);
  }

  onPageSizeChange(pageSize: number): void {
    if (Number(pageSize) > Number(this.filterParams.pageSize)) {
      this.filterParams.pageIndex = 1;
    }
    this.filterParams.pageSize = pageSize;
    this.getListEmployee(this.filterParams);
    window.scroll(0, 0);
  }

  updateTagGroupAndFilter(adjustedFilter: any): void {
    this.filterParams = adjustedFilter;
    this.changeDetectorRef.detectChanges();
    this.getListEmployee(this.filterParams);
  }

  onFilterButtonClick(): void {
    const modalRef = this.ngbModal.open(StaffFilterComponent, {
      size: 'lg',
      windowClass: 'filter-dialog-custom-size',
      centered: true,
    });
    const staffFilterComponent = modalRef.componentInstance as StaffFilterComponent;
    staffFilterComponent.adjustAppliedData =
      this.adjustFilterApplied &&
      this.adjustFilterApplied.appliedFilter !== undefined &&
      this.adjustFilterApplied.appliedFilter.length > 0
        ? { ...this.adjustFilterApplied }
        : undefined;

    staffFilterComponent.choicesData = { ...this.filterOptions };
    staffFilterComponent.currentDepartmentId = [
      this.departmentModel.currentDepartmentId,
    ];
    this.changeDetectorRef.detectChanges();

    this.subscription.add(
      staffFilterComponent.applyClick.subscribe(
        (appliedData: {
          queryParams: FilterParamModel;
          filteredData: FilterModel;
        }) => {
          this.adjustFilterApplied = cloneDeep(appliedData.filteredData);
          this.filterDataApplied = cloneDeep(appliedData.filteredData);
          this.updateQueryParams(appliedData.filteredData);
          this.getListEmployee(this.filterParams);
          this.changeDetectorRef.detectChanges();
          modalRef.close();
        }
      )
    );
    this.subscription.add(
      staffFilterComponent.cancel.subscribe(() => {
        modalRef.close();
      })
    );
  }

  updateFilteredData($event: FilterModel): void {
    this.adjustFilterApplied = { ...$event };
    this.updateQueryParams($event);
    this.getListEmployee(this.filterParams);
    this.changeDetectorRef.detectChanges();
  }

  updateQueryParams(tagGroupChanged: FilterModel): void {
    this.filterParams = new FilterParamModel();
    this.filterParams.pageIndex = 1;
    this.setFilterParamDepartmentId(this.departmentModel.currentDepartmentId);
    this.filterParams.filterOnSubDepartment = this.isSearchedAcrossSubOrg;
    if (tagGroupChanged.appliedFilter === undefined) {
      return;
    }
    const statusTypeLogs = {
      statusTypeIds: [IdpStatusEnum.Approved],
      changedAfter: '',
      changedBefore: '',
    };
    tagGroupChanged.appliedFilter.forEach((item) => {
      const filterCondition = item.data.value;
      switch (item.filterOptions.data) {
        case GroupFilterConst.STATUS:
          this.filterParams.entityStatuses = filterCondition;
          break;
        case GroupFilterConst.AGE_GROUP:
          this.filterParams.ageRanges = filterCondition;
          break;
        case GroupFilterConst.ROLE:
          this.filterParams.multiUserTypeIds[0] = filterCondition;
          break;
        case GroupFilterConst.ACCOUNT_TYPE:
          const selectedFullyUserAccountType = 2;
          if (filterCondition.length !== selectedFullyUserAccountType) {
            this.filterParams.externallyMastered =
              filterCondition[0] === UserEntityStatusEnum.ManualUserAccount
                ? false
                : true;
          }
          break;
        case GroupFilterConst.TYPE_OF_OU:
          this.filterParams.organizationalUnitTypeIds.push(filterCondition);
          break;
        case GroupFilterConst.CREATION_DATE:
          this.filterParams.createdBefore = Utilities.formatDateToISO(
            filterCondition[this.toDateIndex]
          );
          this.filterParams.createdAfter = Utilities.formatDateToISO(
            filterCondition[this.fromDateIndex]
          );
          break;
        case GroupFilterConst.EXPIRATION_DATE:
          this.filterParams.expirationDateBefore = Utilities.formatDateToISO(
            filterCondition[this.toDateIndex]
          );
          this.filterParams.expirationDateAfter = Utilities.formatDateToISO(
            filterCondition[this.fromDateIndex]
          );
          break;
        case GroupFilterConst.SERVICE_SCHEME:
          const serviceSchemeIndex = 0;
          this.filterParams.multiUserTypeExtIds[serviceSchemeIndex].push(
            filterCondition
          );
          break;
        case GroupFilterConst.TEACHING_SUBJECTS:
          this.filterParams.userDynamicAttributes.push(
            `$.teachingSubjects[]=${filterCondition.map((x) => x.id)}`
          );
          break;
        case GroupFilterConst.JOB_FAMILY:
          this.filterParams.userDynamicAttributes.push(
            `$.jobFamily[]=${filterCondition.map((x) => x.id)}`
          );
          break;
        case GroupFilterConst.DEVELOPMENTAL_ROLE:
          const developmentalRoleIndex = 1;
          this.filterParams.multiUserTypeExtIds[developmentalRoleIndex].push(
            filterCondition
          );
          break;
        case GroupFilterConst.DESIGNATION:
          this.filterParams.userDynamicAttributes.push(
            `$.designation=${filterCondition}`
          );
          break;
        case GroupFilterConst.LNA_STATUS:
          this.filterParams.multiStatusTypeIds.LearningNeed =
            filterCondition && filterCondition.length > 0
              ? filterCondition
              : [];
          break;
        case GroupFilterConst.PDP_STATUS:
          this.filterParams.multiStatusTypeIds.LearningPlan =
            filterCondition && filterCondition.length > 0
              ? filterCondition
              : [];
          break;
        case GroupFilterConst.USER_GROUP:
          this.filterParams.multiUserGroupIds[0] = filterCondition;
          break;
        case GroupFilterConst.APPROVAL_GROUP:
          if (filterCondition && filterCondition.length > 0) {
            filterCondition.forEach((value) => {
              this.filterParams.multiUserGroupIds[1].push(value);
            });
          }
          break;
        case GroupFilterConst.LNA_ACKNOWLEDGED_PERIOD:
          statusTypeLogs.changedAfter = Utilities.formatDateToISO(
            filterCondition[this.fromDateIndex]
          );
          statusTypeLogs.changedBefore = Utilities.formatEndDateToISO(
            filterCondition[this.toDateIndex]
          );
          this.filterParams.statusTypeLogs.LearningNeed = [statusTypeLogs];
          break;
        case GroupFilterConst.PD_PLAN_ACKNOWLEDGED_PERIOD:
          statusTypeLogs.changedAfter = Utilities.formatDateToISO(
            filterCondition[this.fromDateIndex]
          );
          statusTypeLogs.changedBefore = Utilities.formatEndDateToISO(
            filterCondition[this.toDateIndex]
          );
          this.filterParams.statusTypeLogs.LearningPlan = [statusTypeLogs];
          break;
        default:
          break;
      }
    });
  }

  getListEmployee(filterParams: FilterParamModel): void {
    this.gridApi.showLoadingOverlay();
    // set filter params for idp/employeelist api
    this.addMoreFilterCondition(filterParams);
    // get employees
    this.subscription.add(
      this.userService.getListEmployee(filterParams).subscribe(
        (pagedEmployeeList: any) => {
          if (pagedEmployeeList) {
            pagedEmployeeList.items = pagedEmployeeList.items
              ? pagedEmployeeList.items
              : [];

            // TODO: remove this fake data code block when release
            pagedEmployeeList.items.forEach((item) => {
              const date = new Date();
              date.setDate(date.getDate() - Math.floor(Math.random() * 10));
              item.lastActive = date;
            });
            // End.
            this.pagedStaffsList = pagedEmployeeList;

            this.setRowData(this.pagedStaffsList.items);
            if (this.pagedStaffsList.items.length > this.staffsPerPage) {
              this.showPaging = true;
            }

            this.processFilterOptions(this.pagedStaffsList);
          } else {
            // TODO show text empty list
            this.toastrService.warning('List employee empty!');
          }
          this.gridApi.hideOverlay();
          window.scroll(0, 0);
          this.changeDetectorRef.detectChanges();
        },
        () => {
          this.toastrService.error(
            "Can't get list employee, please try again!"
          );
          this.gridApi.hideOverlay();
          this.changeDetectorRef.detectChanges();
        }
      )
    );
  }
  addMoreFilterCondition(
    filterParams: FilterParamModel,
    isExport: boolean = false
  ): void {
    if (!isExport && (!filterParams.pageIndex || filterParams.pageIndex <= 1)) {
      filterParams.includeFilterOptions = {
        statusTypeInfo: true,
      };
    } else {
      filterParams.includeFilterOptions = {};
    }
    filterParams.idpEmployeeSearchKey = this.searchParam;
    filterParams.sortField =
      this.staffSortField === '' ? undefined : this.staffSortField;
    filterParams.sortOrder =
      this.currentStaffTableSortType === CxColumnSortType.UNSORTED
        ? undefined
        : this.currentStaffTableSortType === CxColumnSortType.ASCENDING
        ? 'Ascending'
        : 'Descending';
    if (filterParams && filterParams.entityStatuses.length === 0) {
      filterParams.entityStatuses = this.defaultUserStatusFilter;
    }
  }
  processFilterOptions(pagedStaffsList: any): void {
    if (pagedStaffsList && pagedStaffsList.filterOptions) {
      const choicesDataMap = { ...pagedStaffsList.filterOptions };
      if (
        choicesDataMap.statusTypeInfos &&
        choicesDataMap.statusTypeInfos.LearningNeed
      ) {
        choicesDataMap.statusTypeInfos.LearningNeed = choicesDataMap.statusTypeInfos.LearningNeed.map(
          (item) => {
            return {
              value: item.assessmentStatusId,
              text: item.assessmentStatusName ? item.assessmentStatusName : '',
            };
          }
        );
      }

      if (
        choicesDataMap.statusTypeInfos &&
        choicesDataMap.statusTypeInfos.LearningPlan
      ) {
        choicesDataMap.statusTypeInfos.LearningPlan = choicesDataMap.statusTypeInfos.LearningPlan.filter(
          (item) => item.assessmentStatusCode !== IdpStatusCodeEnum.NotAdded
        );
        choicesDataMap.statusTypeInfos.LearningPlan = choicesDataMap.statusTypeInfos.LearningPlan.map(
          (item) => {
            return {
              value: item.assessmentStatusId,
              text: item.assessmentStatusName ? item.assessmentStatusName : '',
            };
          }
        );
      }

      this.filterOptions = { ...choicesDataMap };
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
    this.onSelectDepartmentNode(objectRoute);
  }
  searchSubOrg(isSearchSubOrg: boolean): void {
    if (isSearchSubOrg === undefined) {
      return;
    }
    this.filterParams.filterOnSubDepartment = isSearchSubOrg;
    this.isSearchedAcrossSubOrg = isSearchSubOrg;

    this.changeSelectedColumn(isSearchSubOrg, this.columnDefs[1]);
    this.gridColumnApi.columnController.columnDefs[1].hide = !isSearchSubOrg;
    this.columns = this.gridColumnApi.columnController.columnDefs;
    this.getListEmployee(this.filterParams);
  }
  onSearch(searchkey: any): void {
    this.cxGlobalLoaderService.showLoader();
    if (searchkey !== '') {
      this.departmentModel.isShowSearchResult = true;
      this.subscription.add(
        this.staffListService
          .getHierarchicalDepartmentsByQuery(
            this.currentUser.defaultHierarchyDepartment.departmentId,
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
            this.cxGlobalLoaderService.hideLoader();
            this.changeDetectorRef.detectChanges();
          })
      );
    } else {
      this.departmentModel.isNoSearchResult = false;
      this.departmentModel.searchDepartmentResult = [];
      this.departmentModel.isShowSearchResult = false;
      this.reloadDataAfterClearSearch = true;
      this.reloadDepartmentAndEmployees(
        this.currentUser.defaultHierarchyDepartment.departmentId
      );
    }
  }
  onAssignLearningNeedsClicked(): void {
    if (!this.gridApi) {
      return;
    }
    let selectedEmployees = [];
    const selectedRows = this.gridApi.getSelectedRows();
    if (selectedRows && selectedRows.length > 0) {
      selectedEmployees = selectedRows.map((row) => {
        return row.staff;
      }) as any[];
    }

    if (!this.validateEmployeesForAssignLearningNeeds(selectedEmployees)) {
      return;
    }
    this.showPopupAssignLNA(selectedEmployees);
  }
  onClickBreadcrumbNavigation(selectedDepartment: any): void {
    const newRoute = {};
    this.departmentModel.currentDepartmentId = selectedDepartment.identity.id;
    this.clearDepartmentFilter();
    this.reloadDepartmentAndEmployees(this.departmentModel.currentDepartmentId);
    this.departmentHierarchyFilterService.setCurrentGlobalDepartmentId(
      this.departmentModel.currentDepartmentId
    );
  }
  onSelectDepartmentNode(objectRoute: CxObjectRoute<any>): void {
    this.departmentModel.currentDepartmentId = objectRoute.object.identity.id;
    this.clearDepartmentFilter();
    this.reloadDepartmentAndEmployees(this.departmentModel.currentDepartmentId);
    this.departmentHierarchyFilterService.setCurrentGlobalDepartmentId(
      this.departmentModel.currentDepartmentId
    );
  }

  clearDepartmentFilter(): void {
    // Clear all filters whenever user changes the department unit
    if (!!this.filterDataApplied) {
      this.filterDataApplied.appliedFilter = [];
    }
    if (!!this.adjustFilterApplied) {
      this.adjustFilterApplied.appliedFilter = [];
    }
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
  onButtonGroupActionClick($event: ActionsModel): void {
    if ($event) {
      switch ($event.actionType) {
        case StatusActionTypeEnum.Reminder:
          this.onClickReminder();
          break;
        case StatusActionTypeEnum.AssignLearningNeeds:
          this.onAssignLearningNeedsClicked();
          break;
        case StatusActionTypeEnum.Export:
          this.onClickExport();
          break;
        default:
          break;
      }
    }
  }
  openDialogToEnableColumns(): void {
    const modalRef = this.ngbModal.open(StaffShowHideComponent, {
      size: 'sm',
      centered: true,
    });
    const instanceComponent = modalRef.componentInstance as StaffShowHideComponent;
    const showHideColumns = cloneDeep(
      this.columnDefs.filter((item, index) => index !== 0)
    );

    showHideColumns.forEach((showHideColumn) => {
      const hiddenColumn = this.columnDefCopyToCheckShowHides.find(
        (col) => col.colId === showHideColumn.colId
      );
      if (hiddenColumn) {
        showHideColumn.hide = hiddenColumn.hide;
      }
    });
    instanceComponent.staffListColumnDef = showHideColumns;

    this.subscription.add(
      instanceComponent.changeShowHideColumn.subscribe((observe) => {
        this.changeSelectedColumn(observe.$event, observe.columnNeedToHide);
      })
    );
    this.subscription.add(
      instanceComponent.cancel.subscribe(() => {
        modalRef.close();
      })
    );
  }

  onGridSelectionChanged(): void {
    this.updateUserActionBaseOnGridSelection();
  }

  onGridRowDataChanged(): void {
    this.updateUserActionBaseOnGridSelection();
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
      if (
        department.identity.id !== currentDepartmentId &&
        !this.checkExistingBreadCrumbItem(
          currentDepartmentCrumb,
          currentDepartmentId
        )
      ) {
        // Prevent duplication when adding a new item into the bread crumb.
        continue;
      }
      currentDepartmentCrumb.push({
        name: department.departmentName,
        identity: department.identity,
      });
      if (
        department.identity.id === userDepartmentId &&
        !this.checkExistingBreadCrumbItem(
          currentDepartmentCrumb,
          userDepartmentId
        )
      ) {
        // Prevent duplication when adding a new item into the bread crumb.
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
  /**
   * Checks whether the specified identity existing in the bread crumb items or not.
   * @param breadCrumbItems The list of bread crumb items.
   * @param checkingIdentity The identity which using for checking exist.
   */
  private checkExistingBreadCrumbItem(
    breadCrumbItems: CxBreadCrumbItem[],
    checkingIdentity: number
  ): boolean {
    return (
      breadCrumbItems &&
      breadCrumbItems.findIndex(
        (a: CxBreadCrumbItem) => a.identity.id === checkingIdentity
      ) !== findIndexCommon.notFound
    );
  }
  private reloadDepartmentAndEmployees(currentDepartmentId: number): void {
    this.filterParams = new FilterParamModel();
    this.filterParams.filterOnSubDepartment = this.isSearchedAcrossSubOrg;
    this.setFilterParamDepartmentId(currentDepartmentId);
    this.getListEmployee(this.filterParams);
    this.getHierarchicalDepartments(
      currentDepartmentId,
      this.initFilterDepartment()
    );
  }
  private setFilterParamDepartmentId(currentDepartmentId?: number): void {
    this.filterParams.departmentIds =
      !currentDepartmentId ||
      this.currentUser.defaultHierarchyDepartment.departmentId ===
        currentDepartmentId
        ? []
        : [currentDepartmentId];
  }
  private getHierarchicalDepartments(
    currentDepartmentId: number,
    departmentQuery: DepartmentQueryModel,
    isBuildTableAndCurmb: boolean = true
  ): void {
    this.cxGlobalLoaderService.showLoader();
    this.subscription.add(
      this.staffListService
        .getHierarchicalDepartmentsByQuery(currentDepartmentId, departmentQuery)
        .subscribe((response: any) => {
          if (!response) {
            this.cxGlobalLoaderService.hideLoader();

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
                currentDepartmentId ===
                  this.currentUser.topAccessibleDepartment.identity.id
                  ? this.currentUser.defaultHierarchyDepartment.departmentId
                  : currentDepartmentId
              )
            );
          }
          this.departmentModel.isDetectExpandTree = false;
          this.reloadDataAfterClearSearch = false;
          this.cxGlobalLoaderService.hideLoader();
          this.changeDetectorRef.detectChanges();
          this.isDisplayOrganisationSearch =
            this.departmentModel.departments.length > 1;
        })
    );
  }
  private isFirstColumn(params: any): boolean {
    const displayedColumns = params.columnApi.getAllDisplayedColumns();
    const thisIsFirstColumn = displayedColumns[0] === params.column;

    return thisIsFirstColumn;
  }
  private setColumnDefs(): void {
    this.columnDefs = [
      {
        headerName: 'Name',
        field: 'name',
        cellRenderer: 'nameRenderer',
        colId: 'firstName',
        sortable: true,
        sort: 'asc',
        hide: false,
        width: 480,
        minWidth: 400,
        cellClass: 'text-overflow-overlay',
        pinned: 'left',
        lockPinned: true,
      },
      {
        headerName: 'Organisation Unit',
        field: 'departmentName',
        colId: 'departmentName',
        hide: true,
        width: 320,
        minWidth: 240,
        headerClass: 'text-overflow-wrap',
      },
      {
        headerName: 'Service Scheme',
        field: 'serviceScheme',
        cellRenderer: 'serviceSchemeRenderer',
        colId: 'PersonnelGroup',
        hide: false,
        minWidth: 200,
        headerClass: 'text-overflow-wrap',
        sortable: true,
      },
      {
        headerName: 'LNA Completion Rate',
        field: 'LNACompletionRate',
        cellRenderer: 'LnaCompletionRateRenderer',
        colId: 'lnaCompletionRate',
        hide: false,
        minWidth: 220,
        sortable: true,
      },
      {
        headerName: 'LNA Status',
        field: 'LNAStatus',
        cellRenderer: 'LNAStatusRenderer',
        colId: 'LearningNeedStatusType',
        minWidth: 220,
        maxWidth: 250,
        hide: false,
        sortable: true,
      },
      {
        headerName: 'PD Plan Status',
        field: 'PDPStatus',
        cellRenderer: 'PDPStatusRenderer',
        colId: 'LearningPlanStatusType',
        minWidth: 220,
        maxWidth: 250,
        hide: false,
        sortable: true,
      },
      {
        headerName: 'Approving Officers',
        field: 'approvingOfficer',
        cellRenderer: 'approvingOfficerRenderer',
        colId: 'ApprovalGroup',
        minWidth: 200,
        hide: false,
        sortable: true,
      },
      {
        headerName: 'User Groups',
        field: 'userGroups',
        cellRenderer: 'userGroupsRenderer',
        colId: 'UserPool',
        minWidth: 200,
        hide: true,
        sortable: true,
      },
    ];
    this.columnDefCopyToCheckShowHides = cloneDeep(
      this.columnDefs.filter((item, index) => index !== 0)
    );
  }
  private setRowData(staffs: any): void {
    if (staffs) {
      if (this.gridColumnApi && this.gridColumnApi.columnController) {
        if (this.columns && this.columns.length <= 0) {
          this.columns = this.gridColumnApi.columnController.columnDefs;
          this.columns.pop();
        }
        this.rowData = new Array();
        staffs.forEach((staff) => {
          this.rowData.push({
            name: {
              email: staff.email,
              fullName: staff.fullName,
              id: staff.identity.id,
              avatarUrl: staff.avatarUrl
                ? staff.avatarUrl
                : AppConstant.defaultAvatar,
            },
            departmentName: staff.department.name,
            approvingOfficer: staff.approvalGroups,
            userGroups: staff.userPools,
            serviceScheme: staff.personnelGroups,
            LNACompletionRate: staff.assessmentInfos.LearningNeed,
            LNAStatus: {
              userId: staff.identity.id,
              assessmentInfo: staff.assessmentInfos.LearningNeed,
            },
            PDPStatus: {
              userId: staff.identity.id,
              assessmentInfo: staff.assessmentInfos.LearningPlan,
            },
            detail: `/employee/detail/${staff.identity.id}`,
            staff,
          });
        });
      }
    }
  }

  private showPopupAssignLNA(selectedEmployees: any): void {
    const assignLearningNeedsModalRef = this.ngbModal.open(
      AssignLnAssessmentsDialogComponent,
      { centered: true, size: 'lg' }
    );
    const assignLNADialogComponent = assignLearningNeedsModalRef.componentInstance as AssignLnAssessmentsDialogComponent;
    assignLNADialogComponent.selectedEmployees = selectedEmployees;
    this.subscription.add(
      assignLNADialogComponent.done.subscribe(
        (successfullyAssignedLNAssessmentsUsers: any[]) => {
          this.onAssignLNA(successfullyAssignedLNAssessmentsUsers);
          assignLearningNeedsModalRef.close();
        }
      )
    );
    this.subscription.add(
      assignLNADialogComponent.cancel.subscribe(() => {
        assignLearningNeedsModalRef.close();
      })
    );
  }
  private onAssignLNA = (assignedLNAUsers: Staff[]) => {
    if (assignedLNAUsers === undefined || assignedLNAUsers.length === 0) {
      return;
    }

    const assignedLNAUsersMap = {};
    assignedLNAUsers.forEach((employee) => {
      assignedLNAUsersMap[employee.identity.id] = employee;
    });

    // update page staff list items
    this.pagedStaffsList.items.forEach((employee, index) => {
      if (assignedLNAUsersMap[employee.identity.id] === undefined) {
        return;
      }
      this.pagedStaffsList.items[index] =
        assignedLNAUsersMap[employee.identity.id];
    });

    this.pagedStaffsList = {
      ...this.pagedStaffsList,
      items: [...this.pagedStaffsList.items],
    };

    this.rowData.forEach((rowData) => {
      if (
        !rowData.staff ||
        !rowData.staff.identity ||
        !rowData.staff.identity.id
      ) {
        return;
      }
      const currentStaff = rowData.staff;
      const staffId = currentStaff.identity.id;
      if (assignedLNAUsersMap[staffId] === undefined) {
        return;
      }
      const newEmployee = assignedLNAUsersMap[staffId];
      rowData.LNAStatus = {
        userId: staffId,
        assessmentInfo: newEmployee.assessmentInfos.LearningNeed,
      };
      rowData.staff = newEmployee;
    });

    this.rowData = [...this.rowData];
    this.changeDetectorRef.detectChanges();
  };

  private getLNAYearPeriod(): number {
    return new Date().getFullYear();
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
  private initFilterDepartment(searchText?: string): DepartmentQueryModel {
    const paramQuery = new DepartmentQueryModel({
      includeParent: false,
      includeChildren:
        intersection(
          this.currentUserRoles,
          environment.pdplanConfig.permissionConfig.departmentHierarchyBrowser
            .includeChildren.roleExtIds
        ).length > 0,
      maxChildrenLevel: 1,
      countChildren: true,
    });
    if (searchText !== undefined) {
      paramQuery.searchText = searchText;
    }

    return paramQuery;
  }

  private validateEmployeesForAssignLearningNeeds(
    selectedEmployees: Array<Staff>
  ): boolean {
    if (selectedEmployees.length === 0) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.AssignLearningNeedsAnalysis.ValidationMessage.NoItemSelected'
        )
      );

      return false;
    }

    const notLearners = selectedEmployees.filter(
      (emp) =>
        !emp.systemRoleInfos ||
        !emp.systemRoleInfos.some(
          (role) => role.identity.extId === UserRoleEnum.Learner.toString()
        )
    );
    if (notLearners.length > 0) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.AssignLearningNeedsAnalysis.ValidationMessage.NotLearners'
        )
      );

      return false;
    }

    const validUserStatuses = [
      UserStatusTypeEnum.New,
      UserStatusTypeEnum.Active,
    ];
    const invalidUsers = selectedEmployees.filter(
      (emp) =>
        !emp.entityStatus ||
        !validUserStatuses.includes(emp.entityStatus.statusId)
    );
    if (invalidUsers.length > 0) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.AssignLearningNeedsAnalysis.ValidationMessage.InvalidAccountStatus',
          { invalidUsers: this.getEmployeeNamesAsString(invalidUsers) }
        )
      );

      return false;
    }

    const notApplicableServiceSchemeGuid =
      '72a1df40-d592-11e9-9740-0242ac120004';
    const employeesHaveNoServiceScheme = selectedEmployees.filter(
      (emp) =>
        emp.personnelGroups.length === 0 ||
        !!emp.personnelGroups.find(
          (serviceScheme) =>
            serviceScheme.identity.extId === notApplicableServiceSchemeGuid
        )
    );
    if (employeesHaveNoServiceScheme.length > 0) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.AssignLearningNeedsAnalysis.ValidationMessage.NoServiceScheme'
        )
      );

      return false;
    }

    const employeesHaveNoApprovingOfficers = selectedEmployees.filter(
      (emp) => !emp.approvalGroups || !emp.approvalGroups.length
    );
    if (employeesHaveNoApprovingOfficers.length > 0) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.AssignLearningNeedsAnalysis.ValidationMessage.NoApprovingOfficer'
        )
      );

      return false;
    }

    const checkingDueDateOfLNAStatuses: string[] = [
      IdpStatusCodeEnum.NotStarted,
      IdpStatusCodeEnum.Started,
      IdpStatusCodeEnum.Rejected,
    ];
    const employeesHaveInvalidLNAStatus = selectedEmployees.every(
      (emp) =>
        emp.assessmentInfos.LearningNeed.statusInfo.assessmentStatusCode ===
          IdpStatusCodeEnum.NotAdded ||
        (checkingDueDateOfLNAStatuses.includes(
          emp.assessmentInfos.LearningNeed.statusInfo.assessmentStatusCode
        ) &&
          emp.assessmentInfos.LearningNeed.dueDate &&
          DateTimeUtil.isInThePast(emp.assessmentInfos.LearningNeed.dueDate))
    );
    if (!employeesHaveInvalidLNAStatus) {
      this.toastrService.warning(
        this.translateAdapterService.getValueImmediately(
          'Staff.AssignLearningNeedsAnalysis.ValidationMessage.HasBeenAssigned'
        )
      );

      return false;
    }

    return true;
  }

  private getEmployeeNamesAsString(employees: Staff[]): string {
    if (!employees) {
      return '';
    }
    const employeeNames = employees.map((emp) => emp.fullName.trim());

    return employeeNames.join(', ');
  }

  private getStaffListByQueryParam(): void {
    this.filterParams.filterOnSubDepartment = this.isSearchedAcrossSubOrg;
    this.filterParams.pageIndex = 1;
    this.setFilterParamDepartmentId();
    this.getListEmployee(this.filterParams);
  }

  private updateUserActionBaseOnGridSelection(): void {
    if (!this.userActions || isEmpty(this.userActions.listEssentialActions)) {
      return;
    }

    const selectedRows = this.gridApi.getSelectedRows();
    const isEnable = selectedRows.length > 0;
    this.userActions.listEssentialActions.forEach((i) => {
      i.disable = !isEnable;
    });
  }
}
