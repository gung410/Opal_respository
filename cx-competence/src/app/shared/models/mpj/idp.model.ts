import { AssessmentStatusInfo } from 'app-models/assessment.model';
import { Identity } from 'app-models/common.model';
import { PdPlanType } from 'app-models/pdplan.model';
import { IdpDto } from 'app/organisational-development/models/idp.model';

export class LnaResultModel {
  answer: any;
  assessmentRoleId: number;
  assessmentStatusInfo: AssessmentStatusInfo;
  created: string;
  createdBy: any;
  dueDate: string;
  identity: { archetype: string; id: number; extId: string };
  lastUpdated: string;
  lastUpdatedBy: any;
  objectiveInfo: any;
  parentResultExtId: string;
  pdPlanActivity: string;
  pdPlanType: PdPlanType;
  resultIdentity: Identity;
  surveyInfo: {
    description?: string;
    displayName?: string;
    endDate?: string;
    finishText?: string;
    info?: string;
    name?: string;
    startDate?: string;
    identity?: Identity;
    periodInfo?: {
      description?: string;
      endDate?: string;
      name?: string;
      no?: number;
      startDate?: string;
      identity?: Identity;
    };
  };
  timestamp: string;
}

export class ActionItemResultResponeDTO {
  identity: Identity;
  status: number;
  message: string;
}

export class ChangeMassPdPlanStatusType {
  resultIdentities: Identity[];
  targetStatusType: AssessmentStatusInfo;
  constructor(data?: Partial<ChangeMassPdPlanStatusType>) {
    if (!data) {
      return;
    }
    this.resultIdentities = data.resultIdentities
      ? data.resultIdentities
      : undefined;
    this.targetStatusType = data.targetStatusType
      ? data.targetStatusType
      : undefined;
  }
}

export class ChangePdPlanStatusTypeResult {
  sourceStatusType: AssessmentStatusInfo;
  targetStatusType: AssessmentStatusInfo;
  identity: Identity;
  status: number;
  message: string;
  constructor(data?: Partial<ChangePdPlanStatusTypeResult>) {
    if (!data) {
      return;
    }
    this.sourceStatusType = data.sourceStatusType
      ? data.sourceStatusType
      : undefined;
    this.targetStatusType = data.targetStatusType
      ? data.targetStatusType
      : undefined;
    this.identity = data.identity ? data.identity : undefined;
    this.status = data.status ? data.status : undefined;
    this.message = data.message ? data.message : undefined;
  }
}

export class LearningNeedsDataModel {
  public learningNeedUnsubmitted: IdpDto;
  public learningNeedSubmitted: IdpDto[];
}

export enum LearningNeedsCompletionRate {
  Started = 0,
  CareerAspiration = 20,
  Competencies = 60,
  PrioritiesOfLearningAreas = 100,
}
