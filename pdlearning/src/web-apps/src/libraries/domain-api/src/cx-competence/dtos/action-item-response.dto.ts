import { IResultIdentity } from './deactivate-action-item.dto';

interface IObjectiveInfo {
  email: string;
  identity: IResultIdentity;
  name: string;
}

interface IAnswer {
  learningOpportunity: {
    uri: string;
    source: string;
  };
  addedToIDPDate: string;
}
interface IAssessmentStatusInfo {
  assessmentStatusId: number;
  assessmentStatusCode: string;
  assessmentStatusName: string;
  assessmentStatusDescription: string;
  no: number;
}
interface ICreatedBy {
  email: string;
  identity: IResultIdentity;
  name: string;
}
interface ILastUpdatedBy {
  email: string;
  identity: IResultIdentity;
  name: string;
}

interface ISurveyInfo {
  identity: IResultIdentity;
  periodInfo: {
    identity: IResultIdentity;
    startDate: string;
    endDate: string;
    name: string;
    description: string;
    no: number;
  };
}

interface IAdditionalProperties {
  type: string;
  learningOpportunityUri: string;
}

export interface IActionItemResponse {
  resultIdentity: IResultIdentity;
  objectiveInfo: IObjectiveInfo;
  timestamp: string;
  answer: IAnswer;
  assessmentRoleId: number;
  startDate: string;
  assessmentStatusInfo: IAssessmentStatusInfo;
  parentResultExtId: string;
  pdPlanType: string;
  pdPlanActivity: string;
  createdBy: ICreatedBy;
  created: string;
  lastUpdated: string;
  lastUpdatedBy: ILastUpdatedBy;
  surveyInfo: ISurveyInfo;
  forceCreateResult: boolean;
  additionalProperties: IAdditionalProperties;
}

export class ActionItemResponse implements IActionItemResponse {
  public resultIdentity: IResultIdentity;
  public objectiveInfo: IObjectiveInfo;
  public timestamp: string;
  public answer: IAnswer;
  public assessmentRoleId: number;
  public startDate: string;
  public assessmentStatusInfo: IAssessmentStatusInfo;
  public parentResultExtId: string;
  public pdPlanType: string;
  public pdPlanActivity: string;
  public createdBy: ICreatedBy;
  public created: string;
  public lastUpdated: string;
  public lastUpdatedBy: ILastUpdatedBy;
  public surveyInfo: ISurveyInfo;
  public forceCreateResult: boolean;
  public additionalProperties: IAdditionalProperties;
  constructor(data?: IActionItemResponse) {
    if (data == null) {
      return;
    }
    this.resultIdentity = data.resultIdentity;
    this.objectiveInfo = data.objectiveInfo;
    this.timestamp = data.timestamp;
    this.answer = data.answer;
    this.assessmentRoleId = data.assessmentRoleId;
    this.startDate = data.startDate;
    this.assessmentStatusInfo = data.assessmentStatusInfo;
    this.parentResultExtId = data.parentResultExtId;
    this.pdPlanType = data.pdPlanType;
    this.pdPlanActivity = data.pdPlanActivity;
    this.createdBy = data.createdBy;
    this.created = data.created;
    this.lastUpdated = data.lastUpdated;
    this.lastUpdatedBy = data.lastUpdatedBy;
    this.surveyInfo = data.surveyInfo;
    this.forceCreateResult = data.forceCreateResult;
    this.additionalProperties = data.additionalProperties;
  }
}
