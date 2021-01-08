import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StatusHistoryDto } from 'app-models/common/status-history.model';
import {
  ChangeMassPdPlanStatusType,
  ChangePdPlanStatusTypeResult,
} from 'app-models/mpj/idp.model';
import {
  IdpConfigParams,
  PDPlanConfig,
  ReportData,
} from 'app-models/pdplan.model';
import { AsyncRespone } from 'app-utilities/cx-async';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import {
  IChangeStatusModel,
  IChangeStatusRedultDto,
} from 'app/approval-page/models/learning-need-grid-row.model';
import {
  AcceptExportDto,
  ExportLearningNeedsAnalysisParams,
} from 'app/learning-needs-analysis/models/export-learning-needs-analysis-params';
import {
  IdpDto,
  IdpFilterParams,
} from 'app/organisational-development/models/idp.model';
import { Observable } from 'rxjs';
import { APIConstant, AppConstant } from '../app.constant';

@Injectable()
export class IdpService {
  private competenceApiPrefix: string;
  constructor(private httpHelpers: HttpHelpers) {
    this.competenceApiPrefix = AppConstant.api.competence;
  }

  getNeedsResult(params: IdpFilterParams): Observable<IdpDto[]> {
    return this.httpHelpers.get<IdpDto[]>(
      `${this.competenceApiPrefix}/idp/needs/results`,
      params
    );
  }

  getNeedsResultAsync(
    params: IdpFilterParams
  ): Promise<AsyncRespone<IdpDto[]>> {
    return this.httpHelpers.getAsync<IdpDto[]>(
      `${this.competenceApiPrefix}/idp/needs/results`,
      params
    );
  }

  getPlansResult(params: IdpFilterParams): Observable<IdpDto[]> {
    return this.httpHelpers.get<IdpDto[]>(
      `${this.competenceApiPrefix}/idp/plans/results`,
      params
    );
  }

  changeStatusActionItems(
    changeMassPdPlanStatusType: ChangeMassPdPlanStatusType
  ): Observable<ChangePdPlanStatusTypeResult[]> {
    return this.httpHelpers.post<ChangePdPlanStatusTypeResult[]>(
      `${this.competenceApiPrefix}/idp/actionitems/results/changestatus`,
      changeMassPdPlanStatusType
    );
  }

  savePlanResult(dto: IdpDto): Observable<IdpDto> {
    return this.httpHelpers.post<IdpDto>(
      `${this.competenceApiPrefix}/idp/plans/results`,
      dto
    );
  }

  saveNeedsResult(
    idpDto: IdpDto,
    ignoreHandleError?: boolean
  ): Observable<IdpDto> {
    return this.httpHelpers.post<IdpDto>(
      `${this.competenceApiPrefix}/idp/needs/results`,
      idpDto,
      null,
      { avoidIntercepterCatchError: ignoreHandleError }
    );
  }

  saveNeedsResultAsync(
    idpDto: IdpDto,
    ignoreHandleError?: boolean
  ): Promise<AsyncRespone<IdpDto>> {
    return this.httpHelpers.postAsync<IdpDto>(
      `${this.competenceApiPrefix}/idp/needs/results`,
      idpDto,
      null,
      { avoidIntercepterCatchError: ignoreHandleError }
    );
  }

  getLearningNeedConfig(params: IdpConfigParams): Observable<PDPlanConfig> {
    return this.httpHelpers.get<PDPlanConfig>(
      `${this.competenceApiPrefix}/idp/needs/config`,
      params
    );
  }

  getLearningNeedReport(params: IdpConfigParams): Observable<ReportData[]> {
    return this.httpHelpers.get<ReportData[]>(
      `${this.competenceApiPrefix}/idp/needs/reports/${params.resultId}`
    );
  }

  exportLearningNeedsAnalysisAsync(
    exportLearningNeedsAnalysisParams: ExportLearningNeedsAnalysisParams
  ): Observable<AcceptExportDto> {
    return this.httpHelpers.post<AcceptExportDto>(
      `${this.competenceApiPrefix}/idp/needs/export/async`,
      exportLearningNeedsAnalysisParams
    );
  }

  getLearningNeedsAnalysisStatusChangeHistory(
    resultExtId: string
  ): Observable<StatusHistoryDto[]> {
    return this.httpHelpers.get<StatusHistoryDto[]>(
      `${this.competenceApiPrefix}/idp/needs/results/extid/${resultExtId}/statustypechanges`
    );
  }

  downloadExportLearningNeedsAnalysis(fileName: string): Observable<any> {
    return this.httpHelpers.post(
      `${this.competenceApiPrefix}/idp/needs/export/download?fileName=${fileName}`,
      null,
      undefined,
      this.buildBlobZipFileHelperOptions()
    );
  }

  changeStatusLearningNeeds(
    changeStatusDTO: IChangeStatusModel
  ): Observable<IChangeStatusRedultDto[]> {
    const url = APIConstant.IDP_NEED_CHANGE_STATUS;

    return this.httpHelpers.post(url, changeStatusDTO);
  }

  changeStatusPDPLan(
    changeStatusDTO: IChangeStatusModel
  ): Observable<IChangeStatusRedultDto[]> {
    const url = APIConstant.IDP_PLAN_CHANGE_STATUS;

    return this.httpHelpers.post(url, changeStatusDTO);
  }

  private buildBlobZipFileHelperOptions(): any {
    const responseBlobType = 'arraybuffer';
    const headers = new HttpHeaders({ Accept: 'application/zip' });

    return {
      responseType: responseBlobType,
      headers,
    };
  }
}
