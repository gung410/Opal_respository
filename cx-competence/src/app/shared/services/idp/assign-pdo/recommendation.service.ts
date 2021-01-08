import { Injectable } from '@angular/core';
import { ResultIdentity } from 'app-models/assessment.model';
import {
  AssignPDOpportunityPayload,
  DepartmentAssignPDOResultModel,
  GroupAssignPDOResultModel,
  LearnerAssignPDOResultModel,
} from 'app-models/mpj/assign-pdo.model';
import { PDOAddType } from 'app-models/mpj/pdo-action-item.model';
import { UserGroupModel } from 'app-models/user-group.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { AssignModeEnum } from 'app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/planned-pdo-detail.model';
import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { Observable } from 'rxjs';
import { AssignPDOService } from './assign-pdo.service';

@Injectable()
export class RecommendationService {
  constructor(private assignPDOService: AssignPDOService) {}

  getLearnersObs = (
    searchKey?: string,
    departmentId?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.assignPDOService.getLearnersObs(searchKey, departmentId);
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

  async recommendPDOAsync(
    payload: AssignPDOpportunityPayload
  ): Promise<boolean> {
    return await this.assignPDOService.assignPDO(
      payload,
      AssignModeEnum.Recommend
    );
  }

  async removeRecommendedItemAsync(
    actionItemResultIdentity: ResultIdentity
  ): Promise<boolean> {
    return await this.assignPDOService.removeAssignedActionItem(
      actionItemResultIdentity
    );
  }

  async getIndividualRecommendationAsync(
    courseId: string,
    departmentId: number,
    timestamp: string,
    pageIndex?: number,
    pageSize?: number
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return await this.assignPDOService.getIndividualLearnerAssignedPDOAsync(
      departmentId,
      PDOAddType.Recommended,
      timestamp,
      courseId,
      pageIndex,
      pageSize
    );
  }

  async getLearnerRecommendationOfGroupAsync(
    courseId: string,
    departmentId: number,
    groupId: number,
    timestamp: string,
    pageIndex?: number,
    pageSize?: number
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return await this.assignPDOService.getLearnerAssignedPDOForGroupAsync(
      departmentId,
      PDOAddType.Recommended,
      groupId,
      timestamp,
      courseId,
      null,
      null,
      pageIndex,
      pageSize
    );
  }

  async getLearnerRecommendationOfDepartmentAsync(
    departmentId: number,
    timestamp: string,
    courseId?: string,
    pageIndex?: number,
    pageSize?: number
  ): Promise<PagingResponseModel<LearnerAssignPDOResultModel>> {
    return await this.assignPDOService.getLearnerAssignedPDOForDepartmentAsync(
      departmentId,
      PDOAddType.Recommended,
      timestamp,
      courseId,
      null,
      null,
      pageIndex,
      pageSize
    );
  }

  async getGroupRecommendationAsync(
    courseId: string,
    departmentId: number,
    timestamp: string
  ): Promise<GroupAssignPDOResultModel[]> {
    return await this.assignPDOService.getGroupAssignedPDO(
      departmentId,
      PDOAddType.Recommended,
      timestamp,
      courseId
    );
  }

  async getDepartmentRecommendationAsync(
    courseId: string,
    departmentId: number,
    timestamp: string
  ): Promise<DepartmentAssignPDOResultModel[]> {
    return await this.assignPDOService.getDepartmentAssignedPDO(
      departmentId,
      PDOAddType.Recommended,
      timestamp,
      courseId
    );
  }
}
