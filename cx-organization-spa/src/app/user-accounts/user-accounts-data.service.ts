import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'app-environments/environment';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AsyncResponse, toCxAsync } from 'app/core/cx-async';
import { DepartmentQueryModel } from 'app/department-hierarchical/models/filter-params.model';
import { AppConstant } from 'app/shared/app.constant';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { UserTypeEnum } from 'app/shared/constants/user-type.enum';
import { MembershipDto } from 'app/user-groups/user-groups.model';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';

import {
  ApprovalGroup,
  ApprovalGroupQueryModel
} from './models/approval-group.model';
import { MassUsersCreationValidationResultDto } from './models/mass-users-creation-exception.model';
import { InstructionReporting } from './models/reporting-by-systemrole.model';
import {
  IGetUserBasicInfoParameters,
  UserBasicInfo
} from './models/user-basic-info.model';
import {
  PagingResponseModel,
  UserManagement,
  UserManagementQueryModel
} from './models/user-management.model';

@Injectable()
export class UserAccountsDataService {
  constructor(
    private httpHelper: HttpHelpers,
    private http: HttpClient,
    private toastrService: ToastrService,
    private translateAdapterService: TranslateAdapterService
  ) {}

  getUsers(
    filterParamModel: UserManagementQueryModel
  ): Observable<PagingResponseModel<UserManagement>> {
    return this.httpHelper.get<PagingResponseModel<UserManagement>>(
      `${AppConstant.api.organization}/usermanagement/users`,
      filterParamModel.preProcessSpecialFields()
    );
  }

  getUsersBasicInfo(
    getUserBasicInfoParameters: IGetUserBasicInfoParameters
  ): Observable<PagingResponseModel<UserBasicInfo>> {
    return this.httpHelper.post<PagingResponseModel<UserBasicInfo>>(
      `${AppConstant.api.organization}/userinfo/basic`,
      getUserBasicInfoParameters
    );
  }

  getUserInfoWithPost(
    filterParamModel: any
  ): Observable<PagingResponseModel<UserManagement>> {
    const url = `${AppConstant.api.organization}/usermanagement/get_users`;

    return this.httpHelper.post(url, filterParamModel);
  }
  getHierarchicalDepartments(
    departmentId: number,
    departmentQuery: DepartmentQueryModel
  ): Observable<[]> {
    if (departmentQuery.searchText) {
      departmentQuery.maxChildrenLevel = undefined;
    }

    return this.httpHelper.get(
      `${AppConstant.api.organization}/departments/${departmentId}/hierarchydepartmentidentifiers`,
      departmentQuery
    );
  }

  // This function is not used yet.
  getSystemRoles(): Observable<any> {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: UserTypeEnum.SystemRole,
      includeLocalizedData: true.toString()
    });
  }

  editUser(userDto: UserManagement): Observable<UserManagement> {
    return this.httpHelper.put<UserManagement>(
      `${AppConstant.api.organization}/usermanagement/users/${userDto.identity.id}`,
      userDto
    );
  }

  createUser(userDTO: any): Observable<any> {
    return this.httpHelper.post(
      `${AppConstant.api.organization}/usermanagement/users`,
      userDTO
    );
  }

  getPersonnelGroups(): Observable<any> {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: UserTypeEnum.PersonnelGroup,
      includeLocalizedData: true.toString()
    });
  }

  getCareerPaths(): Observable<any> {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: UserTypeEnum.CareerPath,
      includeLocalizedData: true.toString()
    });
  }

  getCategoryInServiceScheme(serviceSchemeId: string): Observable<any> {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: UserTypeEnum.ExperienceCategory,
      parentIds: serviceSchemeId,
      includeLocalizedData: true.toString()
    });
  }

  getDevelopmentalRole(categoryId: string): Observable<any> {
    return this.httpHelper.get(`${AppConstant.api.organization}/usertypes`, {
      archetypeEnums: UserTypeEnum.DevelopmentalRole,
      parentIds: categoryId,
      includeLocalizedData: true.toString()
    });
  }

  changeEmployeeStatus(userDto: any): Observable<any> {
    return this.httpHelper.put(
      `${AppConstant.api.organization}/usermanagement/users/${userDto.identity.id}`,
      userDto
    );
  }

  createApprovalGroup(approvalGroupDto: any): Observable<ApprovalGroup> {
    return this.httpHelper.post<ApprovalGroup>(
      `${AppConstant.api.organization}/approvalgroups`,
      approvalGroupDto
    );
  }

  updateApprovalGroup(approvalGroupDto: any): Observable<ApprovalGroup> {
    return this.httpHelper.put<ApprovalGroup>(
      `${AppConstant.api.organization}/approvalgroups/${approvalGroupDto.identity.id}`,
      approvalGroupDto
    );
  }

  getApprovalGroups(
    params: ApprovalGroupQueryModel
  ): Observable<ApprovalGroup[]> {
    return this.httpHelper.get<ApprovalGroup[]>(
      `${AppConstant.api.organization}/approvalgroups`,
      params
    );
  }

  addUsersToApprovalGroup(
    employeeIds: number[],
    memberDto: MembershipDto
  ): Observable<any> {
    return this.httpHelper.post(
      `${AppConstant.api.organization}/employees/memberships/approvalgroups`,
      memberDto,
      {
        employeeIds: employeeIds.map((item) => item.toString())
      }
    );
  }

  removeUsersFromApprovalGroup(
    employeeIds: number[],
    memberDto: MembershipDto
  ): Observable<any> {
    return this.httpHelper.delete(
      `${AppConstant.api.organization}/employees/memberships/approvalgroups`,
      memberDto,
      {
        employeeIds: employeeIds.map((item) => item.toString())
      }
    );
  }

  getStatusHistoricalData(userId: number): Observable<any> {
    const getBooksQuery = () => `{
            eventMany(filter: {OR:[
                {routing: {action: "cx-organization-api.crud.entitystatus_changed.employee", entityId: "${userId}"}},
                {routing: {action: "cx-organization-api.crud.created.employee"},
                    payload: {
                        body: {
                            userData: {
                                identity: {
                                    id: ${userId}
                                }
                            }
                        }
                    }
                }
            ]}) {
              _id
              type
              version
              id
              created
              routing {
                action
                actionVersion
                entity
                entityId
              }
              payload {
                identity {
                  clientId
                  customerId
                  sourceIp
                  userId
                  onBehalfOfUser
                }
                references {
                  externalId
                  correlationId
                  commandId
                  eventId
                }
                body
              }
            }
          }`;

    return this.httpHelper.post(
      `${environment.apiGatewayOrigin}/${environment.apiGatewayResource.dataHub}/event`,
      { query: getBooksQuery() }
    );
  }

  deleteUser(
    userId: number,
    entityStatusReason: string
  ): Observable<UserManagement> {
    return this.httpHelper.delete<UserManagement>(
      `${AppConstant.api.organization}/usermanagement/users/${userId}?entityStatusReason=${entityStatusReason}`
    );
  }

  archiveUser(
    userId: number,
    entityStatusReason: string
  ): Observable<UserManagement> {
    return this.httpHelper.post<UserManagement>(
      `${AppConstant.api.organization}/usermanagement/archive/${userId}?entityStatusReason=${entityStatusReason}`
    );
  }

  unarchiveUser(userId: number): Observable<UserManagement> {
    return this.httpHelper.post<UserManagement>(
      `${AppConstant.api.organization}/usermanagement/unarchive/${userId}`
    );
  }

  exportAsyncAccounts(options: object): Observable<InstructionReporting> {
    return this.httpHelper.post<InstructionReporting>(
      `${AppConstant.api.organization}/usermanagement/users/export/async`,
      options
    );
  }

  async getUsersWithOtherPlaceOfWorkAync(
    searchKey: string,
    pageIndex: number,
    pageSize: number,
    filterParam?: UserManagementQueryModel
  ): Promise<PagingResponseModel<UserManagement>> {
    const response = await this.getUsersWithOtherPlaceOfWork(
      searchKey,
      pageIndex,
      pageSize,
      filterParam
    );
    const responseData = response.data;

    if (response.error || (responseData && !responseData.items)) {
      return;
    }

    return responseData;
  }

  async validateMassUsersCreation(
    uploadedFile: File
  ): Promise<MassUsersCreationValidationResultDto> {
    const result = await this.validateMassUsersCreationData(uploadedFile);
    if (!result && !result.data) {
      this.toastrService.error(
        this.translateAdapterService.getValueImmediately(
          'RequestErrorMessage.504'
        )
      );

      return;
    }

    return result.data;
  }

  async createMassUsers(
    uploadedFile: File
  ): Promise<MassUsersCreationValidationResultDto> {
    const result = await this.uploadMassUsersCreationData(uploadedFile);
    if (!result && !result.data) {
      this.toastrService.error(
        this.translateAdapterService.getValueImmediately(
          'RequestErrorMessage.504'
        )
      );

      return null;
    }

    return result.data;
  }

  async validateMassUsersCreationFile(
    uploadedFile: File
  ): Promise<MassUsersCreationValidationResultDto> {
    const result = await this.validateMassUserCreationFile(uploadedFile);
    if (!result && !result.data) {
      this.toastrService.error(
        this.translateAdapterService.getValueImmediately(
          'RequestErrorMessage.504'
        )
      );

      return;
    }

    return result.data;
  }

  private async getUsersWithOtherPlaceOfWork(
    searchKey: string,
    pageIndex: number,
    pageSize: number,
    filterParam?: UserManagementQueryModel
  ): Promise<AsyncResponse<PagingResponseModel<UserManagement>>> {
    const otherDepartmentId: number = environment.OtherDepartmentId;
    const defaultStatuses = [
      StatusTypeEnum.PendingApproval1st.code,
      StatusTypeEnum.PendingApproval2nd.code,
      StatusTypeEnum.PendingApproval3rd.code
    ];
    const defaultFilter = new UserManagementQueryModel();
    defaultFilter.searchKey = searchKey;
    defaultFilter.pageSize = pageSize;
    defaultFilter.pageIndex = pageIndex;
    defaultFilter.parentDepartmentId = [otherDepartmentId];
    defaultFilter.userEntityStatuses = defaultStatuses;
    const filter = filterParam
      ? {
          ...filterParam,
          parentDepartmentId: otherDepartmentId,
          pageSize,
          pageIndex,
          searchKey,
          userEntityStatuses: filterParam.userEntityStatuses || defaultStatuses
        }
      : defaultFilter;

    return this.httpHelper.getAsync<PagingResponseModel<UserManagement>>(
      `${AppConstant.api.organization}/usermanagement/users`,
      filter
    );
  }

  private async validateMassUsersCreationData(
    uploadedFile: File
  ): Promise<AsyncResponse<MassUsersCreationValidationResultDto>> {
    const formData = new FormData();
    formData.append('file', uploadedFile, uploadedFile.name);
    const headers = new HttpHeaders();

    return await toCxAsync(
      this.http
        .post<MassUsersCreationValidationResultDto>(
          `${AppConstant.api.organization}/usermanagement/masscreations/data/validate`,
          formData,
          { headers }
        )
        .toPromise()
    );
  }

  private async uploadMassUsersCreationData(
    uploadedFile: File
  ): Promise<AsyncResponse<any>> {
    const formData = new FormData();
    formData.append('file', uploadedFile, uploadedFile.name);
    const headers = new HttpHeaders();

    return await toCxAsync(
      this.http
        .post<any>(
          `${AppConstant.api.organization}/usermanagement/masscreations/create`,
          formData,
          { headers }
        )
        .toPromise()
    );
  }

  private async validateMassUserCreationFile(
    uploadedFile: File
  ): Promise<AsyncResponse<MassUsersCreationValidationResultDto>> {
    const formData = new FormData();
    formData.append('file', uploadedFile, uploadedFile.name);
    const headers = new HttpHeaders({});

    return await toCxAsync(
      this.http
        .post<MassUsersCreationValidationResultDto>(
          `${AppConstant.api.organization}/usermanagement/masscreations/file/validate`,
          formData,
          { headers }
        )
        .toPromise()
    );
  }
}
