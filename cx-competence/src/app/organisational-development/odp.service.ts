import { Injectable } from '@angular/core';
import { Identity } from '@conexus/cx-angular-common/lib/models/identity.model';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import {
  DeactivateAssessmentParams,
  DeactivateAssessmentResponseDto,
} from 'app-models/deactivateAssessment.model';
import { LearnerAssignPDOResultModel } from 'app-models/mpj/assign-pdo.model';
import { PDPlanConfig, PDPlanDto } from 'app-models/pdplan.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { AsyncRespone } from 'app-utilities/cx-async';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import {
  ChangePDPlanStatusDto,
  ODPConfigFilterParams,
  ODPFilterParams,
} from 'app/organisational-development/models/odp.models';
import { AppConstant, ODPAPIConstant } from 'app/shared/app.constant';
import { Observable, of, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { DuplicatePDPlanRequest } from './dtos/duplicate-pd-plan.dto';
import { GetMassNominationLearnerParams } from './dtos/odp.dto';
import { OdpActivity } from './learning-plan-detail/odp.constant';
import { IdpDto } from './models/idp.model';

@Injectable({
  providedIn: 'root',
})
export class OdpService {
  private responseCache: Map<string, any> = new Map();

  constructor(
    private http: HttpHelpers,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService
  ) {}

  public getLearningPlanList(
    filterParams: ODPFilterParams
  ): Observable<[PDPlanDto]> {
    return this.http.get<[PDPlanDto]>(
      `${AppConstant.api.competence}` + `/odp/plans/results?countChildren=true`,
      filterParams
    );
  }

  public getPlanResultList(
    filterParams: ODPFilterParams
  ): Observable<[PDPlanDto]> {
    return this.http.get<[PDPlanDto]>(
      `${AppConstant.api.competence}/odp/plans/results`,
      filterParams
    );
  }

  public getLearningPlanConfig(
    filterParams: ODPConfigFilterParams
  ): Observable<any> {
    return this.http.get<any>(
      `${AppConstant.api.competence}/odp/plans/config`,
      filterParams
    );
  }

  public savePlan(
    pdplanDto: PDPlanDto,
    activity: OdpActivity,
    ignoreHandleError?: boolean
  ): Observable<any> {
    switch (activity) {
      case OdpActivity.Plan:
        return this.http.post<PDPlanDto>(
          `${AppConstant.api.competence}/odp/plans/results`,
          pdplanDto,
          null,
          { avoidIntercepterCatchError: ignoreHandleError }
        );
      case OdpActivity.Direction:
        return this.http.post<PDPlanDto>(
          `${AppConstant.api.competence}/odp/directions/results`,
          pdplanDto,
          null,
          { avoidIntercepterCatchError: ignoreHandleError }
        );
      case OdpActivity.Programme:
        return this.http.post<PDPlanDto>(
          `${AppConstant.api.competence}/odp/programmes/results`,
          pdplanDto,
          null,
          { avoidIntercepterCatchError: ignoreHandleError }
        );
      default:
        return;
    }
  }

  public changeStatusPlan(
    resultID: number,
    changePDPlanStatusDto: ChangePDPlanStatusDto,
    activity: string | OdpActivity
  ): Observable<ChangePDPlanStatusDto> {
    if (activity === OdpActivity.Plan) {
      return this.http.post<ChangePDPlanStatusDto>(
        `${AppConstant.api.competence}/odp/plans/results/${resultID}/changestatus`,
        changePDPlanStatusDto
      );
    }

    if (activity === OdpActivity.Direction) {
      return this.http.post<ChangePDPlanStatusDto>(
        `${AppConstant.api.competence}/odp/directions/results/${resultID}/changestatus`,
        changePDPlanStatusDto
      );
    }
  }

  public changeStatusPlans(
    planResultIdentities: Identity[],
    targetStatusType: AssessmentStatusInfo,
    activity: OdpActivity
  ): Observable<PDPlanDto[]> {
    const resource = activity === OdpActivity.Plan ? 'plans' : 'directions';
    const payload = {
      resultIdentities: planResultIdentities,
      targetStatusType,
    };

    return this.http.post<[PDPlanDto]>(
      `${AppConstant.api.competence}/odp/${resource}/results/changestatus`,
      payload
    );
  }

  public getLearningPlanResult(resultId: number): Observable<PDPlanDto> {
    return this.http.get<PDPlanDto>(
      `${AppConstant.api.competence}/odp/plans/results/${resultId}`
    );
  }

  public getLearningPlanResultByExtID(extId: string): Observable<PDPlanDto[]> {
    return this.http.get<[PDPlanDto]>(
      `${AppConstant.api.competence}` +
        `/odp/plans/results?resultExtIds=${extId}&includeChildren=true`
    );
  }

  public getDirectionResult(resultId: number): Observable<any> {
    return this.http.get(
      `${AppConstant.api.competence}/odp/directions/results/${resultId}`
    );
  }

  public getProgrammeResult(resultId: number): Observable<any> {
    return this.http.get(
      `${AppConstant.api.competence}/odp/programmes/results/${resultId}`
    );
  }

  public getDirectionResults(filterParams: ODPFilterParams): Observable<any> {
    return this.http.get(
      `${AppConstant.api.competence}/odp/directions/results`,
      filterParams
    );
  }

  public getProgrammeResults(): Observable<any> {
    return this.http.get(
      `${AppConstant.api.competence}/odp/programmes/results`
    );
  }

  public getOdpResults(filterParams: ODPFilterParams): Observable<any> {
    return this.http.get(
      `${AppConstant.api.competence}/odp/results`,
      filterParams
    );
  }

  public getOdpConfigs(): Observable<PDPlanConfig[]> {
    const key = `getOdpConfigs`;
    const odpConfigsFromCache = this.responseCache.get(key);
    if (odpConfigsFromCache) {
      return of(odpConfigsFromCache);
    }

    return this.http
      .get<PDPlanConfig[]>(`${AppConstant.api.competence}/odp/configs`)
      .pipe(
        map((response) => {
          this.responseCache.set(key, response);

          return response;
        })
      );
  }

  public deactivateODP(
    deactivateAssessmentParams: DeactivateAssessmentParams
  ): Observable<DeactivateAssessmentResponseDto> {
    const odpResourceName = this.getODPResourceName(
      deactivateAssessmentParams.pdPlan.pdPlanActivity
    );
    if (odpResourceName === '') {
      throwError(
        `The '${deactivateAssessmentParams.pdPlan.pdPlanActivity}' is not supported`
      );

      return;
    }
    const deactivateUrl = `${AppConstant.api.competence}/odp/${odpResourceName}/results/deactivate`;
    const deactivateDto = {
      identities: [deactivateAssessmentParams.pdPlan.resultIdentity],
      deactivateAllVersion: deactivateAssessmentParams.deactivateAllVersion,
      deactivateAllDescendants:
        deactivateAssessmentParams.deactivateAllDescendants,
    };

    return this.http
      .post<DeactivateAssessmentResponseDto[]>(deactivateUrl, deactivateDto)
      .pipe()
      .map((res) => {
        return res[0];
      });
  }
  public duplicatePlan(
    requestDto: DuplicatePDPlanRequest,
    activity: string,
    ignoreHandleError?: boolean
  ): Observable<PDPlanDto[]> {
    switch (activity) {
      case OdpActivity.Plan:
        return this.http.post<PDPlanDto[]>(
          `${AppConstant.api.competence}/odp/plans/results/duplicate`,
          requestDto,
          null,
          { avoidIntercepterCatchError: ignoreHandleError }
        );
      case OdpActivity.Direction:
        return this.http.post<PDPlanDto[]>(
          `${AppConstant.api.competence}/odp/directions/results/duplicate`,
          requestDto,
          null,
          { avoidIntercepterCatchError: ignoreHandleError }
        );
      case OdpActivity.Programme:
        return this.http.post<PDPlanDto[]>(
          `${AppConstant.api.competence}/odp/programmes/results/duplicate`,
          requestDto,
          null,
          { avoidIntercepterCatchError: ignoreHandleError }
        );
      default:
        return;
    }
  }

  getStrategicThrusts(filterParams: ODPFilterParams): Observable<any> {
    return this.http.get<any>(
      `${AppConstant.api.competence}/odp/objectives/results`,
      filterParams
    );
  }

  getStrategisThrusts(opjDepartmentId: number): Observable<any> {
    const filterParams: ODPFilterParams = new ODPFilterParams({
      departmentIds: [opjDepartmentId],
    });

    return this.http.get<any>(
      `${AppConstant.api.competence}/odp/objectives/results`,
      filterParams
    );
  }

  getStrategicThrustsConfig(
    filterParams: ODPConfigFilterParams
  ): Observable<PDPlanConfig> {
    const key = `getStrategicThrustsConfig`;
    const strategicThrustsConfigFromCache = this.responseCache.get(key);
    if (strategicThrustsConfigFromCache) {
      return of(strategicThrustsConfigFromCache);
    }

    return this.http
      .get<PDPlanConfig>(
        `${AppConstant.api.competence}/odp/objectives/config`,
        filterParams
      )
      .pipe(
        map((response) => {
          this.responseCache.set(key, response);

          return response;
        })
      );
  }
  updateOrCreateStrategicThrust(pdplanDto: PDPlanDto): Observable<any> {
    return this.http.post<any>(
      `${AppConstant.api.competence}/odp/objectives/results`,
      pdplanDto
    );
  }

  deleteStrategicThrusts(pdplanDto: {}): Observable<any> {
    return this.http.post<any>(
      `${AppConstant.api.competence}/odp/objectives/results/deactivate`,
      pdplanDto,
      null,
      { avoidIntercepterCatchError: true }
    );
  }

  async getMassNominationLearnerAsync(
    params: GetMassNominationLearnerParams
  ): Promise<AsyncRespone<PagingResponseModel<LearnerAssignPDOResultModel>>> {
    return await this.http.getAsync<
      PagingResponseModel<LearnerAssignPDOResultModel>
    >(`${AppConstant.api.competence}/idp/massnominations/learners`, params, {
      avoidIntercepterCatchError: true,
    });
  }

  async saveKeyLearningProgram(planDto: IdpDto): Promise<AsyncRespone<IdpDto>> {
    return this.http.postAsync<IdpDto>(
      ODPAPIConstant.KEY_LEARNING_PROGRAM_BASE_URL,
      planDto,
      null,
      { avoidIntercepterCatchError: true }
    );
  }

  private getODPResourceName(pdPlanActivity: string): string {
    let odpResourceName = '';
    switch (pdPlanActivity) {
      case OdpActivity.Plan:
        odpResourceName = 'plans';
        break;
      case OdpActivity.Direction:
        odpResourceName = 'directions';
        break;
      default:
        break;
    }

    return odpResourceName;
  }
}
