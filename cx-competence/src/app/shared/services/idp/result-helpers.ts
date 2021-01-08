import { IdpStatusCodeEnum } from 'app/individual-development/idp.constant';
import { IdpDto } from 'app/organisational-development/models/idp.model';

export class ResultHelper {
  static checkStatus(result: IdpDto, statusCode: IdpStatusCodeEnum): boolean {
    const resultSatusCode = this.getStatusCode(result);

    return statusCode === resultSatusCode;
  }

  static getStatusCode(result: IdpDto): string {
    if (
      result &&
      result.assessmentStatusInfo &&
      result.assessmentStatusInfo.assessmentStatusCode
    ) {
      return result.assessmentStatusInfo.assessmentStatusCode;
    }

    return;
  }

  static getStatusName(result: IdpDto): string {
    if (
      result &&
      result.assessmentStatusInfo &&
      result.assessmentStatusInfo.assessmentStatusName
    ) {
      return result.assessmentStatusInfo.assessmentStatusName;
    }

    return;
  }

  static checkIsCompletedResult(result: IdpDto): boolean {
    const STATUSES_COMPLETE = [
      IdpStatusCodeEnum.Approved,
      IdpStatusCodeEnum.Completed,
    ];
    const resultStatusCode = ResultHelper.getStatusCode(result);

    return STATUSES_COMPLETE.includes(resultStatusCode as IdpStatusCodeEnum);
  }

  static hasValidResultIdentity(result: IdpDto): boolean {
    if (result && result.resultIdentity && result.resultIdentity.id) {
      return true;
    }

    return false;
  }

  static getResultId(result: IdpDto): number {
    if (!result || !result.resultIdentity) {
      return;
    }

    return result.resultIdentity.id;
  }

  static getResultExtId(result: IdpDto): string {
    if (!result || !result.resultIdentity) {
      return;
    }

    return result.resultIdentity.extId;
  }

  static getObjectiveId(result: IdpDto): number {
    if (!result || !result.objectiveInfo || !result.objectiveInfo.identity) {
      return;
    }

    return result.objectiveInfo.identity.id;
  }

  static checkIsCurrentYearResult(result: IdpDto): boolean {
    if (!result || !result.surveyInfo) {
      return false;
    }

    const currentYear = new Date().getFullYear();
    const surveyYear = +result.surveyInfo.name;

    return surveyYear > 0 && surveyYear === currentYear;
  }
}
