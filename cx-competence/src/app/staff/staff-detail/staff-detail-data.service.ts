import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { LnaResultModel } from 'app-models/mpj/idp.model';
import { Constant, AppConstant } from 'app/shared/app.constant';

class GetLnaResultsQueryModel {
  resultIds: number[];
  userIds: number[];
  excludeAnswer: boolean;
}
@Injectable()
export class StaffDetailDataService {
  constructor(private http: HttpHelpers) {}

  public getLnaResultWithAnswer(resultId: number) {
    let getLnaResultQueryModel = new GetLnaResultsQueryModel();
    getLnaResultQueryModel = {
      ...getLnaResultQueryModel,
      resultIds: [resultId],
      excludeAnswer: false,
    };
    return this.http
      .get<LnaResultModel[]>(
        `${AppConstant.api.competence}/idp/needs/results`,
        getLnaResultQueryModel,
        { avoidIntercepterCatchError: true }
      )
      .pipe()
      .map((res) => res[0]);
  }

  public getNoAnswerLnaResults(userId: number) {
    let getLnaResultQueryModel = new GetLnaResultsQueryModel();
    getLnaResultQueryModel = {
      ...getLnaResultQueryModel,
      userIds: [userId],
      excludeAnswer: true,
    };
    return this.http.get<LnaResultModel[]>(
      `${AppConstant.api.competence}/idp/needs/results`,
      getLnaResultQueryModel,
      { avoidIntercepterCatchError: true }
    );
  }

  public getListActionItem(userId: number, period: number) {
    const middleYear = new Date(
      Date.UTC(period, Constant.MIDDLE_MONTH_OF_YEAR_VALUE)
    ); // example: 1/6/2019
    const params = {
      userIds: userId,
      excludeAnswer: true,
      includeChildren: false,
      surveyStartBefore: middleYear.toISOString(),
      surveyEndAfter: middleYear.toISOString(),
    };
    return this.http.get(
      `${AppConstant.api.competence}/idp/actionitems/results`,
      params
    );
  }

  public getLearningOpportunities = (uris: string[]) => {
    const body = {
      courseIds: uris,
      pageSize: uris.length,
    };
    return this.http.post(
      `${AppConstant.api.competence}/learningopportunities`,
      body
    );
  };
}
