import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { ICreateActionItemResultRequest, LearningOpportunitySourceType } from '../dtos/create-action-item-result-request';
import { IIdpPlanPdos, IdpPlanPdos } from '../dtos/idp-plans-pdos-response.dto';

import { DeactivatePDPlanDto } from '../dtos/deactivate-action-item.dto';
import { IActionItemResponse } from '../dtos/action-item-response.dto';
import { IIdpPlanPdosRequest } from '../dtos/idp-plans-pdos-request.dto';
import { IRecommendationByOrganisationItemResult } from '../dtos/recommendation-by-organisation-dto';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class IndividualDevelopmentPlanApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.competenceApiUrl + '/idp';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public addActionItemResult(courseId: string): Promise<IActionItemResponse> {
    const request = this.createActionItemRequest(courseId);
    return this.post<ICreateActionItemResultRequest, IActionItemResponse>('/actionitems/results', request).toPromise();
  }

  public getListActionsItemsLearner(userId: string): Promise<unknown> {
    return this.get<ICreateActionItemResultRequest>(`/actionitems/results?userIds=${userId}`).toPromise();
  }

  /**
   * To check is the course added to the plan of current user in the current year or not
   * @param courseId
   */
  public getActionItemsOfCurrentYearByCourseId(courseId: string): Promise<IActionItemResponse[]> {
    const learningOpportunityUri = this.createLearningOpportunityUri(courseId);
    const currentYear = new Date().getFullYear(); // 2020
    const middleOfCurrentYear = new Date(Date.UTC(currentYear, 5)); // ex: Jun 01 2020

    const params: IGetParams = {
      'additionalProperties[learningOpportunityUri]': learningOpportunityUri,
      surveyStartBefore: middleOfCurrentYear.toISOString(),
      surveyEndAfter: middleOfCurrentYear.toISOString()
    };
    return this.get<IActionItemResponse[]>(`/actionitems/results`, params).toPromise();
  }

  public getPlanPdos(request: IIdpPlanPdosRequest, showSpinner: boolean = true): Promise<IdpPlanPdos> {
    const params: IGetParams = { ...request };
    return this.get<IIdpPlanPdos>(`/plans/pdos`, params, showSpinner)
      .pipe(map(_ => new IdpPlanPdos(_)))
      .toPromise();
  }

  public getListOrganisationCourseRecommended(
    userId: string,
    departmentIds: string,
    showSpinner: boolean = true
  ): Promise<ICreateActionItemResultRequest[]> {
    return this.get<ICreateActionItemResultRequest[]>(
      `/actionitems/results?userIds=${userId}&departmentIds=${departmentIds}&additionalProperties[type]=recommended`,
      null,
      showSpinner
    ).toPromise();
  }

  public deactivateActionItem(userId: string): Promise<IActionItemResponse> {
    return this.post<DeactivatePDPlanDto, IActionItemResponse>(
      `/actionitems/results/deactivate`,
      new DeactivatePDPlanDto(userId)
    ).toPromise();
  }

  public getListRecommendationsByOU(
    pageIndex: number,
    pageSize: number,
    showSpinner: boolean = true
  ): Promise<IRecommendationByOrganisationItemResult> {
    return this.get<IRecommendationByOrganisationItemResult>(
      `/recommendationsByOU`,
      {
        pageIndex: pageIndex,
        pageSize: pageSize
      },
      showSpinner
    ).toPromise();
  }

  private createActionItemRequest(courseId: string): ICreateActionItemResultRequest {
    const nowInISO = new Date().toISOString();
    const learningOpportunityUri = this.createLearningOpportunityUri(courseId);
    const userId = AppGlobal.user.extId;
    const request: ICreateActionItemResultRequest = {
      objectiveInfo: {
        identity: {
          extId: userId, // extId of learner or you can put learner id instead
          archetype: 'Employee'
        }
      },
      answer: {
        learningOpportunity: {
          uri: learningOpportunityUri,
          source: LearningOpportunitySourceType.CoursepadPdo // fixed
        },
        addedToIDPDate: nowInISO // ISO string
      },
      forceCreateResult: true, // fixed
      additionalProperties: {
        type: 'self-registered', // fixed
        learningOpportunityUri: learningOpportunityUri
      },
      timestamp: nowInISO // ISO string
    };
    return request;
  }

  private createLearningOpportunityUri(courseId: string): string {
    return `urn:opal2.moe.sg:coursepad-pdo:${courseId}`;
  }
}
