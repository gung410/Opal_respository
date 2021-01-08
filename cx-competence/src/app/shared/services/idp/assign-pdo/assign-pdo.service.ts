import { Injectable } from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ArcheTypeEnum } from 'app-enums/user-type.enum';
import { ResultIdentity } from 'app-models/assessment.model';
import { LearnerInfoDTO } from 'app-models/common/learner-info.model';
import {
  AssignPDOpportunityPayload,
  AssignPDOpportunityResponse,
  ChangeStatusNominateRequestPayload,
  DepartmentAssignPDOResultModel,
  GetAdhocMassNominationLearnerParams,
  GetLearnerAssignedPDOParams,
  GetMassAssignedPDOParams,
  GroupAssignPDOResultModel,
  LearnerAssignPDOResultModel,
  MassAssignPDOpportunityPayload,
  MassAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import {
  AssignedPDOResultDTO,
  MassAssignedPDOResultDTO,
  PDOActionItemDTO,
  PDOAddType,
} from 'app-models/mpj/pdo-action-item.model';
import { MassNominationValidationResultDto } from 'app-models/pdcatalog/pdcatalog.dto';
import { UserGroupModel } from 'app-models/user-group.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { IDPService } from 'app-services/idp/idp.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { UserService } from 'app-services/user.service';
import { AsyncRespone, toCxAsync } from 'app-utilities/cx-async';
import { AssignPDOResultDialogComponent } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/assign-pdo-result-dialog/assign-pdo-result-dialog.component';
import { AssignResultModel } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/assign-pdo-result-dialog/assign-pdo-result-dialog.model';
import { AssignModeEnum } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/planned-pdo-detail.model';
import {
  NominateStatusCodeEnum,
  NominationChangeStatusAction,
  NominationSource,
} from 'app/organisational-development/learning-plan-detail/odp.constant';
import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { isEmpty } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { DownloadMassAssignedPDOReportFileModel } from './../../../models/mpj/assign-pdo.model';
import { AssignPDOHelper } from './assign-pdo.helper';

@Injectable()
export class AssignPDOService {
  constructor(
    private ngbModal: NgbModal,
    private idpService: IDPService,
    private userService: UserService,
    private toastrService: ToastrService,
    private globalLoader: CxGlobalLoaderService,
    private translateAdapterService: TranslateAdapterService
  ) {}

  getLearnersObs = (
    searchKey?: string,
    departmentId?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    const filterParams = AssignPDOHelper.filterParamLearnerBuilder(
      searchKey,
      departmentId
    );

    return this.userService
      .getListEmployee(filterParams)
      .pipe(map(AssignPDOHelper.mapPagedStaffsToCxSelectItems));
  };

  getAvailableLearnersObs = (
    searchKey?: string,
    departmentId?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    const filterParams = AssignPDOHelper.filterParamLearnerBuilder(
      searchKey,
      departmentId
    );

    return this.userService
      .getListEmployee(filterParams)
      .pipe(map(AssignPDOHelper.mapPagedStaffsToCxSelectItems));
  };

  getApprovingOfficerObs = (
    searchKey?: string,
    departmentId?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    const filterParams = AssignPDOHelper.filterParamApprovingOfficerBuilder(
      searchKey,
      departmentId
    );

    return this.userService
      .getListEmployee(filterParams)
      .pipe(map(AssignPDOHelper.mapPagedStaffsToCxSelectItems));
  };

  getAdminObs = (
    searchKey?: string,
    departmentId?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    const filterParams = AssignPDOHelper.filterParamAdminBuilder(
      searchKey,
      departmentId
    );

    return this.userService
      .getListEmployee(filterParams)
      .pipe(map(AssignPDOHelper.mapPagedStaffsToCxSelectItems));
  };

  getUserGroupObs = (
    departmentId: number
  ): Observable<CxSelectItemModel<UserGroupModel>[]> => {
    return this.idpService
      .getUserGroupByDepartmentId(departmentId)
      .pipe(map(AssignPDOHelper.mapPagedUserGroupDTOToCxSelectItems));
  };

  async recommendPDO(payload: AssignPDOpportunityPayload): Promise<boolean> {
    return await this.assignPDO(payload, AssignModeEnum.Recommend);
  }

  async adhocNominatePDO(
    payload: AssignPDOpportunityPayload
  ): Promise<boolean> {
    return await this.assignPDO(payload, AssignModeEnum.AdhocNominate);
  }

  async validateMassNomination(
    massAssignedPDOpayload: MassAssignPDOpportunityPayload
  ): Promise<MassNominationValidationResultDto> {
    const result = await this.idpService.uploadMassNominationFile(
      massAssignedPDOpayload
    );
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

  async validateMassNominationFile(
    file: File
  ): Promise<MassNominationValidationResultDto> {
    const result = await this.idpService.validateMassNominationFile(file);
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

  async massNominate(
    massAssignedPDOpayload: MassAssignPDOpportunityPayload,
    assignPDOMode: AssignModeEnum
  ): Promise<void> {
    switch (assignPDOMode) {
      case AssignModeEnum.Nominate:
        await this.idpService.massNominate(massAssignedPDOpayload);
        break;
      case AssignModeEnum.AdhocNominate:
        await this.idpService.adHocMassNominate(massAssignedPDOpayload);
        break;
      default:
        console.error('massNominate: Invalid input');
        break;
    }
  }

  async removeAssignedActionItem(
    actionItemResultIdentity: ResultIdentity
  ): Promise<boolean> {
    const minSuccessHTTPCode = 200;
    const maxSuccessHTTPCode = 200;
    const response = await this.idpService.removeAssignedPDOResult([
      actionItemResultIdentity,
    ]);

    if (response.error || isEmpty(response.data)) {
      return false;
    }
    const result = response.data[0];

    if (
      result.status >= minSuccessHTTPCode &&
      result.status <= maxSuccessHTTPCode
    ) {
      return true;
    }

    return false;
  }

  async changeStatusNominateRequest(
    payload: ChangeStatusNominateRequestPayload,
    action: NominationChangeStatusAction,
    type: NominationSource
  ): Promise<boolean> {
    if (!payload) {
      return false;
    }

    const response = await this.idpService.changeStatusNomiateRequestPDOs(
      payload,
      action,
      type
    );

    const assignResult = response.data;

    if (response.error || isEmpty(assignResult)) {
      console.error(response.error);
      const errorMessage = AssignPDOHelper.getAssignPDOErrorMessage(
        AssignModeEnum.Nominate
      );
      this.toastrService.error(errorMessage);

      return false;
    }
    const isApproveMode = AssignPDOHelper.isApproveMode(
      payload.changePDOpportunityStatus
    );
    const isSuccess = assignResult.isSuccess;
    const assignedLearnerExceptionResults = assignResult.assignedLearnerResults;
    const messageCode = AssignPDOHelper.getAssignPDOMessageCode(
      assignResult,
      AssignModeEnum.Nominate
    );

    if (isEmpty(assignedLearnerExceptionResults)) {
      const message = AssignPDOHelper.getChangeNominateStatusMessage(
        assignResult,
        isApproveMode
      );
      const _ = isSuccess
        ? this.toastrService.success(message)
        : this.toastrService.info(message);

      return isSuccess;
    }

    if (!isSuccess && messageCode === 'HAS_INVALID_LEARNER') {
      this.globalLoader.hideLoader();
      const isConfirmed = await this.showPopupConfirmProceedAssignPDO(
        assignResult,
        AssignModeEnum.Nominate,
        true
      );
      if (isConfirmed) {
        this.globalLoader.showLoader();
        const proceedAssign = true;
        const exceptResultIds = assignedLearnerExceptionResults.map(
          (result) => result.resultId
        );
        const isSuccessResult = await this.changeStatusNominateRequest(
          {
            ...payload,
            proceedAssign,
            exceptResultIds,
          },
          action,
          type
        );
        this.globalLoader.hideLoader();

        return isSuccessResult;
      } else {
        return false;
      }
    }

    this.showPopupAssignPDOResult(
      assignResult,
      AssignModeEnum.Nominate,
      false,
      true
    );

    return isSuccess;
  }

  async changeStatusMassNominateRequest(
    payload: ChangeStatusNominateRequestPayload,
    action: NominationChangeStatusAction,
    type: NominationSource
  ): Promise<boolean> {
    if (!payload) {
      return false;
    }

    const response = await this.idpService.changeStatusMassNomiateRequestPDOs(
      payload,
      action,
      type
    );

    const assignResult = response.data;

    if (response.error || isEmpty(assignResult)) {
      console.error(response.error);
      const errorMessage = AssignPDOHelper.getAssignPDOErrorMessage(
        AssignModeEnum.Nominate
      );
      this.toastrService.error(errorMessage);

      return false;
    }
    const isApproveMode = AssignPDOHelper.isApproveMode(
      payload.changePDOpportunityStatus
    );
    const isSuccess = assignResult.isSuccess;
    const assignedLearnerExceptionResults = assignResult.assignedLearnerResults;
    const messageCode = AssignPDOHelper.getAssignPDOMessageCode(
      assignResult,
      AssignModeEnum.Nominate
    );

    if (isEmpty(assignedLearnerExceptionResults)) {
      const message = AssignPDOHelper.getChangeNominateStatusMessage(
        assignResult,
        isApproveMode
      );
      const _ = isSuccess
        ? this.toastrService.success(message)
        : this.toastrService.info(message);

      return isSuccess;
    }

    if (!isSuccess && messageCode === 'HAS_INVALID_LEARNER') {
      this.globalLoader.hideLoader();
      const isConfirmed = await this.showPopupConfirmProceedAssignPDO(
        assignResult,
        AssignModeEnum.Nominate,
        true
      );
      if (isConfirmed) {
        this.globalLoader.showLoader();
        const proceedAssign = true;
        const exceptResultIds = assignedLearnerExceptionResults.map(
          (result) => result.resultId
        );
        const isSuccessResult = await this.changeStatusNominateRequest(
          {
            ...payload,
            proceedAssign,
            exceptResultIds,
          },
          action,
          type
        );
        this.globalLoader.hideLoader();

        return isSuccessResult;
      } else {
        return false;
      }
    }

    this.showPopupAssignPDOResult(
      assignResult,
      AssignModeEnum.Nominate,
      false,
      true
    );

    return isSuccess;
  }

  async assignPDO(
    payload: AssignPDOpportunityPayload,
    assignPDOMode: AssignModeEnum
  ): Promise<boolean> {
    if (!payload) {
      return false;
    }

    let response: AsyncRespone<AssignPDOpportunityResponse>;
    switch (assignPDOMode) {
      case AssignModeEnum.Nominate:
        response = await this.idpService.nomiatePDOs(payload);
        break;
      case AssignModeEnum.AdhocNominate:
        response = await this.idpService.adhocNomiatePDOs(payload);
        break;
      case AssignModeEnum.Recommend:
        response = await this.idpService.recommendPDOs(payload);
        break;
      default:
        console.error('Invalid mode when call assignPDO');

        return false;
    }

    const assignResult = response.data;

    if (response.error || isEmpty(assignResult)) {
      console.error(response.error);
      const errorMessage = AssignPDOHelper.getAssignPDOErrorMessage(
        assignPDOMode
      );
      this.toastrService.error(errorMessage);
      this.globalLoader.hideLoader();

      return false;
    }

    const isSuccess = assignResult.isSuccess;
    const assignedLearnerExceptionResults = assignResult.assignedLearnerResults;
    const messageCode = AssignPDOHelper.getAssignPDOMessageCode(
      assignResult,
      assignPDOMode
    );

    if (isEmpty(assignedLearnerExceptionResults)) {
      const message = AssignPDOHelper.getAssignPDOMessage(
        assignResult,
        assignPDOMode
      );
      const _ = isSuccess
        ? this.toastrService.success(message)
        : this.toastrService.info(message);
      this.globalLoader.hideLoader();

      return isSuccess;
    }

    if (!isSuccess && messageCode === 'HAS_INVALID_LEARNER') {
      this.globalLoader.hideLoader();
      const isConfirmed = await this.showPopupConfirmProceedAssignPDO(
        assignResult,
        assignPDOMode
      );
      if (isConfirmed) {
        this.globalLoader.showLoader();
        const proceedAssign = true;
        const exceptLearnerExtIds = assignedLearnerExceptionResults.map(
          (result) => result.identity.extId
        );
        const isSuccessResult = await this.assignPDO(
          { ...payload, proceedAssign, exceptLearnerExtIds },
          assignPDOMode
        );

        this.globalLoader.hideLoader();

        return isSuccessResult;
      } else {
        return false;
      }
    }

    this.showPopupAssignPDOResult(assignResult, assignPDOMode, false);
    this.globalLoader.hideLoader();

    return isSuccess;
  }

  async getIndividualLearnerAssignedPDOAsync(
    departmentId: number,
    assignType: PDOAddType,
    timestamp: string,
    courseId?: string,
    pageIndex?: number,
    pageSize?: number,
    isExternalPDO?: boolean
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return this.getAssignedPDOForLearners(
      departmentId,
      assignType,
      timestamp,
      courseId,
      null,
      pageIndex,
      pageSize,
      -1,
      undefined,
      isExternalPDO
    );
  }

  async getLearnerAssignedPDOForGroupAsync(
    departmentId: number,
    assignType: PDOAddType,
    groupId: number,
    timestamp: string,
    courseId?: string,
    classRunId?: string,
    status?: NominateStatusCodeEnum,
    pageIndex?: number,
    pageSize?: number,
    isExternalPDO?: boolean
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return this.getAssignedPDOForLearners(
      departmentId,
      assignType,
      timestamp,
      courseId,
      status,
      pageIndex,
      pageSize,
      groupId,
      ArcheTypeEnum.UserPool,
      isExternalPDO,
      classRunId
    );
  }

  async getLearnerAssignedPDOForDepartmentAsync(
    departmentId: number,
    assignType: PDOAddType,
    timestamp: string,
    courseId?: string,
    classRunId?: string,
    status?: NominateStatusCodeEnum,
    pageIndex?: number,
    pageSize?: number,
    isExternalPDO?: boolean
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return this.getAssignedPDOForLearners(
      departmentId,
      assignType,
      timestamp,
      courseId,
      status,
      pageIndex,
      pageSize,
      departmentId,
      ArcheTypeEnum.OrganizationalUnit,
      isExternalPDO,
      classRunId
    );
  }

  async getAdhocMassNominationLearnerAsync(
    departmentId: number,
    resultId: string | number,
    status: NominateStatusCodeEnum,
    timestamp: string,
    pageIndex?: number,
    pageSize?: number
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    const params = new GetAdhocMassNominationLearnerParams({
      status,
      resultId,
      timestamp,
      departmentId,
      pageIndex,
      pageSize,
    });

    const response = await this.idpService.getAdhocMassNominationLearnerAsync(
      params
    );

    const responseData = response.data;
    if (response.error || !responseData || isEmpty(responseData.items)) {
      return;
    }

    responseData.items = responseData.items
      .sort(this.sortActionItemByDate)
      .filter(this.filterValidResult);

    return AssignPDOHelper.toPagingNominateLearnerRequest(responseData);
  }

  async getGroupAssignedPDO(
    departmentId: number,
    assignType: PDOAddType,
    timestamp: string,
    courseId?: string
  ): Promise<GroupAssignPDOResultModel[]> {
    if (!assignType || assignType === PDOAddType.SelfRegistered) {
      return [];
    }

    const params = new GetLearnerAssignedPDOParams({
      courseId,
      departmentId,
      timestamp,
    });

    let response: AsyncRespone<PagingResponseModel<AssignedPDOResultDTO>>;

    switch (assignType) {
      case PDOAddType.Nominated:
        params.includeClassRunInfo = true;
        response = await this.idpService.getNominatedGroupAsync(params);
        break;
      case PDOAddType.AdhocNominated:
        params.includeClassRunInfo = true;
        params.includeCourseInfo = true;
        response = await this.idpService.getAdhocNominatedGroupAsync(params);
        break;
      case PDOAddType.Recommended:
        response = await this.idpService.getRecommendedGroupAsync(params);
        break;
      default:
        console.error('Invalid assignType when call getGroupAssignedPDO');

        return;
    }

    if (response.error || !response.data || isEmpty(response.data.items)) {
      return [];
    }

    const resultDtos = response.data.items
      .sort(this.sortActionItemByDate)
      .filter(this.filterValidResult);

    return AssignPDOHelper.toNominateGroupModels(resultDtos);
  }

  async getDepartmentAssignedPDO(
    departmentId: number,
    assignType: PDOAddType,
    timestamp: string,
    courseId?: string
  ): Promise<DepartmentAssignPDOResultModel[]> {
    if (!assignType || assignType === PDOAddType.SelfRegistered) {
      return;
    }

    const params = new GetLearnerAssignedPDOParams({
      courseId,
      departmentId,
      timestamp,
    });

    let response: AsyncRespone<PagingResponseModel<AssignedPDOResultDTO>>;

    switch (assignType) {
      case PDOAddType.Nominated:
        params.includeClassRunInfo = true;
        response = await this.idpService.getNominatedDepartmentAsync(params);
        break;
      case PDOAddType.AdhocNominated:
        params.includeClassRunInfo = true;
        params.includeCourseInfo = true;
        response = await this.idpService.getAdhocNominatedDepartmentAsync(
          params
        );
        break;
      case PDOAddType.Recommended:
        response = await this.idpService.getRecommendedDepartmentAsync(params);
        break;
      default:
        console.error('Invalid assignType when call getGroupAssignedPDO');

        return;
    }

    if (response.error || !response.data || isEmpty(response.data.items)) {
      return;
    }

    const resultDtos = response.data.items
      .sort(this.sortActionItemByDate)
      .filter(this.filterValidResult);

    return AssignPDOHelper.toNominateDepartmentModels(resultDtos);
  }

  async getAssignedPDOForMassNominationAsync(
    departmentId: number,
    pageIndex?: number,
    pageSize?: number,
    klpExtId?: string
  ): Promise<PagingResponseModel<MassAssignPDOResultModel>> {
    const params = new GetMassAssignedPDOParams({
      klpExtId,
      departmentId,
      includeClassRunInfo: false,
      includeCourseInfo: false,
    });

    let response: AsyncRespone<PagingResponseModel<MassAssignedPDOResultDTO>>;
    response = await this.idpService.getMassNominatedResultAsync(params);
    const responseData = response.data;
    if (response.error || !responseData || isEmpty(responseData.items)) {
      return;
    }

    responseData.items = responseData.items
      .sort(this.sortActionItemByDate)
      .filter(this.filterValidResult);

    if (isEmpty(responseData.items)) {
      return;
    }

    return AssignPDOHelper.toPagingMassNominationRequest(responseData);
  }

  async getAssignedPDOForAdHocMassNominationAsync(
    departmentId: number,
    pageIndex?: number,
    pageSize?: number
  ): Promise<PagingResponseModel<MassAssignPDOResultModel>> {
    const params = new GetMassAssignedPDOParams({
      departmentId,
      pageSize,
      pageIndex,
    });

    let response: AsyncRespone<PagingResponseModel<MassAssignedPDOResultDTO>>;
    response = await this.idpService.getAdHocMassNominatedResultAsync(params);

    const responseData = response.data;
    if (response.error || !responseData || isEmpty(responseData.items)) {
      return;
    }

    responseData.items = responseData.items
      .sort(this.sortActionItemByDate)
      .filter(this.filterValidResult);

    if (isEmpty(responseData.items)) {
      return;
    }

    return AssignPDOHelper.toPagingMassNominationRequest(responseData);
  }

  async downloadMassNominationReportFileAsync(
    resultId: number
  ): Promise<DownloadMassAssignedPDOReportFileModel> {
    let response: AsyncRespone<DownloadMassAssignedPDOReportFileModel>;
    response = await this.idpService.downloadMassNominationReportFileAsync(
      resultId
    );
    const responseData = response.data;
    if (response.error || !responseData) {
      return;
    }

    return responseData;
  }

  private async getAssignedPDOForLearners(
    departmentId: number,
    assignType: PDOAddType,
    timestamp: string,
    courseId?: string,
    status?: NominateStatusCodeEnum,
    pageIndex?: number,
    pageSize?: number,
    relatedObjectId?: number,
    relatedObjectArchetype?: string,
    isExternalPDO?: boolean,
    classRunId?: string
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    if (!assignType || assignType === PDOAddType.SelfRegistered) {
      console.error('Invalid assignType when call getAssignedPDOForLearners');

      return;
    }

    const params = new GetLearnerAssignedPDOParams({
      status,
      courseId,
      classRunId,
      timestamp,
      departmentId,
      pageIndex,
      pageSize,
    });

    if (relatedObjectId === -1) {
      params.relatedObjectId = -1;
    } else if (relatedObjectId > 0 && relatedObjectArchetype) {
      params.relatedObjectId = relatedObjectId;
      params.relatedObjectArchetype = relatedObjectArchetype;
    }

    let response: AsyncRespone<PagingResponseModel<AssignedPDOResultDTO>>;

    switch (assignType) {
      case PDOAddType.Nominated:
        params.includeClassRunInfo = !isExternalPDO ? true : undefined;
        response = await this.idpService.getNominatedLearnerAsync(params);
        break;
      case PDOAddType.AdhocNominated:
        params.includeClassRunInfo = !isExternalPDO ? true : undefined;
        params.includeCourseInfo = true;
        response = await this.idpService.getAdhocNominatedLearnerAsync(params);
        break;
      case PDOAddType.Recommended:
        response = await this.idpService.getRecommendedLearnerAsync(params);
        break;
      default:
        console.error('Invalid assignType when call getAssignedPDOForLearners');

        return;
    }

    const responseData = response.data;
    if (response.error || !responseData || isEmpty(responseData.items)) {
      return;
    }

    responseData.items = responseData.items
      .sort(this.sortActionItemByDate)
      .filter(this.filterValidResult);

    if (isEmpty(responseData.items)) {
      return;
    }

    return AssignPDOHelper.toPagingNominateLearnerRequest(responseData);
  }

  private async showPopupConfirmProceedAssignPDO(
    nominateResult: AssignPDOpportunityResponse,
    assignMode: AssignModeEnum,
    approvalMode: boolean = false
  ): Promise<boolean> {
    if (this.ngbModal.hasOpenModals()) {
      return;
    }

    const ngbModal = await this.showPopupAssignPDOResult(
      nominateResult,
      assignMode,
      true,
      approvalMode
    );
    const response = await toCxAsync<boolean>(ngbModal.result);

    return response.data !== undefined ? response.data : false;
  }

  private async showPopupAssignPDOResult(
    nominateResult: AssignPDOpportunityResponse,
    assignMode: AssignModeEnum,
    isConfirmMode: boolean = false,
    approvalMode: boolean = false
  ): Promise<NgbModalRef> {
    const assignedLearnerExceptionResults =
      nominateResult.assignedLearnerResults;
    const totalLearner = nominateResult.totalLearner || 0;
    let invalidResultsSorted: AssignResultModel[];

    const learnerExtIds = assignedLearnerExceptionResults.map(
      (result) => result.identity.extId
    );
    const learnerInfos = await this.getLearnerInfo(learnerExtIds);
    const nominateResults = assignedLearnerExceptionResults.map((result) => {
      return AssignPDOHelper.mapToAssignResultModel(result, learnerInfos);
    });
    invalidResultsSorted = nominateResults.sort(this.sortAssignResultByName);

    const ngbModal = this.ngbModal.open(AssignPDOResultDialogComponent, {
      centered: true,
      size: 'lg',
      windowClass: 'mobile-dialog-slide-right',
    });

    const modalRef = ngbModal.componentInstance as AssignPDOResultDialogComponent;
    modalRef.invalidResults = invalidResultsSorted;
    modalRef.assignMode = assignMode;
    modalRef.isSuccess = nominateResult.isSuccess;
    modalRef.totalLearner = totalLearner;
    modalRef.isConfirmMode = isConfirmMode;
    modalRef.approvalMode = approvalMode;

    modalRef.confirm.subscribe(() => ngbModal.close(true));
    modalRef.close.subscribe(() => ngbModal.close(false));

    return ngbModal;
  }

  private sortAssignResultByName(
    item1: AssignResultModel,
    item2: AssignResultModel
  ): number {
    if (item1.name < item2.name) {
      return -1;
    }
    if (item1.name > item2.name) {
      return 1;
    }

    return 0;
  }

  private async getLearnerInfo(
    learnerExtIds: string[]
  ): Promise<LearnerInfoDTO[]> {
    const response = await this.userService.getLearnerInfoAsync(learnerExtIds);

    return response.data;
  }

  private sortActionItemByDate(
    actionItem1: PDOActionItemDTO,
    actionItem2: PDOActionItemDTO
  ): number {
    const date1 = +new Date(actionItem1.created);
    const date2 = +new Date(actionItem2.created);

    return date2 - date1;
  }

  private filterValidResult(pdoAcionItem: PDOActionItemDTO): boolean {
    return (
      !isEmpty(pdoAcionItem.objectiveInfo) &&
      !isEmpty(pdoAcionItem.objectiveInfo.identity)
    );
  }
}
