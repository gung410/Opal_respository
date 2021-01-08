import { Injectable } from '@angular/core';
import { CxFooterData } from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { UserBasicInfo } from 'app-models/auth.model';
import { Identity } from 'app-models/common.model';
import { LearnerInfoDTO } from 'app-models/common/learner-info.model';
import { UserCountingParameter } from 'app-models/user-counting-parameter.model';
import { UserCounting } from 'app-models/user-counting.model';
import {
  PagingResponseModel,
  UserManagement,
  UserManagementQueryModel,
} from 'app-models/user-management.model';
import { AsyncRespone, toCxAsync } from 'app-utilities/cx-async';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { ImageHelpers } from 'app-utilities/image-helpers';
import { AcceptExportDto } from 'app/learning-needs-analysis/models/export-learning-needs-analysis-params';
import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import {
  PagedStaffsList,
  Staff,
} from 'app/staff/staff.container/staff-list/models/staff.model';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';
import { AppConstant } from '../app.constant';
import { TranslateAdapterService } from './translate-adapter.service';

@Injectable()
export class UserService {
  parentDepartmentId: number;

  private baseCompetenceUrl: string = AppConstant.api.competence;
  private baseOrganizationUrl: string = AppConstant.api.organization;

  constructor(
    private http: HttpHelpers,
    private authService: AuthService,
    private translateAdapterService: TranslateAdapterService
  ) {
    this.parentDepartmentId = environment.ParentDepartmentId;
  }

  getCurrentUserBasicInfo(): UserBasicInfo {
    const userData = this.authService.userData().getValue();
    if (!userData) {
      return;
    }

    return new UserBasicInfo({
      fullName: userData.fullName,
      email: userData.emails,
      identity: userData.identity,
      avatarUrl: userData.avatarUrl,
    });
  }

  async getStaffProfile(userId?: number): Promise<Staff> {
    const filter = new FilterParamModel();

    if (userId) {
      filter.userIds = [userId];
    } else {
      filter.forCurrentUser = true;
    }

    const response = await toCxAsync(this.getListEmployee(filter).toPromise());
    if (response.error) {
      console.error('Cannot get staff profile');

      return;
    }

    const results = response.data;
    if (!results || !results.items || !results.items.length) {
      return;
    }

    return results.items[0];
  }

  getUserIdentity(): Identity {
    const userData = this.authService.userData().getValue();
    if (!userData || !userData.identity) {
      return;
    }

    return userData.identity;
  }

  getUserId(): number {
    const userIdentity = this.getUserIdentity();

    return userIdentity ? userIdentity.id : undefined;
  }

  getListEmployee(filterParams: FilterParamModel): Observable<PagedStaffsList> {
    const avoidIntercepterCatchError = true;
    const clonedFilterBody = JSON.parse(
      JSON.stringify(filterParams)
    ) as FilterParamModel;
    clonedFilterBody.sortOrder =
      clonedFilterBody.sortOrder === ''
        ? undefined
        : clonedFilterBody.sortOrder;
    clonedFilterBody.sortField =
      clonedFilterBody.sortField === ''
        ? undefined
        : clonedFilterBody.sortField;

    return this.http.post<PagedStaffsList>(
      `${this.baseCompetenceUrl}/idp/employeelist`,
      clonedFilterBody,
      undefined,
      { avoidIntercepterCatchError }
    );
  }

  exportUser(filterParams: FilterParamModel): Observable<AcceptExportDto> {
    return this.http.post(
      `${this.baseCompetenceUrl}/idp/employeelist/export/async`,
      filterParams
    );
  }

  getGravatarImageUrl(
    email: string,
    imageSize: number = 80,
    gravatarTypeD: string = 'mm'
  ): string {
    return ImageHelpers.getAvatarFromEmail(email, imageSize, gravatarTypeD);
  }

  getUsers(
    filterParamModel: UserManagementQueryModel
  ): Observable<PagingResponseModel<UserManagement>> {
    return this.http.get<PagingResponseModel<UserManagement>>(
      `${this.baseOrganizationUrl}/usermanagement/users`,
      filterParamModel
    );
  }

  async getLearnerInfoAsync(
    learnerExtIds: string[]
  ): Promise<AsyncRespone<LearnerInfoDTO[]>> {
    const payload = { userCxIds: learnerExtIds };

    return this.http.postAsync<LearnerInfoDTO[]>(
      `${this.baseOrganizationUrl}/userinfo/public`,
      payload
    );
  }

  async getUserInfoAsync(learnerExtIds: string[]): Promise<LearnerInfoDTO[]> {
    const result = await this.getLearnerInfoAsync(learnerExtIds);

    return result.data;
  }

  async getUserBasicInfoAsync(
    departmentIds: number[],
    userTypeIds: number[]
  ): Promise<AsyncRespone<PagingResponseModel<LearnerInfoDTO>>> {
    const payload = { departmentIds, userTypeIds };

    return this.http.postAsync<PagingResponseModel<LearnerInfoDTO>>(
      `${this.baseCompetenceUrl}/usermanagement/userinfo/basic`,
      payload
    );
  }

  editUser(userDto: UserManagement): Observable<UserManagement> {
    return this.http.put<UserManagement>(
      `${this.baseOrganizationUrl}/usermanagement/users/${userDto.identity.id}`,
      userDto
    );
  }

  getFooterData(releaseDate: string): CxFooterData {
    const copyRightYear = new Date().getFullYear();
    const vulnerabilityText = this.translateAdapterService.getValueImmediately(
      'Footer.ReportVulnerability'
    );
    const privacyStatementText = this.translateAdapterService.getValueImmediately(
      'Footer.PrivacyStatement'
    );
    const termsOfUseText = this.translateAdapterService.getValueImmediately(
      'Footer.TermsOfUse'
    );
    const copyrightText = this.translateAdapterService.getValueImmediately(
      'Footer.CopyRight',
      { year: copyRightYear }
    );
    const rightReservedText = this.translateAdapterService.getValueImmediately(
      'Footer.AllRightReserved'
    );
    const lastUpdatedText = this.translateAdapterService.getValueImmediately(
      'Footer.LastUpdated'
    );
    const dateReleaseText = lastUpdatedText + ': ' + releaseDate;

    return {
      vulnerabilityText,
      privacyStatementText,
      termsOfUseText,
      copyrightText,
      rightReservedText,
      dateReleaseText,
      vulnerabilityUrl: environment.site.footer.vulnerabilityUrl,
      privacyStatementUrl: environment.site.footer.privacyStatementUrl,
      termsOfUseUrl: environment.site.footer.termsOfUseUrl,
    };
  }

  async getUserCountingByUserTypesAsync(
    userCountingParameter: UserCountingParameter
  ): Promise<UserCounting[]> {
    return this.http
      .post<UserCounting[]>(
        `${this.baseOrganizationUrl}/userinfo/getusercountingbyusertypes`,
        userCountingParameter
      )
      .toPromise();
  }
}
