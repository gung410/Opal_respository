import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { Router } from '@angular/router';
import {
  CxConfirmationDialogComponent,
  CxFormModal,
  CxGlobalLoaderService,
  CxInformationDialogService,
  CxSurveyjsEventModel,
  CxSurveyjsFormModalOptions,
  CxSurveyjsVariable,
  CxTreeButtonCondition,
  CxTreeDropdownComponent,
  CxTreeIcon,
  CxTreeText,
  DepartmentHierarchiesModel
} from '@conexus/cx-angular-common';
import { CxObjectRoute } from '@conexus/cx-angular-common/lib/components/cx-tree/models/cx-object-route.model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { ShowHideColumnModel } from 'app-models/ag-grid-column.model';
import { ColumDefModel } from 'app-models/ag-grid-config.model';
import { User } from 'app-models/auth.model';
import { DepartmentType } from 'app-models/department-type.model';
import { EmailModel, TemplateModel } from 'app-models/email.model';
import { EntityStatus } from 'app-models/entity-status.model';
import { Identity } from 'app-models/identity.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { CrossSiteScriptingUtil } from 'app-utilities/cross-site-scripting.utils';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { SurveyUtils } from 'app-utilities/survey-utils';
import { SystemRole } from 'app/core/models/system-role';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { CareerPathsStoreService } from 'app/core/store-services/career-paths-data.service';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import { DevelopmentalRolesStoreService } from 'app/core/store-services/developmental-roles-store.service';
import { LearningFrameWorksStoreService } from 'app/core/store-services/learning-frame-works-store.service';
import { PDCatalogueStoreService } from 'app/core/store-services/pd-catalogue-store.service';
import { PersonnelGroupsStoreService } from 'app/core/store-services/personnel-groups-store.service';
import { GlobalKeySearchStoreService } from 'app/core/store-services/search-key-store.service';
import { Department } from 'app/department-hierarchical/models/department.model';
import { DepartmentQueryModel } from 'app/department-hierarchical/models/filter-params.model';
import { OrganizationUnitType } from 'app/department-hierarchical/models/organization-unit-type.enum';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import {
  commonCxFloatAttribute,
  findIndexCommon
} from 'app/shared/constants/common.const';
import { DesignationQueryParamEnum } from 'app/shared/constants/designation-query-param.enum';
import { fieldNonJsonDynamic } from 'app/shared/constants/fields-non-json-dynamic.constant';
import { GroupFilterConst } from 'app/shared/constants/filter-group.constant';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import {
  StatusReasonTypeConstant,
  StatusTypeEnum
} from 'app/shared/constants/user-status-type.enum';
import { UserTypeEnum } from 'app/shared/constants/user-type.enum';
import { CheckingUserRolesService } from 'app/user-accounts/services/checking-user-roles.service';
import { AddMemberToGroupFormJSON } from 'app/user-groups/user-group-form';
import { UserGroupsDataService } from 'app/user-groups/user-groups-data.service';
import { MembershipDto } from 'app/user-groups/user-groups.model';
import * as _ from 'lodash';
import { combineLatest, forkJoin, from, Observable } from 'rxjs';
import { isNullOrUndefined } from 'util';

import { GridApi } from '@ag-grid-community/core';
import { environment } from 'app-environments/environment';
import { Utils } from 'app-utilities/utils';
import { isEmpty } from 'lodash';
import { stringEmpty } from '../shared/common.constant';
import { AssignAODialogComponent } from './assign-ao-dialog/assign-ao-dialog.component';
import { ApprovalGroupTypeEnum } from './constants/approval-group.enum';
import { PDCatalogueConstant } from './constants/pd-catalogue.enum';
import { GenderEnum } from './constants/user-field-mapping.constant';
import { EditUserDialogComponent } from './edit-user-dialog/edit-user-dialog.component';
import {
  ApprovalInfoTabModel,
  EditUserDialogSubmitModel,
  MemberApprovalGroupModel
} from './edit-user-dialog/edit-user-dialog.model';
import { SuspensionReasonFormJSON } from './form/inactive-status-reason-form';
import { DefaultSystemRoleData } from './mock-data/user-account-mock-data';
import {
  ApprovalGroup,
  ApprovalGroupQueryModel
} from './models/approval-group.model';
import { CxBreadCrumbItem } from './models/breadcumb.model';
import { EditUser } from './models/edit-user.model';
import { PDCatalogueEnumerationDto } from './models/pd-catalogue.model';
import {
  AddingMoreOptionsBaseOnRole,
  DefaultOptionModel,
  InstructionReporting
} from './models/reporting-by-systemrole.model';
import {
  initUniversalToolbar,
  initUserActions,
  USER_ACTION_MAPPING_CONST
} from './models/user-action-mapping';
import {
  PagingResponseModel,
  UserManagement,
  UserManagementQueryModel
} from './models/user-management.model';

import { SAM_PERMISSIONS } from 'app/shared/constants/sam-permission.constant';
import { map } from 'rxjs/operators';
import { moveUserAccountFormJSON } from './move-user-form';
import { UserAccountConfirmationDialogComponent } from './user-account-confirmation-dialog/user-account-confirmation-dialog.component';
import { UserAccountsDataService } from './user-accounts-data.service';
import { UserAccountsHelper, UserAccountTabEnum } from './user-accounts.helper';
import { UserEntityStatusEnum } from './user-accounts.model';
import { ActionsModel } from './user-actions/models/actions.model';
import { UserActionsModel } from './user-actions/models/user-actions.model';
import { UserExportComponent } from './user-export/user-export.component';
import { FilterModel } from './user-filter/applied-filter.model';
import { UserFilterComponent } from './user-filter/user-filter.component';
import { UserShowHideComponent } from './user-show-hide-column/user-show-hide-column.component';

@Component({
  selector: 'user-accounts',
  templateUrl: './user-accounts.component.html',
  styleUrls: ['./user-accounts.component.scss'],
  providers: [
    UserAccountsDataService,
    ToastrAdapterService,
    UserAccountsHelper
  ],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserAccountsComponent
  extends BaseScreenComponent
  implements OnInit {
  get showFliter(): boolean {
    return this.isUserAccountTab || this.isOtherPlaceOfWorkTab;
  }

  get isUserAccountTab(): boolean {
    return this.currentTabAriaLabel === UserAccountTabEnum.UserAccounts;
  }

  get isOtherPlaceOfWorkTab(): boolean {
    return this.currentTabAriaLabel === UserAccountTabEnum.UserOtherPlace;
  }
  @ViewChild(CxTreeDropdownComponent)
  currentDepartment: Department;
  departmentTreeDropdown: CxTreeDropdownComponent<any>;
  defaultPageSize: number = AppConstant.ItemPerPage;
  buttonCondition: CxTreeButtonCondition<any> = new CxTreeButtonCondition();
  icon: CxTreeIcon = new CxTreeIcon();
  text: CxTreeText = new CxTreeText();

  userItemsData: PagingResponseModel<UserManagement>;
  userOtherPlaceList: UserManagement[];

  isLoadingDepartments: boolean = true;
  isLoadingEmployee: boolean = true;

  pending1stLevelApprovalList: UserManagement[];
  pending2ndLevelApprovalList: UserManagement[];
  pendingSpecialLevelApprovalList: UserManagement[];

  filterDataApplied: FilterModel = new FilterModel();
  isDisplayOrganisationNavigation: boolean = false;

  divisionBranchSchoolRoles: any[] = [];
  superAdminRole: any[] = [];
  noneAdminRole: any[] = [];
  userAccountAdminRole: any[] = [];

  selectedUser: UserManagement[] = [];
  isClearSelected: boolean = false;
  isDisplayOrganisationSearch: boolean = false;

  showHideColumns: ShowHideColumnModel;
  departmentModel: DepartmentHierarchiesModel = new DepartmentHierarchiesModel();
  breadCrumbNavigation: any[] = [];
  isSearchedAcrossSubOrg: boolean;
  get userActions(): UserActionsModel {
    return this._userActions;
  }
  set userActions(v: UserActionsModel) {
    this._userActions = v;
  }
  userActionsForExportButton: ActionsModel[] = [];
  userActionsForCreateAccButton: ActionsModel[] = [];
  isVerticalToShowMenuAction: boolean;
  userAccountTabEnum: object = UserAccountTabEnum;
  gridToolbarAttribute: object = commonCxFloatAttribute;

  samPermissions = SAM_PERMISSIONS;

  private currentTabAriaLabel: string = UserAccountTabEnum.UserAccounts;
  private userListSearchKeyHistory: Map<string, string> = new Map([
    [UserAccountTabEnum.UserAccounts, ''],
    [UserAccountTabEnum.Pending1st, ''],
    [UserAccountTabEnum.Pending2nd, ''],
    [UserAccountTabEnum.Pending3rd, ''],
    [UserAccountTabEnum.UserOtherPlace, '']
  ]);

  private tabLabelToPendingCode: Map<UserAccountTabEnum, string> = new Map([
    [UserAccountTabEnum.Pending1st, StatusTypeEnum.PendingApproval1st.code],
    [UserAccountTabEnum.Pending2nd, StatusTypeEnum.PendingApproval2nd.code],
    [UserAccountTabEnum.Pending3rd, StatusTypeEnum.PendingApproval3rd.code]
  ]);

  private _userActions: UserActionsModel;
  private pdCataloguePersonnelGroups: PDCatalogueEnumerationDto[] = [];
  private pdCatalogueCareerPaths: PDCatalogueEnumerationDto[] = [];
  private pdCatalogueDevelopmentalRoles: PDCatalogueEnumerationDto[] = [];

  private personnelGroups: { [id: string]: any };
  private careerPaths: { [id: string]: any };
  private learningFrameworks: { [id: string]: any };
  private developmentalRoles: { [id: string]: any };

  private departmentRouteMap: any = {};

  private adjustFilterApplied: FilterModel;
  private filterParams: UserManagementQueryModel = new UserManagementQueryModel();
  private pendingStatusFilter: string[] = [];

  private currentSortType: string = 'asc';
  private currentFieldSort: string = 'firstName';
  private searchParam: string = stringEmpty;
  private searchKey: string = stringEmpty;
  private OTHER_PLACE_OF_WORK_EXTERNAL_ID: string = 'NewOURequestBucket';

  private defaultStatusFilter: any = [
    StatusTypeEnum.Active.code,
    StatusTypeEnum.New.code,
    StatusTypeEnum.Inactive.code,
    StatusTypeEnum.IdentityServerLocked.code,
    StatusTypeEnum.Archived.code
  ];

  private employeeAvatarFromEmailsMap: any = {};

  private userDepartmentId: number;
  private columnDefCopyToCheckShowHides: ColumDefModel[] = [];
  private userListColumnDef: ColumDefModel[];
  private reloadDataAfterClearSearch: boolean = false;
  public isHideFilterButton: boolean = false;
  public isHideExportButton: boolean = false;
  public isHideColumnButton: boolean = false;
  public isHideCreateUserAccountRequestButton: boolean = false;
  private surveyJSEnumerationVariables: any = [];

  private fromDateIndex: number = 0;
  private toDateIndex: number = 1;

  private gridApi: GridApi;

  private selectedPendingStatusToAction: string =
    StatusTypeEnum.PendingApproval1st.code;
  private isDepartmentFiltered: boolean = false;
  constructor(
    public authService: AuthService,
    private router: Router,
    private userAccountsDataService: UserAccountsDataService,
    private personnelGroupsStoreService: PersonnelGroupsStoreService,
    private careerPathsStoreService: CareerPathsStoreService,
    private developmentalRolesStoreService: DevelopmentalRolesStoreService,
    private learningFrameWorksStoreService: LearningFrameWorksStoreService,
    private pdCatalogueService: PDCatalogueStoreService,
    private formModal: CxFormModal,
    private translateAdapterService: TranslateAdapterService,
    changeDetectorRef: ChangeDetectorRef,
    private ngbModal: NgbModal,
    private toastr: ToastrAdapterService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private globalKeySearchStoreService: GlobalKeySearchStoreService,
    private userGroupsDataService: UserGroupsDataService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private departmentStoreService: DepartmentStoreService,
    private checkingUserRolesService: CheckingUserRolesService,
    private informationDialogService: CxInformationDialogService,
    private systemRolesDataService: SystemRolesDataService,
    private userAccountsHelper: UserAccountsHelper
  ) {
    super(changeDetectorRef, authService);
  }

  async ngOnInit(): Promise<void> {
    this.departmentModel = initUniversalToolbar(this.translateAdapterService);

    this.filterParams.pageSize = this.defaultPageSize;
    this.subscription.add(
      this.globalKeySearchStoreService.get().subscribe((result: any) => {
        if (result) {
          this.searchParam = result.searchKey;
          if (result.isSearch) {
            this.filterParams.pageIndex = 1;
            this.getCurrentTabUserList();
          }
        }
      })
    );

    this.subscription.add(
      this.authService.userData().subscribe(async (user: User) => {
        this.currentUser = user;
        if (this.currentUser) {
          this.userDepartmentId = this.currentUser.departmentId;
          this.departmentModel.currentDepartmentId = UserAccountsHelper.getDefaultRootDepartment(
            this.currentUser
          );
          if (this.userDepartmentId === undefined) {
            return;
          }
          if (this.currentUser.systemRoles) {
            this.divisionBranchSchoolRoles = this.currentUser.systemRoles.filter(
              (systemRole: any) =>
                systemRole.identity.archetype === UserTypeEnum.SystemRole &&
                (systemRole.identity.extId === UserRoleEnum.DivisionAdmin ||
                  systemRole.identity.extId === UserRoleEnum.BranchAdmin ||
                  systemRole.identity.extId === UserRoleEnum.SchoolAdmin)
            );
            this.superAdminRole = this.currentUser.systemRoles.filter(
              (systemRole: any) =>
                systemRole.identity.extId ===
                UserRoleEnum.OverallSystemAdministrator
            );
            this.userAccountAdminRole = this.currentUser.systemRoles.filter(
              (systemRole: any) =>
                systemRole.identity.extId ===
                UserRoleEnum.UserAccountAdministrator
            );
            this.noneAdminRole = this.currentUser.systemRoles.filter(
              (systemRole: any) =>
                systemRole.identity.extId ===
                  UserRoleEnum.SchoolStaffDeveloper ||
                systemRole.identity.extId ===
                  UserRoleEnum.DivisionalLearningCoordinator
            );
          }
          this.filterParams.userEntityStatuses = this.defaultStatusFilter;
          if (!(this.noneAdminRole && this.noneAdminRole.length)) {
            this.pendingStatusFilter.push(
              StatusTypeEnum.PendingApproval3rd.code
            );
            this.pendingStatusFilter.push(
              StatusTypeEnum.PendingApproval2nd.code
            );
            if (
              this.divisionBranchSchoolRoles &&
              this.divisionBranchSchoolRoles.length
            ) {
              this.pendingStatusFilter.push(
                StatusTypeEnum.PendingApproval1st.code
              );
            }
          }
          this.getHierarchicalDepartments(
            this.departmentModel.currentDepartmentId,
            this.initFilterDepartment(),
            true,
            true,
            this.currentUser.departmentId,
            true
          );

          this.initEditForm();
          this.changeDetectorRef.detectChanges();
        }
      })
    );

    this.subscription.add(
      this.personnelGroupsStoreService.get().subscribe((data: SystemRole[]) => {
        if (data && data.length) {
          this.personnelGroups = data;
          this.userAccountsHelper.personnelGroups = data;
        }
      })
    );

    this.subscription.add(
      this.careerPathsStoreService.get().subscribe((data: SystemRole[]) => {
        if (data && data.length) {
          this.careerPaths = data;
          this.userAccountsHelper.careerPaths = data;
        }
      })
    );

    this.subscription.add(
      this.learningFrameWorksStoreService
        .get()
        .subscribe((data: SystemRole[]) => {
          if (data && data.length) {
            this.learningFrameworks = data;
            this.userAccountsHelper.learningFrameworks = data;
          }
        })
    );

    this.subscription.add(
      this.developmentalRolesStoreService
        .get()
        .subscribe((data: SystemRole[]) => {
          if (data && data.length) {
            this.developmentalRoles = data;
            this.userAccountsHelper.developmentalRoles = data;
          }
        })
    );

    await this.initPDCatalogueData();
  }

  onPageSizeChange(pageSize: number): void {
    if (Number(pageSize) > Number(this.filterParams.pageSize)) {
      this.filterParams.pageIndex = 1;
    }
    this.filterParams.pageSize = pageSize;
    this.getUsers();
    window.scroll(0, 0);
  }

  async initPDCatalogueData(): Promise<void> {
    this.surveyJSEnumerationVariables = this.createPDCatalogueEnumerationSurveyJSVariables();

    if (this.surveyJSEnumerationVariables) {
      const servicesSchemeCode = this.surveyJSEnumerationVariables.find(
        (item: any) => item.name === PDCatalogueConstant.ServiceSchemes.code
      ).value;
      this.pdCataloguePersonnelGroups = await this.pdCatalogueService.getPDCatalogueAsync(
        servicesSchemeCode
      );

      const tracksCode = this.surveyJSEnumerationVariables.find(
        (item: any) => item.name === PDCatalogueConstant.Tracks.code
      ).value;
      this.pdCatalogueCareerPaths = await this.pdCatalogueService.getPDCatalogueAsync(
        tracksCode
      );

      const developmentalRoleCode = this.surveyJSEnumerationVariables.find(
        (item: any) => item.name === PDCatalogueConstant.DevelopmentalRoles.code
      ).value;
      this.pdCatalogueDevelopmentalRoles = await this.pdCatalogueService.getPDCatalogueAsync(
        developmentalRoleCode
      );
    }
  }

  onSortTypeChange($event: any): void {
    this.currentSortType = $event.currentSortType;
    this.currentFieldSort = $event.currentFieldSort;
    this.getUsers();
  }

  onSelectedDepartmentClick(selectedDepartment: CxBreadCrumbItem): void {
    this.cxGlobalLoaderService.showLoader();
    if (!this.isDisplayOrganisationNavigation) {
      this.cxGlobalLoaderService.hideLoader();

      return;
    }
    this.clearDepartmentFilter();
    this.resetDepartmentDropdownData(selectedDepartment);
    this.departmentModel.currentDepartmentId = selectedDepartment.identity.id;
    this.filterParams.filterOnSubDepartment = this.isSearchedAcrossSubOrg;
    this.filterParams.userEntityStatuses = this.defaultStatusFilter;
    this.getHierarchicalDepartments(
      this.departmentModel.currentDepartmentId,
      this.initFilterDepartment(),
      true,
      true,
      selectedDepartment.identity.id
    );
    this.changeDetectorRef.detectChanges();
  }

  clearDepartmentFilter(): void {
    this.isDepartmentFiltered = false;
    // Clear all filters whenever user changes the department unit
    this.filterParams = new UserManagementQueryModel();
    this.filterParams.pageIndex = 1;
    if (this.filterDataApplied !== undefined) {
      this.filterDataApplied.appliedFilter = [];
    }
    if (this.adjustFilterApplied !== undefined) {
      this.adjustFilterApplied.appliedFilter = [];
    }
  }

  onSelectDepartmentTree(objectRoute: CxObjectRoute<any>): void {
    this.resetDepartmentDropdownData(objectRoute.object);
    this.clearDepartmentFilter();
    this.departmentModel.currentDepartmentId = objectRoute.object.identity.id;
    this.filterParams.filterOnSubDepartment = this.isSearchedAcrossSubOrg;
    this.filterParams.userEntityStatuses = this.defaultStatusFilter;
    this.getHierarchicalDepartments(
      this.departmentModel.currentDepartmentId,
      this.initFilterDepartment(),
      true,
      true,
      objectRoute.object.identity.id
    );
    this.changeDetectorRef.detectChanges();
  }

  onRemoveEmployees(employees: any[]): void {
    const delayTimeToRemoveEmployee = 1000;
    setTimeout(() => {
      this.clearSelectedItems();
      this.changeDetectorRef.detectChanges();
    }, delayTimeToRemoveEmployee);
  }

  onEditUserClicked(selectedUser: any, isEditNormalUser: boolean = true): void {
    const usersQueryParams = {
      userIds: [selectedUser.identity.id],
      userEntityStatuses: [
        StatusTypeEnum.All.code,
        StatusTypeEnum.Deactive.code
      ],
      getRoles: true,
      getDepartment: true,
      getGroups: true,
      pageIndex: 1,
      pageSize: AppConstant.ItemPerPage
    };
    usersQueryParams.userIds = [selectedUser.identity.id];
    this.cxGlobalLoaderService.showLoader();
    this.subscription.add(
      this.userAccountsDataService
        .getUserInfoWithPost(usersQueryParams)
        .subscribe(
          async (usersResponse: PagingResponseModel<UserManagement>) => {
            if (
              !usersResponse ||
              !usersResponse.items ||
              !usersResponse.items.length
            ) {
              return;
            }
            let userBeingEdited = usersResponse.items[0];
            const isReportingOfficer = selectedUser.systemRoles
              ? selectedUser.systemRoles.filter(
                  (systemRole: any) =>
                    systemRole.identity.extId === UserRoleEnum.ReportingOfficer
                ).length > 0
              : false;
            // TODO: need to move to object util for clone deep
            if (!this.ngbModal.hasOpenModals()) {
              const dataJson: any = this.buildDataJsonForEditUser(
                userBeingEdited
              );
              const userDepartmentTypes = await this.departmentStoreService.getDepartmentTypesByDepartmentIdToPromise(
                userBeingEdited.departmentId
              );
              const userDepartment = await this.departmentStoreService.getDepartmentByIdToPromise(
                userBeingEdited.departmentId
              );
              const isCurrentUserHasPermissionToEdit = this.hasPermissionToEdit();
              const surveyjsVariables = await this.buildSurveyVariablesForEditUser(
                userBeingEdited,
                isEditNormalUser,
                isCurrentUserHasPermissionToEdit,
                userDepartment,
                userDepartmentTypes
              );
              const options = {
                showModalHeader: true,
                modalHeaderText: 'Edit user',
                cancelName: 'Cancel',
                submitName: 'Save',
                variables: surveyjsVariables
              } as CxSurveyjsFormModalOptions;
              const modalRef = this.ngbModal.open(EditUserDialogComponent, {
                windowClass: 'modal-size-xl',
                backdrop: 'static'
              });

              const editUserDialogComponent = modalRef.componentInstance as EditUserDialogComponent;
              editUserDialogComponent.user = userBeingEdited;
              editUserDialogComponent.currentUser = this.currentUser;
              editUserDialogComponent.fullUserInfoJsonData = dataJson;
              editUserDialogComponent.surveyjsOptions = options;
              editUserDialogComponent.isPendingUser = !isEditNormalUser;
              editUserDialogComponent.isCurrentUserHasPermissionToEdit = isCurrentUserHasPermissionToEdit;
              this.subscription.add(
                // tslint:disable-next-line: no-unsafe-any
                editUserDialogComponent.submit.subscribe(
                  (submittedResult: EditUserDialogSubmitModel) => {
                    const editedUser = new EditUser(submittedResult.userData);

                    // TODO: will be implemented later
                    // if (!UserAccountsHelper.isUserEdited(dataJson, editedUser)) {
                    //   this.updateApprovalData(submittedResult.approvalData, userBeingEdited);
                    //   modalRef.close();

                    //   return;
                    // }

                    userBeingEdited = this.userAccountsHelper.processEditUser(
                      userBeingEdited,
                      editedUser,
                      submittedResult,
                      isEditNormalUser
                    );

                    this.onEditUser(
                      submittedResult.approvalData,
                      editedUser.firstName,
                      userBeingEdited,
                      isReportingOfficer,
                      isEditNormalUser
                    );
                    this.isVerticalToShowMenuAction = true;

                    modalRef.close();
                  },
                  (error: HttpErrorResponse) => {
                    this.isVerticalToShowMenuAction = false;
                    this.toastr.error('Update user failed!');
                  }
                )
              );
              this.subscription.add(
                editUserDialogComponent.cancel.subscribe(() => {
                  modalRef.close();
                })
              );
            }

            this.cxGlobalLoaderService.hideLoader();
          },
          () => this.cxGlobalLoaderService.hideLoader()
        )
    );
  }

  isEditActivateDate(userStatus: any): boolean {
    if (
      userStatus === StatusTypeEnum.PendingApproval1st.code ||
      userStatus === StatusTypeEnum.PendingApproval2nd.code ||
      userStatus === StatusTypeEnum.PendingApproval3rd.code ||
      userStatus === StatusTypeEnum.New.code
    ) {
      return true;
    }

    return false;
  }

  removeUserFromApprovalGroup(
    approvalGroup: ApprovalGroup,
    employeeIds: number[]
  ): void {
    if (!approvalGroup) {
      return;
    }
    approvalGroup.entityStatus.statusId = StatusTypeEnum.Deactive.code;
    this.userAccountsDataService
      .removeUsersFromApprovalGroup(
        employeeIds,
        new MembershipDto(approvalGroup)
      )
      .subscribe(
        (response: any) => {
          if (response && response.length === employeeIds.length) {
            response.forEach((element: any) => {
              const index = this.userItemsData.items.findIndex(
                (item: any) => item.identity.id === element.memberId
              );
              if (index < 0) {
                return;
              }

              const removedGroupIndex = this.userItemsData.items[
                index
              ].groups.findIndex(
                (group: any) => group.identity.id === element.groupId
              );
              const currentGroups = this.userItemsData.items[index].groups;
              if (removedGroupIndex > findIndexCommon.notFound) {
                currentGroups.splice(removedGroupIndex);
              }
              this.userItemsData.items[index] = {
                ...this.userItemsData.items[index],
                groups: currentGroups
              };
            });
            this.refreshUserList();
            this.toastr.success(
              `${response.length} user(s) assigned to Approving Officer/Alternate Approving Officer ${approvalGroup.name}.`,
              'Notification'
            );
          }
        },
        (error: any) => {
          this.toastr.error(
            "Something's wrong, please try again later",
            'Alert'
          );
        }
      );
  }

  changeSelectedColumn(selected: any, column: any): void {
    if (column && column.colId) {
      this.showHideColumns = new ShowHideColumnModel({
        column,
        selected
      });
      const columnNeedToHide = this.columnDefCopyToCheckShowHides.find(
        (col) => col.colId === column.colId
      );
      if (columnNeedToHide) {
        columnNeedToHide.hide = !selected;
      }
    }
    this.changeDetectorRef.detectChanges();
  }

  evaluateActionOnUser(
    message: string,
    type: string,
    employee?: any,
    statusReasonId?: string
  ): void {
    const selectedItemMap = employee
      ? employee instanceof Array
        ? employee
        : [employee]
      : this.selectedUser;
    const selectedItemMapLength = selectedItemMap.length;
    const isSystemRoleNull = selectedItemMap.some((x: any) => !x.systemRoles);
    const emptySystemRoleUsers = [];
    selectedItemMap.forEach((item) => {
      if (!item.systemRoles) {
        emptySystemRoleUsers.push(item.firstName);
      }
    });
    if (isSystemRoleNull) {
      this.toastr.warning(
        `${this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Validation.Warning.System_Role_Must_Not_Be_Empty_In_Users'
        )}: ${emptySystemRoleUsers.join(', ')}`
      );

      return;
    }
    const modalRef = this.ngbModal.open(
      UserAccountConfirmationDialogComponent,
      {
        size: 'lg',
        centered: true
      }
    );
    const userAccountConfirmationDialogComponent = modalRef.componentInstance as UserAccountConfirmationDialogComponent;
    let doneButtonText = stringEmpty;
    switch (type) {
      case 'Reject':
        doneButtonText = 'Reject';
        break;
      default:
        doneButtonText = 'Accept';
        break;
    }
    userAccountConfirmationDialogComponent.items = selectedItemMap;
    userAccountConfirmationDialogComponent.doneButtonText = doneButtonText;
    userAccountConfirmationDialogComponent.number = selectedItemMapLength;
    userAccountConfirmationDialogComponent.status = type;

    switch (message) {
      case 'User_Account_Page.User_List.Remove_User_Warning':
        userAccountConfirmationDialogComponent.message =
          selectedItemMap.length > 1
            ? 'User_Account_Page.User_List.Remove_Multiple_User_Warning'
            : message;
        break;

      case 'User_Account_Page.User_List.Archive_User_Warning':
        userAccountConfirmationDialogComponent.message =
          selectedItemMap.length > 1
            ? 'User_Account_Page.User_List.Archive_Multiple_User_Warning'
            : message;
        break;
      case 'User_Account_Page.User_List.UnArchive_User_Warning':
        userAccountConfirmationDialogComponent.message =
          selectedItemMap.length > 1
            ? 'User_Account_Page.User_List.UnArchive_Multiple_User_Warning'
            : message;
        break;
      default:
        userAccountConfirmationDialogComponent.message = message;
        break;
    }

    this.subscription.add(
      userAccountConfirmationDialogComponent.done.subscribe((data: any) => {
        if (!data) {
          modalRef.close();

          return;
        }

        this.processAssigningApprovingOfficers(type, data, selectedItemMap);

        const requestData = [...selectedItemMap];
        if (
          type !== StatusActionTypeEnum.Delete &&
          type !== StatusActionTypeEnum.Archive &&
          type !== StatusActionTypeEnum.Unarchive
        ) {
          this.subscription.add(
            forkJoin(
              requestData.map((emp: any) => {
                let clonedEmployee;
                this.selectedPendingStatusToAction = emp.entityStatus.statusId;
                switch (type) {
                  case StatusTypeEnum.Active.code:
                    clonedEmployee = {
                      ...emp,
                      entityStatus: {
                        ...emp.entityStatus,
                        statusId: StatusTypeEnum.Active.code.toString()
                      }
                    };
                    break;
                  case StatusTypeEnum.Suspended.code:
                    clonedEmployee = {
                      ...emp,
                      entityStatus: {
                        ...emp.entityStatus,
                        statusId: StatusTypeEnum.Inactive.code.toString(),
                        statusReasonId: statusReasonId
                          ? statusReasonId
                          : emp.entityStatus.statusReasonId
                      }
                    };
                    break;
                  case StatusActionTypeEnum.Unlock:
                    clonedEmployee = {
                      ...emp,
                      entityStatus: {
                        ...emp.entityStatus,
                        statusId: StatusTypeEnum.Active.code.toString()
                      }
                    };
                    break;
                  case StatusActionTypeEnum.ResetPassword:
                    clonedEmployee = {
                      ...emp,
                      resetOtp: true
                    };
                    break;
                  case StatusActionTypeEnum.Accept:
                    if (
                      emp.entityStatus.statusId ===
                      StatusTypeEnum.PendingApproval1st.code
                    ) {
                      clonedEmployee = {
                        ...emp,
                        entityStatus: {
                          ...emp.entityStatus,
                          statusId: StatusTypeEnum.PendingApproval2nd.code.toString()
                        }
                      };
                    } else if (
                      emp.entityStatus.statusId ===
                        StatusTypeEnum.PendingApproval2nd.code ||
                      emp.entityStatus.statusId ===
                        StatusTypeEnum.PendingApproval3rd.code
                    ) {
                      if (data.finalApprovalInfo) {
                        const activationDate = DateTimeUtil.surveyToDateLocalTimeISO(
                          data.finalApprovalInfo.activeDate
                        );
                        const expirationDate = DateTimeUtil.surveyToEndDateLocalTimeISO(
                          data.finalApprovalInfo.expirationDate
                        );

                        clonedEmployee = {
                          ...emp,
                          resetOtp: true,
                          ...emp,
                          entityStatus: {
                            ...emp.entityStatus,
                            statusId: StatusTypeEnum.New.code.toString()
                          }
                        };

                        if (
                          data.finalApprovalInfo.setDateOption ===
                          'forSelectedUsers'
                        ) {
                          clonedEmployee.entityStatus.activeDate = activationDate;
                          clonedEmployee.entityStatus.expirationDate = expirationDate;
                        } else {
                          // Set if missing only.
                          if (!clonedEmployee.entityStatus.activeDate) {
                            clonedEmployee.entityStatus.activeDate = activationDate;
                          }
                          if (!clonedEmployee.entityStatus.expirationDate) {
                            clonedEmployee.entityStatus.expirationDate = expirationDate;
                          }
                        }
                      }
                    }
                    break;
                  case StatusTypeEnum.Deleted.code:
                    clonedEmployee = {
                      ...emp,
                      entityStatus: {
                        ...emp.entityStatus,
                        statusId: StatusTypeEnum.Deactive.code.toString(),
                        statusReasonId: StatusReasonTypeConstant.ManuallySetDeactive.code.toString()
                      }
                    };
                    break;
                  case StatusTypeEnum.Archived.code:
                    clonedEmployee = {
                      ...emp,
                      entityStatus: {
                        ...emp.entityStatus,
                        statusId: StatusTypeEnum.Archived.toString(),
                        statusReasonId: StatusReasonTypeConstant.Archived_ManuallyArchived.code.toString()
                      }
                    };
                  case StatusActionTypeEnum.Reject:
                    const statusId = emp.entityStatus.statusId;
                    clonedEmployee = {
                      ...emp,
                      entityStatus: {
                        ...emp.entityStatus,
                        statusId: StatusTypeEnum.Rejected.code.toString(),
                        statusReasonId:
                          statusId === StatusTypeEnum.PendingApproval1st.code
                            ? StatusReasonTypeConstant.ManuallyRejectedPending1st.code.toString()
                            : statusId ===
                              StatusTypeEnum.PendingApproval2nd.code
                            ? StatusReasonTypeConstant.ManuallyRejectedPending2nd.code.toString()
                            : StatusReasonTypeConstant.ManuallyRejectedPending3rd.code.toString()
                      },
                      jsonDynamicAttributes: {
                        ...emp.jsonDynamicAttributes,
                        rejectReason: data.rejectReason
                      }
                    };
                    break;
                  case StatusActionTypeEnum.SetExpirationDate:
                    clonedEmployee = {
                      ...emp,
                      entityStatus: {
                        ...emp.entityStatus,
                        expirationDate: DateTimeUtil.surveyToEndDateLocalTimeISO(
                          data.date
                        )
                      }
                    };
                    break;
                  case StatusActionTypeEnum.RequestSpecialApproval:
                    clonedEmployee = {
                      ...emp,
                      entityStatus: {
                        ...emp.entityStatus,
                        statusId: StatusTypeEnum.PendingApproval3rd.code.toString()
                      }
                    };
                    break;
                  default:
                    clonedEmployee = { ...emp };
                    break;
                }
                // The executing action doesn't change the system roles of the user.
                // In order to prevent unexpectedly changing of the system roles, they should be removed from the PAYLOAD.
                delete clonedEmployee.systemRoles;

                return this.userAccountsDataService.changeEmployeeStatus(
                  clonedEmployee
                );
              })
            ).subscribe(
              (response: any) => {
                if (response) {
                  this.handleUserResponseData(response, type, statusReasonId);
                  this.toastr.success(
                    `${response.length} user(s) updated successfully.`
                  );
                }
              },
              (error: HttpErrorResponse) => {
                this.toastr.error("Updated user(s)' statuses failed");
              }
            )
          );
        } else {
          switch (type) {
            case StatusActionTypeEnum.Delete: {
              this.processDeleteUsers(requestData);
              break;
            }
            case StatusActionTypeEnum.Archive: {
              this.processArchiveUsers(requestData);
              break;
            }
            case StatusActionTypeEnum.Unarchive: {
              this.processUnarchiveUsers(requestData);
              break;
            }
            default:
              break;
          }
        }
        this.isVerticalToShowMenuAction = true;
        modalRef.close();
      })
    );
    this.subscription.add(
      userAccountConfirmationDialogComponent.cancel.subscribe((data: any) => {
        modalRef.close();
      })
    );
  }

  onCreateNewUser(): void {
    if (this.formModal.hasOpenModals()) {
      return;
    }

    const newUserDepartmentId = this.departmentModel.currentDepartmentId;
    this.cxGlobalLoaderService.showLoader();
    this.subscription.add(
      this.departmentStoreService
        .getDepartmentById(newUserDepartmentId)
        .subscribe(async (newUserDepartment) => {
          const newUserDepartmentTypes = await this.departmentStoreService.getDepartmentTypesByDepartmentIdToPromise(
            newUserDepartmentId
          );
          const surveyjsVariables = await this.buildSurveyVariablesForCreateNewUser(
            newUserDepartment,
            newUserDepartmentTypes
          );
          this.cxGlobalLoaderService.hideLoader();
          const options = {
            showModalHeader: true,
            modalHeaderText: 'Create user account request',
            cancelName: 'Cancel',
            submitName: 'Ok',
            variables: surveyjsVariables
          } as CxSurveyjsFormModalOptions;
          const modalRef = this.ngbModal.open(EditUserDialogComponent, {
            windowClass: 'modal-size-xl',
            backdrop: 'static'
          });

          const editUserDialogComponent = modalRef.componentInstance as EditUserDialogComponent;
          editUserDialogComponent.isCurrentUserHasPermissionToEdit = true;
          editUserDialogComponent.fullUserInfoJsonData = {
            gender: GenderEnum.Male,
            systemRoles: DefaultSystemRoleData,
            departmentId: newUserDepartmentId
          };
          editUserDialogComponent.surveyjsOptions = options;
          this.subscription.add(
            // tslint:disable-next-line: no-unsafe-any
            editUserDialogComponent.submit.subscribe(
              async (submitData: EditUserDialogSubmitModel) => {
                if (submitData) {
                  const newUserDTO: Partial<UserManagement> = this.buildNewUserDtoFromSubmittedForm(
                    submitData.userData
                  );
                  if (newUserDTO.systemRoles) {
                    const existLearnerRole =
                      newUserDTO.systemRoles.findIndex(
                        (role: any) =>
                          role.identity.extId === UserRoleEnum.Learner
                      ) > findIndexCommon.notFound;
                    if (existLearnerRole) {
                      newUserDTO.jsonDynamicAttributes.finishOnBoarding = false;
                    }
                  }
                  this.subscription.add(
                    this.userAccountsDataService
                      .createUser(newUserDTO)
                      .subscribe(
                        async (newUser: any) => {
                          const isSysAdminOrAccountAdmin =
                            this.userAccountAdminRole.length > 0 ||
                            this.superAdminRole.length > 0;
                          if (isSysAdminOrAccountAdmin) {
                            if (
                              await this.isDepartmentInCurrentList(
                                newUser.departmentId
                              )
                            ) {
                              this.userItemsData.items.unshift(newUser);
                              this.userItemsData.items = [
                                ...this.userItemsData.items
                              ];
                            }
                          } else {
                            if (
                              this.pending2ndLevelApprovalList &&
                              this.departmentModel.currentDepartmentId ===
                                newUser.departmentId
                            ) {
                              this.pending2ndLevelApprovalList.unshift(newUser);
                              this.pending2ndLevelApprovalList = [
                                ...this.pending2ndLevelApprovalList
                              ];
                            }
                          }

                          this.employeeAvatarFromEmailsMap[
                            newUser.identity.id
                          ] = this.getUserImage(newUser);
                          this.changeDetectorRef.detectChanges();
                          this.toastr.success(
                            `User ${newUser.firstName} has been created successfully`
                          );
                          if (
                            newUser.systemRoles &&
                            newUser.systemRoles.findIndex(
                              (role: any) =>
                                role.identity.extId ===
                                UserRoleEnum.ReportingOfficer
                            ) > findIndexCommon.notFound
                          ) {
                            this.createApprovalGroup(newUser);
                          }
                          this.initUserActionsListBasedOnRoles();
                        },
                        (error: any) => {
                          this.toastr.error(
                            error.error.Message,
                            'An error occured when creating new user.'
                          );
                        }
                      )
                  );
                }
                modalRef.close();
              },
              (error: any) => {
                this.toastr.error(
                  error.error.Message,
                  'An error occured when creating new user.'
                );
              }
            )
          );
          this.subscription.add(
            editUserDialogComponent.cancel.subscribe(() => {
              modalRef.close();
            })
          );
        })
    );
  }

  onAssignReportingOfficerClicked(): void {
    const modalRef = this.ngbModal.open(AssignAODialogComponent, {
      size: 'lg',
      centered: true
    });
    const assignAODialogComponent = modalRef.componentInstance as AssignAODialogComponent;
    assignAODialogComponent.itemsSelected = this.selectedUser;
    assignAODialogComponent.department = this.currentDepartment;
    this.subscription.add(
      assignAODialogComponent.done.subscribe((data: any) => {
        if (!data) {
          return;
        }
        const employeeIds = data.itemsSelected.map((item: any) => {
          return item.identity.id;
        });
        if (data.primaryApprovalGroup) {
          this.addUserToApprovalGroup(data.primaryApprovalGroup, employeeIds);
        } else {
          data.itemsSelected.forEach((currentSelectedUser) => {
            const currentPrimaryGroup =
              currentSelectedUser.groups &&
              currentSelectedUser.groups.find(
                (group: any) =>
                  group.type === ApprovalGroupTypeEnum.PrimaryApprovalGroup
              );

            if (currentPrimaryGroup) {
              this.removeUserFromApprovalGroup(currentPrimaryGroup, [
                currentSelectedUser.identity.id
              ]);
            }
          });
        }
        if (data.alternateApprovalGroup) {
          this.addUserToApprovalGroup(data.alternateApprovalGroup, employeeIds);
        } else {
          data.itemsSelected.forEach((currentSelectedUser) => {
            const currentAlternativeApprovalGroup =
              currentSelectedUser.groups &&
              currentSelectedUser.groups.find(
                (group: any) =>
                  group.type === ApprovalGroupTypeEnum.AlternativeApprovalGroup
              );

            if (currentAlternativeApprovalGroup) {
              this.removeUserFromApprovalGroup(
                currentAlternativeApprovalGroup,
                [currentSelectedUser.identity.id]
              );
            }
          });
        }

        this.isVerticalToShowMenuAction = true;
        modalRef.close();
      })
    );
    this.subscription.add(
      assignAODialogComponent.cancel.subscribe((data: any) => {
        modalRef.close();
      })
    );
  }

  onSuspend(employee?: UserManagement): void {
    const dataJson = {
      users: employee ? [employee] : this.selectedUser
    };
    const areAllSelectedUsersActive = dataJson.users.every(
      (user: UserManagement) =>
        user.entityStatus.statusId === StatusTypeEnum.Active.code ||
        user.entityStatus.statusId === StatusTypeEnum.New.code
    );
    if (!areAllSelectedUsersActive) {
      this.toastr.warning(
        this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Validation.Warning.Change_Status_Action.Cannot_Suspend_User'
        )
      );
    }
    if (!this.formModal.hasOpenModals() && areAllSelectedUsersActive) {
      const surveyjsVariables = [
        new CxSurveyjsVariable({
          name: 'selectedUserCount',
          value: dataJson.users.length
        })
      ];
      const options = {
        showModalHeader: true,
        modalHeaderText: 'Reason For Suspension',
        cancelName: 'Cancel',
        submitName: 'OK',
        variables: surveyjsVariables
      } as CxSurveyjsFormModalOptions;

      const form = SuspensionReasonFormJSON;

      // Open modal.
      const modalRef = this.formModal.openSurveyJsForm(
        form,
        dataJson,
        [],
        options,
        { size: 'lg', backdrop: 'static' }
      );
      const submitObserver = this.subscription.add(
        this.formModal.submit.subscribe(
          (submitData: any) => {
            if (submitData) {
              this.evaluateActionOnUser(
                'User_Account_Page.User_List.Change_User_Status_Warning',
                StatusTypeEnum.Suspended.code,
                employee,
                submitData.suspensionReason
              );
              this.isVerticalToShowMenuAction = true;
            }
            modalRef.close();
          },
          (error: any) => {
            this.isVerticalToShowMenuAction = false;
            this.toastr.error(
              error.error.Message,
              'An error occurred when suspending user account.'
            );
          }
        )
      );
      modalRef.result
        .then((res: any) => {
          submitObserver.unsubscribe();
        })
        .catch((err: any) => {
          submitObserver.unsubscribe();
        });
    }
  }

  onAssignUserGroupClicked(): void {
    if (!this.formModal.hasOpenModals()) {
      const users = this.selectedUser;
      users.forEach((user: any) => {
        user.avatarUrl = this.employeeAvatarFromEmailsMap[user.identity.id];
        user.firstName = CrossSiteScriptingUtil.encodeHtmlEntity(
          user.firstName
        );
      });
      const dataJson = {
        users
      };

      const surveyjsVariables = [
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.currentDepartment_id,
          value: this.departmentModel.currentDepartmentId
        }),
        new CxSurveyjsVariable({
          name: 'selectedUserCount',
          value: dataJson.users.length
        }),
        new CxSurveyjsVariable({
          name: 'replaceTS',
          value: Math.random().toString()
        })
      ];
      const options = {
        showModalHeader: true,
        fixedButtonsFooter: true,
        modalHeaderText: 'Please select a user group',
        cancelName: 'Cancel',
        submitName: 'Confirm',
        variables: surveyjsVariables
      } as CxSurveyjsFormModalOptions;
      const form = AddMemberToGroupFormJSON;

      // Open modal.
      const modalRef = this.formModal.openSurveyJsForm(
        form,
        dataJson,
        [],
        options,
        { size: 'lg', backdrop: 'static' }
      );
      const submitObserver = this.subscription.add(
        this.formModal.submit.subscribe(
          (submitData: any) => {
            if (submitData) {
              if (!submitData.users) {
                this.toastr.warning(
                  this.translateAdapterService.getValueImmediately(
                    'User_Account_Page.Validation.Warning.Add_Member_To_Group_Without_User_Selected'
                  )
                );
                modalRef.close();

                return;
              }
              const selectedUsers: MembershipDto[] = submitData.users;
              const newMembers: MembershipDto[] = selectedUsers.map(
                (user: MembershipDto) => {
                  return this.buildMemberDtoFromSubmittedForm(
                    user,
                    submitData.userGroup.identity.id
                  );
                }
              );
              this.subscription.add(
                this.userGroupsDataService
                  .addMembersToUserGroup(
                    submitData.userGroup.identity.id,
                    newMembers
                  )
                  .subscribe(
                    (newCreatedMembers: any) => {
                      this.toastr.success(
                        `${newCreatedMembers.length} user account(s) has been added into group '${submitData.userGroup.name}'`
                      );
                    },
                    (error: any) => {
                      this.toastr.error(
                        error.error.Message,
                        'An error occurred when adding user accounts to group.'
                      );
                    }
                  )
              );
              this.isVerticalToShowMenuAction = true;
            }
            modalRef.close();
          },
          (error: any) => {
            this.isVerticalToShowMenuAction = false;
            this.toastr.error(
              error.error.Message,
              'An error occurred when adding user accounts to group.'
            );
          }
        )
      );
      modalRef.result
        .then((res: any) => {
          submitObserver.unsubscribe();
        })
        .catch((err: any) => {
          submitObserver.unsubscribe();
        });
    }
  }

  onUserAccountPageChange(pageIndex: number): void {
    this.filterParams.pageIndex = pageIndex;
    this.getUsers();
    window.scroll(0, 0);
  }

  onFilterButtonClick(): void {
    const modalRef = this.ngbModal.open(UserFilterComponent, {
      size: 'lg',
      backdrop: 'static',
      centered: true,
      windowClass: 'filter-dialog-custom-size'
    });
    const userFilterComponentDialog = modalRef.componentInstance as UserFilterComponent;
    userFilterComponentDialog.adjustAppliedData =
      this.adjustFilterApplied &&
      this.adjustFilterApplied.appliedFilter !== undefined &&
      this.adjustFilterApplied.appliedFilter.length > 0
        ? { ...this.adjustFilterApplied }
        : null;
    const pdCatalogueVariables = this.createPDCatalogueEnumerationSurveyJSVariables();
    const organisationUnitTypes = this.createOrganizationUnitTypesSurveyJSVariables();
    const fromDepartmentVariable = new CxSurveyjsVariable({
      name: 'fromDepartmentId',
      value: UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
    });

    userFilterComponentDialog.filterVariables = pdCatalogueVariables
      .concat(organisationUnitTypes)
      .concat([fromDepartmentVariable]);
    this.changeDetectorRef.detectChanges();
    this.subscription.add(
      userFilterComponentDialog.applyClick.subscribe(
        (appliedData: { filteredData: FilterModel }) => {
          this.adjustFilterApplied = _.cloneDeep(appliedData.filteredData);
          this.filterDataApplied = _.cloneDeep(appliedData.filteredData);
          this.updateQueryParams(appliedData.filteredData);

          if (
            !this.isDepartmentFiltered ||
            this.filterParams.parentDepartmentId === undefined
          ) {
            this.filterParams.parentDepartmentId = [
              this.departmentModel.currentDepartmentId
            ];
          }

          this.loadUsersDataAccordingToCurrentTab();

          this.changeDetectorRef.detectChanges();
          modalRef.close();
        }
      )
    );
    this.subscription.add(
      userFilterComponentDialog.cancel.subscribe(() => {
        modalRef.close();
      })
    );
  }

  getSuspendedStatusReason(statusReasonId: string): string {
    switch (statusReasonId) {
      case StatusReasonTypeConstant.ManuallyInactiveAbsenceMoreThan90Days.code:
        return StatusReasonTypeConstant.ManuallyInactiveAbsenceMoreThan90Days
          .text;
      case StatusReasonTypeConstant.ManuallyInactiveResignation.code:
        return StatusReasonTypeConstant.ManuallyInactiveResignation.text;
      case StatusReasonTypeConstant.ManuallyInactiveRetirement.code:
        return StatusReasonTypeConstant.ManuallyInactiveRetirement.text;
      case StatusReasonTypeConstant.ManuallyInactiveTermination.code:
        return StatusReasonTypeConstant.ManuallyInactiveTermination.text;
      case StatusReasonTypeConstant.ManuallyInactiveLeftWithoutAdvanceNotice
        .code:
        return StatusReasonTypeConstant.ManuallyInactiveLeftWithoutAdvanceNotice
          .text;
      default:
        return stringEmpty;
    }
  }

  onExportUser(isExportAll: boolean = false): void {
    if (!isExportAll && this.selectedUser && this.selectedUser.length === 0) {
      this.toastr.warning(
        this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Export_User.Not_Selected_Users'
        )
      );

      return;
    }

    const modalRef = this.ngbModal.open(UserExportComponent, {
      windowClass: 'modal-size-xl',
      backdrop: 'static'
    });

    const exportModalRef = modalRef.componentInstance as UserExportComponent;
    exportModalRef.appliedFilterData = {
      ...this.filterParams
    };

    if (!isExportAll) {
      exportModalRef.selectedItems = this.selectedUser;
    }
    this.subscription.add(
      exportModalRef.completeExport.subscribe(() => {
        modalRef.close();
      })
    );
    this.subscription.add(
      exportModalRef.cancel.subscribe(() => {
        modalRef.close();
      })
    );
  }

  openDialogToEnableColumns(): void {
    const modalRef = this.ngbModal.open(UserShowHideComponent, {
      size: 'sm',
      backdrop: 'static',
      centered: true
    });
    const instanceComponent = modalRef.componentInstance as UserShowHideComponent;
    const showHideColumns = _.cloneDeep(
      this.userListColumnDef.filter((item, index) => index !== 0)
    );

    showHideColumns.forEach((showHideColumn) => {
      const hiddenColumn = this.columnDefCopyToCheckShowHides.find(
        (col) => col.colId === showHideColumn.colId
      );
      if (hiddenColumn) {
        showHideColumn.hide = hiddenColumn.hide;
      }
    });
    instanceComponent.userListColumnDef = showHideColumns;

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

  updateQueryParams(tagGroupChanged: FilterModel): void {
    this.filterParams = new UserManagementQueryModel();
    this.filterParams.pageIndex = 1;
    this.filterParams.filterOnSubDepartment = this.isSearchedAcrossSubOrg;
    this.isDepartmentFiltered =
      tagGroupChanged.appliedFilter !== undefined &&
      tagGroupChanged.appliedFilter.find(
        (f) => f.filterOptions.data === GroupFilterConst.ORGANISATION_UNIT
      ) !== undefined;
    if (tagGroupChanged.appliedFilter === undefined) {
      return;
    }
    let multiUserTypeExtIdFilterIndex = 0;
    tagGroupChanged.appliedFilter.forEach((item: any) => {
      const filterCondition = item.data.value;
      switch (item.filterOptions.data) {
        case GroupFilterConst.STATUS:
          this.filterParams.userEntityStatuses = filterCondition;
          break;
        case GroupFilterConst.AGE_GROUP:
          this.filterParams.ageRanges = filterCondition;
          break;
        case GroupFilterConst.USER_GROUP:
          this.filterParams.userGroupIds = filterCondition;
          break;
        case GroupFilterConst.ROLE:
          this.filterParams.multiUserTypefilters[0] = filterCondition;
          break;
        case GroupFilterConst.ACCOUNT_TYPE:
          const selectedFullyUserAccountType = 2;
          if (
            Array.isArray(filterCondition) &&
            filterCondition.length !== selectedFullyUserAccountType
          ) {
            this.filterParams.externallyMastered =
              filterCondition[0] === UserEntityStatusEnum.ManualUserAccount
                ? false
                : true;
          }
          break;
        case GroupFilterConst.TYPE_OF_OU:
          this.filterParams.orgUnittypeIds.push(filterCondition);
          break;
        case GroupFilterConst.CREATION_DATE:
          this.filterParams.createdBefore = filterCondition[this.toDateIndex];
          this.filterParams.createdAfter = filterCondition[this.fromDateIndex];
          break;
        case GroupFilterConst.EXPIRATION_DATE:
          this.filterParams.expirationDateBefore =
            filterCondition[this.toDateIndex];
          this.filterParams.expirationDateAfter =
            filterCondition[this.fromDateIndex];
          break;
        case GroupFilterConst.SERVICE_SCHEME:
          this.filterParams.multiUserTypeExtIdFilters[
            multiUserTypeExtIdFilterIndex
          ].push(filterCondition);
          multiUserTypeExtIdFilterIndex++;
          break;
        case GroupFilterConst.TEACHING_SUBJECTS:
          this.filterParams.jsonDynamicData.push(
            `$.teachingSubjects[]=${filterCondition.map(
              (data: any) => data.id
            )}`
          );
          break;
        case GroupFilterConst.JOB_FAMILY:
          this.filterParams.jsonDynamicData.push(
            `$.jobFamily[]=${filterCondition.map((data: any) => data.id)}`
          );
          break;
        case GroupFilterConst.DEVELOPMENTAL_ROLE:
          this.filterParams.multiUserTypeExtIdFilters[
            multiUserTypeExtIdFilterIndex
          ].push(filterCondition);
          multiUserTypeExtIdFilterIndex++;
          break;
        case GroupFilterConst.DESIGNATION:
          this.filterParams.jsonDynamicData.push(
            `$.designation=${filterCondition.map((data: any) => data.id)}`
          );
          break;
        case GroupFilterConst.ORGANISATION_UNIT:
          this.filterParams.parentDepartmentId = [];
          for (const filter of filterCondition) {
            this.filterParams.parentDepartmentId.push(filter.identity.id);
          }
          break;
        default:
          break;
      }
    });
  }

  searchSubOrg(isSearchSubOrg: boolean): void {
    if (isSearchSubOrg === undefined) {
      return;
    }
    this.filterParams.filterOnSubDepartment = isSearchSubOrg;
    this.filterParams.pageIndex = 1;
    this.isSearchedAcrossSubOrg = isSearchSubOrg;
    if (isSearchSubOrg) {
      this.cxGlobalLoaderService.showLoader();

      this.getUserFromApi();
    } else {
      this.getUsers();
    }
  }

  onSearch(searchKey: any): void {
    if (searchKey !== stringEmpty) {
      this.departmentModel.isShowSearchResult = true;
      this.departmentModel.isNoSearchResult = false;
      this.searchKey = searchKey;
      this.cxGlobalLoaderService.showLoader();
      this.subscription.add(
        this.userAccountsDataService
          .getHierarchicalDepartments(
            UserAccountsHelper.getDefaultRootDepartment(this.currentUser),
            this.initFilterDepartment(this.searchKey)
          )
          .subscribe((response: Department[]) => {
            //When user search by text, don't know the id of department parent( can't build crumb, can't build tree)
            this.departmentModel.departments = _.uniqBy(
              this.departmentModel.departments.concat(response),
              'identity.id'
            );
            this.departmentModel.searchDepartmentResult = UserAccountsHelper.searchDepartments(
              response,
              this.searchKey
            );
            this.departmentModel.departmentPathMap = [];
            this.departmentModel.searchDepartmentResult.forEach(
              (element: any) => {
                this.departmentModel.departmentPathMap[
                  element.identity.id
                ] = this.getPathOfDepartment(response, element);
                const routeDepartment = {};
                this.getRouteOfDepartment(response, element, routeDepartment);
                this.departmentRouteMap[element.identity.id] = routeDepartment;
              }
            );
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
      this.getHierarchicalDepartments(
        UserAccountsHelper.getDefaultRootDepartment(this.currentUser),
        this.initFilterDepartment()
      );
    }
  }

  onClickSearchResult(department: any): void {
    const objectRoute = {
      route: this.departmentRouteMap[department.identity.id],
      object: department
    };
    this.departmentModel.isShowSearchResult = false;
    this.departmentModel.searchDepartmentResult = [];
    this.departmentModel.isDetectExpandTree = true;
    this.changeDetectorRef.detectChanges();
    this.onSelectDepartmentTree(objectRoute);
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

  checkPendingUsersLevelHasDataFnCreator(
    pendingStatus: string
  ): () => Observable<number> {
    const pendingFilterParam = new UserManagementQueryModel();
    pendingFilterParam.userEntityStatuses = [pendingStatus];
    pendingFilterParam.orderBy = 'firstName Asc';
    pendingFilterParam.parentDepartmentId = [
      UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
    ];
    pendingFilterParam.filterOnSubDepartment = true;
    // pendingFilterParam.searchKey = this.searchParam;
    pendingFilterParam.pageSize = 10000;

    return () =>
      this.userAccountsDataService
        .getUsers(pendingFilterParam)
        .pipe(map((pendingUsersResponse) => pendingUsersResponse.items.length));
  }

  checkPendingUsersOtherPlaceOfWorkHasDataFnCreator(): () => Observable<number> {
    if (
      this.adjustFilterApplied &&
      !isEmpty(this.adjustFilterApplied.appliedFilter)
    ) {
      return () =>
        from(this.getListLearnerOtherPlaceOfWork('', null)).pipe(
          map((learners) => learners.length)
        );
    } else {
      return () =>
        from(this.getListLearnerOtherPlaceOfWork('')).pipe(
          map((learners) => learners.length)
        );
    }
  }

  onActionChanged($event: UserActionsModel): void {
    this.userActions = $event;
    const totalItems =
      $event.listNonEssentialActions.length + $event.listSpecifyActions.length;
    const maximumItems = 7;
    this.isVerticalToShowMenuAction = totalItems > maximumItems ? false : true;
    this.changeDetectorRef.detectChanges();
  }

  onExportButtonActionChanged($event: ActionsModel[]): void {
    this.userActionsForExportButton = $event;
  }

  onCreateAccButtonActionChanged($event: ActionsModel[]): void {
    this.userActionsForCreateAccButton = $event;
  }

  onSingleActionClicked($event: any): void {
    if ($event && $event.action) {
      switch ($event.action.actionType) {
        case StatusActionTypeEnum.Suspend:
          this.onSuspend($event.item);
          break;
        case StatusActionTypeEnum.SetApprovingOfficers:
          this.onAssignReportingOfficerClicked();
          break;
        case StatusActionTypeEnum.AddToGroup:
          this.onAssignUserGroupClicked();
          break;
        case StatusActionTypeEnum.Edit:
          this.onEditUserClicked($event.item);
          break;
        default:
          this.evaluateActionOnUser(
            $event.action.message,
            $event.action.actionType,
            $event.item
          );
          break;
      }
    }
  }

  onSelectedUserChange($event: UserManagement[]): void {
    this.selectedUser = $event;
    this.isClearSelected = false;
    this.updateUserActionBaseOnGridSelection();
    this.changeDetectorRef.detectChanges();
  }

  onMenuActionClick($event: ActionsModel): void {
    if ($event) {
      switch ($event.actionType) {
        case StatusActionTypeEnum.Suspend:
          this.onSuspend();
          break;
        case StatusActionTypeEnum.SetApprovingOfficers:
          this.onAssignReportingOfficerClicked();
          break;
        case StatusActionTypeEnum.AddToGroup:
          this.onAssignUserGroupClicked();
          break;
        case StatusActionTypeEnum.CreateAccount:
          this.onCreateNewUser();
          break;
        case StatusActionTypeEnum.MassCreateAccount:
          this.onMassCreateUsersClicked();
          break;
        case StatusActionTypeEnum.Export:
          this.onExportUser();
          break;
        case StatusActionTypeEnum.ExportAll:
          const exportAll = true;
          this.onExportUser(exportAll);
          break;
        case StatusActionTypeEnum.ShowHideColumn:
          this.openDialogToEnableColumns();
          break;
        case StatusActionTypeEnum.GenerateAccountReviewReport:
          this.onGenerateReport();
          break;
        case StatusActionTypeEnum.ChangeUserPlaceOfWork:
          this.onChangeUserPlaceOfWork();
          break;
        case StatusActionTypeEnum.CreateNewOrgUnit:
          this.onClickCreateNewOrgUnit();
          break;
        default:
          this.evaluateActionOnUser(
            $event.message,
            $event.actionType,
            this.selectedUser
          );
          break;
      }
    }
  }

  async onClickCreateNewOrgUnit(): Promise<void> {
    await this.informationDialogService.info({
      message: this.translateAdapterService.getValueImmediately(
        'User_Account_Page.Change_User_Place_of_Work.Message.Create_Org_Unit_Hint'
      )
    });
    this.router.navigate([AppConstant.siteURL.menus.organization]);
  }

  onChangeUserPlaceOfWork(): void {
    const selectedUsers = Utils.cloneDeep(this.selectedUser);
    if (!this.formModal.hasOpenModals()) {
      // Prevent moving yourself to other Place of Work.
      const selectedUserIndex = selectedUsers.findIndex(
        (user) => user.identity.id === this.currentUser.identity.id
      );
      if (selectedUserIndex > findIndexCommon.notFound) {
        selectedUsers.splice(selectedUserIndex, 1);
        this.toastr.warning(
          this.translateAdapterService.getValueImmediately(
            'User_Account_Page.Change_User_Place_of_Work.Validation.Move_Yourself'
          )
        );
        if (!selectedUsers.length) {
          return;
        }
      }
      const users = selectedUsers.filter(
        (selectedUser) =>
          !selectedUser.entityStatus ||
          selectedUser.entityStatus.externallyMastered === false
      );
      // Prevent moving externally user accounts.
      const externallyUsers = _.differenceBy(
        selectedUsers,
        users,
        'identity.id'
      );
      if (externallyUsers.length > 0) {
        this.toastr.warning(
          this.translateAdapterService.getValueImmediately(
            'User_Account_Page.Change_User_Place_of_Work.Validation.Move_Externally__User_Count',
            {
              numberOfInvalidUsers: externallyUsers.length
            }
          )
        );

        if (!users.length) {
          return;
        }
      }

      users.forEach((user) => {
        user.avatarUrl = user.avatarUrl
          ? user.avatarUrl
          : this.employeeAvatarFromEmailsMap[user.identity.id];
        user.firstName = CrossSiteScriptingUtil.encodeHtmlEntity(
          user.firstName
        );
      });
      const dataJson = {
        users
      };

      const surveyjsVariables = [
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.currentDepartment_id,
          value: this.departmentModel.currentDepartmentId
        }),
        new CxSurveyjsVariable({
          name: 'selectedUserCount',
          value: dataJson.users.length
        }),
        new CxSurveyjsVariable({
          name: 'fromDepartmentId',
          value: UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
        })
      ];
      const options = {
        showModalHeader: true,
        fixedButtonsFooter: true,
        modalHeaderText: this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Change_User_Place_of_Work.Dialog.Header'
        ),
        cancelName: this.translateAdapterService.getValueImmediately(
          'Common.Button.Cancel'
        ),
        submitName: this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Change_User_Place_of_Work.Dialog.Confirm_Button'
        ),
        variables: surveyjsVariables
      } as CxSurveyjsFormModalOptions;

      // Open modal.
      const modalRef = this.formModal.openSurveyJsForm(
        moveUserAccountFormJSON,
        dataJson,
        [],
        options,
        { size: 'lg', backdrop: 'static' }
      );
      const submitObserver = this.subscription.add(
        this.formModal.submitting.subscribe(
          (surveyEvent: CxSurveyjsEventModel) => {
            surveyEvent.options.allowComplete = false; // Prevent submitting the form which will remove all the fields.
            const submitData = surveyEvent.survey.data;
            const selectedUsers: UserManagement[] = submitData.users;
            if (!selectedUsers || selectedUsers.length === 0) {
              this.toastr.warning(
                this.translateAdapterService.getValueImmediately(
                  'User_Account_Page.Change_User_Place_of_Work.Validation.No_User_Selected'
                )
              );
              modalRef.close();

              return;
            }
            const newDepartmentId = submitData.departmentId;
            const usersWithSameDestination = selectedUsers.filter(
              (u) => u.departmentId === newDepartmentId
            );
            const validUsers = _.clone(
              selectedUsers.filter((u) => u.departmentId !== newDepartmentId)
            );

            if (!validUsers || validUsers.length === 0) {
              this.toastr.warning(
                this.translateAdapterService.getValueImmediately(
                  'User_Account_Page.Change_User_Place_of_Work.Validation.All_User_Have_Same_New_Destination'
                )
              );

              return;
            }

            this.cxGlobalLoaderService.showLoader();
            // TODO: Call the new API endpoint to handle moving users to another department.
            this.departmentStoreService
              .getDepartmentTypesByDepartmentId(newDepartmentId)
              .subscribe((departmentTypes) => {
                const availableSystemRoleExtIdsInDepartment = this.cxSurveyjsExtendedService.buildAvailableRolesByDepartmentTypes(
                  departmentTypes
                );
                this.systemRolesDataService
                  .getSystemRoles()
                  .subscribe((allSystemRoles) => {
                    const availableSystemRolesInDepartment = allSystemRoles.filter(
                      (role) =>
                        availableSystemRoleExtIdsInDepartment.includes(
                          role.identity.extId
                        )
                    );

                    // Set new department for each user and map the existing roles to new roles in the new department.
                    validUsers.forEach((validUser) => {
                      validUser.departmentId = newDepartmentId;
                      validUser.systemRoles = UserAccountsHelper.buildNewSystemRoles(
                        validUser.systemRoles,
                        availableSystemRolesInDepartment
                      );
                    });

                    forkJoin(
                      validUsers.map((updatingUser: UserManagement) => {
                        return this.userAccountsDataService.editUser(
                          updatingUser
                        );
                      })
                    ).subscribe((editedUsers: UserManagement[]) => {
                      if (editedUsers.length > 0) {
                        // After changing the Place of Work, if the user still matches the filter
                        // then he should be there and his department info should be updated in the UI.
                        const isPendingList = this.tabLabelToPendingCode.has(
                          this.currentTabAriaLabel as UserAccountTabEnum
                        );
                        if (isPendingList) {
                          // Move users happened in the Pending User List.
                          const currentPendingCode = this.tabLabelToPendingCode.get(
                            this.currentTabAriaLabel as UserAccountTabEnum
                          );
                          this.updatePendingUserListAfterPlaceOfWorkChanged(
                            currentPendingCode,
                            editedUsers
                          );
                        } else if (
                          this.currentTabAriaLabel ===
                          UserAccountTabEnum.UserOtherPlace
                        ) {
                          this.getUserOtherPlace();
                        } else {
                          // Move users happened in the User Account List (the first tab).
                          this.updateUserListAfterPlaceOfWorkChanged(
                            newDepartmentId,
                            editedUsers
                          );
                        }

                        this.toastr.success(
                          this.translateAdapterService.getValueImmediately(
                            'User_Account_Page.Change_User_Place_of_Work.Message.Move_User_Success',
                            {
                              userCount: editedUsers.length,
                              newDepartmentName: editedUsers[0].departmentName
                            }
                          )
                        );
                      } else {
                        const failedMovingUsers = _.differenceBy(
                          validUsers,
                          editedUsers,
                          'identity.id'
                        );
                        this.toastr.error(
                          this.translateAdapterService.getValueImmediately(
                            'User_Account_Page.Change_User_Place_of_Work.Message.Move_User_Error',
                            {
                              users: failedMovingUsers
                                .map((u) => u.firstName)
                                .join(', ')
                            }
                          )
                        );
                      }
                      if (usersWithSameDestination.length > 0) {
                        this.toastr.warning(
                          this.translateAdapterService.getValueImmediately(
                            'User_Account_Page.Change_User_Place_of_Work.Message.Move_User_To_Same_Location_Warning',
                            {
                              users: usersWithSameDestination
                                .map((u) => u.firstName)
                                .join(', ')
                            }
                          )
                        );
                      }
                      this.cxGlobalLoaderService.hideLoader();
                      this.isVerticalToShowMenuAction = true;
                      modalRef.close();
                    });
                  });
                this.isVerticalToShowMenuAction = true;
              });
          },
          (error: any) => {
            this.isVerticalToShowMenuAction = false;
            this.toastr.error(
              error.error.Message,
              'An error occurred when moving user accounts.'
            );
          }
        )
      );
      modalRef.result
        .then((res: any) => {
          submitObserver.unsubscribe();
        })
        .catch((err: any) => {
          submitObserver.unsubscribe();
        });
    }
  }

  onPendingActionChanged($event: UserActionsModel): void {
    this.userActions = $event;

    if (!$event) {
      return;
    }

    const listNonEssentialActionsLength = $event.listNonEssentialActions
      ? $event.listNonEssentialActions.length
      : 0;
    const listSpecifyActionsLength = $event.listSpecifyActions
      ? $event.listSpecifyActions.length
      : 0;
    const totalItems = listNonEssentialActionsLength + listSpecifyActionsLength;
    const maximumItems = 7;
    this.isVerticalToShowMenuAction = totalItems > maximumItems ? false : true;
    this.changeDetectorRef.detectChanges();
  }

  onPendingSingleActionClicked($event: any): void {
    if ($event) {
      if ($event.action.actionType === StatusActionTypeEnum.Edit) {
        this.onEditUserClicked($event.item, false);

        return;
      }
      if (
        $event.action.actionType === StatusActionTypeEnum.Accept ||
        $event.action.actionType === StatusActionTypeEnum.CreateAccount
      ) {
        if (!$event.item.systemRoles || $event.item.systemRoles.length === 0) {
          this.toastr.warning(
            this.translateAdapterService.getValueImmediately(
              'User_Account_Page.Validation.Warning.System_Role_Must_Not_Be_Empty'
            )
          );

          return;
        }
      }
      this.evaluateActionOnUser(
        $event.action.message,
        $event.action.actionType,
        $event.item
      );
    }
  }

  onPendingSelectedUserChange($event: UserManagement[]): void {
    this.selectedUser = $event;
    this.isClearSelected = false;
    this.updateUserActionBaseOnGridSelection();
    this.changeDetectorRef.detectChanges();
  }

  onPlaceOfWorkUserActionChanged($event: UserActionsModel): void {
    this.userActions = $event;
    const totalItems =
      $event.listNonEssentialActions.length + $event.listSpecifyActions.length;
    const maximumItems = 7;
    this.isVerticalToShowMenuAction = totalItems > maximumItems ? false : true;
    this.changeDetectorRef.detectChanges();
  }

  onPlaceOfWorkSelectedUserChanged($event: UserManagement[]): void {
    this.selectedUser = $event;
    this.isClearSelected = false;
    this.changeDetectorRef.detectChanges();
  }

  onSelectedTabChange(tabChangeEvent: MatTabChangeEvent): void {
    this.clearSelectedItems();
    this.currentTabAriaLabel = tabChangeEvent.tab.ariaLabel;

    UserAccountsDataService.setCurrentTabLabel(this.currentTabAriaLabel);

    this.isHideFilterButton = !(
      this.currentTabAriaLabel === UserAccountTabEnum.UserAccounts ||
      this.currentTabAriaLabel === UserAccountTabEnum.UserOtherPlace
    );
    this.isHideExportButton = !(
      this.currentTabAriaLabel === UserAccountTabEnum.UserAccounts
    );

    this.isHideColumnButton = !(
      this.currentTabAriaLabel === UserAccountTabEnum.UserAccounts
    );

    this.isHideCreateUserAccountRequestButton = !(
      this.currentTabAriaLabel === UserAccountTabEnum.UserAccounts ||
      this.currentTabAriaLabel === UserAccountTabEnum.Pending1st ||
      this.currentTabAriaLabel === UserAccountTabEnum.Pending2nd ||
      this.currentTabAriaLabel === UserAccountTabEnum.Pending3rd
    );

    this.getCurrentTabUserList();

    this.initUserActionsListBasedOnRoles();
  }

  onColumnShowHideDef($event: ColumDefModel[]): void {
    this.userListColumnDef = $event;
    this.columnDefCopyToCheckShowHides = _.cloneDeep(
      this.userListColumnDef.filter((item, index) => index !== 0)
    );
  }

  onGridApiReady(gridApi: GridApi): void {
    this.gridApi = gridApi;
  }

  private hasPermissionToEdit(): boolean {
    switch (this.currentTabAriaLabel) {
      case UserAccountTabEnum.UserAccounts:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.BasicUserAccountsManagement
        );
      case UserAccountTabEnum.Pending1st:
        return this.currentUser.hasPermission(SAM_PERMISSIONS.EditPending1st);
      case UserAccountTabEnum.Pending2nd:
        return this.currentUser.hasPermission(SAM_PERMISSIONS.EditPending2nd);
      case UserAccountTabEnum.Pending3rd:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.EditPendingSpecial
        );
      case UserAccountTabEnum.UserOtherPlace:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.EditOtherPlaceOfWork
        );
      default:
        return false;
    }
  }

  private loadUsersDataAccordingToCurrentTab(): void {
    switch (this.currentTabAriaLabel) {
      case UserAccountTabEnum.UserAccounts:
        this.getUsers();
        break;

      case UserAccountTabEnum.UserOtherPlace:
        this.getUserOtherPlace();
        break;

      default:
        break;
    }

    this.clearSelectedItems();
    this.initUserActionsListBasedOnRoles();
    this.updateUserActionBaseOnGridSelection();
  }

  private processUnarchiveUsers(requestData: any[]): void {
    const unarchivedUsers = requestData.map((emp: any) => {
      return {
        ...emp,
        entityStatus: {
          ...emp.entityStatus,
          statusId: StatusTypeEnum.Archived.toString(),
          statusReasonId: StatusReasonTypeConstant.Archived_ManuallyArchived.code.toString()
        }
      };
    });

    if (unarchivedUsers) {
      this.subscription.add(this.unarchiveUser(unarchivedUsers));
    }
  }

  private processArchiveUsers(requestData: any[]): void {
    const archivedUsers = requestData.map((emp: any) => {
      return {
        ...emp,
        entityStatus: {
          ...emp.entityStatus,
          statusId: StatusTypeEnum.Archived.toString(),
          statusReasonId: StatusReasonTypeConstant.Archived_ManuallyArchived.code.toString()
        }
      };
    });

    if (archivedUsers) {
      this.subscription.add(
        this.archiveUser(
          archivedUsers,
          StatusReasonTypeConstant.Archived_ManuallyArchived.code.toString()
        )
      );
    }
  }

  private processDeleteUsers(requestData: any[]): void {
    const deleteUsers = requestData.map((emp: any) => {
      return {
        ...emp,
        entityStatus: {
          ...emp.entityStatus,
          statusReasonId: StatusReasonTypeConstant.ManuallyArchived.code.toString()
        }
      };
    });

    if (deleteUsers) {
      this.subscription.add(
        this.deleteUser(
          deleteUsers,
          StatusReasonTypeConstant.ManuallyArchived.code.toString()
        )
      );
    }
  }

  private onMassCreateUsersClicked(): void {
    this.router.navigate(['user-accounts/mass-users-creation']);
  }

  private getCurrentTabUserList(): void {
    const pendingCode = this.tabLabelToPendingCode.get(
      this.currentTabAriaLabel as UserAccountTabEnum
    );
    switch (this.currentTabAriaLabel) {
      case UserAccountTabEnum.Pending1st:
      case UserAccountTabEnum.Pending2nd:
      case UserAccountTabEnum.Pending3rd:
        if (
          this.searchParam !== this.userListSearchKeyHistory.get(pendingCode) ||
          !this.getPendingListItems(pendingCode)
        ) {
          this.getPendingUser(pendingCode);
        }
        break;
      case UserAccountTabEnum.UserOtherPlace:
        this.getUserOtherPlace();
        break;
      default:
        // is user accounts list
        if (
          this.searchParam !==
            this.userListSearchKeyHistory.get(
              UserAccountTabEnum.UserAccounts
            ) ||
          !this.userItemsData
        ) {
          this.getUsers();
        }
        break;
    }
  }

  private buildMemberDtoFromSubmittedForm(
    newMember: any,
    userGroupId: number
  ): MembershipDto {
    const newMemberDto = new MembershipDto({ ...newMember });
    newMemberDto.identity = new Identity({
      ownerId: newMember.identity.ownerId,
      customerId: newMember.identity.customerId,
      archetype: UserTypeEnum.Unknown
    });
    newMemberDto.memberId = newMember.identity.id;
    newMemberDto.groupId = userGroupId;
    newMemberDto.createdBy = this.currentUser.identity.id;
    newMemberDto.entityStatus = {
      statusId: StatusTypeEnum.Active.code,
      statusReasonId: 'Active_None',
      lastUpdatedBy: this.currentUser.identity.id
    };

    return newMemberDto;
  }

  private buildCurrentDepartmentCrumb(
    currentDepartmentCrumb: CxBreadCrumbItem[],
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
        !CxBreadCrumbItem.checkExistingBreadCrumbItem(
          currentDepartmentCrumb,
          currentDepartmentId
        )
      ) {
        // Prevent duplication when adding a new item into the bread crumb.
        continue;
      }
      currentDepartmentCrumb.push(
        new CxBreadCrumbItem({
          name: department.departmentName,
          identity: department.identity
        })
      );
      if (
        department.identity.id === userDepartmentId &&
        !CxBreadCrumbItem.checkExistingBreadCrumbItem(
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

  private getUsers(): void {
    if (
      this.filterParams &&
      this.filterParams.userEntityStatuses.length === 0
    ) {
      this.filterParams.userEntityStatuses = this.defaultStatusFilter;
    }
    this.filterParams.orderBy = `${this.currentFieldSort} ${this.currentSortType}`;
    this.cxGlobalLoaderService.showLoader();
    this.filterParams.searchKey = this.searchParam;
    this.getUserFromApi();
  }

  private getUserFromApi(): void {
    this.subscription.add(
      this.userAccountsDataService.getUsers(this.filterParams).subscribe(
        (result: PagingResponseModel<UserManagement>) => {
          this.userListSearchKeyHistory.set(
            UserAccountTabEnum.UserAccounts,
            this.searchParam
          );
          this.isLoadingEmployee = false;
          this.userItemsData = result;
          this.userItemsData.items.forEach((emp: UserManagement) => {
            if (emp.groups && emp.groups.length > 0) {
              emp.groups = emp.groups.filter(
                (group: any) =>
                  group.entityStatus.statusId !== StatusTypeEnum.Deactive.code
              );
            }
            this.employeeAvatarFromEmailsMap[
              emp.identity.id
            ] = this.getUserImage(emp);
          });
          this.changeDetectorRef.detectChanges();
          if (!this.isLoadingEmployee && !this.isLoadingDepartments) {
            this.cxGlobalLoaderService.hideLoader();
          }
        },
        (error: any) => {
          this.cxGlobalLoaderService.hideLoader();
          this.isLoadingEmployee = false;
        }
      )
    );
  }

  private getPendingUser(pendingStatus: string): void {
    const pendingPageSize = 10000;
    const pendingFilterParam = new UserManagementQueryModel();
    pendingFilterParam.userEntityStatuses = [pendingStatus];
    pendingFilterParam.orderBy = 'firstName Asc';
    pendingFilterParam.parentDepartmentId = [
      UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
    ];
    pendingFilterParam.filterOnSubDepartment = true;
    pendingFilterParam.searchKey = this.searchParam;
    pendingFilterParam.pageSize = pendingPageSize;
    this.cxGlobalLoaderService.showLoader();

    this.subscription.add(
      this.userAccountsDataService.getUsers(pendingFilterParam).subscribe(
        async (result: any) => {
          this.userListSearchKeyHistory.set(pendingStatus, this.searchParam);
          result.items = await this.filterUserWithoutkOtherPlaceOfWork(
            result.items
          );
          this.updatePendingListItems(pendingStatus, result.items);
          this.cxGlobalLoaderService.hideLoader();
          this.changeDetectorRef.detectChanges();
        },
        (error: any) => {
          this.cxGlobalLoaderService.hideLoader();
        }
      )
    );
  }

  private async getUserOtherPlace(): Promise<void> {
    this.cxGlobalLoaderService.showLoader();
    if (
      this.adjustFilterApplied &&
      !isEmpty(this.adjustFilterApplied.appliedFilter)
    ) {
      this.userOtherPlaceList = await this.getListLearnerOtherPlaceOfWork(
        this.searchParam,
        this.filterParams
      );
    } else {
      this.userOtherPlaceList = await this.getListLearnerOtherPlaceOfWork(
        this.searchParam
      );
    }

    this.updateUserAvatar(this.userOtherPlaceList);
    this.userListSearchKeyHistory.set(
      UserAccountTabEnum.UserOtherPlace,
      this.searchParam
    );
    this.cxGlobalLoaderService.hideLoader();
    this.changeDetectorRef.detectChanges();
  }

  private async getListLearnerOtherPlaceOfWork(
    searchKey: string,
    filterParam?: UserManagementQueryModel
  ): Promise<UserManagement[]> {
    // TODO: implement paging
    const pageIndex = 0;
    const pageSize: number = 1000;
    const pagingResponse = await this.userAccountsDataService.getUsersWithOtherPlaceOfWorkAync(
      searchKey,
      pageIndex,
      pageSize,
      filterParam
    );
    if (!pagingResponse) {
      return [];
    }

    return pagingResponse.items;
  }

  private updateUserAvatar(users: UserManagement[]): void {
    users.forEach((user: UserManagement) => {
      user.avatarUrl =
        user.jsonDynamicAttributes.avatarUrl || AppConstant.defaultAvatar;
    });
  }

  private getHierarchicalDepartments(
    currentDepartmentId: number,
    departmentQuery: DepartmentQueryModel,
    isBuildTableAndCurmb: boolean = true,
    getUserList: boolean = false,
    parentDepartmentId?: number,
    isJustLogin: boolean = false
  ): void {
    this.cxGlobalLoaderService.showLoader();
    const departmentQueryCloned = _.clone(departmentQuery);
    this.userAccountsDataService
      .getHierarchicalDepartments(currentDepartmentId, departmentQueryCloned)
      .subscribe(
        (response: any[]) => {
          this.isLoadingDepartments = false;
          if (this.reloadDataAfterClearSearch) {
            this.departmentModel.departments = [];
            this.changeDetectorRef.detectChanges();
          }
          this.departmentModel.departments = _.uniqBy(
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
                this.userDepartmentId,
                currentDepartmentId === AppConstant.topDepartmentId
                  ? this.userDepartmentId
                  : currentDepartmentId
              )
            );
          }
          if (this.departmentModel.departments.length > 1) {
            this.isDisplayOrganisationNavigation = true;
            this.changeDetectorRef.detectChanges();
          }
          this.currentDepartment = UserAccountsHelper.getCurrentDepartment(
            this.departmentModel
          );
          this.departmentModel.currentDepartmentId = this.currentDepartment.identity.id;
          this.cxSurveyjsExtendedService.setCurrentDepartmentVariables(
            this.departmentModel.currentDepartmentId
          );
          if (getUserList) {
            const defaultDepartmentId = 1;
            if (
              isJustLogin &&
              this.currentUser.topAccessibleDepartment.identity.id !==
                defaultDepartmentId
            ) {
              this.filterParams.parentDepartmentId = [
                this.currentUser.topAccessibleDepartment.identity.id
              ];
            } else if (parentDepartmentId) {
              this.filterParams.parentDepartmentId = [parentDepartmentId];
            }
            this.getUsers();
          }
        },
        (error) => {
          this.cxGlobalLoaderService.hideLoader();
        },
        () => {
          this.departmentModel.isDetectExpandTree = false;
          this.reloadDataAfterClearSearch = false;
          if (!this.isLoadingEmployee && !this.isLoadingDepartments) {
            this.cxGlobalLoaderService.hideLoader();
          }
          this.changeDetectorRef.detectChanges();
          this.isDisplayOrganisationSearch =
            this.departmentModel.departments.length > 1;
        }
      );
  }

  private resetDepartmentDropdownData(newDepartment: any): void {
    if (!this.currentUser) {
      return;
    }
    this.initUserActionsListBasedOnRoles();
  }

  private getDisplayRolesForUser(
    roles: any[],
    languageCode: string,
    field: string = 'Name'
  ): any {
    return roles.map((role: any) => {
      return SurveyUtils.getPropLocalizedData(
        role.localizedData,
        field,
        languageCode
      );
    });
  }

  private initEditForm(): void {
    this.formModal.cancelName = 'Cancel';
    this.formModal.submitName = 'Save';
    // TODO: should request for data only when open modal
  }

  private updateApprovalData(
    approvalData: ApprovalInfoTabModel,
    userBeingEdited: UserManagement
  ): void {
    if (!approvalData || _.isEmpty(approvalData) || !userBeingEdited) {
      return;
    }
    const currentPrimaryGroup =
      userBeingEdited.groups &&
      userBeingEdited.groups.find(
        (group: any) =>
          group.type === ApprovalGroupTypeEnum.PrimaryApprovalGroup
      );
    if (currentPrimaryGroup && !approvalData.primaryApprovalGroup) {
      this.removeUserFromApprovalGroup(currentPrimaryGroup, [
        userBeingEdited.identity.id
      ]);
    } else if (
      this.isApprovalGroupChanged(
        currentPrimaryGroup,
        approvalData.primaryApprovalGroup
      )
    ) {
      this.addUserToApprovalGroup(approvalData.primaryApprovalGroup, [
        userBeingEdited.identity.id
      ]);
    }

    const currentAlternateGroup =
      userBeingEdited.groups &&
      userBeingEdited.groups.find(
        (group: any) =>
          group.type === ApprovalGroupTypeEnum.AlternativeApprovalGroup
      );
    if (currentAlternateGroup && !approvalData.alternateApprovalGroup) {
      this.removeUserFromApprovalGroup(currentAlternateGroup, [
        userBeingEdited.identity.id
      ]);
    } else if (
      this.isApprovalGroupChanged(
        currentAlternateGroup,
        approvalData.alternateApprovalGroup
      )
    ) {
      this.addUserToApprovalGroup(approvalData.alternateApprovalGroup, [
        userBeingEdited.identity.id
      ]);
    }

    this.updateMembersOfApprovalGroup(
      approvalData.memberOfPrimaryApprovalGroup
    );

    this.updateMembersOfApprovalGroup(
      approvalData.memberOfAlternateApprovalGroup
    );
  }

  private updateMembersOfApprovalGroup(
    memberOfGroup: MemberApprovalGroupModel
  ): void {
    if (!memberOfGroup || !memberOfGroup.approvalGroup) {
      return;
    }
    const addedMemberIds = _.difference(
      memberOfGroup.currentMemberIds,
      memberOfGroup.previousMemberIds
    );
    const removedMemberIds = _.difference(
      memberOfGroup.previousMemberIds,
      memberOfGroup.currentMemberIds
    );
    const approvalGroup = memberOfGroup.approvalGroup;
    if (addedMemberIds && addedMemberIds.length) {
      this.addUserToApprovalGroup(approvalGroup, addedMemberIds);
    }
    if (removedMemberIds && removedMemberIds.length) {
      this.removeUserFromApprovalGroup(approvalGroup, removedMemberIds);
    }
  }

  private isApprovalGroupChanged(
    currentGroup: ApprovalGroup,
    newGroup: ApprovalGroup
  ): boolean {
    return (
      (currentGroup && currentGroup.identity.id) !==
      (newGroup && newGroup.identity.id)
    );
  }

  private addUserToApprovalGroup(
    approvalGroup: ApprovalGroup,
    employeeIds: number[]
  ): void {
    if (approvalGroup) {
      this.userAccountsDataService
        .addUsersToApprovalGroup(employeeIds, new MembershipDto(approvalGroup))
        .subscribe(
          (response: any) => {
            if (response && response.length === employeeIds.length) {
              response.forEach((element: any) => {
                const index = this.userItemsData.items.findIndex(
                  (item: any) => item.identity.id === element.memberId
                );
                if (index < 0) {
                  return;
                }
                let currentGroups = this.userItemsData.items[index].groups
                  ? this.userItemsData.items[index].groups
                  : [];
                currentGroups = currentGroups.filter(
                  (group: any) => group.type !== approvalGroup.type
                );
                currentGroups.push(approvalGroup);
                this.userItemsData.items[index] = {
                  ...this.userItemsData.items[index],
                  groups: currentGroups
                };
              });
              this.refreshUserList();
              this.toastr.success(
                `${response.length} user(s) assigned to Approving Officer/Alternate Approving Officer ${approvalGroup.name}.`
              );
            }
          },
          (error: any) => {
            this.toastr.error("Something's wrong, please try again later");
          }
        );
    }
  }

  private unarchiveUser(employees: UserManagement[]): void {
    forkJoin(
      employees.map((element: UserManagement) => {
        return this.userAccountsDataService.unarchiveUser(element.identity.id);
      })
    ).subscribe((unarchivedEmployees: UserManagement[]) => {
      this.handleUserResponseData(unarchivedEmployees, stringEmpty);
      this.toastr.success(
        `${unarchivedEmployees.length} user(s) has been unarchived`
      );
    });
  }

  private archiveUser(
    employees: UserManagement[],
    entityStatusReason: string
  ): void {
    forkJoin(
      employees.map((element: UserManagement) => {
        return this.userAccountsDataService.archiveUser(
          element.identity.id,
          entityStatusReason
        );
      })
    ).subscribe((archivedEmployees: UserManagement[]) => {
      this.handleUserResponseData(archivedEmployees, stringEmpty);
      this.toastr.success(
        `${archivedEmployees.length} user(s) has been archived`
      );
    });
  }

  private deleteUser(
    employees: UserManagement[],
    entityStatusReason: string
  ): void {
    forkJoin(
      employees.map((element: UserManagement) => {
        return this.userAccountsDataService.deleteUser(
          element.identity.id,
          entityStatusReason
        );
      })
    ).subscribe((deletedEmployees: UserManagement[]) => {
      deletedEmployees.forEach((deletedEmployee: UserManagement) => {
        const index = this.findUserIndexInUserList(deletedEmployee);
        if (index < 0) {
          return;
        }
        this.userItemsData.items.splice(index, 1);
      });
      this.refreshUserList();
      this.toastr.success(
        `${deletedEmployees.length} user(s) has been deleted`
      );
    });
  }

  private findUserIndexInUserList(user: UserManagement): number {
    return this.userItemsData.items.findIndex(
      (employee: UserManagement) => employee.identity.id === user.identity.id
    );
  }

  private handleUserResponseData(
    users: UserManagement[],
    actionType: string,
    statusReasonId?: string
  ): void {
    if (
      this.selectedPendingStatusToAction ===
        StatusTypeEnum.PendingApproval1st.code ||
      this.selectedPendingStatusToAction ===
        StatusTypeEnum.PendingApproval2nd.code ||
      this.selectedPendingStatusToAction ===
        StatusTypeEnum.PendingApproval3rd.code
    ) {
      this.updatePendingListAfterMakeAction(users, actionType);
    }
    users.forEach((user: UserManagement) => {
      const index = this.findUserIndexInUserList(user);
      if (index < 0) {
        return;
      }
      this.userItemsData.items[index] = user;
    });
    this.refreshUserList();
  }

  private updatePendingListAfterMakeAction(
    users: UserManagement[],
    actionType: string
  ): void {
    if (this.currentTabAriaLabel === UserAccountTabEnum.UserOtherPlace) {
      this.userOtherPlaceList = this.removeUserOnList(
        this.userOtherPlaceList,
        users
      );
    }

    if (
      this.selectedPendingStatusToAction ===
      StatusTypeEnum.PendingApproval1st.code
    ) {
      this.pending1stLevelApprovalList = this.removeUserOnList(
        this.pending1stLevelApprovalList,
        users
      );
      if (
        actionType === StatusActionTypeEnum.Accept &&
        this.pending2ndLevelApprovalList !== undefined
      ) {
        this.pending2ndLevelApprovalList = this.addItemToListItems(
          this.pending2ndLevelApprovalList,
          users
        );
      }
    } else if (
      this.selectedPendingStatusToAction ===
      StatusTypeEnum.PendingApproval2nd.code
    ) {
      this.pending2ndLevelApprovalList = this.removeUserOnList(
        this.pending2ndLevelApprovalList,
        users
      );
      if (actionType === StatusActionTypeEnum.Accept) {
        this.userItemsData.items = this.addItemToListItems(
          this.userItemsData.items,
          users
        );
      } else if (
        actionType === StatusActionTypeEnum.RequestSpecialApproval &&
        this.pendingSpecialLevelApprovalList !== undefined
      ) {
        this.pendingSpecialLevelApprovalList = this.addItemToListItems(
          this.pendingSpecialLevelApprovalList,
          users
        );
      }
    } else if (
      this.selectedPendingStatusToAction ===
      StatusTypeEnum.PendingApproval3rd.code
    ) {
      this.pendingSpecialLevelApprovalList = this.removeUserOnList(
        this.pendingSpecialLevelApprovalList,
        users
      );
      if (actionType === StatusActionTypeEnum.Accept) {
        this.userItemsData.items = this.addItemToListItems(
          this.userItemsData.items,
          users
        );
      }
    }
  }

  private removeUserOnList(
    userList: UserManagement[],
    userListToRemove: UserManagement[]
  ): UserManagement[] {
    const userNotFound = -1;
    if (userList) {
      userList = userList.filter(
        (item: UserManagement) =>
          userListToRemove.findIndex(
            (user: UserManagement) => user.identity.id === item.identity.id
          ) === userNotFound
      );
    }

    return userList;
  }

  private addItemToListItems(
    targetList: UserManagement[],
    addList: UserManagement[]
  ): UserManagement[] {
    targetList = targetList ? targetList : [];
    targetList = targetList.concat(addList);

    return targetList;
  }

  private updatePendingListItems(
    pendingStatus: string,
    dataResponse: UserManagement[]
  ): void {
    switch (pendingStatus) {
      case StatusTypeEnum.PendingApproval1st.code:
        this.pending1stLevelApprovalList = dataResponse;
        break;
      case StatusTypeEnum.PendingApproval2nd.code:
        this.pending2ndLevelApprovalList = dataResponse;
        break;
      case StatusTypeEnum.PendingApproval3rd.code:
        this.pendingSpecialLevelApprovalList = dataResponse;
        break;
      default:
        break;
    }
    this.changeDetectorRef.detectChanges();
  }

  private getPendingListItems(pendingStatus: string): UserManagement[] {
    switch (pendingStatus) {
      case StatusTypeEnum.PendingApproval1st.code:
        return this.pending1stLevelApprovalList;
      case StatusTypeEnum.PendingApproval2nd.code:
        return this.pending2ndLevelApprovalList;
      case StatusTypeEnum.PendingApproval3rd.code:
        return this.pendingSpecialLevelApprovalList;
      default:
        return;
    }
  }

  private buildNewUserDtoFromSubmittedForm(
    submittedData: EditUser
  ): UserManagement {
    const newUserDto = new UserManagement({ ...(submittedData as any) });
    newUserDto.firstName = CrossSiteScriptingUtil.encodeHtmlEntity(
      newUserDto.firstName
    );
    if (!!newUserDto.dateOfBirth) {
      newUserDto.dateOfBirth = DateTimeUtil.surveyToServerFormat(
        newUserDto.dateOfBirth
      );
    }
    newUserDto.identity = {
      ownerId: AppConstant.ownerId,
      customerId: AppConstant.customerId,
      archetype: UserTypeEnum.Employee
    };

    newUserDto.entityStatus = {
      statusId:
        this.userAccountAdminRole.length > 0 || this.superAdminRole.length > 0
          ? StatusTypeEnum.New.code
          : StatusTypeEnum.PendingApproval2nd.code,
      statusReasonId: 'Unknown',
      expirationDate: DateTimeUtil.surveyToEndDateLocalTimeISO(
        submittedData.expirationDate
      ),
      activeDate: DateTimeUtil.surveyToDateLocalTimeISO(
        submittedData.activeDate
      )
    };

    if (submittedData.personnelGroups) {
      newUserDto.personnelGroups = [
        this.personnelGroups.find(
          (group: any) =>
            group.identity.extId === submittedData.personnelGroups.id
        )
      ];
    }

    if (submittedData.developmentalRoles) {
      newUserDto.developmentalRoles = [
        this.developmentalRoles.find(
          (group: any) =>
            group.identity.extId === submittedData.developmentalRoles.id
        )
      ];
    }

    if (submittedData.learningFrameworks) {
      const learningFrameworkIds = submittedData.learningFrameworks.map(
        (item: any) => item
      );
      newUserDto.learningFrameworks = this.learningFrameworks.filter(
        (group: any) => learningFrameworkIds.includes(group.identity.extId)
      );
    }

    return this.mapJsonDynamicAttributes(newUserDto, null, submittedData);
  }

  private onEditUser(
    approvalData: ApprovalInfoTabModel,
    originalEditedUserName: string,
    editedUser: any,
    isReportingOfficer: boolean,
    isEditNormalUser: boolean = true
  ): void {
    this.cxGlobalLoaderService.showLoader();

    this.updateApprovalData(approvalData, editedUser);

    /*
           WE NEED TO ENSURE AO/AAO IS SAVED BEFORE PROCEED TO UPDATE USER
           BECAUSE THEY ARE IN TWO SEPARATE REQUESTS.
           THIS IS NOT THE OFFICIAL WAY, SINCE WE ARE IN RUSH TIME, WE WOULD GO WITH SETTIMEOUT,
           IT WILL BE REFACTORED AT R.3 RELEASE
        */
    const sleepTime = 1000;
    setTimeout(() => {
      this.subscription.add(
        this.userAccountsDataService.editUser(editedUser).subscribe(
          (employee: UserManagement) => {
            this.cxGlobalLoaderService.hideLoader();
            if (isEditNormalUser) {
              const employeeIndex = this.userItemsData.items.findIndex(
                (item: UserManagement) =>
                  item.identity.id === editedUser.identity.id
              );
              const newEmployeeList = Utils.cloneDeep(this.userItemsData.items);
              const updatingEmployee = Utils.cloneDeep(employee);
              newEmployeeList[employeeIndex] = updatingEmployee;
              this.userItemsData.items = newEmployeeList;
              this.toastr.success(
                `User ${originalEditedUserName} updated successfully`
              );
              this.processApprovalGroupUpdate(employee, isReportingOfficer);
            } else {
              if (employee.departmentId === environment.OtherDepartmentId) {
                this.updateUserOtherPlaceListAfterEditingUser(
                  editedUser.entityStatus.statusId,
                  employee
                );
              } else {
                this.updatePendingListAfterEditingUser(
                  editedUser.entityStatus.statusId,
                  employee
                );
              }
              this.toastr.success(
                `User ${originalEditedUserName} updated successfully`
              );
            }

            this.initUserActionsListBasedOnRoles();
            this.changeDetectorRef.detectChanges();
          },
          (error: any) => {
            this.cxGlobalLoaderService.hideLoader();
            this.toastr.error(error.error.Message, 'Error');
          }
        )
      );
    }, sleepTime);
  }

  private initUserActionsListBasedOnRoles(): void {
    this.isVerticalToShowMenuAction = true;
    const isPendingList = this.tabLabelToPendingCode.has(
      this.currentTabAriaLabel as UserAccountTabEnum
    );
    const theRight = this.checkingUserRolesService.hasRightToAccessReportingUser(
      this.currentUser.systemRoles
    );
    if (isPendingList) {
      this.userActions.listEssentialActions = [];
      this.userActions.listNonEssentialActions = [];
      this.userActions.listSpecifyActions = [];
    } else if (this.currentTabAriaLabel === UserAccountTabEnum.UserOtherPlace) {
      this.userActions = this.getCreateAccountRequestAction();
    } else {
      this.userActions = initUserActions(
        this.translateAdapterService,
        true,
        theRight,
        this.currentUser.hasPermission(
          SAM_PERMISSIONS.BasicUserAccountsManagement
        ),
        this.currentUser.hasPermission(SAM_PERMISSIONS.ExportUsers)
      );
    }
  }

  private getCreateAccountRequestAction(): UserActionsModel {
    const createOrgUnitAction = this.getCreateOrgUnitAction();
    const hasPermissionToCreateOrgUnitAction = this.currentUser.hasPermission(
      SAM_PERMISSIONS.CreateOrganisationUnitInOtherPlaceOfWork
    );
    const actions = new UserActionsModel({
      listEssentialActions: hasPermissionToCreateOrgUnitAction
        ? [createOrgUnitAction]
        : []
    });

    return actions;
  }

  private getCreateOrgUnitAction(): ActionsModel {
    const createNewOrgUnitActionMapped = USER_ACTION_MAPPING_CONST.find(
      (action) => action.targetAction === StatusActionTypeEnum.CreateNewOrgUnit
    );
    const createNewOrgUnitActionTarget =
      createNewOrgUnitActionMapped.targetAction;
    const createNewOrgUnitActionText = this.translateAdapterService.getValueImmediately(
      `User_Account_Page.User_Context_Menu.${createNewOrgUnitActionTarget}`
    );

    return new ActionsModel({
      actionType: createNewOrgUnitActionMapped.targetAction,
      icon: null,
      message: createNewOrgUnitActionMapped.message,
      text: createNewOrgUnitActionText
    });
  }

  private initEmailTemplateDataForChangingEmail(
    emailAddress: string,
    emailSubject: string,
    emailTemplateName: string,
    emailContent: string,
    emailAlertText: string
  ): EmailModel {
    const emailModel = new EmailModel({
      emails: [emailAddress],
      templateData: new TemplateModel({
        project: 'Opal',
        module: 'SystemAdmin'
      }),
      isHtmlEmail: true
    });
    emailModel.body = emailSubject;
    emailModel.subject = emailSubject;
    emailModel.templateData.templateName = emailTemplateName;
    emailModel.templateData.data = {
      EmailContent: emailContent,
      AlertText: emailAlertText,
      CallBackUrl: `${window.location.origin}/login`,
      CallBackText: 'Login into system',
      LogoPath: `${window.location.origin}/assets/images/opal.png`
    };

    return emailModel;
  }

  private updatePendingListAfterEditingUser(
    statusType: string,
    employee: UserManagement
  ): void {
    let employeeIndex;
    let newEmployeeList;
    switch (statusType) {
      case StatusTypeEnum.PendingApproval1st.code:
        employeeIndex = this.pending1stLevelApprovalList.findIndex(
          (item: UserManagement) => item.identity.id === employee.identity.id
        );
        newEmployeeList = _.clone(this.pending1stLevelApprovalList);
        newEmployeeList[employeeIndex] = employee;
        this.pending1stLevelApprovalList = newEmployeeList;
        break;
      case StatusTypeEnum.PendingApproval2nd.code:
        employeeIndex = this.pending2ndLevelApprovalList.findIndex(
          (item: UserManagement) => item.identity.id === employee.identity.id
        );
        newEmployeeList = _.clone(this.pending2ndLevelApprovalList);
        newEmployeeList[employeeIndex] = employee;
        this.pending2ndLevelApprovalList = newEmployeeList;
        break;
      case StatusTypeEnum.PendingApproval3rd.code:
        employeeIndex = this.pendingSpecialLevelApprovalList.findIndex(
          (item: UserManagement) => item.identity.id === employee.identity.id
        );
        newEmployeeList = _.clone(this.pendingSpecialLevelApprovalList);
        newEmployeeList[employeeIndex] = employee;
        this.pendingSpecialLevelApprovalList = newEmployeeList;
        break;
      default:
        return;
    }
  }

  private updateUserOtherPlaceListAfterEditingUser(
    statusType: string,
    employee: UserManagement
  ): void {
    const employeeIndex = this.userOtherPlaceList.findIndex(
      (item: UserManagement) => item.identity.id === employee.identity.id
    );
    const newEmployeeList = _.clone(this.userOtherPlaceList);
    newEmployeeList[employeeIndex] = employee;
    this.userOtherPlaceList = newEmployeeList;
  }

  private processApprovalGroupUpdate(
    user: UserManagement,
    isReportingOfficer: boolean
  ): void {
    const isReportingOfficerAfterUpdated = user.systemRoles
      ? user.systemRoles.findIndex(
          (role: any) => role.identity.extId === UserRoleEnum.ReportingOfficer
        ) > findIndexCommon.notFound
      : false;
    if (isReportingOfficer === isReportingOfficerAfterUpdated) {
      return;
    } else {
      if (isReportingOfficer) {
        return this.deactivateApprovalGroup(user.identity.id);
      } else {
        return this.createApprovalGroup(user);
      }
    }
  }

  private createApprovalGroup(user: UserManagement): void {
    const approvalGroupPrimary = new ApprovalGroup({
      approverId: user.identity.id,
      name: user.firstName,
      departmentId: user.departmentId,
      type: ApprovalGroupTypeEnum.PrimaryApprovalGroup,
      identity: new Identity({ id: 0 } as Identity),
      entityStatus: new EntityStatus({
        statusId: StatusTypeEnum.Active.code
      } as EntityStatus)
    } as ApprovalGroup);
    const approvalGroupAlternate = new ApprovalGroup(approvalGroupPrimary);
    approvalGroupAlternate.type =
      ApprovalGroupTypeEnum.AlternativeApprovalGroup;
    this.subscription.add(
      combineLatest(
        this.userAccountsDataService.createApprovalGroup(approvalGroupPrimary),
        this.userAccountsDataService.createApprovalGroup(approvalGroupAlternate)
      ).subscribe(
        ([primaryApprovalGroup, alternateApprovalGroup]) => {
          // do nothing
        },
        () => {
          this.toastr.error(
            `An error occured when creating new approval group.`
          );
        }
      )
    );
  }

  private deactivateApprovalGroup(approverId: number): void {
    const getApprovalGroupsParams = new ApprovalGroupQueryModel();
    getApprovalGroupsParams.parentDepartmentId = this.departmentModel.currentDepartmentId;
    getApprovalGroupsParams.statusEnums = [StatusTypeEnum.Active.code];
    getApprovalGroupsParams.approverIds = [approverId];
    this.subscription.add(
      this.userAccountsDataService
        .getApprovalGroups(getApprovalGroupsParams)
        .subscribe(
          (approvalGroup: ApprovalGroup[]) => {
            if (approvalGroup) {
              const primaryApprovalGroup: ApprovalGroup = approvalGroup[0];
              primaryApprovalGroup.entityStatus.statusId =
                StatusTypeEnum.Deactive.code;
              const alternateApprovalGroup: ApprovalGroup = approvalGroup[1];
              alternateApprovalGroup.entityStatus.statusId =
                StatusTypeEnum.Deactive.code;
              this.subscription.add(
                combineLatest(
                  this.userAccountsDataService.updateApprovalGroup(
                    primaryApprovalGroup
                  ),
                  this.userAccountsDataService.updateApprovalGroup(
                    alternateApprovalGroup
                  )
                ).subscribe(
                  ([
                    primaryUpdatedApprovalGroup,
                    alternateUpdatedApprovalGroup
                  ]) => {
                    this.toastr.success(
                      `Approval group(s) of user ${primaryUpdatedApprovalGroup.name} has been deactivated.`
                    );
                    const affectedUserList = this.userItemsData.items.filter(
                      (item: UserManagement) =>
                        item.groups[0] &&
                        item.groups[0].identity.id ===
                          primaryUpdatedApprovalGroup.identity.id
                    );
                    if (affectedUserList.length > 0) {
                      affectedUserList.forEach(
                        (user: UserManagement) => (user.groups = [])
                      );
                      this.changeDetectorRef.detectChanges();
                    }
                  },
                  () => {
                    this.toastr.error(
                      `An error occured when updating approval group.`
                    );
                  }
                )
              );
            }
          },
          () => {
            this.toastr.error(`An error occured when getting approval group.`);
          }
        )
    );
  }

  private getPathOfDepartment(departmentArray: any[], department: any): any {
    const parentDepartment = departmentArray.find(
      (element: any) => element.identity.id === department.parentDepartmentId
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
    departmentArray: any[],
    department: any,
    newObject: any
  ): void {
    const parentDepartment = departmentArray.find(
      (element: any) => element.identity.id === department.parentDepartmentId
    );
    if (!parentDepartment) {
      newObject[department.identity.id] = department.identity.id;
    } else {
      newObject[parentDepartment.identity.id] = parentDepartment.identity.id;
      this.getRouteOfDepartment(departmentArray, parentDepartment, newObject);
    }
  }

  private createPDCatalogueEnumerationSurveyJSVariables(): CxSurveyjsVariable[] {
    return Object.keys(PDCatalogueConstant).map((key: string) => {
      const cxSurveyjsVariable = new CxSurveyjsVariable({
        name: PDCatalogueConstant[key].code,
        value: PDCatalogueConstant[key].id
      });

      return cxSurveyjsVariable;
    });
  }

  private createOrganizationUnitTypesSurveyJSVariables(): CxSurveyjsVariable[] {
    const currentDepartment = this.departmentModel
      ? this.departmentModel.departments.find(
          (department) =>
            department.identity.id === this.departmentModel.currentDepartmentId
        )
      : null;

    const isSchoolDepartment =
      currentDepartment && currentDepartment.organizationalUnitTypes
        ? currentDepartment.organizationalUnitTypes.findIndex(
            (orgType: DepartmentType) =>
              orgType.identity.extId ===
              OrganizationUnitType.School.toLowerCase()
          ) > findIndexCommon.notFound
        : false;

    let designationQueryParam = DesignationQueryParamEnum.HQ.toString();

    if (isSchoolDepartment) {
      designationQueryParam = DesignationQueryParamEnum.SCHOOL.toString();
    }

    return [
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.currentDepartment_typesQueryParam,
        value: designationQueryParam
      })
    ];
  }

  private mapJsonDynamicAttributes(
    user: UserManagement,
    initialJsonData?: any,
    editedUser?: any
  ): any {
    // Init SurveyJS form
    if (initialJsonData) {
      initialJsonData.jsonDynamicAttributes = {};

      if (user.personnelGroups && user.personnelGroups.length) {
        const personnelGroup = this.pdCataloguePersonnelGroups.find(
          (data: PDCatalogueEnumerationDto) =>
            data.id === user.personnelGroups[0].identity.extId
        );
        initialJsonData.personnelGroups = personnelGroup;
      }

      if (user.careerPaths && user.careerPaths.length) {
        const careerPaths = this.pdCatalogueCareerPaths
          .filter((data: PDCatalogueEnumerationDto) => {
            return (
              user.careerPaths.findIndex(
                (career: any) => data.id === career.identity.extId
              ) > findIndexCommon.notFound
            );
          })
          .map((item: any) => item.id);
        initialJsonData.careerPaths = careerPaths;
      }

      if (user.developmentalRoles && user.developmentalRoles.length) {
        const developmentalRole = this.pdCatalogueDevelopmentalRoles.find(
          (data: PDCatalogueEnumerationDto) =>
            data.id === user.developmentalRoles[0].identity.extId
        );
        initialJsonData.developmentalRoles = developmentalRole;
      }

      if (user.learningFrameworks && user.learningFrameworks.length) {
        const learningFrameworks = user.learningFrameworks.map(
          (property: any) => property.identity.extId
        );
        initialJsonData.learningFrameworks = learningFrameworks;
      }

      if (user.jsonDynamicAttributes) {
        let attributeKeys = Object.keys(user.jsonDynamicAttributes);
        attributeKeys = attributeKeys.filter(
          (attributeKey: string) =>
            !Object.keys(fieldNonJsonDynamic).includes(attributeKey)
        );
        attributeKeys.forEach((attributeKey: string) => {
          initialJsonData[attributeKey] =
            user.jsonDynamicAttributes[attributeKey];
        });
        if (user.jsonDynamicAttributes.avatarUrl) {
          initialJsonData.avatarUrl = user.jsonDynamicAttributes.avatarUrl;
        }
      }

      return initialJsonData;
    }

    if (editedUser) {
      const newKeys = Object.keys(editedUser).filter(
        (key: string) => !Object.keys(user).includes(key)
      );
      if (newKeys.length) {
        user.jsonDynamicAttributes = {};
        newKeys.forEach((key: string) => {
          user.jsonDynamicAttributes[key] = editedUser[key];
        });
      }
    }

    return user;
  }
  private initFilterDepartment(searchText?: string): DepartmentQueryModel {
    const paramQuery = new DepartmentQueryModel({
      includeDepartmentType: true,
      includeParent: false,
      includeChildren: true,
      maxChildrenLevel: 1,
      countChildren: true
    });
    if (searchText !== undefined) {
      // Send a dummy parameter into the API in order to get the full list of hierarchy since client will do filtering.
      paramQuery.searchText = searchText;
      paramQuery.maxChildrenLevel = undefined;
    } else if (
      !this.currentDepartment &&
      (this.superAdminRole.length || this.userAccountAdminRole.length) &&
      this.currentUser.departmentId !== AppConstant.topDepartmentId
    ) {
      paramQuery.maxChildrenLevel = 1;
    }

    return paramQuery;
  }

  private clearSelectedItems(): void {
    this.selectedUser = [];
    this.isClearSelected = true;
    this.changeDetectorRef.detectChanges();
  }

  private addUserTypes(listUserTypes: any, submittedData: any): any[] {
    let userTypesResult = submittedData;
    if (!submittedData) {
      return [];
    }

    if (!(submittedData instanceof Array)) {
      userTypesResult = [userTypesResult];
    }

    return listUserTypes.filter((userType: any) => {
      return (
        userTypesResult.findIndex(
          (item: string) => item === userType.identity.extId
        ) > findIndexCommon.notFound
      );
    });
  }

  private onGenerateReport(): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'lg',
      centered: true
    });
    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateAdapterService.getValueImmediately(
      'Common.Button.Cancel'
    );
    modalComponent.confirmButtonText = this.translateAdapterService.getValueImmediately(
      'Common.Button.OK'
    );
    modalComponent.header = this.translateAdapterService.getValueImmediately(
      'User_Account_Page.Report_User_Account.Header'
    );
    modalComponent.content = this.translateAdapterService.getValueImmediately(
      'User_Account_Page.Report_User_Account.Message',
      { emailUser: this.currentUser.emails }
    );

    modalComponent.confirm.subscribe(() => {
      modalRef.close();
      const defaultOptions = DefaultOptionModel(
        UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
      );
      let optionsReport = {};
      this.currentUser.systemRoles.forEach((element: SystemRole) => {
        optionsReport = AddingMoreOptionsBaseOnRole(
          defaultOptions,
          element.identity.extId
        );
      });
      this.userAccountsDataService.exportAsyncAccounts(optionsReport).subscribe(
        (instructionReporting: InstructionReporting) => {
          this.informationDialogService.success({
            message: this.translateAdapterService.getValueImmediately(
              'User_Account_Page.Report_User_Account.Sent'
            )
          });
        },
        (errors) => {
          this.toastr.error(`An error occured when sending email`);
        }
      );
    });

    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
  }

  private buildDataJsonForEditUser(userBeingEdited: UserManagement): any {
    let dataJson: any = {
      firstName: userBeingEdited.firstName,
      emailAddress: userBeingEdited.emailAddress,
      mobileNumber: userBeingEdited.mobileNumber,
      mobileCountryCode: userBeingEdited.mobileCountryCode,
      externallyMastered: userBeingEdited.entityStatus.externallyMastered,
      ssn: userBeingEdited.ssn,
      gender: userBeingEdited.gender,
      departmentName: userBeingEdited.departmentName,
      departmentId: userBeingEdited.departmentId,
      expirationDate: DateTimeUtil.toSurveyFormat(
        userBeingEdited.entityStatus.expirationDate
      ),
      activeDate: DateTimeUtil.toSurveyFormat(
        userBeingEdited.entityStatus.activeDate
      ),
      systemRoles: userBeingEdited.systemRoles
        ? userBeingEdited.systemRoles
        : stringEmpty,
      learningFrameworks: userBeingEdited.learningFrameworks
        ? userBeingEdited.learningFrameworks
        : stringEmpty,
      developmentalRoles: userBeingEdited.developmentalRoles
        ? userBeingEdited.developmentalRoles
        : stringEmpty,
      entityStatus: userBeingEdited.entityStatus
        ? userBeingEdited.entityStatus.statusId
        : stringEmpty,
      jsonDynamicAttributes: userBeingEdited.jsonDynamicAttributes
        ? userBeingEdited.jsonDynamicAttributes
        : stringEmpty
    };

    if (!!userBeingEdited.dateOfBirth) {
      dataJson.dateOfBirth = DateTimeUtil.toSurveyFormat(
        userBeingEdited.dateOfBirth
      );
    }

    dataJson = this.mapJsonDynamicAttributes(userBeingEdited, dataJson, null);
    dataJson.entityStatus = dataJson.entityStatus
      ? StatusTypeEnum[dataJson.entityStatus].text
      : undefined;

    return dataJson;
  }

  private buildSurveyVariablesForEditUser(
    userBeingEdited: UserManagement,
    isEditNormalUser: boolean,
    isCurrentUserHasPermissionToEdit: boolean,
    userDepartment: Department,
    userDepartmentTypes: DepartmentType[]
  ): Promise<CxSurveyjsVariable[]> {
    let surveyjsVariables = [
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.formDisplayMode,
        value: 'edit'
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.currentUser_hasPermissionToEdit,
        value: isCurrentUserHasPermissionToEdit
      }),
      new CxSurveyjsVariable({
        name: 'currentObject_emailAddress',
        value: userBeingEdited.emailAddress
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.isVisibleActivateDate,
        value: this.isEditActivateDate(userBeingEdited.entityStatus.statusId)
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.isPendingUser,
        value: !isEditNormalUser
      }),
      new CxSurveyjsVariable({
        name: 'fromDepartmentId',
        value: userBeingEdited.departmentId
      }),
      new CxSurveyjsVariable({
        name: 'includeChildrenDepartments',
        value: false
      })
    ];
    if (this.surveyJSEnumerationVariables.length) {
      surveyjsVariables = _.union(
        surveyjsVariables,
        this.surveyJSEnumerationVariables
      );
    }

    surveyjsVariables.push(
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.currentObject_availableRoles,
        value: this.cxSurveyjsExtendedService.buildAvailableRolesByDepartmentTypes(
          userDepartmentTypes
        )
      })
    );

    // Get the current user variables to detect for permission changes during creating new users
    return this.cxSurveyjsExtendedService
      .setCurrentUserVariables(this.currentUser)
      .then((userVariables) => {
        return _.union(
          surveyjsVariables,
          this.cxSurveyjsExtendedService.buildCurrentObjectVariables(
            userBeingEdited
          ),
          this.cxSurveyjsExtendedService.buildCurrentObjectDepartmentVariables(
            userDepartment
          ),
          this.cxSurveyjsExtendedService.buildCurrentObjectDepartmentTypes(
            userDepartmentTypes
          ),
          this.cxSurveyjsExtendedService.buildCurrentObjectOrganizationUnitTypes(
            userDepartmentTypes
          ),
          userVariables
        );
      });
  }

  private buildSurveyVariablesForCreateNewUser(
    newUserDepartment: Department,
    newUserDepartmentTypes: DepartmentType[]
  ): Promise<CxSurveyjsVariable[]> {
    let surveyjsVariables = [
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.currentObject_isExternallyMastered,
        value: false
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.formDisplayMode,
        value: 'create'
      }),
      new CxSurveyjsVariable({
        name: 'formDisplayTabs',
        value: ['basic', 'advanced']
      }),
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.isVisibleActivateDate,
        value: true
      }),
      new CxSurveyjsVariable({
        name: 'fromDepartmentId',
        value: UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
      }),
      new CxSurveyjsVariable({
        name: 'includeChildrenDepartments',
        value: true
      })
    ];
    if (this.surveyJSEnumerationVariables.length) {
      surveyjsVariables = _.union(
        surveyjsVariables,
        this.surveyJSEnumerationVariables
      );
    }

    surveyjsVariables.push(
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.currentObject_availableRoles,
        value: this.cxSurveyjsExtendedService.buildAvailableRolesByDepartmentTypes(
          newUserDepartmentTypes
        )
      })
    );

    // Get the current user variables to detect for permission changes during creating new users
    return this.cxSurveyjsExtendedService
      .setCurrentUserVariables(this.currentUser)
      .then((userVariables) => {
        return _.union(
          surveyjsVariables,
          this.cxSurveyjsExtendedService.buildCurrentObjectDepartmentVariables(
            newUserDepartment
          ),
          this.cxSurveyjsExtendedService.buildCurrentObjectDepartmentTypes(
            newUserDepartmentTypes
          ),
          userVariables
        );
      });
  }

  private updatePendingUserListAfterPlaceOfWorkChanged(
    currentPendingCode: string,
    editedUsers: UserManagement[]
  ): void {
    editedUsers.forEach((editedUser) => {
      switch (currentPendingCode) {
        case StatusTypeEnum.PendingApproval1st.code:
          this.updatePlaceOfWorkChangedInUserList(
            editedUser,
            this.pending1stLevelApprovalList
          );
          break;
        case StatusTypeEnum.PendingApproval2nd.code:
          this.updatePlaceOfWorkChangedInUserList(
            editedUser,
            this.pending2ndLevelApprovalList
          );
          break;
        case StatusTypeEnum.PendingApproval3rd.code:
          this.updatePlaceOfWorkChangedInUserList(
            editedUser,
            this.pendingSpecialLevelApprovalList
          );
          break;
        default:
          break;
      }
    });
    // Change reference and run detect changes to update the list.
    switch (currentPendingCode) {
      case StatusTypeEnum.PendingApproval1st.code:
        this.pending1stLevelApprovalList = [
          ...this.pending1stLevelApprovalList
        ];
        break;
      case StatusTypeEnum.PendingApproval2nd.code:
        this.pending2ndLevelApprovalList = [
          ...this.pending2ndLevelApprovalList
        ];
        break;
      case StatusTypeEnum.PendingApproval3rd.code:
        this.pendingSpecialLevelApprovalList = [
          ...this.pendingSpecialLevelApprovalList
        ];
        break;
      default:
        break;
    }
    this.changeDetectorRef.detectChanges();
  }

  /**
   * Updates the user list after the Place of Work of a user has been changed successfully.
   * @param editedUser The user who has been changed the Place of Work.
   * @param currentUserList The whole list containing the edited user.
   */
  private updatePlaceOfWorkChangedInUserList(
    editedUser: UserManagement,
    currentUserList: UserManagement[]
  ): void {
    const matchingUser = currentUserList.find(
      (currentUser) => currentUser.identity.id === editedUser.identity.id
    );
    if (matchingUser) {
      matchingUser.departmentId = editedUser.departmentId;
      matchingUser.departmentName = editedUser.departmentName;
    }
  }

  private updateUserListAfterPlaceOfWorkChanged(
    newDepartmentId: number,
    editedUsers: UserManagement[]
  ): void {
    const currentNavigationDepartmentId = this.departmentModel
      .currentDepartmentId;
    const isFilteringOnSpecificDepartments =
      this.filterParams.parentDepartmentId &&
      this.filterParams.parentDepartmentId.length > 0;

    if (!this.isSearchedAcrossSubOrg && !isFilteringOnSpecificDepartments) {
      this.removeUsersFromList(editedUsers);

      return;
    }

    const rootDepartmentIds = isFilteringOnSpecificDepartments
      ? [...this.filterParams.parentDepartmentId]
      : [currentNavigationDepartmentId];

    const newDepartmentExistingInTheRootDepartments = rootDepartmentIds.includes(
      newDepartmentId
    );
    if (newDepartmentExistingInTheRootDepartments) {
      editedUsers.forEach((editedUser: UserManagement) => {
        this.updatePlaceOfWorkChangedInUserList(
          editedUser,
          this.userItemsData.items
        );
      });
      this.refreshUserList();

      return;
    }

    if (!this.isSearchedAcrossSubOrg) {
      this.removeUsersFromList(editedUsers);

      return;
    }

    const findDepartmentIds = rootDepartmentIds;
    findDepartmentIds.push(newDepartmentId);

    // Determines whether the new department is a descendant department of any root.
    forkJoin(
      findDepartmentIds.map((departmentId: number) => {
        return this.departmentStoreService.getDepartmentById(departmentId);
      })
    ).subscribe((departments: Department[]) => {
      const newDepartment = departments.find(
        (dept) => dept.identity.id === newDepartmentId
      );
      const rootDepartments = departments.filter(
        (dept) => dept.identity.id !== newDepartmentId
      );
      const newDepartmentExistingInBranch =
        newDepartment &&
        rootDepartments.findIndex((dept) =>
          newDepartment.path.includes(dept.path)
        ) > findIndexCommon.notFound;
      if (newDepartmentExistingInBranch) {
        editedUsers.forEach((editedUser: UserManagement) => {
          this.updatePlaceOfWorkChangedInUserList(
            editedUser,
            this.userItemsData.items
          );
        });
        this.refreshUserList();

        return;
      }

      this.removeUsersFromList(editedUsers);
    });
  }

  private removeUsersFromList(removingUsers: UserManagement[]): void {
    removingUsers.forEach((removingUser: UserManagement) => {
      const index = this.findUserIndexInUserList(removingUser);
      if (index < 0) {
        return;
      }
      this.userItemsData.items.splice(index, 1);
    });
    this.refreshUserList();
  }

  private refreshUserList(): void {
    this.userItemsData.items = [...this.userItemsData.items];
    this.clearSelectedItems();
    this.initUserActionsListBasedOnRoles();
    this.changeDetectorRef.detectChanges();
  }

  private async isDepartmentInCurrentList(
    departmentId: number
  ): Promise<boolean> {
    if (isNullOrUndefined(departmentId)) {
      return false;
    }

    const isFilteringOnSpecificDepartments =
      this.filterParams.parentDepartmentId &&
      this.filterParams.parentDepartmentId.length > 0;

    const filteredDepartments = isFilteringOnSpecificDepartments
      ? [...this.filterParams.parentDepartmentId]
      : [this.departmentModel.currentDepartmentId];

    const isFilteringCurrentDepartment = filteredDepartments.includes(
      departmentId
    );

    if (isFilteringCurrentDepartment) {
      return true;
    }

    if (!this.isSearchedAcrossSubOrg) {
      return false;
    }

    filteredDepartments.push(departmentId);
    const getDepartmentIdRequests = filteredDepartments.map((filterDeptId) => {
      return this.departmentStoreService
        .getDepartmentById(filterDeptId)
        .toPromise();
    });

    const departments = await Promise.all(getDepartmentIdRequests);

    const department = departments.find(
      (dept) => dept.identity.id === departmentId
    );

    if (!department) {
      return false;
    }

    const departmentPath = department.path;

    const rootDepartments = departments.filter(
      (dept) => dept.identity.id !== departmentId
    );

    const isDescendantDepartment =
      rootDepartments.findIndex((dept) => departmentPath.includes(dept.path)) >
      -1;

    return isDescendantDepartment;
  }

  private processAssigningApprovingOfficers(
    actionType: string,
    data: any,
    selectedItemMap: any[]
  ): void {
    if (
      actionType === StatusActionTypeEnum.Accept &&
      data.primaryApprovalGroup
    ) {
      const employeeIds = selectedItemMap.map((emp) => emp.identity.id);
      this.addUserToApprovalGroup(data.primaryApprovalGroup, employeeIds);
      if (data.alternateApprovalGroup) {
        this.addUserToApprovalGroup(data.alternateApprovalGroup, employeeIds);
      }
    }
  }

  private updateUserActionBaseOnGridSelection(): void {
    if (!this.userActions || isEmpty(this.userActions.listEssentialActions)) {
      return;
    }

    const selectedUserStatuses = _.uniqBy(
      this.selectedUser,
      'entityStatus.statusId'
    ).map((user: UserManagement) => {
      return user.entityStatus.statusId;
    });

    switch (this.currentTabAriaLabel) {
      case UserAccountTabEnum.UserAccounts:
        if (
          selectedUserStatuses.length &&
          this.isUserActionDisabled(selectedUserStatuses)
        ) {
          this.userActions.listEssentialActions.forEach((item) => {
            if (item.actionType === 'Export') {
              item.disable = false;
            }
          });
        } else if (
          selectedUserStatuses.length &&
          this.isUserActionDisableSetAO(selectedUserStatuses)
        ) {
          this.userActions.listEssentialActions.forEach((item) => {
            item.disable = false;
            if (item.actionType === StatusActionTypeEnum.SetApprovingOfficers) {
              item.disable = true;
            }
          });
        } else if (
          this.selectedUser.length &&
          !this.isUserActionDisabled(selectedUserStatuses)
        ) {
          this.userActions.listEssentialActions.forEach((item) => {
            if (
              item.actionType === StatusActionTypeEnum.AddToGroup ||
              item.actionType === StatusActionTypeEnum.SetApprovingOfficers ||
              item.actionType === StatusActionTypeEnum.Export
            ) {
              item.disable = false;
            }
          });
        }

        break;
      case UserAccountTabEnum.Pending1st:
      case UserAccountTabEnum.Pending2nd:
      case UserAccountTabEnum.Pending3rd:
        this.userActions.listEssentialActions.forEach((item) => {
          if (
            item.actionType === StatusActionTypeEnum.Reject ||
            item.actionType === StatusActionTypeEnum.Accept
          ) {
            item.disable = false;
          }
        });
        break;
      case UserAccountTabEnum.UserOtherPlace:
        this.userActions.listEssentialActions.forEach((item) => {
          if (item.actionType === StatusActionTypeEnum.CreateNewOrgUnit) {
            item.disable = false;
          }
        });
        break;

      default:
        return;
    }
  }

  private isUserActionDisabled(selectedUserStatuses: string[]): boolean {
    return selectedUserStatuses.some(
      (selectedUserStatus: string) =>
        selectedUserStatus === StatusTypeEnum.Deleted.code ||
        selectedUserStatus === StatusTypeEnum.Archived.code ||
        selectedUserStatus === StatusTypeEnum.Rejected.code ||
        selectedUserStatus === StatusTypeEnum.Deactive.code
    );
  }
  private isUserActionDisableSetAO(selectedUserStatuses: string[]): boolean {
    return selectedUserStatuses.some(
      (selectedUserStatus: string) =>
        selectedUserStatus === StatusTypeEnum.Suspended.code ||
        selectedUserStatus === StatusTypeEnum.Inactive.code ||
        selectedUserStatus === StatusTypeEnum.IdentityServerLocked.code
    );
  }
  private filterUserWithoutkOtherPlaceOfWork(
    userDepartments: UserManagement[]
  ): Promise<UserManagement[]> {
    return new Promise<UserManagement[]>((resolve, reject) => {
      this.departmentStoreService
        .getDepartmentExternalById(
          this.OTHER_PLACE_OF_WORK_EXTERNAL_ID,
          AppConstant.ownerId
        )
        .subscribe(
          (user) => {
            const otherPlaceOfWorkDeparmentId = user.identity.id;
            const userWithoutOtherPlaceOfWork = userDepartments.filter(
              (userDepartment) =>
                userDepartment.departmentId !== otherPlaceOfWorkDeparmentId
            );
            resolve(userWithoutOtherPlaceOfWork);
          },
          (err) => reject
        );
    });
  }
}
