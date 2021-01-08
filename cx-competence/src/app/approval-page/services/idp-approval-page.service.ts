import { Injectable } from '@angular/core';
import { environment } from 'app-environments/environment';
import { Identity } from 'app-models/common.model';
import { IDictionary } from 'app-models/dictionary';
import {
  AssignedPDOResultModel,
  ChangeStatusNominateRequestPayload,
  GetLearnerAssignedPDOParams,
  MassAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { AssignPDOHelper } from 'app-services/idp/assign-pdo/assign-pdo.helper';
import { AssignPDOService } from 'app-services/idp/assign-pdo/assign-pdo.service';
import { IDPService } from 'app-services/idp/idp.service';
import { AsyncRespone } from 'app-utilities/cx-async';
import { IdpStatusEnum } from 'app/individual-development/idp.constant';
import {
  NominateStatusCodeEnum,
  NominationChangeStatusAction,
  NominationSource,
  OdpStatusCode,
} from 'app/organisational-development/learning-plan-detail/odp.constant';
import { ODPFilterParams } from 'app/organisational-development/models/odp.models';
import { isEmpty } from 'lodash';
import { ApprovalPageHelper } from '../helpers/approval-page.helper';
import { ChangeNominationStatusTargetEnum } from '../models/approval.enum';
import {
  ChangeStatusClassChangeRequestDTO,
  ChangeStatusClassRegistrationDTO,
  ChangeStatusClassWithdrawalDTO,
  GetClassRegistrationDTO,
  RegistrationDTO,
  RegistrationFilterTypeEnum,
} from '../models/class-registration.dto';
import {
  ClassRegistrationStatusEnum,
  RegistrationModel,
} from '../models/class-registration.model';
import { IChangeStatusModel } from '../models/learning-need-grid-row.model';
import {
  ChangeStatusPdPlanDto,
  PdPlanGridRowModel,
} from '../models/pd-plan-grid-row.model';

@Injectable()
export class IdpApprovalPageService {
  constructor(
    private idpService: IDPService,
    private assignPDOService: AssignPDOService
  ) {}

  //#region Get data
  async getClassRegistrations(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage,
    queryParams: IDictionary<unknown> = {}
  ): Promise<PagingResponseModel<RegistrationModel>> {
    const payload: GetClassRegistrationDTO = {
      ...queryParams,
      pageIndex,
      pageSize,
      registrationFilterType: RegistrationFilterTypeEnum.Registration,
    };

    const response = await this.idpService.getClassRegistrations(payload);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return this.toPagingResgistration(response.data);
  }

  async getClassWithDrawals(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage,
    queryParams: IDictionary<unknown> = {}
  ): Promise<PagingResponseModel<RegistrationModel>> {
    const payload: GetClassRegistrationDTO = {
      ...queryParams,
      pageIndex,
      pageSize,
      registrationFilterType: RegistrationFilterTypeEnum.Withdraw,
    };

    const response = await this.idpService.getClassRegistrations(payload);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return this.toPagingResgistration(response.data);
  }

  async getClassChangeRequests(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage,
    queryParams: IDictionary<unknown> = {}
  ): Promise<PagingResponseModel<RegistrationModel>> {
    const payload: GetClassRegistrationDTO = {
      ...queryParams,
      pageIndex,
      pageSize,
      registrationFilterType: RegistrationFilterTypeEnum.ClassRunChangeRequest,
    };

    const response = await this.idpService.getClassRegistrations(payload);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return this.toPagingResgistration(response.data);
  }

  async getAdhocNominateLearnerRequests(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<PagingResponseModel<AssignedPDOResultModel>> {
    const params = new GetLearnerAssignedPDOParams({
      includeClassRunInfo: true,
      includeCourseInfo: true,
      getForApproval: true,
      status: NominateStatusCodeEnum.PendingForApproval,
      pageIndex,
      pageSize,
    });

    // params.relatedObjectId = -1; // Get request do not releted any object

    const response = await this.idpService.getAdhocNominatedLearnerAsync(
      params
    );
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingNominateLearnerRequest(response.data);
  }

  async getAdhocNominateGroupRequests(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<PagingResponseModel<AssignedPDOResultModel>> {
    const params = new GetLearnerAssignedPDOParams({
      includeClassRunInfo: true,
      includeCourseInfo: true,
      getForApproval: true,
      status: NominateStatusCodeEnum.PendingForApproval,
      pageIndex,
      pageSize,
    });

    const response = await this.idpService.getAdhocNominatedGroupAsync(params);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingNominateGroupRequest(response.data);
  }

  async getAdhocNominateDepartmentRequests(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<any> {
    const params = new GetLearnerAssignedPDOParams({
      includeClassRunInfo: true,
      includeCourseInfo: true,
      getForApproval: true,
      status: NominateStatusCodeEnum.PendingForApproval,
      pageIndex,
      pageSize,
    });

    const response = await this.idpService.getAdhocNominatedDepartmentAsync(
      params
    );

    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingNominateDepartmentRequest(response.data);
  }

  async getAdhocMassNominateRequests(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<PagingResponseModel<MassAssignPDOResultModel>> {
    const params = new GetLearnerAssignedPDOParams({
      includeClassRunInfo: true,
      includeCourseInfo: true,
      getForApproval: true,
      status: NominateStatusCodeEnum.PendingForApproval,
      pageIndex,
      pageSize,
    });

    const response = await this.idpService.getAdhocNominatedMassNominationdAsync(
      params
    );

    if (response.error || isEmpty(response.data)) {
      return null;
    }
    return AssignPDOHelper.toPagingMassNominationRequest(response.data);
  }

  async getLearningNeedRequests(
    departmentId: number,
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<any> {
    const params = new ODPFilterParams({
      departmentIds: [departmentId],
      latestStatusTypeCodes: OdpStatusCode.PendingForApproval,
      excludeAnswer: true,
    });

    const response = await this.idpService.getLearningNeedResultAsync(params);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingLearningNeedRequest(response.data);
  }

  async getPdPlanRequests(
    departmentId: number,
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<PagingResponseModel<PdPlanGridRowModel>> {
    const params = new ODPFilterParams({
      departmentIds: [departmentId],
      latestStatusTypeCodes: OdpStatusCode.PendingForApproval,
      excludeAnswer: true,
    });

    const response = await this.idpService.getPlansResult(params);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingPdPlanRequest(response.data);
  }

  async getPendingPDORequests(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<PagingResponseModel<AssignedPDOResultModel>> {
    const response = await this.idpService.getPendingPDOpportunitiesForApproval();
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingNominateLearnerRequest(response.data);
  }
  //#endregion Get data

  //#region Approve

  async approveClassRegistration(registrationIds: string[]): Promise<boolean> {
    const payload: ChangeStatusClassRegistrationDTO = {
      ids: registrationIds,
      status: ClassRegistrationStatusEnum.Approved,
    };

    const response = await this.idpService.changeStatusClassRegistrations(
      payload
    );

    return this.checkResponse(response);
  }

  async approveClassWithDrawal(registrationIds: string[]): Promise<boolean> {
    const payload: ChangeStatusClassWithdrawalDTO = {
      ids: registrationIds,
      withdrawalStatus: ClassRegistrationStatusEnum.Approved,
    };

    return await this.changeStatusWithDrawal(payload);
  }

  async approveClasChangeRequest(registrationIds: string[]): Promise<boolean> {
    const payload: ChangeStatusClassChangeRequestDTO = {
      ids: registrationIds,
      classRunChangeStatus: ClassRegistrationStatusEnum.Approved,
    };
    const response = await this.idpService.changeStatusClassChangeRequest(
      payload
    );

    return this.checkResponse(response);
  }

  async approveAdhocNominateRequest(
    resultIds: number[],
    changeNominationStatusTarget: ChangeNominationStatusTargetEnum
  ): Promise<boolean> {
    const payload = ApprovalPageHelper.buildChangeNominationStatusPayload(
      resultIds,
      changeNominationStatusTarget
    );

    return await this.assignPDOService.changeStatusNominateRequest(
      payload,
      NominationChangeStatusAction.Approve,
      NominationSource.Adhoc
    );
  }

  async approveAdhocMassNominateRequest(
    resultIds: number[],
    changeNominationStatusTarget: ChangeNominationStatusTargetEnum
  ): Promise<boolean> {
    const payload = ApprovalPageHelper.buildChangeNominationStatusPayload(
      resultIds,
      changeNominationStatusTarget
    );

    return await this.assignPDOService.changeStatusMassNominateRequest(
      payload,
      NominationChangeStatusAction.Approve,
      NominationSource.Adhoc
    );
  }

  async approveLearningNeedRequest(
    resultIdentities: Identity[]
  ): Promise<boolean> {
    const payload: IChangeStatusModel = {
      resultIdentities,
      targetStatusType: {
        assessmentStatusId: IdpStatusEnum.Approved,
      },
    };

    await this.idpService.changeStatusLearningNeeds(payload);

    return true;
  }

  async approvePdPlanRequest(resultIdentities: Identity[]): Promise<boolean> {
    const payload: ChangeStatusPdPlanDto = {
      resultIdentities,
      targetStatusType: {
        assessmentStatusId: IdpStatusEnum.Approved,
      },
    };

    await this.idpService.changeStatusPdPlans(payload);

    return true;
  }

  async approveSelfAssignPDORequest(
    resultIdentities: Identity[]
  ): Promise<boolean> {
    const payload: ChangeStatusPdPlanDto = {
      resultIdentities,
      targetStatusType: {
        assessmentStatusId: IdpStatusEnum.Approved,
      },
    };

    const response = await this.idpService.changeStatusActionItems(payload);

    if (!response || isEmpty(response.data)) {
      return false;
    }

    return true;
  }

  //#endregion Approve

  //#region Reject
  async rejectClassRegistration(
    registrationIds: string[],
    reason?: string
  ): Promise<boolean> {
    const payload: ChangeStatusClassRegistrationDTO = {
      ids: registrationIds,
      status: ClassRegistrationStatusEnum.Rejected,
    };

    if (!isEmpty(reason)) {
      payload.comment = reason;
    }

    return await this.changeStatusRegistration(payload);
  }

  async rejectClassWithDrawal(
    registrationIds: string[],
    reason?: string
  ): Promise<boolean> {
    const payload: ChangeStatusClassWithdrawalDTO = {
      ids: registrationIds,
      withdrawalStatus: ClassRegistrationStatusEnum.Rejected,
    };

    if (!isEmpty(reason)) {
      payload.comment = reason;
    }

    return await this.changeStatusWithDrawal(payload);
  }

  async rejectClassChangeRequest(
    registrationIds: string[],
    reason?: string
  ): Promise<boolean> {
    const payload: ChangeStatusClassChangeRequestDTO = {
      ids: registrationIds,
      classRunChangeStatus: ClassRegistrationStatusEnum.Rejected,
    };

    if (!isEmpty(reason)) {
      payload.comment = reason;
    }

    const response = await this.idpService.changeStatusClassChangeRequest(
      payload
    );

    return this.checkResponse(response);
  }

  async rejectAdhocNominateRequest(
    resultIds: number[],
    changeNominationStatusTarget: ChangeNominationStatusTargetEnum,
    reason?: string
  ): Promise<boolean> {
    const payload: ChangeStatusNominateRequestPayload = {
      target: changeNominationStatusTarget,
      resultIds,
      changePDOpportunityStatus: NominateStatusCodeEnum.Rejected,
    };

    if (!isEmpty(reason)) {
      payload.reason = reason;
    }

    return await this.assignPDOService.changeStatusNominateRequest(
      payload,
      NominationChangeStatusAction.Reject,
      NominationSource.Adhoc
    );
  }

  async rejectAdhocMassNominateRequest(
    resultIds: number[],
    changeNominationStatusTarget: ChangeNominationStatusTargetEnum,
    reason?: string
  ): Promise<boolean> {
    const payload: ChangeStatusNominateRequestPayload = {
      target: changeNominationStatusTarget,
      resultIds,
      changePDOpportunityStatus: NominateStatusCodeEnum.Rejected,
    };

    if (!isEmpty(reason)) {
      payload.reason = reason;
    }

    return await this.assignPDOService.changeStatusMassNominateRequest(
      payload,
      NominationChangeStatusAction.Reject,
      NominationSource.Adhoc
    );
  }

  async rejectLearningNeedRequest(
    resultIdentities: Identity[]
  ): Promise<boolean> {
    const payload: IChangeStatusModel = {
      resultIdentities,
      targetStatusType: {
        assessmentStatusId: IdpStatusEnum.Rejected,
      },
    };

    await this.idpService.changeStatusLearningNeeds(payload);

    return true;
  }

  async rejectPdPlanRequest(resultIdentities: Identity[]): Promise<boolean> {
    const payload: IChangeStatusModel = {
      resultIdentities,
      targetStatusType: {
        assessmentStatusId: IdpStatusEnum.Rejected,
      },
    };

    await this.idpService.changeStatusPdPlans(payload);

    return true;
  }

  async rejectSelfAssignPDORequest(
    resultIdentities: Identity[]
  ): Promise<boolean> {
    const payload: IChangeStatusModel = {
      resultIdentities,
      targetStatusType: {
        assessmentStatusId: IdpStatusEnum.ExternalRejected,
      },
    };

    const response = await this.idpService.changeStatusActionItems(payload);

    if (!response || isEmpty(response.data)) {
      return false;
    }

    return true;
  }

  //#endregion Reject

  private toPagingResgistration(
    pagingRegistration: PagingResponseModel<RegistrationDTO>
  ): PagingResponseModel<RegistrationModel> {
    if (!pagingRegistration) {
      return;
    }

    return new PagingResponseModel<RegistrationModel>({
      totalItems: pagingRegistration.totalItems,
      pageIndex: pagingRegistration.pageIndex,
      pageSize: pagingRegistration.pageSize,
      items: this.toClassRegistrations(pagingRegistration.items),
      hasMoreData: pagingRegistration.hasMoreData,
    });
  }

  private toClassRegistrations(
    classRunRegistrationDTOs: RegistrationDTO[]
  ): RegistrationModel[] {
    if (isEmpty(classRunRegistrationDTOs)) {
      return [];
    }

    return classRunRegistrationDTOs.map((dto) => new RegistrationModel(dto));
  }

  private async changeStatusWithDrawal(
    changeStatusDTO: ChangeStatusClassWithdrawalDTO
  ): Promise<boolean> {
    const response = await this.idpService.changeStatusClassWithdrawals(
      changeStatusDTO
    );
    if (response.error || !response.data) {
      return false;
    }

    return response.data.isSuccess;
  }

  private async changeStatusRegistration(
    changeStatusDTO: ChangeStatusClassRegistrationDTO
  ): Promise<boolean> {
    const response = await this.idpService.changeStatusClassRegistrations(
      changeStatusDTO
    );
    if (response.error || !response.data) {
      return false;
    }

    return response.data.isSuccess;
  }

  private checkResponse(
    response: AsyncRespone<{ isSuccess: boolean }>
  ): boolean {
    if (response.error || !response.data) {
      return false;
    }

    return response.data.isSuccess;
  }
}
