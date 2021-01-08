import { Injectable } from '@angular/core';
import { Identity } from '@conexus/cx-angular-common/lib/models/identity.model';
import { environment } from 'app-environments/environment';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import {
  AssignedPDOResultModel,
  ChangeStatusNominateRequestPayload,
  GetLearnerAssignedPDOParams,
  LearnerAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { AssignPDOHelper } from 'app-services/idp/assign-pdo/assign-pdo.helper';
import { AssignPDOService } from 'app-services/idp/assign-pdo/assign-pdo.service';
import { IDPService } from 'app-services/idp/idp.service';
import { GetMassNominationLearnerParams } from 'app/organisational-development/dtos/odp.dto';
import {
  NominateStatusCodeEnum,
  NominationChangeStatusAction,
  NominationSource,
  OdpActivity,
  OdpStatusCode,
} from 'app/organisational-development/learning-plan-detail/odp.constant';
import { ODPFilterParams } from 'app/organisational-development/models/odp.models';
import { OdpService } from 'app/organisational-development/odp.service';
import { isEmpty } from 'lodash';
import { ApprovalPageHelper } from '../helpers/approval-page.helper';
import {
  ApprovalTargetEnum,
  ChangeNominationStatusTargetEnum,
} from '../models/approval.enum';
import { ILearningPlanGridRowModel } from '../models/learning-plan-grid-row.model';

@Injectable()
export class OdpApprovalPageService {
  constructor(
    private odpService: OdpService,
    private idpService: IDPService,
    private assignPDOService: AssignPDOService
  ) {}

  async rejectRequest(
    identities: Identity[],
    approvalTarget: ApprovalTargetEnum,
    reason: string
  ): Promise<boolean> {
    switch (approvalTarget) {
      case ApprovalTargetEnum.LearningPlan:
        await this.rejectPlanRequests(identities, reason, OdpActivity.Plan);

        return true;
      case ApprovalTargetEnum.LearningDirection:
        await this.rejectPlanRequests(
          identities,
          reason,
          OdpActivity.Direction
        );

        return true;
      default:
        return false;
    }
  }

  async getLearningPlanRequests(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<PagingResponseModel<ILearningPlanGridRowModel>> {
    const params: ODPFilterParams = {
      latestStatusTypeCodes: OdpStatusCode.PendingForApproval,
    };

    const pendingPlans = await this.odpService
      .getPlanResultList(params)
      .toPromise();
    if (!pendingPlans || isEmpty(pendingPlans)) {
      return null;
    }

    const planRowModels: ILearningPlanGridRowModel[] = pendingPlans.map(
      (plan) => {
        return {
          name: {
            display: plan.answer ? plan.answer.Title : '',
            route: `/odp/plan-detail/${plan.resultIdentity.extId}`,
          },
          period: plan.surveyInfo ? plan.surveyInfo.name : 'N/A',
          learningDirections: plan.children ? plan.children.length : 0,
          status: plan.assessmentStatusInfo.assessmentStatusName
            ? plan.assessmentStatusInfo.assessmentStatusName
            : plan.assessmentStatusInfo.assessmentStatusCode,
          completionRate: plan.additionalProperties.completionRate
            ? plan.additionalProperties.completionRate + '%'
            : '0%',
          plan,
          identity: plan.resultIdentity,
        };
      }
    );

    return new PagingResponseModel<ILearningPlanGridRowModel>({
      totalItems: pendingPlans.length,
      pageIndex,
      pageSize,
      items: planRowModels,
      hasMoreData: false,
    });
  }

  async getLearningDirectionRequests(
    pageIndex: number = 0,
    pageSize: number = environment.ItemPerPage
  ): Promise<PagingResponseModel<ILearningPlanGridRowModel>> {
    const params: ODPFilterParams = {
      latestStatusTypeCodes: OdpStatusCode.PendingForApproval,
    };

    const pendingPlans = await this.odpService
      .getDirectionResults(params)
      .toPromise();
    if (!pendingPlans || isEmpty(pendingPlans)) {
      return null;
    }

    const planRowModels: ILearningPlanGridRowModel[] = pendingPlans.map(
      (plan) => {
        return {
          name: {
            display: plan.answer ? plan.answer.Title : '',
            route: `/odp/plan-detail/${plan.parentResultExtId}`,
            params: {
              node: plan.resultIdentity.extId,
            },
          },
          period: plan.surveyInfo ? plan.surveyInfo.name : 'N/A',
          learningDirections: plan.children ? plan.children.length : 0,
          status: plan.assessmentStatusInfo.assessmentStatusName
            ? plan.assessmentStatusInfo.assessmentStatusName
            : plan.assessmentStatusInfo.assessmentStatusCode,
          plan,
          identity: plan.resultIdentity,
        };
      }
    );

    return new PagingResponseModel<ILearningPlanGridRowModel>({
      totalItems: pendingPlans.length,
      pageIndex,
      pageSize,
      items: planRowModels,
      hasMoreData: false,
    });
  }

  async getNominateLearnerRequests(
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

    //params.relatedObjectId = -1; // Get request do not releted any object

    const response = await this.idpService.getNominatedLearnerAsync(params);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingNominateLearnerRequest(response.data);
  }

  async getNominateGroupRequests(
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

    const response = await this.idpService.getNominatedGroupAsync(params);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingNominateGroupRequest(response.data);
  }

  async getNominateDepartmentRequests(
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

    const response = await this.idpService.getNominatedDepartmentAsync(params);
    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingNominateDepartmentRequest(response.data);
  }

  async getMassNominateRequests(
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

    const response = await this.idpService.getNominatedMassNominationdAsync(
      params
    );

    if (response.error || isEmpty(response.data)) {
      return null;
    }

    return AssignPDOHelper.toPagingMassNominationRequest(response.data);
  }

  async approvePlanRequests(
    resultIdentities: Identity[],
    activity: OdpActivity
  ): Promise<PDPlanDto[]> {
    const targetAssessmentStatus = {
      assessmentStatusCode: OdpStatusCode.Approved,
    } as AssessmentStatusInfo;

    return this.odpService
      .changeStatusPlans(resultIdentities, targetAssessmentStatus, activity)
      .toPromise();
  }

  async approveNominateRequest(
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
      NominationSource.OPJ
    );
  }

  async approveMassNominateRequest(
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
      NominationSource.OPJ
    );
  }

  async rejectPlanRequests(
    planResultIdentities: Identity[],
    reason: string,
    activity: OdpActivity
  ): Promise<PDPlanDto[]> {
    const targetAssessmentStatus = {
      assessmentStatusCode: OdpStatusCode.Rejected,
    } as AssessmentStatusInfo;

    return this.odpService
      .changeStatusPlans(planResultIdentities, targetAssessmentStatus, activity)
      .toPromise();
  }

  async rejectNominateRequest(
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
      NominationSource.OPJ
    );
  }

  async rejectMassNominateRequest(
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
      NominationSource.OPJ
    );
  }

  async getMassNominationLearnerAsync(
    departmentId: number,
    resultId: string | number,
    status: NominateStatusCodeEnum,
    timestamp: string,
    pageIndex?: number,
    pageSize?: number
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    const params = new GetMassNominationLearnerParams({
      status,
      resultId,
      timestamp,
      departmentId,
      pageIndex,
      pageSize,
    });

    const response = await this.odpService.getMassNominationLearnerAsync(
      params
    );

    const responseData = response.data;
    if (response.error || !responseData || isEmpty(responseData.items)) {
      return;
    }

    return AssignPDOHelper.toPagingNominateLearnerRequest(responseData);
  }
}
