import { AssessmentStatusInfo } from 'app-models/assessment.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { OdpStatusCode } from '../learning-plan-detail/odp.constant';

export class ODPDto extends PDPlanDto {}

export class ODPFilterParams {
  resultIds?: number[];
  userIds?: number[];
  departmentIds?: number[];
  lastUpdatedBefore?: Date;
  lastUpdatedAfter?: Date;
  startDateBefore?: Date;
  startDateAfter?: Date;
  surveyStartBefore?: string;
  surveyStartAfter?: string;
  surveyEndBefore?: string;
  surveyEndAfter?: string;
  validFromBefore?: Date;
  validFromAfter?: Date;
  validToBefore?: Date;
  validToAfter?: Date;
  resultExtIds?: string[];
  parentResultExtIds?: string[];
  additionalProperties?: any;
  statusTypeIds?: number[];
  statusTypeCodes?: string[];
  excludeAnswer?: boolean;
  includeChildren?: boolean;
  latestStatusTypeCodes?: OdpStatusCode;

  pdplanActivities?: string[];

  constructor(data?: Partial<ODPFilterParams>) {
    if (!data) {
      return;
    }
    this.resultIds = data.resultIds ? data.resultIds : undefined;
    this.userIds = data.userIds ? data.userIds : undefined;
    this.departmentIds = data.departmentIds ? data.departmentIds : undefined;
    this.lastUpdatedBefore = data.lastUpdatedBefore
      ? data.lastUpdatedBefore
      : undefined;
    this.lastUpdatedAfter = data.lastUpdatedAfter
      ? data.lastUpdatedAfter
      : undefined;
    this.startDateBefore = data.startDateBefore
      ? data.startDateBefore
      : undefined;
    this.startDateAfter = data.startDateAfter ? data.startDateAfter : undefined;
    this.surveyStartBefore = data.surveyStartBefore
      ? data.surveyStartBefore
      : undefined;
    this.surveyStartAfter = data.surveyStartAfter
      ? data.surveyStartAfter
      : undefined;
    this.surveyEndBefore = data.surveyEndBefore
      ? data.surveyEndBefore
      : undefined;
    this.surveyEndAfter = data.surveyEndAfter ? data.surveyEndAfter : undefined;
    this.validFromBefore = data.validFromBefore
      ? data.validFromBefore
      : undefined;
    this.validFromAfter = data.validFromAfter ? data.validFromAfter : undefined;
    this.validToBefore = data.validToBefore ? data.validToBefore : undefined;
    this.validToAfter = data.validToAfter ? data.validToAfter : undefined;
    this.resultExtIds = data.resultExtIds ? data.resultExtIds : undefined;
    this.parentResultExtIds = data.parentResultExtIds
      ? data.parentResultExtIds
      : undefined;
    this.additionalProperties = data.additionalProperties
      ? data.additionalProperties
      : undefined;
    this.statusTypeIds = data.statusTypeIds ? data.statusTypeIds : undefined;
    this.statusTypeCodes = data.statusTypeCodes
      ? data.statusTypeCodes
      : undefined;
    this.excludeAnswer =
      data.excludeAnswer !== null ? data.excludeAnswer : undefined;
    this.includeChildren =
      data.includeChildren !== null ? data.includeChildren : undefined;
    this.latestStatusTypeCodes = data.latestStatusTypeCodes
      ? data.latestStatusTypeCodes
      : undefined;
    this.pdplanActivities = data.pdplanActivities
      ? data.pdplanActivities
      : undefined;
  }
}

export class ODPConfigFilterParams {
  ResultId?: number;
}

export class LDDto extends ODPDto {
  children: ODPDto[];
}

export class ChangePDPlanStatusDto {
  targetStatusType: AssessmentStatusInfo;
  autoMapTargetStatusType: boolean;
  constructor(object: any) {
    if (!object) {
      return;
    }
    this.targetStatusType = object.targetStatusType;
    this.autoMapTargetStatusType = object.autoMapTargetStatusType;
  }
}
