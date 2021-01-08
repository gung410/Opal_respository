import { Injectable } from '@angular/core';
import { ResultIdentity } from 'app-models/assessment.model';
import {
  AssignPDOpportunityPayload,
  DepartmentAssignPDOResultModel,
  GroupAssignPDOResultModel,
  LearnerAssignPDOResultModel,
  MassAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import { PDOAddType } from 'app-models/mpj/pdo-action-item.model';
import { UserGroupModel } from 'app-models/user-group.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { AssignModeEnum } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/planned-pdo-detail.model';
import { NominateStatusCodeEnum } from 'app/organisational-development/learning-plan-detail/odp.constant';
import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { Observable } from 'rxjs';
import { IDPService } from '../idp.service';
import { AssignPDOService } from './assign-pdo.service';

@Injectable()
export class KlpNominationService {
  constructor(
    private assignPDOService: AssignPDOService,
    private idpService: IDPService
  ) {}

  getLearnersObs = (
    searchKey?: string,
    departmentId?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.assignPDOService.getAvailableLearnersObs(
      searchKey,
      departmentId
    );
  };

  getApprovingOfficerObs = (
    searchKey?: string,
    departmentId?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.assignPDOService.getApprovingOfficerObs(
      searchKey,
      departmentId
    );
  };

  getAdminObs = (
    searchKey?: string,
    departmentId?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.assignPDOService.getAdminObs(searchKey, departmentId);
  };

  getUserGroupObs = (
    departmentId: number
  ): Observable<CxSelectItemModel<UserGroupModel>[]> => {
    return this.assignPDOService.getUserGroupObs(departmentId);
  };

  async nominatePDOAsync(
    payload: AssignPDOpportunityPayload
  ): Promise<boolean> {
    return await this.assignPDOService.assignPDO(
      payload,
      AssignModeEnum.Nominate
    );
  }

  async removeNominatedItemAsync(
    actionItemResultIdentity: ResultIdentity
  ): Promise<boolean> {
    return await this.assignPDOService.removeAssignedActionItem(
      actionItemResultIdentity
    );
  }

  // Get nomination result
  async getIndividualNominationAsync(
    courseId: string,
    departmentId: number,
    timestamp: string,
    pageIndex?: number,
    pageSize?: number,
    isExternalPDO?: boolean
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return await this.assignPDOService.getIndividualLearnerAssignedPDOAsync(
      departmentId,
      PDOAddType.Nominated,
      timestamp,
      courseId,
      pageIndex,
      pageSize,
      isExternalPDO
    );
  }

  async getLearnerNominationOfGroupAsync(
    courseId: string,
    classRunId: string,
    departmentId: number,
    groupId: number,
    status: NominateStatusCodeEnum,
    timestamp: string,
    pageIndex?: number,
    pageSize?: number,
    isExternalPDO?: boolean
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return await this.assignPDOService.getLearnerAssignedPDOForGroupAsync(
      departmentId,
      PDOAddType.Nominated,
      groupId,
      timestamp,
      courseId,
      classRunId,
      status,
      pageIndex,
      pageSize,
      isExternalPDO
    );
  }

  async getLearnNominationOfDepartmentAsync(
    departmentId: number,
    timestamp: string,
    courseId?: string,
    classRunId?: string,
    status?: NominateStatusCodeEnum,
    pageIndex?: number,
    pageSize?: number,
    isExternalPDO?: boolean
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return await this.assignPDOService.getLearnerAssignedPDOForDepartmentAsync(
      departmentId,
      PDOAddType.Nominated,
      timestamp,
      courseId,
      classRunId,
      status,
      pageIndex,
      pageSize,
      isExternalPDO
    );
  }

  async getGroupNominationAsync(
    courseId: string,
    departmentId: number,
    timestamp: string
  ): Promise<GroupAssignPDOResultModel[]> {
    return await this.assignPDOService.getGroupAssignedPDO(
      departmentId,
      PDOAddType.Nominated,
      timestamp,
      courseId
    );
  }

  async getDepartmentNominationAsync(
    courseId: string,
    departmentId: number,
    timestamp: string
  ): Promise<DepartmentAssignPDOResultModel[]> {
    return await this.assignPDOService.getDepartmentAssignedPDO(
      departmentId,
      PDOAddType.Nominated,
      timestamp,
      courseId
    );
  }

  async getMassNominationAssignedPDOAsync(
    departmentId: number,
    klpExtId?: string,
    pageIndex?: number,
    pageSize?: number
  ): Promise<PagingResponseModel<MassAssignPDOResultModel>> {
    return this.assignPDOService.getAssignedPDOForMassNominationAsync(
      departmentId,
      pageIndex,
      pageSize,
      klpExtId
    );
  }

  async getAdHocMassNominationAssignedPDOAsync(
    departmentId: number,
    klpExtId?: string,
    pageIndex?: number,
    pageSize?: number
  ): Promise<PagingResponseModel<MassAssignPDOResultModel>> {
    return this.assignPDOService.getAssignedPDOForAdHocMassNominationAsync(
      departmentId,
      pageIndex,
      pageSize
    );
  }
}
