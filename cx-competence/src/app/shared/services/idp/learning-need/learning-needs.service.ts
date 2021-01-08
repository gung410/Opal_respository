import { Injectable } from '@angular/core';
import {
  CxSurveyjsEventModel,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import {
  ChartInfo,
  IdpConfigParams,
  ReportData,
} from 'app-models/pdplan.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { IDPService } from 'app-services/idp/idp.service';
import { AsyncRespone } from 'app-utilities/cx-async';
import { ObjectUtilities } from 'app-utilities/object-utils';
import {
  IdpStatusCodeEnum,
  IdpStatusEnum,
} from 'app/individual-development/idp.constant';
import LearningAreaChartHelper from 'app/individual-development/shared/learning-area-chart/learning-area-chart.helper';
import { LearningAreaChartModel } from 'app/individual-development/shared/learning-area-chart/learning-area-chart.model';
import {
  IdpDto,
  IdpFilterParams,
} from 'app/organisational-development/models/idp.model';
import { Constant } from 'app/shared/app.constant';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { isEmpty } from 'lodash';
import { ResultHelper } from '../result-helpers';
import { LearningNeedHelpers } from './learning-needs-helpers';

@Injectable()
export class LearningNeedService {
  constructor(
    private idpService: IDPService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService
  ) {}

  async getLearningNeedsResultsAsync(
    userId: number,
    period?: number,
    statusCodes?: string[]
  ): Promise<IdpDto[]> {
    const filterParams = new IdpFilterParams();
    let response: AsyncRespone<IdpDto[]>;

    if (userId) {
      // Case get by userId
      filterParams.userIds = [userId];
    }

    if (period) {
      const middleYear = new Date(
        Date.UTC(period, Constant.MIDDLE_MONTH_OF_YEAR_VALUE)
      ); // example: 1/6/2019
      filterParams.surveyStartBefore = middleYear.toISOString();
      filterParams.surveyEndAfter = middleYear.toISOString();
    }

    if (statusCodes) {
      filterParams.statusTypeCodes = statusCodes;
    }

    // If not it will get result of current user
    response = await this.idpService.getNeedsResults(filterParams);

    if (response.error) {
      return;
    }

    return response.data;
  }

  async getLearningNeedUnsubmittedAsync(
    userId: number,
    period?: number
  ): Promise<IdpDto> {
    const statusCodesUnsubmitted = LearningNeedHelpers.LEARNING_NEED_UNSUBMITTED_STATUS_CODE.map(
      (code) => code.toString()
    );
    const responseItems = await this.getLearningNeedsResultsAsync(
      userId,
      period
    );

    if (responseItems && responseItems.length > 0) {
      // TODO: In the future we need UI/UX to support multiple LNA
      // Temporary we just get first item for show on LNA page
      const learningNeedUnsubmitted = responseItems.find((item) =>
        statusCodesUnsubmitted.includes(
          item.assessmentStatusInfo.assessmentStatusCode
        )
      );

      return learningNeedUnsubmitted;
    }

    return null;
  }

  async getLearningNeedsSubmittedAsync(
    userId: number,
    period?: number
  ): Promise<IdpDto[]> {
    const statusCodesSubmitted = LearningNeedHelpers.LEARNING_NEED_SUBMITTED_STATUS_CODE.map(
      (code) => code.toString()
    );
    const responseItems = await this.getLearningNeedsResultsAsync(
      userId,
      period
    );
    if (responseItems && responseItems.length > 0) {
      let learningNeedResults = responseItems.filter((res) =>
        statusCodesSubmitted.includes(
          res.assessmentStatusInfo.assessmentStatusCode
        )
      );

      learningNeedResults = learningNeedResults.sort((result1, result2) => {
        const date1 = +new Date(result1.surveyInfo.startDate);
        const date2 = +new Date(result2.surveyInfo.startDate);

        return date2 - date1;
      });

      return learningNeedResults;
    }

    return [];
  }

  async getLearningNeedConfigAsync(resultId: number): Promise<any> {
    const params = new IdpConfigParams({ resultId });
    const response = await this.idpService.getLearningNeedConfig(params);

    if (response.error) {
      return;
    }

    if (response.data) {
      return this.processLNAForm(response.data.configuration);
    }

    return null;
  }

  async getLearningNeedsReportDataAsync(
    needResultId: number
  ): Promise<ReportData[]> {
    const params = new IdpConfigParams({
      resultId: needResultId,
    });

    const response = await this.idpService.getLearningNeedReport(params);

    if (response.error) {
      return;
    }

    return response.data;
  }

  async saveNeedResultAsync(
    learningNeed: IdpDto,
    answerData?: any,
    statusCode?: AssessmentStatusInfo
  ): Promise<IdpDto> {
    const learningNeedResult = ObjectUtilities.clone(learningNeed) as IdpDto;
    if (answerData) {
      learningNeedResult.answer = answerData;
    }

    if (answerData) {
      learningNeedResult.assessmentStatusInfo = statusCode;
    }

    const response = await this.idpService.saveNeedsResult(learningNeedResult);

    if (response.error) {
      return;
    }

    return response.data;
  }

  async saveIncompleteNeedResultAsync(
    learningNeed: IdpDto,
    answerData: any
  ): Promise<IdpDto> {
    const statusCode = ResultHelper.getStatusCode(learningNeed);

    let newStatusInfo: AssessmentStatusInfo;
    if (statusCode && statusCode !== IdpStatusCodeEnum.Started) {
      newStatusInfo = new AssessmentStatusInfo({
        assessmentStatusId: IdpStatusEnum.Started,
        assessmentStatusCode: IdpStatusCodeEnum.Started,
      });
    }

    return await this.saveNeedResultAsync(
      learningNeed,
      answerData,
      newStatusInfo
    );
  }

  async submitLearningNeedResultAsync(
    learningNeed: IdpDto,
    resultAnswer: any
  ): Promise<IdpDto> {
    const newStatusInfo = new AssessmentStatusInfo({
      assessmentStatusId: IdpStatusEnum.PendingForApproval,
      assessmentStatusCode: IdpStatusCodeEnum.PendingForApproval,
    });

    return await this.saveNeedResultAsync(
      learningNeed,
      resultAnswer,
      newStatusInfo
    );
  }

  async getChartFromLNA(resultId: number): Promise<LearningAreaChartModel[]> {
    const params = new IdpConfigParams({ resultId });
    const response = await this.idpService.getLearningNeedReport(params);

    if (response.error) {
      // Handle error here
      return [];
    }

    if (isEmpty(response.data)) {
      // Handle data empty here
      return [];
    }

    const chartModels: LearningAreaChartModel[] = [];
    const reportDatas = response.data;

    reportDatas.forEach((report) => {
      const prioritisationChart: ChartInfo = LearningAreaChartHelper.reportDataToChartInfo(
        report
      );
      const chartModel = new LearningAreaChartModel({
        prioritisationChart,
      });
      chartModels.push(chartModel);
    });

    return chartModels;
  }

  getClonedLearningNeedAnswer(lnaResult: IdpDto): any {
    return lnaResult.answer ? ObjectUtilities.clone(lnaResult.answer) : {};
  }

  checkResultSubmitted(lnaResult: IdpDto): boolean {
    return ResultHelper.checkStatus(
      lnaResult,
      IdpStatusCodeEnum.PendingForApproval
    );
  }
  checkResultCompleted(lnaResult: IdpDto): boolean {
    const statusCode = ResultHelper.getStatusCode(lnaResult);
    const resultIndex = LearningNeedHelpers.LEARNING_NEED_SUBMITTED_STATUS_CODE.findIndex(
      (code) => code.toString() === statusCode
    );

    return resultIndex > -1;
  }

  checkValidToSubmit(
    surveyEvent: CxSurveyjsEventModel,
    lnaResult: IdpDto
  ): boolean {
    if (
      !lnaResult ||
      !surveyEvent ||
      !surveyEvent.options ||
      !surveyEvent.options.allowComplete ||
      !surveyEvent.survey ||
      !surveyEvent.survey.data
    ) {
      return false;
    }

    return true;
  }

  surveyFormVariableBuilder(): CxSurveyjsVariable[] {
    const formDisplayModeValue = {
      name: SurveyVariableEnum.formDisplayMode,
      value: 'edit',
    };
    const formDisplayMode = new CxSurveyjsVariable(formDisplayModeValue);

    return [formDisplayMode];
  }

  setSurveyJsObjectVariables(result: IdpDto, user: Staff): void {
    // Pass the user which the object belonging to so that it would be set into the default variables.
    const cloneResult = ObjectUtilities.clone<any>(result);
    // tslint:disable-next-line: no-unsafe-any
    cloneResult.currentObjectUser = user;
    this.cxSurveyjsExtendedService.setCurrentObjectVariables(result);
  }

  private processLNAForm(surveyJSON: any): any {
    if (!surveyJSON) {
      return;
    }
    surveyJSON.showNavigationButtons = 'none';

    return surveyJSON;
  }
}
