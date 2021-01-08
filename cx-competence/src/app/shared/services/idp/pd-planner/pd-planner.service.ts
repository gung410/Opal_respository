import { Injectable } from '@angular/core';
import { CxInformationDialogService } from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {
  AssessmentStatusInfo,
  ResultIdentity,
} from 'app-models/assessment.model';
import { Identity } from 'app-models/common.model';
import {
  PDOActionItemDTO,
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOpportunityModel,
} from 'app-models/mpj/pdo-action-item.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ObjectUtilities } from 'app-utilities/object-utils';
import {
  IdpStatusCodeEnum,
  IdpStatusEnum,
} from 'app/individual-development/idp.constant';
import { LnaSurveyLinkComponent } from 'app/individual-development/shared/lna-survey-link/lna-survey-link.component';
import { CareerAspirationChartDataModel } from 'app/organisational-development/models/career-gantt-chart.model';
import {
  DeactivatePDPlanDto,
  IdpDto,
} from 'app/organisational-development/models/idp.model';
import { ODPFilterParams } from 'app/organisational-development/models/odp.models';
import { CxGanttChartModel } from 'app/shared/components/cx-gantt-chart/models/cx-gantt-chart.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { isEmpty } from 'lodash';
import { IDPService } from '../idp.service';
import { ResultHelper } from '../result-helpers';
import { PDPlannerHelpers } from './pd-planner-helpers';

@Injectable()
export class PdPlannerService {
  constructor(
    private idpService: IDPService,
    private pdOpportunityService: PDOpportunityService,
    private infoDialogService: CxInformationDialogService,
    private translateService: TranslateAdapterService,
    private ngbModal: NgbModal
  ) {}

  async getPDPlansAsync(userId: number): Promise<IdpDto[]> {
    let filterParams: ODPFilterParams;

    if (userId) {
      filterParams = new ODPFilterParams({ userIds: [userId] });
    }

    const response = await this.idpService.getPlansResult(filterParams);

    // Check error, if true, it return undefined, and in the caller need to check respone to handle error
    if (response.error) {
      return [];
    }

    let planList =
      response.data && response.data.length > 0 ? response.data : [];
    planList = PDPlannerHelpers.sortPlanByCreatedDate(planList);

    return planList || [];
  }

  async startNewPDPlanAsync(userIdentity: Identity): Promise<IdpDto> {
    const newPlan = PDPlannerHelpers.generateNewPlan(userIdentity);
    const response = await this.idpService.savePlanResult(newPlan);

    if (response.error) {
      return;
    }

    return response.data;
  }

  async addExternalPDOToPlanAsync(
    externalPDODTO: PDOpportunityDTO,
    timestamp: string,
    userIdentity: Identity
  ): Promise<PDOpportunityModel> {
    if (!externalPDODTO) {
      return;
    }

    return await this.addPDOToPlanAsync(
      externalPDODTO,
      userIdentity,
      timestamp
    );
  }

  async addPDOCatalogToPlanAsync(
    pdCatalogCourse: PDOpportunityDetailModel,
    timestamp: string,
    userIdentity: Identity
  ): Promise<PDOpportunityModel> {
    if (!pdCatalogCourse) {
      return;
    }

    const pdoDTO = PDPlannerHelpers.toPDOpportunityDTO(pdCatalogCourse);

    return await this.addPDOToPlanAsync(pdoDTO, userIdentity, timestamp);
  }

  async updateExternalPDOInfoAsync(
    externalPDODto: PDOpportunityDTO,
    answer: PDOpportunityAnswerDTO,
    resultId: number
  ): Promise<PDOpportunityModel> {
    if (!externalPDODto) {
      return;
    }
    const newAnswer = ObjectUtilities.clone(answer);

    newAnswer.learningOpportunity = externalPDODto;
    const resultActionItem = await this.updatePDOInfoAsync(newAnswer, resultId);

    return resultActionItem;
  }

  public async updateExternalPdoCompleteStatus(
    pdoModel: PDOpportunityModel,
    isCompleted: boolean
  ): Promise<PDOpportunityModel> {
    if (
      !pdoModel ||
      !pdoModel.identityActionItemDTO ||
      !pdoModel.identityActionItemDTO.id
    ) {
      return;
    }
    const newActionItem: PDOActionItemDTO = {
      additionalProperties: {
        isCompleted,
      },
      skipCloningResult: true,
    };
    const response = await this.idpService.updateActionItem(
      newActionItem,
      pdoModel.identityActionItemDTO.id
    );
    if (response.error) {
      return;
    }
    const addedPDO = response.data as PDOActionItemDTO;
    if (!addedPDO) {
      return null;
    }

    return PDPlannerHelpers.toPDOpportunityModel(addedPDO);
  }

  async getPlannedPDOsAsync(
    period: number,
    userId: number,
    pageSize: number = 10,
    pageIndex: number = 1
  ): Promise<PagingResponseModel<PDOpportunityModel>> {
    const response = await this.idpService.getPDOpportunitiesOnPDPlan(
      period,
      userId,
      pageSize,
      pageIndex
    );

    if (response.error || !response.data || isEmpty(response.data.items)) {
      return {};
    }

    const pdoList: PDOActionItemDTO[] = response.data.items.filter(
      (pdoActionItem) => !isEmpty(pdoActionItem.answer)
    );
    const plannedPDOsActionItems = PDPlannerHelpers.toArrayPDOpportunityModel(
      pdoList
    );
    const updatedPlannedPDOsActionItems = await this.updateInfoForCataloguePDOAsync(
      plannedPDOsActionItems
    );
    const pagingResponse: PagingResponseModel<PDOpportunityModel> = {
      ...response.data,
      items: updatedPlannedPDOsActionItems,
    };

    return pagingResponse;
  }

  async removePDOOnPlanAsync(resultIdentity: ResultIdentity): Promise<boolean> {
    const deactivatePDPlanDto = new DeactivatePDPlanDto();
    deactivatePDPlanDto.identities = [resultIdentity];
    deactivatePDPlanDto.deactivateAllVersion = true;

    const response = await this.idpService.removeActionItems(
      deactivatePDPlanDto
    );
    if (response.error) {
      return false;
    }

    if (response.data && response.data[0]) {
      const firstReponseItem = response.data[0];
      const successHTTPCode = 200;

      return firstReponseItem.status === successHTTPCode;
    }

    return false;
  }

  async changePlanStatusAsync(
    plan: IdpDto,
    status: IdpStatusCodeEnum
  ): Promise<IdpDto> {
    if (!plan || !status) {
      return;
    }

    const newPlan = { ...plan };
    newPlan.assessmentStatusInfo = {
      assessmentStatusCode: status,
    };

    const response = await this.idpService.savePlanResult(newPlan);

    if (response.error) {
      return;
    }

    return response.data;
  }

  checkCanSubmitPlan(
    plan: IdpDto,
    needsResults: IdpDto[],
    staffProfile: Staff
  ): boolean {
    if (
      !staffProfile ||
      !staffProfile.approvalGroups ||
      !staffProfile.approvalGroups.length
    ) {
      const text = this.translateService.getValueImmediately(
        'MyPdJourney.Message.SubmitWithUnassignAO'
      );
      this.infoDialogService.warning({ message: text });

      return false;
    }

    const currentLearningNeed = PDPlannerHelpers.getPDPlanSamePeriodFromPDPlans(
      plan,
      needsResults
    );
    const isLearningNeedCompleted = ResultHelper.checkIsCompletedResult(
      currentLearningNeed
    );
    if (!isLearningNeedCompleted) {
      const text = this.translateService.getValueImmediately(
        'MyPdJourney.Message.SubmitPlanWithUnacknowledgedLNA'
      );
      this.infoDialogService.warning({ message: text });

      return false;
    }

    return true;
  }

  async getCarrerAspirationData(userExtId: string): Promise<CxGanttChartModel> {
    const response = await this.idpService.getCareerAspirationData(userExtId);

    if (!response || !response.data) {
      console.error('Cannot get Career Aspiration chart data');

      return;
    }

    return new CareerAspirationChartDataModel(response.data);
  }

  async getTodoLNASurvey(): Promise<string> {
    const response = await this.idpService.getTodoLnaSurvey();

    return response?.data?.surveyLink;
  }

  showLNASurveyDialog(link: string): LnaSurveyLinkComponent {
    if (!link) {
      return;
    }

    const ngbModal = this.ngbModal.open(LnaSurveyLinkComponent, {
      centered: true,
      size: 'lg',
      windowClass: 'mobile-dialog-slide-right',
      backdrop: 'static',
    });

    const modalRef = ngbModal.componentInstance as LnaSurveyLinkComponent;
    modalRef.link = link;
    modalRef.clickStart.subscribe(() => ngbModal.close());

    return modalRef;
  }

  private async addPDOToPlanAsync(
    pdoDTO: PDOpportunityDTO,
    userIdentity: Identity,
    timestamp: string
  ): Promise<PDOpportunityModel> {
    if (!pdoDTO || !timestamp) {
      return;
    }

    const pdoAnswer = PDPlannerHelpers.toPDOpportunityAnswer(pdoDTO);
    const actionItemDTO = PDPlannerHelpers.generatePDOActionItem(
      pdoAnswer,
      userIdentity
    );
    if (pdoDTO.extensions.enableExternalPDOApproval) {
      actionItemDTO.assessmentStatusInfo = new AssessmentStatusInfo({
        assessmentStatusId: IdpStatusEnum.ExternalPendingForApproval,
        assessmentStatusCode: IdpStatusCodeEnum.ExternalPendingForApproval,
      });
    }
    const approval = {
      enableExternalPDOApproval: pdoDTO.extensions.enableExternalPDOApproval,
      approvalType: pdoDTO.extensions.approvalType,
      externalPDOApprovingOfficerExtId: pdoDTO.extensions.externalPDOApprovingOfficerExtId?.toLowerCase(),
    };
    actionItemDTO.additionalProperties = {
      ...actionItemDTO.additionalProperties,
      ...approval,
    };

    actionItemDTO.timestamp = timestamp;

    const response = await this.idpService.saveActionItem(actionItemDTO);
    if (response.error) {
      return;
    }

    const addedPDO = response.data as PDOActionItemDTO;
    if (addedPDO) {
      return PDPlannerHelpers.toPDOpportunityModel(addedPDO);
    }

    return null;
  }

  private async updatePDOInfoAsync(
    newAnswer: PDOpportunityAnswerDTO,
    resultId: number
  ): Promise<PDOpportunityModel> {
    if (!newAnswer || !resultId) {
      return;
    }

    const newActionItem: PDOActionItemDTO = {
      answer: newAnswer,
      skipCloningResult: true,
    };

    ObjectUtilities.removeUndefinedFields(newActionItem);

    const response = await this.idpService.updateActionItem(
      newActionItem,
      resultId
    );
    if (response.error) {
      return;
    }

    const addedPDO = response.data as PDOActionItemDTO;
    if (!addedPDO) {
      return null;
    }

    return PDPlannerHelpers.toPDOpportunityModel(addedPDO);
  }

  private async updateInfoForCataloguePDOAsync(
    pdoModelList: PDOpportunityModel[]
  ): Promise<PDOpportunityModel[]> {
    const catalogPDOIds = PDPlannerHelpers.getPDCataloguePDOsIdFromPDOModels(
      pdoModelList
    );
    if (isEmpty(catalogPDOIds)) {
      return pdoModelList;
    }

    const listPDODetail = await this.pdOpportunityService.getPDCatalogPDODetailListAsync(
      catalogPDOIds
    );

    PDPlannerHelpers.updatePDOpportunityModelInfo(pdoModelList, listPDODetail);

    return pdoModelList;
  }
}
