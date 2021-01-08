import {
  AssessmentStatusInfo,
  ObjectiveInfo,
  ResultIdentity,
} from 'app-models/assessment.model';
import { Identity } from 'app-models/common.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { ODPFilterParams } from './odp.models';

export class AssignContentsDTO {
  identities: Identity[];
  answer?: any;
  dueDate?: Date;
  forceAssign?: boolean = false;
  updateIfExists?: boolean = false;
  ignoreUpdatingAssessmentStatus?: boolean = false;
  timestamp?: Date;
  additionalProperties?: any = {};
}

export class ActionItemResult {
  resultIdentity: ResultIdentity;
  objectiveInfo: ObjectiveInfo;
}

export class DeactivatePDPlanDto {
  identities: ResultIdentity[];
  deactivateAllVersion: boolean;
}

export class IdpDto extends PDPlanDto {
  skipCloningResult?: boolean; // This variable use to skip clone result when update result, please careful when use
}

export class IdpFilterParams extends ODPFilterParams {}

export class AssignLnAssessmentResultModel {
  identity: {
    ownerId: number;
    customerId: number;
    archetype: string;
    id: number;
    extId: string;
  };
  statusCode: number;
  message: string;
  assessmentStatusInfo: AssessmentStatusInfo;
}
