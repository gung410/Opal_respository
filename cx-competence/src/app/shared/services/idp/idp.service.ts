import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ResultIdentity } from 'app-models/assessment.model';
import { ClassRunDTO } from 'app-models/classrun.model';
import { CourseContentItemDTO } from 'app-models/course-content-item.model';
import {
  CourseReviewModel,
  ReviewPagingResponseModel,
} from 'app-models/course-review.model';
import {
  AssignPDOpportunityPayload,
  AssignPDOpportunityResponse,
  ChangeStatusNominateRequestPayload,
  DownloadMassAssignedPDOReportFileModel,
  GetAdhocMassNominationLearnerParams,
  GetAssignedPDOParams,
  GetLearnerAssignedPDOParams,
  LearnerAssignPDOResultModel,
  MassAssignPDOpportunityPayload,
} from 'app-models/mpj/assign-pdo.model';
import {
  ActionItemResultResponeDTO,
  ChangePdPlanStatusTypeResult,
} from 'app-models/mpj/idp.model';
import {
  GetNoRegistrationFinisedPayload,
  NoRegistrationFinishedDto,
} from 'app-models/mpj/no-registration-completed.models';
import {
  AssignedPDOResultDTO,
  MassAssignedPDOResultDTO,
  PDOActionItemDTO,
} from 'app-models/mpj/pdo-action-item.model';
import {
  MassNominationValidationResultDto,
  PDCatalogSearchDTO,
  PDCatalogSearchPayload,
} from 'app-models/pdcatalog/pdcatalog.dto';
import {
  IdpConfigParams,
  LearnerCourseInfo,
  PDPlanConfig,
  ReportData,
} from 'app-models/pdplan.model';
import { SessionDTO } from 'app-models/session.model';
import {
  GetUserGroupParamDTO,
  UserGroupDTO,
} from 'app-models/user-group.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { MetadataTagGetDTO } from 'app-services/pd-opportunity/metadata-tag.model';
import { PDOpportunityDetailGetDTO } from 'app-services/pd-opportunity/pd-opportunity-detail.dto';
import { AsyncRespone, toCxAsync } from 'app-utilities/cx-async';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import {
  ChangeStatusClassChangeRequestDTO,
  ChangeStatusClassRegistrationDTO,
  ChangeStatusClassWithdrawalDTO,
  GetClassRegistrationDTO,
  RegistrationDTO,
} from 'app/approval-page/models/class-registration.dto';
import { IChangeStatusModel } from 'app/approval-page/models/learning-need-grid-row.model';
import { ChangeStatusPdPlanDto } from 'app/approval-page/models/pd-plan-grid-row.model';
import { SearchClassRunType } from 'app/approval-page/services/course-filter.service';
import { CoursepadNoteResponse } from 'app/individual-development/models/course-note.model';
import {
  NominationChangeStatusAction,
  NominationSource,
} from 'app/organisational-development/learning-plan-detail/odp.constant';
import { CareerAspirationChartDataDto } from 'app/organisational-development/models/career-gantt-chart.model';
import {
  DeactivatePDPlanDto,
  IdpDto,
  IdpFilterParams,
} from 'app/organisational-development/models/idp.model';
import { ODPFilterParams } from 'app/organisational-development/models/odp.models';
import { APIConstant, AppConstant, Constant } from 'app/shared/app.constant';
import { Observable } from 'rxjs';

type PagingAsignedPDOResult = PagingResponseModel<AssignedPDOResultDTO>;

@Injectable()
export class IDPService {
  private competenceApiPrefix: string;
  constructor(private httpHelpers: HttpHelpers, private http: HttpClient) {
    this.competenceApiPrefix = AppConstant.api.competence;
  }

  // LEARNING NEED ENDPOINTS
  async getNeedsResults(
    filter: IdpFilterParams
  ): Promise<AsyncRespone<IdpDto[]>> {
    return await this.httpHelpers.getAsync<IdpDto[]>(
      APIConstant.IDP_NEED_RESULTS,
      filter
    );
  }

  async getLearningNeedConfig(
    params: IdpConfigParams
  ): Promise<AsyncRespone<PDPlanConfig>> {
    return await this.httpHelpers.getAsync<PDPlanConfig>(
      APIConstant.IDP_NEED_CONFIG,
      params
    );
  }

  async getLearningNeedReport(
    params: IdpConfigParams
  ): Promise<AsyncRespone<ReportData[]>> {
    return await this.httpHelpers.getAsync<ReportData[]>(
      `${APIConstant.IDP_NEED_REPORTS}/${params.resultId}`
    );
  }

  async saveNeedsResult(
    idpDto: IdpDto,
    avoidIntercepterCatchError?: boolean
  ): Promise<AsyncRespone<IdpDto>> {
    return await this.httpHelpers.postAsync<IdpDto>(
      APIConstant.IDP_NEED_RESULTS,
      idpDto,
      null,
      { avoidIntercepterCatchError }
    );
  }

  // IDP PD PLAN ENDPOINTS
  async getPlansResult(
    params: IdpFilterParams
  ): Promise<AsyncRespone<IdpDto[]>> {
    return await this.httpHelpers.getAsync<IdpDto[]>(
      APIConstant.IDP_PLAN_RESULTS,
      params
    );
  }

  async savePlanResult(dto: IdpDto): Promise<AsyncRespone<IdpDto>> {
    return await this.httpHelpers.postAsync<IdpDto>(
      APIConstant.IDP_PLAN_RESULTS,
      dto
    );
  }

  // ACTION ITEM ENDPOINTS
  async getPDOpportunitiesOnPDPlan(
    period: number,
    userId: number = null,
    pageSize: number = 10,
    pageIndex: number = 1
  ): Promise<AsyncRespone<PagingResponseModel<IdpDto>>> {
    const middleOfYear = new Date(
      Date.UTC(period, Constant.MIDDLE_MONTH_OF_YEAR_VALUE)
    ); // example: 1/6/2019

    return await this.httpHelpers.getAsync<PagingResponseModel<IdpDto>>(
      APIConstant.IDP_PLAN_PDOS,
      {
        userId,
        timestamp: middleOfYear.toISOString(),
        pageSize,
        pageIndex,
      }
    );
  }

  async getPendingPDOpportunitiesForApproval(
    pageSize: number = 10,
    pageIndex: number = 1
  ): Promise<AsyncRespone<PagingResponseModel<IdpDto>>> {
    return await this.httpHelpers.getAsync<PagingResponseModel<IdpDto>>(
      APIConstant.IDP_PLAN_PDOS_PENDIND,
      {
        pageSize,
        pageIndex,
      }
    );
  }

  async getCourseInfoForCurrentUser(
    courseId: string
  ): Promise<AsyncRespone<LearnerCourseInfo>> {
    return await this.httpHelpers.getAsync<LearnerCourseInfo>(
      `${APIConstant.GET_COURSE_INFO_FOR_LEARNER}/${courseId}`,
      undefined,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  //GET_COURSE_REVIEWS_FOR_LEARNER
  async getCourseReviews(
    itemId: string,
    itemTypeFilter: string = 'Course',
    orderBy: string = 'CreatedDate desc',
    skipCount: number = 0,
    maxResultCount: number = 10
  ): Promise<AsyncRespone<ReviewPagingResponseModel<CourseReviewModel>>> {
    return await this.httpHelpers.getAsync<
      ReviewPagingResponseModel<CourseReviewModel>
    >(
      APIConstant.GET_COURSE_REVIEWS_FOR_LEARNER,
      { itemId, itemTypeFilter, orderBy, skipCount, maxResultCount },
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async nomiatePDOs(
    payload: AssignPDOpportunityPayload
  ): Promise<AsyncRespone<AssignPDOpportunityResponse>> {
    return await this.httpHelpers.postAsync<AssignPDOpportunityResponse>(
      APIConstant.POST_NOMINATE,
      payload,
      undefined,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async adhocNomiatePDOs(
    payload: AssignPDOpportunityPayload
  ): Promise<AsyncRespone<AssignPDOpportunityResponse>> {
    return await this.httpHelpers.postAsync<AssignPDOpportunityResponse>(
      APIConstant.POST_ADHOC_NOMINATE,
      payload,
      undefined,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async changeStatusNomiateRequestPDOs(
    payload: ChangeStatusNominateRequestPayload,
    action: NominationChangeStatusAction,
    type: NominationSource
  ): Promise<AsyncRespone<AssignPDOpportunityResponse>> {
    return await this.httpHelpers.postAsync<AssignPDOpportunityResponse>(
      this.buildNominationChangeStatusUrl(action, type),
      payload,
      undefined,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async changeStatusMassNomiateRequestPDOs(
    payload: ChangeStatusNominateRequestPayload,
    action: NominationChangeStatusAction,
    type: NominationSource
  ): Promise<AsyncRespone<AssignPDOpportunityResponse>> {
    return await this.httpHelpers.postAsync<AssignPDOpportunityResponse>(
      this.buildNominationChangeStatusUrl(action, type),
      payload,
      undefined,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  private buildNominationChangeStatusUrl(
    action: NominationChangeStatusAction,
    type: NominationSource
  ) {
    return `${
      this.competenceApiPrefix
    }/idp/${type.toString()}/changeStatus/${action.toString()}`;
  }

  async recommendPDOs(
    payload: AssignPDOpportunityPayload
  ): Promise<AsyncRespone<AssignPDOpportunityResponse>> {
    return await this.httpHelpers.postAsync<AssignPDOpportunityResponse>(
      APIConstant.POST_RECOMMEND_PDOS,
      payload,
      undefined,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getMassNominatedResultAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingResponseModel<MassAssignedPDOResultDTO>>> {
    return await this.httpHelpers.getAsync<
      PagingResponseModel<MassAssignedPDOResultDTO>
    >(`${APIConstant.GET_MASS_NOMINATED_RESULTS}`, params, {
      avoidIntercepterCatchError: true,
    });
  }

  async getAdhocMassNominationLearnerAsync(
    params: GetAdhocMassNominationLearnerParams
  ): Promise<AsyncRespone<PagingResponseModel<LearnerAssignPDOResultModel>>> {
    return await this.httpHelpers.getAsync<
      PagingResponseModel<LearnerAssignPDOResultModel>
    >(`${APIConstant.GET_ADHOC_MASS_NOMINATED_LEARNER}`, params, {
      avoidIntercepterCatchError: true,
    });
  }

  async downloadMassNominationReportFileAsync(
    resultId: number
  ): Promise<AsyncRespone<DownloadMassAssignedPDOReportFileModel>> {
    return await this.httpHelpers.getAsync<DownloadMassAssignedPDOReportFileModel>(
      `${APIConstant.DOWNLOAD_MASS_NOMINATE_REPORT_FILE}`,
      { resultId },
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getAdHocMassNominatedResultAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingResponseModel<MassAssignedPDOResultDTO>>> {
    return await this.httpHelpers.getAsync<
      PagingResponseModel<MassAssignedPDOResultDTO>
    >(`${APIConstant.GET_ADHOC_MASS_NOMINATED_RESULTS}`, params, {
      avoidIntercepterCatchError: true,
    });
  }

  async getNominatedLearnerAsync(
    params: GetLearnerAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_NOMINATED_LEARNERS}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getNominatedGroupAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_NOMINATED_GROUPS}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getNominatedDepartmentAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_NOMINATED_DEPARTMENT}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getNominatedMassNominationdAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingResponseModel<MassAssignedPDOResultDTO>>> {
    return await this.httpHelpers.getAsync<
      PagingResponseModel<MassAssignedPDOResultDTO>
    >(`${APIConstant.GET_MASS_NOMINATED_RESULTS}`, params, {
      avoidIntercepterCatchError: true,
    });
  }

  // Get adhoc nomiantion results
  async getAdhocNominatedLearnerAsync(
    params: GetLearnerAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_ADHOC_NOMINATED_LEARNERS}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getAdhocNominatedGroupAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_ADHOC_NOMINATED_GROUPS}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getAdhocNominatedDepartmentAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_ADHOC_NOMINATED_DEPARTMENT}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getLearningNeedResultAsync(
    params: ODPFilterParams
  ): Promise<AsyncRespone<IdpDto[]>> {
    return await this.httpHelpers.getAsync<IdpDto[]>(
      `${APIConstant.IDP_NEED_RESULTS}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getAdhocNominatedMassNominationdAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingResponseModel<MassAssignedPDOResultDTO>>> {
    return await this.httpHelpers.getAsync<
      PagingResponseModel<MassAssignedPDOResultDTO>
    >(`${APIConstant.GET_ADHOC_MASS_NOMINATED_RESULTS}`, params, {
      avoidIntercepterCatchError: true,
    });
  }

  // Get adhoc recommendation results
  async getRecommendedLearnerAsync(
    params: GetLearnerAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_RECOMMENDED_LEARNERS}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getRecommendedGroupAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_RECOMMENDED_GROUPS}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getRecommendedDepartmentAsync(
    params: GetAssignedPDOParams
  ): Promise<AsyncRespone<PagingAsignedPDOResult>> {
    return await this.httpHelpers.getAsync<PagingAsignedPDOResult>(
      `${APIConstant.GET_RECOMMENDED_DEPARTMENT}`,
      params,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async removeAssignedPDOResult(
    resultIdentity: ResultIdentity[]
  ): Promise<AsyncRespone<DeactivatePDPlanDto>> {
    const deactivatePDPlanDto = new DeactivatePDPlanDto();
    deactivatePDPlanDto.identities = resultIdentity;
    deactivatePDPlanDto.deactivateAllVersion = true;

    return this.httpHelpers.postAsync<DeactivatePDPlanDto>(
      APIConstant.IDP_ACTION_ITEM_DEACTIVATE,
      deactivatePDPlanDto,
      undefined,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getActionItems(
    payload: ODPFilterParams
  ): Promise<AsyncRespone<PDOActionItemDTO[]>> {
    const url = `${APIConstant.IDP_ACTION_ITEM_RESULTS}`;

    return await this.httpHelpers.getAsync<PDOActionItemDTO[]>(url, payload);
  }

  async getTodoLnaSurvey(): Promise<AsyncRespone<{ surveyLink: string }>> {
    const url = `${APIConstant.GET_LNA_SURVEY}`;

    return await this.httpHelpers.getAsync<{ surveyLink: string }>(url);
  }

  async updateActionItem(
    actionItem: IdpDto | PDOActionItemDTO,
    resultId: number
  ): Promise<AsyncRespone<IdpDto>> {
    const url = `${APIConstant.IDP_ACTION_ITEM_RESULTS}/${resultId}`;

    return await this.httpHelpers.postAsync<IdpDto>(url, actionItem);
  }

  async saveActionItem(
    actionItem: IdpDto | PDOActionItemDTO
  ): Promise<AsyncRespone<IdpDto>> {
    return await this.httpHelpers.postAsync<IdpDto>(
      APIConstant.IDP_ACTION_ITEM_RESULTS,
      actionItem
    );
  }

  async removeActionItems(
    deactivatePDPlanDto: DeactivatePDPlanDto
  ): Promise<AsyncRespone<ActionItemResultResponeDTO[]>> {
    return await this.httpHelpers.postAsync<ActionItemResultResponeDTO[]>(
      APIConstant.IDP_ACTION_ITEM_DEACTIVATE,
      deactivatePDPlanDto
    );
  }

  async getListCourseDetailByIds(
    courseIds: string[]
  ): Promise<AsyncRespone<PDOpportunityDetailGetDTO[]>> {
    const validCourseIds = courseIds.filter(this.isGuid);

    return await this.httpHelpers.postAsync<PDOpportunityDetailGetDTO[]>(
      APIConstant.PD_CATALOGUE_GET_BY_IDS,
      validCourseIds,
      null,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getBookmarkedPDOs(): Promise<AsyncRespone<CoursepadNoteResponse>> {
    if (this.competenceApiPrefix.includes('development')) {
      return new AsyncRespone(null, undefined);
    }

    return this.httpHelpers.getAsync<CoursepadNoteResponse>(
      APIConstant.GET_PD_BOOKMARK,
      null
    );
  }

  async getActionItemsConfig(): Promise<AsyncRespone<PDPlanConfig>> {
    return await this.httpHelpers.getAsync<PDPlanConfig>(
      APIConstant.IDP_ACTION_ITEM_CONFIG
    );
  }

  saveBookmarkToPlan(courseId: string): Observable<object> {
    return this.httpHelpers.post(APIConstant.POST_PD_BOOKMARK, { courseId });
  }

  uncheckBookmark(courseId: string): Observable<object> {
    return this.httpHelpers.delete(APIConstant.DELETE_PD_BOOKMARK, {
      courseId,
    });
  }

  async getClassRegistrations(
    getClassRegistrationDTO: GetClassRegistrationDTO
  ): Promise<AsyncRespone<PagingResponseModel<RegistrationDTO>>> {
    const url = APIConstant.CLASS_REGISTRATIONS;

    return this.httpHelpers.postAsync<PagingResponseModel<RegistrationDTO>>(
      url,
      getClassRegistrationDTO
    );
  }

  async changeStatusClassRegistrations(
    changeStatusDTO: ChangeStatusClassRegistrationDTO
  ): Promise<AsyncRespone<{ isSuccess: boolean }>> {
    const url = APIConstant.CLASS_REGISTRATIONS_CHANGE_STATUS;

    return this.httpHelpers.postAsync<{ isSuccess: boolean }>(
      url,
      changeStatusDTO
    );
  }

  changeStatusLearningNeeds(
    changeStatusDTO: IChangeStatusModel
  ): Promise<unknown> {
    const url = APIConstant.IDP_NEED_CHANGE_STATUS;

    return this.httpHelpers.postAsync<unknown>(url, changeStatusDTO);
  }

  changeStatusPdPlans(
    changeStatusDTO: ChangeStatusPdPlanDto
  ): Promise<unknown> {
    const url = APIConstant.IDP_PLAN_CHANGE_STATUS;

    return this.httpHelpers.postAsync<unknown>(url, changeStatusDTO);
  }

  changeStatusActionItems(
    changeStatusDTO: ChangeStatusPdPlanDto
  ): Promise<AsyncRespone<ChangePdPlanStatusTypeResult[]>> {
    const url = APIConstant.IDP_ACTION_ITEM_CHANGE_STATUS;

    return this.httpHelpers.postAsync<ChangePdPlanStatusTypeResult[]>(
      url,
      changeStatusDTO
    );
  }

  async changeStatusClassWithdrawals(
    changeStatusDTO: ChangeStatusClassWithdrawalDTO
  ): Promise<AsyncRespone<{ isSuccess: boolean }>> {
    const url = APIConstant.CLASS_WITHDRAWS_CHANGE_STATUS;

    return this.httpHelpers.postAsync<{ isSuccess: boolean }>(
      url,
      changeStatusDTO
    );
  }

  async changeStatusClassChangeRequest(
    changeStatusDTO: ChangeStatusClassChangeRequestDTO
  ): Promise<AsyncRespone<{ isSuccess: boolean }>> {
    const url = APIConstant.CLASS_CHANGE_REQUEST_CHANGE_STATUS;

    return this.httpHelpers.postAsync<{ isSuccess: boolean }>(
      url,
      changeStatusDTO
    );
  }

  getClassRunsByCourseId(
    courseId: string,
    pageIndex: number = 0,
    pageSize: number = 10
  ): Observable<PagingResponseModel<ClassRunDTO>> {
    return this.httpHelpers.get<PagingResponseModel<ClassRunDTO>>(
      APIConstant.GET_CLASSRUN_BY_COURSEID,
      { courseId, pageIndex, pageSize },
      { avoidIntercepterCatchError: true }
    );
  }

  getSessionsByClassRunId(
    classRunId: string,
    searchType: string = 'Learner',
    skipCount: number = 0,
    maxResultCount: number = 10
  ): Promise<AsyncRespone<PagingResponseModel<SessionDTO>>> {
    return this.httpHelpers.getAsync<PagingResponseModel<SessionDTO>>(
      APIConstant.GET_SESSION_BY_CLASSRUNID,
      { classRunId, searchType, skipCount, maxResultCount }
    );
  }

  getSessionsByClassRunIds(
    classRunId: string[]
  ): Promise<AsyncRespone<SessionDTO[]>> {
    return this.httpHelpers.postAsync<SessionDTO[]>(
      APIConstant.GET_SESSION_BY_CLASSRUNIDS,
      classRunId
    );
  }

  getTableOfContent(
    courseId: string,
    classRunId?: string,
    includeAdditionalInfo: boolean = false
  ): Promise<AsyncRespone<CourseContentItemDTO[]>> {
    return this.httpHelpers.getAsync<CourseContentItemDTO[]>(
      `${APIConstant.GET_TABLEOFCONTENT}/${courseId}/toc`,
      { courseId, classRunId, includeAdditionalInfo }
    );
  }

  getUserGroupByDepartmentId(
    departmentId: number,
    countActiveMembers: boolean = true,
    orderBy: string = 'name'
  ): Observable<PagingResponseModel<UserGroupDTO>> {
    const param: GetUserGroupParamDTO = {
      departmentIds: departmentId,
      countActiveMembers,
      orderBy,
    };
    const url = APIConstant.GET_USER_GROUP;

    return this.httpHelpers.get<PagingResponseModel<UserGroupDTO>>(url, param);
  }

  async getClassRunById(
    classRunId: string
  ): Promise<AsyncRespone<ClassRunDTO>> {
    return this.httpHelpers.getAsync<ClassRunDTO>(
      `${APIConstant.PD_CLASSRUN_GET_BY_ID}/${classRunId}`
    );
  }

  async getClassRunByCourseId(
    courseId?: string,
    pageIndex: number = 0,
    pageSize: number = 1000
  ): Promise<AsyncRespone<PagingResponseModel<ClassRunDTO>>> {
    const filterParam = {
      courseId,
      page: pageIndex,
      SkipCount: pageIndex * pageSize,
      searchType: SearchClassRunType.OrganisationDevelopment,
    };

    return this.httpHelpers.postAsync<PagingResponseModel<ClassRunDTO>>(
      APIConstant.PD_CLASSRUN_GET_BY_COURSE_ID,
      filterParam,
      {
        avoidIntercepterCatchError: true,
      }
    );
  }

  async getMetadataTag(): Promise<AsyncRespone<MetadataTagGetDTO[]>> {
    const url = APIConstant.GET_PDO_METADATA_TAG;

    return this.httpHelpers.getAsync<MetadataTagGetDTO[]>(url, null, {
      avoidIntercepterCatchError: true,
    });
  }

  async searchPDCatalog(
    payload: PDCatalogSearchPayload
  ): Promise<AsyncRespone<PDCatalogSearchDTO>> {
    const url = APIConstant.PD_CATALOGUE_SEARCH;

    return this.httpHelpers.postAsync<PDCatalogSearchDTO>(url, payload, null, {
      avoidIntercepterCatchError: true,
    });
  }

  async uploadMassNominationFile(
    payload: MassAssignPDOpportunityPayload
  ): Promise<AsyncRespone<MassNominationValidationResultDto>> {
    const formData = payload.toFormData();
    const headers = new HttpHeaders();
    return await toCxAsync(
      this.http
        .post<MassNominationValidationResultDto>(
          APIConstant.CHECK_VALID_MASS_NOMINATION,
          formData,
          { headers }
        )
        .toPromise()
    );
  }

  async validateMassNominationFile(
    file: File
  ): Promise<AsyncRespone<MassNominationValidationResultDto>> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    const headers = new HttpHeaders();
    return await toCxAsync(
      this.http
        .post<MassNominationValidationResultDto>(
          APIConstant.CHECK_VALID_MASS_NOMINATION_FILE,
          formData,
          { headers }
        )
        .toPromise()
    );
  }

  async massNominate(payload: MassAssignPDOpportunityPayload): Promise<any> {
    const formData = payload.toFormData();
    const headers = new HttpHeaders();
    return await toCxAsync(
      this.http
        .post(APIConstant.POST_MASS_NOMINATE, formData, { headers })
        .toPromise()
    );
  }
  async adHocMassNominate(
    payload: MassAssignPDOpportunityPayload
  ): Promise<any> {
    const formData = payload.toFormData();
    const headers = new HttpHeaders();

    return await toCxAsync(
      this.http
        .post(APIConstant.POST_ADHOC_MASS_NOMINATE, formData, { headers })
        .toPromise()
    );
  }

  getCareerAspirationData(
    userExtId: string
  ): Promise<AsyncRespone<CareerAspirationChartDataDto>> {
    return this.httpHelpers.getAsync<CareerAspirationChartDataDto>(
      `${APIConstant.IDP_NEED_CARRER_ASPIRATION}/${userExtId}`,
      null,
      { avoidIntercepterCatchError: true }
    );
  }

  async getNORegistrationFinished(
    payload: GetNoRegistrationFinisedPayload
  ): Promise<AsyncRespone<NoRegistrationFinishedDto>> {
    return await this.httpHelpers.getAsync<NoRegistrationFinishedDto>(
      APIConstant.PD_NO_REGISTRATION_FINISHED,
      payload
    );
  }

  private isGuid(value: string): boolean {
    const regex = /[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i;
    const match = regex.exec(value);

    return match != null;
  }
}
